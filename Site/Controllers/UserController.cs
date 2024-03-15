using Microsoft.AspNetCore.Mvc;
using Site.Interfaces;
using Site.Models.Dtos.Users;
using System.Net;



namespace Site.Controllers;

public class UserController : Controller
{

    public IActionResult RegisterUser()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> RegisterUser(UserRegisterDto dto, [FromServices] IUserNoAuthInterfaces icreateUsr)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var response = await icreateUsr.RegisterAsync(dto);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("Index", "Home");
                else
                    ViewData["ErrorMessage"] = "Error registering user. API returned non-success status code.";
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = "An error occurred while processing your request ";
            }
        }
        return View();
    }

    public IActionResult LoginUser()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> LoginUser(UserLoginDto dto, [FromServices] IUserNoAuthInterfaces iloginUsr)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var response = await iloginUsr.LoginAsync(dto);

                if (response.IsSuccessStatusCode)
                {
                    var token = response.Content;

                    CookieOptions cookieOptions = new()
                    {
                        Secure = true,
                        HttpOnly = true,
                        SameSite = SameSiteMode.Strict
                    };

                    cookieOptions.Expires = DateTimeOffset.UtcNow.AddHours(1);

                    Response.Cookies.Append("AuthToken", token, cookieOptions);
                    
                    return RedirectToAction("Index", "Home");
                }
                else 
                { 
                    ViewData["ErrorMessage"] = "Error registering user. API returned non-success status code.";
                }
            } 
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = "An error occurred while processing your request ";
            }
        }
        return View();
    }

    public IActionResult Loggout()
    {
        Response.Cookies.Delete("AuthToken");
        return RedirectToAction("Index", "Home");
    }

    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto, [FromServices] IUserAuthInterfaces icreateUsr)
    {

        if (ModelState.IsValid)
        {
            try
            {
                var token = Request.Cookies["AuthToken"];
                var response = await icreateUsr.ChangePasswordAsync(token, dto);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Home");
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    ViewData["ErrorMessage"] = "Unauthorized to change password.";
                }
                else
                {
                    ViewData["ErrorMessage"] = "Failed to change password.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = "An error ocourred whiling processing your request";
            }
        }
        return View();
    }
}
