using System;
using System.Linq;
using System.Web.Mvc;
using Boeing_Tool_Design.Models;
using System.Web.Helpers;
using System.Web.Security;
using System.Data.Entity;

namespace Boeing_Tool_Design.Controllers
{
    public class UserController : Controller
    {
        private BoeingContext db = new BoeingContext();
        // Registration GET
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        //Registration POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            AppUser appuser = new AppUser();
            //Model Validation
            if (ModelState.IsValid)
            {
                //Will be true if (1) the email address already exists in the DB and (2) the user hasn't been deleted
                bool userWasDeleted = false;

                #region//Does the email already exist
                appuser.UserEmail = model.UserEmail;
                bool doesExist = DoesEmailExist(appuser.UserEmail);
                if (doesExist)
                {
                    AppUser existingUser = GetUser(appuser.UserEmail);

                    //User exists and is active
                    if (!existingUser.IsDeleted)
                    {
                        ModelState.AddModelError("EmailExist", "User already exists.");
                        return View();
                    }
                    //User exists and is not active
                    else
                    {
                        userWasDeleted = true;
                        //Make the appuser into the existing active user and override info 
                        appuser = existingUser;
                    }
                }
                #endregion

                appuser.FirstName = model.FirstName;
                appuser.LastName = model.LastName;

                #region//Password Salting and Hashing
                appuser.PasswordSalt = Crypto.GenerateSalt();
                var password = model.PasswordHash + appuser.PasswordSalt;
                appuser.PasswordHash = Crypto.SHA256(password);
                #endregion

                //default access level is 2, or a standard user
                #region//AccessLevelID
                appuser.AccessLevelID = 2;
                #endregion
                #region//Save data to databese

                using (BoeingContext db = new BoeingContext())
                {
                    //Do not add an existing user into the DB
                    if (!userWasDeleted)
                    {
                        appuser.CreatedDate = DateTime.Today;
                        db.AppUsers.Add(appuser);
                    }
                    //The user already exists, but now is modified
                    else
                    {
                        //Set user to existant (because they're being registered)
                        appuser.IsDeleted = false;
                        db.Entry(appuser).State = EntityState.Modified;
                    }

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                    {
                        Exception raise = dbEx;
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                string message = string.Format("{0}:{1}",
                                    validationErrors.Entry.Entity.ToString(),
                                    validationError.ErrorMessage);
                                // raise a new exception nesting
                                // the current instance as InnerException
                                raise = new InvalidOperationException(message, raise);
                            }
                        }
                        throw raise;
                    }
                }
                #endregion

