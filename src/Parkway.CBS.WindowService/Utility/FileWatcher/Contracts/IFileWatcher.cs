using Parkway.CBS.WindowService.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.WindowService.Utility.FileWatcher.Contracts
{
    public interface IFileWatcher
    {
        /// <summary>
        /// Get the directory to watch
        /// </summary>
        /// <returns>string</returns>
        string GetDirectoryToWatch();

        /// <summary>
        /// Get directories to watch.
        /// This gets a bunch of directories that have to ben watched
        /// </summary>
        /// <returns>List{string}</returns>
        List<string> GetDirectoriesToWatch();

        /// <summary>
        /// Get the list of paye file processors
        /// </summary>
        /// <returns>List{PayeeFileProcessor}</returns>
        List<FileProcessor> GetFileProcessors();

        /// <summary>
        /// Get the path segment of the full path
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="trimPart"></param>
        /// <returns>string</returns>
        string TrimFilePath(string fullPath, string trimPart);

    }
}
