using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace PSBook.Chapter6
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create and open a runspace
            Runspace runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();

            // Create an empty pipeline
            Pipeline pipeline = runspace.CreatePipeline();

            // Create a get-childitem command and add the
            // 'path' and 'recurse' parameters
            Command dirCommand = new Command("get-childitem");
            dirCommand.Parameters.Add("path",
                  "hklm:\\software\\microsoft\\PowerShell");
            dirCommand.Parameters.Add("recurse");

            // Add the command to the pipeline
            pipeline.Commands.Add(dirCommand);

            // Append a sort-object command using the shorthand method
            pipeline.Commands.Add("sort-object");

            // Invoke the command
            Collection<PSObject> results = pipeline.Invoke();
            foreach (PSObject thisResult in results)
            {
                Console.WriteLine(thisResult.ToString());
            }
        }
    }
}
