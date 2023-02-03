using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.DataExporter.Implementations.Exceptions
{
    public class Guard
    {
        public Guard()
        {

        }
        public static void Against(Boolean condition, string errorMsg, bool throws = true)
        {
            if (condition)
            {
                var ex = new Exception(errorMsg);
                if (throws)
                {
                    throw ex;
                }
            }
        }
        public static void Against<T>(Boolean condition, string errorMsg, bool throws = true) where T : Exception, new()
        {
            if (condition)
            {
                var ex = (T)Activator.CreateInstance(typeof(T), errorMsg);
                if (throws)
                {
                    throw ex;
                }
            }
        }
    }
}
