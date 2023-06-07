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
    public class DeptsController : Controller
    {
        private EmpDeptEntities db = new EmpDeptEntities();

        // GET: Depts
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SaveDept(DeptVM data)
        {

            if (ModelState.IsValid)
            {
                var deparment = db.Depts.Where(x => x.DeptId == data.DeptId).FirstOrDefault();
                if (deparment != null)
                {
                    deparment.DeptName = data.DeptName;
                    deparment.DeptLocation = data.DeptLocation;
                    deparment.DeptJoining = data.DeptJoining;
                    deparment.DeptLeaving = data.DeptLeaving;
                    db.Entry(deparment).State = EntityState.Modified;

                    var empCount = db.Emps.Where(x => x.DeptId == data.DeptId).ToList();
                    if (empCount.Count > 0)
                    {
                        db.Emps.RemoveRange(empCount);

                        var saveEmp = data.EmpVM.Select(x => new Emp
                        {
                            EmpId = x.EmpId,
                            EmpName = x.EmpName,
                            EmpCity = x.EmpCity,
                            EmpState = x.EmpState,
                            DeptId = deparment.DeptId,

                        }).ToList();
                        db.Emps.AddRange(saveEmp);
                    }
                    db.SaveChanges();
                }

                //Dept deparment = new Dept();
                //deparment.DeptId = data.DeptId;
                //deparment.DeptName = data.DeptName;
                //deparment.DeptLocation = data.DeptLocation;
                //deparment.DeptJoining = data.DeptJoining;
                //deparment.DeptLeaving = data.DeptLeaving;

                //deparment.Emps = data.EmpVM.Select(x => new Emp
                //{
                //    EmpId = x.EmpId,
                //    EmpName = x.EmpName,
                //    EmpCity = x.EmpCity,
                //    EmpState = x.EmpState,
                //    DeptId = deparment.DeptId,
                //}).ToList();

                //db.Emps.AddRange(deparment.Emps);
                //db.Entry(deparment).State = EntityState.Modified;
                //db.SaveChanges();

                data.StatueCode = "200";
                data.ResponseMessage = "employee updated successfully";

                return Json(new { success = true });
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
            return Json(new { success = false, errors });
        }


        public ActionResult GetData(int? id)
        {

            DeptVM deptData = new DeptVM();
            var getList = db.Depts.Where(d => d.DeptId == id).FirstOrDefault();
            deptData.DeptId = getList.DeptId;
            deptData.DeptName = getList.DeptName;
            deptData.DeptLocation = getList.DeptLocation;
            deptData.DeptJoining = getList.DeptJoining;
            deptData.DeptLeaving = getList.DeptLeaving;

            deptData.EmpVM = (from g in db.Emps.Where(x => x.DeptId == id)
                              select new EmpVM
                              {
                                  EmpId = g.EmpId,
                                  EmpName = g.EmpName,
                                  EmpCity = g.EmpCity,
                                  EmpState = g.EmpState
                              }).ToList();

            return Json(deptData, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult fetchList()
        {
            List<Dept> getList = new List<Dept>();
            getList = db.Depts.ToList();
            //selecting the desired columns
            var subCategoryToReturn = getList.Select(S => new
            {
                DeptId = S.DeptId,
                DeptName = S.DeptName,
                DeptLocation = S.DeptLocation,
                DeptJoining = S.DeptJoining,
                DeptLeaving = S.DeptLeaving
            });
            return Json(subCategoryToReturn, JsonRequestBehavior.AllowGet);
        }

        // GET: Depts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dept dept = db.Depts.Find(id);
            if (dept == null)
            {
                return HttpNotFound();
            }
            return View(dept);
        }

        // GET: Depts/Create
        public ActionResult Create()
        {
            ViewData["ListValue"] = db.Depts.ToList();

            return View();
        }

        // POST: Depts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DeptId,DeptName,DeptLocation,DeptJoining,DeptLeaving")] Dept dept)
        {
            if (ModelState.IsValid)
            {
                db.Depts.Add(dept);
                db.SaveChanges();
                ViewData["ListValue"] = db.Depts.ToList();

                return RedirectToAction("Create");
            }

            return View(dept);
        }

        // GET: Depts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dept dept = db.Depts.Find(id);
            ViewBag.id = id;
            return View(dept);
        }

        // POST: Depts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DeptId,DeptName,DeptLocation,DeptJoining,DeptLeaving")] Dept dept)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dept).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dept);
        }

        // GET: Depts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dept dept = db.Depts.Find(id);
            if (dept == null)
            {
                return HttpNotFound();
            }
            return View(dept);
        }

        // POST: Depts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Dept dept = db.Depts.Find(id);
            db.Depts.Remove(dept);
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
