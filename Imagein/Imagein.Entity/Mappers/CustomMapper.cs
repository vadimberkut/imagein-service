using Imagein.Entity.Dto;
using Imagein.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Imagein.Entity.Mappers
{
    public static class CustomMapper
    {
        public static FileDto ToFileDto(FileEntity from)
        {
            var to = new FileDto
            {
                Id = from.Id,
                Title = from.Title,
                Description = from.Description,
                Name = from.Name,
                PhysicalName = from.PhysicalName,
                Size = from.Size,
                MimeType = from.MimeType,
                Url = from.Url,
                CreatedOnUtc = from.CreatedOnUtc,
                UpdatedOnUtc = from.UpdatedOnUtc,
            };
            return to;
        }

        public static FileEntity ToFileEntity(FileCreateDto from)
        {
            var to = new FileEntity
            {
                Title = from.Title,
                Description = from.Description,
                Name = from.Name,
                Size = from.Size,
                MimeType = from.MimeType,
                Url = from.Url,
            };
            return to;
        }

        public static FileEntity ToFileEntity(FileDto from)
        {
            var to = new FileEntity
            {
                Id = from.Id,
                Title = from.Title,
                Description = from.Description,
                Name = from.Name,
                PhysicalName = from.PhysicalName,
                Size = from.Size,
                MimeType = from.MimeType,
                Url = from.Url,
                CreatedOnUtc = from.CreatedOnUtc,
                UpdatedOnUtc = from.UpdatedOnUtc,
            };
            return to;
        }
    }
}
