using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MD3db
{
    internal class Student
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Gender { get; set; }

        public string StudentIdNumber { get; set; }

        [Ignore]
        public string FullName => $"{Name} {Surname}";
    }
}
