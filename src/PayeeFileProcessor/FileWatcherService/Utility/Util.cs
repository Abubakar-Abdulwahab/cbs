using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileWatcherService
{
    public class Util
    {
        public String GetConfig(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        private string GetConfig(string key, string defaultValue)
        {
            string res = GetConfig(key);
            return String.IsNullOrEmpty(res) ? defaultValue : res;
        }

        public String WatchedFolder
        {
            get
            {
                return GetConfig("PayeeFilePath");
            }
        }
      
        List<string> _patternsToWatch = null;
        public List<string> PatternsToWatch
        {
            get
            {
                if (_patternsToWatch == null)
                {
                    _patternsToWatch = new List<string>(WatchedFilesPattern.Split(new char[] { ',', ' ', ';' }, StringSplitOptions.RemoveEmptyEntries));
                    if (_patternsToWatch.Contains("*.*"))
                        _patternsToWatch.Clear();
                    for (int i = 0; i < _patternsToWatch.Count; i++)
                    {
                        _patternsToWatch[i] = _patternsToWatch[i].Trim().Replace("*.", ".");
                    }
                }
                return _patternsToWatch;
            }
        }

        public string WatchedFilesPattern
        {
            get
            {
                return GetConfig("WatchedFilesPattern", "");
            }
        }
    }
}
