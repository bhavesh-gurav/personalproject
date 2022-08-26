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
            ViewBag.TabSequenceNo = (TabId + 1);
            ViewBag.ApplicationId = ApplicationId;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PersonalDetails(string ServiceId)
        {

            string RegistrationNo = User.FindFirst(ClaimTypes.Name).Value;

            PersonalDetailsModel model = await _iBOCWSikshanSahayYojanaService.GetPersonalDetailsByRegNo(RegistrationNo);

            Task<List<System.Web.Mvc.SelectListItem>> stateModel = _iBOCWSikshanSahayYojanaService.GetAllStates();
            var states = stateModel.Result.ToList();
            ViewBag.States = states;
            ViewBag.ServiceId = ServiceId;
            ViewBag.TabSequenceNo = 1;

            return PartialView("BOCWSikshanSahayYojana/_PersonalDetails", model);
        }

        [HttpGet]
        public IActionResult BOCWSikshanSahayYojanaSchemeDetails()
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

           // ViewBag.ApplicationId = ApplicationId;

            return PartialView("BOCWSikshanSahayYojana/_BOCWSikshanSahayYojanaSchemeDetails");
        }

        [HttpGet]
        public async Task<IActionResult> BOCWSikshanSahayYojanaDocument(string ServiceId)
        {

            IList<DocumentFileDetails> model = await _iBOCWSikshanSahayYojanaService.GetDocumentFileDetails(Convert.ToInt32(ServiceId));
            return PartialView("BOCWSikshanSahayYojana/_BOCWSikshanSahayYojanaDocument", model);

            //IEnumerable<DocumentDetails> model = await _iBOCWSikshanSahayYojanaService.GetFileDocuments(Convert.ToInt32(ServiceId));
            //return PartialView("BOCWSikshanSahayYojana/_BOCWSikshanSahayYojanaDocument", model);

            //DocumentDetails model = await _iBOCWSikshanSahayYojanaService.GetDocumentsDetails(Convert.ToInt32(ServiceId));
            // return PartialView("BOCWSikshanSahayYojana/_BOCWSikshanSahayYojanaDocument", model);
        }
        [HttpGet]
        public IActionResult TermsCondition()
        {
            return PartialView("BOCWSikshanSahayYojana/_TermsCondition");
        }

        [HttpPost]
        public async Task<IActionResult> UploadDocument(IList<DocumentFileDetails> documentFileDetails)
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
                            objdocumentFileDetails.DocumentMasterId = item.DocumentMasterId;
                            objdocumentFileDetails.DocumentName = item.DocumentName;
                            objdocumentFileDetails.DocumentNumber = item.DocumentNumber;
                            objdocumentFileDetails.CouchDBDocId = couchDBResponse.Id;
                            objdocumentFileDetails.CreatedBy = RegistrationId;
                            lstdocumentFileDetails.Add(objdocumentFileDetails);
                        }
                        else
                        {
                            return PartialView("BOCWSikshanSahayYojana/_BOCWSikshanSahayYojanaDocument");
                        }
                    }
                }


                DataTable dtData = CommonUtils.ToDataTable(lstdocumentFileDetails);
                ResponseMessage model = await _iBOCWSikshanSahayYojanaService.AddUpdateDocumentDetailsNew(dtData);

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

            return View();
        }
        [HttpGet]
        public IActionResult GetDistrict()
        {
            Task<IEnumerable<System.Web.Mvc.SelectListItem>> regions = _iBOCWSikshanSahayYojanaService.GetDistrict();
            //return Json(regions, System.Web.Mvc.JsonRequestBehavior.AllowGet);
            return Json(new { data = regions });
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
        public async Task<IActionResult> AddUpdateApplication(PersonalDetailsModel personalDetails, int TabSequenceNo)
        {
            personalDetails.IpAddress = CommonUtils.GetLocalIPAddress();
            personalDetails.HostName = CommonUtils.GetHostName();
            personalDetails.RegistrationNo = User.FindFirst(ClaimTypes.Name).Value;
            personalDetails.RegistrationId = Convert.ToInt32(_claimPincipal.FindFirstValue("RegistrationId"));
            personalDetails.CreatedDate = DateTime.Now;
            personalDetails.IsDeleted = false;
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

        //[HttpGet]
        //public async Task<IActionResult> SchemeUsers(SchemeUserModel schemeUserModel)
        //{
        //    List<SchemeUserModel> schemeUserModel1 = await _schemeUserServices.GetSchemeUser(schemeUserModel);
        //    return View(schemeUserModel1);
        //}

        public async Task<IActionResult> AddSchemeDetails(SchemeDetails schemeDetails, int TabSequenceNo, int ApplicationId)
        {
            schemeDetails.ApplicationId = ApplicationId;
            ResponseMessage regResponse = await _iBOCWSikshanSahayYojanaService.AddSchemeDetails(schemeDetails);
            return RedirectToAction("AppPersonalDetails", new { id = schemeDetails.ServiceId, TabId = 1 });
        }

    }
}