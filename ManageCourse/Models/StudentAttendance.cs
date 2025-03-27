using System;
using System.Collections.Generic;

namespace ManageCourse.Models;

public partial class StudentAttendance
{
    public int AttendanceId { get; set; }

    public int? StudentId { get; set; }

    public int? ClassId { get; set; }

    public int? SessionNumber { get; set; }

    public byte? IsPresent { get; set; }

    public virtual Class? Class { get; set; }

    public virtual User? Student { get; set; }
}
