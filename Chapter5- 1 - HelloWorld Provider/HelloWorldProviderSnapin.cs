using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.ComponentModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Providers
{
    [RunInstaller(true)]
    public class HelloWorldProviderSnapin : PSSnapIn
    {
        /// <summary>
        /// Public Ctor
        /// </summary>
        public HelloWorldProviderSnapin()
            : base()
        {
        }

        /// <summary>
        /// Specify the name of the PowerShell snap-in.
        /// </summary>
        public override string Name
        {
            get
            {
                return "HelloWorldProviderSnapin";
            }
        }

        /// <summary>
        /// Specify the vendor for the PowerShell snap-in.
        /// </summary>
        public override string Vendor
        {
            get
            {
                return "Microsoft";
            }
        }

        /// <summary>
        /// Specify the localization resource information for the vendor. 
        /// Use the format: resourceBaseName,VendorName. 
        /// </summary>
        public override string VendorResource
        {
            get
            {
                return "HelloWorldProviderSnapin,Microsoft";
            }
        }

        /// <summary>
        /// Specify a description of the PowerShell snap-in.
        /// </summary>
        public override string Description
        {
            get
            {
                return "This is a PowerShell snap-in that includes the HelloWorldProvider.";
            }
        }

        /// <summary>
        /// Specify the localization resource information for the description. 
        /// Use the format: resourceBaseName,Description. 
        /// </summary>
        public override string DescriptionResource
        {
            get
            {
                return "HelloWorldProviderSnapin,This is a PowerShell snap-in that includes a sample Provider.";
            }
        }         
    }
}
