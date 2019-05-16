namespace ThingAppraiser.IO
{
    /// <summary>
    /// Helper class to work with Google Drive parameters.
    /// </summary>
    public class GoogleDriveFilesListOptionalParams
    {
        /// <summary>
        /// The source of files to list.
        /// </summary>
        public string Corpora { get; set; }

        /// <summary>
        /// A comma-separated list of sort keys. Valid keys are 'createdTime', 'folder',
        /// 'modifiedByMeTime', 'modifiedTime', 'name', 'quotaBytesUsed', 'recency',
        /// 'sharedWithMeTime', 'starred', and 'viewedByMeTime'. Each key sorts ascending by
        /// default, but may be reversed with the 'desc' modifier.
        /// </summary>
        /// <example>
        /// ?orderBy=folder,modifiedTime desc,name
        /// </example>
        /// <remarks>
        /// Please note that there is a current limitation for users with approximately one million
        /// files in which the requested sort order is ignored.
        /// </remarks>
        public string OrderBy { get; set; }

        /// <summary>
        /// The maximum number of files to return per page.
        /// </summary>
        public int? PageSize { get; set; }

        /// <summary>
        /// The token for continuing a previous list request on the next page. This should be set
        /// to the value of 'nextPageToken' from the previous response.
        /// </summary> 
        public string PageToken { get; set; }

        /// <summary>
        /// A query for filtering the file results. See the "Search for Files" guide for supported
        /// syntax.
        /// </summary>
        public string Q { get; set; }

        /// <summary>
        /// A comma-separated list of spaces to query within the corpus. Supported values are
        /// 'drive', 'appDataFolder' and 'photos'.
        /// </summary>
        public string Spaces { get; set; }

        /// <summary>
        /// Selector specifying a subset of fields to include in the response.
        /// </summary>
        public string Fields { get; set; }

        /// <summary>
        /// Alternative to userIp.
        /// </summary>
        public string QuotaUser { get; set; }

        /// <summary>
        /// IP address of the end user for whom the API call is being made.
        /// </summary>
        public string UserIp { get; set; }


        /// <summary>
        /// Creates instance with default values.
        /// </summary>
        public GoogleDriveFilesListOptionalParams()
        {
        }
    }
}
