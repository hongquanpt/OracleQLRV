using System;
using System.Collections.Generic;

namespace OracleQLRV.Models;

public partial class ChitietdanhsachGiayto
{
    public int Magiayto { get; set; }

    public int Mactds { get; set; }

    public DateTime? Thoigianlay { get; set; }

    public DateTime? Thoigiantra { get; set; }

    public string? Ghichu { get; set; }
}
