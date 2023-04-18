using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using static Google.Apis.Sheets.v4.SpreadsheetsResource.ValuesResource;

namespace DynamicaLabs
{
    public class SheetsServiceWrapper
    {
        private readonly SheetsService _sheetsService;
        private const string _sheetId = "1zwXkRKw97qzgyKVSs-61gnbjtE35I5EVE5Kd-fLb33A";

        public SheetsServiceWrapper(GoogleCredential credential, string appName)
        {
            _sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = appName
            });
        }

        public async Task UploadLists(List<DownloadedFileInfo> files)
        {
            foreach (var file in files) 
            {
                await AddTab(file);

                var lines = file.Content.Split('\n').Select(x=>(IList<object>)new List<object> { x }).ToList();

                var valueRange = new ValueRange
                {
                    Values = lines
                };
                var appendRequest = _sheetsService.Spreadsheets.Values.Update(valueRange, _sheetId, file.FileName + "!A:A");

                appendRequest.ValueInputOption = UpdateRequest.ValueInputOptionEnum.USERENTERED;

                await appendRequest.ExecuteAsync();
            }
        }

        private async Task AddTab(DownloadedFileInfo file)
        {
            if (!await TabExists(file.FileName))
            {
                var addSheetRequest = new AddSheetRequest
                {
                    Properties = new SheetProperties
                    {
                        Title = file.FileName
                    }
                };
                var batchUpdateRequest = new BatchUpdateSpreadsheetRequest
                {
                    Requests = new List<Request>
                    {
                        new Request
                        {
                            AddSheet = addSheetRequest
                        }
                    }
                };
                var request = _sheetsService.Spreadsheets.BatchUpdate(batchUpdateRequest, _sheetId);
                await request.ExecuteAsync();
            }
        }

        private async Task<bool> TabExists(string name)
        {
            var sheetRequest = _sheetsService.Spreadsheets.Get(_sheetId);
            var sheetResponse = await sheetRequest.ExecuteAsync();
            var sheet = sheetResponse.Sheets.FirstOrDefault(s => s.Properties.Title == name);
            return sheet != null;
        }
    }
}
