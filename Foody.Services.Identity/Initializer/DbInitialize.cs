using Foody.Services.Identity.DbContexts;
using Foody.Services.Identity.Models;
using Foody.Services.Identity.Utils;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Foody.Services.Identity.Initializer
{
    public class DbInitialize : IDbInitialize
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitialize(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            if(_roleManager.FindByNameAsync(Constants.Admin).Result == null)
            {
                _roleManager.CreateAsync(new IdentityRole(Constants.Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(Constants.Customer)).GetAwaiter().GetResult();
            }
            else
            {
                return;
            }

            ApplicationUser admin = new ApplicationUser()
            {
                FirstName = "Bagh",
                LastName = "Admin",
                Email = "bagh.admin@foody.com",
                UserName = "bagh.admin@foody.com",
                EmailConfirmed = true,
                PhoneNumber = "11111111111"
            };

            _userManager.CreateAsync(admin,"Admin@123").GetAwaiter().GetResult();
            

            var tempVar = _userManager.AddClaimsAsync(admin, new Claim[]
            {
                new Claim(JwtClaimTypes.Name,admin.FirstName+" "+admin.LastName),
                new Claim(JwtClaimTypes.Email,admin.Email),
                new Claim(JwtClaimTypes.GivenName,admin.FirstName),
                new Claim(JwtClaimTypes.FamilyName,admin.LastName),
                new Claim(JwtClaimTypes.Role,Constants.Admin)
            }).Result;

            ApplicationUser customer = new ApplicationUser()
            {
                FirstName = "Sunny",
                LastName = "Sam",
                Email = "sam.sunny@foody.com",
                UserName = "sam.sunny@foody.com",
                EmailConfirmed = true,
                PhoneNumber = "11155555111"
            };

            _userManager.CreateAsync(customer, "Customer@123").GetAwaiter().GetResult();

            _userManager.AddToRoleAsync(admin, Constants.Admin).GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(customer, Constants.Customer).GetAwaiter().GetResult();

            var tempVarCu = _userManager.AddClaimsAsync(customer, new Claim[]
            {
                new Claim(JwtClaimTypes.Name,customer.FirstName+" "+customer.LastName),
                new Claim(JwtClaimTypes.Email,customer.Email),
                new Claim(JwtClaimTypes.GivenName,customer.FirstName),
                new Claim(JwtClaimTypes.FamilyName,customer.LastName),
                new Claim(JwtClaimTypes.Role,Constants.Customer)
            }).Result;


        }
    }
}
