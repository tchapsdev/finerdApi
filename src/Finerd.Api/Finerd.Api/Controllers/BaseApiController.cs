﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Finerd.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BaseApiController : ControllerBase
    {
        protected int UserID => int.Parse(FindClaim(ClaimTypes.NameIdentifier));
        private string FindClaim(string claimName)
        {
            var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
            var claim = claimsIdentity.FindFirst(claimName);

            if (claim == null)
            {
                return null;
            }
            return claim.Value;
        }

    }
}
