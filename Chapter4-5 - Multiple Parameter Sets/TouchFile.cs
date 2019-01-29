using System;
using System.IO;
using System.Management.Automation;

namespace PSBook.Chapter4
{
    [Cmdlet("Touch", "File", DefaultParameterSetName = "PathSet")]
    public class TouchFileCommand : PSCmdlet
    {
        private string path = null;

        [Parameter(ParameterSetName = "PathSet", Mandatory=true, Position=1)]
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

        private FileInfo fileInfo = null;

        [Parameter(ParameterSetName = "FileInfoSet", Mandatory = true, Position = 1)]
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

        DateTime date = DateTime.Now;

        [Parameter]
        public DateTime Date
        {
            get
            {
                return date;
            }
            set
            {
                date = value;
            }
        }

        protected override void ProcessRecord()
        {
            if (fileInfo != null)
            {
                fileInfo.LastWriteTime = date;
            }

            if (File.Exists(path))
            {
                File.SetLastWriteTime(path, date);
            }
        }
    }
}