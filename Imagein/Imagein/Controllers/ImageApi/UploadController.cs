using Imagein.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imagein.Controllers.ImageApi
{
    public class UploadController : BaseApiController
    {
        public UploadController()
        {

        }

        [HttpPost]
        public IActionResult Upload()
        {
            return new EmptyResult();
        }
    }
}
