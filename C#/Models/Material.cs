using System;
using System.Collections.Generic;

namespace ConstructionCompany.Models;

public partial class Material
{
    public int MaterialId { get; set; }

    public string? Name { get; set; }

    public int? TotalQuantity { get; set; }

    public decimal? UnitPrice { get; set; }

    public decimal? TotalPrice { get; set; }

    public virtual ICollection<MaterialWarehouse> MaterialWarehouses { get; set; } = new List<MaterialWarehouse>();
}
