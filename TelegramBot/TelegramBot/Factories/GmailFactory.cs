using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using System.IO;
using System.Threading;

namespace TelegramBot
{
    class GmailFactory
    {
        private static UserCredential _credential;
        private static string ApplicationName = "TelegramBot";

        private static string[] Scopes = { 
            GmailService.Scope.MailGoogleCom
        };

        private static void SetUserCredentialFromCfgFile()
        {
            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                _credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None).Result;
            }
        }
        public static GmailService CreateGmailService()
        {
            SetUserCredentialFromCfgFile();

            return new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = _credential,
                ApplicationName = ApplicationName,
            });
        }
    }
}
