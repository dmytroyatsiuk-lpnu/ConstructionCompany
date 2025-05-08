using System;
using System.Collections.Generic;

namespace ConstructionCompany.Models;

public partial class Warehouse
{
    public int WarehouseId { get; set; }

    public string? Address { get; set; }

    public virtual ICollection<MaterialWarehouse> MaterialWarehouses { get; set; } = new List<MaterialWarehouse>();
}
