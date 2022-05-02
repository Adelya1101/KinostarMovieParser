using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinostarMovieParser.Models
{
    /// <summary>
    /// Сущность сеанса
    /// </summary>
    public class Session
    {
        /// <summary>
        /// Время начала сеанса
        /// </summary>
        public TimeSpan StartTime { get; set; }

        /// <summary>
        /// Стоимость
        /// </summary>
        public int Price { get; set; }

        /// <summary>
        /// Доступность сеанса. Сеанас доступен, если его время ещё не наступило
        /// </summary>
        public bool IsAvailable => DateTime.Now.TimeOfDay < StartTime;
    }
}
