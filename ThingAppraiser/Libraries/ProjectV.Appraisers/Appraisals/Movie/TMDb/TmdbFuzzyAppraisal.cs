using System;
using Acolyte.Assertions;
using ProjectV.FuzzyLogicSystem;
using ProjectV.Models.Data;
using ProjectV.Models.Internal;

namespace ProjectV.Appraisers.Appraisals.Movie.Tmdb
{
    /// <summary>
    /// Appraisal to get rating for instances of <see cref="TmdbMovieInfo" />.
    /// </summary>
    public sealed class TmdbFuzzyAppraisal : IAppraisal<TmdbMovieInfo>
    {
        /// <summary>
        /// Fuzzy Controller which binds .NET interface with MATLAB module.
        /// </summary>
        private readonly IFuzzyController _fuzzyController = new FuzzyControllerIFuzzyController();

        /// <inheritdoc />
        public string RatingName { get; } = "Fuzzy logic rating";


        /// <summary>
        /// Initializes instance with default values.
        /// </summary>
        public TmdbFuzzyAppraisal()
        {
        }

        #region IAppraisal<TmdbMovieInfo> Implementation

        /// <summary>
        /// No exctraction will be perfomed because this appraisal no needed in such preparation.
        /// </summary>
        public void PrepareCalculation(RawDataContainer rawDataContainer)
        {
            // Nothing to do.
        }

        /// <summary>
        /// Calculates rating for <see cref="TmdbMovieInfo" /> with specified values.
        /// </summary>
        /// <param name="entity">Target value to calculate rating.</param>
        /// <returns>
        /// Result of execution MATLAB fuzzy controller.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="entity" /> is <c>null</c>.
        /// </exception>
        public double CalculateRating(TmdbMovieInfo entity)
        {
            entity.ThrowIfNull(nameof(entity));

            double ratingValue = _fuzzyController.CalculateRating(
                voteCount:   entity.VoteCount,
                voteAverage: entity.VoteAverage,
                releaseYear: entity.ReleaseDate.Year,
                popularity:  entity.Popularity,
                adult:       entity.Adult ? 1 : 0
            );

            return ratingValue;
        }

        #endregion
    }
}
