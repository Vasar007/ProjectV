namespace ThingAppraiser
{
    /// <summary>
    /// Contains option to transform value of type <typeparamref name="TIn" /> to 
    /// <typeparamref name="TOut" />.
    /// </summary>
    /// <typeparam name="TIn">Input data type.</typeparam>
    /// <typeparam name="TOut">Output data type.</typeparam>
    public interface IDataMapper<in TIn, out TOut>
    {
        /// <summary>
        /// Transforms object to output data type. The passed parameter is usually not modified.
        /// </summary>
        /// <param name="dataObject">Object to transform.</param>
        /// <returns>Created object based on data from <paramref name="dataObject" />.</returns>
        TOut Transform(TIn dataObject);
    }
}
