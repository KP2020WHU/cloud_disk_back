using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CloudService.Models
{
    public class CloudFile
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public long Size { get; set; }

        public CloudFile(string name, string path, long size)
        {
            Name = name;
            Path = path;
            Size = size;
        }
    }


}
