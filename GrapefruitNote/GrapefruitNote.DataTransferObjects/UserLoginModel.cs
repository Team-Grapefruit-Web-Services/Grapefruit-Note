using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrapefruitNote.DataTransferObjects
{
    public class UserLoginModel
    {
        public string Username { get; set; }

        public string AuthCode { get; set; }
    }
}
