using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Site.Interfaces;
using Site.Models.Dtos.Users;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Text;


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

    [HttpPut]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto, [FromServices] IUserAuthInterfaces icreateUsr)
    {

        if (ModelState.IsValid)
        {
            try
            {
                
                //var jwtToken = Request.Cookies["AuthToken"];

                /*
                HttpClient client = new();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

                HttpResponseMessage response = await client.PostAsync(api, content);
                */

                var response = await icreateUsr.ChangePasswordAsync(dto);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Home");
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
        ViewData["ErrorMessage"] = "Pão doce";
        return View();
    }
}
