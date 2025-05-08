using System;
using System.Collections.Generic;

namespace ConstructionCompany.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? Role { get; set; }

    public string? FullName { get; set; }

    public int? EmployeeId { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual ICollection<UserActionLog> UserActionLogs { get; set; } = new List<UserActionLog>();
}
