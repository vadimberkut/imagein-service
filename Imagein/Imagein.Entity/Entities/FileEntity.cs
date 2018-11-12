using Imagein.Entity.Entities.Base;
using Imagein.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Imagein.Entity.Entities
{
    public class FileEntity : BaseEntity, ICreatable, IUpdatable
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public FileStoreType FileStoreType { get; set; }

        /// <summary>
        /// Local path to the file (including file name)
        /// E.g. /files/20181023/images/asdasd_ktty.jpg
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Remote Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Original file name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Physical file name
        /// </summary>
        public string PhysicalName { get; set; }
        public string MimeType { get; set; }
        public int Size { get; set; }


        #region Interface implementation

        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }

        #endregion
    }
}
