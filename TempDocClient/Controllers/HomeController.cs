using System.IO;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TempDocClient.Models;
using WebContract.Interfaces;
using WebContract.Models.Stored;
using WebContract.Models.Request;

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

    public async Task<StoredFileInfo> UpdateExtraInfo(string bucket, string code, FileDtoRequest extra) => 
        await _service.SetExtraSettings(bucket, code, extra);

    [HttpPost]
    public async Task<List<StoredFileInfo>> UploadFiles(string bucket, IFormFileCollection files) => 
        await _service.UploadFiles(bucket, files);

    public async Task<IFormFile> GetFile(string bucket, string code) {
        var (filename, stream) = await _service.GetFile(bucket, code);

        return new FormFile(stream, 0, stream.Length, filename,filename);
    }

    public async Task<List<StoredFileInfo>> GetFolder(string bucket) => await _service.GetBucket(bucket);

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
