using ChatMap.Extensions;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChatMap.Infrastructure
{
    public class GoogleSheetTableSchemaProvider
    {
        private readonly string _sheetId;
        private readonly Dictionary<string, Dictionary<string, int>> _columnIndexes;
        private readonly GoogleApiClient _googleApiService;

        public GoogleSheetTableSchemaProvider(string sheetId, GoogleApiClient googleApiService)
        {
            _sheetId = sheetId;
            _googleApiService = googleApiService;
            _columnIndexes = new Dictionary<string, Dictionary<string, int>>();
        }

        public int GetColumnsCount(string tableName)
        {
            IReadOnlyDictionary<string, int> tableColumns;
            if (!TryGetColumnsIndexes(tableName, out tableColumns))
                throw new ArgumentException($"Could not find columns count for table {tableName} and sheet \"{_sheetId}\"");

            return tableColumns.Count;
        }


        public int GetColumnIndex(string tableName, string columnName)
        {
            int columnIndex;

            if (!TryGetColumnIndex(tableName, columnName, out columnIndex))
                throw new ArgumentException($"Could not find column {columnName} for table {tableName} and sheet \"{_sheetId}\"");

            return columnIndex;
        }

        public async Task<int> GetColumnIndexAsync(string tableName, string columnName, CancellationToken cancellationToken)
        {
            int columnIndex;

            if (!TryGetColumnIndex(tableName, columnName, out columnIndex))
            {
                var schemas = await _googleApiService.GetSheetTablesShema(_sheetId, cancellationToken);

                foreach(var schema in schemas)
                {
                    UpdateColumnsSchema(schema.Key, schema.Value);
                }
            }

            if (!TryGetColumnIndex(tableName, columnName, out columnIndex))
                throw new ArgumentException($"Could not find column {columnName} for table {tableName} and sheet \"{_sheetId}\"");

            return columnIndex;
        }

        private bool TryGetColumnsIndexes(string tableName, out IReadOnlyDictionary<string, int> tableColumns)
        {
            Dictionary<string, int> tableColumnsDictionary;
            if (!_columnIndexes.TryGetValue(tableName, out tableColumnsDictionary))
                lock (_columnIndexes)
                    if (!_columnIndexes.TryGetValue(tableName, out tableColumnsDictionary))
                    {
                        tableColumns = null;
                        return false;
                    }

            tableColumns = tableColumnsDictionary;
            return true;
        }

        private bool TryGetColumnIndex(string tableName, string columnName, out int columnIndex)
        {
            IReadOnlyDictionary<string, int> tableColumns;
            if(TryGetColumnsIndexes(tableName, out tableColumns))
            {
                columnIndex = tableColumns[columnName];
                return true;
            }

            columnIndex = -1;
            return false;
        }

        public void UpdateColumnsSchema(IEnumerable<DataTable> tables)
        {
            foreach(var table in tables)
                UpdateColumnsSchema(table);
        }

        public void UpdateColumnsSchema(DataTable table)
        {
            UpdateColumnsSchema(table.TableName, table.Columns.Cast<DataColumn>().Where(c => c.ColumnName != "ExternalId"));
        }

        public void UpdateColumnsSchema(string tableName, IEnumerable<DataColumn> columns)
        {
            Dictionary<string, int> tableColumns = _columnIndexes.GetOrAdd(tableName, tN => new Dictionary<string, int>());

            lock (tableColumns)
            {
                if (tableColumns.Count > 0)
                    tableColumns.Clear();

                int index = 0;
                foreach(var col in columns)
                {
                    tableColumns.Add(col.ColumnName, index++);
                }
            }
        }
    }
}
