using System;
using System.Collections.Generic;

namespace OracleQLRV.Models;

public partial class Taikhoan
{
    public int Mataikhoan { get; set; }

    public string Tdn { get; set; } = null!;

    public string Matkhau { get; set; } = null!;

    public int? Maqn { get; set; }

    public int? Manhom { get; set; }
}
