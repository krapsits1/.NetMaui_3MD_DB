using SQLite;
using System;

namespace MD3db
{
    internal class Assignment
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime DeadLine { get; set; }

        public string Description { get; set; }

        [Indexed]
        public int CourseId { get; set; } // Primārā atslēga uz Course tabulu

        public bool IsEditing { get; set; } // Add this property

        [Ignore]
        public string CourseName { get; set; }

    }
}
