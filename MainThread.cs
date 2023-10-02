using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace AutomatedWindowsService
{
    internal sealed partial class MainThread
    {
        private readonly CancellationToken _cancellationToken;
        private readonly ManualResetEvent _startupEvent, _shutdownEvent;
        private System.Timers.Timer aTimer;

        public MainThread(CancellationToken cancellationToken, ManualResetEvent startupEvent, ManualResetEvent shutdownEvent)
        {
            _cancellationToken = cancellationToken;
            _startupEvent = startupEvent;
            _shutdownEvent = shutdownEvent;
        }

        private void MainQueueActivation()
        {
            //SendEmail("sathishkumarr2168@gmail.com", null, null, "Daily Report of DailyMailSchedulerService on " + DateTime.Now.ToString("dd-MMM-yyyy"), "");
            aTimer = new System.Timers.Timer();
            aTimer.Interval = 1100;
            aTimer.Elapsed += new System.Timers.ElapsedEventHandler(ServiceTimer_Tick);
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private void ServiceTimer_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                aTimer.Enabled = false;
                aTimer.AutoReset=false;
                aTimer.Stop();
                ServiceLog.SendEmail("sathishkumarr2168@gmail.com", null, null, "Daily Report of DailyMailSchedulerService on " + DateTime.Now.ToString("dd-MMM-yyyy"), "");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public void Main()
        {
            try
            {
                _startupEvent.Set();
                MainQueueActivation();
                _shutdownEvent.Set();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SendEmail(String ToEmail, string cc, string bcc, String Subj, string Message)
        {
            //Reading sender Email credential from web.config file  

            string HostAdd = ConfigurationManager.AppSettings["Host"].ToString();
            string FromEmailid = ConfigurationManager.AppSettings["FromMail"].ToString();
            string Pass = ConfigurationManager.AppSettings["Password"].ToString();

            //creating the object of MailMessage  
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(FromEmailid); //From Email Id  
            mailMessage.Subject = Subj; //Subject of Email  
            mailMessage.Body = Message; //body or message of Email  
            mailMessage.IsBodyHtml = true;

            string[] ToMuliId = ToEmail.Split(',');
            foreach (string ToEMailId in ToMuliId)
            {
                mailMessage.To.Add(new MailAddress(ToEMailId)); //adding multiple TO Email Id  
            }

            if (!string.IsNullOrEmpty(cc))
            {
                string[] CCId = cc.Split(',');

                foreach (string CCEmail in CCId)
                {
                    mailMessage.CC.Add(new MailAddress(CCEmail)); //Adding Multiple CC email Id  
                }
            }

            if (!string.IsNullOrEmpty(bcc))
            {
                string[] bccid = bcc.Split(',');

                foreach (string bccEmailId in bccid)
                {
                    mailMessage.Bcc.Add(new MailAddress(bccEmailId)); //Adding Multiple BCC email Id  
                }
            }
            
            SmtpClient smtp = new SmtpClient(HostAdd);  // creating object of smptpclient  
            //smtp.Host = HostAdd;              //host of emailaddress for example smtp.gmail.com etc  

            //network and security related credentials  
            smtp.UseDefaultCredentials = false;
            NetworkCredential NetworkCred = new NetworkCredential();
            NetworkCred.UserName = mailMessage.From.Address;
            NetworkCred.Password = Pass;
            smtp.Credentials = NetworkCred;
            smtp.EnableSsl = true;
            smtp.Port = 587;
            smtp.Send(mailMessage); //sending Email  
        }

    }
}
