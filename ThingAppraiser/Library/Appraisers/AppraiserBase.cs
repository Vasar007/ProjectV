using System;
using ThingAppraiser.Data;

namespace ThingAppraiser.Appraisers
{
    /// <summary>
    /// Appraiser base class.
    /// </summary>
    public class AppraiserBase : ITagable, ITypeId
    {
        #region ITagable Implementation

        /// <inheritdoc />
        public virtual string Tag { get; } = "AppraiserBase";

        #endregion

        #region ITypeId Implementation

        /// <summary>
        /// Defines which type of data objects this appraiser can process.
        /// </summary>
        public virtual Type TypeId { get; } = typeof(BasicInfo);

        #endregion

        /// <summary>
        /// Rating name which describes rating calculation.
        /// </summary>
        public virtual string RatingName { get; } = "AppraiserBase has no rating calculation";

        /// <summary>
        /// Specify rating ID for result.
        /// </summary>
        public Guid RatingId { get; set; }


        /// <summary>
        /// Creates instance with default values.
        /// </summary>
        protected AppraiserBase()
        {
        }
    }
}
