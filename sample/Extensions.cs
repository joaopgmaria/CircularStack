using System;

namespace CircularStack.Sample
{
    public static class Extensions
    {
        public static string GetState(this CircularStack stack)
        {
            return String.Join(",", stack);
        }

        public static void Dump(this object obj)
        {
            Console.WriteLine(obj);
        }
    }
}
