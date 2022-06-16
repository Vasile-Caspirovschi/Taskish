using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;

namespace Taskish.Models
{
    public class SendEmail
    {
        public static string PopulateBody(string userName, string currentMonth, string createdTasks, string completedTasks, string pendingTasks, string busiestDay, string freestDay)
        {
            string emailTemplatePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Templates\EmailTemplate.html";
            
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(emailTemplatePath))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{UserName}", userName);
            body = body.Replace("{Month}", currentMonth);
            body = body.Replace("{CreatedTasks}", createdTasks);
            body = body.Replace("{CompletedTasks}", completedTasks);
            body = body.Replace("{PendingTasks}", pendingTasks);
            body = body.Replace("{BusiestDay}", busiestDay);
            body = body.Replace("{FreestDay}", freestDay);
            return body;
        }
        public static string PopulateBody(string userName, string resetToken)
        {
            string emailTemplatePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Templates\ResetPasswordEmailTemplate.html";

            string body = string.Empty;
            using (StreamReader reader = new StreamReader(emailTemplatePath))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{UserName}", userName);
            body = body.Replace("{RestoreCode}", resetToken);
            return body;
        }

        private static async System.Threading.Tasks.Task<UserCredential> GetCredentials(string[] Scopes)
        {
            var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            new ClientSecrets
            {
                ClientId = "339345448027-qiast8ag2rip6tvlbs684rpribmaea5t.apps.googleusercontent.com",
                ClientSecret = "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"
            }, Scopes, "user", CancellationToken.None);
            return credential;
        }

        public static async void SendMessageToEmail(string userEmail, string subject, string body)
        {
            try
            {
                string[] Scopes = { GmailService.Scope.GmailSend };
                var credential = await GetCredentials(Scopes);

                var service = new GmailService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Taskish",
                });
                string message = $"To: {userEmail}\r\nSubject: {subject}\r\nContent-Type: text/html;charset=utf-8\r\n\r\n{body}";
                var newMsg = new Message();
                newMsg.Raw = Base64UrlEncode(message.ToString());
                Message response = service.Users.Messages.Send(newMsg, "me").Execute();
            }
            catch (Exception ep)
            {
                MessageBox.Show($"Exception: {ep.Message}");
            }
        }
        private static string Base64UrlEncode(string input)
        {
            var inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            // Special "url-safe" base64 encode.
            return Convert.ToBase64String(inputBytes)
              .Replace('+', '-')
              .Replace('/', '_')
              .Replace("=", "");
        }
    }
}
