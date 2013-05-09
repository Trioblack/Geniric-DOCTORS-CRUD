using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using AB2EDEMO.Models;

namespace AB2EDEMO.Controllers
{
    /// <summary>
    /// The Class has been made to define actions that the controller will handle
    /// Here, the code for varios actions have been written as various methods of the controller class
    /// this class is conroller specific rather than being a generic class applicable to all the controllers
    /// </summary>
    [SessionState(SessionStateBehavior.Required)]
    public class HospitalController : Controller
    {
        //Constant value to define the number of records on the Grid Screen
        private const int PageSize = 10;
        public String ErrorField;
        public String ErrorMessage;
        //String value to store value of hospital for indexing 
        private string _idkey;

        /// <summary>
        /// Contains code for the default action (index page call)
        /// Action Handled::
        /// GET: /Hospital
        /// </summary>
        /// <param name="page"></param>
        /// <param name="filteredIndex"></param>
        /// <returns></returns>
        public ActionResult HospitalGrid(int? page, int? filteredIndex)
        {
            if (Session["connectionString"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var _db = new Ab2edemoEntities(Session["connectionString"].ToString());
            PagedItem<HospitalGdo> hospitals;
            ConfigurationManager.RefreshSection("connectionStrings");
            ViewBag.userName = Session["username"];
            ViewBag.ValNotFound = false;
            if (page > 0)
                hospitals = GetPagedHospitals((int)(page), PageSize);
            else
            {
                if (page == null)
                {
                    hospitals = GetPagedHospitals(0, PageSize);
                    if ((TempData["programCall"] != null && (bool)TempData["programCall"]) || (TempData["cancelSelect"] != null && (bool)TempData["cancelSelect"]))
                    {
                        SetNavInfo("Hospital/HospitalGrid");
                    }
                }
                else
                {
                    hospitals = GetPagedHospitals(0, (PageSize + Convert.ToInt32(page)));
                }
            }

            if (filteredIndex == null)
                filteredIndex = 0;
            ViewBag.HasPrevious = page - 10 >= filteredIndex && hospitals.HasPrevious;
            var table = hospitals.Entities;
            ViewBag.HasMore = hospitals.HasNext;
            ViewBag.CurrentPage = (page ?? 0);
            ViewBag.FilteredIndex = filteredIndex;
            if (TempData["postBackError"] != null) TempData["postBackError"] = TempData["postBackError"].ToString();
            return PartialView(table);
        }

        /// <summary>
        /// Contains Code for the filtering action from the Grid Page
        /// Default Action:
        /// POST /Hospital
        /// </summary>
        /// <param name="formCollection"></param>
        /// <param name="rowStart"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult HospitalGrid(FormCollection formCollection, string rowStart)
        {
            if (Session["connectionString"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var _db = new Ab2edemoEntities(Session["connectionString"].ToString());
            string rowstart = rowStart.TrimEnd().ToUpper();
            TempData["layout"] = "";
            TempData["Result"] = " ";

            var hosps = _db.Hospitals.OrderBy(hosp => hosp.Hospital_Code).ToList();
            var key = formCollection["ctl_country"];
            if (key != String.Empty) key = key.ToUpper();
            var key2 = formCollection["ctl_hospital_name"];
            var hospitals = (from data in _db.Hospitals.OrderBy(hosp => hosp.Country)
                             where (String.Compare(data.Hospital_Name.TrimEnd(), key2) >= 0 && String.Compare(data.Country.TrimEnd(), key) >= 0)
                             select data).Take(10).ToList();
            var hospitalGdo = new List<HospitalGdo>();
            if (hospitals.Count > 0)
            {
                foreach (var hosp in hospitals)
                {
                    var item = new HospitalGdo
                                   {
                                       Address_Post_Zip = hosp.Address_Post_Zip,
                                       Address_Province = hosp.Address_Province,
                                       Address_Street = hosp.Address_Street,
                                       Address_Town = hosp.Address_Town,
                                       Country = hosp.Country,
                                       Fax_Number = hosp.Fax_Number,
                                       Hospital_Code = hosp.Hospital_Code,
                                       Hospital_Name = hosp.Hospital_Name,
                                       Telephone_Number = hosp.Telephone_Number
                                   };
                    item.Total_Patient = (from patient in _db.Patients
                                          where patient.Hospital_Code == item.Hospital_Code
                                          select patient).Count().ToString();
                    item.Total_Wards = (from ward in _db.Wards
                                        where ward.Hospital_Code == item.Hospital_Code
                                        select ward).Count().ToString();
                    hospitalGdo.Add(item);
                }
            }
            if (hospitals.Count == 0)
            {
                hospitals = (from data in _db.Hospitals.OrderBy(hosp => hosp.Country)
                             where (String.Compare(data.Country.TrimEnd(), rowstart) >= 0)
                             select data).Take(10).ToList();
                foreach (var hosp in hospitals)
                {
                    var item = new HospitalGdo
                    {
                        Address_Post_Zip = hosp.Address_Post_Zip,
                        Address_Province = hosp.Address_Province,
                        Address_Street = hosp.Address_Street,
                        Address_Town = hosp.Address_Town,
                        Country = hosp.Country,
                        Fax_Number = hosp.Fax_Number,
                        Hospital_Code = hosp.Hospital_Code,
                        Hospital_Name = hosp.Hospital_Name,
                        Telephone_Number = hosp.Telephone_Number
                    };
                    item.Total_Patient = (from patient in _db.Patients
                                          where patient.Hospital_Code == item.Hospital_Code
                                          select patient).Count().ToString();
                    item.Total_Wards = (from ward in _db.Wards
                                        where ward.Hospital_Code == item.Hospital_Code
                                        select ward).Count().ToString();
                    hospitalGdo.Add(item);
                }
            }
            int index = hosps.TakeWhile(hosp => hospitals[0].Hospital_Code.TrimEnd() != hosp.Hospital_Code.TrimEnd()).Count();
            ViewBag.HasPrevious = false;
            ViewBag.HasMore = (index + 9 < hosps.Count);
            ViewBag.CurrentPage = ViewBag.FilteredIndex = index;
            return PartialView(hospitalGdo);
        }

        /// <summary>
        /// Contains Code for the Add action on the Grid Screen
        /// Default Action:
        /// GET /Ab2edemo/HospitalEntryPanel/Add
        /// </summary>
        /// <returns></returns>
        public ActionResult HospitalEntryPanel()
        {
            var hospital = new Hospital();
            if (Session["connectionString"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            TempData["readonly"] = "";
            if (TempData["redir"] != null && TempData["redir"].ToString() == "Add") TempData["readonly"] = "false";
            if (TempData["Result"] == null) TempData["Result"] = " ";
            if (Convert.ToBoolean(TempData["cancelSelect"]))
            {
                hospital = (Hospital)Session["HospitalEntryPanel"];
                SetNavInfo("Hospital/HospitalEntryPanel");
            }
            else
            {
                hospital = (Hospital)TempData["dis"];
            }
            LoadDropDownData();
            if (hospital == null)
            {
                return HttpNotFound();
            }
            TempData["redir"] = "Edit";
            return PartialView(hospital);
        }

        /// <summary>
        /// Contains Code for various action that can be taken on the Grid Screen
        /// Default Action:
        /// POST /Ab2edemo/HospitalEntryPanel
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult HospitalEntryPanel(FormCollection formCollection)
        {
            if (Session["connectionString"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            
            for (int index = 1; index < (formCollection.Count - 1); index++)
            {
                if (formCollection[index].Split(',').ToArray()[0].Trim() == "true")
                {
                    _idkey = formCollection.AllKeys[index];
                }
            }
            if (_idkey == null)
            {
                TempData["postBackError"] = "Please Select a record !!";
                return RedirectToAction("HospitalGrid", "Hospital");
            }
            var _db = new Ab2edemoEntities(Session["connectionString"].ToString());
            String mode = formCollection[0].Split(',').ToArray()[1].TrimEnd();
            var hospital = _db.Hospitals.Find(_idkey);
            switch (mode)
            {
                case "Wards":
                    TempData["programCall"] = true;
                    return RedirectToAction("WardsGrid", "Wards", new { hospitalCode = _idkey });
                default:
                    TempData["readonly"] = "";
                    TempData["redir"] = "Edit";
                    ViewBag.Title = "HospitalEntryPanel";
                    break;
            }
            LoadDropDownData();
            SetNavInfo("Hospital/HospitalEntryPanel");
            return PartialView(hospital);
        }

        /// <summary>
        /// Contains code to load the data for country dropdown.
        /// </summary>
        /// <param name="hospital">model object</param>
        public void LoadDropDownData()
        {
            var sources = new List<SelectListItem>
                              {
                                  new SelectListItem() {Text = "USA -United States of America", Value = "USA"},
                                  new SelectListItem() {Text = "RSA -South Africa", Value = "RSA"},
                                  new SelectListItem() {Text = "UK -United Kingdom", Value = "UK"},
                                  new SelectListItem() {Text = "FRA -France", Value = "FRA"},
                                  new SelectListItem() {Text = "AUS -Australia", Value = "AUS"},
                                  new SelectListItem() {Text = "CAN -Canada", Value = "CAN"},
                                  new SelectListItem() {Text = "GER -Germany", Value = "GER"},
                                  new SelectListItem() {Text = "-Blank"}
                              };
            ViewBag.countrylist = sources;
        }

        /// <summary>
        /// Contains Code for the Panel Screen
        /// Default Action:
        /// POST /Ab2edemo/HospitalPanel
        /// </summary>
        /// <param name="hospitals">model object</param>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult HospitalPanel(Hospital hospitals, FormCollection formCollection)
        {
            TempData["Readonly"] = "";
            TempData["redir"] = formCollection.AllKeys[formCollection.Count - 2];
            Hospital hospital = hospitals;
            return PartialView(hospital);
        }
        /// <summary>
        /// Contains Code for the Save action on the Panel Screen
        /// Default Action:
        /// POST /Ab2edemo/HospitalEntryPanel/Save
        /// </summary>
        /// <param name="hospital"></param>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Save(Hospital hospital, FormCollection formCollection)
        {
            if (Session["connectionString"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var _db = new Ab2edemoEntities(Session["connectionString"].ToString());
            string redirectAction;
            ValidateModel(hospital);
            if (ModelState.IsValid)
            {
                if (formCollection.AllKeys[0].ToString() == "Wards")
                {
                    Session["HospitalEntryPanel"] = hospital;
                    TempData["programCall"] = true;
                    return RedirectToAction("WardsGrid", "Wards", new { hospitalCode = hospital.Hospital_Code });
                }
            }

            if (formCollection.AllKeys[0].ToString() == "Delete")
            {
                Hospital hosp = _db.Hospitals.Find(hospital.Hospital_Code.TrimEnd());
                _db.Hospitals.Remove(hosp);
                _db.SaveChanges();
                TempData["programCall"] = true;
                TempData["Result"] = "";
                redirectAction = "HospitalGrid";
            }
            else
            {
                //ValidateModel(hospital);
                if (ModelState.IsValid)
                {
                    try
                    {
                        if (formCollection.Keys[formCollection.Count - 2] == "Add")
                        {
                            hospital.Hospital_Code = hospital.Hospital_Code.Trim().ToUpper();
                            _db.Entry(hospital).State = EntityState.Added;
                            _db.SaveChanges();
                            TempData["Result"] = "";
                        }
                        else
                        {
                            hospital.Country = hospital.Country.Split('-')[0].TrimEnd();
                            Hospital context = _db.Hospitals.Find(hospital.Hospital_Code);
                            if (!Check.AreEqual(hospital,context))
                            {
                                var objectStateManager = ((((IObjectContextAdapter)_db).ObjectContext)).GetObjectByKey(new EntityKey("Ab2edemoEntities.Hospitals", "Hospital_Code", hospital.Hospital_Code));
                                if (((Hospital)objectStateManager).Hospital_Code == hospital.Hospital_Code)
                                    ((((IObjectContextAdapter)_db).ObjectContext)).ApplyCurrentValues(
                                        "Ab2edemoEntities.Hospitals", hospital);
                                else
                                    _db.Entry(hospital).State = EntityState.Modified;
                                _db.SaveChanges();
                                TempData["programCall"] = true;
                                TempData["Result"] = "";
                            }
                            else
                            {
                                TempData["Result"] = "Nothing to Update!!!";
                            }
                        }
                        var navInfo = (Stack<NameValueCollection>)Session["navigationHistory"];
                        navInfo.Pop();
                        navInfo.Pop();
                        Session["navigationHistory"] = navInfo;
                        redirectAction = "HospitalGrid";
                    }
                    catch (DbEntityValidationException dbEx)
                    {
                        foreach (DbEntityValidationResult validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (DbValidationError validationError in validationErrors.ValidationErrors)
                            {
                                TempData["Result"] = TempData["Result"] + "<br />" + validationError.ErrorMessage;
                            }
                        }
                        TempData["dis"] = hospital;
                        redirectAction = "HospitalEntryPanel";
                    }
                }
                else
                {
                    redirectAction = "HospitalEntryPanel";
                    if (!ModelState.IsValid)
                    {
                        TempData["redir"] = formCollection.Keys[formCollection.Count - 2];
                        TempData["dis"] = hospital;
                        IEnumerable<string> allElements = from item in ModelState
                                                          where item.Value.Errors.Any()
                                                          select item.Key;
                        IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                        foreach (ModelError item in allErrors)
                        {
                            if (ErrorMessage != null)
                            {
                                ErrorMessage = ErrorMessage + Environment.NewLine + item.ErrorMessage;
                            }
                            else
                            {
                                ErrorMessage = item.ErrorMessage;
                            }
                        }
                        foreach (string item in allElements)
                        {
                            if (ErrorField != null)
                            {
                                ErrorField = ErrorField + " " + item;
                            }
                            else
                            {
                                ErrorField = item;
                            }
                        }
                        TempData["errorElement"] = ErrorField;
                        TempData["message"] = ErrorMessage;
                    }
                    else
                    {
                        return PartialView(hospital);
                    }

                }
            }
            return RedirectToAction(redirectAction);
        }

        /// <summary>
        /// Method to validate the model(Purchases)
        /// </summary>
        /// <param name="hospital"></param>
        [AcceptVerbs(HttpVerbs.Post)]
        public void ValidateModel(Hospital hospital)
        {
            if (UtilsController.MessagesCol == null)
                UtilsController.LoadMessages();
            var errMsg = new string[] { };
            if (hospital.Hospital_Name == null || hospital.Hospital_Name.Trim() == "")
            {
                if (UtilsController.MessagesCol != null) errMsg = UtilsController.MessagesCol.GetValues("USR0073");
                if (errMsg != null) ModelState.AddModelError("hospital_name", errMsg[0]);
            }
        }

        /// <summary>
        /// Method to query specific number of records from the database.
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public PagedItem<HospitalGdo> GetPagedHospitals(int skip, int take)
        {
            var _db = new Ab2edemoEntities(Session["connectionString"].ToString());
            var hospitalGdo = new List<HospitalGdo>();
            var getHospitals = _db.Hospitals.OrderBy(c => c.Country);
            foreach (var hospital in getHospitals)
            {
                var gdo = new HospitalGdo
                              {
                                  Country = hospital.Country,
                                  Fax_Number = hospital.Fax_Number,
                                  Telephone_Number = hospital.Telephone_Number,
                                  Hospital_Code = hospital.Hospital_Code,
                                  Hospital_Name = hospital.Hospital_Name,
                                  Address_Post_Zip = hospital.Address_Post_Zip,
                                  Address_Province = hospital.Address_Province,
                                  Address_Street = hospital.Address_Street,
                                  Address_Town = hospital.Address_Town
                              };
                gdo.Total_Wards = (from ward in _db.Wards
                                   where ward.Hospital_Code == gdo.Hospital_Code
                                   select ward).Count().ToString();
                gdo.Total_Patient = (from patient in _db.Patients
                                     where patient.Hospital_Code == gdo.Hospital_Code
                                     select patient).Count().ToString();
                hospitalGdo.Add(gdo);
            }
            var hospitalCount = getHospitals.Count();
            var hospitals = hospitalGdo.Skip(skip).Take(take).ToList();
            return new PagedItem<HospitalGdo>
            {
                Entities = hospitals,
                HasNext = (skip + 10 < hospitalCount),
                HasPrevious = (skip > 0)
            };
        }

        /// <summary>
        /// Method to add navigation History into a session stack
        /// </summary>
        /// <param name="navigation"></param>
        /// <returns></returns>

        public void SetNavInfo(string navigation)
        {
            Stack<NameValueCollection> navInfo;
            if (Session["navigationHistory"] == null)
                navInfo = new Stack<NameValueCollection>();
            else
                navInfo = (Stack<NameValueCollection>)Session["navigationHistory"];
            navInfo.Push(new NameValueCollection { { (navigation.Split('/'))[0], (navigation.Split('/'))[1] } });
            Session["navigationHistory"] = navInfo;
        }
    }
}