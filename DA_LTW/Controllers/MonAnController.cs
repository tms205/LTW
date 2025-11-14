using System;
using System.Collections.Generic;
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
            return View(ds);
        }

        // GET: Chi tiết món ăn
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
                data.Entry(monAn).State = System.Data.Entity.EntityState.Modified;
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
        public ActionResult DeleteConfirmed(int id)
        {
            var monAn = data.MONANs.Find(id);
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