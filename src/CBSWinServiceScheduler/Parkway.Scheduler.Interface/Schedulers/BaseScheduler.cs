using Parkway.Scheduler.Interface.Loggers.Contracts;
using Parkway.Scheduler.Interface.Schedulers.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.Scheduler.Interface.Schedulers
{
    public abstract class BaseScheduler
    {
        private ISchedulerLogger Logger;

        public BaseScheduler(ISchedulerLogger logger)
        {
            Logger = logger;
        }

        /// <summary>
        /// Fall back logger
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        protected void Log(string message, Exception exception = null)
        {
            FileStream fs = new FileStream(@"c:\\ScheduleServiceLog.txt", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.BaseStream.Seek(0, SeekOrigin.End);
            message += exception == null ? "" : exception.Message;
            sw.WriteLine(message);
            sw.Flush();
            sw.Close();
        }
    }
}
