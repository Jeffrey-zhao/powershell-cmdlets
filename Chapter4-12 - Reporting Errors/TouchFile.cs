using System;
using System.IO;
using System.Management.Automation;

namespace PSBook.Chapter4
{
    [Cmdlet("Touch", "File", DefaultParameterSetName = "Path")]
    public class TouchFileCommand : PSCmdlet
    {
        private string path = null;

        [Parameter(ParameterSetName = "Path", Mandatory = true, Position = 1,
            ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        [Alias("FullName")]
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
            FileInfo myFileInfo = fileInfo;

            if (myFileInfo == null && File.Exists(path))
            {
                myFileInfo = new FileInfo(path);
            }

            if (myFileInfo != null)
            {
                try
                {
                    myFileInfo.LastWriteTime = date;
                }
                catch (UnauthorizedAccessException uae)
                {
                    ErrorRecord errorRecord = new ErrorRecord(uae,
                        "UnauthorizedFileAccess",
                        ErrorCategory.PermissionDenied,
                        myFileInfo.FullName);

                    WriteError(errorRecord);
                    return;
                }

                WriteObject(myFileInfo);
            }
        }
    }
}