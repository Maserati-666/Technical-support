﻿using System.ComponentModel.DataAnnotations;

namespace Technical_support.ViewModel
{
    public class CreateUser
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
        
        [Required]
        public string RoleId { get; set; }
    }
}
