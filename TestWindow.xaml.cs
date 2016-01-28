using Microsoft.Samples.Kinect.BodyBasics.Model;
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

namespace Microsoft.Samples.Kinect.BodyBasics
{
    /// <summary>
    /// Logica di interazione per TestWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        DbFileManager db;

        public TestWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.None;
            db = new DbFileManager();
            Product p = new Product(db.movies[0]);
            DemoRelateds a = new DemoRelateds(GetRelateds(p));
            this.contentControl.Content = a;
        }

        private List<Model.Product> GetRelateds(Model.Product prod)
        {
            int type = prod.GetTyp();
            string[] favrelateds = prod.GetFavouriteRelateds();
            List<Model.Product> relateds = new List<Model.Product>();

            if (type == 1)
            {
                foreach (Model.Movie mov in db.movies)
                    if (StringArrayContains(favrelateds, mov.ID + ""))
                        relateds.Add(new Model.Product(mov));
            }
            else if (type == 2)
            {
                foreach (Model.Book mov in db.books)
                    if (StringArrayContains(favrelateds, mov.ID + ""))
                        relateds.Add(new Model.Product(mov));
            }
            else if (type == 3)
            {
                foreach (Model.Music mov in db.musics)
                    if (StringArrayContains(favrelateds, mov.ID + ""))
                        relateds.Add(new Model.Product(mov));
            }

            return relateds;
        }

        private bool StringArrayContains(string[] arr, string test)
        {
            foreach (string s in arr)
                if (test == s)
                    return true;

            return false;
        }
    }
}
