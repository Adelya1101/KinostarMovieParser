using HtmlAgilityPack;
using KinostarMovieParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace KinostarMovieParser
{
    /// <summary>
    /// Осущеставляет парсинг фильмов с сайта https://kinostar86.ru
    /// </summary>
    public class KinostarParser
    {
        private const string KINOSTAR_URL = "https://kinostar86.ru";

        /// <summary>
        /// Получить список фильмов на сегодня
        /// </summary>
        public async Task<List<Movie>> GetMovies()
        {
            return await Task.Run(() =>
            {
                List<Movie> movies = new List<Movie>();

                try
                {
                    HtmlWeb loader = new HtmlWeb();

                    HtmlDocument mainPageDoc = loader.Load(KINOSTAR_URL);

                    // Вытаскиваем все фильмы за сегодня
                    var movieNodes = mainPageDoc.DocumentNode.SelectNodes("//div[@class='sc-yf63q6-3 kNdiSE event rental large']");

                    if (movieNodes == null || movieNodes.Count == 0) return new List<Movie>();

                    // Проходим на страницу каждого из фильмов и вытаскиваем инфо о нём
                    foreach (var movie in movieNodes)
                    {
                        var currentMovie = new Movie();

                        currentMovie.Id = Guid.NewGuid();

                        var movieLinkNode = movie.SelectSingleNode(".//a[@class='event-name']");
                        if (movieLinkNode == null) continue;

                        currentMovie.Name = movieLinkNode.InnerText;

                        var movieLink = movieLinkNode.GetAttributeValue("href", string.Empty);
                        if (string.IsNullOrEmpty(movieLink)) continue;

                        HtmlDocument currentMovieDoc = loader.Load($"{KINOSTAR_URL}{movieLink}");

                        var movieInfoNodes = currentMovieDoc.DocumentNode.SelectNodes("//div[@class='sc-jp24ki-1 qqVem']");

                        foreach (var movieInfoNode in movieInfoNodes)
                        {
                            var currentNodeTitle = movieInfoNode.SelectSingleNode(".//div[@class='sc-jp24ki-2 hGZZBS']")?.InnerText;
                            var currentNodeInfo = movieInfoNode.SelectSingleNode(".//div[@class='sc-jp24ki-3 daPpud']")?.InnerText;

                            if (string.IsNullOrEmpty(currentNodeTitle) || string.IsNullOrEmpty(currentNodeInfo)) continue;

                            switch (currentNodeTitle)
                            {
                                case "В прокате с":
                                    currentMovie.StartDate = currentNodeInfo;
                                    break;
                                case "В прокате до":
                                    currentMovie.EndDate = currentNodeInfo;
                                    break;
                                case "Хронометраж":
                                    currentMovie.Timing = currentNodeInfo;
                                    break;
                                case "Режиссер":
                                    currentMovie.Director = currentNodeInfo;
                                    break;
                                case "В ролях":
                                    currentMovie.Actors = currentNodeInfo;
                                    break;
                            }
                        }

                        // вытягиваем активные
                        var showNodes = currentMovieDoc.DocumentNode.SelectNodes("//div[@class='sc-sw9zb-2 iknTqF show']");

                        ParseShows(currentMovie, showNodes);

                        // вытягиваем неактивные сеансы
                        var disabledShowsNodes = currentMovieDoc.DocumentNode.SelectNodes("//div[@class='sc-sw9zb-2 iknTqF show disabled']");

                        ParseShows(currentMovie, disabledShowsNodes);                      

                        var movieDescription = currentMovieDoc.DocumentNode.SelectSingleNode("//div[@class='sc-rnk5eh-4 jXZuVv']")?.InnerText;
                        currentMovie.Description = movieDescription;

                        var moviewPosterNode = currentMovieDoc.DocumentNode.SelectSingleNode("//div[@class='sc-hq414j-1 dswmJi event-poster']");
                        var moviewPosterUrl = moviewPosterNode?.SelectSingleNode(".//img")?.GetAttributeValue("src", string.Empty)?.Replace("22x32", "540x800").Replace(":blur(2)", string.Empty);

                        currentMovie.PosterURL = moviewPosterUrl;

                        movies.Add(currentMovie);
                    }

                    return movies;
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Произошла ошибка при считывании информации о фильмах: {e.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return movies;
                }
            });
        }

        /// <summary>
        /// Парсинг сеансов
        /// </summary>
        private void ParseShows(Movie currentMovie, HtmlNodeCollection showNodes)
        {
            if (showNodes != null)
            {
                currentMovie.Sessions = currentMovie.Sessions ?? new List<Session>();

                foreach (var showNode in showNodes)
                {
                    var showTime = showNode.SelectSingleNode(".//div[@class='show-time']")?.InnerText;
                    var showPrice = showNode.SelectSingleNode(".//div[@class='sc-sw9zb-0 jENqIc price']")?.InnerText;

                    if (string.IsNullOrEmpty(showTime) || string.IsNullOrEmpty(showPrice)) continue;

                    var time = TimeSpan.Parse(showTime);

                    // убираем все символы из строки с ценой, кроме цифр
                    var price = int.Parse(Regex.Replace(showPrice, @"[^\d]", string.Empty));

                    currentMovie.Sessions.Add(new Session()
                    {
                        StartTime = time,
                        Price = price
                    });
                }
            }
        }
    }
}
