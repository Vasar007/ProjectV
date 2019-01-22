using System;
using System.Collections.Generic;

namespace FilmsEvaluator.Crawlers
{
    public class TMDBCrawler : Crawler<TMDBMovie>
    {
        private const string APIKey = "";

        public override string GetSearchQueryString(string filmName)
        {
            return $"https://api.themoviedb.org/3/search/movie?query={filmName}&api_key={APIKey}";
        }

        public override List<TMDBMovie> GetFilmsInfo(string[] films, bool ouput = false)
        {
            List<TMDBMovie> searchResults = new List<TMDBMovie>();
            foreach (var film in films)
            {
                var response = GetSearchResult(SendSearchQuery(film));

                // Get first search result from response.
                var result = response["results"][0];
                if (ouput)
                {
                    Console.WriteLine(result.ToString());
                }
                // JToken.ToObject is a helper method that uses JsonSerializer internally
                var searchResult = result.ToObject<TMDBMovie>();
                searchResults.Add(searchResult);
            }
            return searchResults;
        }
    }
}
