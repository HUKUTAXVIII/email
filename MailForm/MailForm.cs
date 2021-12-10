using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Net.Pop3;
using System.Threading;

namespace MailForm
{
    public partial class MailForm : Form
    {

        #region password
        static string password = "";
        #endregion

        List<MimeMessage> messages;

        public MailForm()
        {
            InitializeComponent();
            messages = new List<MimeMessage>();
        }

        private void GetAllMail() {
            try
            {
                using (var client = new Pop3Client())
                {
                    password = this.PasswordBox.Text;
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    client.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
                    client.Connect("pop.gmail.com", 995, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(this.MailBox.Text, password);

                    messages.Clear();
                    this.MailBox.Items.Clear();
                    //Fetch Emails
                    for (int i = 0; i < client.Count; i++)
                    {
                        var message = client.GetMessage(i);
                        messages.Add(message);
                        this.MailBox.Items.Add(message.Subject);
                        if (i > 25)
                        {
                            break;
                        }
                    }

                    //Disconnect Connection
                    client.Disconnect(true);

                }
            }
            catch (Exception ex) {
                MessageBox.Show("Can't connect to Email!");
            }
        }
        private void RefreshBtn_Click(object sender, EventArgs e)
        {
            GetAllMail();
        }

        private void MailBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var index = this.MailBox.SelectedItem;


            this.ContentBox.Text = messages.Where((item) => item.Subject == index).ToList().First().TextBody;
        }

        private void SearchBtn_Click(object sender, EventArgs e)
        {
            this.MailBox.Items.Clear();
            foreach (var item in messages.Where((item)=>item.Subject.Contains(this.SearchBox.Text)).ToList())
            {
                this.MailBox.Items.Add(item.Subject);
            }
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            this.MailBox.Items.Clear();
            foreach (var item in messages.Where((item) => item.Subject.Contains(this.SearchBox.Text)).ToList())
            {
                this.MailBox.Items.Add(item.Subject);
            }
        }

        private void SortBtn_Click(object sender, EventArgs e)
        {
            this.MailBox.Sorted = !this.MailBox.Sorted;
            this.MailBox.Items.Clear();
            
            foreach (var item in messages)
            {
                this.MailBox.Items.Add(item);
            }
        }
    }
}
