using Parkway.CBS.WindowService.Configuration;
using Parkway.CBS.WindowService.Utility.FileWatcher.Contracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Parkway.CBS.WindowService.Utility.FileWatcher
{
    public class FileWatcher : IFileWatcher
    {

        /// <summary>
        /// Get the directory to watch
        /// </summary>
        /// <returns>string</returns>
        public string GetDirectoryToWatch()
        {
            var xmlstring = (ConfigurationManager.GetSection(typeof(FileWatcherDirectoryCollection).Name) as string);

            XmlSerializer serializer = new XmlSerializer(typeof(FileWatcherDirectoryCollection));

            FileWatcherDirectoryCollection payeefileProcessors = new FileWatcherDirectoryCollection();

            using (StringReader reader = new StringReader(xmlstring))
            {
                payeefileProcessors = (FileWatcherDirectoryCollection)serializer.Deserialize(reader);
            }
            return payeefileProcessors.FileProcessor.FirstOrDefault(x => x.IsActive == true).Path.Directorytowatch;
        }


        /// <summary>
        /// Get directories to watch.
        /// This gets a bunch of directories that have to ben watched
        /// </summary>
        /// <returns>List{string}</returns>
        public List<string> GetDirectoriesToWatch()
        {
            var xmlstring = (ConfigurationManager.GetSection(typeof(FileWatcherDirectoryCollection).Name) as string);

            XmlSerializer serializer = new XmlSerializer(typeof(FileWatcherDirectoryCollection));

            FileWatcherDirectoryCollection payeefileProcessors = new FileWatcherDirectoryCollection();

            using (StringReader reader = new StringReader(xmlstring))
            {
                payeefileProcessors = (FileWatcherDirectoryCollection)serializer.Deserialize(reader);
            }
            return payeefileProcessors.FileProcessor.Select(x => x.Path.Directorytowatch).ToList();
        }


        /// <summary>
        /// Get the list of paye file processors
        /// </summary>
        /// <returns>List{PayeeFileProcessor}</returns>
        public List<FileProcessor> GetFileProcessors()
        {
            var xmlstring = (ConfigurationManager.GetSection(typeof(FileWatcherDirectoryCollection).Name) as string);

            XmlSerializer serializer = new XmlSerializer(typeof(FileWatcherDirectoryCollection));

            FileWatcherDirectoryCollection payeefileProcessors = new FileWatcherDirectoryCollection();

            using (StringReader reader = new StringReader(xmlstring))
            {
                payeefileProcessors = (FileWatcherDirectoryCollection)serializer.Deserialize(reader);
            }
            if (payeefileProcessors == null)
            {
                return new List<FileProcessor>();
            }
            return payeefileProcessors.FileProcessor.ToList();
        }


        /// <summary>
        /// Get the path segment of the full path
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="trimPart"></param>
        /// <returns>string</returns>
        public string TrimFilePath(string fullPath, string trimPart)
        {
            string charsToRemove = "\\" + trimPart;
            string path = fullPath.Remove((fullPath.Length - charsToRemove.Length), charsToRemove.Length);
            return path;
        }
    }
}
