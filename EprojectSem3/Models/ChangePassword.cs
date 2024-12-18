﻿using System.ComponentModel.DataAnnotations;

namespace Realtors_Portal.Models
{
    public class ChangePassword
    {
        [Required(ErrorMessage = "Old password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Old Password must be at least 6 characters long.")]
        [DataType(DataType.Password)]
        public string OldPassword {  get; set; }
        [Required(ErrorMessage = "New password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm password is required.")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
