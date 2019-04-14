using System;

namespace ThingAppraiser.IO
{
    /// <summary>
    /// Helper class to work with Google Drive parameters.
    /// </summary>
    public class CGoogleDriveFilesListOptionalParams
    {
        /// <summary>
        /// The source of files to list.
        /// </summary>
        public String Corpora { get; set; }

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
        public String OrderBy { get; set; }

        /// <summary>
        /// The maximum number of files to return per page.
        /// </summary>
        public Int32? PageSize { get; set; }

        /// <summary>
        /// The token for continuing a previous list request on the next page. This should be set
        /// to the value of 'nextPageToken' from the previous response.
        /// </summary> 
        public String PageToken { get; set; }

        /// <summary>
        /// A query for filtering the file results. See the "Search for Files" guide for supported
        /// syntax.
        /// </summary>
        public String Q { get; set; }

        /// <summary>
        /// A comma-separated list of spaces to query within the corpus. Supported values are
        /// 'drive', 'appDataFolder' and 'photos'.
        /// </summary>
        public String Spaces { get; set; }

        /// <summary>
        /// Selector specifying a subset of fields to include in the response.
        /// </summary>
        public String Fields { get; set; }

        /// <summary>
        /// Alternative to userIp.
        /// </summary>
        public String QuotaUser { get; set; }

        /// <summary>
        /// IP address of the end user for whom the API call is being made.
        /// </summary>
        public String UserIp { get; set; }
    }
}
