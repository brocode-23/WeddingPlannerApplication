﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WeddingPlannerApplication.Data;
using WeddingPlannerApplication.Models;
using WeddingPlannerApplication.ViewModels;

namespace WeddingPlannerApplication.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult RegisterCouple()
        {
            return View();
        }

        //[HttpGet]
        //public IActionResult Login()
        //{
        //    return View();
        //}

        [HttpPost]
        public async Task<IActionResult> RegisterCouple(RegisterCoupleModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                if (!await _roleManager.RoleExistsAsync("Couple"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Couple"));
                }

                var groom = new ApplicationUser
                {
                    UserName = model.GroomEmail,
                    Email = model.GroomEmail,
                    FirstName = model.GroomFirstName,
                    LastName = model.GroomLastName,
                    PhoneNumber = model.GroomPhone,
                    Role = "Couple"
                };

                var bride = new ApplicationUser
                {
                    UserName = model.BrideEmail,
                    Email = model.BrideEmail,
                    FirstName = model.BrideFirstName,
                    LastName = model.BrideLastName,
                    PhoneNumber = model.BridePhone,
                    Role = "Couple"
                };

                var groomResult = await _userManager.CreateAsync(groom, model.GroomPassword);
                if (!groomResult.Succeeded)
                {
                    foreach (var error in groomResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }

                var brideResult = await _userManager.CreateAsync(bride, model.BridePassword);
                if (!brideResult.Succeeded)
                {
                    foreach (var error in brideResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }

                await _userManager.AddToRoleAsync(groom, "Couple");
                await _userManager.AddToRoleAsync(bride, "Couple");

                var couple = new Couple
                {
                    WeddingDate = model.WeddingDate,
                    Budget = model.Budget,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                _context.Couples.Add(couple);
                await _context.SaveChangesAsync();

                var groomMember = new CoupleMember
                {
                    CoupleId = couple.Id,
                    UserId = groom.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                var brideMember = new CoupleMember
                {
                    CoupleId = couple.Id,
                    UserId = bride.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                _context.CoupleMembers.Add(groomMember);
                _context.CoupleMembers.Add(brideMember);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Couple registered successfully. You can now log in.";
                return Redirect("~/Identity/Account/Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> Login(LoginModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest("Invalid login details.");
        //    }

        //    try
        //    {
        //        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

        //        if (!result.Succeeded)
        //        {
        //            return Unauthorized("Invalid credentials.");
        //        }

        //        return Ok("Login successful.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return Redirect("~/");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult RegisterPlanner()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterPlanner(RegisterPlannerModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                if (!await _roleManager.RoleExistsAsync("Planner"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Planner"));
                }

                var planner = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.Phone,
                    Role = "Planner",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                var result = await _userManager.CreateAsync(planner, model.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }

                await _userManager.AddToRoleAsync(planner, "Planner");

                TempData["SuccessMessage"] = "Planner registered successfully. You can now log in.";
                return Redirect("~/Identity/Account/Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }
    }
}
