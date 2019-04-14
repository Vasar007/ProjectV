using System;

namespace ThingAppraiser
{
    /// <summary>
    /// Adds tag name to class instance.
    /// </summary>
    public interface ITagable
    {
        /// <summary>
        /// Instance tag.
        /// </summary>
        String Tag { get; }
    }
}
