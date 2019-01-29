using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading;

namespace MultiplePipeReader2
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a runspace
            Runspace runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();

            // Create a pipeline
            Pipeline pipeline = runspace.CreatePipeline("1..10 | foreach {$_; write-error $_; start-sleep 1}");

            // Subscribe to the DataReady events of the pipes
            pipeline.Output.DataReady += new EventHandler(HandleDataReady);
            pipeline.Error.DataReady += new EventHandler(HandleDataReady);

            // Start the pipeline
            pipeline.InvokeAsync();
            pipeline.Input.Close();

            // Do important things in the main thread
            do
            {
                Thread.Sleep(1000);
                Console.Title = string.Format("Time: {0}", DateTime.Now);
            } while (pipeline.PipelineStateInfo.State == PipelineState.Running);
        }

        static void HandleDataReady(object sender, EventArgs e)
        {
            PipelineReader<PSObject> output = sender as PipelineReader<PSObject>;
            if (output != null)
            {
                while (output.Count > 0)
                {
                    Console.WriteLine("Output: {0}", output.Read());
                }
                return;
            }

            PipelineReader<object> error = sender as PipelineReader<object>;
            if (error != null)
            {
                while (error.Count > 0)
                {
                    Console.WriteLine("Error: {0}", error.Read());
                }
                return;
            }
        }
    }
}
