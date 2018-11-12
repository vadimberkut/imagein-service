using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Imagein.Entity.Dto
{
    public class FileCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public string Url { get; set; }

        public string Name { get; set; }
        public string MimeType { get; set; }
        public int Size { get; set; }

        [JsonIgnore]
        public bool IsUploaded => String.IsNullOrEmpty(Url);
    }
}
