using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Boeing_Tool_Design.Models;

namespace Boeing_Tool_Design.Controllers
{
    [Authorize]
    public class EstimatesController : Controller
    {
        private BoeingContext db = new BoeingContext();

        // GET: Estimates
        public ActionResult Index()
        {
            return RedirectToAction("DesignOrders", "Estimates");
        }
        
        // Design Orders Action
        public ActionResult DesignOrders()
        {
            // Get all Design Orders
            IQueryable<DesignOrder> DesignOrderLINQ = db.DesignOrders;
            List<DesignOrder> DesignOrders = DesignOrderLINQ.Where(d => d.IsDeleted == false).ToList();

            // Get only users who have created design orders
            List<AppUser> AppUsers = db.AppUsers.Where(u => DesignOrderLINQ.Select(d => d.CreatedByUserID).Contains(u.AppUserID)).ToList();

            // Get the latest estimates for those design orders
            List<Estimate> Estimates = db.Estimates.Where(e => e.IsLatestEstimate == true).ToList();

            // Add models to view model
            EstimatesViewModel ViewModel = new EstimatesViewModel();
            ViewModel.DesignOrders = DesignOrders;
            ViewModel.AppUsers = AppUsers;
            ViewModel.Estimates = Estimates;
            ViewModel.ToolTypes = db.ToolTypes.ToList();

            // Return the view
            return View(ViewModel);
        }

        // List Action
        /*
            Lists the estimates made for one particular design order
        */
        public ActionResult List(int? id)
        {
            // Pass the estimates to the view that match the selected design order
            ViewBag.DesignOrderSID = id;
            if (id == null)
            {
                // Redirect to the design orders page if they haven't selected a design order yet
                return RedirectToAction("DesignOrders", "Estimates");
            }
            else
            {
                // Get the Design Orders
                IQueryable<DesignOrder> DesignOrderLINQ = db.DesignOrders;
                List<DesignOrder> DesignOrders = DesignOrderLINQ.Where(d => d.DesignOrderSID == id).ToList();

                // Get all users
                List<AppUser> AppUsers = db.AppUsers.ToList();

                int DesignOrderSID = DesignOrders.First().DesignOrderSID;

                // Get all latest estimates for those design orders
                List<Estimate> Estimates = db.Estimates.Where(
                        e => e.DesignOrderSID == DesignOrderSID
                        && e.IsDeleted == false
                    ).OrderByDescending(e => e.CreatedDate).ToList();

                EstimatesViewModel ViewModel = new EstimatesViewModel();
                ViewModel.DesignOrders = DesignOrders;
                ViewModel.AppUsers = AppUsers;
                ViewModel.Estimates = Estimates;
                ViewModel.ToolTypes = db.ToolTypes.ToList();

                // Get all estimates for the selected design order
                return View(ViewModel);
            }
        }

        // GET Edit a design Order
        public ActionResult EditOrder(int? id)
        {
            // Initialize view model
            EstimatesViewModel ViewModel = new EstimatesViewModel();

            // Get the design order from the submitted ID
            DesignOrder DOrder = db.DesignOrders.Find(id);
            
            // Redirect if the ID is not a valid design order id
            if (DOrder == null)
            {
                return RedirectToAction("DesignOrders", "Estimates");
            }

            // Put needed elements on the view model
            ViewModel.ToolTypes = db.ToolTypes.ToList();
            ViewModel.DesignOrders = new List<DesignOrder> { DOrder };

            // Return the view
            return View(ViewModel);
        }

