using System;
using System.Collections.Generic;

namespace ManageCoure.Models;

public partial class Subject
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Code { get; set; }

    public int? Category { get; set; }

    public virtual Category? CategoryNavigation { get; set; }

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();
}
