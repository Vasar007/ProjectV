﻿using System.Linq;
using ProjectV.TmdbService.Models;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;

namespace ProjectV.TmdbService.Mappers
{
    internal sealed class DataMapperTmdbContainer :
        IDataMapper<SearchContainer<SearchMovie>, TmdbSearchContainer>
    {
        /// <summary>
        /// Helper class to transform raw DTO objects to concrete object without extra data.
        /// </summary>
        private readonly DataMapperTmdbMovie _mapperTmdbMovie = new DataMapperTmdbMovie();


        public DataMapperTmdbContainer()
        {
        }

        #region IDataMapper<SearchContainer<SearchMovie>, TmdbSearchContainer> Implementation

        public TmdbSearchContainer Transform(SearchContainer<SearchMovie> dataObject)
        {
            var results = dataObject.Results
                .Select(tmdb => _mapperTmdbMovie.Transform(tmdb))
                .ToList();

            return new TmdbSearchContainer(
                page: dataObject.Page,
                results: results,
                totalPages: dataObject.TotalPages,
                totalResults: dataObject.TotalResults
            );
        }

        #endregion
    }
}
