using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using DA_LTW.Models;

namespace DA_LTW.Controllers
{
    public class DonHangController : Controller
    {
        private readonly QL_NHAHANGEntities db = new QL_NHAHANGEntities();

        // ================================
        // 1. Danh sách đơn hàng + tìm kiếm + lọc
        // ================================
        public ActionResult Index(string search = "", string trangthai = "")
        {
            var query = db.HOADONs.Include(h => h.DATBAN)
                                   .Include(h => h.NHANVIEN)
                                   .AsQueryable();

            // Tìm kiếm theo mã hoặc tên khách
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(h =>
                    h.MaHD.Contains(search) ||
                    h.DATBAN.KHACHHANG.HoTen.Contains(search));
            }

            // Lọc theo trạng thái đặt bàn (Đang dùng / Hủy / Đã thanh toán)
            if (!string.IsNullOrWhiteSpace(trangthai))
            {
                query = query.Where(h => h.DATBAN.TrangThai == trangthai);
            }

            var donHang = query
                .OrderByDescending(h => h.NgayLap)
                .Select(h => new DonHangViewModel
                {
                    MaHD = h.MaHD,
                    NgayLap = h.NgayLap,
                    TongTien = h.TongTien,
                    PhuongThucTT = h.PhuongThucTT,
                    TenNV = h.NHANVIEN.HoTen,
                    TenBan = h.DATBAN.BANAN.TenBan,
                    TenKH = h.DATBAN.KHACHHANG.HoTen
                }).ToList();

            ViewBag.Search = search;
            ViewBag.TrangThai = trangthai;

            return View(donHang);
        }

        // ================================
        // 2. Chi tiết đơn hàng
        // ================================
        public ActionResult Detail(string id)
        {
            if (id == null) return HttpNotFound();

            var hoaDon = db.HOADONs
                .Where(h => h.MaHD == id)
                .Include(h => h.CHITIETHOADONs)
                .Select(h => new DonHangViewModel
                {
                    MaHD = h.MaHD,
                    NgayLap = h.NgayLap,
                    TongTien = h.TongTien,
                    PhuongThucTT = h.PhuongThucTT,
                    TenNV = h.NHANVIEN.HoTen,
                    TenBan = h.DATBAN.BANAN.TenBan,
                    TenKH = h.DATBAN.KHACHHANG.HoTen,
                    ChiTiet = h.CHITIETHOADONs.Select(c => new ChiTietDonHang
                    {
                        MaMon = c.MaMon,
                        TenMon = c.MONAN.TenMon,
                        SoLuong = c.SoLuong ?? 0,
                        DonGia = c.DonGia
                    }).ToList()
                }).FirstOrDefault();

            if (hoaDon == null) return HttpNotFound();

            return View(hoaDon);
        }

        // ================================
        // 3. Tạo đơn hàng (chọn bàn → chọn món)
        // ================================
        public ActionResult Create()
        {
            var monAnList = db.MONANs.Where(m => m.TrangThai == "Còn món").ToList();
            ViewBag.MonAn = monAnList;

            ViewBag.MaDatBan = new SelectList(
                db.DATBANs
                  .Where(d => d.TrangThai == "Đang dùng" &&
                         !db.HOADONs.Any(h => h.MaDatBan == d.MaDatBan)),
                "MaDatBan", "MaDatBan");

            ViewBag.MaNV = new SelectList(db.NHANVIENs, "MaNV", "HoTen");

            // FIX LỖI INDEX OUT OF RANGE
            var model = new TaoDonHangViewModel
            {
                MonDaChon = monAnList.Select(m => new ChiTietOrder
                {
                    MaMon = m.MaMon,
                    SoLuong = 0
                }).ToList()
            };


            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TaoDonHangViewModel model)
        {
            if (model.MonDaChon == null || !model.MonDaChon.Any(m => m.SoLuong > 0))
            {
                ModelState.AddModelError("", "Chưa chọn món ăn!");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.MaDatBan = new SelectList(
                    db.DATBANs.Where(d => d.TrangThai == "Đang dùng"),
                    "MaDatBan", "MaDatBan");

                ViewBag.MaNV = new SelectList(db.NHANVIENs, "MaNV", "HoTen");
                ViewBag.MonAn = db.MONANs.Where(m => m.TrangThai == "Còn món").ToList();

                return View(model);
            }

            using (var tran = db.Database.BeginTransaction())
            {
                try
                {
                    // Tạo hóa đơn
                    var hd = new HOADON
                    {
                        MaHD = TaoMaHoaDon(),
                        MaDatBan = model.MaDatBan,
                        MaNV = model.MaNV,
                        NgayLap = DateTime.Now,
                        TongTien = 0,
                        PhuongThucTT = "Tiền mặt"
                    };
                    db.HOADONs.Add(hd);

                    int tongTien = 0;

                    foreach (var item in model.MonDaChon.Where(m => m.SoLuong > 0))
                    {
                        var mon = db.MONANs.Find(item.MaMon);

                        if (mon == null || mon.TrangThai != "Còn món")
                        {
                            tran.Rollback();
                            ModelState.AddModelError("", $"Món {mon?.TenMon} đã hết!");
                            return View(model);
                        }

                        var ct = new CHITIETHOADON
                        {
                            MaHD = hd.MaHD,
                            MaMon = item.MaMon,
                            SoLuong = item.SoLuong,
                            DonGia = mon.Gia ?? 0
                        };

                        db.CHITIETHOADONs.Add(ct);
                        tongTien += item.SoLuong * (mon.Gia ?? 0);
                    }

                    hd.TongTien = tongTien;

                    // Cập nhật trạng thái bàn
                    var datBan = db.DATBANs.Find(model.MaDatBan);
                    datBan.TrangThai = "Đang dùng";

                    db.SaveChanges();
                    tran.Commit();

                    return RedirectToAction("ChiTiet", new { id = hd.MaHD });
                }
                catch
                {
                    tran.Rollback();
                    ModelState.AddModelError("", "Có lỗi xảy ra khi tạo đơn hàng!");
                }
            }

            return View(model);
        }

        // ================================
        // 4. Hủy đơn (chỉ hủy đơn chưa tính tiền)
        // ================================
        public ActionResult Huy(string id)
        {
            var hd = db.HOADONs.Find(id);
            if (hd == null) return RedirectToAction("Index");

            if (hd.TongTien > 0)
            {
                TempData["Err"] = "Không thể hủy hóa đơn đã có tổng tiền!";
                return RedirectToAction("Index");
            }

            // Trả trạng thái bàn về "Đang dùng"
            var datBan = db.DATBANs.Find(hd.MaDatBan);
            if (datBan != null)
                datBan.TrangThai = "Đang dùng";

            // Xóa luôn chi tiết (nếu có)
            var ct = db.CHITIETHOADONs.Where(c => c.MaHD == id).ToList();
            foreach (var item in ct) db.CHITIETHOADONs.Remove(item);

            db.HOADONs.Remove(hd);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // ================================
        // 5. Sinh mã hóa đơn an toàn
        // ================================
        private string TaoMaHoaDon()
        {
            string prefix = "HD" + DateTime.Now.ToString("yyMMdd");
            int count = db.HOADONs.Count(h => h.MaHD.StartsWith(prefix)) + 1;
            return prefix + count.ToString("D3");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
