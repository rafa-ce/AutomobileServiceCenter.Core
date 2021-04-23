using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ASC.Web.Areas.Accounts.Models
{
    public class ProfileViewModel
    {
        [Required]
        public string UserName { get; set; }
    }
}
