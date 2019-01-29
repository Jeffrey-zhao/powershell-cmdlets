using System;
using System.Management.Automation;
using System.ComponentModel;

namespace ProfessionalPowerShell.Templates
{
    [RunInstaller(true)]
    public class SampleSnapIn : PSSnapIn
    {
        // TODO: Customize the strings in this section for your project
        #region Snap-In Information

        private const string name = "ProfessionalPowerShell.Templates.Snap-In";
        private const string vendor = "Wiley";
        private const string description = "Empty template for authoring PowerShell snap-Ins";

        #endregion Snap-In Information

        #region Public Properties
        // Name for the PowerShell snap-in.
        public override string Name
        {
            get { return name; }
        }

        // Vendor information for the PowerShell snap-in.
        public override string Vendor
        {
            get { return vendor; }
        }

        // Description of the PowerShell snap-in
        public override string Description
        {
            get { return description; }
        }
        #endregion Public Properties
    }

    public class SampleCmdlet : PSCmdlet
    {
        // TODO: Use this section to declare private fields
        #region Private Fields

        //private string name = null;

        #endregion Private Fields

        // TODO: Use this section to declare cmdlet parameters
        #region Parameters

        //[Parameter]
        //public string Name
        //{
        //    get { return name; }
        //    set { name = value; }
        //}

        #endregion Parameters

        // TODO: Use this section to override BeginProcessing, ProcessRecord, and EndProcessing
        #region Overrides

        // Uncomment and add code to run once when your cmdlet starts.
        //protected override void BeginProcessing()
        //{
        //}

        // Uncomment and add code to run for each input object.  (For cmdlets which
        // don't take input from the pipeline, this will still run once.)
        //protected override void ProcessRecord()
        //{
        //}

        // Uncomment and add code to run once when all pipeline objects have been processed.
        //protected override void EndProcessing()
        //{
        //}

        #endregion Overrides
    }
}
