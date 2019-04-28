using System;

namespace ThingAppraiser.FuzzyLogicSystem
{
    /// <summary>
    /// Provides type safe interface for MATLAB script.
    /// </summary>
    /// <remarks>This interface binds controller from MATLAB Fuzzy Logic Toolbox and .NET.</remarks>
    public interface IFuzzyController
    {
        /// <summary>
        /// Calculates movie rating based on specified parameters.
        /// </summary>
        /// <param name="voteCount">Vote count of movie.</param>
        /// <param name="voteAverage">Average vote of movie.</param>
        /// <param name="releaseYear">Year of the movie release date.</param>
        /// <param name="popularity">Value of movie popularity metric.</param>
        /// <param name="adult">If movie is adult only.</param>
        /// <returns>Movie rating which calculates in MATLAB module</returns>
        /// <remarks>MATLAB accepts only <c>single</c> or <c>double</c>> values.</remarks>
        Double CalculateRating(Double voteCount, Double voteAverage, Double releaseYear,
            Double popularity, Double adult);
    }
}
