using System;
using System.Collections;
using System.Collections.Generic;

namespace ThingAppraiser.Models.Internal
{
    /// <summary>
    /// Class-aggregator for working with ratings. Represents a simple wrapper around
    /// <see cref="List{T}" /> where internal type is <see cref="ResultInfo" />.
    /// </summary>
    public sealed class ResultList : IEnumerable<ResultInfo>
    {
        /// <summary>
        /// Data container.
        /// </summary>
        private readonly List<ResultInfo> _rating = new List<ResultInfo>();


        /// <summary>
        /// Creates instance with default values.
        /// </summary>
        public ResultList()
        {
        }

        /// <inheritdoc cref="List{T}.Add" />
        public void Add(ResultInfo item)
        {
            _rating.Add(item);
        }

        /// <inheritdoc cref="List{T}.Sort()" />
        public void Sort()
        {
            _rating.Sort();
        }

        /// <inheritdoc cref="List{T}.Sort(Comparison{T})" />
        public void Sort(Comparison<ResultInfo> comparison)
        {
            _rating.Sort(comparison);
        }

        #region IEnumerable<CResultInfo> Implementation

        /// <inheritdoc />
        public IEnumerator<ResultInfo> GetEnumerator()
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
