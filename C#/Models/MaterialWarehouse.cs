using System;
using System.Collections.Generic;

namespace ConstructionCompany.Models;

public partial class MaterialWarehouse
{
    public int MaterialId { get; set; }

    public int WarehouseId { get; set; }

    public int? Quantity { get; set; }

    public virtual Material Material { get; set; } = null!;

    public virtual Warehouse Warehouse { get; set; } = null!;
}
