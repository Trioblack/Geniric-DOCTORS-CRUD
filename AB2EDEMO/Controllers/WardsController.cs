using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using AB2EDEMO.Models;

namespace AB2EDEMO.Controllers
{
    public class WardsController : Controller
    {
        //Constant value to define the number of records on the Grid Screen
        private const int PageSize = 10;
        //String value to store value of customer for indexing 
        private string _idkey;
        public String ErrorField;
        public String ErrorMessage;
        //
        // GET: /Wards/

        public static string HospitalCode;
        public ActionResult WardsGrid(int? page, int? filteredIndex, string hospitalCode)
        {
            if (Session["connectionString"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var _db = new Ab2edemoEntities(Session["connectionString"].ToString());
            if (hospitalCode != null) HospitalCode = hospitalCode;
            PagedItem<WardsGdo> ward;
            if (page > 0)
                ward = GetPagedWards((int)(page), PageSize, hospitalCode);
            else
            {

                if (page == null)
                {
                    ViewBag.Load01D = true;
                    if ((TempData["programCall"] != null && (bool)TempData["programCall"]) || (TempData["cancelSelect"] != null && (bool)TempData["cancelSelect"]))
                    {
                        SetNavInfo("Wards/WardsGrid");
                    }
                    if (hospitalCode == String.Empty)
                    {
                        RedirectToAction("Cancel", "Utils");
                    }
                    var wardsPdo = getWardsPdo(HospitalCode);
                    ViewBag.country = wardsPdo.Country;
                    ViewBag.hospitalName = wardsPdo.Hospital_Name;
                    ViewBag.telephoneno = wardsPdo.Telephone_Number;
                    ward = wardsPdo.Ward;

                }
                else
                {
                    ward = GetPagedWards(0, PageSize + Convert.ToInt32(page), hospitalCode);
                }
            }
            if (filteredIndex == null)
                filteredIndex = 0;
            ViewBag.hospitalcode = HospitalCode;
            ViewBag.HasPrevious = page - 10 >= filteredIndex && ward.HasPrevious;
            var table = ward.Entities;
            ViewBag.HasMore = ward.HasNext;
            ViewBag.CurrentPage = (page ?? 0);
            ViewBag.FilteredIndex = filteredIndex;
            return PartialView(table);
        }
        public ActionResult WardsEntryPanel(string hospitalCode, string hospitalName)
        {
            var wardList = new List<Ward>();
            ViewBag.hospitalCode = hospitalCode.TrimEnd();
            ViewBag.hospitalName = hospitalName.TrimEnd();
            if (TempData["mode"] != null && (string)TempData["mode"] == "AddMode" && TempData["wardList"] == null)
            {
                wardList.Add(new Ward(){Hospital_Code = hospitalCode});
                wardList.Add(new Ward() { Hospital_Code = hospitalCode });
                wardList.Add(new Ward() { Hospital_Code = hospitalCode });
                wardList.Add(new Ward() { Hospital_Code = hospitalCode });
                wardList.Add(new Ward() { Hospital_Code = hospitalCode });
                wardList.Add(new Ward() { Hospital_Code = hospitalCode });
                wardList.Add(new Ward() { Hospital_Code = hospitalCode });
                wardList.Add(new Ward() { Hospital_Code = hospitalCode });
                wardList.Add(new Ward() { Hospital_Code = hospitalCode });
                wardList.Add(new Ward() { Hospital_Code = hospitalCode });
                ViewBag.mode = (string)TempData["mode"];
            }
            else if (TempData["mode"] != null && (string)TempData["mode"] == "AddMode" && TempData["wardList"] != null)
            {
                wardList = (List<Ward>) TempData["wardList"];
                if (wardList.Count < 10)
                {
                    for (var i = 0; i < (10-wardList.Count); i++)
                    {
                        wardList.Add(new Ward());
                    }
                }
                TempData["errorElement"] = TempData["errorElement"];
                TempData["message"] = TempData["message"];
                ViewBag.mode = (string)TempData["mode"];
            }
            else if (TempData["mode"] != null && (string)TempData["mode"] == "ChangeMode")
            {
                wardList = (List<Ward>)TempData["wardList"];
                TempData["errorElement"] = TempData["errorElement"];
                TempData["message"] = TempData["message"];
                ViewBag.mode = (string)TempData["mode"];
            }
            return PartialView(wardList);
        }
        [HttpPost]
        public ActionResult WardsEntryPanel(WardsGdo wardsGdo, string hospitalCode, string hospitalName, FormCollection formCollection)
        {
            var wardGdoList = new List<Ward>();
            //TODO: OPTIMIZATION
            for (var i = 0; i < (new int[] { formCollection["hospital_code"].Split(',').Count(), formCollection["patient_count"].Split(',').Count(), formCollection["ward_code"].Split(',').Count(), formCollection["ward_name"].Split(',').Count() }).Max(); i++)
            {
                var getwardGdo = new Ward()
                                     {
                                         Hospital_Code = 
                                             i > formCollection["hospital_code"].Split(',').Count()
                                                 ? ""
                                                 : formCollection["hospital_code"].Split(',')[i],
                                         Ward_Code = 
                                             i > formCollection["ward_code"].Split(',').Count()
                                                 ? ""
                                                 : formCollection["ward_code"].Split(',')[i],
                                         Ward_Name = 
                                             i > formCollection["ward_name"].Split(',').Count()
                                                 ? ""
                                                 : formCollection["ward_name"].Split(',')[i]
                                     };
                wardGdoList.Add(getwardGdo);
            }
            ViewBag.hospitalCode = hospitalCode.TrimEnd();
            ViewBag.hospitalName = hospitalName.TrimEnd();
            ViewBag.mode = "ChangeMode";
            SetNavInfo("Wards/WardsEntryPanel");
            return PartialView(wardGdoList);
        }

        private WardsPdo getWardsPdo(string hospitalCode)
        {
            var _db = new Ab2edemoEntities(Session["connectionString"].ToString());
            var wardsPdo = new WardsPdo
            {
                Ward = GetPagedWards(0, 10, hospitalCode),
                Country = _db.Hospitals.Find(hospitalCode).Country.TrimEnd(),
                Hospital_Name = _db.Hospitals.Find(hospitalCode).Hospital_Name.TrimEnd(),
                Telephone_Number = Convert.ToDecimal(_db.Hospitals.Find(hospitalCode).Telephone_Number)
            };
            return wardsPdo;
        }

        private PagedItem<WardsGdo> GetPagedWards(int skip, int take, string hospitalCode)
        {
            var _db = new Ab2edemoEntities(Session["connectionString"].ToString());
            var wards = _db.Wards.OrderBy(t => t.Ward_Code);
            var wardsCount = (from ward in wards
                              where (String.Compare(ward.Hospital_Code.TrimEnd(), hospitalCode) == 0)
                              select ward).Count();
            var getWards = (from ward in wards
                            where (String.Compare(ward.Hospital_Code.TrimEnd(), hospitalCode) == 0)
                            select ward).Skip(skip).Take(take).ToList();
            var getWardsGdo = new List<WardsGdo>();
            foreach (var ward in getWards)
            {
                var getWardsGdoItem = new WardsGdo
                                          {
                                              Hospital_Code = ward.Hospital_Code,
                                              Ward_Code = ward.Ward_Code,
                                              Ward_Name = ward.Ward_Name
                                          };
                getWardsGdoItem.Patient_Count = (from patient in _db.Patients.OrderBy(pat => pat.Patient_Code)
                                                 where (patient.Hospital_Code == getWardsGdoItem.Hospital_Code) && (patient.Ward_Code == getWardsGdoItem.Ward_Code)
                                                 select patient).Count();
                getWardsGdo.Add(getWardsGdoItem);
            }
            return new PagedItem<WardsGdo>
            {
                Entities = getWardsGdo,
                HasNext = (skip + 10 < wardsCount),
                HasPrevious = (skip > 0)
            };
        }
        [HttpPost]
        public ActionResult Save(FormCollection formcollection, string hospitalCode, string hospitalName)
        {
            var _db = new Ab2edemoEntities(Session["connectionString"].ToString());
            if (formcollection.AllKeys[0].TrimEnd() == "AddMode")
            {
                TempData["mode"] = "AddMode";
                Session["formcollection"] = formcollection;
                return RedirectToAction("WardsEntryPanel", "Wards", new { hospitalCode, hospitalName });
            }
            if (formcollection.AllKeys[0].TrimEnd()=="Delete")
            {
                var index = new List<int>();
                for (int i = 0; i < formcollection.Count; i++)
                {
                    if (formcollection[i].Split(',')[0].ToString().ToUpper()=="TRUE")
                    {
                        index.Add(i);
                    }
                }
                if (index.Count==0)
                {
                    var wardGdoList = new List<Ward>();
                    //TODO: OPTIMIZATION
                    for (var i = 0; i < (new int[] { formcollection["hospital_code"].Split(',').Count(), formcollection["ward_code"].Split(',').Count(), formcollection["ward_name"].Split(',').Count() }).Max(); i++)
                    {
                        var getwardGdo = new Ward()
                        {
                            Hospital_Code = 
                                i > formcollection["hospital_code"].Split(',').Count()
                                    ? ""
                                    : formcollection["hospital_code"].Split(',')[i],
                            Ward_Code = 
                                i > formcollection["ward_code"].Split(',').Count()
                                    ? ""
                                    : formcollection["ward_code"].Split(',')[i],
                            Ward_Name = 
                                i > formcollection["ward_name"].Split(',').Count()
                                    ? ""
                                    : formcollection["ward_name"].Split(',')[i]
                        };
                        wardGdoList.Add(getwardGdo);
                    }
                    TempData["wardList"] = wardGdoList;
                    TempData["mode"] = "ChangeMode";
                    TempData["postBackError"] = "Please Select a record !!";
                    return RedirectToAction("WardsEntryPanel", "Wards", new { hospitalCode = hospitalCode, hospitalName = hospitalName });
                }
                else
                {
                    foreach (var item in index)
                    {
                        var ward = _db.Wards.Find(formcollection["hospital_code"].Split(',')[0],
                                                  formcollection.AllKeys[item].TrimEnd());
                        _db.Entry(ward).State = EntityState.Deleted;
                        _db.SaveChanges();
                        TempData["programCall"] = true;
                    }
                }
            }
            if (formcollection.AllKeys[0].TrimEnd() == "ChangeMode")
            {
                formcollection = (FormCollection)Session["formcollection"];
                var wardGdoList = new List<Ward>();
                //TODO: OPTIMIZATION
                for (var i = 0; i < (new int[] { formcollection["hospital_code"].Split(',').Count(), formcollection["ward_code"].Split(',').Count(), formcollection["ward_name"].Split(',').Count() }).Max(); i++)
                {
                    var getwardGdo = new Ward()
                    {
                        Hospital_Code = 
                            i > formcollection["hospital_code"].Split(',').Count()
                                ? ""
                                : formcollection["hospital_code"].Split(',')[i],
                        Ward_Code = 
                            i > formcollection["ward_code"].Split(',').Count()
                                ? ""
                                : formcollection["ward_code"].Split(',')[i],
                        Ward_Name = 
                            i > formcollection["ward_name"].Split(',').Count()
                                ? ""
                                : formcollection["ward_name"].Split(',')[i]
                    };
                    wardGdoList.Add(getwardGdo);
                }
                TempData["wardList"] = wardGdoList;
                TempData["errorElement"] ="";
                TempData["message"] = "";
                TempData["mode"]="ChangeMode";
                return RedirectToAction("WardsEntryPanel", "Wards", new {hospitalCode = hospitalCode, hospitalName = hospitalName });
            }
            if (formcollection.AllKeys[0].TrimEnd() == "Submit")
            {
                if (formcollection["CurrentMode"] == "AddMode")
                {
                    //ToDo: Code for Add Mode Screen Submit
                    var wardList = new List<Ward>();
                    for (int i = 0; i < formcollection["ward_code"].Split(',').Count(); i++)
                    {
                        var wardCode = (formcollection["ward_code"].Split(','))[i];
                        if (wardCode==String.Empty)continue;
                        var hospiName = (formcollection["hospital_code"].Split(','))[i];
                        var wardName = (formcollection["ward_name"].Split(','))[i];
                        var dbWard = _db.Wards.Find(hospiName, wardCode);
                        
                        if (dbWard !=null && wardName == dbWard.Ward_Name.TrimEnd()) continue;
                        ValidateModel(new Ward() { Hospital_Code = hospiName, Ward_Code = wardCode, Ward_Name = wardName }, "AddMode");

                        if (ModelState.IsValid)
                        {
                            var waardI = new Ward() { Hospital_Code = hospiName, Ward_Code = wardCode, Ward_Name = wardName };
                            wardList.Add(waardI);
                        }
                        else if (!ModelState.IsValid)
                        {
                            var wardlist = new List<Ward>();
                            for (var j = 0; j < formcollection["ward_code"].Split(',').Count(); j++)
                            {
                                wardlist.Add(new Ward() { Hospital_Code = (formcollection["hospital_code"].Split(','))[j], Ward_Code = (formcollection["ward_code"].Split(','))[j], Ward_Name = (formcollection["ward_name"].Split(','))[j] });
                            }
                            TempData["wardList"] = wardlist;
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
                            TempData["mode"] = "AddMode";
                            TempData["errorElement"] = ErrorField;
                            TempData["message"] = ErrorMessage;
                            return RedirectToAction("WardsEntryPanel", "Wards", new { hospitalCode, hospitalName });
                        }
                    }
                    foreach (var ward in wardList)
                    {
                        _db.Entry(ward).State = EntityState.Added;
                        _db.SaveChanges();
                        TempData["programCall"] = true;
                    }
                }
                if (formcollection["CurrentMode"] == "ChangeMode")
                {
                    var wardList = new List<Ward>();
                    for (int i = 0; i < formcollection["ward_code"].Split(',').Count(); i++)
                    {
                        var hospiName = (formcollection["hospital_code"].Split(','))[i].TrimEnd();
                        var wardCode = (formcollection["ward_code"].Split(','))[i].TrimEnd();
                        var wardName = (formcollection["ward_name"].Split(','))[i].TrimEnd();
                        var dbWardName = _db.Wards.Find(hospiName, wardCode).Ward_Name.TrimEnd();
                        if (wardName == dbWardName) continue;
                        ValidateModel(new Ward() { Hospital_Code = hospiName, Ward_Code = wardCode, Ward_Name = wardName }, "ChangeMode");

                        if (ModelState.IsValid)
                        {
                            var waardI = new Ward() { Hospital_Code = hospiName, Ward_Code = wardCode, Ward_Name = wardName };
                            wardList.Add(waardI);
                        }
                        else if (!ModelState.IsValid)
                        {
                            var wardlist = new List<Ward>();
                            for (var j = 0; j < (new int[] { formcollection["hospital_code"].Split(',').Count(), formcollection["ward_code"].Split(',').Count(), formcollection["ward_name"].Split(',').Count() }).Max(); j++)
                            {
                                wardlist.Add(new Ward() { Hospital_Code = (formcollection["hospital_code"].Split(','))[j], Ward_Code = (formcollection["ward_code"].Split(','))[j], Ward_Name = (formcollection["ward_name"].Split(','))[j] });
                            }
                            TempData["wardList"] = wardlist;
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
                            TempData["mode"] = "ChangeMode";
                            TempData["errorElement"] = ErrorField;
                            TempData["message"] = ErrorMessage;
                            return RedirectToAction("WardsEntryPanel", "Wards", new { hospitalCode, hospitalName });
                        }
                    }
                    foreach (var ward in wardList)
                    {
                        _db.Entry(ward).State = EntityState.Modified;
                        _db.SaveChanges();
                        TempData["programCall"] = true;
                    }
                }
            }
            var navInfo = (Stack<NameValueCollection>)Session["navigationHistory"];
            navInfo.Pop();
            navInfo.Pop();
            Session["navigationHistory"] = navInfo;
            return RedirectToAction("WardsGrid", "Wards", new { hospitalCode = hospitalCode });
        }

        public ActionResult WardsPanel()
        {
            ViewBag.hospitalCode = ViewBag.hospitalCode ?? "";
            ViewBag.hospitalName = ViewBag.hospitalName ?? "";
            var wardsAddMode = new List<Ward>();
            for (int i = 0; i < 10; i++)
            {
                wardsAddMode.Add(new Ward());
            }
            return PartialView(wardsAddMode);
        }
        /// <summary>
        /// Method to validate the model(Purchases)
        /// </summary>
        /// <param name="ward"></param>
        /// <param name="mode"></param>
        [AcceptVerbs(HttpVerbs.Post)]

        public void ValidateModel(Ward ward, String mode)
        {
            var _db = new Ab2edemoEntities(Session["connectionString"].ToString());
            if (UtilsController.MessagesCol == null)
                UtilsController.LoadMessages();
            var errMsg = new string[] { };
            if (ward.Ward_Name == null || ward.Ward_Name.Trim() == "")
            {
                if (UtilsController.MessagesCol != null) errMsg = UtilsController.MessagesCol.GetValues("USR0071");
                if (errMsg != null) ModelState.AddModelError("ward_name", errMsg[0]);
            }
            if (mode == "AddMode")
            {
                if (_db.Wards.Find(ward.Hospital_Code, ward.Hospital_Code) != null)
                {
                    if (UtilsController.MessagesCol != null) errMsg = UtilsController.MessagesCol.GetValues("USR0072");
                    if (errMsg != null) ModelState.AddModelError("ward_code", errMsg[0]);
                }
            }

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
