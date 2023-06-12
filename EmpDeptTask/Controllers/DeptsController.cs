using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EmpDeptTask;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;


namespace EmpDeptTask.Controllers
{
    public class DeptsController : Controller
    {
        private EmpDeptEntities db = new EmpDeptEntities();

        //Get: Exprot to Excel through server side

        public ActionResult ExportToExcel()
        {
            var departments = db.Depts.ToList();

            var grid = new GridView();
            grid.DataSource = departments.Select(d => new
            {
                DeptId = d.DeptId,
                DeptName = d.DeptName,
                DeptLocation = d.DeptLocation,
                DeptJoining = d.DeptJoining,
                DeptLeaving = d.DeptLeaving
            });
            grid.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=DepartmentData.xls");
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return RedirectToAction("Index");
        }


        public ActionResult GetServerData(JqueryDatatableParam param)
        {
            List<DeptVM> employees = new List<DeptVM>();
            employees = (from S in db.Depts.AsNoTracking().AsEnumerable()
                         select new DeptVM
                         {
                             DeptId = S.DeptId,
                             DeptName = S.DeptName,
                             DeptLocation = S.DeptLocation,
                             DeptJoining = S.DeptJoining,
                             DeptLeaving = S.DeptLeaving
                         }).ToList();

            // Filtering
            string searchValue = Request["search[value]"];
            if (!string.IsNullOrEmpty(searchValue))
            {
                searchValue = searchValue.ToLower();
                employees = employees.Where(x =>
                    x.DeptName.ToLower().Contains(searchValue) ||
                    x.DeptLocation.ToLower().Contains(searchValue) ||
                    x.DeptJoining.ToLower().Contains(searchValue) ||
                    x.DeptLeaving.ToString().Contains(searchValue)
                ).ToList();
            }

            // Sorting
            var sortColumnIndex = Convert.ToInt32(HttpContext.Request["order[0][column]"]);
            var sortDirection = HttpContext.Request["order[0][dir]"];

            Func<DeptVM, string> orderingFunction = null;

            switch (sortColumnIndex)
            {
                case 0:
                    orderingFunction = e => e.DeptName;
                    break;
                case 1:
                    orderingFunction = e => e.DeptLocation;
                    break;
                case 2:
                    orderingFunction = e => e.DeptJoining;
                    break;
                case 3:
                    orderingFunction = e => e.DeptLeaving;
                    break;
                default:
                    orderingFunction = e => e.DeptName;
                    break;
            }

            if (sortDirection == "desc")
                employees = employees.OrderByDescending(orderingFunction).ToList();
            else
                employees = employees.OrderBy(orderingFunction).ToList();

            // Pagination
            var displayResult = employees.Skip(param.start).Take(param.length).ToList();

            return Json(new
            {
                draw = param.draw,
                recordsTotal = employees.Count,
                recordsFiltered = employees.Count,
                data = displayResult
            }, JsonRequestBehavior.AllowGet);
        }



        // GET: Depts
        public ActionResult Index()
        {
            List<DeptVM> getList = new List<DeptVM>();
            getList = (from S in db.Depts.AsNoTracking().AsEnumerable()
                       select new DeptVM
                       {
                           DeptId = S.DeptId,
                           DeptName = S.DeptName,
                           DeptLocation = S.DeptLocation,
                           DeptJoining = S.DeptJoining,
                           DeptLeaving = S.DeptLeaving
                       }).ToList();

            return View(getList);
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
                    if (empCount.Count >= 0)
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
        [HttpPost]
        public ActionResult Delete(int id)
        {
            Dept dept = db.Depts.Find(id);

            var department = GetDepartmentById(dept, id);
            if (department == null)
            {
                return HttpNotFound();
            }

            if (HasEmployees(department))
            {
                return Content("Cannot delete the department.");
            }

            db.Depts.Remove(dept);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        private Dept GetDepartmentById(Dept dept, int id)
        {
            var departments = GetDepartmentsFromDatabase();
            return departments.FirstOrDefault(d => d.DeptId == id);
        }

        private bool HasEmployees(Dept department)
        {

            return db.Emps.Any(e => e.DeptId == department.DeptId);

        }
        private List<Dept> GetDepartmentsFromDatabase()
        {

            return db.Depts.ToList();
        }

        [HttpGet]
        public ActionResult GetEmployeeCount(int deptId)
        {
            try
            {
                Dept department = db.Depts.Find(deptId);
                if (department != null)
                {
                    int employeeCount = department.Emps.Count;
                    return Json(new { count = employeeCount }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return HttpNotFound();
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = "An error occurred while retrieving the employee count." }, JsonRequestBehavior.AllowGet);
            }
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
