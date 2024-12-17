using System;
using System.Collections.Generic;

namespace OracleQLRV.Models;

public partial class Chitietdanhsach
{
    public int Hinhthucrn { get; set; }

    public int Mahocvien { get; set; }

    public string Lydo { get; set; } = null!;

    public string Diadiem { get; set; } = null!;

    public DateTime Thoigianra { get; set; }

    public DateTime Thoigianvao { get; set; }

    public string? Ghichu { get; set; }

    public int? Tinhtrang { get; set; }

    public int Mactds { get; set; }
}
