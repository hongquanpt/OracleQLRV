using System;
using System.Collections.Generic;

namespace OracleQLRV.Models;

public partial class Quannhan
{
    public int Maqn { get; set; }

    public string Hoten { get; set; } = null!;

    public int? Macv { get; set; }

    public int? Madv { get; set; }

    public int? Macapbac { get; set; }

    public string Diachi { get; set; } = null!;
}
