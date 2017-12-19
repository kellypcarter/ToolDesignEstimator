using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Boeing_Tool_Design.Models;
using System.Globalization;
using System.Data.Entity;
using System.Data;
using System.Web.Helpers;
using System.Web.Security;

namespace Boeing_Tool_Design.Controllers
{
    [Authorize]
    public class AdminController : Controller
	{
		private BoeingContext db = new BoeingContext();
		// GET: Admin
		public ActionResult Statistics()
		{
            Boolean isAdmin = CheckIfAdmin();
            if (!isAdmin) return RedirectToAction("Index", "Home");

            List<ToolType> ToolTypes = db.ToolTypes.OrderBy(x => x.ToolCode).ToList();
			List<ComplexityLevel> ComplexityLevels = db.ComplexityLevels.OrderBy(x => x.ComplexityLevelNum).ToList();
			List<Statistic> Statistics = db.Statistics.Where(d => d.isCurrent == true).ToList();
            List<AppUser> users = db.AppUsers.ToList();
            AdjustmentFactor AdjustFactor = db.AdjustmentFactors.FirstOrDefault();

			AdminViewModel viewModel = new AdminViewModel();
			viewModel.ComplexityLevels = ComplexityLevels;
			viewModel.ToolTypes = ToolTypes;
			viewModel.Statistics = Statistics;
            viewModel.AdjustFactor = AdjustFactor;
            viewModel.AppUsers = users;


			//var tools = db.ToolTypes.OrderBy(x => x.ToolCode).ToArray();
			//var complexities = db.ComplexityLevels.OrderBy(x => x.ComplexityLevelNum).ToArray();
			//var statistics = db.Statistics.Where( d => d.isCurrent == true).ToArray();

			//return View(Tuple.Create(tools, complexities, statistics));
			return View(viewModel);
		}

		[HttpPost]
        public string getToolName(FormCollection form)
		{
			var toolCode = form["tool"];
			ToolType tool = db.ToolTypes.Where(d => d.ToolCode == toolCode).SingleOrDefault();

            string toolName = tool.ToolTypeName;

			return toolName;
		}

        [HttpPost]
        public string getToolDesc(FormCollection form)
        {
            var toolCode = form["tool"];
            ToolType tool = db.ToolTypes.Where(d => d.ToolCode == toolCode).SingleOrDefault();

            string toolDesc = tool.ToolTypeDescription;

            return toolDesc;
        }

        [HttpPost]
        public string getStFlow(FormCollection form)
        {
            var toolCode = form["tool"];
            ToolType tool = db.ToolTypes.Where(d => d.ToolCode == toolCode).SingleOrDefault();

            string stFlow = tool.StandardFlow.ToString();

            return stFlow;
        }

        [HttpPost]
        public string getStressDefault(FormCollection form)
        {
            string stressDefault = "false";
            var toolCode = form["tool"];
            ToolType tool = db.ToolTypes.Where(d => d.ToolCode == toolCode).SingleOrDefault();
		    
            bool trueorfalse = tool.IsStressRequiredDefault;

            if (trueorfalse == true)
            {
                stressDefault = "Yes";
            }
            else
            {
                stressDefault = "No";
            }

            return stressDefault;
        }


		[HttpPost]
		public string getStDev(FormCollection form)
		{
			string stDev = "";
			var toolCode = form["tools"];
			var complexityLevel = Convert.ToInt32(form["complexity"]);
			ToolType tool = db.ToolTypes.Where(d => d.ToolCode == toolCode).SingleOrDefault();
			foreach (var item in db.Statistics.Where(d => d.ComplexityLevel == complexityLevel && d.ToolTypeID == tool.ToolTypeID && d.isCurrent == true))
			{
				
				stDev = item.StandardDeviation.ToString();
			}

			return stDev;
		}

