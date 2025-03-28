using System;
using System.Collections.Generic;

namespace ManageCourse.Models;

public partial class Grade
{
    public int GradeId { get; set; }

    public int StudentId { get; set; }

    public int ClassId { get; set; }

    public double? Score1 { get; set; }

    public double? Score2 { get; set; }

    public double? Score3 { get; set; }

    public double? Score4 { get; set; }

    public double? Score5 { get; set; }

    public double? FinalScore { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual User Student { get; set; } = null!;
}
