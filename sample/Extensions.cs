using System;

namespace CircularStack.Sample
{
    public static class Extensions
    {
        public static string GetState(this CircularStack stack)
        {
            string str = "";
            foreach (var item in stack)
            {
                str += item?.ToString() + ",";
            }

            str = str.Trim(',');
            return str;
        }

        public static void Dump(this object obj)
        {
            Console.WriteLine(obj.ToString());
        }
    }
}
