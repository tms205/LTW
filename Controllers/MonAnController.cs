using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DA_LTW.Models;

namespace DA_LTW.Controllers
{
    public class MonAnController : Controller
    {
        QL_NHAHANGEntities data = new QL_NHAHANGEntities();

        // GET: MonAn
        public ActionResult Index()
        {
            List<MONAN> ds = data.MONANs.ToList();
            ViewBag.ActiveMenu = "MonAn";
            return View(ds);
        }

        // GET: Chi tiết món ăn (dành cho khách xem chi tiết sản phẩm)
        public ActionResult ChiTiet(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }

            // Lấy món hiện tại
            var monAn = data.MONANs.SingleOrDefault(m => m.MaMon == id);
            if (monAn == null)
            {
                return HttpNotFound();
            }

            // Lấy danh sách món liên quan cùng loại (trừ món hiện tại)
            var monLienQuan = data.MONANs
                .Where(m => m.MaLoai == monAn.MaLoai && m.MaMon != monAn.MaMon)
                .OrderByDescending(m => m.Gia)
                .Take(4)
                .ToList();

            ViewBag.MonLienQuan = monLienQuan;

            return View(monAn);
        }

        // Giữ lại action Details để các phần khác trong dự án vẫn dùng được
        public ActionResult Details(string id)
        {
            var monAn = data.MONANs.Find(id);
            if (monAn == null)
            {
                return HttpNotFound();
            }
            return View(monAn);
        }

        // GET: Tạo món ăn mới
        public ActionResult Create()
        {
            return View();
        }

        // POST: Tạo món ăn mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MONAN monAn)
        {
            if (ModelState.IsValid)
            {
                data.MONANs.Add(monAn);
                data.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(monAn);
        }

        // GET: Sửa món ăn
        public ActionResult Edit(string id)
        {
            var monAn = data.MONANs.Find(id);
            if (monAn == null)
            {
                return HttpNotFound();
            }
            return View(monAn);
        }

        // POST: Sửa món ăn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MONAN monAn)
        {
            if (ModelState.IsValid)
            {
                data.Entry(monAn).State = EntityState.Modified;
                data.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(monAn);
        }

        // GET: Xóa món ăn
        public ActionResult Delete(string id)
        {
            var monAn = data.MONANs.Find(id);
            if (monAn == null)
            {
                return HttpNotFound();
            }
            return View(monAn);
        }

        // POST: Xóa món ăn
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)   // <-- đổi từ int sang string
        {
            var monAn = data.MONANs.Find(id);
            if (monAn == null)
            {
                return HttpNotFound();
            }

            data.MONANs.Remove(monAn);
            data.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                data.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
