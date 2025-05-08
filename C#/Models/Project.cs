using System;
using System.Collections.Generic;

namespace ConstructionCompany.Models;

public partial class Project
{
    public int ProjectId { get; set; }

    public string Name { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? Address { get; set; }

    public decimal? Budget { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Brigade> Brigades { get; set; } = new List<Brigade>();

    public virtual ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();
}
