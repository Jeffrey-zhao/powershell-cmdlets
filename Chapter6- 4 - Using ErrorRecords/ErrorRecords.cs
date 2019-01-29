using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace PSBook.Chapter6
{
    class Sample4
    {
        static void Main(string[] args)
        {
            // Create and open a runspace that uses the default host
            Runspace runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();

            // Create a pipeline that enumerates hklm:\
            Pipeline pipeline = runspace.CreatePipeline("get-childitem hklm:\\");

            // Invoke the pipeline
            pipeline.Invoke();

            // Display errors from the error pipe
            while (!pipeline.Error.EndOfPipeline)
            {
                Console.WriteLine("Error: {0}", pipeline.Error.Read());
            }
        }
    }
}
