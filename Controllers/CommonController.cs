using API.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Net.Http.Headers;

namespace API.Controllers
{
    /// <summary>
    /// 通用相关
    /// </summary>
    [Produces("application/json")]
    [Route("[controller]")]
    public class CommonController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        readonly ICurrentUser _userAccount;
        public CommonController(IWebHostEnvironment webHostEnvironment, ICurrentUser userAccount)
        {
            _webHostEnvironment = webHostEnvironment;
            _userAccount = userAccount;
        }
        [HttpPost("Upload"), DisableRequestSizeLimit]
        public ActionResult UploadFile()
        {
            var file = Request.Form.Files[0];
            var folderName = "files";
            var path = Path.Combine(_webHostEnvironment.ContentRootPath, folderName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (file.Length > 0)
            {
                var uploadFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                var fileName = DateTime.Now.ToString("yyyyMMddHHmmss_") + uploadFileName;
                var filePath = path + "//" + fileName;
                using (var stream = System.IO.File.Create(filePath))
                {
                    file.CopyTo(stream);
                }
                return Ok(new { url = folderName + "/" + fileName, name = uploadFileName });
            }
            else
            {
                return BadRequest();
            }
        }
    }
}