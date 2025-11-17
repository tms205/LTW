using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DA_LTW.Models
{
    public class DonHangViewModel
    {
        public string MaHD { get; set; }

        [Display(Name = "Ngày lập")]
        public DateTime? NgayLap { get; set; }

        [Display(Name = "Tổng tiền")]
        public int? TongTien { get; set; }

        [Display(Name = "Phương thức")]
        public string PhuongThucTT { get; set; }

        [Display(Name = "Nhân viên")]
        public string TenNV { get; set; }

        [Display(Name = "Bàn")]
        public string TenBan { get; set; }

        [Display(Name = "Khách hàng")]
        public string TenKH { get; set; }

        public List<ChiTietDonHang> ChiTiet { get; set; } = new List<ChiTietDonHang>();
    }

    public class MonChonViewModel
    {
        public string MaMon { get; set; }
        public int SoLuong { get; set; }
    }
    public class ChiTietDonHang
    {
        public string MaMon { get; set; }
        public string TenMon { get; set; }
        public int SoLuong { get; set; }
        public int? DonGia { get; set; }
        public int ThanhTien => SoLuong * (DonGia ?? 0);
    }

    // Dùng để tạo đơn mới
    public class TaoDonHangViewModel
    {
        public string MaDatBan { get; set; }
        public string MaNV { get; set; }
        public List<ChiTietOrder> MonDaChon { get; set; } = new List<ChiTietOrder>();
    }

    public class ChiTietOrder
    {
        public string MaMon { get; set; }
        public int SoLuong { get; set; }
    }
}