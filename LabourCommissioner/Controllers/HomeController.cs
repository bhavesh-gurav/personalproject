using LabourCommissioner.Abstraction.DataModels;
using LabourCommissioner.Abstraction.Repositories;
using LabourCommissioner.Abstraction.Services;
using LabourCommissioner.Common.CustomAuthentication;
using LabourCommissioner.Common.Utility;
using LabourCommissioner.Models;
//using LabourCommissioner.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using LabourCommissioner.Abstraction.ViewDataModels;

namespace LabourCommissioner.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeService _ihomeService;
        private readonly IHomeRepository _homeRepository;
        private readonly ISchemeService _iscchemeService;
        private readonly ISchemeUserServices _schemeUserServices;
        public HomeController(IHomeService homeService, IHomeRepository homeRepository, ISchemeService schemeService, ISchemeUserServices schemeUserServices)

        {
            _ihomeService = homeService ?? throw new ArgumentNullException(nameof(homeService));
            _homeRepository = homeRepository;
            _iscchemeService = schemeService ?? throw new ArgumentNullException(nameof(schemeService));
            _schemeUserServices = schemeUserServices;
        }

        public IActionResult Index()
        {
            ViewBag.showLoginButtons = true;
            ViewBag.hideHomeButton = true;
            ViewBag.showSchemeMenu = true;
            ViewBag.hideAppStatus = true;
            return View();
        }
        public async Task<IActionResult> GLWBScheme(string ServiceId = "2")
        {
            var model = await _ihomeService.GetSchemeByServiceId(Convert.ToInt32(ServiceId));
           // IEnumerable<ServiceMaster> model = (IEnumerable<ServiceMaster>)await _homeRepository.GetServicesByHodId(HodId);
            return View(model);

        }
        public IActionResult GLWBSchemeDetails()
        {
            return View();
        }
        public IActionResult Dashboard()
        {
            return View();
        }
        public IActionResult Form()
        {
            return View();
        }
        public IActionResult Grid()
        {
            return View();
        }
        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult Form2()
        {
            return View();
        }
        public IActionResult Form3()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Registration()
        {
            var vm = new Registration()
            {
                //    Gender = new List<Gender>
                //    {
                //        new Gender {Value = 1, Text = "Male"},
                //        new Gender {Value = 2, Text = "Female"}
                //}
            };


            return View(vm);
            //return View();
        }
        [HttpPost]
        public IActionResult Registration(Registration registration)
        {
            return View(registration);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //[PermissionRequirement(PermissionConstant.IsView)]
        //[Route("View/{id?}")]
        public async Task<IActionResult> Home()
        {
            IEnumerable<ServiceMaster> model = await _ihomeService.BindServicesUserWiseFilter();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetSchemeDescription(string ServiceId)
        {
            var model = await _ihomeService.GetSchemeByServiceId(Convert.ToInt32(ServiceId));
            return PartialView("_SchemeDescription", model);
        }

        [HttpGet]
        public async Task<IActionResult> SchemeUsers(SchemeUserModel schemeUserModel)
        {
            List<SchemeUserModel> schemeUserModel1 = await _schemeUserServices.GetSchemeUser(schemeUserModel);
            return View(schemeUserModel1);
        }

        
        //public async Task<IActionResult> ForgetPasswordReset(int UserId)
        //{
        //    return View();
        //}

        public async Task<IActionResult> ForgetPasswordReset(ForgetPassword uForgetPassword)
        {

            return View(uForgetPassword);
        }

    }
}