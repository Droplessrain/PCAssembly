using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PCAssembly;

public partial class AssemblyComponent
{
    [Key]
    public int AssemblyComponentId { get; set; }
    public int AssemblyId { get; set; }

    public int ComponentId { get; set; }

    public virtual Assembly Assembly { get; set; } = null!;

    public virtual Component Component { get; set; } = null!;
}
