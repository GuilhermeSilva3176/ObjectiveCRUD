using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Site.Models.Dtos.Users;
using System.Text;


namespace Site.Controllers;

public class UserController : Controller
{
    public IActionResult RegisterUser()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> RegisterUser(UserRegisterDto dto)
    {

        if (ModelState.IsValid)
        {
            try
            {
                string api = "https://localhost:7299/api/User/Create";

                string json = JsonConvert.SerializeObject(dto);

                var content = new StringContent(json, Encoding.UTF8, "application/json"); 

                HttpClient client = new();
                HttpResponseMessage response = await client.PostAsync(api, content);

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
    public async Task<IActionResult> LoginUser(UserLoginDto dto)
    {
        if (ModelState.IsValid)
        {
            try
            {
                string api = "https://localhost:7299/api/User/Login";
                string json = JsonConvert.SerializeObject(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpClient client = new();
                HttpResponseMessage response = await client.GetAsync($"{api}?email={dto.Email}&password={dto.Password}");

                if (response.IsSuccessStatusCode) 
                {
                    var token = await response.Content.ReadAsStringAsync();

                    Response.Cookies.Append("AuthToken", token);
                    return RedirectToAction("Index", "Home");
                }
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
}
