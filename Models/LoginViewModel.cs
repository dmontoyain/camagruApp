using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Pomelo.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace camagruApp.Models
{
    [ModelMetadataType(typeof(LoginViewModelMetaData))]
    public class LoginViewModel
    {
        public int id { get; set; }
        
        public string Username { get; set; }  
         
        public string Password { get; set; } 

        public bool online { get; set; }
    }
    public class LoginViewModelMetaData
    {
        [Required(AllowEmptyStrings =false, ErrorMessage = "Username is required")]
        public string Username { get; set; }  
        
        [DataType(DataType.Password)] 
        [Required(AllowEmptyStrings =false, ErrorMessage = "Password is required")]
        public string Password { get; set; }    
    }
}