using System;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace camagruApp.Models
{
    [ModelMetadataType(typeof(UserMetaData))]
    public partial class User
    {
//        public string ConfirmPassword { get; set; }
    }

    public class UserMetaData
    {
        public int id { get; set; }

        [Required(AllowEmptyStrings =false, ErrorMessage = "First Name is required")]
        [Display(Name ="First Name")]
        public string firstname { get; set; }

        [Required(AllowEmptyStrings =false, ErrorMessage = "Last Name is required")]
        [Display(Name ="Last Name")]
        public string lastname { get; set; }

        [Required(AllowEmptyStrings =false, ErrorMessage = "Email required")]
        [Display(Name ="Email")]
        public string email { get; set; }

        [Required(AllowEmptyStrings =false, ErrorMessage = "Username required")]
        [Display(Name ="Username")]
        public string username { get; set; }

        [Required(AllowEmptyStrings =false, ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage ="Minimum 6 characters required")]
        [Display(Name ="Password")]
        public string passwd {get; set; }

        [Display(Name ="Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("passwd", ErrorMessage ="Passwords don't match")]
        public string ConfirmPassword { get; set; }
    }
}