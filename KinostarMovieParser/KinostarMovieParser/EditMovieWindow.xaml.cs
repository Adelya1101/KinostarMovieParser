using KinostarMovieParser.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KinostarMovieParser
{
    /// <summary>
    /// Interaction logic for EditMovieWindow.xaml
    /// </summary>
    public partial class EditMovieWindow : Window
    {
        /// <summary>
        /// Изменённый объект фильма
        /// </summary>
        public Movie Movie { get; private set; }

        public EditMovieWindow(Movie movie)
        {
            InitializeComponent();

            Title = $"Редактирование - {movie.Name}";

            // работаем не с оригинальным объектом, а с копией, чтобы в оригинальном объекте не изменялись параметры
            var movieCopy = JsonConvert.DeserializeObject<Movie>(JsonConvert.SerializeObject(movie));
            Movie = movieCopy;

            MovieNameText.Text = movieCopy.Name;
            StartDateText.Text = movieCopy.StartDate;
            EndDateText.Text = movieCopy.EndDate;
            TimingText.Text = movieCopy.Timing;
            DirectorText.Text = movieCopy.Director;
            ActorsText.Text = movieCopy.Actors;
            DecriptionText.Text = movieCopy.Description;
            SessionsItemsControl.ItemsSource = movieCopy.Sessions;
            PosterImage.Source = new BitmapImage(new Uri(movieCopy.PosterURL));
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Movie.Name = MovieNameText.Text;
            Movie.StartDate = StartDateText.Text;
            Movie.EndDate = EndDateText.Text;
            Movie.Timing = TimingText.Text;
            Movie.Director = DirectorText.Text;
            Movie.Actors = ActorsText.Text;
            Movie.Description = DecriptionText.Text;

            DialogResult = true;
        }
    }
}
