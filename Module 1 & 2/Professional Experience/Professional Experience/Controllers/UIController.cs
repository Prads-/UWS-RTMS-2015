﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Professional_Experience.Controllers
{
    public class UIController : Controller
    {
        //Database can be accessed using this object
        protected PX_Model.PX_Entities _db = new PX_Model.PX_Entities();
    }
}