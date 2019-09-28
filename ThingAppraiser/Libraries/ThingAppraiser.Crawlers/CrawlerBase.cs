using System;

namespace ThingAppraiser.Crawlers
{
    /// <summary>
    /// Crawlers base interface.
    /// </summary>
    public interface ICrawlerBase : IDisposable, ITagable, ITypeId
    {
    }
}
