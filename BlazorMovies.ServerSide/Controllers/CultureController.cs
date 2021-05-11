﻿using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMovies.ServerSide.Controllers
{
    [Route("[controller]/[action]")]
    public class CultureController: Controller
    {
        public IActionResult SetCulture(string culture, string redirectionUri)
        {
            if(culture!=null)
            {
                HttpContext.Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture))
                );

            }
            
            return LocalRedirect(redirectionUri);
        }
    }
}