        // POST: Edit a design Order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOrder([Bind(Include = "DesignOrderSID,DescriptiveName,DesignOrderNumber,ToolTypeID,PartNumber,IsCompleted,ActualHours")] DesignOrder DOrder)
        {
            if (ModelState.IsValid)
            {
                DesignOrder NewOrder = db.DesignOrders.Find(DOrder.DesignOrderSID);

                NewOrder.DesignOrderNumber = DOrder.DesignOrderNumber;
                NewOrder.DescriptiveName = DOrder.DescriptiveName;
                NewOrder.ToolTypeID = DOrder.ToolTypeID;
                NewOrder.PartNumber = DOrder.PartNumber;
                NewOrder.IsCompleted = DOrder.IsCompleted;
                NewOrder.ActualHours = DOrder.ActualHours;

                db.Entry(NewOrder).State = EntityState.Modified;

                NewOrder.UpdatedDate = DateTime.Today;

                db.SaveChanges();
                return RedirectToAction("DesignOrders");
            }

            // Initialize view model
            EstimatesViewModel ViewModel = new EstimatesViewModel();

            // Redirect if the ID is not a valid design order id
            if (DOrder == null)
            {
                return RedirectToAction("DesignOrders", "Estimates");
            }

            // Put needed elements on the view model
            ViewModel.ToolTypes = db.ToolTypes.ToList();
            ViewModel.DesignOrders = new List<DesignOrder> { DOrder };

            return View(ViewModel);
        }

        public ActionResult DeleteOrder(int? id)
        {
            if (id != null)
            {
                DesignOrder DOrder = db.DesignOrders.Find(id);

                if (DOrder != null)
                {
                    DOrder.IsDeleted = true;
                    db.Entry(DOrder).State = EntityState.Modified;
                    db.SaveChanges();
                }

                // Delete all estimates made for that design order
                List<Estimate> lEstimate = db.Estimates.Where(e => e.DesignOrderSID == id).ToList();
                foreach (Estimate eEstimate in lEstimate)
                {
                    eEstimate.IsDeleted = true;
                    db.Entry(eEstimate).State = EntityState.Modified;
                }
                db.SaveChanges();
            }

            return RedirectToAction("DesignOrders");
        }

        // GET: Estimates/Create
        public ActionResult CreateOrder()
        {

            // Initialize the view model
            EstimatesViewModel ViewModel = new EstimatesViewModel();

            // Add the tool types to the view model (for drop down)
            ViewModel.DesignOrders = new List<DesignOrder> { new DesignOrder()};
            ViewModel.ToolTypes = db.ToolTypes.ToList();

            // Return the view
            return View(ViewModel);
        }

        // POST: Estimates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrder([Bind(Include = "DesignOrderSID,DescriptiveName,DesignOrderNumber,ToolTypeID,PartNumber,Completed,ActualHours")] DesignOrder DOrder)
        {
            if (ModelState.IsValid)
            {

                // check uniqueness 

                // aka if the db has a design order with that design order number already
                if (db.DesignOrders.Where( v => v.DesignOrderNumber == DOrder.DesignOrderNumber).FirstOrDefault() != null) {
                    ModelState.AddModelError("DOrderExists", "<span class='text-danger'> Design Order already exists. Click <a href=\"List/" + db.DesignOrders.Where( v => v.DesignOrderNumber == DOrder.DesignOrderNumber).FirstOrDefault().DesignOrderSID + "\">here</a> to view it.</span>");

                    // Initialize the view model
                    EstimatesViewModel ViewModel = new EstimatesViewModel();

                    // Add the tool types to the view model (for drop down)
                    ViewModel.DesignOrders = new List<DesignOrder> { new DesignOrder() };
                    ViewModel.ToolTypes = db.ToolTypes.ToList();

                    // Return the view
                    return View(ViewModel);
                }

                else {

                    db.DesignOrders.Add(DOrder);

                    DOrder.CreatedDate = DateTime.Today;
                    DOrder.IsDeleted = false;
                    DOrder.CreatedByUserID = db.AppUsers.Single(u => u.UserEmail == User.Identity.Name).AppUserID;

                    db.SaveChanges();

                
                    if (Request.Form["saveAndEstimate"] != null)
                    {
                        return RedirectToAction("Create", new { id = DOrder.DesignOrderSID });
                    }

                    return RedirectToAction("DesignOrders");
                }
            }

            return View(DOrder);
        }

        // GET: Estimates/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("DesignOrders");
            }
            Estimate estimate = db.Estimates.Find(id);
            if (estimate == null)
            {
                return HttpNotFound();
            }

