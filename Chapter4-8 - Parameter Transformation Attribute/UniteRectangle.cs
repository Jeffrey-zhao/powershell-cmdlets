using System;
using System.Management.Automation;
using System.Drawing;
using System.Collections;

namespace PSBook.Chapter4
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ListToRectangleConverterAttribute : ArgumentTransformationAttribute
    {
        public override object Transform(EngineIntrinsics ei, object inputData)
        {
            object input = inputData;

            if (input is PSObject)
                input = ((PSObject)input).BaseObject;

            if (input is IList)
            {
                IList list = input as IList;

                if (list.Count == 4)
                {
                    return new Rectangle((int)list[0], (int)list[1],
                          (int)list[2], (int)list[3]);
                }
            }

            return inputData;
        }
    }

    [Cmdlet("Unite", "Rectangle")]
    public class UniteRectangleCommand : PSCmdlet
    {
        Rectangle rectangle1 = new Rectangle(0,0,0,0);

        [Parameter(Mandatory = true, Position = 1)]
        [ListToRectangleConverter]
        public Rectangle Rectangle1
        {
            get
            {
                return rectangle1;
            }
            set
            {
                rectangle1 = value;
            }
        }

        Rectangle rectangle2 = new Rectangle(0, 0, 0, 0);

        [Parameter(Mandatory = true, Position = 2)]
        [ListToRectangleConverter]
        public Rectangle Rectangle2
        {
            get
            {
                return rectangle2;
            }
            set
            {
                rectangle2 = value;
            }
        }

        protected override void ProcessRecord()
        {
            WriteObject(Rectangle.Union(rectangle1, rectangle2));
        }
    }
}