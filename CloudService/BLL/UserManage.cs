using CloudService.Helpers;
using CloudService.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CloudService.BLL
{
    public class UserManage
    {
        private readonly UserContext _context;
        private readonly AppSettings _appSettings;

        public UserManage(UserContext context, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _appSettings = appSettings.Value;
        }

        public AuthenticationResponse Authenticate(AuthenticationRequest model)
        {
            var user = _context.users.SingleOrDefault(x => x.UserId == model.UserId && x.Passwd == model.Passwd);

            // return null if user not found
            if (user == null) return null;

            // authentication successful so generate jwt token
            var token = generateJwtToken(user);

            return new AuthenticationResponse(user.UserId, token);
        }

        private string generateJwtToken(User user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.UserId.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public User GetById(string userId)
        {
            IQueryable<User> query = _context.users;
            //var user = query.Where(u => u.UserId.Equals(userId));
            return query.FirstOrDefault(u => u.UserId.Equals(userId));
        }

        public List<User> GetAll()
        {
            return _context.users.ToList();
        }

        public User SignIn(string userId, string passwd)
        {
            IQueryable<User> query = _context.users;
            //var user = query.Where(u => u.UserId.Equals(userId));
            var user = query.FirstOrDefault(u => u.UserId.Equals(userId));
            if (user is null)
                throw new Exception("用户不存在");
            if (!user.Valid(passwd))
                throw new Exception("用户密码错误");
            return user;
        }

        public User SignUp(string userId, string passwd)
        {
            IQueryable<User> query = _context.users;
            if (!(query.FirstOrDefault(u => u.UserId.Equals(userId)) is null))
                throw new Exception("用户已存在");
            User newUser = new User(userId, passwd);
            _context.users.Add(newUser);
            _context.SaveChanges();
            return newUser;
        }


        public bool UserExists(string userId)
        {
            IQueryable<User> query = _context.users;
            return !(query.FirstOrDefault(u => u.UserId.Equals(userId)) is null);
        }

        public void UpdateUser(string userId, long allocatedStorage)
        {
            IQueryable<User> query = _context.users;
            User user = query.FirstOrDefault(u => u.UserId.Equals(userId));
            user.AllocatedSpace = allocatedStorage;
            _context.SaveChanges();
        }

    }
}
