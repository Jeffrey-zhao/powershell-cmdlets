using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading;

namespace PSBook.Chapter6
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

            // Read output and error until the pipeline finishes
            pipeline.InvokeAsync();
            WaitHandle[] handles = new WaitHandle[2];
            handles[0] = pipeline.Output.WaitHandle;
            handles[1] = pipeline.Error.WaitHandle;
            pipeline.Input.Close();
            while (pipeline.PipelineStateInfo.State == PipelineState.Running)
            {
                switch (WaitHandle.WaitAny(handles))
                {
                    case 0:
                        while (pipeline.Output.Count > 0)
                        {
                            Console.WriteLine("Output: {0}", pipeline.Output.Read());
                        }
                        break;

                    case 1:
                        while (pipeline.Error.Count > 0)
                        {
                            Console.WriteLine("Error: {0}", pipeline.Error.Read());
                        }
                        break;
                }
            }
        }
    }
}
