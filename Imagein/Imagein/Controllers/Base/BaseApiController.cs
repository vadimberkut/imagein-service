using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imagein.Controllers.Base
{
    public class BaseApiController : Controller
    {
        protected IActionResult SuccessResponse(object data)
        {
            return new JsonResult(new
            {
                Status = StatusCodes.Status200OK,
                Data = data,
                Message = ""
            });
        }
    }
}
