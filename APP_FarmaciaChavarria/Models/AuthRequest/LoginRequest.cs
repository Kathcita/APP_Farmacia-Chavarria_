using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP_FarmaciaChavarria.Models.AuthRequest
{
    public class LoginRequest
    {
        public string nombre { get; set; }
        public int pin { get; set; }

        public LoginRequest() { }

        public LoginRequest(string usuario, int pin)
        {
            this.nombre = usuario.Trim().ToLower(); 
            this.pin = pin;   
        }
    }
}
