using Microsoft.AspNetCore.Mvc;
using OracleQLRV.authorize;
using OracleQLRV.Models;
using System.Text;
using System.Security.Cryptography;
using OracleQLRV.DTO;
using X.PagedList;
using X.PagedList.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Newtonsoft.Json;

namespace OracleQLRV.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext obj;

        public AdminController(AppDbContext obj)
        {
            this.obj = obj;
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
        public IActionResult Index()
        {
            return View();
        }
        #region admin
        #region Quản lý đơn vị
        public IActionResult QuanLyDonVi()
        {
            var connection = obj.Database.GetDbConnection();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SP_GetDonVis";
            command.CommandType = CommandType.StoredProcedure;

            var pRefCursor = new Oracle.ManagedDataAccess.Client.OracleParameter("p_refCursor", Oracle.ManagedDataAccess.Client.OracleDbType.RefCursor, System.Data.ParameterDirection.Output);
            command.Parameters.Add(pRefCursor);

            List<Donvi> relist = new List<Donvi>();

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var dv = new Donvi
                    {
                        // Giả sử cột Madv kiểu int và Tendv kiểu string
                        Madv = reader.GetInt32(reader.GetOrdinal("Madv")),
                        Cap = (short)reader.GetInt32(reader.GetOrdinal("Cap")),
                        Tendv = reader.GetString(reader.GetOrdinal("Tendv"))

                        // Điền các cột khác theo yêu cầu
                    };
                    relist.Add(dv);
                }
            }

            return View(relist);
        }

        [HttpPost]
        public ActionResult ThemDonVi(string tendv, short cap)
        {
            using (var connection = obj.Database.GetDbConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SP_InsertDonvi";
                    command.CommandType = CommandType.StoredProcedure;

                    var pTendv = new Oracle.ManagedDataAccess.Client.OracleParameter("p_tendv", Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2, tendv, ParameterDirection.Input);
                    command.Parameters.Add(pTendv);

                    var pCap = new Oracle.ManagedDataAccess.Client.OracleParameter("p_cap", Oracle.ManagedDataAccess.Client.OracleDbType.Int16, cap, ParameterDirection.Input);
                    command.Parameters.Add(pCap);

                    command.ExecuteNonQuery();
                }
            }

            return Json(new
            {
                status = true
            });
        }

        public IActionResult XoaDonVi(int madv)
        {
            using (var connection = obj.Database.GetDbConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SP_XoaDonVi";
                    command.CommandType = CommandType.StoredProcedure;

                    var pMadv = new Oracle.ManagedDataAccess.Client.OracleParameter("p_madv", Oracle.ManagedDataAccess.Client.OracleDbType.Int32, madv, ParameterDirection.Input);
                    command.Parameters.Add(pMadv);

                    int rowsAffected = command.ExecuteNonQuery();

                    return Json(new
                    {
                        status = (rowsAffected > 0)
                    });
                }
            }
        }

        public IActionResult SuaDonVi(int madv)
        {
            using (var connection = obj.Database.GetDbConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SP_GetDonviById";
                    command.CommandType = CommandType.StoredProcedure;

                    var pMadv = new Oracle.ManagedDataAccess.Client.OracleParameter("p_madv", Oracle.ManagedDataAccess.Client.OracleDbType.Int32, madv, ParameterDirection.Input);
                    var pRefCursor = new Oracle.ManagedDataAccess.Client.OracleParameter("p_refCursor", Oracle.ManagedDataAccess.Client.OracleDbType.RefCursor, ParameterDirection.Output);

                    command.Parameters.Add(pMadv);
                    command.Parameters.Add(pRefCursor);

                    Donvi model = null;
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            model = new Donvi
                            {
                                Madv = reader.GetInt32(reader.GetOrdinal("Madv")),
                                Tendv = reader.GetString(reader.GetOrdinal("Tendv")),
                                Cap = (short)reader.GetInt16(reader.GetOrdinal("Cap"))
                            };
                        }
                    }

                    return View(model);
                }
            }
        }

        [HttpPost]
        public IActionResult SuaDonVi(int madv, string tendv, short cap)
        {
            using (var connection = obj.Database.GetDbConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SP_UpdateDonvi";
                    command.CommandType = CommandType.StoredProcedure;

                    var pMadv = new Oracle.ManagedDataAccess.Client.OracleParameter("p_madv", Oracle.ManagedDataAccess.Client.OracleDbType.Int32, madv, ParameterDirection.Input);
                    var pTendv = new Oracle.ManagedDataAccess.Client.OracleParameter("p_tendv", Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2, tendv, ParameterDirection.Input);
                    var pCap = new Oracle.ManagedDataAccess.Client.OracleParameter("p_cap", Oracle.ManagedDataAccess.Client.OracleDbType.Int16, cap, ParameterDirection.Input);

                    command.Parameters.Add(pMadv);
                    command.Parameters.Add(pTendv);
                    command.Parameters.Add(pCap);

                    int rowsAffected = command.ExecuteNonQuery();

                    return Json(new
                    {
                        status = (rowsAffected > 0)
                    });
                }
            }
        }

        #endregion
        #region Quản lý cấp bậc
        public IActionResult QuanLyCapBac()
        {
            using (var connection = obj.Database.GetDbConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SP_GetAllCapbacs";
                    command.CommandType = CommandType.StoredProcedure;

                    var pRefCursor = new Oracle.ManagedDataAccess.Client.OracleParameter("p_refCursor", Oracle.ManagedDataAccess.Client.OracleDbType.RefCursor, ParameterDirection.Output);
                    command.Parameters.Add(pRefCursor);

                    List<Capbac> result = new List<Capbac>();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var cb = new Capbac
                            {
                                Macapbac = reader.GetInt32(reader.GetOrdinal("Macapbac")),
                                Capbac1 = reader.GetString(reader.GetOrdinal("Capbac")),
                                Kyhieu = reader.GetString(reader.GetOrdinal("Kyhieu"))
                            };
                            result.Add(cb);
                        }
                    }
                    return View(result);
                }
            }
        }


        [HttpPost]
        public IActionResult ThemCapBac(string tenCB, string KH)
        {
            if (string.IsNullOrEmpty(tenCB) || string.IsNullOrEmpty(KH))
            {
                return Json(new { status = false });
            }

            using (var connection = obj.Database.GetDbConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SP_InsertCapbac";
                    command.CommandType = CommandType.StoredProcedure;

                    var pCapbac1 = new Oracle.ManagedDataAccess.Client.OracleParameter("p_capbac1", Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2, tenCB, ParameterDirection.Input);
                    var pKyhieu = new Oracle.ManagedDataAccess.Client.OracleParameter("p_kyhieu", Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2, KH, ParameterDirection.Input);

                    command.Parameters.Add(pCapbac1);
                    command.Parameters.Add(pKyhieu);

                    command.ExecuteNonQuery();
                }
            }

            return Json(new { status = true });
        }

        public IActionResult SuaCapBac(int maCB)
        {
            using (var connection = obj.Database.GetDbConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SP_GetCapbacById";
                    command.CommandType = CommandType.StoredProcedure;

                    var pMadv = new Oracle.ManagedDataAccess.Client.OracleParameter("p_macapbac", Oracle.ManagedDataAccess.Client.OracleDbType.Int32, maCB, ParameterDirection.Input);
                    var pRefCursor = new Oracle.ManagedDataAccess.Client.OracleParameter("p_refCursor", Oracle.ManagedDataAccess.Client.OracleDbType.RefCursor, ParameterDirection.Output);
                    command.Parameters.Add(pMadv);
                    command.Parameters.Add(pRefCursor);

                    Capbac cb = null;
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cb = new Capbac
                            {
                                Macapbac = reader.GetInt32(reader.GetOrdinal("Macapbac")),
                                Capbac1 = reader.GetString(reader.GetOrdinal("Capbac")),
                                Kyhieu = reader.GetString(reader.GetOrdinal("Kyhieu"))
                            };
                        }
                    }

                    if (cb != null)
                        return View(cb);
                    else
                        return Json(new { status = false });
                }
            }
        }

        [HttpPost]
        public IActionResult SuaCapBac(int maCB, string tenCB, string KH)
        {
            using (var connection = obj.Database.GetDbConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SP_UpdateCapbac";
                    command.CommandType = CommandType.StoredProcedure;

                    var pMacapbac = new Oracle.ManagedDataAccess.Client.OracleParameter("p_macapbac", Oracle.ManagedDataAccess.Client.OracleDbType.Int32, maCB, ParameterDirection.Input);
                    var pCapbac1 = new Oracle.ManagedDataAccess.Client.OracleParameter("p_capbac1", Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2, tenCB, ParameterDirection.Input);
                    var pKyhieu = new Oracle.ManagedDataAccess.Client.OracleParameter("p_kyhieu", Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2, KH, ParameterDirection.Input);

                    command.Parameters.Add(pMacapbac);
                    command.Parameters.Add(pCapbac1);
                    command.Parameters.Add(pKyhieu);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                        return Json(new { status = true });
                    else
                        return Json(new { status = false });
                }
            }
        }

        public IActionResult XoaCapBac(int maCB)
        {
            using (var connection = obj.Database.GetDbConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SP_DeleteCapbac";
                    command.CommandType = CommandType.StoredProcedure;

                    var pMacapbac = new Oracle.ManagedDataAccess.Client.OracleParameter("p_macapbac", Oracle.ManagedDataAccess.Client.OracleDbType.Int32, maCB, ParameterDirection.Input);
                    command.Parameters.Add(pMacapbac);

                    int rowsAffected = command.ExecuteNonQuery();
                    return Json(new { status = (rowsAffected > 0) });
                }
            }
        }

        #endregion
        #region Quản lý chức vụ

        public IActionResult QuanLyChucVu()
        {
            using (var connection = obj.Database.GetDbConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SP_GetAllChucvus";
                    command.CommandType = CommandType.StoredProcedure;

                    var pRefCursor = new Oracle.ManagedDataAccess.Client.OracleParameter("p_refCursor", Oracle.ManagedDataAccess.Client.OracleDbType.RefCursor, System.Data.ParameterDirection.Output);
                    command.Parameters.Add(pRefCursor);

                    List<Chucvu> chucvus = new List<Chucvu>();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var cv = new Chucvu
                            {
                                Macv = reader.GetInt32(reader.GetOrdinal("Macv")),
                                Tencv = reader.GetString(reader.GetOrdinal("Tencv")),
                                Kyhieu = reader.GetString(reader.GetOrdinal("Kyhieu"))
                            };
                            chucvus.Add(cv);
                        }
                    }

                    return View(chucvus);
                }
            }
        }


        [HttpPost]
        public ActionResult ThemChucVu(string TenCv, string KH)
        {
            using (var connection = obj.Database.GetDbConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SP_InsertChucvu";
                    command.CommandType = CommandType.StoredProcedure;

                    var pTencv = new Oracle.ManagedDataAccess.Client.OracleParameter("p_tencv", Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2, TenCv, System.Data.ParameterDirection.Input);
                    var pKyhieu = new Oracle.ManagedDataAccess.Client.OracleParameter("p_kyhieu", Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2, KH, System.Data.ParameterDirection.Input);

                    command.Parameters.Add(pTencv);
                    command.Parameters.Add(pKyhieu);

                    command.ExecuteNonQuery();
                }
            }

            return Json(new { status = true });
        }

        public IActionResult XoaChucVu(int macv)
        {
            using (var connection = obj.Database.GetDbConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SP_DeleteChucvu";
                    command.CommandType = CommandType.StoredProcedure;

                    var pMacv = new Oracle.ManagedDataAccess.Client.OracleParameter("p_macv", Oracle.ManagedDataAccess.Client.OracleDbType.Int32, macv, System.Data.ParameterDirection.Input);
                    command.Parameters.Add(pMacv);

                    int rowsAffected = command.ExecuteNonQuery();
                    return Json(new { status = (rowsAffected > 0) });
                }
            }
        }


        public IActionResult SuaChucVu(int macv)
        {
            using (var connection = obj.Database.GetDbConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SP_GetChucvuById";
                    command.CommandType = CommandType.StoredProcedure;

                    var pMacv = new Oracle.ManagedDataAccess.Client.OracleParameter("p_macv", Oracle.ManagedDataAccess.Client.OracleDbType.Int32, macv, System.Data.ParameterDirection.Input);
                    var pRefCursor = new Oracle.ManagedDataAccess.Client.OracleParameter("p_refCursor", Oracle.ManagedDataAccess.Client.OracleDbType.RefCursor, System.Data.ParameterDirection.Output);

                    command.Parameters.Add(pMacv);
                    command.Parameters.Add(pRefCursor);

                    Chucvu model = null;
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            model = new Chucvu
                            {
                                Macv = reader.GetInt32(reader.GetOrdinal("Macv")),
                                Tencv = reader.GetString(reader.GetOrdinal("Tencv")),
                                Kyhieu = reader.GetString(reader.GetOrdinal("Kyhieu"))
                            };
                        }
                    }

                    return View(model);
                }
            }
        }

        [HttpPost]
        public IActionResult SuaChucVu(int macv, string TenCv, string KH)
        {
            using (var connection = obj.Database.GetDbConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SP_UpdateChucvu";
                    command.CommandType = CommandType.StoredProcedure;

                    var pMacv = new Oracle.ManagedDataAccess.Client.OracleParameter("p_macv", Oracle.ManagedDataAccess.Client.OracleDbType.Int32, macv, System.Data.ParameterDirection.Input);
                    var pTencv = new Oracle.ManagedDataAccess.Client.OracleParameter("p_tencv", Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2, TenCv, System.Data.ParameterDirection.Input);
                    var pKyhieu = new Oracle.ManagedDataAccess.Client.OracleParameter("p_kyhieu", Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2, KH, System.Data.ParameterDirection.Input);

                    command.Parameters.Add(pMacv);
                    command.Parameters.Add(pTencv);
                    command.Parameters.Add(pKyhieu);

                    int rowsAffected = command.ExecuteNonQuery();
                    return Json(new { status = (rowsAffected > 0) });
                }
            }
        }


        #endregion
        #region Quản lý giấy tờ
        public IActionResult QuanLyGiayTo()
        {
            List<Giayto> giaytos = new List<Giayto>();
            var dv= obj.Donvis.ToList();
            using (var connection = obj.Database.GetDbConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SP_GetAllGiaytos";
                    command.CommandType = CommandType.StoredProcedure;

                    var pRefCursor = new Oracle.ManagedDataAccess.Client.OracleParameter("p_refCursor", Oracle.ManagedDataAccess.Client.OracleDbType.RefCursor, System.Data.ParameterDirection.Output);
                    command.Parameters.Add(pRefCursor);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var gt = new Giayto
                            {
                                Magiayto = reader.GetInt32(reader.GetOrdinal("Magiayto")),
                                Sogiay = reader.GetInt32(reader.GetOrdinal("Sogiay")),
                                Tinhtrang = reader.GetInt32(reader.GetOrdinal("Tinhtrang")) == 1,
                                Madv = reader.GetInt32(reader.GetOrdinal("Madv"))
                            };
                            giaytos.Add(gt);
                        }
                    }
                }
            }

            ViewBag.ChonDonVi = dv;
            return View(giaytos);
        }

        [HttpPost]
        public ActionResult ThemGiayTo(int sogiay, int madv)
        {
            using (var connection = obj.Database.GetDbConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SP_InsertGiayto";
                    command.CommandType = CommandType.StoredProcedure;

                    var pSogiay = new Oracle.ManagedDataAccess.Client.OracleParameter("p_sogiay", Oracle.ManagedDataAccess.Client.OracleDbType.Int32, sogiay, System.Data.ParameterDirection.Input);
                    var pMadv = new Oracle.ManagedDataAccess.Client.OracleParameter("p_madv", Oracle.ManagedDataAccess.Client.OracleDbType.Int32, madv, System.Data.ParameterDirection.Input);

                    command.Parameters.Add(pSogiay);
                    command.Parameters.Add(pMadv);

                    command.ExecuteNonQuery();
                }
            }

            return Json(new { status = true });
        }

        public IActionResult XoaGiayTo(int magiayto)
        {
            using (var connection = obj.Database.GetDbConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SP_DeleteGiayto";
                    command.CommandType = CommandType.StoredProcedure;

                    var pMagiayto = new Oracle.ManagedDataAccess.Client.OracleParameter("p_magiayto", Oracle.ManagedDataAccess.Client.OracleDbType.Int32, magiayto, System.Data.ParameterDirection.Input);
                    command.Parameters.Add(pMagiayto);

                    int rowsAffected = command.ExecuteNonQuery();
                    return Json(new { status = (rowsAffected > 0) });
                }
            }
        }


        public IActionResult SuaGiayTo(int magiayto)
        {
            Giayto model = null;
            var dv = obj.Donvis.ToList();
            using (var connection = obj.Database.GetDbConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SP_GetGiaytoById";
                    command.CommandType = CommandType.StoredProcedure;

                    var pMagiayto = new Oracle.ManagedDataAccess.Client.OracleParameter("p_magiayto", Oracle.ManagedDataAccess.Client.OracleDbType.Int32, magiayto, System.Data.ParameterDirection.Input);
                    var pRefCursor = new Oracle.ManagedDataAccess.Client.OracleParameter("p_refCursor", Oracle.ManagedDataAccess.Client.OracleDbType.RefCursor, System.Data.ParameterDirection.Output);

                    command.Parameters.Add(pMagiayto);
                    command.Parameters.Add(pRefCursor);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            model = new Giayto
                            {
                                Magiayto = reader.GetInt32(reader.GetOrdinal("Magiayto")),
                                Sogiay = reader.GetInt32(reader.GetOrdinal("Sogiay")),
                                Tinhtrang = reader.GetInt32(reader.GetOrdinal("Tinhtrang")) == 1,
                                Madv = reader.GetInt32(reader.GetOrdinal("Madv"))
                            };
                        }
                    }
                }
            }

            ViewBag.ChonDonVi = dv;
            return View(model);
        }

        [HttpPost]
        public IActionResult SuaGiayTo(int magiayto, int sogiay, int madv, int tinhtrang)
        {
            using (var connection = obj.Database.GetDbConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SP_UpdateGiayto";
                    command.CommandType = CommandType.StoredProcedure;

                    var pMagiayto = new Oracle.ManagedDataAccess.Client.OracleParameter("p_magiayto", Oracle.ManagedDataAccess.Client.OracleDbType.Int32, magiayto, System.Data.ParameterDirection.Input);
                    var pSogiay = new Oracle.ManagedDataAccess.Client.OracleParameter("p_sogiay", Oracle.ManagedDataAccess.Client.OracleDbType.Int32, sogiay, System.Data.ParameterDirection.Input);
                    var pMadv = new Oracle.ManagedDataAccess.Client.OracleParameter("p_madv", Oracle.ManagedDataAccess.Client.OracleDbType.Int32, madv, System.Data.ParameterDirection.Input);
                    var pTinhtrang = new Oracle.ManagedDataAccess.Client.OracleParameter("p_madv", Oracle.ManagedDataAccess.Client.OracleDbType.Int32,tinhtrang, System.Data.ParameterDirection.Input);
                    command.Parameters.Add(pMagiayto);
                    command.Parameters.Add(pSogiay);
                    command.Parameters.Add(pMadv);
                    command.Parameters.Add(pTinhtrang);

                    int rowsAffected = command.ExecuteNonQuery();
                    return Json(new { status = (rowsAffected > 0) });
                }
            }
        }


        #endregion
        #region Quản lý tài khoản
        public IActionResult QuanLyTK()
        {
            // Thực hiện truy vấn và phân trang
            var query = from tk in obj.Taikhoans
                        join cv in obj.Nhoms on tk.Manhom equals cv.Manhom
                        join qn in obj.Quannhans on tk.Maqn equals qn.Maqn
                        join dv in obj.Donvis on qn.Madv equals dv.Madv
                        join cb in obj.Capbacs on qn.Macapbac equals cb.Macapbac
                        join ch in obj.Chucvus on qn.Macv equals ch.Macv
                        select new TT_TK
                        {
                            MaTaiKhoan = tk.Mataikhoan,
                            Tdn = tk.Tdn,
                            MaNhom = cv.Manhom,
                           
                            MaQn = qn.Maqn,
                            HoTen = qn.Hoten,
                            TenNhom = cv.Tennhom,
                            TenDv = dv.Tendv,
                            TenCv = ch.Tencv,
                            CapBac1 = cb.Capbac1
                        };

            var nhomquyen = obj.NhomQuyens.ToList();
            var pagedList = query.ToList();
            ViewBag.nhomquyen = nhomquyen;
            ViewBag.ChonCapBac = (obj.Capbacs.ToList());
            ViewBag.ChonChucVu = (obj.Chucvus.ToList());
            ViewBag.ChonDonVi = (obj.Donvis.ToList());
            ViewBag.ChonQuanNhan = (obj.Quannhans.ToList());
            ViewBag.ChonNhomQuyen = (obj.NhomQuyens.ToList());
            ViewBag.ChonNhom = (obj.Nhoms.ToList());
            return View(pagedList);
        }
        [HttpPost]
        public ActionResult ThemTaiKhoan(string tdn, string matkhau, int maqn, int manhom)
        {
            var moi = new Models.Taikhoan();
            moi.Tdn = tdn;
            moi.Matkhau = GetMD5(matkhau);
            moi.Maqn = maqn;
            moi.Manhom = manhom;
            obj.Taikhoans.Add(moi);
            obj.SaveChanges();
            return Json(new
            {
                status = true
            });
        }
        public IActionResult XoaTaiKhoan(int mataikhoan)
        {
            var taikhoan = obj.Taikhoans.Find(mataikhoan);
            if (taikhoan != null)
            {
                obj.Taikhoans.Remove(taikhoan);
                obj.SaveChanges();
                return Json(new
                {
                    status = true
                });
            }
            else
            {
                return Json(new
                {
                    status = false
                });
            }

        }

        public IActionResult SuaTaiKhoan(int mataikhoan)
        {
            var model = obj.Taikhoans.Find(mataikhoan);
            ViewBag.ChonQuanNhan = (obj.Quannhans.ToList());
            // ViewBag.ChonNhomQuyen = (obj.NhomQuyens.ToList());
            ViewBag.ChonCapBac = (obj.Capbacs.ToList());
            ViewBag.ChonChucVu = (obj.Chucvus.ToList());
            ViewBag.ChonDonVi = (obj.Donvis.ToList());
            //  ViewBag.ChonQuanNhan = (obj.Quannhans.ToList());
            // ViewBag.ChonNhomQuyen = (obj.NhomQuyens.ToList());
            ViewBag.ChonNhom = (obj.Nhoms.ToList());
            ViewBag.matk = mataikhoan;
            return View(model);
        }
        [HttpPost]
        public IActionResult SuaTaiKhoan(int mataikhoan, string tdn, string matkhau, int maqn, int manhom)
        {
            var taikhoan = obj.Taikhoans.Find(mataikhoan);
            if (taikhoan != null)
            {
                taikhoan.Tdn = tdn;
                taikhoan.Matkhau = matkhau;
                taikhoan.Maqn = maqn;
                taikhoan.Manhom = manhom;
                obj.SaveChanges();
                return Json(new
                {
                    status = true
                });
            }
            else
            {
                return Json(new
                {
                    status = false
                });
            }
        }
        #endregion

        #endregion

        #region Quản lý danh sách     
        public IActionResult QuanLyDanhSach()
        {
            var query2 = from ct in obj.Chitietdanhsaches
                         join qn in obj.Quannhans on ct.Mahocvien equals qn.Maqn
                         join cb in obj.Capbacs on qn.Macapbac equals cb.Macapbac
                         join dv in obj.Donvis on qn.Madv equals dv.Madv
                         join cv in obj.Chucvus on qn.Macv equals cv.Macv
                         join dsgt in obj.ChitietdanhsachGiaytos on ct.Mactds equals dsgt.Mactds
                         join gt in obj.Giaytos on dsgt.Magiayto equals gt.Magiayto
                         where ct.Tinhtrang == 3
                         select new DSGT
                         {
                             MaCtds = dsgt.Mactds,
                             MaHocVien = ct.Mahocvien,
                             MaGiayTo = dsgt.Magiayto,
                             SoGiay = gt.Sogiay,
                             ThoiGianLay = dsgt.Thoigianlay,
                             ThoiGianTra = dsgt.Thoigiantra,
                             DaTra = gt.Tinhtrang,
                             MaCv = qn.Macv,
                             MaDv = qn.Madv,
                             MaCapBac = qn.Macapbac,
                             CapBac1 = cb.Capbac1,
                             TenCv = cv.Tencv,
                             TenDv = dv.Tendv,
                             TinhTrang = ct.Tinhtrang,
                             ThoiGianRa = ct.Thoigianra,
                             ThoiGianVao = ct.Thoigianvao,
                             HoTen = qn.Hoten
                         };
            List<DSGT> ds = query2.ToList();
            List<Giayto> giayto = obj.Giaytos.Where(c => c.Tinhtrang == true).ToList();
            var hv = obj.Quannhans.Where(c => c.Macv == 1 || c.Macv == 2).ToList();
            List<DSRN> dsrnList = new List<DSRN>();

            using (var conn = obj.Database.GetDbConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SP_GetAllDSRN";
                    cmd.CommandType = CommandType.StoredProcedure;

                    var pRefCursor = new Oracle.ManagedDataAccess.Client.OracleParameter("p_refCursor", Oracle.ManagedDataAccess.Client.OracleDbType.RefCursor, ParameterDirection.Output);
                    cmd.Parameters.Add(pRefCursor);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var dsrn = new DSRN
                            {
                                MaCTDS = reader.GetInt32(reader.GetOrdinal("MaCTDS")),
                                MaHocVien = reader.GetInt32(reader.GetOrdinal("MaHocVien")),
                                LyDo = reader.IsDBNull(reader.GetOrdinal("LyDo")) ? null : reader.GetString(reader.GetOrdinal("LyDo")),
                                DiaDiem = reader.IsDBNull(reader.GetOrdinal("DiaDiem")) ? null : reader.GetString(reader.GetOrdinal("DiaDiem")),
                                ThoiGianRa = reader.GetDateTime(reader.GetOrdinal("ThoiGianRa")),
                                ThoiGianVao = reader.GetDateTime(reader.GetOrdinal("ThoiGianVao")),
                                TinhTrang = reader.GetInt32(reader.GetOrdinal("TinhTrang")),
                                HinhThucRn = reader.IsDBNull(reader.GetOrdinal("HinhThucRn")) ? 0 : reader.GetInt32(reader.GetOrdinal("HinhThucRn")),
                                MaCv = reader.GetInt32(reader.GetOrdinal("MaCv")),
                                MaDv = reader.GetInt32(reader.GetOrdinal("MaDv")),
                                MaCapBac = reader.GetInt32(reader.GetOrdinal("MaCapBac")),
                                CapBac1 = reader.GetString(reader.GetOrdinal("Capbac")),
                                TenCv = reader.GetString(reader.GetOrdinal("TenCv")),
                                TenDv = reader.GetString(reader.GetOrdinal("TenDv")),
                                DiaChi = reader.GetString(reader.GetOrdinal("DiaChi")),
                                HoTen = reader.GetString(reader.GetOrdinal("HoTen"))
                            };
                            dsrnList.Add(dsrn);
                        }
                    }
                }
            }

            // Bây giờ bạn có dsrnList tương đương query ban đầu, bạn có thể filter, count như code gốc
            var tinhTrang5 = dsrnList.Where(o => o.TinhTrang == 4).OrderBy(o => o.MaCTDS).ToList();
            var tinhTrang0 = dsrnList.Where(o => o.TinhTrang == 0).OrderBy(o => o.MaCTDS).ToList();
            var tinhTrang1 = dsrnList.Where(o => o.TinhTrang == 1).OrderBy(o => o.MaCTDS).ToList();
            var tinhTrang2 = dsrnList.Where(o => o.TinhTrang == 2).OrderBy(o => o.MaCTDS).ToList();
            var tinhTrang3 = dsrnList.Where(o => o.TinhTrang == 3).OrderBy(o => o.MaCTDS).ToList();
            var tinhTrang4 = dsrnList.Where(o => o.TinhTrang == 4).OrderBy(o => o.MaCTDS).ToList();

            ViewBag.pagedList5 = tinhTrang5;
            ViewBag.pagedList0 = tinhTrang0;
            ViewBag.pagedList1 = tinhTrang1;
            ViewBag.pagedList2 = tinhTrang2;
            ViewBag.pagedList3 = tinhTrang3;
            ViewBag.pagedList4 = tinhTrang4;

            int totalOrders = dsrnList.Count(o => o.TinhTrang == 4);
            int unpaidOrdersCount = dsrnList.Count(o => o.TinhTrang == 0);
            int pendingOrdersCount = dsrnList.Count(o => o.TinhTrang == 1);
            int shippingOrdersCount = dsrnList.Count(o => o.TinhTrang == 2);
            int completedOrdersCount = dsrnList.Count(o => o.TinhTrang == 3);
            int canceledOrdersCount = dsrnList.Count(o => o.TinhTrang == 4);

            ViewBag.DaHoanThanh = totalOrders;
            ViewBag.DaBiTuChoi = unpaidOrdersCount;
            ViewBag.ChuaPheDuyet = pendingOrdersCount;
            ViewBag.PheDuyetC = shippingOrdersCount;
            ViewBag.PheDuyetD = completedOrdersCount;
            ViewBag.PheDuyetCong = canceledOrdersCount;

            
           
            HttpContext.Session.SetJson("DS", ds);
 
            HttpContext.Session.SetJson("GT", giayto);
            var ds2 = dsrnList.Where(c => c.TinhTrang != 4 && c.TinhTrang != 0);
            List<int> mahv = ds2.Select(c => c.MaHocVien).ToList();
          
            var hocvien = hv.Where(c => !mahv.Contains(c.Maqn)).ToList();
            HttpContext.Session.SetJson("HV", hocvien);
            return View();
        }
        public IActionResult Duyet1(int mactds, int maqn)
        {

            var dh = obj.Chitietdanhsaches.FirstOrDefault(c => c.Mactds == mactds);

            if (dh != null)
            {
                var duyet = new CanboDuyet
                {
                    Mactds = mactds,
                    Macb = maqn,
                    Thoigianduyet = DateTime.Now,
                    Ghichu = "Phê duyệt đại đội"
                };
                obj.CanboDuyets.Add(duyet);
                dh.Tinhtrang = 2;
                obj.SaveChanges();
                return Json(new
                {
                    status = true
                });
            }
            else
            {
                return Json(new
                {
                    status = false
                });
            }
        }


        public IActionResult TC1(int mactds, int maqn)
        {

            var dh = obj.Chitietdanhsaches.Where(c => c.Mactds == mactds).FirstOrDefault();
            if (dh != null)
            {
                var duyet = new CanboDuyet
                {
                    Mactds = mactds,
                    Macb = maqn,
                    Thoigianduyet = DateTime.Now,
                    Ghichu = "Từ chối đại đội"
                };
                obj.CanboDuyets.Add(duyet);
                dh.Tinhtrang = 0;
                obj.SaveChanges();
                return Json(new
                {
                    status = true
                });
            }
            else
            {
                return Json(new
                {
                    status = false
                });
            }

        }
        public IActionResult All1(int maqn)
        {
            using (var conn = obj.Database.GetDbConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SP_All1";
                    cmd.CommandType = CommandType.StoredProcedure;

                    var pMaqn = new Oracle.ManagedDataAccess.Client.OracleParameter("p_maqn", Oracle.ManagedDataAccess.Client.OracleDbType.Int32, maqn, ParameterDirection.Input);
                    cmd.Parameters.Add(pMaqn);

                    cmd.ExecuteNonQuery();
                }
            }
            return Json(new { status = true });
        }


        public IActionResult AllT1(int maqn)
        {
            using (var conn = obj.Database.GetDbConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SP_AllT1";
                    cmd.CommandType = CommandType.StoredProcedure;

                    var pMaqn = new Oracle.ManagedDataAccess.Client.OracleParameter("p_maqn", Oracle.ManagedDataAccess.Client.OracleDbType.Int32, maqn, ParameterDirection.Input);
                    cmd.Parameters.Add(pMaqn);

                    cmd.ExecuteNonQuery();
                }
            }
            return Json(new { status = true });
        }

        // Duyet d
        public IActionResult Duyet2(int mactds, int maqn)
        {

            var dh = obj.Chitietdanhsaches.Find(mactds);
            if (dh != null)
            {
                var duyet = new CanboDuyet
                {
                    Mactds = mactds,
                    Macb = maqn,
                    Thoigianduyet = DateTime.Now,
                    Ghichu = "Phê duyệt tiểu đoàn"
                };
                obj.CanboDuyets.Add(duyet);
                dh.Tinhtrang = 3;
                obj.SaveChanges();
                return Json(new
                {
                    status = true
                });
            }
            else
            {
                return Json(new
                {
                    status = false
                });
            }

        }
        public IActionResult TC2(int mactds, int maqn)
        {

            var dh = obj.Chitietdanhsaches.Where(c => c.Mactds == mactds).FirstOrDefault();
            if (dh != null)
            {
                var duyet = new CanboDuyet
                {
                    Mactds = mactds,
                    Macb = maqn,
                    Thoigianduyet = DateTime.Now,
                    Ghichu = "Từ chối tiểu đoàn"
                };
                obj.CanboDuyets.Add(duyet);
                dh.Tinhtrang = 0;
                obj.SaveChanges();
                return Json(new
                {
                    status = true
                });
            }
            else
            {
                return Json(new
                {
                    status = false
                });
            }

        }
        public IActionResult All2(int maqn)
        {


            // Lấy danh sách chi tiết đánh sách có trạng thái là 1
            var danhSachChiTiet = obj.Chitietdanhsaches.Where(c => c.Tinhtrang == 2).ToList();

            // Lấy danh sách ID của vi phạm
            List<int> viphamsIds = obj.Viphams.Select(v => v.Mahv).ToList();
            //List<string> diachi= obj.Quannhans.Select(v=>v.DiaChi).ToList();
            // Kiểm tra xem danh sách ID của vi phạm có dữ liệu và danh sách chi tiết có dữ liệu không
            if (viphamsIds.Any() && danhSachChiTiet != null && danhSachChiTiet.Any())
            {
                // Lọc và chỉ giữ lại các đối tượng không có mã trong danh sách vi phạm
                var danhSachKhongViPham = danhSachChiTiet.Where(c => !viphamsIds.Contains(c.Mahocvien)).ToList();
                var danhSachViPham = danhSachChiTiet.Where(c => viphamsIds.Contains(c.Mahocvien)).ToList();
                // Cập nhật trạng thái trực tiếp trong cơ sở dữ liệu
                foreach (var chiTiet in danhSachKhongViPham)
                {
                    // Kiểm tra nếu HinhThuc là 1 trong danhSachChiTiet
                    if (chiTiet.Hinhthucrn == 1)
                    {
                        string diachi = obj.Quannhans.Where(c => c.Maqn == chiTiet.Mahocvien).FirstOrDefault().Diachi;
                        // So sánh địa chỉ với obj.QuanNhans và gán TinhTrang tùy thuộc vào kết quả
                        if (chiTiet.Diadiem.Contains(diachi))
                        {
                            var duyet = new CanboDuyet
                            {
                                Mactds = chiTiet.Mactds,
                                Macb = maqn,
                                Thoigianduyet = DateTime.Now,
                                Ghichu = "Từ chối tiểu đoàn"
                            };

                            // Add the entity to the context
                            obj.CanboDuyets.Add(duyet);
                            chiTiet.Tinhtrang = 0;
                        }
                        else
                        {
                            var duyet = new CanboDuyet
                            {
                                Mactds = chiTiet.Mactds,
                                Macb = maqn,
                                Thoigianduyet = DateTime.Now,
                                Ghichu = "Phê duyệt tiểu đoàn"
                            };

                            // Add the entity to the context
                            obj.CanboDuyets.Add(duyet);
                            chiTiet.Tinhtrang = 3;
                        }
                    }
                    else
                    {
                        var duyet = new CanboDuyet
                        {
                            Mactds = chiTiet.Mactds,
                            Macb = maqn,
                            Thoigianduyet = DateTime.Now,
                            Ghichu = "Phê duyệt tiểu đoàn"
                        };

                        // Add the entity to the context
                        obj.CanboDuyets.Add(duyet);
                        // Nếu HinhThuc có giá trị khác 1, gán TinhTrang = 2
                        chiTiet.Tinhtrang = 3;
                    }
                }
                foreach (var chiTiet in danhSachViPham)
                {
                    var duyet = new CanboDuyet
                    {
                        Mactds = chiTiet.Mactds,
                        Macb = maqn,
                        Thoigianduyet = DateTime.Now,
                        Ghichu = "Từ chối tiểu đoàn"
                    };

                    // Add the entity to the context
                    obj.CanboDuyets.Add(duyet);
                    chiTiet.Tinhtrang = 0;
                }
                // Lưu thay đổi vào cơ sở dữ liệu
                obj.SaveChanges();

                return Json(new
                {
                    status = true
                });
            }
            else
            {
                return Json(new
                {
                    status = false
                });
            }
        }

        public IActionResult AllT2(int maqn)
        {

            var danhSachChiTiet = obj.Chitietdanhsaches.Where(c => c.Tinhtrang == 1).ToList();
            if (danhSachChiTiet != null && danhSachChiTiet.Any())
            {
                foreach (var chiTiet in danhSachChiTiet)
                {
                    var duyet = new CanboDuyet
                    {
                        Mactds = chiTiet.Mactds,
                        Macb = maqn,
                        Thoigianduyet = DateTime.Now,
                        Ghichu = "Từ chối tiểu đoàn"
                    };

                    // Add the entity to the context
                    obj.CanboDuyets.Add(duyet);
                    chiTiet.Tinhtrang = 0;
                }
                // Lưu thay đổi vào cơ sở dữ liệu
                obj.SaveChanges();

                return Json(new
                {
                    status = true
                });
            }
            else
            {
                return Json(new
                {
                    status = false
                });
            }
        }
        public IActionResult TraGT(int id, int magiay, DateTime thoigianTra)
        {
            var giayto = obj.Giaytos.Where(c => c.Magiayto == magiay).FirstOrDefault();
            giayto.Tinhtrang = true;
            var tinhtrang = obj.Chitietdanhsaches.Find(id);
            tinhtrang.Tinhtrang = 4;
            var ct = obj.ChitietdanhsachGiaytos.Where(c => c.Mactds == id && c.Magiayto == magiay).FirstOrDefault();
            ct.Thoigiantra = DateTime.Now; 
            obj.SaveChanges();
            return Json(new
            {
                status = true
            });

            /* return Json(new
             {
                 status = false
             });*/

        }
        public IActionResult ThemGT(int id, int giayto, DateTime thoigianLay)
        {

            ChitietdanhsachGiayto ct = new ChitietdanhsachGiayto();
            ct.Mactds = id;
            ct.Magiayto = giayto;
            ct.Thoigianlay = thoigianLay;
            var gt = obj.Giaytos.Find(giayto);
            gt.Tinhtrang = false;
            obj.ChitietdanhsachGiaytos.Add(ct);
            obj.SaveChanges();
            return Json(new
            {
                status = true
            });

            /* return Json(new
             {
                 status = false
             });*/

        }
        public IActionResult ChiTiet(int maCTDS, int maGiayTo)
        {
            var ct = obj.ChitietdanhsachGiaytos.Where(c => c.Mactds == maCTDS && c.Magiayto == maGiayTo).FirstOrDefault();
            return View(ct);
        }
        public IActionResult ThemDS(int hocVien, int hinhThuc, string lyDo, string diaDiem, DateTime thoiGianRa, DateTime thoiGianVao)
        {

            int max = obj.Chitietdanhsaches.Max(r => r.Mactds);
            var ct = new Chitietdanhsach
            {
                Mactds = max + 1,
                Mahocvien = hocVien,
                Hinhthucrn = hinhThuc,
                Lydo = lyDo,
                Diadiem = diaDiem,
                Thoigianra = thoiGianRa,
                Thoigianvao = thoiGianVao,
                Tinhtrang = 1
            };
            obj.Chitietdanhsaches.Add(ct);
            obj.SaveChanges();
            return Json(new
            {
                status = true
            });
        }
        public IActionResult Xoa(int id)
        {
            var ct = obj.Chitietdanhsaches.Find(id);
            if (ct != null)
            {
                obj.Chitietdanhsaches.Remove(ct);
                obj.SaveChanges();
                return Json(new
                {
                    status = true
                });
            }
            else
            {
                // Xử lý trường hợp tk là null (nếu cần)
                return Json(new
                {
                    status = false,
                    message = "Không tìm thấy danh sách"
                });
            }

        }


        #endregion
        #region Quản lý quân nhân
        public IActionResult QuanLyQuanNhan()
        {
            List<TT_QN> resultList = new List<TT_QN>();
            var dv = obj.Donvis.ToList();
            var cv = obj.Chucvus.ToList();
            var cb = obj.Capbacs.ToList();
            using (var connection = obj.Database.GetDbConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SP_GetAllQuannhans";
                    command.CommandType = CommandType.StoredProcedure;

                    var pRefCursor = new Oracle.ManagedDataAccess.Client.OracleParameter("p_refCursor",
                        Oracle.ManagedDataAccess.Client.OracleDbType.RefCursor, System.Data.ParameterDirection.Output);
                    command.Parameters.Add(pRefCursor);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var item = new TT_QN
                            {
                                MaQn = reader.GetInt32(reader.GetOrdinal("Maqn")),
                                HoTen = reader.GetString(reader.GetOrdinal("Hoten")),
                                MaCv = reader.GetInt32(reader.GetOrdinal("Macv")),
                                MaDv = reader.GetInt32(reader.GetOrdinal("Madv")),
                                MaCapBac = reader.GetInt32(reader.GetOrdinal("Macapbac")),
                                CapBac1 = reader.GetString(reader.GetOrdinal("Capbac")),
                                TenCv = reader.GetString(reader.GetOrdinal("Tencv")),
                                TenDv = reader.GetString(reader.GetOrdinal("Tendv")),
                                DiaChi = reader.GetString(reader.GetOrdinal("Diachi"))
                            };
                            resultList.Add(item);
                        }
                    }
                }
            }

            ViewBag.ChonDonVi = dv;
            ViewBag.ChonChucVu = cv;
            ViewBag.ChonCapBac = cb;

            return View(resultList);
        }


        [HttpPost]
        public ActionResult ThemQuanNhan(string hoten, int macv, int madv, int macapbac, string diachi)
        {
            using (var connection = obj.Database.GetDbConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SP_InsertQuannhan";
                    command.CommandType = CommandType.StoredProcedure;

                    var pHoten = new Oracle.ManagedDataAccess.Client.OracleParameter("p_hoten", Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2, hoten, System.Data.ParameterDirection.Input);
                    var pMacv = new Oracle.ManagedDataAccess.Client.OracleParameter("p_macv", Oracle.ManagedDataAccess.Client.OracleDbType.Int32, macv, System.Data.ParameterDirection.Input);
                    var pMadv = new Oracle.ManagedDataAccess.Client.OracleParameter("p_madv", Oracle.ManagedDataAccess.Client.OracleDbType.Int32, madv, System.Data.ParameterDirection.Input);
                    var pMacapbac = new Oracle.ManagedDataAccess.Client.OracleParameter("p_macapbac", Oracle.ManagedDataAccess.Client.OracleDbType.Int32, macapbac, System.Data.ParameterDirection.Input);
                    var pDiachi = new Oracle.ManagedDataAccess.Client.OracleParameter("p_diachi", Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2, diachi, System.Data.ParameterDirection.Input);

                    command.Parameters.Add(pHoten);
                    command.Parameters.Add(pMacv);
                    command.Parameters.Add(pMadv);
                    command.Parameters.Add(pMacapbac);
                    command.Parameters.Add(pDiachi);

                    command.ExecuteNonQuery();
                }
            }

            return Json(new { status = true });
        }

        public IActionResult XoaQuanNhan(int maqn)
        {
            using (var connection = obj.Database.GetDbConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SP_DeleteQuannhan";
                    command.CommandType = CommandType.StoredProcedure;

                    var pMaqn = new Oracle.ManagedDataAccess.Client.OracleParameter("p_maqn", Oracle.ManagedDataAccess.Client.OracleDbType.Int32, maqn, System.Data.ParameterDirection.Input);
                    command.Parameters.Add(pMaqn);

                    int rowsAffected = command.ExecuteNonQuery();
                    return Json(new { status = (rowsAffected > 0) });
                }
            }
        }

        public IActionResult SuaQuanNhan(int maqn)
        {
            List<TT_QN> query1 = new List<TT_QN>();

            using (var connection = obj.Database.GetDbConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SP_GetQuannhanById";
                    command.CommandType = CommandType.StoredProcedure;

                    var pMaqn = new Oracle.ManagedDataAccess.Client.OracleParameter("p_maqn", Oracle.ManagedDataAccess.Client.OracleDbType.Int32, maqn, System.Data.ParameterDirection.Input);
                    var pRefCursor = new Oracle.ManagedDataAccess.Client.OracleParameter("p_refCursor", Oracle.ManagedDataAccess.Client.OracleDbType.RefCursor, System.Data.ParameterDirection.Output);

                    command.Parameters.Add(pMaqn);
                    command.Parameters.Add(pRefCursor);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var item = new TT_QN
                            {
                                MaQn = reader.GetInt32(reader.GetOrdinal("Maqn")),
                                HoTen = reader.GetString(reader.GetOrdinal("Hoten")),
                                MaCv = reader.GetInt32(reader.GetOrdinal("Macv")),
                                MaDv = reader.GetInt32(reader.GetOrdinal("Madv")),
                                MaCapBac = reader.GetInt32(reader.GetOrdinal("Macapbac")),
                                CapBac1 = reader.GetString(reader.GetOrdinal("Capbac")),
                                TenCv = reader.GetString(reader.GetOrdinal("Tencv")),
                                TenDv = reader.GetString(reader.GetOrdinal("Tendv")),
                                DiaChi = reader.GetString(reader.GetOrdinal("Diachi"))
                            };
                            query1.Add(item);
                        }
                    }
                }
            }
            HttpContext.Session.SetJson("QuanNhan", query1);

            return View();
        }

        [HttpPost]
        public IActionResult SuaQuanNhan(int maqn, string hoten, int macv, int madv, int macapbac, string diachi)
        {
            using (var connection = obj.Database.GetDbConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SP_UpdateQuannhan";
                    command.CommandType = CommandType.StoredProcedure;

                    var pMaqn = new Oracle.ManagedDataAccess.Client.OracleParameter("p_maqn", Oracle.ManagedDataAccess.Client.OracleDbType.Int32, maqn, System.Data.ParameterDirection.Input);
                    var pHoten = new Oracle.ManagedDataAccess.Client.OracleParameter("p_hoten", Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2, hoten, System.Data.ParameterDirection.Input);
                    var pMacv = new Oracle.ManagedDataAccess.Client.OracleParameter("p_macv", Oracle.ManagedDataAccess.Client.OracleDbType.Int32, macv, System.Data.ParameterDirection.Input);
                    var pMadv = new Oracle.ManagedDataAccess.Client.OracleParameter("p_madv", Oracle.ManagedDataAccess.Client.OracleDbType.Int32, madv, System.Data.ParameterDirection.Input);
                    var pMacapbac = new Oracle.ManagedDataAccess.Client.OracleParameter("p_macapbac", Oracle.ManagedDataAccess.Client.OracleDbType.Int32, macapbac, System.Data.ParameterDirection.Input);
                    var pDiachi = new Oracle.ManagedDataAccess.Client.OracleParameter("p_diachi", Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2, diachi, System.Data.ParameterDirection.Input);

                    command.Parameters.Add(pMaqn);
                    command.Parameters.Add(pHoten);
                    command.Parameters.Add(pMacv);
                    command.Parameters.Add(pMadv);
                    command.Parameters.Add(pMacapbac);
                    command.Parameters.Add(pDiachi);

                    int rowsAffected = command.ExecuteNonQuery();
                    return Json(new { status = (rowsAffected > 0) });
                }
            }
        }

        #endregion
        #region Quản lý danh sách ra ngoài- giấy tờ
        public IActionResult QuanLyDSGT()
        {
            List<Rangoai> rn = obj.Rangoais.ToList(); // Nếu muốn chuyển sang SP, tạo SP_GetAllRangoais tương tự.
            HttpContext.Session.SetJson("RN", rn);
            List<DSGT> pagedList = new List<DSGT>();

            using (var conn = obj.Database.GetDbConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SP_GetAllDSGT";
                    cmd.CommandType = CommandType.StoredProcedure;

                    var pRefCursor = new Oracle.ManagedDataAccess.Client.OracleParameter("p_refCursor",
                        Oracle.ManagedDataAccess.Client.OracleDbType.RefCursor, System.Data.ParameterDirection.Output);
                    cmd.Parameters.Add(pRefCursor);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var item = new DSGT
                            {
                                MaCtds = reader.GetInt32(reader.GetOrdinal("MaCtds")),
                                MaHocVien = reader.GetInt32(reader.GetOrdinal("MaHocVien")),
                                MaGiayTo = reader.GetInt32(reader.GetOrdinal("MaGiayTo")),
                                SoGiay = reader.GetInt32(reader.GetOrdinal("SoGiay")),
                                ThoiGianLay = reader.GetDateTime(reader.GetOrdinal("ThoiGianLay")),
                                ThoiGianTra = reader.IsDBNull(reader.GetOrdinal("ThoiGianTra")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("ThoiGianTra")),
                                DaTra = reader.GetBoolean(reader.GetOrdinal("DaTra")), // Kiểm tra kiểu dữ liệu cột Tinhtrang trong Giaytos
                                MaCv = reader.GetInt32(reader.GetOrdinal("MaCv")),
                                MaDv = reader.GetInt32(reader.GetOrdinal("MaDv")),
                                MaCapBac = reader.GetInt32(reader.GetOrdinal("MaCapBac")),
                                CapBac1 = reader.GetString(reader.GetOrdinal("Capbac")),
                                TenCv = reader.GetString(reader.GetOrdinal("TenCv")),
                                TenDv = reader.GetString(reader.GetOrdinal("TenDv")),
                                TinhTrang = reader.GetInt32(reader.GetOrdinal("TinhTrang")),
                                ThoiGianRa = reader.GetDateTime(reader.GetOrdinal("ThoiGianRa")),
                                ThoiGianVao = reader.IsDBNull(reader.GetOrdinal("ThoiGianVao")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("ThoiGianVao")),
                                HoTen = reader.GetString(reader.GetOrdinal("HoTen"))
                            };
                            pagedList.Add(item);
                        }
                    }
                }
            }


            return View(pagedList);
        }

        public IActionResult RaNgoai(int id, int magiay, DateTime thoigianra)
        {
            // int max = obj.Rangoais.Max(r => r.MaRn);
            Rangoai rn = new Rangoai();
            rn.Thoigianra = thoigianra;
            // rn.MaRn = max + 1;
            rn.Magiayto = magiay;
            rn.Mactds = id;
            obj.Add(rn);
            obj.SaveChanges();
            return RedirectToAction("QuanLyDSGT", "Admin");
            /* return Json(new
             {
                 status = false
             });*/

        }
        public IActionResult Vao(int id, int magiay)
        {
            var rn = obj.Rangoais.Where(c => c.Mactds == id && c.Magiayto == magiay).FirstOrDefault();
            rn.Thoigianvao = DateTime.Now;
            obj.Update(rn);
            obj.SaveChanges();
            return RedirectToAction("QuanLyDSGT", "Admin");

            /* return Json(new
             {
                 status = false
             });*/

        }
        #endregion
        #region Quản lý danh sách vi phạm
        public IActionResult QuanLyViPham()
        {
            List<HV_VP> pagedList = new List<HV_VP>();
            var qn = obj.Quannhans.ToList();
            using (var connection = obj.Database.GetDbConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SP_GetAllViPhams";
                    command.CommandType = CommandType.StoredProcedure;

                    var pRefCursor = new Oracle.ManagedDataAccess.Client.OracleParameter("p_refCursor",
                        Oracle.ManagedDataAccess.Client.OracleDbType.RefCursor, System.Data.ParameterDirection.Output);
                    command.Parameters.Add(pRefCursor);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var item = new HV_VP
                            {
                                MaQn = reader.GetInt32(reader.GetOrdinal("Maqn")),
                                HoTen = reader.GetString(reader.GetOrdinal("Hoten")),
                                MaVp = reader.GetInt32(reader.GetOrdinal("Mavp")),
                                MaDv = reader.GetInt32(reader.GetOrdinal("Madv")),
                                MaCapBac = reader.GetInt32(reader.GetOrdinal("Macapbac")),
                                MaCv = reader.GetInt32(reader.GetOrdinal("Macv")),
                                TenCv = reader.GetString(reader.GetOrdinal("Tencv")),
                                CapBac1 = reader.GetString(reader.GetOrdinal("Capbac")),
                                ThoiGian = reader.GetDateTime(reader.GetOrdinal("Thoigian")),
                                TenDv = reader.GetString(reader.GetOrdinal("Tendv")),
                                Ghichu = reader.IsDBNull(reader.GetOrdinal("Ghichu")) ? null : reader.GetString(reader.GetOrdinal("Ghichu")),
                                MoTa = reader.IsDBNull(reader.GetOrdinal("Mota")) ? null : reader.GetString(reader.GetOrdinal("Mota"))
                            };
                            pagedList.Add(item);
                        }
                    }
                }
            }
            
            ViewBag.ChonQuanNhan = qn;
            return View(pagedList);
        }

        [HttpPost]
        public ActionResult ThemViPham(string mota, DateTime thoigian, string ghichu, int mahv)
        {
            using (var connection = obj.Database.GetDbConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SP_InsertVipham";
                    command.CommandType = CommandType.StoredProcedure;

                    var pMota = new Oracle.ManagedDataAccess.Client.OracleParameter("p_mota", Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2, mota, System.Data.ParameterDirection.Input);
                    var pThoigian = new Oracle.ManagedDataAccess.Client.OracleParameter("p_thoigian", Oracle.ManagedDataAccess.Client.OracleDbType.Date, thoigian, System.Data.ParameterDirection.Input);
                    var pGhichu = new Oracle.ManagedDataAccess.Client.OracleParameter("p_ghichu", Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2, ghichu, System.Data.ParameterDirection.Input);
                    var pMahv = new Oracle.ManagedDataAccess.Client.OracleParameter("p_mahv", Oracle.ManagedDataAccess.Client.OracleDbType.Int32, mahv, System.Data.ParameterDirection.Input);

                    command.Parameters.Add(pMota);
                    command.Parameters.Add(pThoigian);
                    command.Parameters.Add(pGhichu);
                    command.Parameters.Add(pMahv);

                    command.ExecuteNonQuery();
                }
            }

            return Json(new { status = true });
        }

        public IActionResult XoaViPham(int mavp)
        {
            using (var connection = obj.Database.GetDbConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SP_DeleteVipham";
                    command.CommandType = CommandType.StoredProcedure;

                    var pMavp = new Oracle.ManagedDataAccess.Client.OracleParameter("p_mavp", Oracle.ManagedDataAccess.Client.OracleDbType.Int32, mavp, System.Data.ParameterDirection.Input);
                    command.Parameters.Add(pMavp);

                    int rowsAffected = command.ExecuteNonQuery();
                    return Json(new { status = (rowsAffected > 0) });
                }
            }
        }


        public IActionResult SuaViPham(int mavp)
        {
            Models.Vipham model = null;

            using (var connection = obj.Database.GetDbConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SP_GetViphamById";
                    command.CommandType = CommandType.StoredProcedure;

                    var pMavp = new Oracle.ManagedDataAccess.Client.OracleParameter("p_mavp", Oracle.ManagedDataAccess.Client.OracleDbType.Int32, mavp, System.Data.ParameterDirection.Input);
                    var pRefCursor = new Oracle.ManagedDataAccess.Client.OracleParameter("p_refCursor", Oracle.ManagedDataAccess.Client.OracleDbType.RefCursor, System.Data.ParameterDirection.Output);

                    command.Parameters.Add(pMavp);
                    command.Parameters.Add(pRefCursor);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            model = new Models.Vipham
                            {
                                Mavp = reader.GetInt32(reader.GetOrdinal("Mavp")),
                                Mota = reader.GetString(reader.GetOrdinal("Mota")),
                                Thoigian = reader.GetDateTime(reader.GetOrdinal("Thoigian")),
                                Ghichu = reader.IsDBNull(reader.GetOrdinal("Ghichu")) ? null : reader.GetString(reader.GetOrdinal("Ghichu")),
                                Mahv = reader.GetInt32(reader.GetOrdinal("Mahv"))
                            };
                        }
                    }
                }
            }

            ViewBag.ChonQuanNhan = obj.Quannhans.ToList();

            if (model != null)
            {
                return View(model);
            }
            else
            {
                return Json(new { status = false });
            }
        }

        [HttpPost]
        public IActionResult SuaViPham(int mavp, string mota, DateTime thoigian, string ghichu, int mahv)
        {
            using (var connection = obj.Database.GetDbConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SP_UpdateVipham";
                    command.CommandType = CommandType.StoredProcedure;

                    var pMavp = new Oracle.ManagedDataAccess.Client.OracleParameter("p_mavp", Oracle.ManagedDataAccess.Client.OracleDbType.Int32, mavp, System.Data.ParameterDirection.Input);
                    var pMota = new Oracle.ManagedDataAccess.Client.OracleParameter("p_mota", Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2, mota, System.Data.ParameterDirection.Input);
                    var pThoigian = new Oracle.ManagedDataAccess.Client.OracleParameter("p_thoigian", Oracle.ManagedDataAccess.Client.OracleDbType.Date, thoigian, System.Data.ParameterDirection.Input);
                    var pGhichu = new Oracle.ManagedDataAccess.Client.OracleParameter("p_ghichu", Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2, ghichu, System.Data.ParameterDirection.Input);
                    var pMahv = new Oracle.ManagedDataAccess.Client.OracleParameter("p_mahv", Oracle.ManagedDataAccess.Client.OracleDbType.Int32, mahv, System.Data.ParameterDirection.Input);

                    command.Parameters.Add(pMavp);
                    command.Parameters.Add(pMota);
                    command.Parameters.Add(pThoigian);
                    command.Parameters.Add(pGhichu);
                    command.Parameters.Add(pMahv);

                    int rowsAffected = command.ExecuteNonQuery();
                    return Json(new { status = (rowsAffected > 0) });
                }
            }
        }

        #endregion

        #region Lịch sử ra ngoài
        public IActionResult LichSu()
        {
            // Giờ chỉ cần lấy từ bảng LICH_SU_JSON
            var lichSuJsonList = obj.LichSuJsons
                          .FromSqlRaw("SELECT JSON_DATA FROM LICH_SU_JSON")
                          .Select(x => x.JsonData)
                          .ToList();


            // Deserialize từ JSON sang model LS (cần dùng Newtonsoft.Json hoặc System.Text.Json)
            List<LS> model = new List<LS>();
            foreach (var jsonStr in lichSuJsonList)
            {
                LS item = JsonConvert.DeserializeObject<LS>(jsonStr); // nếu dùng Newtonsoft.Json
                model.Add(item);
            }

            // Phần còn lại như cũ
            List<Quannhan> rn = obj.Quannhans.ToList();
            HttpContext.Session.SetJson("QN", rn);
            return View(model);
        }
        #endregion

    }
}
