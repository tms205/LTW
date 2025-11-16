using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DA_LTW.Models
{
    public class DashboardViewModel
    {
        public int DoanhThuHomNay { get; set; }
        public int SoBanDangDung { get; set; }
        public int DatBanHomNay { get; set; }
        public int NguyenLieuSapHet { get; set; }

        public List<ChartItem> DoanhThuTuan { get; set; }
        public List<ChartItem> TyLeLoaiMon { get; set; }
    }

    public class ChartItem
    {
        public string Ten { get; set; }
        public DateTime Ngay { get; set; }
        public int Tong { get; set; }
    }
}
