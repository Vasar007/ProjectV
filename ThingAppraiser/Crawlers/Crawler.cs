using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RestSharp;
using ThingAppraiser.Data;

namespace ThingAppraiser.Crawlers
{
    /// <summary>
    /// Crawlers base class. You should inherit this class if would like to create your own crawler.
    /// </summary>
    public abstract class CCrawler : ITagable, ITypeID
    {
        #region ITagable Implementation

        /// <inheritdoc />
        public virtual String Tag => "Crawler";

        #endregion

        #region ITypeID Implementation

        /// <summary>
        /// Defines which type of data objects this crawler can produce.
        /// </summary>
        public virtual Type TypeID => typeof(CBasicInfo);

        #endregion


        /// <summary>
        /// Default constructor.
        /// </summary>
        public CCrawler()
        {
        }

        /// <summary>
        /// Gets response from data storage and process it.
        /// </summary>
        /// <param name="entities">Names to find data.</param>
        /// <param name="outputResults">Flag to define need to output.</param>
        /// <returns>Transformed data to appraise.</returns>
        /// <remarks>
        /// Response collection must be unique because rating calculation errors can occur in such
        /// situations.
        /// </remarks>
        public abstract List<CBasicInfo> GetResponse(List<String> entities, Boolean outputResults);

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
