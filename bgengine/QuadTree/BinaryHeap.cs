using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BGEngine
{
    public class BinaryHeap<T> where T : IComparable
    {
        public int MaxSize { get; private set; }
        public int Count { get; private set; }
        T[] Elements;

        public BinaryHeap(int AllocationSize = 10)
        {
            MaxSize = AllocationSize;
            Count = 0;
            Elements = new T[MaxSize];
        }

        public void Push(T Element)
        {
            if (Count >= MaxSize)
            {
                Grow();
            }
            
            Elements[Count++] = Element;
            if (Count == 1)
            {
                return;
            }

            int at = Count - 1;
            while (Elements[at].CompareTo((object)Elements[(at - 1) / 2]) > 0)
            {
                T temp = Elements[at];
                Elements[at] = Elements[(at - 1) / 2];
                Elements[(at - 1) / 2] = temp;
                at = (at - 1) / 2;
                if (at <= 0)
                    break;
            }
        }

        public T Pop()
        {
            if (Count == 0)
                throw new InvalidOperationException("Cannot pop from empty heap");

            T largest = Elements[0];
            Elements[0] = Elements[--Count];
            int at = 0;

            while ((at * 2 + 1) < Count)
            {
                int largest_child = at * 2;
                if ( (at * 2 + 2) < Count )
                {
                    T leftChild = Elements[at * 2 + 1];
                    T rightChild = Elements[at * 2 + 2];
                    largest_child += leftChild.CompareTo(rightChild) > 0 ? 1 : 2;
                }

                if (Elements[at].CompareTo(Elements[largest_child]) < 0)
                {
                    T temp = Elements[at];
                    Elements[at] = Elements[largest_child];
                    Elements[largest_child] = temp;
                    at = largest_child;
                }
                else
                    break;
            }
            return largest;
        }

        public T Peek()
        {
            if (Count == 0)
                throw new InvalidOperationException("Cannot peak at empty heap");
            return Elements[0];
        }

        public bool IsEmpty()
        {
                return Count == 0;
        }

        private void Grow()
        {
            int NewSize = MaxSize * 2;
            T[] NewElements = new T[NewSize];
            for (int n = 0; n < MaxSize; ++n)
            {
                NewElements[n] = Elements[n];
            }
            
            MaxSize = NewSize;
            Elements = NewElements;
        }
    }
}