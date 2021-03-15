using System;
using System.Collections.Generic;
using System.Text;
using ElCamino.AspNetCore.Identity.AzureTable;
using ElCamino.AspNetCore.Identity.AzureTable.Model;
using Microsoft.EntityFrameworkCore;

namespace ASC.Web.Data
{
    public class ApplicationDbContext : IdentityCloudContext
    {
        public ApplicationDbContext() : base() { } 
        public ApplicationDbContext(IdentityConfiguration config) : base(config) { }
    }
}
