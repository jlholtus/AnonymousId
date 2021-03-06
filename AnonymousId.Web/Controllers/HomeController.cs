﻿using Microsoft.AspNetCore.Mvc;
using Onots.AspNetCore.Identity.Anonymous;

namespace Onots.AspNetCore.Identity.Anonymous.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            IAnonymousIdFeature feature = HttpContext.Features.Get<IAnonymousIdFeature>();
            if (feature != null)
            {
                string anonymousId = feature.AnonymousId;
            }

            return View();
        }
    }
}