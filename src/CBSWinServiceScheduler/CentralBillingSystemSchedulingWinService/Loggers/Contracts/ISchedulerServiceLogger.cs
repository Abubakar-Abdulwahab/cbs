using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralBillingSystemSchedulingWinService.Loggers.Contracts
{
    public interface ISchedulerServiceLogger
    {
        void S();
        void Error(string v);
    }
}
