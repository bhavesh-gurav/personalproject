﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LabourCommissioner.Controllers
{
    public class GLWBCycleYojnaController : Controller
    {
        public IActionResult AppPersonalDetails()
        {
            return View();
        }
    }
}