using System;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Net.Pop3;
using System.Threading;

namespace NetConsoleApp
{




    class Program
    {

        #region password
        static string password = "User20Vektor03Nik";
        #endregion

        static void Main(string[] args)
        {
            //SendEmailAsync("nikita.dmitrovich@gmail.com","test","test").GetAwaiter().GetResult();
            GetMail();
            Console.Read();
        }


        public static void GetMail()
        {
            using (var client = new Pop3Client())
            {

                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
                client.Connect("pop.gmail.com", 995, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate("nikita.dmitrovich@gmail.com", password);

                //Fetch Emails
                for (int i = 0; i < client.Count; i++)
                {
                    var message = client.GetMessage(i);
                    Console.WriteLine("Subject: {0}", message.Subject);
                }

                //Disconnect Connection
                client.Disconnect(true);

            }
        }

        public static async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Администрация сайта", "nikita.dmitrovich@gmail.com"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, false);
                await client.AuthenticateAsync("nikita.dmitrovich@gmail.com", password);
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }

    }
}