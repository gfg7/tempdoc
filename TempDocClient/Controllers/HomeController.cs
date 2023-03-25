using System.Diagnostics;
using gRPCContract.Interfaces;
using Microsoft.AspNetCore.Mvc;
using TempDocClient.Models;

namespace TempDocClient.Controllers;

public class HomeController : Controller
{
    private readonly IClientBucketManagement _service;

    public HomeController(IClientBucketManagement service)
    {
        _service = service;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> GetFolder(string bucket) => await _service.GetBucket(bucket);

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
