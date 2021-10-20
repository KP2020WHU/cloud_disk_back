using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CloudService.Models;
using System.IO;
using CloudService.BLL;

namespace CloudService.Controllers
{
    // 用于查询、移动、上传、下载文件等操作
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CloudFilesController : ControllerBase
    {
        private readonly CloudFileContext _context;
        private CloudFileManage cloudFileManage;

        public CloudFilesController(CloudFileContext context)
        {
            _context = context;
            
            cloudFileManage = new CloudFileManage(context);
        }

        // PUT: api/CloudFiles/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.

        [HttpGet]
        [Authorize]
        //[Route("download")]
        public ActionResult GetFile(string filePath)
        {
            User user = (User)HttpContext.Items["User"];

            try
            {
                var file = cloudFileManage.ReadFile(user.UserId, filePath);
                return File(file.Item1, file.Item2);
            }
            catch (Exception e)
            {
                return NotFound($"文件 {filePath} 不存在：" + e.Message);
            }

        }

        [HttpPost]
        [Authorize]
        public ActionResult<MyFile> UploadCloudFile([FromForm] IFormFile file, [FromForm] string path)
        {
            User user = (User)HttpContext.Items["User"];
            try
            {
                //保存文件到本地
                var myFile = cloudFileManage.WriteFile(file, user.UserId, path);
                return myFile;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult<MyFile> DeleteCloudFile(string path)
        {
            var user = (User)HttpContext.Items["User"];
            try
            {
                var myFile = cloudFileManage.DeleteFile(user.UserId, path);
                return myFile;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult<List<MyFile>> DeleteCloudFiles(MyFile[] files)
        {
            var user = (User)HttpContext.Items["User"];
            try
            {
                var deletedFiles = cloudFileManage.Delete(user.UserId, files);
                return deletedFiles;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Authorize]
        public ActionResult<MyFile> MoveCloudFile(string sourcePath, string destPath)
        {
            var user = (User)HttpContext.Items["User"];
//             try
//             {
                //保存文件到本地
                var myFile = cloudFileManage.MoveFile(user.UserId, sourcePath, destPath);
                return myFile;
//             }
//             catch (Exception ex)
//             {
//                 return BadRequest(ex.Message);
//             }
            
        }

        // TODO: improve the business logic, which currently doesn't make any sense
        [HttpGet]
        [Authorize]
        public ActionResult<MyFile> ShareFile(string destUser, string sourcePath, string destPath)
        {
            var user = (User)HttpContext.Items["User"];
            return cloudFileManage.ShareFile(destUser, user.UserId, sourcePath, destPath);
        }

        [HttpGet]
        [Authorize]
        public ActionResult<MyFile> MakeDir(string path)
        {
            var user = (User)HttpContext.Items["User"];
            return cloudFileManage.MakeDir(user.UserId, path);
        }

        // TODO: 添加不同用户的信息
        // 测试获取文件列表接口
        // GET: 
        [HttpGet]
        [Authorize]
        public ActionResult<List<MyFile>> GetCloudFileList(string path = "") // 或许可以统一fileList与fileListAll接口，通过添加一个是否递归查询
        {
            User user = (User)HttpContext.Items["User"];
            try
            {
                var fileList = cloudFileManage.GetFileList(user.UserId, path);
                return fileList;
            }
            catch (Exception ex)
            {
                return NotFound($"文件夹 {path} 不存在：" + ex.Message);
            }
        }

        // 测试获取文件列表接口
        // GET: 
        [HttpGet]
        [Authorize]
        public ActionResult<List<MyFile>> GetCloudFileListAll()
        {
            User user = (User)HttpContext.Items["User"];
            try
            {
                var fileListAll = cloudFileManage.GetFileListAll(user.UserId);
                return fileListAll;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
