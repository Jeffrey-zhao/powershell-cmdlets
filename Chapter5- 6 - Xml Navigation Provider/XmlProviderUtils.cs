using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.IO;
using System.Xml;

using System.Management.Automation;
using System.Management.Automation.Provider;


namespace Providers
{    
    /// <summary>
    /// Static utility class that houses some common utility routines dealing with path string manipulation.
    /// </summary>
    public static class XmlProviderUtils 
    {
        public const string XML_PATH_SEP = "/";
        public const string XML_DRIVE_SEP = ":";
        public const string XML_PROVIDER_SEP = "::";

        public readonly static char[] DriveSeparator = new char[] { ':' };            

        #region Helper methods

        public static XmlDriveInfo GetDriveFromPath(string path, ProviderInfo provider)
        {
            // two formats possible
            // provider::drive:\path
            // drive:\path
            string[] paths = path.Split(DriveSeparator, 2);
            string drivepath = paths[0];            
            foreach (PSDriveInfo drive in provider.Drives)
            {
                if (drive.Name == drivepath)
                    return drive as XmlDriveInfo;
            }
            return null;
        }

        public static string NormalizePath(string path)
        {
            string result = path;

            if (!String.IsNullOrEmpty(path))
                result = path.Replace("\\", XML_PATH_SEP);

            // this is for Container & Navigation providers. They parse out the drive and 
            // the leading path separator "\" or "/" so we need to add it if it isn't already present at the beginning
            // -----------------
            if (!result.StartsWith(XML_PATH_SEP))
                result = XML_PATH_SEP + result;

            return result;
        }

        public static string PathNoDrive(string path)
        {
            string[] paths = path.Split(DriveSeparator, 2);
            string pathNoDrive = paths[0];
            if (paths.Length == 2)
                pathNoDrive = paths[1];

            return pathNoDrive;
        }

        #endregion
    }
}
