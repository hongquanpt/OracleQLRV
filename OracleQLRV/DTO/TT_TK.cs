namespace OracleQLRV.DTO
{
    public class TT_TK
    {
        public int MaTaiKhoan { get; set; }
        public string? Tdn { get; set; }
        public int? MaNhom { get; set; }
      
        public string? TenDv { get; set; }


        public int? MaQn { get; set; }
        public string? HoTen { get; set; }
        public string? TenNhom { get; set; }
        public string? CapBac1 { get; set; }
        public string? TenCv { get; set; }
        public TimeSpan? ThoiGianRa { get; internal set; }
    }
}
