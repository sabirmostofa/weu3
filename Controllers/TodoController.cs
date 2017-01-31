using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace WebApplication1.Controllers
{
    public class TodoController : Controller
    {
        //private TodoDBContext db = new TodoDBContext();
        private ApplicationDbContext db;
        private UserManager<ApplicationUser> manager;

        public TodoController() {
            db = new ApplicationDbContext();
            manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }
        

        // GET: Todo
        [Authorize]
        public ActionResult Index()
        {
            var currentUser = manager.FindById(User.Identity.GetUserId());
            return View(db.Todos.ToList().Where(todo => todo.User.Id == currentUser.Id));

            //return View(db.Todos.ToList());
        }

        [Authorize (Roles="Admin")]
        public ActionResult IndexAll()
        {
            return View(db.Todos.ToList());
        }

        // GET: Todo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Todo todo = db.Todos.Find(id);
            if (todo == null)
            {
                return HttpNotFound();
            }
            return View(todo);
        }

        // GET: Todo/Create
        public ActionResult Add()
        {
            return View();
        }

        // POST: Todo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Include = "ID,Title,Description,User")] Todo todo)
        {
            var currentUser = manager.FindById(User.Identity.GetUserId());
            if (ModelState.IsValid)
            {
                todo.User = currentUser;
                db.Todos.Add(todo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(todo);
        }

        // GET: Todo/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            var currentUser = manager.FindById(User.Identity.GetUserId());
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Todo todo = db.Todos.Find(id);
            if (todo == null)
            {
                return HttpNotFound();
            }
            var isAdmin = false;
            if (manager.GetRoles(currentUser.Id).Where(o => string.Equals("Admin", o, StringComparison.OrdinalIgnoreCase)).Any()) {
                 isAdmin = true;
            }
            if (todo.User.Id != currentUser.Id )
            {
                if(!isAdmin)
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return View(todo);
        }

        // POST: Todo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,Description")] Todo todo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(todo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(todo);
        }

        // GET: Todo/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Todo todo = db.Todos.Find(id);
            if (todo == null)
            {
                return HttpNotFound();
            }
            return View(todo);
        }

        // POST: Todo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Todo todo = db.Todos.Find(id);
            db.Todos.Remove(todo);
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
