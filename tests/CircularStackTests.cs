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
            
            var obj = new CircularStack(items);

            Assert.AreEqual(2, obj.Count);
            Assert.AreEqual(items.Count, obj.Capacity);
            Assert.AreSame(items[1], obj.Peek());
        }

        [Test]
        public void CircularStack_Should_BeInitializedWithCollectionAndCapacity()
        {
            var items = new List<string> { "a", "b" };

            var obj = new CircularStack(items, 4);

            Assert.AreEqual(2, obj.Count);
            Assert.AreEqual(4, obj.Capacity);
            Assert.AreSame(items[1], obj.Peek());
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
        }

        [Test]
        public void Push_Should_AddItem()
        {
            var arr = new List<string> { "a", "b" };
            var obj = new CircularStack(arr, 4);

            obj.Push("c");

            Assert.AreEqual(3, obj.Count);
        }
        
        [Test]
        public void Push_Should_AddItem_When_ThereIsOnlyOneSpaceLeft()
        {
            var obj = new CircularStack(2);

            obj.Push(1);
            obj.Push(2);

            Assert.AreEqual(2, obj.Count);
        }

        [Test]
        public void Push_Should_DropLastItem_When_StackIsFull()
        {
            var expected = "b";
            var arr = new List<string> { "a", expected };
            var obj = new CircularStack(arr);
            
            obj.Push("c");

            obj.Pop(); //c
            var actual = obj.Pop();

            Assert.AreSame(expected, actual);
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
            //Create stack with capacity 4 and 2 items
            var arr = new List<string> { "a", "b" };
            var obj = new CircularStack(arr, 4);

            var actual = obj.Clone() as CircularStack<object>;

            Assert.AreEqual(obj.Capacity, actual.Capacity);
            Assert.AreEqual(obj.Count, actual.Count);
            Assert.AreEqual(obj.IsEmpty, actual.IsEmpty);
            Assert.AreEqual(obj.IsFull, actual.IsFull);
            Assert.AreEqual(obj.IsSynchronized, actual.IsSynchronized);
        }
    }
}
