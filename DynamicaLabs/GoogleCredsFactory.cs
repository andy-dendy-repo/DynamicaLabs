using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicaLabs
{
    public static class GoogleCredsFactory
    {
        public static GoogleCredential GetCreds(string clientSecrets)
        {
            string[] scopes = { SheetsService.Scope.Spreadsheets, DriveService.Scope.DriveReadonly };

            GoogleCredential credential;
            using (var stream = new FileStream(clientSecrets, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(scopes);
            }

            return credential;
        }
    }
}
