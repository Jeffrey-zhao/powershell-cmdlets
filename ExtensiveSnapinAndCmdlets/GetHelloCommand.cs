using System;
using System.Management.Automation;

namespace ExtensiveSnapinAndCmdlets
{

    [Cmdlet(VerbsCommon.Get, "Hello")]
    public class GetHelloCommand : Cmdlet
    {
        protected override void EndProcessing()
        {
            WriteObject("Hello", true);
        }
    }

    class MainClass1
    {
        public static void Main(string[] args)
        {
            PowerShell powerShell = PowerShell.Create();

            // import commands from the current executing assembly
            powerShell.AddCommand("Import-Module")
                .AddParameter("Assembly",
                      System.Reflection.Assembly.GetExecutingAssembly());
            powerShell.Invoke();
            powerShell.Commands.Clear();


            powerShell.AddCommand("Get-Hello");
            foreach (string str in powerShell.AddCommand("Out-String").Invoke<string>())
                Console.WriteLine(str);
        }
    }
}
