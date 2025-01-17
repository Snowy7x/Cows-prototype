﻿using System.Collections;
using System.Collections.Generic;

namespace _Core.Internal
{
    /// <summary>Generic list with O(1) insertion and removal.</summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>Adding duplicate values and removing items that are not present are both no-op.</item>
    ///   <item>
    ///     Items may be added and removed while the collection is being enumerated without raising exceptions.
    ///     This collection guarantees that all items are enumerated, even if insertions or removals happen during enumeration.
    ///   </item>
    ///   <item>
    ///     All enumerators reference the same list index, so avoid having more than one enumerator at the same time.
    ///     Creating a new enumerator resets this shared index.
    ///   </item>
    /// </list>
    /// </remarks>
    public class FastRemoveList<T> : IReadOnlyList<T>
    {
        private readonly List<T> _list = new List<T>();
        private readonly Dictionary<T, int> _indexMap = new Dictionary<T, int>();
        private int _loopIndex;

        /// <returns>Get the number of items contained in the <see cref="FastRemoveList{T}"/>.</returns>
        public int Count => _list.Count;

        /// <returns>Item registered at <paramref name="index"/> if index is valid, <see langword="default"/> otherwise.</returns>
        public T this[int index] => index >= 0 && index < Count ? _list[index] : default;

        /// <summary>Add <paramref name="value"/> to the end of the list.</summary>
        /// <returns><see langword="true"/> if value was not already present in the list and was successfully added, <see langword="false"/> otherwise.</returns>
        /// <remarks>
        /// Adding a value while the collection is being enumerated is permitted and won't raise exceptions.
        /// This operation is O(1).
        /// </remarks>
        public bool Add(T value)
        {
            if (_indexMap.ContainsKey(value))
            {
                return false;
            }

            _list.Add(value);
            _indexMap.Add(value, _list.Count - 1);
            return true;
        }

        /// <summary>Remove <paramref name="value"/> from the list, if found.</summary>
        /// <returns><see langword="true"/> if value was present in the list and was successfully removed, <see langword="false"/> otherwise.</returns>
        /// <remarks>
        /// Removing a value while the collection is being enumerated is permitted and won't raise exceptions.
        /// This operation is O(1).
        /// </remarks>
        public bool Remove(T value)
        {
            if (!_indexMap.TryGetValue(value, out int indexToRemove))
            {
                return false;
            }

            _indexMap.Remove(value);

            // If removing the object that was just enumerated, make sure the
            // new object swapped back to this index gets enumerated as well
            if (indexToRemove == _loopIndex)
            {
                _loopIndex--;
            }
            // If removing an object that was already enumerated while the loop is
            // still running, swap it with current loop index to make sure we
            // still enumerate the last element that will be swapped back later
            else if (indexToRemove < _loopIndex)
            {
                _list.Swap(_loopIndex, indexToRemove, out T swappedValue);
                _indexMap[swappedValue] = indexToRemove;
                indexToRemove = _loopIndex;
                _loopIndex--;
            }

            _list.RemoveAtSwapBack(indexToRemove, out T swappedBack);
            if (swappedBack != null)
            {
                _indexMap[swappedBack] = indexToRemove;
            }

            return true;
        }

        /// <summary>Removes all values from the <see cref="FastRemoveList{T}"/>.</summary>
        public void Clear()
        {
            _list.Clear();
            _indexMap.Clear();
        }

        /// <summary>Returns an enumerator that iterates through the <see cref="FastRemoveList{T}"/>.</summary>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct Enumerator : IEnumerator<T>
        {
            private FastRemoveList<T> _list;

            public Enumerator(FastRemoveList<T> list)
            {
                _list = list;
                Reset();
            }

            public T Current => _list[_list._loopIndex];
            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (_list._loopIndex < _list.Count - 1)
                {
                    _list._loopIndex++;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public void Reset()
            {
                _list._loopIndex = -1;
            }
        }
    }
    
    public static class ListExtensions
    {
        public static void RemoveAtSwapBack<T>(this IList<T> list, int index, out T swappedValue)
        {
            int lastIndex = list.Count - 1;
            if (lastIndex > 0 && lastIndex != index)
            {
                swappedValue = list[index] = list[lastIndex];
            }
            else
            {
                swappedValue = default;
            }
            list.RemoveAt(lastIndex);
        }

        public static void Swap<T>(this IList<T> list, int sourceIndex, int destinationIndex, out T newDestinationValue)
        {
            newDestinationValue = list[sourceIndex];
            list[sourceIndex] = list[destinationIndex];
            list[destinationIndex] = newDestinationValue;
        }
    }
}