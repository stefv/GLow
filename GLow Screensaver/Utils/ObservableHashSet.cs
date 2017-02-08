//
// GLow screensaver
// Copyright(C) Stéphane VANPOPERYNGHE
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or(at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
//

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace GLow_Screensaver.Utils
{
    /// <summary>
    /// An observable hashset.
    /// </summary>
    /// <typeparam name="T">The type applied to the observable hashset.</typeparam>
    public sealed class ObservableHashSet<T> : ISet<T>, INotifyCollectionChanged
    {
        /// <summary>
        /// Event when the collection is changed.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// The internal set.
        /// </summary>
        private HashSet<T> _innerSet = new HashSet<T>();

        /// <summary>
        /// The number of elements in the set.
        /// </summary>
        public int Count { get { return _innerSet.Count; } }

        /// <summary>
        /// Read/Write collection.
        /// </summary>
        public bool IsReadOnly { get { return false; } }

        /// <summary>
        /// Raise an event when the collection is changed.
        /// </summary>
        /// <param name="e">Argument for this event.</param>
        private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null) CollectionChanged(this, e);
        }

        /// <summary>
        /// Add an element.
        /// </summary>
        /// <param name="item">The element to add.</param>
        /// <returns>true if the element is added.</returns>
        public bool Add(T item)
        {
            bool result = _innerSet.Add(item);
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            return result;
        }

        /// <summary>
        /// Clear the set.
        /// </summary>
        public void Clear()
        {
            _innerSet.Clear();
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Check if the set contains the item.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>true if the set contains the item.</returns>
        public bool Contains(T item)
        {
            return _innerSet.Contains(item);
        }

        /// <summary>
        /// Copy the items to an array.
        /// </summary>
        /// <param name="array">The array where to copy the items.</param>
        /// <param name="arrayIndex">The index in the array where to start the copy.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            _innerSet.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Delete the given items.
        /// </summary>
        /// <param name="other">The items to delete.</param>
        public void ExceptWith(IEnumerable<T> other)
        {
            _innerSet.ExceptWith(other);
        }

        /// <summary>
        /// Return an enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _innerSet.GetEnumerator();
        }

        /// <summary>
        /// Change the set to have only the elements from other.
        /// </summary>
        /// <param name="other">The items to keep.</param>
        public void IntersectWith(IEnumerable<T> other)
        {
            _innerSet.IntersectWith(other);
            // TODO Add event
        }

        /// <summary>
        /// Check is the set is a proper subset of other.
        /// </summary>
        /// <param name="other">The other items.</param>
        /// <returns>True if it's a proper subset.</returns>
        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return _innerSet.IsProperSubsetOf(other);
        }

        /// <summary>
        /// Check if the set is a subset of other.
        /// </summary>
        /// <param name="other">The other items.</param>
        /// <returns>true if it's a subset.</returns>
        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return _innerSet.IsProperSupersetOf(other);
        }

        /// <summary>
        /// Check is the set is a subset of other.
        /// </summary>
        /// <param name="other">The other items.</param>
        /// <returns>True if it's a subset.</returns>
        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return _innerSet.IsSubsetOf(other);
        }

        /// <summary>
        /// Check if the set is a superset of other.
        /// </summary>
        /// <param name="other">The other items.</param>
        /// <returns>true if it's a superset.</returns>
        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return _innerSet.IsSupersetOf(other);
        }

        /// <summary>
        /// Check if the set and the other share items.
        /// </summary>
        /// <param name="other">The other set.</param>
        /// <returns>True if the set and the other share items.</returns>
        public bool Overlaps(IEnumerable<T> other)
        {
            return _innerSet.Overlaps(other);
        }

        /// <summary>
        /// Remove the item.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>true if the remove is ok.</returns>
        public bool Remove(T item)
        {
            bool result =  _innerSet.Remove(item);
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            return result;
        }

        /// <summary>
        /// Check if the sets have the same items.
        /// </summary>
        /// <param name="other">The other set.</param>
        /// <returns>true if the are equals.</returns>
        public bool SetEquals(IEnumerable<T> other)
        {
            return _innerSet.SetEquals(other);
        }

        /// <summary>
        /// Change the set to have only unique items in both sets.
        /// </summary>
        /// <param name="other">The other set.</param>
        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            _innerSet.SymmetricExceptWith(other);
        }

        /// <summary>
        /// Merge the 2 sets in the set.
        /// </summary>
        /// <param name="other">The other set.</param>
        public void UnionWith(IEnumerable<T> other)
        {
            _innerSet.UnionWith(other);
        }

        /// <summary>
        /// Add the given item to a collection.
        /// </summary>
        /// <param name="item"></param>
        void ICollection<T>.Add(T item)
        {
            _innerSet.Add(item);
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        /// <summary>
        /// Return an enumerator.
        /// </summary>
        /// <returns>The enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _innerSet.GetEnumerator();
        }
    }
}
