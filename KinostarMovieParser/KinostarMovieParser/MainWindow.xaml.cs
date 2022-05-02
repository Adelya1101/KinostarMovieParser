using KinostarMovieParser.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace KinostarMovieParser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Все сохранённые фильмы
        /// </summary>
        private ObservableCollection<Movie> allMovies = new ObservableCollection<Movie>();

        private KinostarParser parser;

        private string moviesFilepath = Path.GetFullPath("movies.json");

        public MainWindow()
        {
            InitializeComponent();

            parser = new KinostarParser();

            allMovies = LoadMoviesFromFile();

            moviesItemsControl.ItemsSource = allMovies;

            // проставляем видимость тексту с информацией о том, что нет загруженных фильмов
            UpdateNoContentText(allMovies);
        }

        /// <summary>
        /// Обновить текст в центре экрана в зависимости от того, есть ли фильмы для отображения
        /// </summary>
        /// <param name="movies">Фильмы</param>
        /// <param name="text">Отображаемый текст</param>
        private void UpdateNoContentText(ObservableCollection<Movie> movies, string text = "Нет загруженных фильмов...\nНажмите 'Загрузить' для загрузки фильмов")
        {
            // проставляем видимость тексту с информацией о том, что нет загруженных фильмов
            NoContentText.Visibility = movies == null || !movies.Any() ? Visibility.Visible : Visibility.Collapsed;
            NoContentText.Text = text;
        }

        /// <summary>
        /// Загрузка фильмов из файлового хранилища
        /// </summary>
        private ObservableCollection<Movie> LoadMoviesFromFile()
        {
            try
            {
                // если файла с фильмами нет, то возвращаем пустой список
                if (!File.Exists(moviesFilepath)) return new ObservableCollection<Movie>();

                // читаем весь текст из файла
                var filedata = File.ReadAllText(moviesFilepath);

                // конвертируем json текст в объект List<Movie>
                return JsonConvert.DeserializeObject<ObservableCollection<Movie>>(filedata);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Ошибка при загрузке фильмов из файла: {e.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return new ObservableCollection<Movie>();
            }
        }

        /// <summary>
        /// Сохранить все текущие фильмы в файл
        /// </summary>
        private void SaveMoviesToFile()
        {
            try
            {
                // получаем JSON текст из объекта allMovies
                var jsonText = JsonConvert.SerializeObject(allMovies, Formatting.Indented);

                // записываем полученный JSON в файл
                File.WriteAllText(moviesFilepath, jsonText);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Ошибка при сохранении фильмов в файл: {e.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Обработка нажатия на кнопку Загрузить
        /// </summary>
        private async void LoadMoviesButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshButton.IsEnabled = false;
            LoadingImg.Visibility = Visibility.Visible;

            var movies = await parser.GetMovies();

            // сохраняем результат в файл только если что-то получили от парсера
            if (movies != null && movies.Count > 0)
            {
                allMovies = new ObservableCollection<Movie>(movies);

                SaveMoviesToFile();
            }

            moviesItemsControl.ItemsSource = allMovies;

            NoContentText.Visibility = allMovies.Any() ? Visibility.Collapsed : Visibility.Visible;

            UpdateNoContentText(allMovies);

            RefreshButton.IsEnabled = true;
            LoadingImg.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Обработка нажатия на кнопку поиска
        /// </summary>
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            // Если поиск по пустой строке - то показываем все фильмы
            if (string.IsNullOrEmpty(SearchText.Text))
            {
                moviesItemsControl.ItemsSource = allMovies;
                return;
            }

            // иначе - ищем по названию без учета регистра
            var filteredMovies = new ObservableCollection<Movie>(allMovies.Where(x => x.Name.ToUpper().Contains(SearchText.Text.ToUpper())));

            moviesItemsControl.ItemsSource = filteredMovies;

            UpdateNoContentText(filteredMovies, $"По запросу '{SearchText.Text}' ничего не найдено");
        }

        /// <summary>
        /// Обработка нажатия на кнопку редактирования фильма
        /// </summary>
        private void EditMovieButton_Click(object sender, RoutedEventArgs e)
        {
            var movie = (sender as Button).Tag as Movie;

            EditMovieWindow window = new EditMovieWindow(movie);
            var editResult = window.ShowDialog();

            if (editResult.HasValue && editResult.Value)
            {
                // находим в списке со всеми фильмами изменённый фильм
                var existingMovie = allMovies.FirstOrDefault(x => x.Id == window.Movie.Id);
                if (existingMovie == null) return;

                // заменяем старый на новый
                allMovies[allMovies.IndexOf(existingMovie)] = window.Movie;
                SaveMoviesToFile();
            }
        }

        /// <summary>
        /// Обработка нажатия на кнопку удаления фильма
        /// </summary>
        private void DeleteMovieButton_Click(object sender, RoutedEventArgs e)
        {
            var movie = (sender as Button).Tag as Movie;

            var result = MessageBox.Show($"Вы действительно хотите удалить фильм '{movie.Name}'?", "Подтвердите действие", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // удаляем фильм
                allMovies.Remove(movie);
                SaveMoviesToFile();
                moviesItemsControl.ItemsSource = allMovies;
                UpdateNoContentText(allMovies);
            }
        }

        /// <summary>
        /// Обработка нажатия на название фильма
        /// </summary>
        private void MovieNameText_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var movie = (sender as TextBlock).Tag as Movie;

            MovieInfoWindow window = new MovieInfoWindow(movie);
            window.ShowDialog();
        }
    }
}
