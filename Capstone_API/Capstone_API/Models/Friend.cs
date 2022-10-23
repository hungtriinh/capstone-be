﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capstone_API.Models
{
    [Table("Friend")]
    public class Friend
    {
        [ForeignKey("User")]
        [Column(Order = 1)]
        public int UserID { get; set; }
        [ForeignKey("User")]
        [Column(Order = 2)]
        public int UserFriendID { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        
    }
}
