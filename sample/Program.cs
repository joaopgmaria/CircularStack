using System.Collections.Generic;

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
}
