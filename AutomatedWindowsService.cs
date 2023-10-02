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
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private MainThread _mainThread;
        private Timer _timer;
        private readonly ManualResetEvent mainStartupEvent = new ManualResetEvent(false),
            mainShutdownEvent = new ManualResetEvent(false);

        public AutomatedWindowsService()
        {
            InitializeComponent();
        }

        public void BeginService()
        {
            try
            {
                _mainThread = new MainThread(_cts.Token,mainStartupEvent,mainShutdownEvent);
                _mainThread.Main();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        int getCallType;
        string timeString;
        Timer timer1 = null;

        protected override void OnStart(string[] args)
        {
            BeginService();
        }

        protected override void OnStop()
        {
            _cts.Cancel();
            mainShutdownEvent.WaitOne();
            mainShutdownEvent.Dispose();
            mainStartupEvent.Dispose();
            _cts.Dispose();
        }

    }
}
