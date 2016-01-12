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

        public void InsertItem(
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
            Model.Item i = new Model.Item();
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
    }
}
