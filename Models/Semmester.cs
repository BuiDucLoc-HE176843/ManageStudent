using System;
using System.Collections.Generic;

namespace ManageCoure.Models;

public partial class Semmester
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();
}
