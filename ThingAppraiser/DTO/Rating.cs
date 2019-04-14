using System;
using System.Collections;
using System.Collections.Generic;

namespace ThingAppraiser.Data
{
    /// <summary>
    /// Class-aggregator for working with ratings. represents a simple wrapper around
    /// <see cref="List{T}" /> where internal type is <see cref="CResultInfo" />.
    /// </summary>
    public class CRating : IEnumerable<CResultInfo>
    {
        /// <summary>
        /// Data container.
        /// </summary>
        private readonly List<CResultInfo> _rating = new List<CResultInfo>();


        /// <summary>
        /// Default constructor.
        /// </summary>
        public CRating()
        {
        }

        /// <inheritdoc cref="List{T}.Add" />
        public void Add(CResultInfo item)
        {
            _rating.Add(item);
        }

        /// <inheritdoc cref="List{T}.Sort()" />
        public void Sort()
        {
            _rating.Sort();
        }

        /// <inheritdoc cref="List{T}.Sort(Comparison{T})" />
        public void Sort(Comparison<CResultInfo> comparison)
        {
            _rating.Sort(comparison);
        }

        #region IEnumerable<CResultInfo> Implementation

        /// <inheritdoc />
        public IEnumerator<CResultInfo> GetEnumerator()
        {
            return _rating.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _rating.GetEnumerator();
        }

        #endregion
    }
}
