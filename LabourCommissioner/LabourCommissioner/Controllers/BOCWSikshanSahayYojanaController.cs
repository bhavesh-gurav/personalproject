using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using LabourCommissioner.Abstraction.DataModels;
using LabourCommissioner.Abstraction.Services;
using LabourCommissioner.Abstraction.ViewDataModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using LabourCommissioner.Abstraction;
using LabourCommissioner.Common;
using LabourCommissioner.Common.Utility;
using Newtonsoft.Json.Linq;
using static LabourCommissioner.Abstraction.ViewDataModels.DocumentDetails;
using System.Data;

namespace LabourCommissioner.Controllers
{

    //TEST@
    public class BOCWSikshanSahayYojanaController : Controller
    {
        private readonly IBOCWSikshanSahayYojanaService _iBOCWSikshanSahayYojanaService;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ClaimsPrincipal _claimPincipal;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public BOCWSikshanSahayYojanaController(IBOCWSikshanSahayYojanaService iBOCWSikshanSahayYojanaService, IConfiguration config, IHttpClientFactory clientFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _iBOCWSikshanSahayYojanaService = iBOCWSikshanSahayYojanaService ?? throw new ArgumentNullException(nameof(_iBOCWSikshanSahayYojanaService));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));

            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _claimPincipal = _httpContextAccessor.HttpContext.User ?? throw new ArgumentNullException(nameof(_httpContextAccessor.HttpContext.User));
        }

        public IActionResult AppPersonalDetails(int id, int TabId = 0, int ApplicationId = 0)
        {
            Task<List<TabModel>> model = _iBOCWSikshanSahayYojanaService.GetServiceTabByServiceId(id);
            ViewBag.AppPersonalDetails = model.Result.ToList();
            Task<TabModel> tab = _iBOCWSikshanSahayYojanaService.GetTabSequenceByApplicationId(ApplicationId);
            if (tab.Result == null)
            {
                ViewBag.TabSequenceNo = TabId + 1;

            }
            else
            {
                ViewBag.TabSequenceNo = Convert.ToInt32(tab.Result.SequenceNo) + 1;
                ViewBag.isFilled = tab.Result.isfilled;
            }
            ViewBag.ApplicationId = ApplicationId;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PersonalDetails(string ServiceId, long ApplicationId, bool isFilled)
        {

            string RegistrationNo = User.FindFirst(ClaimTypes.Name).Value;



            PersonalDetailsModel model = await _iBOCWSikshanSahayYojanaService.GetPersonalDetailsByRegNo(RegistrationNo);
            PersonalDetailsModel model1 = await _iBOCWSikshanSahayYojanaService.GetApplicationDetailsByAppId(ApplicationId);            
            model.ApplicationNo = User.FindFirst(ClaimTypes.Name).Value;
            ViewBag.ServiceId = ServiceId;
            ViewBag.TabSequenceNo = 1;
            Task<List<System.Web.Mvc.SelectListItem>> stateModel = _iBOCWSikshanSahayYojanaService.GetAllStates();
            var states = stateModel.Result.ToList();
            ViewBag.States = states;
            ViewBag.isFilled = isFilled;
            Task<IEnumerable<System.Web.Mvc.SelectListItem>> districtModel = _iBOCWSikshanSahayYojanaService.GetDistrict();
            var district = districtModel.Result.ToList();
            ViewBag.District = district;            
            ViewBag.ApplicationId = ApplicationId;
            if (isFilled == true)
            {
                Task<IEnumerable<System.Web.Mvc.SelectListItem>> PtalukaModel = _iBOCWSikshanSahayYojanaService.GetTalukaByDistrictId(model1.PDistrictId);
                var Ptaluka = PtalukaModel.Result.ToList();

                Task<IEnumerable<System.Web.Mvc.SelectListItem>> PvillageModel = _iBOCWSikshanSahayYojanaService.GetVillageByDistrictIdAndTalukaId(model1.PDistrictId, model1.PTalukaId);
                var Pvillage = PvillageModel.Result.ToList();

                Task<IEnumerable<System.Web.Mvc.SelectListItem>> talukaModel = _iBOCWSikshanSahayYojanaService.GetTalukaByDistrictId(model1.CDistrictId);
                var Ctaluka = talukaModel.Result.ToList();

                Task<IEnumerable<System.Web.Mvc.SelectListItem>> villageModel = _iBOCWSikshanSahayYojanaService.GetVillageByDistrictIdAndTalukaId(model1.CDistrictId, model1.CTalukaId);
                var Cvillage = villageModel.Result.ToList();

                ViewBag.CTaluka = Ctaluka;
                ViewBag.CVillage = Cvillage;
                ViewBag.PTaluka = Ptaluka;
                ViewBag.PVillage = Pvillage;
                return PartialView("BOCWSikshanSahayYojana/_PersonalDetails", model1);
            }
            else
            {
                return PartialView("BOCWSikshanSahayYojana/_PersonalDetails", model);
            }




            //return PartialView("BOCWSikshanSahayYojana/_PersonalDetails", model);

        }

        [HttpGet]
        public IActionResult BOCWSikshanSahayYojanaSchemeDetails(string ServiceId, string ApplicationId, bool isFilled)
        {
            Task<IEnumerable<System.Web.Mvc.SelectListItem>> educationModel = _iBOCWSikshanSahayYojanaService.GetEducation(nameof(EnumLookup.ResourcesType.Education));
            var education = educationModel.Result.ToList();
            ViewBag.Syllabus = education;

            Task<IEnumerable<System.Web.Mvc.SelectListItem>> degreeModel = _iBOCWSikshanSahayYojanaService.GetEducation(nameof(EnumLookup.ResourcesType.Degree));
            var degree = degreeModel.Result.ToList();
            ViewBag.Degree = degree;

            Task<IEnumerable<System.Web.Mvc.SelectListItem>> semesterModel = _iBOCWSikshanSahayYojanaService.GetEducation(nameof(EnumLookup.ResourcesType.Semester));
            var semester = semesterModel.Result.ToList();
            ViewBag.Semester = semester;
            ViewBag.ServiceId = ServiceId;
            ViewBag.ApplicationId = ApplicationId;
            ViewBag.TabSequenceNo = 2;
            // ViewBag.ApplicationId = ApplicationId;

            return PartialView("BOCWSikshanSahayYojana/_BOCWSikshanSahayYojanaSchemeDetails");
        }

        [HttpGet]
        public async Task<IActionResult> BOCWSikshanSahayYojanaDocument(string ServiceId, string ApplicationId)
        {
            ViewBag.ServiceId = ServiceId;
            ViewBag.ApplicationId = ApplicationId;
            ViewBag.TabSequenceNo = 3;
            IList<DocumentFileDetails> model = await _iBOCWSikshanSahayYojanaService.GetDocumentFileDetails(Convert.ToInt32(ServiceId));
            return PartialView("BOCWSikshanSahayYojana/_BOCWSikshanSahayYojanaDocument", model);

            //IEnumerable<DocumentDetails> model = await _iBOCWSikshanSahayYojanaService.GetFileDocuments(Convert.ToInt32(ServiceId));
            //return PartialView("BOCWSikshanSahayYojana/_BOCWSikshanSahayYojanaDocument", model);

            //DocumentDetails model = await _iBOCWSikshanSahayYojanaService.GetDocumentsDetails(Convert.ToInt32(ServiceId));
            // return PartialView("BOCWSikshanSahayYojana/_BOCWSikshanSahayYojanaDocument", model);
        }
        [HttpGet]
        public IActionResult TermsCondition(string ServiceId, string ApplicationId)
        {
            ViewBag.ServiceId = ServiceId;
            ViewBag.ApplicationId = ApplicationId;
            ViewBag.TabSequenceNo = 4;
            return PartialView("BOCWSikshanSahayYojana/_TermsCondition");
        }

        //[HttpPost]

        [HttpGet]
        public IActionResult GetDistrict()
        {
            Task<IEnumerable<System.Web.Mvc.SelectListItem>> regions = _iBOCWSikshanSahayYojanaService.GetDistrict();
            //return Json(regions, System.Web.Mvc.JsonRequestBehavior.AllowGet);
            return Json(new { data = regions });
        }
        [HttpGet]
        public IActionResult GetSubject(int subjectId)
        {
            Task<IEnumerable<System.Web.Mvc.SelectListItem>> subject = _iBOCWSikshanSahayYojanaService.GetSubject(subjectId);
            return Json(new { data = subject });
        }

        [HttpGet]
        public IActionResult GetTalukaByDistrictId(int districtId)
        {
            Task<IEnumerable<System.Web.Mvc.SelectListItem>> regions = _iBOCWSikshanSahayYojanaService.GetTalukaByDistrictId(districtId);
            //return Json(regions, System.Web.Mvc.JsonRequestBehavior.AllowGet);
            return Json(new { data = regions });
        }
        [HttpGet]
        public IActionResult GetVillageByDistrictIdAndTalukaId(int districtId, int talukaId)
        {
            Task<IEnumerable<System.Web.Mvc.SelectListItem>> regions = _iBOCWSikshanSahayYojanaService.GetVillageByDistrictIdAndTalukaId(districtId, talukaId);
            //return Json(regions, System.Web.Mvc.JsonRequestBehavior.AllowGet);
            return Json(new { data = regions });

        }

        [HttpGet]
        public ActionResult GetEducation()
        {
            Task<IEnumerable<System.Web.Mvc.SelectListItem>> regions = _iBOCWSikshanSahayYojanaService.GetEducation(nameof(EnumLookup.ResourcesType.Education));
            return Json(regions, System.Web.Mvc.JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[PermissionRequirement(PermissionConstant.IsInsert, PermissionConstant.IsUpdate)]
        public async Task<IActionResult> AddUpdateApplication(PersonalDetailsModel personalDetails, int TabSequenceNo, string ApplicationNo)
        {
            personalDetails.IpAddress = CommonUtils.GetLocalIPAddress();
            personalDetails.HostName = CommonUtils.GetHostName();
            personalDetails.RegistrationNo = User.FindFirst(ClaimTypes.Name).Value;
            personalDetails.RegistrationId = Convert.ToInt32(_claimPincipal.FindFirstValue("RegistrationId"));
            var extension = Path.GetExtension(personalDetails.Photo.FileName);
            personalDetails.FileName = personalDetails.RegistrationId + "_" + DateTime.Now.ToString("yyyyMMDDhhmmss") + Convert.ToString(extension);
            personalDetails.CreatedDate = DateTime.Now;
            personalDetails.IsDeleted = false;
            personalDetails.ApplicationNo = ApplicationNo;
            personalDetails.CreatedBy = Convert.ToInt32(_claimPincipal.FindFirstValue("RegistrationId"));
            personalDetails.AadharCardNo = CommonUtils.EncryptCRY(personalDetails.AadharCardNo);

            #region Upload Profile Photo to CouchDB
            CouchDBRequest couchDBRequest = new CouchDBRequest();
            CouchDBResponse couchDBResponse = new CouchDBResponse();

            if (personalDetails.Photo != null && personalDetails.Photo.FileName != "" && personalDetails.Photo.Length > 0)
            {
                //Insert new attachment

                byte[] fileBytes = null;
                using (MemoryStream msstream = new MemoryStream())
                {
                    await personalDetails.Photo.CopyToAsync(msstream);
                    fileBytes = msstream.ToArray();

                }
                couchDBRequest.FileName = personalDetails.Photo.FileName;
                couchDBRequest.AttachmentData = fileBytes;
                couchDBRequest.FileExtension = System.IO.Path.GetExtension(personalDetails.Photo.FileName).Replace(".", "").ToUpper(); ;
                couchDBRequest.CreatedOn = DateTime.Now.ToString();
                couchDBResponse = await new CommonUtils(_config, _clientFactory).UplodToCouchDB(couchDBRequest);
                if (couchDBResponse != null && couchDBResponse.IsSuccess)
                {
                    personalDetails.CouchDBDocId = couchDBResponse.Id;
                    personalDetails.CouchDBDocRevId = couchDBResponse.Rev;
                }
                else
                {
                    TempData["Message"] = CommonUtils.ConcatString("Somthing went wrong please try after sometime.", Convert.ToString((int)EnumLookup.ResponseMsgType.error), "||");
                    return RedirectToAction("AppPersonalDetails", "BOCWSikshanSahayYojana", personalDetails);
                }
            }


            #endregion


            ResponseMessage regResponse = await _iBOCWSikshanSahayYojanaService.AddUpdateApplication(personalDetails);
            if (regResponse != null)
            {
                //string errorMsg = regResponse.ResponseMsg == null ? "Somthing went wrong please try again." : regResponse.ResponseMsg;
                if (regResponse != null && regResponse != null && regResponse.Error == 0)
                {
                    //TempData["Message"] = CommonUtils.ConcatString(errorMsg, Convert.ToString((int)EnumLookup.ResponseMsgType.info), "||");
                    ModelState.Clear();
                    PersonalDetailsModel empEmpty = new PersonalDetailsModel();
                    //return RedirectToAction("BOCWSikshanSahayYojanaSchemeDetails", "BOCWSikshanSahayYojana");
                    return RedirectToAction("AppPersonalDetails", new { id = personalDetails.ServiceId, TabId = 1, ApplicationId = regResponse.Id });
                }
                else
                {
                    //TempData["Message"] = CommonUtils.ConcatString(errorMsg, Convert.ToString((int)EnumLookup.ResponseMsgType.warning), "||");
                    ModelState.Clear();
                    Registration empEmpty = new Registration();
                    return RedirectToAction("AppPersonalDetails", "BOCWSikshanSahayYojana");
                }
            }
            else
            {
                TempData["Message"] = CommonUtils.ConcatString("Somthing went wrong please try after sometime.", Convert.ToString((int)EnumLookup.ResponseMsgType.error), "||");
                return RedirectToAction("AppPersonalDetails", "BOCWSikshanSahayYojana", personalDetails);
            }

            //return RedirectToAction("AppPersonalDetails", "BOCWSikshanSahayYojana", personalDetails);

            return RedirectToAction("BOCWSikshanSahayYojanaSchemeDetails", "BOCWSikshanSahayYojana");
        }
        public async Task<IActionResult> AddSchemeDetails(SchemeDetails schemeDetails, int TabSequenceNo, int ApplicationId)
        {
            schemeDetails.ApplicationId = ApplicationId;
            schemeDetails.IpAddress = CommonUtils.GetLocalIPAddress();
            schemeDetails.HostName = CommonUtils.GetHostName();
            schemeDetails.CreatedBy = Convert.ToInt32(_claimPincipal.FindFirstValue("RegistrationId"));
            schemeDetails.TabSequenceNo = TabSequenceNo;
            // schemeDetails.UserId = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            ResponseMessage regResponse = await _iBOCWSikshanSahayYojanaService.AddSchemeDetails(schemeDetails);
            return RedirectToAction("AppPersonalDetails", new { id = schemeDetails.ServiceId, TabId = 2, ApplicationId = regResponse.Id });
        }
        public async Task<IActionResult> UploadDocument(IList<DocumentFileDetails> documentFileDetails, int TabSequenceNo, int ApplicationId)
        {

            //if(ModelState.Is)
            //return RedirectToAction("AppPersonalDetails", new { id = 2, TabId = 4 });
            //if (ModelState.IsValid)
            //{
            if (documentFileDetails != null && documentFileDetails.Count() > 0)
            {

                List<DocumentFileDetails> lstdocumentFileDetails = new List<DocumentFileDetails>();
                int RegistrationId = Convert.ToInt32(_claimPincipal.FindFirstValue("RegistrationId"));
                foreach (var item in documentFileDetails)
                {
                    DocumentFileDetails objdocumentFileDetails = new DocumentFileDetails();
                    CouchDBRequest couchDBRequest = new CouchDBRequest();
                    CouchDBResponse couchDBResponse = new CouchDBResponse();

                    if (item.IdImage != null && item.IdImage.File != null && item.IdImage.File.FileName != "" && item.IdImage.File.Length > 0)
                    {
                        //Insert new attachment

                        byte[] fileBytes = null;
                        using (MemoryStream msstream = new MemoryStream())
                        {
                            await item.IdImage.File.CopyToAsync(msstream);
                            fileBytes = msstream.ToArray();

                        }
                        couchDBRequest.FileName = item.IdImage.File.FileName;
                        couchDBRequest.AttachmentData = fileBytes;
                        couchDBRequest.FileExtension = System.IO.Path.GetExtension(item.IdImage.File.FileName).Replace(".", "").ToUpper(); ;
                        couchDBRequest.CreatedOn = DateTime.Now.ToString();
                        couchDBResponse = await new CommonUtils(_config, _clientFactory).UplodToCouchDB(couchDBRequest);
                        if (couchDBResponse != null && couchDBResponse.IsSuccess)
                        {
                            objdocumentFileDetails.ApplicationId = ApplicationId;
                            objdocumentFileDetails.DocumentMasterId = item.DocumentMasterId;
                            objdocumentFileDetails.DocumentName = item.DocumentName;
                            objdocumentFileDetails.DocumentNumber = item.DocumentNumber;
                            objdocumentFileDetails.CouchDBDocId = couchDBResponse.Id;
                            objdocumentFileDetails.ServiceId = item.ServiceId;
                            objdocumentFileDetails.CreatedBy = RegistrationId;
                            objdocumentFileDetails.IpAddress = CommonUtils.GetLocalIPAddress();
                            objdocumentFileDetails.HostName = CommonUtils.GetHostName();
                            objdocumentFileDetails.TabSequenceNo = item.TabSequenceNo;
                            lstdocumentFileDetails.Add(objdocumentFileDetails);
                        }
                        else
                        {
                            return PartialView("BOCWSikshanSahayYojana/_BOCWSikshanSahayYojanaDocument");
                        }
                    }
                }
                DataTable dtData = CommonUtils.ToDataTable(lstdocumentFileDetails);
                dtData.Columns.Remove("DocumentNameGuj");
                dtData.Columns.Remove("ServiceDocumentType");
                dtData.Columns.Remove("DocumentTypeIds");
                dtData.Columns.Remove("IsCompulsary");
                dtData.Columns.Remove("IsNumberInput");
                dtData.Columns.Remove("IsActive");
                dtData.Columns.Remove("IdImage");

                ResponseMessage model = await _iBOCWSikshanSahayYojanaService.AddUpdateDocumentDetailsNew(dtData);
                //ResponseMessage model = await _iBOCWSikshanSahayYojanaService.AddUpdateDocumentDetails(lstdocumentFileDetails);
                return RedirectToAction("AppPersonalDetails", new { id = model.Id, TabId = 3, ApplicationId = model.ApplicationNo });
            }
            //}
            //else
            //{
            //    IList<DocumentFileDetails> model = await _iBOCWSikshanSahayYojanaService.GetDocumentFileDetails(Convert.ToInt32(2));
            //    return RedirectToAction("AppPersonalDetails", new { id = 2, TabId = 4 });
            //}

            #region OLD
            //if (documentDetails.FormFile != null && documentDetails.FormFile.Count() > 0)
            //{
            //    DocumentDetails objdocumentDetails = new DocumentDetails();
            //    List<DocumentDetails> lstdocumentDetails = new List<DocumentDetails>();
            //    foreach (IFormFile item in documentDetails.FormFile)
            //    {

            //        CouchDBRequest couchDBRequest = new CouchDBRequest();
            //        CouchDBResponse couchDBResponse = new CouchDBResponse();

            //        if (item.FileName != null && item.FileName != "" && item.Length > 0)
            //        {
            //            // Insert new attachment  

            //            byte[] fileBytes = null;
            //            using (MemoryStream msstream = new MemoryStream())
            //            {
            //                await item.CopyToAsync(msstream);
            //                fileBytes = msstream.ToArray();

            //            }
            //            couchDBRequest.FileName = item.FileName;
            //            couchDBRequest.AttachmentData = fileBytes;
            //            couchDBRequest.FileExtension = System.IO.Path.GetExtension(item.FileName).Replace(".", "").ToUpper(); ;
            //            couchDBRequest.CreatedOn = DateTime.Now.ToString();
            //            //CommonUtils commonUtils = new CommonUtils(_config);
            //            couchDBResponse = await new CommonUtils(_config, _clientFactory).UplodToCouchDB(couchDBRequest);
            //            if (couchDBResponse != null && couchDBResponse.IsSuccess)
            //            {
            //                objdocumentDetails.DocumentMasterId = documentDetails.DocumentMasterId;
            //                objdocumentDetails.DocumentName = item.FileName;
            //                objdocumentDetails.DocumentNumber = documentDetails.DocumentNumber;
            //                objdocumentDetails.DocumentMasterId = documentDetails.DocumentMasterId;
            //                objdocumentDetails.DocumentMasterId = documentDetails.DocumentMasterId;
            //                //lstcouchDBResponse.Add(couchDBResponse);
            //            }
            //            else
            //            {
            //                return PartialView("BOCWSikshanSahayYojana/_BOCWSikshanSahayYojanaDocument");
            //            }
            //        }
            //        else
            //        {
            //            //// Update existing attachment  
            //            //var docData = await new CommonUtils(_config, _clientFactory).GetAttachmentByteArray("1", Convert.ToString(item.FileName));
            //            //couchDBRequest.AttachmentData = docData.Result;
            //            //couchDBRequest.CreatedOn = DateTime.Now.ToString();
            //            //var result = await new CommonUtils(_config, _clientFactory).UpdateAttachment(couchDBRequest);
            //            //if (result.IsSuccess)
            //            //{
            //            //    return RedirectToAction("Index");
            //            //}
            //        }
            //    }
            //    //if (lstcouchDBResponse != null && lstcouchDBResponse.Count() > 0)
            //    //{
            //    //    List<DocumentDetails> model = await _iBOCWSikshanSahayYojanaService.SaveDocumentDetails(lstcouchDBResponse);
            //    //    return PartialView("BOCWSikshanSahayYojana/_BOCWSikshanSahayYojanaDocument", model);
            //    //}


            //    return View();
            //}

            #endregion

            //return View();
            return RedirectToAction("AppPersonalDetails", "BOCWSikshanSahayYojana", documentFileDetails);
        }
        [HttpPost]
        public async Task<IActionResult> FinalSubmit(FinalSubmitModel finalSubmitModel)
        {

            finalSubmitModel.ipaddress = CommonUtils.GetLocalIPAddress();
            finalSubmitModel.hostname = CommonUtils.GetHostName();
            finalSubmitModel.ResigtrationId = Convert.ToInt32(_claimPincipal.FindFirstValue("RegistrationId"));
            finalSubmitModel.tablename = "bocw_ssy.personaldetails";
            finalSubmitModel.schemaname = "bocw_ssy";
            //  finalSubmitModel.userid = Convert.ToInt32(_claimPincipal.FindFirstValue("RegistrationId"));
            finalSubmitModel.userid = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            finalSubmitModel.benificiarytype = Convert.ToInt32(_claimPincipal.FindFirstValue("BeneficiaryType"));
            finalSubmitModel.greensoldierreferralcode = "";

            ResponseMessage regResponse = await _iBOCWSikshanSahayYojanaService.FinalSubmit(finalSubmitModel);
            if (regResponse != null)
            {
                if (regResponse != null && regResponse.Error == 0)
                {
                    string msg = "Your Form Is Submitted SuccessFully..!!";
                    TempData["Message"] = CommonUtils.ConcatString(msg, Convert.ToString((int)EnumLookup.ResponseMsgType.success), "||");
                    return RedirectToAction("ApplicationDetails", "Home", new { ApplicationId = regResponse.Id });
                }
            }
            string errorMsg = "Form Subbmission Failed..!!";
            //  TempData["Message"] = CommonUtils.ConcatString(errorMsg, Convert.ToString((int)EnumLookup.ResponseMsgType.error), "||");

            return RedirectToAction("TermsCondition", "BOCWSikshanSahayYojana");

        }


        //[HttpGet]
        //public async Task<IActionResult> SchemeUsers(SchemeUserModel schemeUserModel)
        //{
        //    List<SchemeUserModel> schemeUserModel1 = await _schemeUserServices.GetSchemeUser(schemeUserModel);
        //    return View(schemeUserModel1);
        //}
    }
}