using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EmpDeptTask;

namespace EmpDeptTask.Controllers
{
    public class EmpsController : Controller
    {
        private EmpDeptEntities db = new EmpDeptEntities();

        [HttpPost]
        public JsonResult SaveEmp(DeptVM data)
        {
            Dept deparment = new Dept();
            deparment.DeptId = data.DeptId;
            deparment.DeptName = data.DeptName;
            deparment.DeptLocation = data.DeptLocation;
            deparment.DeptJoining = data.DeptJoining;
            deparment.DeptLeaving = data.DeptLeaving;
            db.Depts.Add(deparment);


            var result = data.EmpVM.Select(x => new Emp
            {
                EmpId = x.EmpId,
                EmpName = x.EmpName,
                EmpCity = x.EmpCity,
                EmpState = x.EmpState,
                DeptId = deparment.DeptId,
            }).ToList();
            db.Emps.AddRange(result);
            db.SaveChanges();

            data.StatueCode = "200";
            data.ResponseMessage = "employee saved successfully";


            // Return the response as JSON
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        // GET: Emps
        public ActionResult Index()
        {
            var emps = db.Emps.Include(e => e.Dept);
            return View(emps.ToList());
        }

        // GET: Emps/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Emp emp = db.Emps.Find(id);
            if (emp == null)
            {
                return HttpNotFound();
            }
            return View(emp);
        }

        // GET: Emps/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Emps/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EmpId,EmpName,EmpCity,EmpState,DeptId")] Emp emp)
        {
            if (ModelState.IsValid)
            {
                db.Emps.Add(emp);
                db.SaveChanges();
                ViewData["ListValue"] = db.Emps.ToList();

                return RedirectToAction("Create");
            }
            ViewBag.DeptId = new SelectList(db.Depts, "DeptId", "DeptName", emp.DeptId);
            return View(emp);
        }

        // GET: Emps/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Emp emp = db.Emps.Find(id);
            if (emp == null)
            {
                return HttpNotFound();
            }
            ViewBag.DeptId = new SelectList(db.Depts, "DeptId", "DeptName", emp.DeptId);
            return View(emp);
        }

        // POST: Emps/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmpId,EmpName,EmpCity,EmpState,DeptId")] Emp emp)
        {
            if (ModelState.IsValid)
            {
                db.Entry(emp).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DeptId = new SelectList(db.Depts, "DeptId", "DeptName", emp.DeptId);
            return View(emp);
        }

        // GET: Emps/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Emp emp = db.Emps.Find(id);
            if (emp == null)
            {
                return HttpNotFound();
            }
            return View(emp);
        }

        // POST: Emps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Emp emp = db.Emps.Find(id);
            db.Emps.Remove(emp);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
