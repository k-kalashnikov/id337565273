using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SP.Messenger.Infrastructure.Services.Infrastructure
{
    public class FileWriterOnDisk : IFileWriter
    {
        private readonly string _rootPath;
        private readonly string _directorySeparatorChar;

        public FileWriterOnDisk(string rootPath, string directorySeparatorChar)
        {
            _rootPath = rootPath;
            _directorySeparatorChar = directorySeparatorChar;
        }
        public static FileWriterOnDisk Create(string rootPath, string directorySeparatorChar)
            => new FileWriterOnDisk(rootPath, directorySeparatorChar);
        
        public async Task<string> WriterFileAsync(IFileMessage file)
        {
            var relativePath = string.Join(_directorySeparatorChar, file.RelativeDirectories);
            
            var relativePathWithFile = string.Join(_directorySeparatorChar, relativePath, file.Name);
            
            var fullPath = string.Join(_directorySeparatorChar, _rootPath, relativePath, file.Name);
            
            var fileDirectory = Path.GetDirectoryName(fullPath);

            if (!Directory.Exists(fileDirectory))
            {
                Directory.CreateDirectory(fileDirectory);
            }
            
            var fileDirectoryForWrite = BuildPath(relativePath, fileDirectory);
            
            var fileName = GenerationFileName(file.Name, fileDirectoryForWrite);
            fullPath = string.Join(_directorySeparatorChar, fileDirectoryForWrite, fileName);

            if (!Directory.Exists(fileDirectoryForWrite))
            {
                Directory.CreateDirectory(fileDirectoryForWrite);
            }

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            try
            {
                using (var fileStream = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    await file.Data.CopyToAsync(fileStream);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return fullPath.Replace(_rootPath,string.Empty);
        }

        private string BuildPath(string relativePath, string fileDirectory)
        {
            var path = string.Join(_directorySeparatorChar, _rootPath, relativePath);

            int count = 0;
            var list = new List<DirInfoPath>();

            var oldPath = String.Empty;
            
            int level = 2;
            for (int i = 1; i <= level; i++)
            {
                var dirInfo = new DirInfoPath();
                if (i == 1)
                {
                    dirInfo.Path = path;
                    dirInfo.Count = count;
                }
                else
                {
                    dirInfo.Path = oldPath;
                    dirInfo.Count = count;
                }
                
                dirInfo.RootPath = fileDirectory;
                
                dirInfo.Level = i;
                dirInfo = CallRecursive(dirInfo);
                count++; dirInfo.Count = count;
                list.Add(dirInfo);
                oldPath = dirInfo?.Path;

                if (dirInfo.IsStep)
                    i -= 1;
            }

            var infoPath = list.OrderByDescending(x => x.Count).FirstOrDefault();
            return infoPath?.Path;
        }

        private DirInfoPath CallRecursive(DirInfoPath infoPath)
        {
            var pathFull = $@"{infoPath.Path}";
            
            var catalogInfo = GetMaxLevel(pathFull, GetLevelValue(infoPath.Level));

            if (catalogInfo.mustMakeDir)
            {
                var dirInfo = new DirectoryInfo(infoPath.RootPath);
                var arrayDirectories = dirInfo.GetDirectories();
                
                var newPath =  $@"{infoPath.RootPath}{_directorySeparatorChar}{GetLevelValue(infoPath.Level - 1).ToString()}{arrayDirectories.Length + 1}";
                CreateDirectory(newPath);
                infoPath.MaxDirNumber = 1;
                infoPath.Path = newPath;
                infoPath.IsStep = true;
            }
            else if (catalogInfo.mustMakeDirOverflowFile)
            {
                var dirInfo = new DirectoryInfo(infoPath.Path);
                var arrayDirectories = dirInfo.GetDirectories();
                
                var newPath =  $@"{dirInfo}{_directorySeparatorChar}{GetLevelValue(infoPath.Level).ToString()}{arrayDirectories.Length + 1}";
                CreateDirectory(newPath);
                infoPath.MaxDirNumber = arrayDirectories.Length + 1;
                infoPath.Path = newPath;
                infoPath.IsStep = false;
            }
            else
            {
                infoPath.MaxDirNumber = catalogInfo.maxCatalogNumberLevel;
                infoPath.Path = $@"{infoPath.Path}{_directorySeparatorChar}{GetLevelValue(infoPath.Level).ToString()}{catalogInfo.maxCatalogNumberLevel}";
            }
            
            return infoPath;
        }

        private (int maxCatalogNumberLevel, bool mustMakeDir, bool mustMakeDirOverflowFile) GetMaxLevel(string path, LevelDirectory level)
        {
            var pathFull = string.Empty;
            int countDirs = 0;
            
            var dirInfo = new DirectoryInfo(path);
            var arrayDirectories = dirInfo.GetDirectories();
            if (arrayDirectories.Length == 0)
            {
                var number = arrayDirectories.Length + 1;
                pathFull = $@"{path}{_directorySeparatorChar}{level.ToString()}{number}";
                CreateDirectory(pathFull);
                countDirs = 1;
                return (countDirs, false, false);
            }
            else
            {
                if (level == LevelDirectory.a)
                {
                    return (arrayDirectories.Length, false, false);
                }
                else if(level == LevelDirectory.b)
                {
                    if (arrayDirectories.Length >= 100)
                        return (arrayDirectories.Length, true, false);
                    else
                    {
                        if (GetMaxFileInDir($@"{path}{_directorySeparatorChar}{level.ToString()}{arrayDirectories.Length}") >= 100)
                            return  (arrayDirectories.Length, false, true);
                        else
                            return  (arrayDirectories.Length, false, false);
                    }

                    
                }
            }
            
            return (arrayDirectories.Length, false, false);
        }

        private void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        private int GetMaxFileInDir(string path)
        {
            var dirInfo = new DirectoryInfo(path);
            return dirInfo.GetFiles()?.Length ?? 0;
        }

        private LevelDirectory GetLevelValue(int level)
        {
            switch (level)
            {
                case 1:
                    return LevelDirectory.a;
                case 2:
                    return LevelDirectory.b;
                default:
                    return LevelDirectory.a;
            }
        }

        private string GenerationFileName(string fileName, string path)
        {
            var fullPath = $"{path}{_directorySeparatorChar}{fileName}";
            if (File.Exists(fullPath))
            {
                var info = new FileInfo(fullPath);
                var pureName = info.Name.Replace($"{info.Extension}", string.Empty);
                var files = Directory.GetFiles(path, $"*{pureName}*", SearchOption.TopDirectoryOnly);
                return $"{pureName}({files.Length}){info.Extension}";
            }
            else
                return fileName;
        }
    }

    public enum LevelDirectory : int
    {
        [Description("a")] a = 1,
        [Description("b")] b = 2
    }

    public class DirInfoPath
    {
        public int Level { get; set; }
        public int Count { get; set; }
        public string Path { get; set; }
        public string RootPath { get; set; }
        public int MaxDirNumber { get; set; }
        public bool IsStep { get; set; }
    }

}