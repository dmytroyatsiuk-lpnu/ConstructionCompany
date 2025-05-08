using System;
using System.Collections.Generic;

namespace ConstructionCompany.Models;

public partial class Equipment
{
    public int EquipmentId { get; set; }

    public string? Name { get; set; }

    public string? Brand { get; set; }

    public DateOnly? PurchaseDate { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
