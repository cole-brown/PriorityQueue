using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriorityQueue
{

// Fine. I'll do the Not Invented Here version. After all that harping I did about the
// SortedSetStore.
public class IntervalHeap<T> where T : class
{
   IComparer<T> comparer;

   private class Node<N>
   {
      public N low;
      public N high;

      public Node(N low_, N high_)
      {
         low = low_;
         high = high_;
      }
   }

   // One-indexed, so heap[0] is a dummy cell
   private ArrayList heap = new ArrayList(); // could switch to actual array - would need to always insert at end, and do resize logic
   private int count = 0;

   public int Count
   {
      get { return count; }
   }

   public IntervalHeap(IComparer<T> comparer_)
   {
      comparer = comparer_;

      // creater our dummy cell
      heap.Add(new Node<T>((T)null, (T)null));
   }

   public void Add(T value)
   {
      if (value == null)
         throw new ArgumentException("No nulls allowed.");

      // New node at end of heap
      if (count % 2 == 0)
      {
         heap.Add(new Node<T>(value, (T)null));
      }
      else
      {
         Node<T> current = heap[heap.Count - 1] as Node<T>;

         // decide on low or high insert
         if (comparer.Compare(current.low, value) > 0)
         {
            current.high = current.low;
            current.low = value;
         }
         else
         {
            current.high = value;
         }
      }

      count++;

      // nothing to do if nothing to compare to
      if (count <= 2)
         return;

      // decide on min or max heap insert
      Node<T> parent = heap[(heap.Count - 1) / 2] as Node<T>;
      if (comparer.Compare(parent.low, value) > 0)
         MinHeapInsert();
      else if (comparer.Compare(parent.low, value) < 0)
         MaxHeapInsert();
      // else node is in proper place
   }

   private void MinHeapInsert()
   {
      int index = heap.Count - 1;
      Node<T> current = heap[index] as Node<T>;

      // bubble up until root or proper spot found
      while (index > 1)
      {
         int parentIndex = index / 2;
         Node<T> parent = heap[parentIndex] as Node<T>;

         // done if above lower bound
         if (comparer.Compare(current.low, parent.low) >= 0) break;

         // not done - swap with parent and continue
         T temp = current.low;
         current.low = parent.low;
         parent.low = temp;

         index = parentIndex;
         current = parent;
      }
   }

   private void MaxHeapInsert()
   {
      int index = heap.Count - 1;
      Node<T> current = heap[index] as Node<T>;
      
      // bubble up until root or proper spot found
      while (index > 1)
      {
         int parentIndex = index / 2;
         Node<T> parent = heap[parentIndex] as Node<T>;

         if (current.high == null)
         {
            // below the upper bound - we're good
            if (comparer.Compare(current.low, parent.high) < 0) break;

            // not done - swap with parent and continue
            T temp = current.low;
            current.low = parent.high;
            parent.high = temp;

            index = parentIndex;
            current = parent;
         }
         else
         {
            // below the upper bound - done
            if (comparer.Compare(current.high, parent.high) < 0) break;

            // not done - swap and continue
            T temp = current.high;
            current.high = parent.high;
            parent.high = temp;

            index = parentIndex;
            current = parent;
         }
      }
   }

   public T Min()
   {
      if (count == 0)
         throw new InvalidOperationException("empty heap");

      Node<T> min = heap[1] as Node<T>;
      return min.low;
   }

   public T Max()
   {
      if (count == 0)
         throw new InvalidOperationException("empty heap");

      Node<T> max = heap[1] as Node<T>;
      return count == 1 ? max.low : max.high;
   }

   public T DequeueMin()
   {
      T retVal = Min();

      if (count == 1)
      {
         heap.RemoveAt(1);
         count--;
         return retVal;
      }

      // move in a new min
      Node<T> last = heap[heap.Count - 1] as Node<T>;
      (heap[1] as Node<T>).low = last.low;

      // empty last node?
      if (count % 2 == 1)
      {
         heap.RemoveAt(heap.Count - 1);
      }
      else // move high down to low
      {
         last.low = last.high;
         last.high = null;
      }
      count--;

      // bubble down
      int index = 1;
      Node<T> current = heap[index] as Node<T>;
      while (true)
      {
         // no more children
         if (index * 2 >= heap.Count)
            break;

         int childIndex;
         if (index * 2 + 1 < heap.Count)
            childIndex = (comparer.Compare((heap[index * 2] as Node<T>).low, (heap[2 * index + 1] as Node<T>).low) < 0 ? index * 2 : index * 2 + 1);
         else
            childIndex = index * 2;

         // smaller than child means done
         Node<T> child = heap[childIndex] as Node<T>;
         if (comparer.Compare(current.low, child.low) < 0)
            break;

         T temp = child.low;
         child.low = current.low;
         current.low = temp;

         // check endpoints are correct
         if (child.high != null && comparer.Compare(child.low, child.high) > 0)
         {
            temp = child.low;
            child.low = child.high;
            child.high = temp;
         }

         // update and reloop
         index = childIndex;
         current = child;
      }

      return retVal;
   }

   public T DequeueMax()
   {
      T retVal = Max();

      if (count == 1)
      {
         heap.RemoveAt(1);
         count--;
         return retVal;
      }

      // move in a new max
      Node<T> last = heap[heap.Count - 1] as Node<T>;
      if (count % 2 == 1)
      {
         // odd number - grab from low and lose last node
         (heap[1] as Node<T>).high = last.low;
         heap.RemoveAt(heap.Count - 1);
      }
      else
      {
         (heap[1] as Node<T>).high = last.high;
         last.high = null;
      }
      count--;

      // bubble down
      int index = 1;
      Node<T> current = heap[index] as Node<T>;

      while (true)
      {
         // no children - done
         if (index * 2 >= heap.Count)
            break;

         int childIndex;
         if (index * 2 + 1 < heap.Count)
         {
            if (count % 2 == 1 && index * 2 + 1 == heap.Count - 1)
               childIndex = (comparer.Compare((heap[index * 2] as Node<T>).high, (heap[2 * index + 1] as Node<T>).low) > 0 ? index * 2 : index * 2 + 1);
            else
               childIndex = (comparer.Compare((heap[index * 2] as Node<T>).high, (heap[2 * index + 1] as Node<T>).high) > 0 ? index * 2 : index * 2 + 1);
         }
         else
         {
            childIndex = index * 2;
         }

         Node<T> child = heap[childIndex] as Node<T>;
         if (child.high == null)
         {
            if (comparer.Compare(child.low, current.high) < 0)
               break;

            T temp = child.low;
            child.low = current.high;
            current.high = temp;
         }
         else
         {
            if (comparer.Compare(child.high, current.high) < 0)
               break;

            T temp = child.high;
            child.high = current.high;
            current.high = temp;

            if (comparer.Compare(child.low, child.high) > 0)
            {
               temp = child.high;
               child.high = child.low;
               child.low = temp;
            }
         }

         index = childIndex;
         current = child;
      }

      return retVal;
   }
}

public class IntervalHeapStore<T> : IPriorityQueueStore<T>
{
   private class QueueComparer<N> : IComparer<Tuple<int, N>>
   {
      public int Compare(Tuple<int, N> left, Tuple<int, N> right)
      {
         return left.Item1.CompareTo(right.Item1);
      }
   }

   // IntervalHeap has O(log n) en/dequeue
   private IntervalHeap<Tuple<int, T>> store = new IntervalHeap<Tuple<int, T>>(new QueueComparer<T>());

   public void Enqueue(T value, int priority)
   {
      store.Add(new Tuple<int, T>(priority, value));
   }

   public T DequeueMin()
   {
      Tuple<int, T> min = store.DequeueMin();
      return min.Item2;
   }
       
   public T DequeueMax()
   {
      Tuple<int, T> max = store.DequeueMax();
      return max.Item2;
   }

   public int Count
   {
      get { return store.Count; }
   }
}

}
