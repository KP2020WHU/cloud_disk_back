using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CloudService.Models
{
    public class MyFile
    {
        public static int CloudRootFileDepth { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string CreateUserName { get; set; }
        public string CreateTime { get; set; }
        public string EditTime { get; set; }
        public long Size { get; set; } // KB 为单位的文件大小
        public string SuffixName { get; set; }
        public int Type { get; set; }

        public MyFile(FileSystemInfo fsInfo)
        {
            Name = fsInfo.Name;
            Path = string.Join('/',fsInfo.FullName.Split('\\')[(CloudRootFileDepth + 3)..]); // 在云盘根路径后还有 Users Username root 三级目录
            CreateTime = fsInfo.CreationTimeUtc.ToString();
            EditTime = fsInfo.LastWriteTime.ToString();
            Size = fsInfo is FileInfo ? ((FileInfo)fsInfo).Length / (8 * 1024) + 1 : 0;
            SuffixName = fsInfo is FileInfo ? ((FileInfo)fsInfo).Extension[1..] : "";
            Type = fsInfo is FileInfo ? 0 : 1;
        }

        public MyFile()
        {

        }
    }
}
