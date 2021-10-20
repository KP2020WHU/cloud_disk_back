using CloudService.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CloudService.BLL
{
    public class CloudFileManage
    {
        private readonly CloudFileContext _context;
        private readonly string _cloudRootPath = "D:/Cloud";

        public CloudFileManage(CloudFileContext context)
        {
            // TODO: initialize cloud file directory if doesn't exist
            _context = context;
            MyFile.CloudRootFileDepth = _cloudRootPath.Split('/').Length; // 获取云盘根路径深度，用于获取用户相对路径
        }

        public Tuple<FileStream, string> ReadFile(string userId, string path)
        {
            var filefullPath = Path.Combine(_cloudRootPath, "Users", userId, "root", path);
            string contentType;
            new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider().TryGetContentType(path, out contentType);
            contentType = contentType is null ? "application/octet-stream" : contentType;
            FileStream fs = new FileStream(filefullPath, FileMode.Open);
            return new Tuple<FileStream, string>(fs, contentType);

        }

        public MyFile WriteFile(IFormFile file, string userId, string path) // TODO: 更新用户信息
        {
            var filefullPath = Path.Combine(_cloudRootPath, "Users", userId, "root", path, file.FileName);
            using (FileStream fs = new FileStream(filefullPath, FileMode.Create))
            {
                file.CopyTo(fs);
                fs.Flush();
            }
            MyFile res = new MyFile(new FileInfo(filefullPath));
            res.CreateUserName = userId;
            return res;
        }

        public MyFile DeleteFile(string userId, string path) // TODO: 用户剩余空间检测，更新用户空间信息
        {
            var filefullPath = Path.Combine(_cloudRootPath, "Users", userId, "root", path);
            FileInfo fInfo = new FileInfo(filefullPath);
            MyFile file = new MyFile(fInfo);
            File.Delete(filefullPath);
            return file;
        }

        public MyFile DeleteDirectory(string userId, string path) // TODO: 用户剩余空间检测，更新用户空间信息
        {
            var filefullPath = Path.Combine(_cloudRootPath, "Users", userId, "root", path);
            DirectoryInfo fInfo = new DirectoryInfo(filefullPath);
            MyFile file = new MyFile(fInfo);
            Directory.Delete(filefullPath, true);
            return file;
        }

        public List<MyFile> Delete(string userId, MyFile[] files)
        {
            List<MyFile> deletedFiles = new List<MyFile>();
            for (int i = 0; i < files.Length; i++)
            {
                var filefullPath = Path.Combine(_cloudRootPath, "Users", userId, "root", files[i].Path); // 计算云端路径
                // 判断是文件夹还是其他文件
                if (files[i].Type == 1) 
                    deletedFiles.Add(DeleteDirectory(userId, filefullPath));
                else
                    deletedFiles.Add(DeleteFile(userId, filefullPath));
            }
            return deletedFiles;
        }

        public MyFile MoveFile(string userId, string source, string dest)
        {
            source = Path.Combine(_cloudRootPath, "Users", userId, "root", source);
            dest = Path.Combine(_cloudRootPath, "Users", userId, "root", dest);
            File.Copy(source, dest, true);
            return new MyFile(new FileInfo(dest));
        }

        // 使用soft link?
        public MyFile ShareFile(string sourceUserId, string destUserId, string source, string dest)
        {
            source = Path.Combine(_cloudRootPath, "Users", sourceUserId, "root", source);
            dest = Path.Combine(_cloudRootPath, "Users", destUserId, "root", dest);
            File.Copy(source, dest); //? 
            return new MyFile(new FileInfo(dest));
        }

        public MyFile MakeDir(string userId, string path)
        {
            var filefullPath = Path.Combine(_cloudRootPath, "Users", userId, "root", path);
            DirectoryInfo fInfo = Directory.CreateDirectory(filefullPath);
            MyFile file = new MyFile(fInfo);
            return file;
        }

        public List<MyFile> GetFileList(string userId, string path)
        {
            string fullPath = Path.Combine(_cloudRootPath, "Users", userId, "root", path); // 将用户相对路径转换为云盘上绝对路径
            var dir = new DirectoryInfo(fullPath);
            var fsInfos = dir.GetFileSystemInfos();
            List<MyFile> res = new List<MyFile>();
            foreach (var fsInfo in fsInfos)
            {
                MyFile myFile = new MyFile(fsInfo);
                myFile.CreateUserName = userId;
                res.Add(myFile);
            }
            return res;
        }

        public List<MyFile> GetFileListAll(string userId)
        {
            void Recur(DirectoryInfo dir, ref List<MyFile> res)
            {
                var fsInfos = dir.GetFileSystemInfos();
                foreach (var fsInfo in fsInfos)
                {
                    MyFile myFile = new MyFile(fsInfo);
                    myFile.CreateUserName = userId;
                    res.Add(myFile);
                    if (fsInfo is DirectoryInfo)
                        Recur((DirectoryInfo)fsInfo, ref res);
                }
            }

            string fullPath = Path.Combine(_cloudRootPath, "Users", userId, "root"); // 将用户相对路径转换为云盘上绝对路径
            var dir = new DirectoryInfo(fullPath);
            List<MyFile> res = new List<MyFile>();
            Recur(dir, ref res);
            return res;
        }

        public long GetAllocatedSpace(string userId)
        {
            long Recur(DirectoryInfo dir)
            {
                var fsInfos = dir.GetFileSystemInfos();
                long size = 0;
                foreach (var fsInfo in fsInfos)
                    size += (fsInfo is FileInfo ? ((FileInfo)fsInfo).Length / (1024 * 8): Recur((DirectoryInfo)fsInfo));
                return size;
            }

            string fullPath = Path.Combine(_cloudRootPath, "Users", userId, "root"); // 将用户相对路径转换为云盘上绝对路径
            var dir = new DirectoryInfo(fullPath);
            return Recur(dir);
        }


        // TODO: Add other configuration file of the user while initializing
        public void CreateUserRoot(string userId)
        {
            string userDir = Path.Combine(_cloudRootPath, "Users", userId);
            string userRootDir = Path.Combine(userDir, "root");
            string userConfigFilePath = Path.Combine(userDir, "UserConfig");
            Directory.CreateDirectory(userDir);
            Directory.CreateDirectory(userRootDir);
            File.CreateText(userConfigFilePath);
        }




    }
}
