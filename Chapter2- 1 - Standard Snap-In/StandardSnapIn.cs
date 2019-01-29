using System;
using System.Management.Automation;
using System.ComponentModel;

namespace PSBook.Chapter2
{
    [RunInstaller(true)]
    public class PSBookChapter2MySnapIn : PSSnapIn
    {
        // Name for the PowerShell snap-in.
        public override string Name
        {
            get
            {
                return "Wiley.PSProfessional.Chapter2";
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

    // Code to implement cmdlet Write-Hi
    [Cmdlet(VerbsCommunications.Write, "Hi")]
    public class SayHi : Cmdlet
    {
        protected override void ProcessRecord()
        {
            WriteObject("Hi, World!");
        }
    }

    // Code to implement cmdlet Write-Hello
    [Cmdlet(VerbsCommunications.Write, "Hello")]
    public class SayHello : Cmdlet
    {
        protected override void ProcessRecord()
        {
            WriteObject("Hello, World!");
        }
    }
}
