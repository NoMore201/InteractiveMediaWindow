using System.Windows;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    public partial class DatabaseWindow : Window
    {

        private DbFileManager db;

        public DatabaseWindow()
        {
            InitializeComponent();
            db = new DbFileManager();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            db.Close();
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MovieGrid_Loaded(object sender, RoutedEventArgs e)
        {
            movieGrid.RowHeight = 50;
            movieGrid.ItemsSource = db.movies;
        }

        private void bookGrid_Loaded(object sender, RoutedEventArgs e)
        {
            bookGrid.RowHeight = 50;
            bookGrid.ItemsSource = db.books;
        }

        private void musicGrid_Loaded(object sender, RoutedEventArgs e)
        {
            musicGrid.RowHeight = 50;
            musicGrid.ItemsSource = db.musics;
        }

        private void TracklistGrid_Loaded(object sender, RoutedEventArgs e)
        {
            tracklistGrid.RowHeight = 25;
            tracklistGrid.ItemsSource = db.tracklists;
        }
    }
}
