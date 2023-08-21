using Firebase.Auth;
using Firebase.Storage;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SWPBirdBoarding.Models;
using SWPBirdBoarding.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SWPBirdBoarding.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class FirebaseServicesController : ControllerBase
    {

        private readonly IHostingEnvironment _env;
        private readonly IConfiguration _config;
        private readonly SWPBirdBoardingContext _context;
        //private readonly ISendNotiService _sendnoti;
        //   private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private static string apiKey = "AIzaSyBw4dBuBoJV7jZSdDCqb8SlrHsFXXTLmbU";

        private static string Bucket = "swpbirdboarding.appspot.com";
        private static string AuthEmail = "admin1@gmail.com";
        private static string AuthPassword = "admin1234";
        public FirebaseServicesController(IConfiguration config, SWPBirdBoardingContext context, IHostingEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _config = config;
            _context = context;
            _env = env;
            _httpContextAccessor = httpContextAccessor;

        }
        [HttpPost("uploadfile")]
        public async Task<IActionResult> Upload(IFormFile file)
        {

            var fileupload = file;
            FileStream fs = null;
            if (fileupload.Length > 0)
            {
                {
                    string foldername = "firebaseFiles";
                    string path = Path.Combine($"Images", $"Images/{foldername}");


                    if (Directory.Exists(path))
                    {

                        using (fs = new FileStream(Path.Combine(path, fileupload.FileName), FileMode.Create))
                        {

                            await fileupload.CopyToAsync(fs);
                        }

                        fs = new FileStream(Path.Combine(path, fileupload.FileName), FileMode.Open);


                    }


                    else
                    {
                        Directory.CreateDirectory(path);
                    }
                }
                var auth = new FirebaseAuthProvider(new FirebaseConfig(apiKey));

                var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);


                var cancel = new CancellationTokenSource();

                var upload = new FirebaseStorage(
                    Bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true
                    }
                    ).Child("images").Child(fileupload.FileName).PutAsync(fs, cancel.Token);

                // await upload;
                try
                {
                    string link = await upload;

                    return Ok(new { StatusCode = 200, Message = "Upload FIle success", data = link });
                }
                catch (Exception ex)
                {
                    throw;
                }

            }


            return BadRequest("Failed Upload");
        }




    }
}
