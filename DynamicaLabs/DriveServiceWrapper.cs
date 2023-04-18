using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicaLabs
{
    public class DriveServiceWrapper
    {
        private readonly DriveService _driverService;

        public DriveServiceWrapper(GoogleCredential credential, string appName)
        {
            _driverService = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = appName
            });
        }

        private async Task<string> GetFolderIdAsync()
        {
            var request = _driverService.Files.List();

            var folderQuery = "mimeType='application/vnd.google-apps.folder' and trashed = false and name='DynamicaLabs'";

            request.Q = folderQuery;

            var response = await request.ExecuteAsync();

            var id = response.Files.First().Id;

            return id;
        }

        private async Task<string> GetFileContent(string id)
        {
            var fileRequest = _driverService.Files.Get(id);
            using var fileStream = new MemoryStream();
            await fileRequest.DownloadAsync(fileStream);

            string result = Encoding.UTF8.GetString(fileStream.ToArray());

            return result;
        }

        public async Task<List<DownloadedFileInfo>> GetFilesAsync()
        {
            var files = new List<DownloadedFileInfo>();

            var folderId = await GetFolderIdAsync();

            var query = $"parents='{folderId}' and trashed=false";
            var request = _driverService.Files.List();
            request.Q = query;

            var result = await request.ExecuteAsync();

            foreach ( var item in result.Files)
            {
                var content = await GetFileContent(item.Id);

                files.Add(new DownloadedFileInfo 
                { 
                    Content = content,
                    FileName = item.Name
                }
                );
            }

            return files;
        }
    }
}
