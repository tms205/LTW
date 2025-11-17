using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DA_LTW.Models;

namespace DA_LTW.Controllers
{
    public class LoaiMonController : Controller
    {
        QL_NHAHANGEntities data = new QL_NHAHANGEntities();

        // GET: Index
        public ActionResult Index()
        {
            var loaiMon = data.LOAIMONs.Include("MONANs").ToList();
            return View(loaiMon);
        }

        // GET: Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LOAIMON loai)
        {
            if (ModelState.IsValid)
            {
                data.LOAIMONs.Add(loai);
                data.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(loai);
        }

        // GET: Edit
        public ActionResult Edit(string id)
        {
            var loai = data.LOAIMONs.Find(id);
            if (loai == null) return HttpNotFound();
            return View(loai);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LOAIMON loai)
        {
            if (ModelState.IsValid)
            {
                data.Entry(loai).State = System.Data.Entity.EntityState.Modified;
                data.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(loai);
        }

        // GET: Delete
        public ActionResult Delete(string id)
        {
            var loai = data.LOAIMONs.Find(id);
            if (loai == null) return HttpNotFound();
            return View(loai);
        }

        // POST: DeleteConfirmed
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var loai = data.LOAIMONs.Find(id);
            if (loai != null)
            {
                data.LOAIMONs.Remove(loai);
                data.SaveChanges();
            }
            return RedirectToAction("Index");
        }


        // Lọc món theo mã loại
        public ActionResult MonTheoLoai(string id)
        {
            var loai = data.LOAIMONs.Find(id);
            if (loai == null) return HttpNotFound();

            ViewBag.TenLoai = loai.TenLoai;

            var mon = data.MONANs.Where(m => m.MaLoai == id).ToList();
            return View(mon);
        }
    }
}
