using System;
using System.Collections.Generic;

namespace ManageCourse.Models;

public partial class Schedule
{
    public int ScheduleId { get; set; }

    public int ClassId { get; set; }

    public int DayOfWeek { get; set; }

    public byte Session { get; set; }

    public virtual Class Class { get; set; } = null!;
}
