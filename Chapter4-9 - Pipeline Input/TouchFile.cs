using System;
using System.IO;
using System.Management.Automation;

namespace PSBook.Chapter4
{
    [Cmdlet("Touch", "File", DefaultParameterSetName = "Path")]
    public class TouchFileCommand : PSCmdlet
    {
        private string path = null;
        private FileInfo fileInfo = null;

        [Parameter(ParameterSetName = "Path")]
        public string Path
        {
            get
            {
                return path;
            }
            set
            {
                path = value;
            }
        }

        [Parameter(ParameterSetName = "FileInfo", ValueFromPipeline = true)]
        public FileInfo FileInfo
        {
            get
            {
                return fileInfo;
            }
            set
            {
                fileInfo = value;
            }
        }
        
        protected override void ProcessRecord()
        {
            if (fileInfo != null)
            {
                fileInfo.LastWriteTime = DateTime.Now;
            }

            if (File.Exists(path))
            {
                File.SetLastWriteTime(path, DateTime.Now);
            }
        }
    }

}