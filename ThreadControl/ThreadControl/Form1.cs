using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Threading;
using System.Reactive.Linq;
using System.Reactive.Concurrency;

namespace ThreadControl
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }
        private void button1_Click(object sender, EventArgs e)
        {

        }
        private void before_work()
        {
            this.button1.Enabled = false;
        }
        private void busy_work()
        {
            Thread.Sleep(3000);
        }
        private void after_work()
        {
            this.button1.Enabled = true;
        }
        /// Thread Pool
        void threadPool()
        {
            before_work();
            ThreadPool.QueueUserWorkItem(_ =>
            {
                busy_work();
                BeginInvoke((Action)(() => { after_work(); }));
            }, null);
        }
        /// AsynchronouseProngraingModel
        void apm()
        {
            before_work();
            var method = new Func<double>(() => { busy_work(); return 0; });
            method.BeginInvoke(ar =>
            {
                var result = method.EndInvoke(ar);// get return value
                this.BeginInvoke((Action)(() => { after_work(); }));
            }, null);
        }
        //// Event-based Asynchronous Pattern
        void eas()
        {
            before_work();
            var worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler((sender, eventarg) => { busy_work(); eventarg.Result = (Object)0; });
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler((sender, eventarg) => { after_work(); });
        }
        /// Reactiove Extension
        void rx()
        {
            Observable.FromEventPattern(this.button1, "Click")
                                  .Do(_ => before_work())
                                  .ObserveOn(Scheduler.ThreadPool)
                                  .Do(_ => before_work())
                                  .ObserveOn(SynchronizationContext.Current)
                                  .Subscribe(_ => after_work());
        }
    }
}