		[HttpPost]
		public string getAverageHours(FormCollection form)
		{
			string baselineHrs = "";
			var toolCode = form["tools"];
			var complexityLevel = Convert.ToInt32(form["complexity"]);
			ToolType tool = db.ToolTypes.Where(d => d.ToolCode == toolCode).SingleOrDefault();
			foreach (var item in db.Statistics.Where(d => d.ComplexityLevel == complexityLevel && d.ToolTypeID == tool.ToolTypeID && d.isCurrent == true))
			{

				baselineHrs = item.AverageHours.ToString();
			}

			return baselineHrs;
		}

        [HttpPost]
        public string getReleaseHours(FormCollection form)
        {
            string releaseHrs = "";
            var toolCode = form["tools"];
            var complexityLevel = Convert.ToInt32(form["complexity"]);
            ToolType tool = db.ToolTypes.Where(d => d.ToolCode == toolCode).SingleOrDefault();
            foreach (var item in db.Statistics.Where(d => d.ComplexityLevel == complexityLevel && d.ToolTypeID == tool.ToolTypeID && d.isCurrent == true))
            {

                releaseHrs = item.ReleaseHours.ToString();
            }

            return releaseHrs;
        }

		[HttpPost]
		public string getImagePath(FormCollection form)
		{
			string imagePath = "";
			var toolCode = form["tools"];
			var complexityLevel = Convert.ToInt32(form["complexity"]);
			ToolType tool = db.ToolTypes.Where(d => d.ToolCode == toolCode).SingleOrDefault();
			foreach (var item in db.Statistics.Where(d => d.ComplexityLevel == complexityLevel && d.ToolTypeID == tool.ToolTypeID && d.isCurrent == true))
			{

				imagePath = item.ImageFilePath.ToString();
			}

			return imagePath;
		}

		[HttpPost]
		public string getNotes(FormCollection form)
		{
			string notes = "";
			var toolCode = form["tools"];
			var complexityLevel = Convert.ToInt32(form["complexity"]);
			ToolType tool = db.ToolTypes.Where(d => d.ToolCode == toolCode).SingleOrDefault();
			foreach (var item in db.Statistics.Where(d => d.ComplexityLevel == complexityLevel && d.ToolTypeID == tool.ToolTypeID && d.isCurrent == true))
			{
				if (item.StatisticNotes != null) {
					notes = item.StatisticNotes.ToString();
				}
				else {
					notes = "";
				}
			}

			return notes;
		}

