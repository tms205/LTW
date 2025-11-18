using DA_LTW.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DA_LTW.Controllers
{
    public class TaiKhoanController : Controller
    {
        public QL_NHAHANGEntities db = new QL_NHAHANGEntities();

        // GET: TaiKhoan
        public ActionResult DangKy()
        {
            return View();
        }

        public ActionResult DangNhap()
        {
            return View();
        }

        public ActionResult TimKiem(string keyword)
        {
            List<MONAN> dsMon = new List<MONAN>();

            if (string.IsNullOrEmpty(keyword))
            {
                // Không có keyword → hiển thị tất cả món
                dsMon = db.MONANs.ToList();
            }
            else
            {
                // Tìm theo keyword
                var ketQua = db.MONANs
                               .Where(m => m.TenMon.Contains(keyword) || m.MoTa.Contains(keyword))
                               .ToList();

                if (ketQua.Count == 0)
                {
                    // Không tìm thấy món → hiển thị tất cả món
                    dsMon = db.MONANs.ToList();
                }
                else
                {
                    // Tìm thấy 1 hoặc nhiều món → hiển thị kết quả tìm kiếm
                    dsMon = ketQua;
                }
            }

            return View(dsMon);
        }
    }
}
