using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace AutomatedWindowsService
{
    public partial class AutomatedWindowsService : ServiceBase
    {
        public AutomatedWindowsService()
        {
            InitializeComponent();
            int strTime = Convert.ToInt32(ConfigurationManager.AppSettings["callDuration"]);
            getCallType = Convert.ToInt32(ConfigurationManager.AppSettings["CallType"]);
            if (getCallType == 1)
            {
                timer1 = new System.Timers.Timer();
                double inter = (double)GetNextInterval();
                timer1.Interval = inter;
                timer1.Elapsed += new ElapsedEventHandler(ServiceTimer_Tick);
            }
            else
            {
                timer1 = new System.Timers.Timer();
                timer1.Interval = strTime * 1000;
                timer1.Elapsed += new ElapsedEventHandler(ServiceTimer_Tick);
            }
        }

        int getCallType;
        string timeString;
        Timer timer1 = null;

        protected override void OnStart(string[] args)
        {
            timer1.AutoReset = true;
            timer1.Enabled = true;
            ServiceLog.WriteErrorLog("Daily Reporting service started");
        }

        protected override void OnStop()
        {
            timer1.AutoReset = false;
            timer1.Enabled = false;
            ServiceLog.WriteErrorLog("Daily Reporting service stopped");
        }

        private double GetNextInterval()
        {
            timeString = ConfigurationManager.AppSettings["StartTime"];
            DateTime t = DateTime.Parse(timeString);
            TimeSpan ts = new TimeSpan();
            int x;
            ts = t - System.DateTime.Now;
            if (ts.TotalMilliseconds < 0)
            {
                ts = t.AddDays(1) - System.DateTime.Now;//Here you can increase the timer interval based on your requirments.   
            }
            return ts.TotalMilliseconds;
        }
        private void SetTimer()
        {
            try
            {
                double inter = (double)GetNextInterval();
                timer1.Interval = inter;
                timer1.Start();
            }
            catch (Exception ex)
            {
            }
        }

        private void ServiceTimer_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            string Msg = "Hi ! This is DailyMailSchedulerService mail.";//whatever msg u want to send write here.  
                                                                        // Here you can write the   
            ServiceLog.SendEmail("sathishkumarr2168@gmail.com", null, null, "Daily Report of DailyMailSchedulerService on " + DateTime.Now.ToString("dd-MMM-yyyy"), Msg);

            if (getCallType == 1)
            {
                timer1.Stop();
                System.Threading.Thread.Sleep(1000000);
                SetTimer();
            }
        }
    }
}
