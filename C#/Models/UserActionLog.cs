using System;
using System.Collections.Generic;

namespace ConstructionCompany.Models;

public partial class UserActionLog
{
    public int LogId { get; set; }

    public DateOnly ActionDate { get; set; }

    public TimeOnly ActionTime { get; set; }

    public int UserId { get; set; }

    public string ActionDescription { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
