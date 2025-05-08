using System;
using System.Collections.Generic;

namespace ConstructionCompany.Models;

public partial class Task
{
    public int TaskId { get; set; }

    public DateOnly CreatedAt { get; set; }

    public DateOnly? Deadline { get; set; }

    public string? Description { get; set; }

    public int? BrigadeId { get; set; }

    public string? Status { get; set; }

    public virtual Brigade? Brigade { get; set; }
}
