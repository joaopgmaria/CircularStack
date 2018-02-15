using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace CircularStack
{
    [TestFixture]
    public class CircularStackTests
    {
        [Test]
        public void CircularStack_Should_BeInitializedWithDefaultCapacity()
        {
            var obj = new CircularStack();
            Assert.AreEqual(CircularStack.DefaultCapacity, obj.Capacity);
        }

        [Test]
        public void CircularStack_Should_BeInitializedWithProvidedCapacity()
        {
            int capacity = 100;

            var obj = new CircularStack(capacity);
            Assert.AreEqual(capacity, obj.Capacity);
        }

        [Test]
        public void CircularStack_Should_BeInitializedWithCollection()
        {
            var items = new List<string> { "a", "b" };
            
            var obj = new CircularStack<string>(items);

            Assert.AreEqual(2, obj.Count);
            Assert.AreEqual(items.Count, obj.Capacity);
            obj.ShouldContainElementsReversed(items);
        }

        [Test]
        public void CircularStack_Should_BeInitializedWithCollectionAndCapacity()
        {
            var items = new List<string> { "a", "b" };

            var obj = new CircularStack<string>(items, 4);

            Assert.AreEqual(2, obj.Count);
            Assert.AreEqual(4, obj.Capacity);
            obj.ShouldContainElementsReversed(items);
        }

        [Test]
        public void Count_Should_ReturnItemCount()
        {
            var arr = new List<string> { "a", "b" };
            var obj = new CircularStack(arr);

            Assert.AreEqual(2, obj.Count);
        }

        [Test]
        public void Count_Should_ReturnZero_When_StackIsEmpty()
        {
            var obj = new CircularStack();

            Assert.AreEqual(0, obj.Count);
        }

        [Test]
        public void IsFull_Should_ReturnTrue_WhenStackIsFull()
        {
            var arr = new List<string> { "a", "b" };
            var obj = new CircularStack(arr);

            Assert.IsTrue(obj.IsFull);
        }

        [Test]
        public void Push_Should_AddItem_WhenStackIsEmpty()
        {
            var obj = new CircularStack();

            obj.Push(1);

            Assert.AreEqual(1, obj.Count);
            obj.ShouldContainElement(1);
        }

        [Test]
        public void Push_Should_AddItem()
        {
            var arr = new List<string> { "a", "b" };
            var obj = new CircularStack<string>(arr, 4);

            obj.Push("c");

            Assert.AreEqual(3, obj.Count);

            var expected = new List<string> { "c", "b", "a" };
            obj.ShouldContainElementsInOrder(expected);
        }
        
        [Test]
        public void Push_Should_AddItem_When_ThereIsOnlyOneSpaceLeft()
        {
            var obj = new CircularStack<int>(2);

            obj.Push(1);
            obj.Push(2);

            Assert.AreEqual(2, obj.Count);

            var expected = new List<int> { 2, 1 };
            obj.ShouldContainElementsInOrder(expected);
        }

        [Test]
        public void Push_Should_DropLastItem_When_StackIsFull()
        {
            var expected = "b";
            var arr = new List<string> { "a", expected };
            var obj = new CircularStack<string>(arr);
            
            obj.Push("c");

            obj.Pop();
            var actual = obj.Pop();

            Assert.AreSame(expected, actual);

            var expectedItems = new List<string> { "c", "b" };
            obj.ShouldContainElementsInOrder(expectedItems);
        }

        [Test]
        public void Peek_Should_ReturnLastItem()
        {
            var expected = "b";
            var arr = new List<string> { "a", "b" };
            var obj = new CircularStack(arr);

            var actual = obj.Peek();

            Assert.AreSame(expected, actual);
        }

        [Test]
        public void Peek_ShouldNot_RemoveItem()
        {
            var arr = new List<string> { "a", "b" };
            var obj = new CircularStack(arr);

            obj.Peek();

            Assert.AreEqual(2, obj.Count);
        }

        [Test]
        public void Pop_Should_ReturnItem()
        {
            var expected = "b";
            var arr = new List<string> { "a", "b" };
            var obj = new CircularStack(arr);

            var actual = obj.Pop();

            Assert.AreSame(expected, actual);
        }

        [Test]
        public void Pop_Should_ReturnItem_When_ThereIsOnlyOne()
        {
            var expected = "a";
            var arr = new List<string> { "a" };
            var obj = new CircularStack(arr);

            var actual = obj.Pop();

            Assert.AreSame(expected, actual);
        }

        [Test]
        public void Pop_Should_RemoveItem()
        {
            var arr = new List<string> { "a", "b" };
            var obj = new CircularStack(arr);

            obj.Pop();

            Assert.AreEqual(1, obj.Count);
        }

        [Test]
        public void Pop_Should_RemoveItem_When_ThereIsOnlyOne()
        {
            var arr = new List<string> { "a" };
            var obj = new CircularStack(arr);

            obj.Pop();

            Assert.AreEqual(0, obj.Count);
        }

        [Test]
        public void Pop_Should_RespectCapacity()
        {
            var arr = new List<object> { new { } };
            var obj = new CircularStack(arr);

            obj.Pop();

            Assert.Throws<InvalidOperationException>(() => obj.Pop());
        }

        [Test]
        public void Push_ShouldNot_ChangeCapacity()
        {
            var obj = new CircularStack();

            obj.Push(1);

            Assert.AreEqual(CircularStack.DefaultCapacity, obj.Capacity);
        }

        [Test]
        public void Pop_ShouldNot_ChangeCapacity()
        {
            var arr = new List<object> { new { }, new { } };
            var obj = new CircularStack(arr);

            obj.Pop();

            Assert.AreEqual(2, obj.Capacity);
        }

        [Test]
        public void Push_Should_WorkAfterPop()
        {
            var expected = "c";
            var arr = new List<string> { "a", "b" };
            var obj = new CircularStack(arr);

            obj.Pop();
            obj.Push(expected);

            Assert.AreSame(expected, obj.Peek());
        }

        [Test]
        public void Pop_Should_WorkAfterPush()
        {
            var expected = "a";
            var obj = new CircularStack();
            
            obj.Push(expected);
            var actual = obj.Pop();

            Assert.AreSame(expected, actual);
        }

        [Test]
        public void Clone_Should_Work()
        {
            var arr = new List<string> { "a", "b" };
            var obj = new CircularStack(arr, 4);

            var actual = obj.Clone() as CircularStack<object>;

            Assert.AreEqual(obj.Capacity, actual.Capacity);
            Assert.AreEqual(obj.Count, actual.Count);
            Assert.AreEqual(obj.IsEmpty, actual.IsEmpty);
            Assert.AreEqual(obj.IsFull, actual.IsFull);
            Assert.AreEqual(obj.IsSynchronized, actual.IsSynchronized);
        }

        [Test]
        public void Pop_Should_Work_When_StackIsInitializedFromCollection()
        {
            var arr = new List<string> { "a", "b", "c", "d" };
            var obj = new CircularStack(arr);

            Assert.AreEqual("d", obj.Pop());
            Assert.AreEqual("c", obj.Pop());
            Assert.AreEqual("b", obj.Pop());
            Assert.AreEqual("a", obj.Pop());
        }

        [Test]
        public void Pop_Should_Work_When_ItemDropped()
        {
            var obj = new CircularStack(2);

            obj.Push("a");
            obj.Push("b");
            obj.Push("c");

            Assert.AreEqual("c", obj.Pop());
            Assert.AreEqual("b", obj.Pop());
            Assert.Throws<InvalidOperationException>(() => obj.Pop());
        }

        [Test]
        public void Pop_Should_Work_When_ItemDroppedAndInitializedFromCollectionAndCapacity()
        {
            var items = new List<string> { "a", "b" };
            var obj = new CircularStack<string>(items, 4);
 
            obj.Push("c");
            obj.Push("d");
            obj.Push("e");

            Assert.AreEqual("e", obj.Pop());
            Assert.AreEqual("d", obj.Pop());
            Assert.AreEqual("c", obj.Pop());
            Assert.AreEqual("b", obj.Pop());
            Assert.Throws<InvalidOperationException>(() => obj.Pop());
        }

        [Test]
        public void Pop_Should_Work_When_StackIsInitializedFromCollectionAndCapacity()
        {
            var arr = new List<string> { "a", "b" };
            var obj = new CircularStack(arr, 4);

            Assert.AreEqual("b", obj.Pop());
            Assert.AreEqual("a", obj.Pop());
        }

        [Test]
        public void Push_Should_Work_When_StackIsInitializedFromCollection()
        {
            var arr = new List<string> { "a", "b" };
            var obj = new CircularStack(arr);

            var expected = "c";

            obj.Push(expected);

            var actual = obj.Peek();

            Assert.AreSame(expected, actual);
        }

        [Test]
        public void Push_Should_Work_When_StackIsInitializedFromCollectionAndCapacity()
        {
            var arr = new List<string> { "a", "b" };
            var obj = new CircularStack(arr, 4);

            var expected = "c";

            obj.Push(expected);

            var actual = obj.Peek();

            Assert.AreSame(expected, actual);
        }

        [Test]
        public void Stack_Should_Work_WhenInitializedFromCollection()
        {
            var arr = new List<string> { "a", "b" };
            var obj = new CircularStack(arr);

            Assert.AreEqual("b", obj.Pop());
            Assert.AreEqual("a", obj.Peek());

            obj.Push("c");

            Assert.AreEqual("c", obj.Peek());
            Assert.AreEqual("c", obj.Pop());
            Assert.AreEqual("a", obj.Pop());
        }
    }

    public static class AssertExtensions
    {
        public static void ShouldContainElementsInOrder<T>(this CircularStack<T> stack, List<T> elements)
        {
            int idx = 0;
            foreach(var item in stack)
            {
                Assert.AreEqual(elements[idx], item);
                idx++;
            }
        }

        public static void ShouldContainElement<T>(this CircularStack<T> stack, T element)
        {
            bool found = false;
            foreach(var item in stack)
            {
                if (item.Equals(element))
                {
                    found = true;
                    break;
                }
            }

            Assert.IsTrue(found);
        }

        public static void ShouldContainElementsReversed<T>(this CircularStack<T> stack, List<T> elements)
        {
            int idx = stack.Count - 1;
            foreach(var item in stack)
            {
                Assert.AreEqual(elements[idx], item);
                idx--;
            }
        }
    }
}
