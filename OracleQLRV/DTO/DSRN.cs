namespace OracleQLRV.DTO
{
    public class DSRN
    {
        public int MaCTDS { get; set; }
        public int MaHocVien { get; set; }
        public string? LyDo { get; set; }
        public string? DiaDiem { get; set; }
        public DateTime? ThoiGianRa { get; set; }
        public DateTime? ThoiGianVao { get; set; }
        public int? TinhTrang { get; set; }
        public int HinhThucRn { get; set; }


        public string HoTen { get; set; }
        public string? DiaChi { get; set; }
        public int? MaCv { get; set; }

        public int? MaDv { get; set; }

        public int? MaCapBac { get; set; }
        public string? CapBac1 { get; set; }
        public string? TenCv { get; set; }
        public string? TenDv { get; set; }
    }
}
