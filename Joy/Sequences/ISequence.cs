namespace AvP.Joy
{
    /// <summary>A stateless alternative to <see cref="System.Collections.Generic.IEnumerable{T}"/>/<see cref="System.Collections.Generic.IEnumerator{T}"/>.</summary>
    /// <typeparam name="T">The type of objects in the sequence.</typeparam>
    public interface ISequence<out T>
    {
        /// <summary>Indicates whether the collection has any elements.</summary>
        bool Any { get; }

        /// <summary>The first element in the sequence.</summary>
        /// <exception cref="InvalidOperationException"><see cref="IsEmpty"/> is <see langword="true"/>.</exception>
        T Head { get; }

        /// <summary>Retrieves the remainder of the sequence.</summary>
        /// <returns>The remainder of the collection, or an empty <see cref="ISequence"/> if <see cref="Head"/> is the last element or <see cref="IsEmpty"/> is already <see langword="true"/>.</returns>
        ISequence<T> GetTail();
    }
}