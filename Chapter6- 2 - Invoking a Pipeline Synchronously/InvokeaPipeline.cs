using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace PSBook.Chapter6
{
    class Sample1
    {
        static void Main(string[] args)
        {
            // Create and open a runspace that uses the default host
            Runspace runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();

            // Create a pipeline that runs the script block "calc"
            Pipeline pipeline = runspace.CreatePipeline("calc");

            // Run it
            pipeline.Invoke();
        }
    }
}
