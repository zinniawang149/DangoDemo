using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DangoAPI.Models
{
    public class Like
    {
        public int LikerId { get; set; } //Who you like
        public int LikeeId { get; set; } //Who likes you
        public User Liker { get; set; }
        public User Likee { get; set; }

    }
}
