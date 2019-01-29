// Save this to a file using filename: PSBook-7-WriteDebugSample.cs

using System;
using System.ComponentModel;
using System.Management.Automation;

namespace PSBook.Chapter7
{
    [RunInstaller(true)]
    public class PSBookChapter7WriteDebugSnapIn : PSSnapIn
    {
        public PSBookChapter7WriteDebugSnapIn()
            : base()
        {
        }
        // Name for the PowerShell snap-in.
        public override string Name
        {
            get
            {
                return "Wiley.PSProfessional.Chapter7.WriteDebug";
            }
        }
        // Vendor information for the PowerShell snap-in.
        public override string Vendor
        {
            get
            {
                return "Wiley";
            }
        }
        // Description of the PowerShell snap-in
        public override string Description
        {
            get
            {
                return "This is a sample PowerShell snap-in";
            }
        }
    }

    [Cmdlet(VerbsCommunications.Write, "DebugSample")]
    public sealed class WriteDebugSampleCommand : Cmdlet
    {
        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true)]
        [AllowEmptyString]
        public string Message
        {
            get { return message; }
            set { message = value; }
        }
        private string message = null;

        protected override void ProcessRecord()
        {
            base.WriteDebug(Message);
        }
    }
}
