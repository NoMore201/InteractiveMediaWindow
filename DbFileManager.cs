using System.Windows;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.Samples.Kinect.BodyBasics.Model;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    class DbFileManager
    {

        public static string MOVIE_FILE = "mov.json";
        public static string BOOK_FILE = "book.json";
        public static string MUSIC_FILE = "music.json";
        public static string TRACK_FILE = "tracks.json";

        public List<Book> books;
        public List<Movie> movies;
        public List<Music> musics;
        public List<Tracklist> tracklists;

        public DbFileManager()
        {
            if (!File.Exists(MOVIE_FILE) && !File.Exists(TRACK_FILE) &&
                !File.Exists(BOOK_FILE) && !File.Exists(MUSIC_FILE))
            {
                File.Create(MOVIE_FILE).Close();
                File.Create(BOOK_FILE).Close();
                File.Create(MUSIC_FILE).Close();
                File.Create(TRACK_FILE).Close();
                movies = new List<Movie>();
                books = new List<Book>();
                musics = new List<Music>();
                tracklists = new List<Tracklist>();
            }
            else
            {
                ReadFile();
            }
        }

        public void Close()
        {
            WriteFile();
        }

        public SortedList<int, Model.Tracklist> GetTracklist(int id){

            SortedList<int, Model.Tracklist> tr = new SortedList<int, Model.Tracklist>();

            foreach(Model.Tracklist track in tracklists)
            {
                if (track.ID == id)
                    tr.Add(track.Number, track);
            }

            return tr;
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
    }
}
