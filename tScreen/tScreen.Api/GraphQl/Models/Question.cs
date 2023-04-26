#region Assembly System.Runtime, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.14\ref\net6.0\System.Runtime.dll
#endregion

#nullable enable


namespace System.Collections.Generic
{
    //
    // Summary:
    //     Defines methods to manipulate generic collections.
    //
    // Type parameters:
    //   T:
    //     The type of the elements in the collection.
    public interface ICollection<T> : IEnumerable<T>, IEnumerable
    {
        //
        // Summary:
        //     Gets the number of elements contained in the System.Collections.Generic.ICollection`1.
        //
        // Returns:
        //     The number of elements contained in the System.Collections.Generic.ICollection`1.
        int Count { get; }
        //
        // Summary:
        //     Gets a value indicating whether the System.Collections.Generic.ICollection`1
        //     is read-only.
        //
        // Returns:
        //     true if the System.Collections.Generic.ICollection`1 is read-only; otherwise,
        //     false.
        bool IsReadOnly { get; }

        //
        // Summary:
        //     Adds an item to the System.Collections.Generic.ICollection`1.
        //
        // Parameters:
        //   item:
        //     The object to add to the System.Collections.Generic.ICollection`1.
        //
        // Exceptions:
        //   T:System.NotSupportedException:
        //     The System.Collections.Generic.ICollection`1 is read-only.
        void Add(T item);
        //
        // Summary:
        //     Removes all items from the System.Collections.Generic.ICollection`1.
        //
        // Exceptions:
        //   T:System.NotSupportedException:
        //     The System.Collections.Generic.ICollection`1 is read-only.
        void Clear();
        //
        // Summary:
        //     Determines whether the System.Collections.Generic.ICollection`1 contains a specific
        //     value.
        //
        // Parameters:
        //   item:
        //     The object to locate in the System.Collections.Generic.ICollection`1.
        //
        // Returns:
        //     true if item is found in the System.Collections.Generic.ICollection`1; otherwise,
        //     false.
        bool Contains(T item);
        //
        // Summary:
        //     Copies the elements of the System.Collections.Generic.ICollection`1 to an System.Array,
        //     starting at a particular System.Array index.
        //
        // Parameters:
        //   array:
        //     The one-dimensional System.Array that is the destination of the elements copied
        //     from System.Collections.Generic.ICollection`1. The System.Array must have zero-based
        //     indexing.
        //
        //   arrayIndex:
        //     The zero-based index in array at which copying begins.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     array is null.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     arrayIndex is less than 0.
        //
        //   T:System.ArgumentException:
        //     The number of elements in the source System.Collections.Generic.ICollection`1
        //     is greater than the available space from arrayIndex to the end of the destination
        //     array.
        void CopyTo(T[] array, int arrayIndex);
        //
        // Summary:
        //     Removes the first occurrence of a specific object from the System.Collections.Generic.ICollection`1.
        //
        // Parameters:
        //   item:
        //     The object to remove from the System.Collections.Generic.ICollection`1.
        //
        // Returns:
        //     true if item was successfully removed from the System.Collections.Generic.ICollection`1;
        //     otherwise, false. This method also returns false if item is not found in the
        //     original System.Collections.Generic.ICollection`1.
        //
        // Exceptions:
        //   T:System.NotSupportedException:
        //     The System.Collections.Generic.ICollection`1 is read-only.
        bool Remove(T item);
    }
}