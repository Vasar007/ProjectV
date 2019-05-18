using System;
using Newtonsoft.Json.Linq;
using RestSharp;
using ThingAppraiser.Data;

namespace ThingAppraiser.Crawlers
{
    /// <summary>
    /// Crawlers base class.
    /// </summary>
    public abstract class CrawlerBase : ITagable, ITypeId
    {
        #region ITagable Implementation

        /// <inheritdoc />
        public virtual string Tag { get; } = "CrawlerBase";

        #endregion

        #region ITypeId Implementation

        /// <summary>
        /// Defines which type of data objects this crawler can produce.
        /// </summary>
        public virtual Type TypeId { get; } = typeof(BasicInfo);

        #endregion


        /// <summary>
        /// Creates instance with default values.
        /// </summary>
        protected CrawlerBase()
        {
        }

        /// <summary>
        /// Extracts result from REST response.
        /// </summary>
        /// <param name="response">Response to process.</param>
        /// <returns>Parsed response content in JSON data format.</returns>
        protected virtual JObject GetResponseResult(IRestResponse response)
        {
            JObject parsedJson = JObject.Parse(response.Content);
            return parsedJson;
        }
    }
}
