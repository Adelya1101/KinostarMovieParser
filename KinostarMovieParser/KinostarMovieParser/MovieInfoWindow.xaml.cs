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
    /// Interaction logic for MovieInfoWindow.xaml
    /// </summary>
    public partial class MovieInfoWindow : Window
    {
        public MovieInfoWindow(Movie movie)
        {
            InitializeComponent();

            Title = $"{movie.Name} - KINOSTAR";

            MovieNameText.Text = movie.Name;
            StartDateText.Text = movie.StartDate;
            EndDateText.Text = movie.EndDate;
            TimingText.Text = movie.Timing;
            DirectorText.Text = movie.Director;
            ActorsText.Text = movie.Actors;
            DecriptionText.Text = movie.Description;
            SessionsItemsControl.ItemsSource = movie.Sessions;
            PosterImage.Source = new BitmapImage(new Uri(movie.PosterURL));
        }
    }
}
