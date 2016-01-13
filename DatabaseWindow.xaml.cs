using System.Windows;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    

    public partial class DatabaseWindow : Window
    {

        public static string DATA_FILE = "data.json";
        
        private List<Model.Item> items;

        public DatabaseWindow()
        {
            InitializeComponent();
            if (!File.Exists(DATA_FILE))
            {
                File.Create(DATA_FILE).Close();
                items = new List<Model.Item>();
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
            i.ID = items.Count + 1;
            i.Year = Year;
            i.Vote = Vote;
            i.Cover = Cover;
            i.Name = Name;
            i.Position = Position;
            i.Description = Description;
            i.OtherInfo = OtherInfo;
            i.PublishingHouse = PublishingHouse;
            items.Add(i);
            this.dataGrid.Items.Refresh();
        }

        private void WriteFile()
        {
            StreamWriter file = File.CreateText(DATA_FILE);
            JsonTextWriter writer = new JsonTextWriter(file);
            string data = JsonConvert.SerializeObject(items);
            writer.WriteRaw(data);
            file.Close();
        }

        private void ReadFile()
        {
            string data = File.ReadAllText(DATA_FILE);
            items = JsonConvert.DeserializeObject<List<Model.Item>>(data);
            DataLog.ToConsole(data);
        }

        private void dataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            this.dataGrid.ItemsSource = items;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WriteFile();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Window1 m = new Window1(this);
            m.Show();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
