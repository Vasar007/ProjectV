using System;

namespace ThingAppraiser
{
    /// <summary>
    /// Adds type id to track which types can be processed by instance.
    /// </summary>
    public interface ITypeID
    {
        /// <summary>
        /// Type of the data structure to process.
        /// </summary>
        Type TypeID { get; }
    }
}
