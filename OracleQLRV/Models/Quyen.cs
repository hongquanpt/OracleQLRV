using System;
using System.Collections.Generic;

namespace OracleQLRV.Models;

public partial class Quyen
{
    public int Maquyen { get; set; }

    public string Ten { get; set; } = null!;

    public string? Actionname { get; set; }

    public string? Controllername { get; set; }
}
