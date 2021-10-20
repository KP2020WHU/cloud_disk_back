using CloudService.BLL;
using CloudService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CloudService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private UserManage _userManage;
        private CloudFileManage _cloudFileManage;
        //private IHttpContextAccessor _accessor;

        public UsersController(UserManage userManage, CloudFileManage cloudFileManage) // update StartUp.cs!!
        {
            _userManage = userManage;
            _cloudFileManage = cloudFileManage;
            //_accessor = accessor;
        }

        // Authorize
        [HttpPost]
        public IActionResult Authenticate(AuthenticationRequest model)
        {
            var response = _userManage.Authenticate(model);

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }

        [HttpPost]
        public ActionResult<User> SignUp(AuthenticationRequest model)
        {
            try
            {
                _cloudFileManage.CreateUserRoot(model.UserId);
                return _userManage.SignUp(model.UserId, model.Passwd);
            } catch(Exception e)
            {
                return BadRequest($"注册失败: {e.Message}");
            }
          
        }

        // function that requires Authorize
        [Authorize]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userManage.GetAll();
            return Ok(users);
        }

        [Authorize]
        [HttpGet]
        public ActionResult<User> GetUserInfo()
        {
            var user = (User)HttpContext.Items["User"];
            // 更新用户已用存储空间
            long alloacted =_cloudFileManage.GetAllocatedSpace(user.UserId);
            _userManage.UpdateUser(user.UserId, alloacted);
            user = _userManage.GetById(user.UserId);
            // 返回部分数据
            return new User()
            {
                UserId=user.UserId,
                UserName = user.UserName,
                DiskSpace = user.DiskSpace,
                AllocatedSpace = user.AllocatedSpace,
                Level = user.Level
            };
        }


        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
