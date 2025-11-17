using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DA_LTW.Models;
using System.IO;

namespace DA_LTW.Controllers
{
    public class MonAnController : Controller
    {
        QL_NHAHANGEntities data = new QL_NHAHANGEntities();

        // GET: Danh sách món ăn
        public ActionResult Index()
        {
            List<MONAN> ds = data.MONANs.ToList();
            return View(ds);
        }

        // GET: Chi tiết món ăn
        public ActionResult Details(string id)
        {
            var monAn = data.MONANs.Find(id);
            if (monAn == null)
                return HttpNotFound();

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
        public ActionResult Create(MONAN monAn, HttpPostedFileBase fileUpload)
        {
            // 1. Check trùng mã
            if (data.MONANs.Find(monAn.MaMon) != null)
                ModelState.AddModelError("MaMon", "⚠ Mã món ăn đã tồn tại!");

            // 2. Check loại món có tồn tại
            if (data.LOAIMONs.Find(monAn.MaLoai) == null)
                ModelState.AddModelError("MaLoai", "⚠ Mã loại không tồn tại!");

            // Xử lý upload ảnh
            if (fileUpload != null && fileUpload.ContentLength > 0)
            {
                string fileName = Path.GetFileName(fileUpload.FileName);
                string savePath = Server.MapPath("~/Content/img/monan/" + fileName);

                fileUpload.SaveAs(savePath);
                monAn.HinhAnh = "/Content/img/monan/" + fileName;
            }
            else
            {
                ModelState.AddModelError("HinhAnh", "Bạn phải chọn hình ảnh!");
            }

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
                return HttpNotFound();

            return View(monAn);
        }

        // POST: Sửa món ăn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MONAN monAn, HttpPostedFileBase imageFile)
        {
            var monAnDb = data.MONANs.Find(monAn.MaMon);

            if (monAnDb == null)
                return HttpNotFound();

            if (ModelState.IsValid)
            {
                // Cập nhật text
                monAnDb.TenMon = monAn.TenMon;
                monAnDb.Gia = monAn.Gia;
                monAnDb.DonViTinh = monAn.DonViTinh;
                monAnDb.TrangThai = monAn.TrangThai;
                monAnDb.MaLoai = monAn.MaLoai;
                monAnDb.MoTa = monAn.MoTa;

                // Upload ảnh mới nếu có
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(imageFile.FileName);
                    string path = "/Content/img/monan/" + fileName;

                    string physicalPath = Server.MapPath(path);
                    imageFile.SaveAs(physicalPath);

                    monAnDb.HinhAnh = path;
                }

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
                return HttpNotFound();

            return View(monAn);
        }

        // POST: Xác nhận xóa
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var monAn = data.MONANs.Find(id);
            if (monAn != null)
            {
                data.MONANs.Remove(monAn);
                data.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                data.Dispose();

            base.Dispose(disposing);
        }
    }
}
