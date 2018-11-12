using Imagein.Data.Repositories.Interface;
using Imagein.Entity.Dto;
using Imagein.Entity.Entities;
using Imagein.Entity.Enums;
using Imagein.Entity.Mappers;
using Imagein.Services.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MimeTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Imagein.Services
{
    public class FileService : IFileService
    {
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IConfiguration configuration;
        private readonly IUnitOfWork unitOfWork;

        public FileService(
            IHostingEnvironment hostingEnvironment, 
            IConfiguration configuration, 
            IUnitOfWork unitOfWork
        )
        {
            this.hostingEnvironment = hostingEnvironment;
            this.configuration = configuration;
            this.unitOfWork = unitOfWork;
        }

        public FileDto GetFileById(string id)
        {
            var entity = unitOfWork.FileRepository.GetById(id);

            var dto = CustomMapper.ToFileDto(entity);
            return dto;
        }

        public FileDto CreateFile(FileCreateDto createDto, IFormFile file)
        {
            FileEntity entity = null;

            // Save to disk
            if(createDto.IsUploaded)
            {
                if(String.IsNullOrEmpty(createDto.MimeType))
                {
                    createDto.MimeType = MimeTypeMap.GetMimeType(Path.GetExtension(createDto.Name));
                }

                entity = SaveToFile(createDto, file);
                entity.FileStoreType = FileStoreType.Local;
            }
            else
            {
                // TODO: Get file metadata?
                entity = CustomMapper.ToFileEntity(createDto);
                entity.FileStoreType = FileStoreType.Remote;
            }

            // Save to DB
            unitOfWork.FileRepository.Add(entity);
            unitOfWork.Commit();

            var dto = CustomMapper.ToFileDto(entity);
            return dto;
        }

        public void DeleteFileById(string id)
        {
            var entity = unitOfWork.FileRepository.GetById(id);
            DeleteFile(entity); // Delete from disk
            unitOfWork.FileRepository.Delete(x => x.Id == id);
            unitOfWork.Commit();
        }

        #region Private members

        private string fileStorePath => configuration.GetSection("FileStore").GetValue<string>("StorePath");
        private int maxPathLength => configuration.GetSection("FileStore").GetValue<int>("MaxPathLength");
        private int maxDirectoriesInDirectory => configuration.GetSection("FileStore").GetValue<int>("MaxDirectoriesInDirectory");
        private int maxFilesInDirectory => configuration.GetSection("FileStore").GetValue<int>("MaxFilesInDirectory");
        private int maxStoreLevel => configuration.GetSection("FileStore").GetValue<int>("MaxStoreLevel");
        private int maxFileSize => configuration.GetSection("FileStore").GetValue<int>("MaxFileSize");

        private FsNode _fileStoreDirectoryImage = null;
        private FsNode fileStoreDirectoryImage
        {
            get
            {
                if(_fileStoreDirectoryImage == null)
                {
                    _fileStoreDirectoryImage = BuildDirectoryImage(fileStorePath);
                }
                return _fileStoreDirectoryImage;
            }
        }

        private FsNode BuildDirectoryImage(string absPath)
        {
            // When Directory
            if(Directory.Exists(absPath))
            {
                var currentNode = new FsNode(absPath);

                var directories = Directory.GetDirectories(absPath);
                var files = Directory.GetFiles(absPath);

                foreach (var directory in directories)
                {
                    // Here recursion starts
                    var childDirectoryNode = BuildDirectoryImage(Path.Combine(absPath, directory));
                    currentNode.AddChildNode(childDirectoryNode);
                }
                foreach (var file in files)
                {
                    currentNode.AddChildNode(new FsNode(Path.Combine(absPath, file)));
                }

                return currentNode;
            }
            // When File
            else if (File.Exists(absPath))
            {
                return new FsNode(absPath);
            }
            // Otherwise
            else
            {
                throw new Exception("Unknown fs node");
            }
        }

        private void PrepareFileStoreStructure()
        {
            if(!Directory.Exists(fileStorePath))
            {
                Directory.CreateDirectory(fileStorePath);
            }
        }

        internal class FindFileStorePathResult
        {
            /// <summary>
            /// Only directories
            /// </summary>
            public string Path { get; set; }
            public string FileName { get; set; }
            public List<string> DirectoriesToCreate = new List<string>();
        }

        // Used wrong approach - recursion. That doesn't allow to go level by level
        private FindFileStorePathResult FindFileStorePathAndCreateIntermediateNodes_DRAFT(string mimeType, IFsNode currentNode, IFsNode parentNode = null, int currentLevel = 0)
        {
            throw new NotImplementedException();

            if(!currentNode.IsDirectory)
            {
                throw new ArgumentException("Node must be a directory");
            }
            if (currentLevel > maxStoreLevel)
            {
                throw new ArgumentException($"Max level {maxStoreLevel} was reached");
            }

            bool isRootNode = parentNode == null;
            var result = new FindFileStorePathResult()
            {
                Path = Path.Combine(parentNode?.Name ?? "")
            };

            // Create initila folder structure for mime types
            if (isRootNode)
            {
                var mimeParts = mimeType.Split('/');
                if(mimeParts.Length != 2)
                {
                    throw new Exception($"Wrong mime type provided: {mimeType}");
                }
                if(!currentNode.ContainsDirectory(mimeParts[0]))
                {
                    // Add both mime 1 and 2 parts
                    result.DirectoriesToCreate.Add(Path.Combine(currentNode.Name, mimeParts[0]));
                    result.DirectoriesToCreate.Add(Path.Combine(currentNode.Name, mimeParts[0], mimeParts[1]));

                    var mime1Node = new FsNode();
                    var mime2Node = new FsNode();

                    mime1Node.SetToBeDirectory(mimeParts[0]);
                    mime2Node.SetToBeDirectory(mimeParts[1]);

                    mime1Node.AddChildNode(mime2Node);
                    currentNode.AddChildNode(mime1Node);
                }
                else
                {
                    // Add only mime 2 part
                    var mime1Node = currentNode.FindDirectoryByName(mimeParts[0]);
                    if (!mime1Node.ContainsDirectory(mimeParts[1]))
                    {
                        result.DirectoriesToCreate.Add(Path.Combine(currentNode.Name, mimeParts[0], mimeParts[1]));

                        var mime2Node = new FsNode();
                        mime2Node.SetToBeDirectory(mimeParts[1]);
                        mime1Node.AddChildNode(mime2Node);
                    }
                }
            }


            // If we can save file in current node
            if (currentNode.FileCount < maxFilesInDirectory)
            {
                // Get file name
                // Template: level_fileNumber.extension
                string fileName = $"{currentLevel}_${currentNode.FileCount}";

                result.Path = Path.Combine(result.Path, currentNode.Name);
                result.FileName = fileName;
                return result;
            }
            // If we can create directory in current node
            else if (currentNode.DirectoryCount < maxDirectoriesInDirectory)
            {
                //Get new dir name
                // E.g. Now 3 dirs (names: 0,1,2). New will be 3 
                string newDirectoryName = $"{currentNode.DirectoryCount}";
                string newDirectoryPath = Path.Combine(result.Path, currentNode.Name, newDirectoryName);

                result.Path = newDirectoryPath;
                result.FileName = $"{currentLevel}_${currentNode.FileCount}";
                result.DirectoriesToCreate.Add(newDirectoryPath);
                return result;
            }
            else
            {
                // TODO - looks like this is redundant
                // Search sub directories
                // Look on files on next nested level
                // E.g. if we on 0 (root) check level 1 files
                foreach (var subDir in currentNode.Directories)
                {
                    if (subDir.FileCount < maxFilesInDirectory)
                    {
                        result.Path = Path.Combine(result.Path, subDir.Name);
                        result.FileName = $"{currentLevel + 1}_${subDir.FileCount}";
                        return result;
                    }
                }

                // TODO
                // Not found - go to next level
                //foreach (var subDir in currentNode.Directories)
                //{
                //    var subResult = FindFileStorePathAndCreateIntermediateNodes(mimeType, subDir, currentNode, currentLevel + 1);
                //    if (subResult != null)
                //    {
                //        result.Path = Path.Combine(result.Path, subResult.Path);
                //        result.FileName = subResult.FileName;
                //        result.DirectoriesToCreate.AddRange(subResult.DirectoriesToCreate);
                //        return result;
                //    }
                //}

               
            }

            // If we get here then path not found
            return null;
            
            
            
            // it means that storage is full
            // throw new Exception($"Can't find file store path. Storage is full");
        }

        private FindFileStorePathResult FindFileStorePathAndCreateIntermediateNodes(string mimeType, IFsNode rootNode)
        {
            var result = new FindFileStorePathResult()
            {
                Path = ""
            };

            // Create initila folder structure for mime types
            var mimeParts = mimeType.Split('/');
            if (mimeParts.Length != 2)
            {
                throw new Exception($"Wrong mime type provided: {mimeType}");
            }
            if (!rootNode.ContainsDirectory(mimeParts[0]))
            {
                // Add both mime 1 and 2 parts
                result.DirectoriesToCreate.Add(Path.Combine(rootNode.Name, mimeParts[0]));
                result.DirectoriesToCreate.Add(Path.Combine(rootNode.Name, mimeParts[0], mimeParts[1]));

                var mime1Node = new FsNode();
                var mime2Node = new FsNode();

                mime1Node.SetToBeDirectory(mimeParts[0]);
                mime2Node.SetToBeDirectory(mimeParts[1]);

                mime1Node.AddChildNode(mime2Node);
                rootNode.AddChildNode(mime1Node);
            }
            else
            {
                // Add only mime 2 part
                var mime1Node = rootNode.FindDirectoryByName(mimeParts[0]);
                if (!mime1Node.ContainsDirectory(mimeParts[1]))
                {
                    result.DirectoriesToCreate.Add(Path.Combine(rootNode.Name, mimeParts[0], mimeParts[1]));

                    var mime2Node = new FsNode();
                    mime2Node.SetToBeDirectory(mimeParts[1]);
                    mime1Node.AddChildNode(mime2Node);
                }
            }



            var levelNodes = new List<IFsNode>();
            levelNodes.Add(rootNode);
            for (int currentLevel = 0; currentLevel < maxStoreLevel; currentLevel++)
            {
                foreach (var levelNode in levelNodes)
                {
                    // If we can save file in current node
                    if (levelNode.FileCount < maxFilesInDirectory)
                    {
                        // Get file name
                        // Template: level_fileNumber.extension
                        string fileName = $"{currentLevel}_${levelNode.FileCount}";

                        result.Path = Path.Combine(result.Path, levelNode.Name);
                        result.FileName = fileName;
                        return result;
                    }
                    // If we can create directory in current node
                    else if (levelNode.DirectoryCount < maxDirectoriesInDirectory)
                    {
                        //Get new dir name
                        // E.g. Now 3 dirs (names: 0,1,2). New will be 3 
                        string newDirectoryName = $"{levelNode.DirectoryCount}";
                        string newDirectoryPath = Path.Combine(result.Path, levelNode.Name, newDirectoryName);

                        result.Path = newDirectoryPath;
                        result.FileName = $"{currentLevel}_${levelNode.FileCount}";
                        result.DirectoriesToCreate.Add(newDirectoryPath);
                        return result;
                    }
                }

                // Go to next level
                levelNodes = levelNodes.SelectMany(x => x.ChildNodes).ToList();
            }

            throw new Exception($"Can't find file store path. Storage is full");
        }

        private void AddFileToFileStoreImage(string relativePath, IFsNode rootNode)
        {
            var parts = relativePath.Split(Path.DirectorySeparatorChar);
            var directoryNames = parts.Take(parts.Length - 1);
            var fileName = parts.Last();

            IFsNode targetDirectoryNode = rootNode;
            foreach (var directoryName in directoryNames)
            {
                var found = targetDirectoryNode.FindDirectoryByName(directoryName);
                if(found == null)
                {
                    throw new Exception("Can't find one directory of target path");
                }
                targetDirectoryNode = found;
            }

            var fileNode = new FsNode();
            fileNode.SetToBeFile(fileName);
            targetDirectoryNode.AddChildNode(fileNode);
        }

        // TODO: How this will work when few threads will execute it???
        // Needed the way to handle parallel requests to FS image and update it and persist data consistency
        private FileEntity SaveToFile(FileCreateDto createDto, IFormFile file)
        {
            PrepareFileStoreStructure();

            // Get path where to save file
            // + image will be updated with intermediate directories
            var pathResult = FindFileStorePathAndCreateIntermediateNodes(createDto.MimeType, fileStoreDirectoryImage);
            
            var relativePath = pathResult.Path;
            var absolutPath = Path.Combine(fileStorePath, relativePath);

            if (absolutPath.Length > maxPathLength)
            {
                throw new Exception($"Max path length {maxPathLength} excedded!");
            }

            // Create intermediate directories
            if(pathResult.DirectoriesToCreate.Any())
            {
                foreach (var dirRelPath in pathResult.DirectoriesToCreate)
                {
                    string dirAbsPath = Path.Combine(fileStorePath, dirRelPath);
                    if(!Directory.Exists(dirAbsPath))
                    {
                        Directory.CreateDirectory(dirAbsPath);
                    }
                }
            }

            // Check the file length and don't bother attempting to
            // read it if the file contains no content. This check
            // doesn't catch files that only have a BOM as their
            // content, so a content length check is made later after 
            // reading the file's content to catch a file that only
            // contains a BOM.
            if (file.Length == 0)
            {
                throw new Exception("The file is empty");
            }
            if(file.Length > maxFileSize)
            {
                throw new Exception($"The file exceeds ${maxFileSize / 1024} KB");
            }

            try
            {
                using (var writeStream = new FileStream(absolutPath, FileMode.CreateNew))
                {
                    file.CopyTo(writeStream);
                }
            }
            catch(PathTooLongException ex)
            {
                throw new Exception("Maximum path size was exceeded", ex);
            }
            catch(IOException ex)
            {
                throw new Exception("The file is already exists", ex);
            }

            // If we get here - file was saved
            AddFileToFileStoreImage(relativePath, fileStoreDirectoryImage);

            // TODO: update cache

            var entity = CustomMapper.ToFileEntity(createDto);
            entity.Path = relativePath;
            entity.PhysicalName = Path.GetFileName(relativePath);
            return null;
        }

        private void DeleteFile(FileEntity entity)
        {
            var path = Path.Combine(fileStorePath, entity.Path);
            if (entity.FileStoreType == FileStoreType.Local && File.Exists(path))
            {
                File.Delete(entity.Path);

                // TODO: update image
                // TODO: update cache
            }
        }

        #endregion
    }

    public interface IFsNode
    {
        string Name { get; }
        bool IsDirectory { get; }
        bool IsFile { get; }

        IList<IFsNode> ChildNodes { get; }

        int DirectoryCount { get; }
        int FileCount { get; }

        IEnumerable<IFsNode> Directories { get; }
        IEnumerable<IFsNode> Files { get; }

        void SetToBeDirectory(string name);
        void SetToBeFile(string name);

        void AddChildNode(IFsNode node);

        bool ContainsDirectory(string name);
        bool ContainsFile(string name);

        IFsNode FindDirectoryByName(string name);
        IFsNode FindFileByName(string name);

    }
    public class FsNode : IFsNode
    {
        public string Name { get; private set; }
        public bool IsDirectory { get; private set; }
        public bool IsFile { get; private set; }

        private IList<IFsNode> _childNodes = new List<IFsNode>();
        public IList<IFsNode> ChildNodes => _childNodes;


        public int DirectoryCount => _childNodes.Count(x => x.IsDirectory);
        public int FileCount => _childNodes.Count(x => x.IsFile);

        public IEnumerable<IFsNode> Directories => _childNodes.Where(x => x.IsDirectory);
        public IEnumerable<IFsNode> Files => _childNodes.Where(x => x.IsFile);

        public FsNode()
        {

        }

        public FsNode(string absolutePath)
        {
            if(Directory.Exists(absolutePath))
            {
                IsDirectory = true;
                Name = Path.GetDirectoryName(absolutePath);
            }
            else if (File.Exists(absolutePath))
            {
                IsFile = true;
                Name = Path.GetFileName(absolutePath);
            }
            else
            {
                throw new Exception("Unsupported fs node");
            }
        }

        public void SetToBeDirectory(string name)
        {
            Name = name;
            IsDirectory = true;
            IsFile = false;
        }
        public void SetToBeFile(string name)
        {
            Name = name;
            IsDirectory = false;
            IsFile = true;
        }

        public void AddChildNode(IFsNode node)
        {
            _childNodes.Add(node);
        }

        public bool ContainsDirectory(string name)
        {
            return _childNodes.Any(x => x.IsDirectory && x.Name.Equals(name));
        }
        public bool ContainsFile(string name)
        {
            return _childNodes.Any(x => x.IsFile && x.Name.Equals(name));
        }

        public IFsNode FindDirectoryByName(string name)
        {
            return _childNodes.FirstOrDefault(x => x.IsDirectory && x.Name == name);
        }
        public IFsNode FindFileByName(string name)
        {
            return _childNodes.FirstOrDefault(x => x.IsFile && x.Name == name);
        }
    }
}
