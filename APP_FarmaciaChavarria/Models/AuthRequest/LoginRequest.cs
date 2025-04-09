using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP_FarmaciaChavarria.Models.AuthRequest
{
    public class LoginRequest
    {
        public string Usuario { get; set; }
        public int Pin { get; set; }

        public LoginRequest() { }

        public LoginRequest(string usuario, int pin)
        {
            this.Usuario = usuario.Trim().ToLower(); 
            this.Pin = pin;   
        }
    }
}
