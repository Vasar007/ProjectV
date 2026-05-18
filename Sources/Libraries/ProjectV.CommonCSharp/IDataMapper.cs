namespace ProjectV
{
    /// <summary>
    /// Contains option to transform value of type <typeparamref name="TIn" /> to 
    /// <typeparamref name="TOut" />.
    /// </summary>
    /// <typeparam name="TIn">The input data type.</typeparam>
    /// <typeparam name="TOut">The output data type.</typeparam>
    public interface IDataMapper<TIn, TOut>
    {
        /// <summary>
        /// Transforms object to output data type. The passed parameter is usually not modified.
        /// </summary>
        /// <param name="dataObject">Object to transform.</param>
        /// <returns>Created object based on data from <paramref name="dataObject" />.</returns>
        TOut Transform(TIn dataObject);
    }
}
