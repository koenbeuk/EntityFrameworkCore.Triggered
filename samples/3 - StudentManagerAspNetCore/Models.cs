using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudentManager.Traits;

namespace StudentManager
{
    public class Student : ISoftDelete, IAudited
    {
        public int Id { get; set; }

        public string DisplayName { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public ICollection<StudentCourse> Courses { get; set; }

        public DateTimeOffset? DeletedOn { get; set; }
    }

    public class Course : ISoftDelete, IAudited
    {
        public int Id { get; set; }

        public string DisplayName { get; set; }

        public bool IsMandatory { get; set; }

        public DateTimeOffset? DeletedOn { get; set; }
    }

    public class StudentCourse
    {
        public int StudentId { get; set; }

        public int CourseId { get; set; }

        public Student Student { get; set; }

        public Course Course { get; set; }
    }

    public class Audit
    {
        public string Discriminator { get; set; }
        
        public int Id { get; set; }

        public DateTimeOffset RecordDate { get; set; }

        public string Record { get; set; }
    }
}
