using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
    public async Task<IActionResult> RegisterUser(UserRegisterDto dto, [FromServices] IUserAuthInterfaces icreateUsr)
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
    public async Task<IActionResult> LoginUser(UserLoginDto dto, [FromServices] IUserAuthInterfaces iloginUsr)
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
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTimeOffset.UtcNow.AddHours(1)
                    };

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

    [HttpPut]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto, [FromServices] IUserAuthInterfaces icreateUsr)
    {

        if (ModelState.IsValid)
        {
            try
            {
                if (Request.Cookies.TryGetValue("AuthToken", out string tokenValue))
                {
                    var response = await icreateUsr.ChangePasswordAsync(dto, tokenValue);

                    var responseContent = response.Content;

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        ViewData["ErrorMessage"] = "Unauthorized to change the password";
                    }
                    else
                    {
                        ViewData["ErrorMessage"] = response.StatusCode;
                        return View();
                    }
                }
                else
                {
                    ViewData["ErrorMessage"] = "Authentication token not found.";
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
    public IActionResult DeleteUser()
    {
        return View();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteUser(DeleteUserDto dto, [FromServices] IUserAuthInterfaces ideleteUrs)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var response = await ideleteUrs.DeleteUserAsync(dto);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewData["ErrorMessage"] = "Unauthorized to delete account.";
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
