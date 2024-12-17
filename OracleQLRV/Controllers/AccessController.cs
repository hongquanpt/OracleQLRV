using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OracleQLRV.Models;
using System.Security.Claims;
using System.Text;
using OracleQLRV.DTO;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using OracleQLRV.authorize;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace OracleQLRV.Controllers
{
    public class AccessController : Controller
    {
        private readonly AppDbContext _context;

        public AccessController(AppDbContext context)
        {
            _context = context;
        }
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;

            //nếu bạn muốn các chữ cái in thường thay vì in hoa thì bạn thay chữ "X" in hoa trong "X2" thành "x"
        }
        public IActionResult Login()
        {
            ViewBag.prevouisPage = Request.Headers.Referer.ToString();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginInfo loginInfo)
        {
            ViewData["action"] = "login";
            var f_MatKhau = GetMD5(loginInfo.MatKhau);

            var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "SP_GetUserRoleData";
            command.CommandType = CommandType.StoredProcedure;

            // Tham số p_TDN (Input)
            var pTdn = new OracleParameter("p_TDN", OracleDbType.Varchar2, loginInfo.TDN, ParameterDirection.Input);
            command.Parameters.Add(pTdn);

            // Tham số p_matKhau (Input)
            var pMatKhau = new OracleParameter("p_matKhau", OracleDbType.Varchar2, f_MatKhau, ParameterDirection.Input);
            command.Parameters.Add(pMatKhau);

            // Tham số p_refCursor (Output)
            var pRefCursor = new OracleParameter("p_refCursor", OracleDbType.RefCursor, ParameterDirection.Output);
            command.Parameters.Add(pRefCursor);

            List<AccountRole> roles = new List<AccountRole>();

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var role = new AccountRole
                    {
                        MaTaiKhoan = reader.GetInt32(reader.GetOrdinal("MaTaiKhoan")),
                        MaQ = reader.GetInt32(reader.GetOrdinal("MaQ")),
                        MaCv = reader.GetInt32(reader.GetOrdinal("MaCv")),
                        TenCV = reader.GetString(reader.GetOrdinal("TenCV")),
                        TenQ = reader.GetString(reader.GetOrdinal("TenQ")),
                        ControllerName = reader.GetString(reader.GetOrdinal("ControllerName")),
                        ActionName = reader.GetString(reader.GetOrdinal("ActionName")),
                        MaQn = reader.GetInt32(reader.GetOrdinal("MaQn"))
                    };
                    roles.Add(role);
                }
            }

            if (roles.Count == 0)
            {
                ViewData["ValidateMessage"] = "Tài khoản không tồn tại hoặc mật khẩu sai.";
                return View();
            }

            HttpContext.Session.SetString("email", roles.First().TenCV);
            HttpContext.Session.SetJson("QuyenTK", roles);

            List<Claim> claims = new List<Claim>()
    {
        new Claim(ClaimTypes.NameIdentifier, loginInfo.TDN),
        new Claim("OtherProperties","Example Role")
    };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties() { AllowRefresh = true };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), properties);

            return RedirectToAction("Index", "Admin");
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterInfo registerInfo)
        {
            try
            {
                ViewData["action"] = "register";
                var user = await _context.Taikhoans.SingleOrDefaultAsync(c => c.Tdn == registerInfo.TDN);
                if (user != null)
                {
                    ViewData["ValidateMessage"] = "Tài khoản đã tồn tại";
                    return RedirectToAction("Login", "Access");
                }
                var f_MatKhau = GetMD5(registerInfo.MatKhau);
                Taikhoan newTk = new Taikhoan()
                {
                    Tdn = registerInfo.TDN,
                    Matkhau = registerInfo.MatKhau,
                    //  Quyen = "khach"
                };
                _context.Taikhoans.Add(newTk);
                await _context.SaveChangesAsync();
                List<Claim> claims = new List<Claim>()
                  {
                      new Claim(ClaimTypes.NameIdentifier,registerInfo.TDN),
                      new Claim("OtherProperties","Example Role")

                  };
                HttpContext.Session.SetString("TDN", registerInfo.TDN);
                HttpContext.Session.SetInt32("Ma", newTk.Mataikhoan);

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                AuthenticationProperties properties = new AuthenticationProperties()
                {
                    AllowRefresh = true
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), properties);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewData["ValidateMessage"] = "Đăng ký thất bại";
                return View();
            }
        }

        public ActionResult Logout()
        {

            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Access");
        }

    }
}

