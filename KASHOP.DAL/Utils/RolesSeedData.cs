using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.Utils
{
    public class RolesSeedData : ISeedData
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesSeedData(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public async Task DataSeed()
        {
            string[] roles = ["User", "Admin", "SuperAdmin"];

            if (!await _roleManager.Roles.AnyAsync())
            {
                foreach (var role in roles)
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }

        }
    }
}
