using System;
using System.IO;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.Schedulers
{
    public interface SimpleInterface : IJob
    {

    }

    public class SimpleJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                LogService("Hello from job " + DateTime.Now.ToLocalTime());
            });
        }

        private void LogService(string content)
        {
            FileStream fs = new FileStream(@"c:\\TestServiceLog.txt", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.BaseStream.Seek(0, SeekOrigin.End);
            sw.WriteLine(content);
            sw.Flush();
            sw.Close();
        }
    }
}