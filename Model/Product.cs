﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Samples.Kinect.BodyBasics.Model
{
    public class Product
    {
        public Model.Book book;
        public Model.Movie movie;
        public Model.Music music;
        public SortedList<int, Model.Tracklist> tracklist;

        public Product(Book b)
        {
            book = b;
        }

        public Product(Music m)
        {
            music = m;
        }

        public Product(Music m, SortedList<int, Model.Tracklist> t)
        {
            music = m;
            tracklist = t;
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

        public string GetCover()
        {
            if (book != null)
                return book.Cover;
            else if (movie != null)
                return movie.Cover;
            else if (music != null)
                return music.Cover;
            else
                return "";
        }

        public string GetName()
        {
            if (book != null)
                return book.Name;
            else if (movie != null)
                return movie.Name;
            else if (music != null)
                return music.Name;
            else
                return "";
        }

        public int GetTyp()
        {
            if (book != null)
                return 2;
            else if (movie != null)
                return 1;
            else if (music != null)
                return 3;
            else
                return 0;

        }

        public int GetPosition()
        {
            if (book != null)
                return book.Position;
            else if (movie != null)
                return movie.Position;
            else if (music != null)
                return music.Position;
            else
                return 0;
        }

        public string[] GetFavouriteRelateds()
        {
            if (book != null)
                return book.FavouriteRelateds.Split(';');
            else if (movie != null)
                return movie.FavouriteRelateds.Split(';');
            else if (music != null)
                return music.FavouriteRelateds.Split(';');
            else
                return new string[0];
        }

    }

}
