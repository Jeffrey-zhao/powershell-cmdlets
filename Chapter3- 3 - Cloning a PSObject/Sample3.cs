using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;

namespace PSBook.Chapter3
{
    class Sample3
    {
        static void Main(string[] args)
        {
            // Create a CLR datetime object
            System.DateTime date = new DateTime(2007, 12, 25);

            // Use it to create a PSObject
            PSObject psobject = new PSObject(date);

            // Create a PSObject using AsPSobject method
            //This will return the existing psobject as result
            PSObject psobject2 = PSObject.AsPSObject(psobject);

            //This will create new PSObject that wraps the date object and return that object as result
            PSObject psobject3 = PSObject.AsPSObject(date);
        }

    }
}