        [HttpPost]
		public void editTool(FormCollection form) {
            var toolCode = form["tool"];
            var toolName = form["toolName"];
            var stFlow = form["stFlow"];
            var toolDesc = form["toolDesc"];
            var stressReq = form["stressReq"];

            ToolType tool = db.ToolTypes.Where(d => d.ToolCode == toolCode).SingleOrDefault();

            tool.ToolCode = toolCode;
            tool.ToolTypeName = toolName;
            tool.ToolTypeDescription = toolDesc;
            tool.StandardFlow = float.Parse(stFlow);
            if (stressReq == "No") {
                tool.IsStressRequiredDefault = false;
            }
            else {
                tool.IsStressRequiredDefault = true;
            }
            tool.UpdatedDate = DateTime.Now;

            db.Entry(tool).State = EntityState.Modified;
            db.SaveChanges();

        }
        [HttpPost]
        public void saveStatistics(FormCollection form, HttpPostedFileBase file)
        {
            var toolCode = form["tools"];
            var complexity = Convert.ToInt32(form["complexity"]);
            var stdeviation = form["stdeviation"];
            var hours = form["hours"];
            var notes = form["notes"];
            var releasehours = form["releasehours"];
            var filepath = "";

            ToolType tool = db.ToolTypes.Where(d => d.ToolCode == toolCode).SingleOrDefault();
            // SET END DATE TO REPLACING STATISTIC, AND MARK NOT CURRENT
            Statistic replacethis = db.Statistics.Where(d => d.ComplexityLevel == complexity && d.ToolTypeID == tool.ToolTypeID && d.isCurrent == true).FirstOrDefault();
            replacethis.isCurrent = false;

            var format = "yyyy-MM-dd HH:mm:ss:fff";
            var stringDate = DateTime.Now.ToString(format);
            var convertedBack = DateTime.ParseExact(stringDate, format, CultureInfo.InvariantCulture);
            replacethis.EffectiveEndDate = convertedBack;
            db.Entry(replacethis).State = EntityState.Modified;
            db.SaveChanges();



            if (file != null)
            {
                filepath = file.FileName;
                if (file.ContentLength > 0)
                {
                    if (file.ContentType.Contains("image"))
                    {
                        var fullPath = "";

                        // It is an image
                        if (file.FileName.Contains("\\"))
                        {
                            var lastIndex = file.FileName.LastIndexOf("\\");
                            filepath = file.FileName.Substring(lastIndex).Replace("\\", "");
                            fullPath = "~/content/toolpictures/" + filepath;
                        }
                        else
                        {
                            fullPath = "~/content/toolpictures/" + filepath;
                        }

                        file.SaveAs(Server.MapPath(fullPath));

                    }
                }
            }

            // CREATE NEW STATISTIC AND MARK ACTIVE

            Statistic newStat = new Statistic();
            const string DEFAULT_VALUE = "0";

            newStat.ComplexityLevel = replacethis.ComplexityLevel;
            newStat.ReleaseHours = replacethis.ReleaseHours;
            newStat.ToolTypeID = replacethis.ToolTypeID;
            if (file != null)
            {
                newStat.ImageFilePath = "../../content/toolpictures/" + filepath;
            }
            else
            {
                newStat.ImageFilePath = replacethis.ImageFilePath;
            }

            AppUser currentuser = db.AppUsers.FirstOrDefault(m => m.UserEmail == User.Identity.Name);
            newStat.CreatedByUserID = currentuser.AppUserID;

            if (hours == "") hours = DEFAULT_VALUE;
            if (stdeviation == "") stdeviation = DEFAULT_VALUE;

            newStat.AverageHours = float.Parse(hours);
            newStat.ReleaseHours = float.Parse(releasehours);
            newStat.CreatedDate = convertedBack;
            newStat.EffectiveStartDate = convertedBack;
            newStat.isCurrent = true;
            newStat.StandardDeviation = float.Parse(stdeviation);
            newStat.StatisticNotes = notes;

            db.Statistics.Add(newStat);
            db.SaveChanges();
        }


		[HttpPost]
		public ActionResult addTool(FormCollection form)
		{
			var toolCode = form["toolCode"];
			var toolName = form["toolName"];
			var toolDesc = form["toolDesc"];
			var stFlow = form["stFlow"];
			var stressReq = form["stressReq"];

            if (string.IsNullOrEmpty(stFlow) && stFlow.All(Char.IsDigit))
            {
                // AKA IT IS EITHER NULL OR NOT A POSITIVE INTEGER
                ModelState.AddModelError("Error", "Standard Flow must be a positive number.");
                return View();
            }

            // CREATE NEW TOOL
            ToolType newtool = new ToolType();
			AppUser currentuser = db.AppUsers.FirstOrDefault(m => m.UserEmail == User.Identity.Name);
			newtool.CreatedByUserID = currentuser.AppUserID;
			newtool.ToolTypeDescription = toolDesc;
			newtool.ToolCode = toolCode;
			if (stressReq == "Yes") {
				newtool.IsStressRequiredDefault = true;
			}
			else {
				newtool.IsStressRequiredDefault = false;
			}

            newtool.CreatedDate = DateTime.Now;

			newtool.ToolTypeName = toolName;
			newtool.StandardFlow = float.Parse(stFlow);
			db.ToolTypes.Add(newtool);
			db.SaveChanges();

            //Add blank Statistics for each complexity for now
            for (int i = 1; i < 6; i++) {
                Statistic stat = new Statistic();
                stat.AverageHours = 0;
                stat.ComplexityLevel = i;
                stat.StandardDeviation = 0;
                stat.isCurrent = true;
                stat.ReleaseHours = 0;
                stat.CreatedByUserID = currentuser.AppUserID;
                stat.CreatedDate = DateTime.Now;
                stat.EffectiveStartDate = DateTime.Now;
                stat.ToolTypeID = newtool.ToolTypeID;
                stat.ImageFilePath = "../../content/toolpictures/default.png";
                db.Statistics.Add(stat);
                db.SaveChanges();
            }

			return RedirectToAction("Statistics");
		}

