using SQLite;
using System;

namespace MD3db
{
    internal class Submission
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime SubmissionTime { get; set; }

        public double Score { get; set; }

        [Indexed]
        public int AssignmentId { get; set; } // primārā atslēga uz Assignment tabulu

        [Indexed]
        public int StudentId { get; set; } // primārā atslēga uz Student tabulu

        [Ignore]
        public string AssignmentDescription { get; set; }

        [Ignore]
        public string StudentName { get; set; }

    }
}
