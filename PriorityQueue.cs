using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriorityQueue
{

public class PriorityQueueException : Exception
{
   public PriorityQueueException(string message)
       : base(message)
   {
      // nothing additional to do
   }
}

public interface IPriorityQueueStore<T>
{
   // Properties
   int Count { get; }

   // Methods
   void Enqueue(T value, int priority);
   T DequeueMin();
   T DequeueMax();
}

public class PriorityQueue<T>
{
   private IPriorityQueueStore<T> store;

   public PriorityQueue(IPriorityQueueStore<T> store_)
   {
      store = store_;
   }

   public void Enqueue(T value, int priority)
   {
      store.Enqueue(value, priority);
   }

   public T DequeueMin()
   {
      if (store.Count == 0)
         throw new PriorityQueueException("Empty queue!");

      return store.DequeueMin();
   }
       
   public T DequeueMax()
   {
      if (store.Count == 0)
         throw new PriorityQueueException("Empty queue!");

      return store.DequeueMax();
   }

   public int Count
   {
      get { return store.Count; }
   }

   // these void versions are still here cuz technically they were asked for, but I want the others so I can unit test better.
   // public void DequeueMin() { store.Remove(store.Min); }
   // public void DequeueMax() { store.Remove(store.Max); }
}

// Engineer's implementation of the problem presented to me:
//    In C#, write a priority queue. It should implement the following methods:
//       - void Enqueue(T value, int priority), enqueue the element with the given priority
//       - void DequeueMin(), dequeue the element with minimum priority
//       - void DequeueMax(), dequeue the element with maximum priority
//     
//    Be thoughtful of the time complexity for these operations.
//
// I could heed the call of "Not Invented Here Syndrome" and make my own heap/tree, which is what you want
// if you need a performant/not-naieve priority queue. But Google has told me that the nice folks at Microsoft 
// have already made me a SortedSet, which dotPeek decompilation shows me is a red-black tree, which
// gives me the proper big O for my time complexity.
//
// I believe this shows that I'm an engineer and I can use the whole toolbox. Actually nevermind - this is a bit bare.
// I'll make the heap too.
public class SortedSetStore<T> : IPriorityQueueStore<T>
{
   private class QueueComparer<N> : IComparer<Tuple<int, N>>
   {
      public int Compare(Tuple<int, N> left, Tuple<int, N> right)
      {
         return left.Item1.CompareTo(right.Item1);
      }
   }

   // SortedSet is implemented in C# .NET as a red-black tree, so en/dequeue will be O(log n)
   private SortedSet<Tuple<int, T>> store = new SortedSet<Tuple<int, T>>(new QueueComparer<T>());

   public void Enqueue(T value, int priority)
   {
      store.Add(new Tuple<int, T>(priority, value));
   }

   public T DequeueMin()
   {
      Tuple<int, T> min = store.Min;
      store.Remove(min);
      return min.Item2;
   }
       
   public T DequeueMax()
   {
      Tuple<int, T> max = store.Max;
      store.Remove(max);
      return max.Item2;
   }

   public int Count
   {
      get { return store.Count; }
   }
}

}
