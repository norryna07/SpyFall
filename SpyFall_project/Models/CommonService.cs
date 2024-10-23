using System;
using System.Collections.Generic;

namespace SpyFall_project.Models;

/// <summary>
/// A class what have the Common Service by port.
/// The same as table in database.
/// </summary>
public partial class CommonService
{
    public decimal PortNumber { get; set; }

    public string ServiceName { get; set; } = null!;
}
