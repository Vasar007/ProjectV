using System;

namespace ProjectV.Crawlers
{
    /// <summary>
    /// Crawlers base interface.
    /// </summary>
    public interface ICrawlerBase : IDisposable, ITagable, ITypeId
    {
    }
}
