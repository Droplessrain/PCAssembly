using System;
using System.Collections.Generic;

namespace PCAssembly;

public partial class TypeComponent
{
    public int TypeComponentsId { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<Component> Components { get; set; } = new List<Component>();
}
