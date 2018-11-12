using Imagein.Controllers.Base;
using Imagein.Entity.Dto;
using Imagein.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imagein.Controllers.ImageApi
{
    [Route("/api/v1/[controller]/[action]")]
    public class UploadController : BaseApiController
    {
        private readonly IFileService fileService = null;

        public UploadController(IFileService fileService)
        {
            this.fileService = fileService;
        }

        [HttpPost]
        [ActionName("Form")]
        public IActionResult UploadForm([FromBody]FileCreateDto data, IFormFile file)
        {
            var result = fileService.CreateFile(data, file);
            return SuccessResponse(result);
        }

        [HttpPost]
        [ActionName("Base64")]
        public IActionResult UploadFromBase64([FromBody]FileCreateDto data)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [ActionName("FormUrl")]
        public IActionResult UploadFromUrl([FromBody]FileCreateDto data)
        {
            throw new NotImplementedException();
        }
    }
}