        public void SaveAdjustments()
        {

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveAdjustments([Bind(Include = "NSAR,NSRR,StressAnalysis,SurfacingRequired,EngineeringReleased,FirstToolInFamily,SecondToolInFamily,KBEDevelopment,KBEFollowOnTooling")] AdjustmentFactor AdjustFactor)
        {
            AdjustmentFactor adj = db.AdjustmentFactors.FirstOrDefault();
            // TODO: MIGHT WANT TO ADD EDITEDBYUSERID
            adj.EngineeringReleased = AdjustFactor.EngineeringReleased;
            adj.FirstToolInFamily = AdjustFactor.FirstToolInFamily;
            adj.KBEDevelopment = AdjustFactor.KBEDevelopment;
            adj.KBEFollowOnTooling = AdjustFactor.KBEFollowOnTooling;
            adj.NSAR = AdjustFactor.NSAR;
            adj.NSRR = AdjustFactor.NSRR;
            adj.SecondToolInFamily = AdjustFactor.SecondToolInFamily;
            adj.StressAnalysis = AdjustFactor.StressAnalysis;
            adj.SurfacingRequired = AdjustFactor.SurfacingRequired;
            adj.UpdatedDate = DateTime.Now;

            db.Entry(adj).State = EntityState.Modified;
            db.SaveChanges();

            return Redirect(Url.Action("Statistics", "Admin"));
        }

		[HttpPost]
		public JsonResult SavePermissions(int accessLevelID, int appUserID)
		{
			AppUser dbUser = db.AppUsers.SingleOrDefault(AppUser => AppUser.AppUserID == appUserID);
            if (Request.IsAuthenticated)
            {
                if (dbUser.AccessLevelID != accessLevelID && ModelState.IsValid)
                {
                    List<AppUser> users = db.AppUsers.ToList();
                    int adminCount = 0;
                    const int MIN_ADMINS = 2;
                    const int ADMIN_PERMISSION = 1;

                    foreach (AppUser appUser in users)
                    {
                        if (appUser.AccessLevelID == ADMIN_PERMISSION && !appUser.IsDeleted) //if user is admin
                        {
                            adminCount++;
                        }
                    }

                    if (adminCount <= MIN_ADMINS && accessLevelID != ADMIN_PERMISSION)
                    {
                        return Json(new { success = false, responseText = "Unable to change permissions. There must be at least two admins." }, JsonRequestBehavior.AllowGet);
                    }

                    try
                    {
                        dbUser.AccessLevelID = accessLevelID;
                        dbUser.UpdatedDate = DateTime.Today;
                        db.Entry(dbUser).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (DataException)
                    {
                        return Json(new { success = false, responseText = "Unable to update permissions" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }

            return Json(new { success = true, responseText = "Successfully updated permissions" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ChangePassword(string newPassword, int appUserID)
        {
            AppUser user = db.AppUsers.Find(appUserID);
            const int MIN_PASSWORD_LENGTH = 6;

            if (Request.IsAuthenticated)
            {
                if (user != null)
                {
                    if (newPassword.Length < MIN_PASSWORD_LENGTH)
                    {
                        return Json(new { success = false, responseText = "Password must be at least 6 characters long." }, JsonRequestBehavior.AllowGet);
                    }

                    user.PasswordSalt = Crypto.GenerateSalt();
                    var password = newPassword + user.PasswordSalt;
                    user.PasswordHash = Crypto.SHA256(password);
                    db.Entry(user).State = EntityState.Modified;
                    try
                    {
                        db.SaveChanges();
                    }
                    catch
                    {
                        return Json(new { success = false, responseText = "There was an error saving the password to the database." }, JsonRequestBehavior.AllowGet);
                    }

                    return Json(new { success = true, responseText = "Password change successful." }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { success = true, responseText = "Password change failed." }, JsonRequestBehavior.AllowGet);
        }

		public JsonResult DeleteUser(int? appUserID)
		{
			if (appUserID != null)
			{
                AppUser user = db.AppUsers.Find(appUserID);

                if (user.UserEmail == System.Web.HttpContext.Current.User.Identity.Name)
                {
                    return Json(new { success = false, responseText = "Delete failed. You cannot delete yourself." }, JsonRequestBehavior.AllowGet);
                }

                //Check the number of admins in the system
                List<AppUser> users = db.AppUsers.ToList();
                int adminCount = 0;
                const int MIN_ADMINS = 2;
                const int ADMIN_PERMISSION = 1;

                foreach (AppUser appUser in users)
                {
                    if (appUser.AccessLevelID == ADMIN_PERMISSION && !appUser.IsDeleted) //if user is admin
                    {
                        adminCount++;
                    }
                }
                
                //Cannot delete an admin if deleting and admin will make the number of admins in the system less than 2
                if (adminCount <= MIN_ADMINS && user.AccessLevelID == ADMIN_PERMISSION)
                {
                    return Json(new { success = false, responseText = "Delete failed. There must be at least two admins." }, JsonRequestBehavior.AllowGet);
                }

                if (user != null)
				{
                    user.IsDeleted = true;
                    db.SaveChanges();
                }

                return Json(new { success = true, responseText = "Delete successful." }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false, responseText = "Delete failed." }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateUser(FormCollection form)
        {
            var firstName = form["FirstName"];
            var lastName = form["LastName"];
            var userEmail = form["UserEmail"];
            var userID = form["UserID"];

            int intUserID = int.Parse(userID.ToString());

            AppUser dbUser = db.AppUsers.Find(intUserID);
            AppUser existingUser = db.AppUsers.SingleOrDefault(AppUser => AppUser.UserEmail == userEmail); //get current user from the database

            if (dbUser != null)
            {
                bool isLogout = false;

                if (existingUser != null && userEmail != dbUser.UserEmail)
                {
                    return Json(new { success = false, responseText = "Error with the input. This email address already exists in the system." }, JsonRequestBehavior.AllowGet);
                }

                dbUser.FirstName = firstName;
                dbUser.LastName = lastName;

                if (dbUser.UserEmail == System.Web.HttpContext.Current.User.Identity.Name && dbUser.UserEmail != userEmail)
                {
                    FormsAuthentication.SignOut();
                    isLogout = true;
                }

                dbUser.UserEmail = userEmail;

                db.Entry(dbUser).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                }
                catch
                {
                    return Json(new { success = false, responseText = "Error with the input. The email address may be invalid." }, JsonRequestBehavior.AllowGet);
                }

                if (isLogout) return Json(new { redirectUrl = Url.Action("Login", "User"), isRedirect = true});
                return Json(new { success = true, responseText = "Changes saved." }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false, responseText = "Unable to save changes to the database." }, JsonRequestBehavior.AllowGet);
        }

        //OTHER METHODS

        public Boolean CheckIfAdmin()
        {
            if (System.Web.HttpContext.Current.User.Identity.Name != "") //Checks to see if user is logged in (if this check isn't here, there will be an error)
            {
                String curUserEmail = System.Web.HttpContext.Current.User.Identity.Name;
                AppUser dbUser = db.AppUsers.SingleOrDefault(AppUser => AppUser.UserEmail == curUserEmail); //get current user from the database
                if (dbUser.AccessLevelID != 1) //if access level is not 1 (1 is Admin), then redirect to homepage 
                {
                    return false;
                }
            }

            return true;
        }
	}
}