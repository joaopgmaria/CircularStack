﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CircularStack
{
    public class CircularStack : CircularStack<object>
    {
        public CircularStack()
        {
        }

        public CircularStack(int capacity) : base(capacity)
        {
        }

        public CircularStack(IEnumerable<object> items) : base(items)
        {
        }

        public CircularStack(IEnumerable<object> items, int capacity) : base(items, capacity)
        {
        }
    }

    public class CircularStack<T> : ICollection
    {
        public const int DefaultCapacity = 10;

        private readonly T[] _space;
        private int _last = -1;
        private readonly object _syncRoot = new object();

        public int Capacity => _space.Length;
        public int Count => _last + 1;
        public bool IsFull => Count == Capacity;
        public bool IsEmpty => Count == 0;
        public bool IsSynchronized => false;
        public object SyncRoot => _syncRoot;

        public CircularStack()
        {
            _space = new T[DefaultCapacity];
        }

        public CircularStack(int capacity)
        {
            _space = new T[capacity];
        }

        public CircularStack(IEnumerable<T> items)
        {
            int count = items.Count();

            _space = new T[count];

            items.ToList()
                 .ForEach(Push);
        }

        public CircularStack(IEnumerable<T> items, int capacity)
        {
            _space = new T[capacity];

            items.ToList()
                 .ForEach(Push);
        }

        /// <summary>  
        /// Push Item in Front  
        /// </summary>  
        /// <param name="item"></param>  
        public void Push(T item)
        {
            if (Count == Capacity)
            {
                _last--;
            }

            if (Count < Capacity)
            {
                for (int i = _last; i >= 0; i--)
                {
                    _space[i + 1] = _space[i];
                }
                
                _space[0] = item;
                _last++;
            }
        }

        /// <summary>  
        /// Pop Item In Front  
        /// </summary>  
        /// <returns></returns>  
        public T Pop()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("Stack is Empty");
            }

            var popedItem = _space[0];

            for (int i = 1; i < Count; i++)
            {
                _space[i - 1] = _space[i];
            }

            _space[_last] = default(T);
            _last--;

            return popedItem;
        }

        /// <summary>  
        /// Peek Item In Front  
        /// </summary>  
        /// <returns></returns>  
        public T Peek()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("Stack is Empty");
            }

            return _space[0];
        }

        public IEnumerator GetEnumerator() => _space.GetEnumerator();

        public void CopyTo(Array array, int index)
        {
            foreach (T i in _space)
            {
                array.SetValue(i, index);
                index = index + 1;
            }
        }
    }
}
