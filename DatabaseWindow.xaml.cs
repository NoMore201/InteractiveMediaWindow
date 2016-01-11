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

        private SQLiteConnection m_dbConnection;

        public DatabaseWindow()
        {
            InitializeComponent();
            if (!File.Exists("IMWDatabase.sqlite"))
            {
                this.buttonCreateDB.IsEnabled = true;
            } else
            {
                connect();
            }
        }

        public void InsertBook(string summary, string authors)
        {
            string query = "insert into books (summary, authors) values ('" +
                summary +
                "', '" +
                authors +
                "')";
            SQLiteCommand command = new SQLiteCommand(query, m_dbConnection);
            command.ExecuteNonQuery();
        }

        public SQLiteDataReader GetBooks()
        {
            string query = "select * from books";
            SQLiteCommand command = new SQLiteCommand(query, m_dbConnection);
            return command.ExecuteReader();
        }

        private void buttonCreateDB_Click(object sender, RoutedEventArgs e)
        {
            DataLog.ToConsole("DB doesn't exist, creating a new one");
            SQLiteConnection.CreateFile("IMWDatabase.sqlite");
            connect();
            string itemsTableQuery = "CREATE TABLE items (" +
                "trailer TEXT, " +
                "genre TEXT, " +
                "lightgenre INTEGER, " +
                "favouriterelateds TEXT, " +
                "popularity INTEGER, " +
                "type INTEGER, " +
                "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "year INTEGER, " +
                "vote INTEGER, " +
                "cover TEXT, " +
                "name TEXT, " +
                "position INTEGER, " +
                "description TEXT, " +
                "otherinfo TEXT, " +
                "publishinghouse TEXT" +
                ")";
            string booksTableQuery = "CREATE TABLE books (summary TEXT, authors TEXT, itemid INTEGER, " +
                "FOREIGN KEY(itemid) REFERENCES items(id)" +
                ")";
            string musicTableQuery = "CREATE TABLE music (artist TEXT, preview_file TEXT, itemid INTEGER, " +
                "FOREIGN KEY(itemid) REFERENCES items(id)" +
                ")";
            SQLiteCommand command = new SQLiteCommand(itemsTableQuery, m_dbConnection);
            command.ExecuteNonQuery();
            command = new SQLiteCommand(booksTableQuery, m_dbConnection);
            command.ExecuteNonQuery();
            command = new SQLiteCommand(musicTableQuery, m_dbConnection);
            command.ExecuteNonQuery();
            this.buttonCreateDB.IsEnabled = true;
        }

        private void buttonDebug_Click(object sender, RoutedEventArgs e)
        {
            GetBooks();
        }

        private void connect()
        {
            m_dbConnection = new SQLiteConnection("Data Source=IMWDatabase.sqlite;Version=3;");
            m_dbConnection.Open();
        }

    }
}
