using System;
using System.IO;
using System.Management.Automation;

namespace PSBook.Chapter4
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ValidateEvenNumberAttribute : ValidateArgumentsAttribute
    {
        protected override void Validate(object element, EngineIntrinsics sessionState)
        {
            if (element == null || !(element is int))
            {
                throw new ArgumentException("Invalid parameter value");
            }

            int i = (int) element;

            if(i % 2 != 0)
            {
                throw new ArgumentException("Not an even number.");
            }
        }
    }

    [Cmdlet("Get", "EvenNumber")]
    public class GetEvenNumberCommand : PSCmdlet
    {
        int number = 0;

        [Parameter(Mandatory=true)]
        [ValidateEvenNumber]
        public int Number
        {
            get
            {
                return number;
            }
            set
            {
                number = value;
            }
        }

        protected override void ProcessRecord()
        {
        }
    }
}