                //Creates an authentication ticket for the supplied user name and adds it to the cookies collection of the response, or to the URL if you are using cookieless authentication.
                FormsAuthentication.SetAuthCookie(appuser.UserEmail, true); //true to keep logged in
            }
            else
            {
                var errors = ModelState.SelectMany(x => x.Value.Errors.Select(z => z.Exception));
            }
            return RedirectToAction("Index", "Home");
        }


        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        //POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            AppUser dbUser = db.AppUsers.SingleOrDefault(AppUser => AppUser.UserEmail == model.Email); //retrieve the user from the DB (null if user does not exist)

            if (dbUser != null)
            {
                if (!dbUser.IsDeleted)
                {
                    string passwordSalt = model.Password + dbUser.PasswordSalt;
                    string passwordHash = Crypto.SHA256(passwordSalt);

                    if (string.Compare(dbUser.PasswordHash, passwordHash) == 0) //compare DB password with form password
                    {
                        FormsAuthentication.SetAuthCookie(dbUser.UserEmail, true); //Authroize user to access pages on the website, true to keep users logged in
                        return RedirectToAction("Index", "Home"); //redirects to the homepage
                    }
                    else
                    {
                        ModelState.AddModelError("Credentials", "Username and password do not match.");
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("UserError", "This user has been deleted. Please re-register.");
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("UserError", "This user does not exist.");
                return View(model);
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
        }

        [NonAction]
        public bool DoesEmailExist(string email)
        {
            using (BoeingContext db = new BoeingContext())
            {
                AppUser user = db.AppUsers.Where(a => a.UserEmail == email).FirstOrDefault();
                return user == null ? false : true;
            }
        }

        public AppUser GetUser(string email)
        {
            using (BoeingContext db = new BoeingContext())
            {
                AppUser user = db.AppUsers.Where(a => a.UserEmail == email).FirstOrDefault();
                return user;
            }
        }

        [Authorize]
        public ActionResult Settings()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Settings(FormCollection form)
        {
            var firstname = form["settingsfirstname"];
            var lastname = form["settingslastname"];
            var email = form["settingsemail"];
            var oldpw = form["oldpw"];
            var newpw = form["newpw"];

            if (firstname == "" || lastname == "" || email == "")
            {
                ModelState.AddModelError("Error", "Please do not leave any items on the form blank.");
                return View();
            }

            AppUser currentuser = db.AppUsers.Where(d => d.UserEmail == User.Identity.Name).SingleOrDefault();

            //check to see if email is already taken
            AppUser existingUser = db.AppUsers.SingleOrDefault(AppUser => AppUser.UserEmail == email); //get current user from the database
            if (existingUser != null)
            {
                if (existingUser.AppUserID != currentuser.AppUserID)
                {
                    ModelState.AddModelError("Error", "That email address is already taken.");
                    return View();
                }
            }

            if (!PasswordSettingsCheck(currentuser, oldpw, newpw)) return View();

            bool isLogout = false;

            currentuser.FirstName = firstname;
            currentuser.LastName = lastname;

            //user must be logged off if email changes to avoid authentication errors
            if (currentuser.UserEmail != email)
            {
                currentuser.UserEmail = email;
                FormsAuthentication.SignOut();
                isLogout = true;
            }

            currentuser.UpdatedDate = DateTime.Today;
           
            try
            {
                db.Entry(currentuser).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {
                ModelState.AddModelError("Error", "There was an error saving your information. Please try again later.");
                return View();
            }

            TempData["success"] = "Success!";

            if (isLogout) return RedirectToAction("Login", "User");
            return View();
        }

        private bool PasswordSettingsCheck(AppUser currentuser, string oldpw, string newpw)
        {
            const int DEFAULT_PASSWORD_LENGTH = 6;

            if (oldpw != "")
            {
                if (newpw != "")
                {
                    if (newpw.Length < DEFAULT_PASSWORD_LENGTH)
                    {
                        ModelState.AddModelError("Error", "Your password must be at least 6 characters in length.");
                        return false;
                    }

                    string passwordSalt = oldpw + currentuser.PasswordSalt;
                    string passwordHash = Crypto.SHA256(passwordSalt);

                    if (string.Compare(currentuser.PasswordHash, passwordHash) == 0) //compare DB password with form password
                    {
                        // If they match, change the pw
                        currentuser.PasswordSalt = Crypto.GenerateSalt();
                        var password = newpw + currentuser.PasswordSalt;
                        currentuser.PasswordHash = Crypto.SHA256(password);
                        db.Entry(currentuser).State = EntityState.Modified;
                        try
                        {
                            db.SaveChanges();
                        }
                        catch
                        {
                            ModelState.AddModelError("Error", "There was an error saving your password.");
                            return false;
                        }

                    }
                    else
                    {
                        ModelState.AddModelError("Error", "That password is incorrect.");
                        return false;
                    }
                }
                else
                {
                    ModelState.AddModelError("Error", "New password field cannot be empty.");
                    return false;
                }
            }
            else if (newpw != "")
            {
                ModelState.AddModelError("Error", "Old password field cannot be empty.");
                return false;
            }

            return true;
        }

        public string getFirstName()
        {
            string firstname;
            if (Request.IsAuthenticated)
            {
                firstname = "";

                AppUser appuser = db.AppUsers.Where(d => d.UserEmail == User.Identity.Name).SingleOrDefault();
                if (appuser != null) {
                    firstname = appuser.FirstName;
                }
            }
            else
            {
                firstname = "";
            }

            return firstname;
        }

        public string getLastName()
        {
            string lastname;
            if (Request.IsAuthenticated)
            {
                lastname = "";

                AppUser appuser = db.AppUsers.Where(d => d.UserEmail == User.Identity.Name).SingleOrDefault();
                if (appuser != null)
                {
                    lastname = appuser.LastName;
                }
            }
            else
            {
                lastname = "";
            }

            return lastname;
        }
    }
}
