using System.Collections.Generic;

namespace DemoData
{
    public class Class1
    {
        public static IEnumerable<string> Strings
        {
            get
            {
                return new List<string>
                {
                    "a",
                    "b",
                    "c"
                };
            }
        } 
    }
}
