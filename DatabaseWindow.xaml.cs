using System.Windows;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    public partial class DatabaseWindow : Window
    {

        public static string MOVIE_FILE = "mov.json";
        public static string BOOK_FILE = "book.json";
        public static string MUSIC_FILE = "music.json";
        public static string TRACK_FILE = "tracks.json";

        private List<Model.Movie> movies;
        private List<Model.Book> books;
        private List<Model.Music> musics;
        private List<Model.Tracklist> tracklists;

        public DatabaseWindow()
        {
            InitializeComponent();
            if (!File.Exists(MOVIE_FILE) && !File.Exists(TRACK_FILE) &&
                !File.Exists(BOOK_FILE) && !File.Exists(MUSIC_FILE))
            {
                File.Create(MOVIE_FILE).Close();
                File.Create(BOOK_FILE).Close();
                File.Create(MUSIC_FILE).Close();
                File.Create(TRACK_FILE).Close();
                movies = new List<Model.Movie>();
                books = new List<Model.Book>();
                musics = new List<Model.Music>();
                tracklists = new List<Model.Tracklist>();
            } else
            {
                ReadFile();
            }
        }

        private void WriteFile()
        {
            StreamWriter movie_file = File.CreateText(MOVIE_FILE);
            StreamWriter book_file = File.CreateText(BOOK_FILE);
            StreamWriter music_file = File.CreateText(MUSIC_FILE);
            StreamWriter track_file = File.CreateText(TRACK_FILE);
            JsonTextWriter movie_writer = new JsonTextWriter(movie_file);
            JsonTextWriter book_writer = new JsonTextWriter(book_file);
            JsonTextWriter music_writer = new JsonTextWriter(music_file);
            JsonTextWriter track_writer = new JsonTextWriter(track_file);
            string data = JsonConvert.SerializeObject(movies);
            movie_writer.WriteRaw(data);
            movie_file.Close();
            data = JsonConvert.SerializeObject(books);
            book_writer.WriteRaw(data);
            book_file.Close();
            data = JsonConvert.SerializeObject(musics);
            music_writer.WriteRaw(data);
            music_file.Close();
            data = JsonConvert.SerializeObject(tracklists);
            track_writer.WriteRaw(data);
            track_file.Close();
        }

        private void ReadFile()
        {
            string data_movie = File.ReadAllText(MOVIE_FILE);
            string data_book = File.ReadAllText(BOOK_FILE);
            string data_music = File.ReadAllText(MUSIC_FILE);
            string data_tracks = File.ReadAllText(TRACK_FILE);
            movies = JsonConvert.DeserializeObject<List<Model.Movie>>(data_movie);
            books = JsonConvert.DeserializeObject<List<Model.Book>>(data_book);
            musics = JsonConvert.DeserializeObject<List<Model.Music>>(data_music);
            tracklists = JsonConvert.DeserializeObject<List<Model.Tracklist>>(data_tracks);
            movieGrid.Items.Refresh();
            bookGrid.Items.Refresh();
            musicGrid.Items.Refresh();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WriteFile();
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MovieGrid_Loaded(object sender, RoutedEventArgs e)
        {
            movieGrid.RowHeight = 50;
            movieGrid.ItemsSource = movies;
        }

        private void bookGrid_Loaded(object sender, RoutedEventArgs e)
        {
            bookGrid.RowHeight = 50;
            bookGrid.ItemsSource = books;
        }

        private void musicGrid_Loaded(object sender, RoutedEventArgs e)
        {
            musicGrid.RowHeight = 50;
            musicGrid.ItemsSource = musics;
        }

        private void TracklistGrid_Loaded(object sender, RoutedEventArgs e)
        {
            tracklistGrid.RowHeight = 25;
            tracklistGrid.ItemsSource = tracklists;
        }
    }
}
