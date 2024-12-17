using System;
using System.Collections.Generic;

namespace OracleQLRV.Models;

public partial class Vipham
{
    public int Mavp { get; set; }

    public string Mota { get; set; } = null!;

    public DateTime? Thoigian { get; set; }

    public string? Ghichu { get; set; }

    public int Mahv { get; set; }
}
