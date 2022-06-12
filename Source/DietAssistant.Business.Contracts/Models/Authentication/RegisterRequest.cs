using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DietAssistant.Business.Contracts.Models.Authentication
{
    public class RegisterRequest
    {
        public string Name { get; set; }

        public String Email { get; set; }

        public String Password { get; set; }
        
        public String ConfirmPassword { get; set; }
    }
}
