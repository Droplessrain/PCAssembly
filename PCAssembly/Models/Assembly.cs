using System;
using System.Collections.Generic;

namespace PCAssembly;

public partial class Assembly
{
    public int AssemblyId { get; set; }

    public required string UserId { get; set; }

    public string AssemblyName { get; set; } = null!;

    public int? Avgrating { get; set; }

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual User User { get; set; } = null!;
}
