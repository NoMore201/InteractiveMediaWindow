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
    /// Logica di interazione per Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private DatabaseWindow dbWin;

        public Window1(DatabaseWindow db)
        {
            InitializeComponent();
            dbWin = db;
        }

        private void insert_Click(object sender, RoutedEventArgs e)
        {
            dbWin.InsertMovie(
                this.summary_tb.Text,
                this.director_tb.Text,
                this.actors_tb.Text,
                this.trailer_tb.Text,
                this.genre_tb.Text,
                Int32.Parse( this.lightgenre_tb.Text),
                this.favouriterelateds_tb.Text,
                Int32.Parse(this.popularity_tb.Text),
                Int32.Parse(this.type_tb.Text),
                Int32.Parse(this.year_tb.Text),
                Int32.Parse(this.vote_tb.Text),
                this.cover_tb.Text,
                this.name_tb.Text,
                Int32.Parse(this.position_tb.Text),
                this.description_tb.Text,
                this.otherinfo_tb.Text,
                this.publishinghouse_tb.Text
                );
            this.Close();
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
