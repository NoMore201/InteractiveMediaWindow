using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    static class Colors
    {
        public static SortedList<int, Model.LightColors> lightMusicColors = new SortedList<int, Model.LightColors>{
            {0, new Model.LightColors("Default1", "FFFF00", "FF0000") },
            {1, new Model.LightColors("Default2", "00FF00", "0000FF") },
            {2, new Model.LightColors("Pop", "87CEEB", "FF8533") },
            {3, new Model.LightColors("Rock", "FF3300", "FFAA00") },
            {4, new Model.LightColors("Metal", "3E3E3E", "990000") },
            {5, new Model.LightColors("EDM", "FF00FF", "00FF40") },
            {6, new Model.LightColors("Electronic", "0000FF", "00FF80") },
            {7, new Model.LightColors("Relax", "99FFDC", "FFFF66") },
            {8, new Model.LightColors("Classical", "FFB700", "EEEEEE") },
            {9, new Model.LightColors("Jazz", "0065FF", "FFC0CB") }
        };

        public static SortedList<int, Model.LightColors> lightBookColors = new SortedList<int, Model.LightColors>{
            {0, new Model.LightColors("Default1", "FFFF00", "FF0000") },
            {1, new Model.LightColors("Default2", "00FF00", "0000FF") },
            {2, new Model.LightColors("Comedy", "FFFF00", "00FF00") },
            {3, new Model.LightColors("Dramatic", "3778B4", "F1C232") },
            {4, new Model.LightColors("Romance", "FDFF67", "FFC0CB") },
            {5, new Model.LightColors("Action/Fantasy", "F1C232", "6AA84F") },
            {6, new Model.LightColors("Horror", "CC0000", "000000") },
            {7, new Model.LightColors("Thriller", "38761D", "CC4125") },
            {8, new Model.LightColors("Documentary", "91FF8A", "FDFF67") },
            {9, new Model.LightColors("Serious", "FFC600", "EEEEEE") }
        };
    }
}
