using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using mvc_project.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Data.Entity;
using mvc_project.ViewModels;
using System.Net;

namespace mvc_project.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        ApplicationDbContext db = new ApplicationDbContext();

        DateTime str;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
            str = DateTime.Now.Date;
        }

        [AllowAnonymous]
        public async Task<ActionResult> CreateRoles()
        {
            RoleManager<IdentityRole> rm = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            IdentityRole r1 = new IdentityRole("Admin");
            IdentityRole r2 = new IdentityRole("Student");
            IdentityRole r3 = new IdentityRole("Security");
            await rm.CreateAsync(r1);
            await rm.CreateAsync(r2);
            await rm.CreateAsync(r3);
            return Content("Roles Created");
        }

        #region Admin3
        public ActionResult StudentsPermissions()
        {
            List<Department> departments = db.Department.ToList<Department>();

            ViewBag.departments = new SelectList(departments, "DeptId", "DeptName");
            return View(departments);
        }

        public ActionResult LoadStudents(int id) // id is deptId
        {
            //int deptId = int.Parse(Request.QueryString["departments"]);
            //string date = Request.QueryString["date"];

            //var perms = db.Permission.Where(p => p.Status == "pending" && p.PermDate == date).Select(s => s.StdId);
            //var perms = db.Permission.Where(p => p.Status == "pending").Select(s => s.StdId);
            var perms = db.Permission.Where(p => p.Status == "pending");
            ApplicationUser std = new ApplicationUser();
            List<AdminViewModels> data = new List<AdminViewModels>();
            foreach (var item in perms)
            {
                if(db.Users.FirstOrDefault(s => s.Id == item.StdId && s.DeptId == id) != null)
                {
                    std = db.Users.FirstOrDefault(s => s.Id == item.StdId && s.DeptId == id);
                    data.Add(new AdminViewModels() { UserID = std.UserId, UserName = std.UserName,
                        permissionDate = item.PermDate.ToString(), permissionNote = item.Note, permissionID = item.PermId });
                }
            }
            //List<ApplicationUser> stds = new List<ApplicationUser>();
            //stds = db.Users.Where(d => d.DeptId == deptId).ToList<ApplicationUser>();
            //ViewBag.date = date;

            //ViewBag.date = permsDates;
            //return PartialView(stds);
            return PartialView(data);
        }

        public ActionResult Allpermissions()
        {
            var perms = db.Permission.ToList();
            ApplicationUser std = new ApplicationUser();
            List<AdminViewModels> data = new List<AdminViewModels>();
            foreach (var item in perms)
            {
                if (db.Users.FirstOrDefault(s => s.Id == item.StdId) != null)
                {
                    std = db.Users.FirstOrDefault(s => s.Id == item.StdId);
                    data.Add(new AdminViewModels()
                    {
                        UserID = std.UserId,
                        UserName = std.UserName,
                        DeptName = std.Dept.DeptName,
                        permissionDate = item.PermDate.ToString(),
                        permissionNote = item.Note,
                        permissionID = item.PermId,
                        permissionStatus = item.Status
                    });
                }
            }
            return View(data);
        }

        public ActionResult PermissionResponse(int id, string permStatus)
        {
            id = int.Parse(Request.QueryString[0]);
            permStatus = Request.QueryString[1];
            var perms = db.Permission.Where(p => p.PermId == id);
            foreach (var item in perms)
            {
                if (permStatus == "a")
                    item.Status = "accepted";
                else
                    item.Status = "refused";
            }
            db.SaveChanges();
            //return View();
            return null;
        }
        #endregion

        #region Student
        //profile
        int? deptid;
        public ActionResult profile()
        {
            List<ApplicationUser> users = db.Users.ToList<ApplicationUser>();
            string name = User.Identity.Name;
            //Department dept=
            foreach (var item in users)
            {
                if (item.UserName == name)
                {
                    ViewBag.u_id = item.UserId;
                    ViewBag.u_name = item.UserName;
                    ViewBag.u_email = item.Email;
                    deptid = item.DeptId;


                }


            }
            List<Department> dept = db.Department.Where(a => a.DeptId == deptid).ToList<Department>();
            ViewBag.u_dept = dept[0].DeptName;
            return View();
            //return Content(name);

        }
        //to read it in multiple scope
        string id;

        public ActionResult View_attend()
        {
            List<ApplicationUser> users = db.Users.ToList<ApplicationUser>();
            string name = User.Identity.Name;
            //string id;
            foreach (var item in users)
            {
                if (item.UserName == name)
                {
                    id = item.Id;

                }
            }
            List<Attendance> attdens = db.Attendance.Where(a => a.StdId == id).ToList<Attendance>();
            ViewBag.attends = attdens;
            return View();
        }
        public ActionResult Perm()
        {
            return View();
        }
        public ActionResult Permupdate(string datee, string res)
        {
            List<ApplicationUser> users = db.Users.ToList<ApplicationUser>();
            string name = User.Identity.Name;
            foreach (var item in users)
            {
                if (item.UserName == name)
                {
                    id = item.Id;

                }
            }
            db.Permission.Add(new Permission() { StdId = id, PermDate = DateTime.Parse(datee), Note = res, Status = "pending" });
            db.SaveChanges();
            //return Content("record inserted");
            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region Adminview2

        [Authorize(Roles = "Admin")]
        // [Route("DSSD")]
        public ActionResult DisplayStudentsStatusInDay()
        {
            List<Department> department = db.Department.ToList<Department>();
            ViewBag.department = new SelectList(department, "DeptId", "DeptName");
            return View();
        }
        //[HttpPost]

        [Authorize(Roles = "Admin")]
        public ActionResult DisplayDep(string department, DateTime Day)
        {
            // return Content(department + Day);
            int DeptID = int.Parse(department);
            List<Attendance> stds = db.Attendance.Include(s => s.User).Where(a => a.AttDate == Day && a.User.DeptId == DeptID).ToList<Attendance>();
            //List<ApplicationUser> users = new List<ApplicationUser>();
            //foreach (var item in stds)
            //{         
            //    users.Add(item.User);
            //}
            //List<ApplicationUser> usersDept = new List<ApplicationUser>();

            //foreach (var item in users)
            //{
            //    if (item.DeptId == DeptID)
            //    {
            //        usersDept.Add(item);
            //    }
            //}

            return View(stds);
        }
        #endregion

        #region Admin1
        [Authorize(Roles = "Admin")]
        public ActionResult ShowFromTo()
        {

            var departments = db.Department.ToList();

            List<Attendance> attends = db.Attendance.Include(a => a.User).ToList();

            //List<AttendInfo> attends =
            //            (from ep in db.Attendance
            //             join e in db.Users on ep.StdId equals e.Id
            //             join t in db.Permission on e.Id equals t.StdId
            //             select (new AttendInfo
            //             {
            //                 Attendances = ep,
            //                 Permissions = t,
            //                 Students = e
            //             })).ToList();


            var AttResult = from student in attends group student by student.User.UserName;

            var permission = db.Permission.Include(p => p.User).ToList();

            var PermResult = from student in permission group student by student.User.UserName;

            //// Common Keys between left and right
            //var r = result.Where(s => result.Select(p => p.Key ).Contains(s.Key)).ToList();

            var viewModel = new StudentAttendanceViewModel
            {
                Departments = departments
                ,
                Attendances = AttResult
                ,
                Permission = PermResult

            };

            return View(viewModel);

        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ShowFromTo(StudentAttendanceViewModel stdAttend)
        {

            //string validformats =  "MM/dd/yyyy";

            //CultureInfo provider = new CultureInfo("en-US");

            var departments = db.Department.ToList();
            //need modification to datetime 

            List<Attendance> attends = db.Attendance.Include(a => a.User)
                .Where(a => a.AttDate >= stdAttend.From && a.AttDate <= stdAttend.To && a.User.DeptId == stdAttend.Student.DeptId).ToList();

            var AttResult = from student in attends group student by student.User.UserName;

            var permission = db.Permission.Include(p => p.User)
   .Where(p => p.PermDate >= stdAttend.From && p.PermDate <= stdAttend.To)
   .ToList();

            var PermResult = from student in permission group student by student.User.UserName;

            stdAttend.Attendances = AttResult;
            stdAttend.Departments = departments;
            stdAttend.Permission = PermResult;

            return View(stdAttend);
        }
        #endregion

        #region security
        public async Task<ActionResult> CreateRows()
        {
         DateTime str = DateTime.Now.Date;
            List<ApplicationUser> users = UserManager.Users.ToList<ApplicationUser>();
            List<ApplicationUser> stds = new List<ApplicationUser>();
            foreach (var item in users)
            {
                if (await UserManager.IsInRoleAsync(item.Id, "Student") == true)
                { stds.Add(item); }
            }

            List<Attendance> at = db.Attendance.Where(a => a.AttDate == str).ToList();
            if (at.Count == 0)
            {


                for (int i = 0; i < stds.Count; i++)
                {
                    Attendance att = new Attendance();
                    att.StdId = stds[i].Id;
                    att.AttDate = DateTime.Now.Date;
                    db.Attendance.Add(att);
                    db.SaveChanges();

                }
            }

            SelectList dpts = new SelectList(db.Department.ToList<Department>(), "DeptId", "DeptName");
            SelectList status = new SelectList(
        new List<SelectListItem>
        {
            new SelectListItem { Text = "Attendance", Value = "1"},
            new SelectListItem { Text = "Exit", Value = "2"},

        }, "Value", "Text");

            ViewBag.dpts = dpts;
            ViewBag.status = status;

            List<Attendance> stdinDep = db.Attendance.Include(s => s.User)
                    .Where(a => a.AttDate == str & a.User.DeptId == 1).ToList();
            ViewBag.stdinDep = stdinDep;
            return View("security");
        }
        public ActionResult Navigation(int? stdRcrdedId, int? id = 1, int state = 1)
        {
          DateTime str = DateTime.Now.Date;
            if (state == 1)
            {
                List<Attendance> stdinDep = db.Attendance.Include(s => s.User)
                    .Where(a => a.AttDate == str & a.User.DeptId == id).ToList();
                ViewBag.stdinDep = stdinDep;
                // int? i = stdRcrdedId; // Std st = db.students.Find(i);
                ViewBag.state = state;
                return PartialView("showStd", stdinDep); //}

            }
            else
            {
                List<Attendance> stdinDep = db.Attendance.Include(s => s.User)
               .Where(a => a.TimeAttend != null & a.AttDate == str & a.User.DeptId == id).ToList();
                ViewBag.state = state;
                ViewBag.stdinDep = stdinDep;
                ViewBag.state = state;
                return PartialView("showStd", stdinDep);// }
            }
        }
        public ActionResult Recording(int slcSt, int? id = 1, int state = 1)
        {
            SelectList dpts = new SelectList(db.Department.ToList<Department>(), "DeptId", "DeptName", id);
            SelectList status = new SelectList(
        new List<SelectListItem>
        {
    new SelectListItem { Text = "Attendance", Value = "1"}, new SelectListItem { Text = "Exit", Value = "2"},
       }, "Value", "Text", slcSt);

            ViewBag.dpts = dpts; ViewBag.status = status;
          DateTime str = DateTime.Now.Date;
            if (state == 1)
            {
                var stdinDep = Session["list"] as List<Attendance>;
                ViewBag.stdinDep = stdinDep;
                // int? i = stdRcrdedId; // Std st = db.students.Find(i);
                ViewBag.state = state;
                return View("Recording", stdinDep); //}

            }
            else
            {
                var stdAttended = Session["EXlist"] as List<Attendance>;
                ViewBag.state = state;

                return View("Recording", stdAttended);// }
            }
        }
        public ActionResult RecordAttnd(int id)
        {
            DateTime str = DateTime.Now.Date;

            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            Attendance attend = db.Attendance.Find(id);
            if (attend == null) { return HttpNotFound(); }
            attend.TimeAttend = DateTime.Now.ToShortTimeString();
            string i = attend.StdId;
            db.SaveChanges();
            ApplicationUser st = db.Users.Find(i);//Find : search by primarykey

            int? dp = st.DeptId;
            List<Attendance> stinDep = db.Attendance.Include(s => s.User)
                    .Where(a => a.AttDate == str & a.User.DeptId == dp).ToList();
            Console.WriteLine(str);
            stinDep.Remove(attend);
            Session["list"] = stinDep;
            return RedirectToAction("Recording", "Account", new { id = dp, state = 1, slcSt = 1 });
        }
        public ActionResult RecordExit(int id)
        {
            DateTime str = DateTime.Now.Date;

            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            Attendance attend = db.Attendance.Find(id);
            if (attend == null) { return HttpNotFound(); }
            attend.TimeLeft = DateTime.Now.ToShortTimeString();
            String i = attend.StdId;
            db.SaveChanges();
            ApplicationUser st = db.Users.Find(i);//Find : search by primarykey

            int? dp = st.DeptId;
            List<Attendance> stdAtt = db.Attendance.Include(s => s.User)
              .Where(a => a.TimeAttend != null & a.AttDate == str & a.User.DeptId == dp).ToList();
            Session["EXlist"] = stdAtt;
            return RedirectToAction("Recording", "Account", new { id = dp, state = 2, slcSt = 2 });
        }

        public ActionResult Check()
        {
            if (User.IsInRole("Security"))

            {
                return RedirectToAction("createRows");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Check");
            }

        }
        
        #endregion

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, change to shouldLockout: true
                var user = UserManager.FindByEmail(model.Email);
                var result = await SignInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, shouldLockout: false);
                switch (result)
                {
                    case SignInStatus.Success:
                        return RedirectToLocal(returnUrl);
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    case SignInStatus.RequiresVerification:
                        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "Invalid login attempt.");
                        return View(model);
                }
            }
            catch (Exception ex)
            {
                //return Content("Wrong account");
                return View("Error");
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            RegisterViewModel mod = new RegisterViewModel();
            mod.Depts = db.Department.ToList();
            return View(mod);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email, UserId = model.UserID, DeptId = model.DeptId };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //var dept = db.Department.Where(d => d.DeptId == model.DeptId).ToList();
                    //foreach (var item in dept)
                    //{
                    //    if(item.Users == null)
                    //    {
                    //        item.Users = new List<ApplicationUser>();
                    //        item.Users.Add(user);
                    //    }
                    //    else
                    //    {
                    //        item.Users.Add(user);
                    //    }
                    //}

                    //await UserManager.AddToRoleAsync(user.Id, "Admin"); // to register the admin in the beginning
                    if (User.IsInRole("Admin"))
                    {
                        await UserManager.AddToRoleAsync(user.Id, "Security"); // for now register only one time
                    }
                    else
                    {
                        await UserManager.AddToRoleAsync(user.Id, "Student");
                    }
                    await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);
                    
                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            //return RedirectToAction("Index", "Home");
            return RedirectToAction("HomePage", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        //private ActionResult RedirectToLocal(string returnUrl)
        //{
        //    if (Url.IsLocalUrl(returnUrl))
        //    {
        //        return Redirect(returnUrl);
        //    }
        //    return RedirectToAction("Index", "Home");
        //}

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}