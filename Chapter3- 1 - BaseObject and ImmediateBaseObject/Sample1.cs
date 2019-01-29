using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;

namespace PSBook.Chapter3
{
    class Sample1
    {
        static void Main(string[] args)
        {
            System.DateTime date = new System.DateTime(2007, 12, 25);
            PSObject psobject = new PSObject(date);
            PSObject psobject2 = new PSObject(psobject);
            Console.WriteLine("Name: {0}", psobject2.ImmediateBaseObject.GetType().Name);
            Console.WriteLine("Type: {0}", psobject2.BaseObject.GetType().Name);
            Console.Read();
        }
    }
}
