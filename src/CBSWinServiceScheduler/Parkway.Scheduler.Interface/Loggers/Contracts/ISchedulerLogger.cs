using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.Scheduler.Interface.Loggers.Contracts
{
    public interface ISchedulerLogger
    {
        void Error(string message);
        void Information(string v);
    }
}
