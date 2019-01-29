using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace PSBook.Chapter6
{
    class Sample2
    {
        static void Main(string[] args)
        {
            // Create and open a runspace that uses the default host
            Runspace runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();

            // Create a pipeline that runs a script block
            Pipeline pipeline = runspace.CreatePipeline("dir c:\\");

            // Invoke the pipeline in a try..catch and save the 
            // collection returned by Invoke()
            Collection<PSObject> results = null;
            try
            {
                results = pipeline.Invoke();
            }
            catch (RuntimeException e)
            {
                // Display a message and exit if a RuntimeException is thrown
                Console.WriteLine("Exception during Invoke(): {0}, {1}",
                                  e.GetType().Name, e.Message);
                return;
            }

            // Display the BaseObject of every PSObject returned by Invoke()
            foreach (PSObject thisResult in results)
            {
                Console.WriteLine("Result is: {0}", thisResult.BaseObject);
            }
        }
    }
}
