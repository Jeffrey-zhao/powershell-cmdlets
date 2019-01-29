using System;
using System.Diagnostics;
using System.Management.Automation;             //Windows PowerShell namespace
using System.ComponentModel;
using System.Collections.ObjectModel;           // For Collection
using System.Management.Automation.Runspaces;   // Needed for CmdletConfigurationEntry

[RunInstaller(true)]
public class PSBookChapter2MyCustomeSnapIn : CustomPSSnapIn
{
    // Specify the cmdlets that belong to this custom PowerShell snap-in.
    private Collection<CmdletConfigurationEntry> cmdlets;
    public override Collection<CmdletConfigurationEntry> Cmdlets
    {
        get
        {
            if (cmdlets == null)
            {
                cmdlets = new Collection<CmdletConfigurationEntry>();
                cmdlets.Add(
                   new CmdletConfigurationEntry(
                      "Say-Hello ",             // cmdlet name
                      typeof(SayHelloCmdlet),   // cmdlet class type	
                      null                      // help filename for the cmdlet
                   )
                );
            }

            return cmdlets;
        }
    }

    public override string Name
    {
        get { return "Wiley.PSProfessional.Chapter2-Custom"; }
    }

    public override string Vendor
    {
        get { return "Wiley"; }
    }

    public override string Description
    {
        get { return " This is a sample PowerShell custom snap-in"; }
    }

    // Specify the providers that belong to this custom PowerShell snap-in.
    private Collection<ProviderConfigurationEntry> providers;
    public override Collection<ProviderConfigurationEntry> Providers
    {
        get
        {
            if (providers == null)
            {
                providers = new Collection<ProviderConfigurationEntry>();
            }
            return providers;
        }
    }

    // Specify the Types that belong to this custom PowerShell snap-in.
    private Collection<TypeConfigurationEntry> types;
    public override Collection<TypeConfigurationEntry> Types
    {
        get
        {
            if (types == null)
            {
                types = new Collection<TypeConfigurationEntry>();
            }
            return types;
        }
    }

    // Specify the Format that belong to this custom PowerShell snap-in.
    private Collection<FormatConfigurationEntry> formats;
    public override Collection<FormatConfigurationEntry> Formats
    {
        get
        {
            if (formats == null)
            {
                formats = new Collection<FormatConfigurationEntry>();
            }
            return formats;
        }
    }

    [Cmdlet("say", "hello")]
    public class SayHelloCmdlet : Cmdlet
    {
        protected override void ProcessRecord()
        {
            WriteObject("Hello!");
        }
    }
}
