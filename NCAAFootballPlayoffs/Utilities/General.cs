using NCAAFootballPlayoffs.Models;
using NCAAFootballPlayoffs.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace NCAAFootballPlayoffs.Utilities
{
    public class General
    {
        public static int GetActiveSeasonID()
        {
            using (NCAAFootballPlayoffsEntities db = new NCAAFootballPlayoffsEntities())
            {
                return db.Seasons.FirstOrDefault(f => !f.Archived && f.ActiveSeason).SeasonID;
            }
        }

        public static int SendEmail(string to, string messageText, string subject, string from = "Rob Senders", string fromEmail = "senders.rob@gmail.com")
        {
            var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
            var message = new MailMessage();
            message.To.Add(new MailAddress(to));  // replace with valid value 
            message.From = new MailAddress(fromEmail);  // replace with valid value
            message.Subject = subject;
            message.Body = string.Format(body, from, fromEmail, messageText);
            message.IsBodyHtml = true;
            try
            {
                using (var smtp = new SmtpClient())
                {
                    //var credential = new NetworkCredential
                    //{
                    //    UserName = "CFBBowlBracket@gmail.com",  // replace with valid value
                    //    Password = "robsenders"  // replace with valid value
                    //};
                    var credential = new NetworkCredential
                    {
                        UserName = "bowlseasonpickem@gmail.com",  // replace with valid value
                        Password = "robsenders"  // replace with valid value
                    };
                    smtp.Credentials = credential;
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    //await smtp.SendMailAsync(message);
                    smtp.Send(message);
                }
                return 1;
            }
            catch
            {
                return -1;
            }
        }
    }
}