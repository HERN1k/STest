using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace STest.App.Utilities
{
    public class ExtendedObservableCollection<T> : ObservableCollection<T>
    {
        public void AddRange(IEnumerable<T> items)
        {
            if (items == null || !items.Any())
            {
                return;
            }

            foreach (T item in items)
            {
                Items.Add(item);
            }
            
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}