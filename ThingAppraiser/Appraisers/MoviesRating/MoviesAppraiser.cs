using System;
using System.Collections.Generic;
using ThingAppraiser.Data;

namespace ThingAppraiser.Appraisers
{
    /// <summary>
    /// Expands appraiser behavior and lets process movie data objects.
    /// </summary>
    public abstract class CMoviesAppraiser : CAppraiser
    {
        /// <inheritdoc />
        public override Type TypeID => typeof(CMovieInfo);


        /// <summary>
        /// Default constructor.
        /// </summary>
        public CMoviesAppraiser()
        {
        }

        #region CAppraiser Overridden Methods

        /// <inheritdoc />
        /// <remarks>This method doesn't change default calculations.</remarks>
        public override CRating GetRatings(List<CBasicInfo> entities, Boolean outputResults)
        {
            return base.GetRatings(entities, outputResults);
        }

        #endregion
    }
}
