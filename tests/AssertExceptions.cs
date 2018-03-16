using NUnit.Framework;
using System.Collections.Generic;

namespace CircularStack.Tests
{
    internal static class AssertExtensions
    {
        public static void ShouldContainElementsInOrder<T>(this CircularStack<T> stack, params T[] elements)
        {
            Assert.AreEqual(elements.Length, stack.Count);

            int idx = 0;
            foreach (var item in stack)
            {
                if (item != null)
                {
                    Assert.AreEqual(elements[idx], item);
                    idx++;
                }
            }
        }

        public static void ShouldContainElementsInOrder<T>(this CircularStack<T> stack, List<T> elements) => ShouldContainElementsInOrder(stack, elements.ToArray());

        public static void ShouldContainElement<T>(this CircularStack<T> stack, T element)
        {
            bool found = false;
            foreach (var item in stack)
            {
                if (item.Equals(element))
                {
                    found = true;
                    break;
                }
            }

            Assert.IsTrue(found);
        }

        public static void ShouldContainElementsReversed<T>(this CircularStack<T> stack, params T[] elements)
        {
            Assert.AreEqual(elements.Length, stack.Count);

            int idx = stack.Count - 1;
            foreach (var item in stack)
            {
                if (item != null)
                {
                    Assert.AreEqual(elements[idx], item);
                    idx--;
                }
            }
        }

        public static void ShouldContainElementsReversed<T>(this CircularStack<T> stack, List<T> elements) => ShouldContainElementsReversed(stack, elements.ToArray());
    }
}
