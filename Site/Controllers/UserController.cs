using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using Site.Models;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Serialization;

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
}
