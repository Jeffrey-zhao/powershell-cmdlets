using System;
using System.IO;
using System.Management.Automation;

namespace PSBook.Chapter4
{
    [Cmdlet("Touch", "File", DefaultParameterSetName = "Path")]
    public class TouchFileCommand : PSCmdlet
    {
        private string path = null;

        [Parameter(ParameterSetName = "Path", Mandatory=true, Position=1, 
            ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [Alias("FullName")]
        [ValidateNotNullOrEmpty]
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

        [Parameter(ParameterSetName = "FileInfo", Mandatory = true, Position = 1, 
            ValueFromPipeline = true)]
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
            if (fileInfo == null && File.Exists(path))
            {
                fileInfo = new FileInfo(path);
            }

            if (fileInfo != null)
            {
                fileInfo.LastWriteTime = date;

                WriteObject(fileInfo);
            }
        }
    }
}