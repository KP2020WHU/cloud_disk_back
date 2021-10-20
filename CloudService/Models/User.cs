using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CloudService.Models
{
    public class User
    {
        public string UserName { get; set; }
        [Required]
        public string UserId { get; set; }
//        [JsonIgnore]
        [Required]
        public string Passwd { get; set; }
        public long DiskSpace { get; set; }
        public long AllocatedSpace { get; set; }
        public int Level { get; set; }
        [NotMapped]
        string Tocken { get; set; }


        public User()
        {
        }
        public User(string userId="", string passwd="", string userName = "Cloud Disk User", long diskSpace = 16384, long allocatedSpace = 0, int level = 1)
        {
            UserId = userId;
            Passwd = passwd;
            UserName = userName;
            DiskSpace = diskSpace;
            AllocatedSpace = allocatedSpace;
            Level = level;
        }
        public User(string userId, string passwd)
        {
            UserId = userId;
            Passwd = passwd;
        }
        public bool Valid(string passwd)
        {
            return Passwd.Equals(passwd);
        }

    }
}
