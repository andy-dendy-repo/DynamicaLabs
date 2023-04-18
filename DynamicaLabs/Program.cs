using DynamicaLabs;
using Google.Apis.Drive.v3;
using Google.Apis.Sheets.v4;

const string clientSecrets = "C:\\Users\\miran\\Downloads\\DynamicaLabs.json";

const string appName = "DynamicaLabs";

var creds = GoogleCredsFactory.GetCreds(clientSecrets);

var driveService = new DriveServiceWrapper(creds, appName);

var sheetsService = new SheetsServiceWrapper(creds, appName);

var timer = new PeriodicTimer(TimeSpan.FromMinutes(15));

do
{
    var files = await driveService.GetFilesAsync();

    await sheetsService.UploadLists(files);
}
while (await timer.WaitForNextTickAsync());