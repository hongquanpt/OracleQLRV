namespace OracleQLRV.DTO
{
    public class DSGT
    {
        public int MaHocVien { get; set; }
        public int MaGiayTo { get; set; }

        public int MaCtds { get; set; }

        public bool DaTra { get; set; }

        public DateTime? ThoiGianLay { get; set; }

        public DateTime? ThoiGianTra { get; set; }
        public int SoGiay { get; set; }

        public DateTime? ThoiGianRa { get; set; }
        public DateTime? ThoiGianVao { get; set; }
        public int? TinhTrang { get; set; }
        public string HoTen { get; set; }

        public int? MaCv { get; set; }

        public int? MaDv { get; set; }

        public int? MaCapBac { get; set; }
        public string? CapBac1 { get; set; }
        public string? TenCv { get; set; }
        public string? TenDv { get; set; }
    }
}
