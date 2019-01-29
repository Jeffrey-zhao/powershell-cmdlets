using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Management.Automation;

namespace RunspaceInvokeSample1
{
    class Program
    {
        static void Main(string[] args)
        {
            RunspaceInvoke invoker = new RunspaceInvoke();
            string[] input = { "system", "software", "security" };
            IList errors;
            string scriptBlock =
                          "$input | foreach {get-item hklm:\\$_}";

            foreach (PSObject thisResult in
                     invoker.Invoke(scriptBlock, input, out errors))
            {
                Console.WriteLine("Output: {0}", thisResult);
            }
            foreach (object thisError in errors)
            {
                Console.WriteLine("Error: {0}", thisError);
            }
        }
    }
}
