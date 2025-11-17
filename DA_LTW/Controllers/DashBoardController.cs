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
            DateTime sevenDaysAgo = today.AddDays(-6); // Bao gồm hôm nay + 6 ngày trước = 7 ngày

            // Doanh thu hôm nay: Tổng tiền từ hóa đơn hợp lệ (TongTien > 0, không hủy)
            model.DoanhThuHomNay = db.HOADONs
                .Where(h => h.NgayLap.HasValue
                    && h.NgayLap.Value >= today
                    && h.NgayLap.Value < tomorrow
                    && h.TongTien > 0
                    && h.MaDatBan != null) // Đảm bảo liên kết với đặt bàn hợp lệ (không hủy)
                .Sum(h => (int?)h.TongTien) ?? 0;

            // Số bàn đang phục vụ: Bàn có trạng thái 'Đang dùng' (từ bảng BANAN và liên kết với DATBAN đang dùng)
            model.SoBanDangDung = db.DATBANs
                .Count(d => d.TrangThai == "Đang dùng");

            // Đặt bàn hôm nay: Số lượng đặt bàn trong ngày hôm nay (không kể trạng thái hủy)
            model.DatBanHomNay = db.DATBANs
                .Where(d => d.NgayDat.HasValue
                    && d.NgayDat.Value >= today
                    && d.NgayDat.Value < tomorrow)
                .Count();

            // Nguyên liệu sắp hết: Số lượng nguyên liệu có SoLuongTon < MucCanhBao (từ bảng NGUYENLIEU)
            model.NguyenLieuSapHet = db.NGUYENLIEUx
                .Count(n => n.SoLuongTon < n.MucCanhBao);

            // Biểu đồ doanh thu 7 ngày gần nhất (chỉ lấy dữ liệu 7 ngày, group by ngày)
            var doanhThuRaw = db.HOADONs
                .Where(h => h.NgayLap.HasValue
                    && h.NgayLap.Value >= sevenDaysAgo
                    && h.NgayLap.Value < tomorrow
                    && h.TongTien > 0)
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

            // Chuyển sang ChartItem và sắp xếp theo ngày (đảm bảo đủ 7 ngày, nếu không có dữ liệu thì Tong=0)
            var allDates = Enumerable.Range(0, 7).Select(i => today.AddDays(-i)).ToList();
            model.DoanhThuTuan = allDates
                .Select(d => new ChartItem
                {
                    Ngay = d,
                    Tong = doanhThuRaw
                        .Where(x => x.Year == d.Year && x.Month == d.Month && x.Day == d.Day)
                        .Select(x => x.Tong)
                        .FirstOrDefault()
                })
                .OrderBy(x => x.Ngay) // Sắp xếp từ cũ đến mới
                .ToList();

            // Biểu đồ tỷ lệ bán theo loại món: Group by TenLoai, sum SoLuong từ CHITIETHOADON (tỷ lệ số lượng bán)
            // Để sát thực tế, chỉ lấy dữ liệu từ hóa đơn đã thanh toán (TongTien > 0)
            model.TyLeLoaiMon = db.CHITIETHOADONs
                .Where(c => c.HOADON.TongTien > 0)
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