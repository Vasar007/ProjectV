using System;
using System.Collections.Generic;

namespace ThingAppraiser.Crawlers
{
    public class TMDBCrawler : Crawler<TMDBMovie>
    {
        private const string APIKey = "";

        public override string GetSearchQueryString(string movieName)
        {
            return $"https://api.themoviedb.org/3/search/movie?query={movieName}&api_key={APIKey}";
        }

        public override List<TMDBMovie> GetData(string[] movies, bool ouput = false)
        {
            List<TMDBMovie> searchResults = new List<TMDBMovie>();
            foreach (var movie in movies)
            {
                var response = GetSearchResult(SendSearchQuery(movie));

                // Get first search result from response.
                var result = response["results"][0];
                if (ouput)
                {
                    Console.WriteLine(result);
                }
                // JToken.ToObject is a helper method that uses JsonSerializer internally
                var searchResult = result.ToObject<TMDBMovie>();
                searchResults.Add(searchResult);
            }
            return searchResults;
        }
    }
}
