using System;
using System.IO;
using System.Management.Automation;

namespace PSBook.Chapter4
{
    [Cmdlet("Touch", "File")]
    public class TouchFileCommand : PSCmdlet
    {
        private string path = null;

        [Parameter(Mandatory = true)]
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

        protected override void ProcessRecord()
        {
            if (File.Exists(path))
            {
                File.SetLastWriteTime(path, DateTime.Now);
            }
        }
    }
}