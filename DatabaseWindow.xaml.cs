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
using System.IO;
using System.Data.SQLite;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    /// <summary>
    /// Interaction logic for DatabaseWindow.xaml
    /// </summary>
    public partial class DatabaseWindow : Window
    {

        public DatabaseWindow()
        {
            InitializeComponent();
        }

        private void buttonCreateDB_Click(object sender, RoutedEventArgs e)
        {
            SQLiteConnection.CreateFile("IMWDatabase.sqlite");
            SQLiteConnection m_dbConnection;
            m_dbConnection = new SQLiteConnection("Data Source=IMWDatabase.sqlite;Version=3;");
            m_dbConnection.Open();
        }
    }
}
