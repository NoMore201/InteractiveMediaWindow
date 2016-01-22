using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Samples.Kinect.BodyBasics.Model
{
    class Product
    {
        public Model.Book book;
        public Model.Movie movie;
        public Model.Music music;

        public Product(Book b)
        {
            book = b;
        }

        public Product(Music m)
        {
            music = m;
        }

        public Product(Movie b)
        {
            movie = b;
        }

        public Model.LightColors GetColors()
        {

            if (book != null)
            {
                return Colors.lightBookColors[book.LightGenre];
            }
            else if (movie != null)
            {
                return Colors.lightBookColors[movie.LightGenre];
            }
            else if (music != null)
            {
                return Colors.lightMusicColors[music.LightGenre];
            }
            else
                return new Model.LightColors("","","");
        }

        public string GetTrailer()
        {
            if (book != null)
                return book.Trailer;
            else if (movie != null)
                return movie.Trailer;
            else if (music != null)
                return music.Trailer;
            else
                return "";
        }

        public int GetTyp()
        {
            if (book != null)
                return book.Type;
            else if (movie != null)
                return movie.Type;
            else if (music != null)
                return music.Type;
            else
                return 0;
        }

    }

}
