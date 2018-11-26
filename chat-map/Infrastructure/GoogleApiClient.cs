using ChatMap.Infrastructure.Google.Sheets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ChatMap.Infrastructure
{
    public class GoogleApiClient
    {
        private readonly IDownloadClientsPool _downloadClientsPool;
        private readonly ConfigurationService _configurationService;

        public const string Unchanged = "00--Unchanged--00";

        public GoogleApiClient(IDownloadClientsPool downloadClientsPool, ConfigurationService configurationService)
        {
            _downloadClientsPool = downloadClientsPool;
            _configurationService = configurationService;
        }

        public async Task<SheetInfoResponseDto> GetSheetInfo(string sheetId, CancellationToken cancellationToken)
        {
            using (Models.HttpClientContainer downloader = await _downloadClientsPool.Consume(cancellationToken))
            {
                string response = await downloader.GetAsString($"https://content-sheets.googleapis.com/v4/spreadsheets/{sheetId}?key={_configurationService.GoogleApiKey}", cancellationToken);

                return JsonConvert.DeserializeObject<SheetInfoResponseDto>(response);
            }
        }

        public async Task<SheetValuesDto> GetValues(string sheetId, string sheetName, int cols, int rows, CancellationToken cancellationToken)
        {
            using (Models.HttpClientContainer downloader = await _downloadClientsPool.Consume(cancellationToken))
            {

                string response = await downloader.GetAsString($"https://sheets.googleapis.com/v4/spreadsheets/{sheetId}/values/{GetRange(sheetName, 1, cols, 1, rows)}?key={_configurationService.GoogleApiKey}", cancellationToken);

                return JsonConvert.DeserializeObject<SheetValuesDto>(response);
            }
        }

        private string GetRange(string tableName, int startCol, int cols, int startRow, int rows)
        {
            char startLetter = (char)('A' + startCol - 1);
            char lastLetter = (char)(startLetter + cols - 1);
            int lastRow = startRow + rows - 1;

            return $"'{tableName}'!{startLetter}{startRow}:{lastLetter}{lastRow}";
        }

        public async Task<IReadOnlyCollection<KeyValuePair<string, IReadOnlyCollection<DataColumn>>>> GetSheetTablesShema(string sheetId, CancellationToken cancellationToken)
        {
            SheetInfoResponseDto sheetsInfo = await GetSheetInfo(sheetId, cancellationToken);

            IEnumerable<Task<KeyValuePair<string, List<List<string>>>>> sheetTasks = sheetsInfo.Sheets.Select(async sheet =>
            {
                SheetValuesDto response = await GetValues(sheetId, sheet.Properties.Title, sheet.Properties.GridProperties.ColumnCount, 1, cancellationToken);

                return new KeyValuePair<string, List<List<string>>>(sheet.Properties.Title, response.Values);
            });

            KeyValuePair<string, List<List<string>>>[] sheets = await Task.WhenAll(sheetTasks);

            return sheets.Select(s =>
                new KeyValuePair<string, IReadOnlyCollection<DataColumn>>(
                    s.Key, s.Value.Single().Select(col => new DataColumn(col, typeof(string))).ToList()
                    )
            ).ToList();
        }


        public async Task<IReadOnlyCollection<DataTable>> GetSheetTables(string sheetId, CancellationToken cancellationToken)
        {
            SheetInfoResponseDto sheetsInfo = await GetSheetInfo(sheetId, cancellationToken);

            IEnumerable<Task<KeyValuePair<string, List<List<string>>>>> sheetTasks = sheetsInfo.Sheets.Select(async sheet =>
            {
                SheetValuesDto response = await GetValues(sheetId, sheet.Properties.Title, sheet.Properties.GridProperties.ColumnCount, sheet.Properties.GridProperties.RowCount, cancellationToken);

                return new KeyValuePair<string, List<List<string>>>(sheet.Properties.Title, response.Values);
            });

            KeyValuePair<string, List<List<string>>>[] sheets = await Task.WhenAll(sheetTasks);

            List<DataTable> tables = new List<DataTable>(sheets.Length);

            foreach (KeyValuePair<string, List<List<string>>> kvp in sheets)
            {
                string sheetName = kvp.Key;
                DataTable table = new DataTable(sheetName);
                List<List<string>> sheet = kvp.Value;
                int columnsCount = sheet[0].Count;

                table.Columns.AddRange(sheet[0].Select(name => new DataColumn(name, typeof(string))).ToArray());
                table.Columns.Add("ExternalId", typeof(int));

                for (int i = 1; i < sheet.Count; i++)
                {
                    List<string> sheetRow = sheet[i];
                    DataRow row = table.NewRow();

                    table.Rows.Add(row);

                    if (sheetRow.Count > table.Columns.Count)
                    {
                        throw new IndexOutOfRangeException();
                    }

                    row.ItemArray = sheetRow.Cast<object>()
                        .Concat(Enumerable.Repeat((string)null, columnsCount - sheetRow.Count))
                        .Concat(Enumerable.Repeat((object)i, 1))
                        .ToArray();
                }

                tables.Add(table);
            }

            return tables;
        }

        private class RangesList: List<SheetRange>
        {
            private readonly int _maxWidth;
            public RangesList(int maxWidth)
            {
                Add(SheetRange.None);
                _maxWidth = maxWidth;
            }

            private bool TryMergeLastLines()
            {
                if(Count > 2)
                {
                    var previos = this[Count - 2];
                    var current = this[Count - 1];
                    if(current.LengthX == _maxWidth && previos.LengthX == _maxWidth)
                    {
                        this[Count - 2] = new SheetRange(previos.StartX, previos.StartY, current.EndX, current.EndY);
                        RemoveAt(Count - 1);

                        return true;
                    }
                }

                return false;
            }

            public void Include(int x, int y)
            {
                var current = Current;

                if(current == SheetRange.None)
                {
                    Current = new SheetRange(x, y, x, y);
                    return;
                }

                if(current.EndY != y)
                {
                    TryMergeLastLines();
                    Add(new SheetRange(x, y, x, y));
                    return;
                }

                Current = new SheetRange(current.StartX, current.StartY, x, y);
            }

            public void Exclude(int x, int y)
            {
                var current = Current;
                if(current != SheetRange.None)
                {
                    Add(SheetRange.None);
                }
            }

            public void End()
            {
                TryMergeLastLines();
                if(Current == SheetRange.None)
                {
                    RemoveAt(Count - 1);
                }
            }

            public SheetRange Current
            {
                get
                {
                    return this[Count - 1];
                }
                set
                {
                    this[Count - 1] = value;
                }
            }
        }

        public async Task UpdateSheetTable(string sheetId, string tableName, IReadOnlyDictionary<int, string[]> changes, CancellationToken cancellationToken)
        {
            RangesList ranges = new RangesList(changes.Count);

            foreach (KeyValuePair<int, string[]> kvp in changes)
            {
                int y = kvp.Key;
                string[] rowChanges = kvp.Value;
                for (int x = 0; x < rowChanges.Length; x++)
                {
                    if (rowChanges[x] == Unchanged)
                    {
                        ranges.Exclude(x, y);
                    }
                    else
                    {
                        ranges.Include(x, y);
                    }
                }
            }

            ranges.End();

            var request = new SheetBatchUpdateRequestDto
            {
                Values = ranges.Select(r => new SheetValuesDto
                {
                    Range = GetRange(tableName, r.StartX + 1, r.LengthX, r.StartY + 1 /* Header + index*/, r.LengthY),
                    Values = Enumerable.Range(r.StartY, r.LengthY).Select(y => Enumerable.Range(r.StartX, r.LengthX).Select(x => changes[y][x]).ToList()).ToList()
                }).ToList()
            };

            using (Models.HttpClientContainer downloader = await _downloadClientsPool.Consume(cancellationToken))
            {
                string response = await downloader.RequestAsString(HttpMethod.Post, $"https://sheets.googleapis.com/v4/spreadsheets/{sheetId}/values:batchUpdate?key={"64ad38afc39bd168fddc664c27270c0ccfe01773"}", JsonConvert.SerializeObject(request), cancellationToken);
            }
        }
    }
}
