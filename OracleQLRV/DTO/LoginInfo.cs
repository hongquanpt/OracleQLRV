namespace OracleQLRV.DTO
{
    public class LoginInfo
    {
        public string TDN { get; set; }
        public string? MaQN { get; set; }
        public string MatKhau { get; set; }
        public bool KeepLoggedIn = false;
    }
}
