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

        private List<Model.Movie> movies;
        private List<Model.Book> books;

        public DatabaseWindow()
        {
            InitializeComponent();
            if (!File.Exists(MOVIE_FILE) &&
                !File.Exists(BOOK_FILE))
            {
                File.Create(MOVIE_FILE).Close();
                File.Create(BOOK_FILE).Close();
                movies = new List<Model.Movie>();
                books = new List<Model.Book>();
            } else
            {
                ReadFile();
            }
        }

        public void InsertMovie( string Summary,
            string Directors,
            string Actors,
            string Trailer,
            string Genre,
            int LightGenre,
            string FavouriteRelateds,
            int Popularity,
            int Type,
            int Year,
            int Vote,
            string Cover,
            string Name,
            int Position,
            string Description,
            string OtherInfo,
            string PublishingHouse)
        {
            Model.Movie i = new Model.Movie();
            i.Summary = Summary;
            i.Director = Directors;
            i.Actors = Actors;
            i.Trailer = Trailer;
            i.Genre = Genre;
            i.LightGenre = LightGenre;
            i.FavouriteRelateds = FavouriteRelateds;
            i.Popularity = Popularity;
            i.Type = Type;
            i.ID = movies.Count + books.Count + 1;
            i.Year = Year;
            i.Vote = Vote;
            i.Cover = Cover;
            i.Name = Name;
            i.Position = Position;
            i.Description = Description;
            i.OtherInfo = OtherInfo;
            i.PublishingHouse = PublishingHouse;
            movies.Add(i);
            this.movieGrid.Items.Refresh();
        }

        public void InsertBook(string Summary,
            string Writers,
            string Trailer,
            string Genre,
            int LightGenre,
            string FavouriteRelateds,
            int Popularity,
            int Type,
            int Year,
            int Vote,
            string Cover,
            string Name,
            int Position,
            string Description,
            string OtherInfo,
            string PublishingHouse)
        {
            Model.Book i = new Model.Book();
            i.Summary = Summary;
            i.Writers = Writers;
            i.Trailer = Trailer;
            i.Genre = Genre;
            i.LightGenre = LightGenre;
            i.FavouriteRelateds = FavouriteRelateds;
            i.Popularity = Popularity;
            i.Type = Type;
            i.ID = movies.Count + books.Count + 1;
            i.Year = Year;
            i.Vote = Vote;
            i.Cover = Cover;
            i.Name = Name;
            i.Position = Position;
            i.Description = Description;
            i.OtherInfo = OtherInfo;
            i.PublishingHouse = PublishingHouse;
            books.Add(i);
            this.bookGrid.Items.Refresh();
        }

        private void WriteFile()
        {
            StreamWriter movie_file = File.CreateText(MOVIE_FILE);
            StreamWriter book_file = File.CreateText(BOOK_FILE);
            JsonTextWriter movie_writer = new JsonTextWriter(movie_file);
            JsonTextWriter book_writer = new JsonTextWriter(book_file);
            string data = JsonConvert.SerializeObject(movies);
            movie_writer.WriteRaw(data);
            movie_file.Close();
            data = JsonConvert.SerializeObject(books);
            book_writer.WriteRaw(data);
            book_file.Close();
        }

        private void ReadFile()
        {
            string data_movie = File.ReadAllText(MOVIE_FILE);
            string data_book = File.ReadAllText(BOOK_FILE);
            movies = JsonConvert.DeserializeObject<List<Model.Movie>>(data_movie);
            books = JsonConvert.DeserializeObject<List<Model.Book>>(data_book);
            this.movieGrid.Items.Refresh();
            this.bookGrid.Items.Refresh();
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
            this.movieGrid.RowHeight = 50;
            this.movieGrid.ItemsSource = movies;
        }

        private void bookGrid_Loaded(object sender, RoutedEventArgs e)
        {
            this.bookGrid.RowHeight = 50;
            this.bookGrid.ItemsSource = books;
        }
    }
}
