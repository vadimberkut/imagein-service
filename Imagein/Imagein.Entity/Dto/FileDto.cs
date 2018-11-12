using System;
using System.Collections.Generic;
using System.Text;

namespace Imagein.Entity.Dto
{
    public class FileDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public string Url { get; set; }

        public string Name { get; set; }
        public string PhysicalName { get; set; }
        public string MimeType { get; set; }
        public int Size { get; set; }
        public double SizeKb => Math.Round((double)Size / (double)1024, 2);
        public double SizeMb => Math.Round((double)SizeKb / (double)1024, 2);

        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
    }
}
