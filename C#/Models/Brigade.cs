using System;
using System.Collections.Generic;

namespace ConstructionCompany.Models;

public partial class Brigade
{
    public int BrigadeId { get; set; }

    public DateOnly? CreatedAt { get; set; }

    public bool? IsActive { get; set; }

    public string? Specialization { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
