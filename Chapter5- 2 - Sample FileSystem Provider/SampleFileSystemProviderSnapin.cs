using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.ComponentModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Providers
{
    /// <summary>
    /// Snapin installer class for the samepl filesystem provider. Run InstallUtil.exe SampleFilesystemProvider.dll
    /// to install this snapin into the registry in order to use it within Powershell.
    /// </summary>
    [RunInstaller(true)]
    public class SampleFileSystemProviderSnapin : PSSnapIn
    {
        /// <summary>
        /// Public Ctor
        /// </summary>
        public SampleFileSystemProviderSnapin()
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
                return "SampleFileSystemProviderSnapin";
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
                return "SampleFileSystemProviderSnapin,Microsoft";
            }
        }

        /// <summary>
        /// Specify a description of the PowerShell snap-in.
        /// </summary>
        public override string Description
        {
            get
            {
                return "This is a PowerShell snap-in that includes a minimal sample file system Provider";
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
                return "SampleFileSystemProviderSnapin,This is a PowerShell snap-in that includes a minimal sample file system Provider.";
            }
        }         
    }
}
