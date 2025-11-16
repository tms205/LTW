using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DA_LTW.Models;

namespace DA_LTW.Controllers
{
    public class DashBoardController : Controller
    {
        private readonly QL_NHAHANGEntities db = new QL_NHAHANGEntities();

        public ActionResult Index()
        {
            var model = new DashboardViewModel();

            DateTime today = DateTime.Today;
            DateTime tomorrow = today.AddDays(1);

            // Doanh thu hôm nay
            model.DoanhThuHomNay = db.HOADONs
                .Where(h => h.NgayLap.HasValue
                    && h.NgayLap.Value >= today
                    && h.NgayLap.Value < tomorrow
                    && h.TongTien > 0)
                .Sum(h => (int?)h.TongTien) ?? 0;

            // Số bàn đang dùng
            model.SoBanDangDung = db.BANANs
                .Count(b => b.TrangThai == "Đang dùng");

            // Đặt bàn hôm nay
            model.DatBanHomNay = db.DATBANs
                .Where(d => d.NgayDat.HasValue
                    && d.NgayDat.Value >= today
                    && d.NgayDat.Value < tomorrow)
                .Count();

            // Nguyên liệu sắp hết
            model.NguyenLieuSapHet = db.NGUYENLIEUx
                .Count(n => n.SoLuongTon < n.MucCanhBao);

            // Biểu đồ doanh thu theo ngày (không dùng Date trong GroupBy)
            var doanhThuRaw = db.HOADONs
    .Where(h => h.NgayLap.HasValue)
    .GroupBy(h => new
    {
        Year = h.NgayLap.Value.Year,
        Month = h.NgayLap.Value.Month,
        Day = h.NgayLap.Value.Day
    })
    .Select(g => new
    {
        g.Key.Year,
        g.Key.Month,
        g.Key.Day,
        Tong = g.Sum(x => (int?)x.TongTien) ?? 0
    })
    .ToList();

            // Chuyển sang ChartItem sau khi EF đã xử lý xong
            model.DoanhThuTuan = doanhThuRaw
                .Select(x => new ChartItem
                {
                    Ngay = new DateTime(x.Year, x.Month, x.Day), // xử lý ngoài LINQ → OK
                    Tong = x.Tong
                })
                .OrderBy(x => x.Ngay)
                .ToList();


            // Biểu đồ tỷ lệ loại món
            model.TyLeLoaiMon = db.CHITIETHOADONs
                .GroupBy(c => c.MONAN.LOAIMON.TenLoai)
                .Select(g => new ChartItem
                {
                    Ten = g.Key,
                    Tong = g.Sum(x => (int?)x.SoLuong) ?? 0
                })
                .ToList();

            return View(model);
        }
    }
}
