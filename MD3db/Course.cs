using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MD3db
{
    internal class Course
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        [Indexed]
        public int TeacherId { get; set; } // Primārā atslēga uz Teacher tabulu
    }
}
