using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace PCAssembly;

public partial class User : IdentityUser
{

    public virtual ICollection<Assembly> Assemblies { get; set; } = new List<Assembly>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
