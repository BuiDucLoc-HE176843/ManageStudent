using System;
using System.Collections.Generic;

namespace ManageCourse.Models;

public partial class Class
{
    public int ClassId { get; set; }

    public string ClassName { get; set; } = null!;

    public int CourseId { get; set; }

    public int TeacherId { get; set; }

    public int? TotalSessions { get; set; }

    public byte Status { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    public virtual ICollection<StudentAttendance> StudentAttendances { get; set; } = new List<StudentAttendance>();

    public virtual User Teacher { get; set; } = null!;
}
