namespace ThingAppraiser
{
    /// <summary>
    /// Contains common methods for manager classes.
    /// </summary>
    /// <typeparam name="T">The type of the manager subordinate.</typeparam>
    public interface IManager<T>
        where T : ITagable
    {
        /// <summary>
        /// Adds item to current manager.
        /// </summary>
        /// <param name="item">The item to add.</param>
        void Add(T item);

        /// <summary>
        /// Removes items from current manager.
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <returns>
        /// <c>true</c> if <paramref name="item" /> is successfully removed, otherwise, 
        /// <c>false</c>. This method also returns false if <paramref name="item" /> was not found 
        /// in current manager.
        /// </returns>
        bool Remove(T item);
    }
}
