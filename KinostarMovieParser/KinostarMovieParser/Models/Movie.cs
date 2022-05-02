using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinostarMovieParser.Models
{
    /// <summary>
    /// Информация о фильме
    /// </summary>
    public class Movie
    {
        /// <summary>
        /// ID Фильма (внутренний)
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название фильма
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Дата начала проката
        /// </summary>
        public string StartDate { get; set; }

        /// <summary>
        /// Дата окончания проката
        /// </summary>
        public string EndDate { get; set; }

        /// <summary>
        /// Хронометраж
        /// </summary>
        public string Timing { get; set; }

        /// <summary>
        /// Режиссёр
        /// </summary>
        public string Director { get; set; }

        /// <summary>
        /// Актёры
        /// </summary>
        public string Actors { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Короткое описание для главной страницы
        /// </summary>
        public string ShortDescription => string.IsNullOrEmpty(Description) ? string.Empty : Description.Length > 150 ? $"{Description.Substring(0, 150)}..." : Description;

        /// <summary>
        /// Ссылка на постер
        /// </summary>
        public string PosterURL { get; set; }

        /// <summary>
        /// Список сансов
        /// </summary>
        public List<Session> Sessions { get; set; }
    }
}
