using System;
using System.Collections.Generic;

namespace ConstructionCompany.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string FullName { get; set; } = null!;

    public DateOnly? HireDate { get; set; }

    public decimal? Salary { get; set; }

    public string? Position { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();

    public virtual ICollection<Brigade> Brigades { get; set; } = new List<Brigade>();

    public virtual ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();
}
