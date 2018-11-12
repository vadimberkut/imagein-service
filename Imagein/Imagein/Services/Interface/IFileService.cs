using Imagein.Entity.Dto;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imagein.Services.Interface
{
    public interface IFileService
   {
        FileDto GetFileById(string id);
        FileDto CreateFile(FileCreateDto dto, IFormFile file);
        void DeleteFileById(string id);
    }
}
