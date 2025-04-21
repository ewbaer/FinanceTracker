using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Windows;
using FinanceTracker.ViewModels;

namespace FinanceTracker.Views
{
    public partial class EmailPromptWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public EmailPromptWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
        }

        private void SendEmail_Click(object sender, RoutedEventArgs e)
        {
            string recipient = EmailBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(recipient) || !recipient.Contains("@"))
            {
                MessageBox.Show("Please enter a valid email address.");
                return;
            }

            try
            {
                var csv = new StringBuilder();
                csv.AppendLine("Id,Title,Amount,Category,Date,Time");

                foreach (var t in _viewModel.Transactions)
                {
                    string date = t.Date.ToString("MM/dd/yyyy");
                    string time = t.Time.ToString("hh:mm tt");
                    csv.AppendLine($"{t.Id},{Escape(t.Title)},{t.Amount},{t.Category},{date},{time}");
                }

                string tempFile = Path.GetTempFileName();
                File.WriteAllText(tempFile, csv.ToString());

                var mail = new MailMessage();
                mail.From = new MailAddress("ethantest815@gmail.com"); 
                mail.To.Add(recipient);
                mail.Subject = "Your Transaction History";
                mail.Body = "Attached is your exported transaction history from FinanceTracker.";

                var attachment = new Attachment(tempFile);
                attachment.Name = "Transactions.csv";
                attachment.ContentType = new ContentType("text/csv");
                mail.Attachments.Add(attachment);

                var smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("ethantest815@gmail.com", "tvhzulovwztumqmu"),
                    EnableSsl = true
                };

                smtp.Send(mail);

                MessageBox.Show("Email sent successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to send email:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string Escape(string input)
        {
            if (input.Contains(",") || input.Contains("\""))
                return $"\"{input.Replace("\"", "\"\"")}\"";
            return input;
        }
    }
}
