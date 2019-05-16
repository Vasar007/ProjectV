namespace ThingAppraiser.Data
{
    /// <summary>
    /// Collections of status values.
    /// </summary>
    public enum ServiceStatus
    {
        /// <summary>
        /// Service didn't get any results when processing the data.
        /// </summary>
        Nothing,

        /// <summary>
        /// Successful work status.
        /// </summary>
        Ok,

        /// <summary>
        /// Unexpected exception occured.
        /// </summary>
        Error,

        /// <summary>
        /// Exception had occured during input component work.
        /// </summary>
        InputError,

        /// <summary>
        /// Exception had occured during crawlers component work.
        /// </summary>
        RequestError,

        /// <summary>
        /// Exception had occured during appraisers component work.
        /// </summary>
        AppraiseError,

        /// <summary>
        /// Exception had occured during output component work.
        /// </summary>
        OutputError,

        /// <summary>
        /// Non fatal problems with saving results.
        /// </summary>
        OutputUnsaved
    }
}
