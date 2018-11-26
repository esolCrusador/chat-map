using ChatMap.Extensions;
using System.Collections.Generic;

namespace ChatMap.Infrastructure
{
    public class GoogleSheetShemaProvider
    {
        private readonly GoogleApiClient _googleApiService;
        private readonly Dictionary<string, GoogleSheetTableSchemaProvider> _tableSchemaProviders;

        public GoogleSheetShemaProvider(GoogleApiClient googleApiService)
        {
            _googleApiService = googleApiService;
            _tableSchemaProviders = new Dictionary<string, GoogleSheetTableSchemaProvider>();
        }

        public GoogleSheetTableSchemaProvider this[string sheetId]
        {
            get
            {
                return _tableSchemaProviders.GetOrAdd(sheetId, sId => new GoogleSheetTableSchemaProvider(sId, _googleApiService));
            }
        }
    }
}
