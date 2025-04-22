using System.Collections;
internal class Node<T>
    {
        public Node (T data)
        {
            Data = data;
        }
        public T Data { get; set; }
        public Node<T>? Next { get; set; }
    }
public class LinkedList<T> : IEnumerable<T>
    {
        Node<T>? head;
        Node<T>? tail;
        int count;
        public void Add(T data)
        {
            Node<T> node = new Node<T>(data);

            if (head == null)
                head = node;
            else
                tail!.Next = node;
            tail = node;
            count++;
        }
        public void Sort()
        {
            if (head == null) return;
            bool swapped;
            do
            {
                swapped = false;
                Node<T> current = head;
                while (current.Next != null)
                {
                    if (Convert.ToInt32(current.Data) > Convert.ToInt32(current.Next.Data))
                    {
                        var temp = current.Data;
                        current.Data = current.Next.Data;
                        current.Next.Data = temp;
                        swapped = true;
                    }
                    current = current.Next;
                }
            } while (swapped);
        }
        public bool Remove(T data)
        {
            Node<T>? current = head;
            Node<T>? previous = null;

            while (current != null && current.Data != null)
            {
                if (current.Data.Equals(data))
                {
                    if (previous != null)
                    {
                        previous.Next = current.Next;
                        if (current.Next == null)
                            tail = previous;
                    }
                    else
                    {
                        head = head?.Next;
                        if (head == null)
                            tail = null;
                    }
                    count--;
                    return true;
                }

                previous = current;
                current = current.Next;
            }
            return false;
        }

        public int Count { get { return count; } }
        public void Clear()
        {
            head = null;
            tail = null;
            count = 0;
        }
        
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            Node<T>? current = head;
            while (current != null)
            {
                yield return current.Data;
                current = current.Next;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }
    }