            EstimatesViewModel ViewModel = new EstimatesViewModel();
            ViewModel.Estimates = new List<Estimate>();
            ViewModel.Estimates.Add(estimate);
            ViewModel.AppUsers = db.AppUsers.ToList();
            ViewModel.ToolTypes = db.ToolTypes.ToList();
            ViewModel.DesignOrders = db.DesignOrders.ToList();
            ViewModel.FamilyClasses = db.FamilyClasses.ToList();
            ViewModel.StressWorkTypes = db.StressWorkTypes.ToList();
            ViewModel.Statistics = db.Statistics.ToList();

            // Also get the id of the latest estimate for this view model
            ViewBag.LatestEstimateID = null;
            Estimate LatestEstimate = db.Estimates.Where(e => e.DesignOrderSID == estimate.DesignOrderSID && e.IsLatestEstimate == true).FirstOrDefault();
            if (LatestEstimate != null)
            {
                ViewBag.LatestEstimateID = LatestEstimate.EstimateID;
            }

            // Return the view model to the view
            return View(ViewModel);
        }

        // GET: Estimates/Create
        public ActionResult Create(int? id)
        {
            // Instantiate the view model we will use in this view
            EstimatesViewModel ViewModel = new EstimatesViewModel();
            
            // Some unused code for checking errors on hidden form elements
            //IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            //ViewBag.Errors = allErrors;

            // Check if they have submitted a design order id
            if (id == null)
            {
                return RedirectToAction("DesignOrders");
            }

            // Make sure the id is for a valid design order
            DesignOrder DOrder = db.DesignOrders.Find(id);
            if (DOrder == null)
            {
                return RedirectToAction("DesignOrders");
            }
            else
            {
                // Store the design order in the view model
                ViewModel.DesignOrders = new List<DesignOrder> { DOrder };
            }

            // Get other needed models for populating drop down lists etc
            ViewModel.StressWorkTypes = db.StressWorkTypes.ToList();
            ViewModel.ToolTypes = db.ToolTypes.ToList();
            ViewModel.FamilyClasses = db.FamilyClasses.ToList();
            ViewModel.AppUsers = db.AppUsers.ToList();
            ViewModel.Statistics = db.Statistics.ToList();
            ViewModel.ComplexityLevels = db.ComplexityLevels.ToList();

            // Get the latest estimate for this design order, if any
            Estimate LatestEstimate = db.Estimates
                .Where(e => e.DesignOrderSID == DOrder.DesignOrderSID)
                .Where(e => e.IsLatestEstimate == true)
                .ToList().FirstOrDefault();

            // Create an empty estimate and set the fields we already know
            Estimate est = new Estimate();
            est.DesignOrderSID = DOrder.DesignOrderSID;
            
            // If there aren't any previous estimates, set a default value for "reason for estimate change" to "initial estimate"
            if (LatestEstimate == null)
            {
                est.ReasonForEstimateChange = "Initial Estimate";
            }
            else
            {
                // Auto-fill this estimate with values from the latest estimate
                est.NeedsSurfacing = LatestEstimate.NeedsSurfacing;
                est.IsStressIncluded = LatestEstimate.IsStressIncluded;
                est.StressWorkTypeID = LatestEstimate.StressWorkTypeID;
                est.ComplexityLevel = LatestEstimate.ComplexityLevel;
                est.EngineeringReleased = LatestEstimate.EngineeringReleased;
                est.FamilyClassID = LatestEstimate.FamilyClassID;

            }

            // Return everything to the view
            ViewModel.Estimates = new List<Estimate> { est };
            return View(ViewModel);
        }

        // POST: Estimates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "DesignOrderSID,NeedsSurfacing,IsStressIncluded,StressWorkTypeID,ComplexityLevel,EngineeringReleased,FamilyClassID,Comment,ReasonForEstimateChange")] Estimate estimate)
        {
            //retrieve the tool's PK from the DB
            //int toolTypeID = db.Database.SqlQuery<DesignOrder>("Select * FROM DesignOrder WHERE DesignOrderSID =" 
            //                                                    + estimate.DesignOrderSID).ElementAt(0).ToolTypeID;

            int toolTypeID = db.DesignOrders.Where(d => d.DesignOrderSID == estimate.DesignOrderSID).FirstOrDefault().ToolTypeID;

            //data object for the statistics of a specific tool
            //Statistic toolData = db.Database.SqlQuery<Statistic>("Select * FROM Statistic WHERE ToolTypeID =" + toolTypeID 
            //                                                    + "AND ComplexityLevel =" + estimate.ComplexityLevel 
            //                                                    + "AND isCurrent = 1").ElementAt(0);

            Statistic toolData = db.Statistics.Where(s => s.ToolTypeID == toolTypeID && s.ComplexityLevel == estimate.ComplexityLevel && s.isCurrent == true).FirstOrDefault();

            //Create the ToolEstimator to create estimates based on user specifications
            ToolEstimator toolEstimator = new ToolEstimator(estimate, toolData, toolTypeID);
            toolEstimator.CreateEstimations();

            //ViewBag variables need to be dynamic
            var releaseHours = (dynamic)null;
            var designHours = (dynamic)null;
            var stressHours = (dynamic)null;
            var checkHours = (dynamic)null;
            var flowDays = (dynamic)null;
            var totalHours = (dynamic)null;

            releaseHours = Math.Round(toolEstimator.GetReleaseHours());
            designHours = Math.Round(toolEstimator.GetDesignHours());
            stressHours = Math.Round(toolEstimator.GetStressHours());
            checkHours = Math.Round(toolEstimator.GetCheckHours());
            flowDays = Math.Round(toolEstimator.GetFlowDays());
            totalHours = Math.Round(releaseHours + designHours + stressHours + checkHours);

            //Set estimate values on the webpage
            ViewBag.complexity = estimate.ComplexityLevel;
            ViewBag.ReleaseHour = releaseHours;
            ViewBag.DesignHour = designHours;
            ViewBag.StressHour = stressHours;
            ViewBag.CheckHour = checkHours;
            ViewBag.FlowDays = flowDays;
            ViewBag.TotalHours = totalHours;

            //User clicked "Accept Estimate"
            if (Request.Form["accept"] != null)
            {
                return AcceptEstimate(estimate);
            }
            //User clicked "Preview Estimate"
            else
            {
                return PreviewEstimate(estimate);
            }
        }

        [ValidateAntiForgeryToken]
        public ActionResult AcceptEstimate(Estimate estimate)
        {
            if (ModelState.IsValid)
            {
                // Set all previous estimates on this design order to not "islatestEstimate"
                List<Estimate> OldEstimates = db.Estimates.Where(e => e.DesignOrderSID == estimate.DesignOrderSID).ToList();
                if (OldEstimates.Count > 0)
                {
                    foreach (Estimate est in OldEstimates)
                    {
                        // Set flag to false
                        est.IsLatestEstimate = false;
                    }
                    db.SaveChanges();
                }

                estimate.IsLatestEstimate = true;
                estimate.IsDeleted = false;
                estimate.CreatedDate = DateTime.Now;
                estimate.CreatedByUserID = db.AppUsers.Single(u => u.UserEmail == User.Identity.Name).AppUserID;

                // Save new estimate to the database
                db.Estimates.Add(estimate);
                db.SaveChanges();

                // Redirect to the list of estimates for this design order
                return RedirectToAction("List", new { id = estimate.DesignOrderSID });
            }
            return View();
        }

        [ValidateAntiForgeryToken]
        public ActionResult PreviewEstimate(Estimate estimate)
        { 
                // Instantiate the view model we will use in this view
                EstimatesViewModel ViewModel = new EstimatesViewModel();

                // Make sure the id is for a valid design order
                DesignOrder DOrder = db.DesignOrders.Find(estimate.DesignOrderSID);
                if (DOrder == null)
                {
                    return RedirectToAction("DesignOrders");
                }
                else
                {
                    // Store the design order in the view model
                    ViewModel.DesignOrders = new List<DesignOrder> { DOrder };
                }

                // Get other needed models for populating drop down lists etc
                ViewModel.StressWorkTypes = db.StressWorkTypes.ToList();
                ViewModel.ToolTypes = db.ToolTypes.ToList();
                ViewModel.FamilyClasses = db.FamilyClasses.ToList();
                ViewModel.AppUsers = db.AppUsers.ToList();
                ViewModel.Statistics = db.Statistics.ToList();
                ViewModel.ComplexityLevels = db.ComplexityLevels.ToList();

                ViewModel.Estimates = new List<Estimate> { estimate };

                return View(ViewModel);
        }

        // GET: Estimates/Delete/5
        /*
            Delete an estimate
         */
        public ActionResult Delete(int? id)
        {
            // TODO Set islatest flag on new latest estimate
            Estimate estimate = db.Estimates.Find(id);
            int DesignOrderSID = estimate.DesignOrderSID;
            estimate.IsDeleted = true;

            if (estimate.IsLatestEstimate == true)
            {
                estimate.IsLatestEstimate = false;

                // If this was the latest estimate, set the previous one to "is Latest"
                Estimate NewLatest = db.Estimates.Where(e => e.IsDeleted == false
                    && e.DesignOrderSID == estimate.DesignOrderSID
                    && e.EstimateID != estimate.EstimateID
                ).OrderByDescending(e => e.CreatedDate).FirstOrDefault();

                if (NewLatest != null)
                {
                    NewLatest.IsLatestEstimate = true;
                    db.Entry(NewLatest).State = EntityState.Modified;
                }
            }

            db.Entry(estimate).State = EntityState.Modified;

            db.SaveChanges();
            return RedirectToAction("List", new { id = DesignOrderSID });
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

    public class ToolEstimator
    {
        private Estimate estimate;
        private Statistic toolData;
        private int toolTypeID;
        private BoeingContext db;

        //CALCULATION NUMBERS (back end numbers to create the estimation)
        private double baseHours; //(cell B8)
        private double minHours; //(cell B9)
        private double NSAR; //(cell B14)
        private double stressAnalysisRequired; //(cell B16)
        private double surfacing; //(cell B17)
        private double engineering; //(cell B18)
        private double adjustment; //(cell B25)

        //ESTIMATE NUMBERS (what the user sees once an estimate is made)
        private double releaseHours;
        private double designHours;
        private double stressHours;
        private double checkHours;
        private double flowDays;

        public ToolEstimator(Estimate estimate, Statistic toolData, int toolTypeID)
        {
            this.estimate = estimate;
            this.toolData = toolData;
            this.toolTypeID = toolTypeID;
            baseHours = toolData.AverageHours; //number is managed in the admin portal
            releaseHours = toolData.ReleaseHours; //default release hours is equal to toolData.ReleaseHours. Can be changed in admin portal
            db = new BoeingContext();
        }

        public void CreateEstimations()
        {
            const int DEFAULT_FACTOR = 0; 

            //Family class IDs
            const int INITIAL_DESIGN = 1;
            const int SECOND_TOOL_IN_FAMILY = 2;
            const int ADDITIONAL_TOOLS_IN_FAMILY = 3;

            //Sets calculation numbers. MUST BE SET IN THE CORRECT ORDER
            SetMinHours();
            SetNSAR();
            SetStressAnalysisRequired();
            SetSurfacing();
            SetEngineering();
            SetAdjustments();
            
            //Calculate defers depending on which hours breakdown the user wants (cells B38:G38)
            if (estimate.FamilyClassID == INITIAL_DESIGN) {
                var initialDesignAdjustmentFactor = db.AdjustmentFactors.Single(u => u.AdjustmentFactorID == 1).FirstToolInFamily;
                var initialDesignFactor = (toolData.StandardDeviation * initialDesignAdjustmentFactor);
                
                CalculateToolFamilies((double)initialDesignFactor);
            }
            else if (estimate.FamilyClassID == SECOND_TOOL_IN_FAMILY){
                var toolFamilyTwoAdjustmentFactorFactor = db.AdjustmentFactors.Single(u => u.AdjustmentFactorID == 1).SecondToolInFamily;
                var toolFamilyTwoFactor = (toolData.StandardDeviation * toolFamilyTwoAdjustmentFactorFactor);
                
                CalculateToolFamilies((double)toolFamilyTwoFactor);
            }
            else if (estimate.FamilyClassID == ADDITIONAL_TOOLS_IN_FAMILY){
                CalculateToolFamilies(DEFAULT_FACTOR);
            }
            else { CalculateKBE((int)estimate.FamilyClassID); }
            
            CalculateFlow();
        }

        private void SetMinHours()
        {
            const int DEFAULT_MIN_HOURS = 40;
            
            if (estimate.ComplexityLevel == 1)
            {
               minHours = DEFAULT_MIN_HOURS;
            }
            else
            {
                int complexitylevel = estimate.ComplexityLevel - 1;
                minHours = db.Database.SqlQuery<Statistic>("Select * FROM Statistic WHERE ToolTypeID =" + toolTypeID + "AND ComplexityLevel =" + complexitylevel + "AND isCurrent = 1").ElementAt(0).AverageHours;
            }
        }

        private void SetNSAR()
        {
            //if stress hour included in data is checked
            if (estimate.IsStressIncluded == true)
            {
                NSAR = 0;
            }
            else if (estimate.StressWorkTypeID == 2 || estimate.StressWorkTypeID == 3)
            {
                //if NSAR is check or Stress Analysis Reuqired is Check(when SAR is checked, NSAR will automatically be checked as well)
                var NSARAdjustmentFactor = db.AdjustmentFactors.Single(u => u.AdjustmentFactorID == 1).NSAR;
                if (toolData.StandardDeviation * NSARAdjustmentFactor < 4)
                {
                    NSAR = 4;
                }
                else
                {
                    NSAR = (double)(toolData.StandardDeviation * NSARAdjustmentFactor);
                }
            }
            else
            {
                NSAR = 0;
            }
        }

        private void SetStressAnalysisRequired()
        {
            if (estimate.IsStressIncluded == true)
            {
                stressAnalysisRequired = 0;
            }
            //If Stress Analysis Required is checked
            else if (estimate.StressWorkTypeID == 3)
            {
                var Stress_Analysis_Factor = db.AdjustmentFactors.Single(u => u.AdjustmentFactorID == 1).StressAnalysis;
                stressAnalysisRequired = (double)(toolData.StandardDeviation * Stress_Analysis_Factor);
            }
            else
            {
                stressAnalysisRequired = 0;
            }
        }

        private void SetSurfacing()
        {
            if (estimate.NeedsSurfacing == true)
            {
                var Surfacing_Factor = db.AdjustmentFactors.Single(u => u.AdjustmentFactorID == 1).SurfacingRequired;
                surfacing = (double)(toolData.StandardDeviation * Surfacing_Factor);
            }
            else
            {
                surfacing = 0;
            }
        }

        private void SetEngineering()
        {
            if (estimate.EngineeringReleased == true)
            {
                engineering = 0;
            }
            else
            {
                var Engineer_Factor = db.AdjustmentFactors.Single(u => u.AdjustmentFactorID == 1).EngineeringReleased;
                engineering = (double)(toolData.StandardDeviation * Engineer_Factor);
            }
        }

        private void SetAdjustments()
        {
            //Check 1 (cell B23)
            string check1;
            if ((NSAR + stressAnalysisRequired + surfacing + engineering + baseHours) < minHours)
            {
                check1 = "T";
            }
            else
            {
                check1 = "F";
            }

            //Check 2 (cell B24)
            string check2;
            if ((NSAR + stressAnalysisRequired + surfacing + engineering) > (3 * toolData.StandardDeviation))
            {
                check2 = "T";
            }
            else
            {
                check2 = "F";
            }

            //Calculate Adjustment (cell B25)
            if (check1 == "T")
            {
                adjustment = minHours - baseHours;
            }
            else if (check2 == "T")
            {
                adjustment = (3 * toolData.StandardDeviation);
            }
            else
            {
                adjustment = (NSAR + stressAnalysisRequired + surfacing + engineering);
            }
        }

        //The factor determines which tool family needs to be calculate, the factor is 0 when additional tools in family is selected
        private void CalculateToolFamilies(double factor){

            double totalHours = baseHours + adjustment - releaseHours;
            
            //Design hours
            if (estimate.IsStressIncluded == false)
            {
                designHours = (totalHours + factor) * (0.9);
            }
            else
            {
                designHours = (totalHours + factor) * (0.95 / 2);
            }
            
            //Stress hours
            CalculateStressHoursToolFamily();
            //Check hours
            CalculateCheckHours(false); //False because this is a non-KBE calculation
        }
        
        private void CalculateStressHoursToolFamily(){

            if (estimate.IsStressIncluded == false)
            {
                if (estimate.StressWorkTypeID == 3)
                {
                    stressHours = designHours;
                }
                else
                {
                    stressHours = 0;
                }
            }
            else
            {
                stressHours = designHours;
            }
        }
        
        private void CalculateKBE(int classID){
            const int DEFAULT_RELEASE_HOURS = 8;
            const int DEFAULT_KBE_STRESS_HOURS = 0;
            const int KBE_DEVELOPMENT_TOOL = 4;
            const int KBE_FOLLOW_ON_TOOL_WITH_NSAR = 5;
            const int KBE_FOLLOW_ON_TOOL_WITH_NSRR = 6;
            
            if (classID == KBE_DEVELOPMENT_TOOL)
            {
                //KBE Development (cell B21)
                var KBE_Factor = db.AdjustmentFactors.Single(u => u.AdjustmentFactorID == 1).KBEDevelopment;
                var KBE = (toolData.StandardDeviation * KBE_Factor);

                designHours = (double)KBE;
                checkHours = stressHours = releaseHours = 0;
                return;
            }
            
            //Design Hours
            CalculateKBEDesignHours();
            //Check Hours
            CalculateCheckHours(true); //true because this is a KBE calculation
            //Release Hours
            releaseHours = DEFAULT_RELEASE_HOURS;
            
            //Stress Hours
            if (classID == KBE_FOLLOW_ON_TOOL_WITH_NSAR){
                //stress hour (cell F40)
                var NSAR_Adj_Factor = db.AdjustmentFactors.Single(u => u.AdjustmentFactorID == 1).NSAR;
                if (toolData.StandardDeviation * NSAR_Adj_Factor > designHours)
                {
                    stressHours = designHours;
                }
                else
                {
                    stressHours = (double)(toolData.StandardDeviation * NSAR_Adj_Factor);
                }
                return;
            }
            else if (classID == KBE_FOLLOW_ON_TOOL_WITH_NSRR) {
                stressHours = DEFAULT_KBE_STRESS_HOURS;
                releaseHours = DEFAULT_RELEASE_HOURS;
            }
        }

        private void CalculateKBEDesignHours(){
            //Magic numbers are constants in the orginal Excel file

            double KBEDesignHours;
            double totalHours = baseHours + adjustment - releaseHours;

            if (estimate.IsStressIncluded == false)
            {
                KBEDesignHours = (totalHours) * (0.9);
            }
            else
            {
                KBEDesignHours = (totalHours) * (0.95 / 2);
            }
            
            if (KBEDesignHours * 0.25 < 24)
            {
                designHours = 24;
            }
            else
            {
                designHours = KBEDesignHours * 0.25;
            }
        }
        
        private void CalculateCheckHours(bool isKBE){
            //Magic numbers are constants in the orginal Excel file

            const double CHECK_HOURS_ADJUSTMENT = 0.11111111111;
            int checkHoursCompare;

            if (isKBE)
            {
                checkHoursCompare = 8;
            }
            else checkHoursCompare = 4;

            //check hour (cell G41)
            if (designHours * CHECK_HOURS_ADJUSTMENT < checkHoursCompare)
            {
                checkHours = checkHoursCompare;
            }
            else
            {
                checkHours = (designHours * CHECK_HOURS_ADJUSTMENT);
            }
        }

        private void CalculateFlow()
        {
            estimate.CheckHour = (int)checkHours;
            estimate.DesignHour = (int)designHours;
            estimate.StressHour = (int)stressHours;
            estimate.ReleaseHour = (int)releaseHours;
            estimate.HoursEstimate = (int)(designHours + stressHours + checkHours + releaseHours);
            float flowFactor = (float)db.ToolTypes.Single(tool => tool.ToolTypeID == toolData.ToolTypeID).StandardFlow;

            //flow estimate in days
            flowDays = Math.Ceiling((estimate.HoursEstimate - stressHours) / flowFactor);
            estimate.DesignFlow = (int)flowDays;
        }

        public double GetReleaseHours() { return releaseHours; }
        public double GetStressHours() { return stressHours;  }
        public double GetDesignHours() { return designHours; }
        public double GetCheckHours() { return checkHours; }
        public double GetFlowDays() { return flowDays; }
    }
}

           
