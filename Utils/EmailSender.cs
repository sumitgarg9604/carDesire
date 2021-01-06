using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Net.Mail;
using System.IO;

namespace Assignment1_v9.Utils
{
    public class EmailSender
    {
        // Please use your API KEY here.
        private const String API_KEY = "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";

        public void Send(String toEmail, String subject, String contents,String name,String path)
        {

            var client = new SendGridClient(API_KEY);
            var from = new EmailAddress("noreply@localhost.com", "CarDesire Queries");
            var to = new EmailAddress("gargsumit9604@gmail.com");
            var plainTextContent = contents;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, "");
            if (path != null)
            { 
                
                var bytes = File.ReadAllBytes(path);
                var file = Convert.ToBase64String(bytes);
                msg.AddAttachment(path, file);
            }
            
            var response = client.SendEmailAsync(msg);


        }


        public void SendBulk(string EmailValues, String subject, String contents, String name, String path)
        {

            
            String[] strlist = EmailValues.Split(',');

            var toEmails = new List<EmailAddress>();
            

            foreach (string address in strlist)
            {
                
                EmailAddress to = new EmailAddress(address);
                toEmails.Add(to);

            }

            var client = new SendGridClient(API_KEY);
            var from = new EmailAddress("noreply@localhost.com", "CarDesire Queries");
            //var to = new EmailAddress("gargsumit9604@gmail.com");
            //List<EmailAddress> BulkEmails = new List<EmailAddress>();
            var plainTextContent = contents;
            // var htmlContent = "<p>" + contents+"</p>";
            //  var filepath = "" + path;
            var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, toEmails, subject, plainTextContent, "");

            if (path != null)
            {
                var bytes = File.ReadAllBytes(path);
                var file = Convert.ToBase64String(bytes);
                msg.AddAttachment(path, file);
            }
            var response = client.SendEmailAsync(msg);

        }


    }
}