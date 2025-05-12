using System;
using System.Collections.ObjectModel;

namespace TestBenchTarget.UWP.Models
{
    public class CustomObservableCollection<T> : ObservableCollection<T>
    {
        // Vždy vloží položku na začiatok kolekcie
        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(0, item);
        }

        // Pridajte explicitnú implementáciu Remove pre debugging
        public new bool Remove(T item)
        {
            int index = IndexOf(item);
            System.Diagnostics.Debug.WriteLine($"Remove called, Item found at index: {index}");

            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }
            return false;
        }
    }
}
