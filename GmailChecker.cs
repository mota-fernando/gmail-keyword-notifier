using System;
using System.Collections.Generic;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class GmailChecker
{
    static string[] Scopes = { GmailService.Scope.GmailReadonly };
    static string ApplicationName = "Gmail Keyword Notifier";

    static void Main(string[] args)
    {
        try
        {
            var service = AuthenticateGmail();
            var keywords = new List<string> { "rafael", "mensagem"};//sample: "fraude", "spam", "phishing", "alerta", "urgente" 
            CheckEmails(service, keywords);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    static GmailService AuthenticateGmail()
    {
        UserCredential credential;
        using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
        {
            string credPath = "token.json";
            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                Scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(credPath, true)).Result;
        }

        return new GmailService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });
    }

    static void CheckEmails(GmailService service, List<string> keywords)
    {
        var request = service.Users.Messages.List("me");
        request.Q = "is:unread";
        var response = request.Execute();

        if (response.Messages == null || response.Messages.Count == 0)
        {
            Console.WriteLine("No new emails found.");
            return;
        }

        foreach (var message in response.Messages)
        {
            var email = service.Users.Messages.Get("me", message.Id).Execute();
            var emailBody = DecodeBase64(email.Payload.Body.Data);

            if (keywords.Any(k => emailBody.IndexOf(k, StringComparison.OrdinalIgnoreCase) >= 0))
            {
                ShowNotification("Keyword Detected!", $"Keyword found in email: {email.Snippet}");
            }
        }
    }

    static string DecodeBase64(string encodedData)
    {
        if (string.IsNullOrEmpty(encodedData))
            return "";

        byte[] data = Convert.FromBase64String(encodedData.Replace("-", "+").Replace("_", "/"));
        return Encoding.UTF8.GetString(data);
    }

    static void ShowNotification(string title, string message)
    {
        // Console notification
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"{title}: {message}");
        Console.ResetColor();
    }
}