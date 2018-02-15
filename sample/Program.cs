using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CircularStack;

namespace CircularStack.Sample
{
    public class Program
    {
        static void Main(string[] args)
        {
            var items = new List<string> {"a","b", "2", "1"};
            var stack = new CircularStack(items);

            $"initial state: {stack.GetState()}".Dump();

            stack.Push("c");
            $"push('c') state: {stack.GetState()}".Dump();

            stack.Push("d");
            $"push('d') state: {stack.GetState()}".Dump();

            $"pop() yields: {stack.Pop()}".Dump();
            $"pop() state: {stack.GetState()}".Dump();

            $"pop() yields: {stack.Pop()}".Dump();
            $"pop() state: {stack.GetState()}".Dump();
        }
    }

    public static class Extensions
    {
        public static string GetState(this CircularStack stack)
        {
            string str = "";
            foreach(var item in stack)
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
