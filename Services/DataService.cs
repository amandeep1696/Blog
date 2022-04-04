using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheBlogProject.Data;
using TheBlogProject.Enums;
using TheBlogProject.Models;

namespace TheBlogProject.Services
{
    public class DataService
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<BlogUser> _userManager;
        private readonly IImageService _imageService;
        private readonly IConfiguration _configuration;

        public DataService(ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<BlogUser> userManager,
            IImageService imageService,
            IConfiguration configuration)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _imageService = imageService;
            _configuration = configuration;
        }

        public async Task ManageDataAsync()
        {
            await _context.Database.MigrateAsync();
            await SeedRolesAsync();
            await SeedUsersAsync();
        }

        private async Task SeedRolesAsync()
        {
            if (_context.Roles.Any())
            {
                // DB already seeded
                return;
            }

            foreach (var role in Enum.GetNames(typeof(BlogRole)))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        private async Task SeedUsersAsync()
        {
            if (_context.Users.Any())
            {
                // Already seeded
                return;
            }

            var adminUser = new BlogUser()
            {
                Email = "admin1@email.com",
                UserName = "admin1@email.com",
                FirstName = "Admin",
                LastName = "One",
                DisplayName = "Admin One",
                GitHubUrl = "https://github.com/amandeep1696",
                PersonalUrl = "https://www.linkedin.com/",
                EmailConfirmed = true,
                ImageData = await _imageService.EncodeImageAsync(_configuration["DefaultUserImage"]),
                ContentType = "image/jpeg"
            };

            await _userManager.CreateAsync(adminUser, "Password123!");
            await _userManager.AddToRoleAsync(adminUser, BlogRole.Administrator.ToString());

            var modUser = new BlogUser()
            {
                Email = "mod1@email.com",
                UserName = "mod1@email.com",
                FirstName = "Mod",
                LastName = "One",
                DisplayName = "Mod One",
                GitHubUrl = "https://github.com/amandeep1696",
                PersonalUrl = "https://www.linkedin.com/",
                EmailConfirmed = true,
                ImageData = await _imageService.EncodeImageAsync(_configuration["DefaultUserImage"]),
                ContentType = "image/jpeg"
            };

            await _userManager.CreateAsync(modUser, "Password123!");
            await _userManager.AddToRoleAsync(modUser, BlogRole.Moderator.ToString());
        }
    }
}
