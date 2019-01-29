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
    /// Snapin installer class to install the snapin in the Registry. Execute 'InstallUtil.exe XmlItemProvider.dll' 
    /// after building this project to configure the appropriate settings in the registry for this snapin. 
    /// If InstallUtil.exe is not in your PATH, it can usually be found in:
    /// '%windir%\microsoft.net\framework\v2.0.50727' directory.
    /// Then from inside powershell, execute 'add-pssnapin XmlItemProviderSnapin' to add the snapin to the
    /// current PowerShell session.
    /// </summary>
    [RunInstaller(true)]
    public class XmlItemProviderSnapin : PSSnapIn
    {
        /// <summary>
        /// Public Ctor
        /// </summary>
        public XmlItemProviderSnapin()
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
                return "XmlItemProviderSnapin";
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
                return "XmlItemProviderSnapin,Microsoft";
            }
        }

        /// <summary>
        /// Specify a description of the PowerShell snap-in.
        /// </summary>
        public override string Description
        {
            get
            {
                return "This is a PowerShell snap-in that includes the XmlItemProvider sample Provider";
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
                return "XmlItemProviderSnapin,This is a PowerShell snap-in that includes the XmlItemProvider sample Provider.";
            }
        }     
    }
}
