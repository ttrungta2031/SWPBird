using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPBirdBoarding.Models;
using SWPBirdBoarding.Common;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using SWPBirdBoarding.ImplementServices;

namespace SWPBirdBoarding.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private Random randomGenerator = new Random();
        private readonly IMailService _mailService;
        private readonly IConfiguration _config;
        private readonly SWPBirdBoardingContext _context;
        private static string apiKey = "AIzaSyBw4dBuBoJV7jZSdDCqb8SlrHsFXXTLmbU";

        private static string Bucket = "swpbirdboarding.appspot.com";
        private static string AuthEmail = "admin1@gmail.com";
        private static string AuthPassword = "admin1234";
        public AccountsController(IConfiguration config, SWPBirdBoardingContext context, IMailService mailService)
        {
            _context = context;
            _mailService = mailService;
            _config = config;
        }

        // GET: api/Accounts
        /// <summary>
        /// lấy danh sách các tài khoản
        /// </summary>
        [HttpGet("GetAccountList")]
   
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts(string search, int pagesize = 10, int pagenumber = 1)
        {
            var result = (from s in _context.Accounts
                          where s.Role != "Admin"
                          select new
                          {
                              Id = s.Id,
                              ImageUrl = s.ImageUrl,
                              Email = s.Email,
                              FullName = s.FullName,
                              HostName = s.BirdShelters.Select(s => s.Name).FirstOrDefault(),
                              Dob = s.Dob,
                              Telephone = s.Telephone,
                              Address = s.Address,
                              Role = s.Role,
                              Code = s.Code,
                              DateCreate = s.DateCreate,
                              Status = s.Status

                          }).ToList();
            if (!string.IsNullOrEmpty(search))
            {
                result = (from s in _context.Accounts
                          where s.Email.Contains(search)  && s.Role != "Admin" || s.FullName.Contains(search) && s.Role != "Admin" || s.Dob.ToString().Contains(search) && s.Role != "Admin" || s.Address.Contains(search) && s.Role != "Admin" || s.Telephone.Contains(search) && s.Role != "Admin"
                          select new
                          {
                              Id = s.Id,
                              ImageUrl = s.ImageUrl,
                              Email = s.Email,
                              FullName = s.FullName,
                              HostName = s.BirdShelters.Select(s => s.Name).FirstOrDefault(),
                              Dob = s.Dob,
                              Telephone = s.Telephone,
                              Address = s.Address,
                              Role = s.Role,
                              Code = s.Code,
                              DateCreate = s.DateCreate,
                              Status = s.Status

                          }).ToList();
            }

            var paging = result.Skip((pagenumber - 1) * pagesize).Take(pagesize).ToList();
            double totalpage1 = (double)result.Count() / pagesize;
            totalpage1 = Math.Ceiling(totalpage1);
            return Ok(new { StatusCode = 200, Content = "Load successful", Data = paging, totalpage = totalpage1 , pagesize = pagesize, pagenumber = pagenumber });



        }

        // GET: api/Accounts/5
        /// <summary>
        /// lấy thông tin chi tiết tài khoản.
        /// </summary>
        [HttpGet("Getbyid")]
        public async Task<ActionResult<Account>> GetAccountById(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null || id < 1)
            {
                return StatusCode(409, new { StatusCode = 409, Message = "The Account not exist" });
            }

            var result = (from s in _context.Accounts
                          where s.Id == id
                          select new
                          {
                              Id = s.Id,
                              Email = s.Email,
                              FullName = s.FullName,
                              Dob = s.Dob,
                              Telephone = s.Telephone,
                              Address = s.Address,
                              Role = s.Role,
                              Status = s.Status,
                              Code = s.Code

                          }).ToList();
           

            return Ok(new { StatusCode = 200, Message = "Load successful", data = result });
        }
        public class AccountInforUpdate
        {
                public int? Id { get; set; }
            public string ImageUrl { get; set; }
            public string FullName { get; set; }
            public DateTime? Dob { get; set; }
            public string Telephone { get; set; }
            public string Address { get; set; }

        }
        // PUT: api/Accounts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("EditAccount")]
        public async Task<IActionResult> EditAccount(AccountInforUpdate account)
        {
            try
            {
                var acc = await _context.Accounts.FindAsync(account.Id);
                //     var acc = GetAccount_byEmail(account.Email);
                if (acc == null)
                {
                    return StatusCode(409, new { StatusCode = 409, Message = "Not Found The Account!" });
                }
                acc.ImageUrl = account.ImageUrl == null ? acc.ImageUrl : account.ImageUrl;
                acc.FullName = account.FullName == null ? acc.FullName : account.FullName;
                acc.Dob = account.Dob == null ? acc.Dob : account.Dob;
                acc.Telephone = account.Telephone == null ? acc.Telephone : account.Telephone;
                acc.Address = account.Address == null ? acc.Address : account.Address;
                _context.Accounts.Update(acc);
                await _context.SaveChangesAsync();

                return Ok(new { StatusCode = 200, Message = "Update Successfull" });

            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }



        public class ChangePasswordDetail
        {
            public int? Id { get; set; }
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }

        }
        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDetail account)
        {
            try
            {
                var acc = await _context.Accounts.FindAsync(account.Id);
                //     var acc = GetAccount_byEmail(account.Email);
                if (acc == null)
                {
                    return StatusCode(409, new { StatusCode = 409, Message = "Not Found The Account!" });
                }
                if(acc.Password == account.OldPassword)
                {
                    acc.Password = account.NewPassword;
                }
                else return StatusCode(409, new { StatusCode = 409, Message = "Old Password not correct!" });

                _context.Accounts.Update(acc);
                await _context.SaveChangesAsync();

                return Ok(new { StatusCode = 200, Message = "Change Password Successfull" });

            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }




        public class MemberAccount
        {
            public string ImageUrl { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string FullName { get; set; }
            public string Telephone { get; set; }
            public string Address { get; set; }

        }
        [HttpPost("CreateAccountMember")]
        public async Task<IActionResult> CreateAccountMember(MemberAccount account)
        {
            try
            {
                var isHasAccount = GetAccount_byEmail(account.Email);
                if (isHasAccount != null) return StatusCode(409, new { StatusCode = 409, message = "Email has already been!" });

                var newAccount = new Models.Account();
                {
                    newAccount.ImageUrl = account.ImageUrl;
                    newAccount.Email = account.Email;
                    newAccount.Password = account.Password;
                    newAccount.FullName = account.FullName;   
                    newAccount.Telephone = account.Telephone;
                    newAccount.Address = account.Address;
                    newAccount.Role = "Member";
                    newAccount.DateCreate = DateTime.UtcNow.AddHours(7);
                    newAccount.Status = "Inactive";
                }
                _context.Accounts.Add(newAccount);
                await _context.SaveChangesAsync();


                var b = GetAccount_byEmail(newAccount.Email);
                await sendCodeEmail(b);


                return Ok(new { StatusCode = 200, Message = "Create Host Account Successfully, Please verify account by code!" });

            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }





        // POST: api/Accounts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// tạo tài khoản cho chủ lưu trú
        /// </summary>
        [HttpPost("CreateAccountHost")]
        public async Task<IActionResult> CreateAccountHost(HostAccountInfor account)
        {
            try
            {
                var isHasAccount = GetAccount_byEmail(account.Email);
                if (isHasAccount != null) return StatusCode(409, new { StatusCode = 409, message = "Email has already been!" });

                var newAccount = new Models.Account();
                {
                 
                    newAccount.Email = account.Email;
                    newAccount.Password = account.Password;
                    newAccount.FullName = account.Name;
                    newAccount.Type = "Cơ sở lưu trú";                  
                    newAccount.Role = "Host";
                    newAccount.DateCreate = DateTime.UtcNow.AddHours(7);
                    newAccount.Status = "Active";
    }
                _context.Accounts.Add(newAccount);
                await _context.SaveChangesAsync();

                var accountnew = _context.Accounts.Max(it => it.Id);

                var newbirdshelter = new BirdShelter();
                {
                    newbirdshelter.AccountId = accountnew;
                    newbirdshelter.Name = account.Name;
                    newbirdshelter.ImageUrl = account.ImageUrl;

                    newbirdshelter.Type = "Cơ sở lưu trú";
                    newbirdshelter.Description = account.Description;
                    newbirdshelter.Status = "Active";
                }
                _context.BirdShelters.Add(newbirdshelter);
                await _context.SaveChangesAsync();







                return Ok(new { StatusCode = 200, Message = "Create Host Account Successfully" });

            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }
        public class Login
        {
            public string Email { get; set; }
            public string Password { get; set; }

        }

        /// <summary>
        /// đăng nhập bằng chủ lưu trú
        /// </summary>
        [HttpPost("LoginByHost")]
        public async Task<IActionResult> LoginByHost(Login account)
        {
            var user = GetAccount_byEmail(account.Email);
            if (user == null) return StatusCode(409, new { StatusCode = 409, Message = "The Account not exist or Invalid email/password!!" });
            var hasusertest = _context.Accounts.SingleOrDefault(p => (p.Email.ToUpper() == account.Email.ToUpper()) && account.Password == p.Password && p.Status == "active");
            if (hasusertest == null)
            {
                return StatusCode(409, new { StatusCode = 409, Message = "The Account not exist or Invalid email/password!!" });
            }    
            if (user != null)
            {
                var hasuser = _context.Accounts.SingleOrDefault(p => (p.Email.ToUpper() == account.Email.ToUpper()) && account.Password == p.Password && p.Status == "active" && p.Role =="Host");
                if (hasuser == null)
                {
                    return StatusCode(409, new { StatusCode = 409, Message = "The Account not exist(Host) or Invalid email/password!!" });
                }

                if (hasuser != null)
                {
                    if (user.Status == "Inactive") return StatusCode(409, new { StatusCode = 409, Message = "The Account has inactive" });
                    var token = GenerateJwtToken(hasuser);
                    //  string tokenfb = authen.FirebaseToken;


                    return Ok(new { StatusCode = 200, Message = "Login with role host successful", AccountId = user.Id, FullName = user.FullName, JWTTOKEN = token, Role = user.Role });


                }
            }
            return StatusCode(409, new { StatusCode = 409, Message = "The Account not exist or Invalid email/password!!" });
        }

        /// <summary>
        /// đăng nhập bằng tài khoản member
        /// </summary>
        [HttpPost("LoginByMember")]
        public async Task<IActionResult> LoginByMember(Login account)
        {
            var user = GetAccount_byEmail(account.Email);
            if (user == null) return StatusCode(409, new { StatusCode = 409, Message = "The Account not exist or Invalid email/password!!" });
            var hasusertest = _context.Accounts.SingleOrDefault(p => (p.Email.ToUpper() == account.Email.ToUpper()) && account.Password == p.Password && p.Status == "active");
            if (hasusertest == null)
            {
                return StatusCode(409, new { StatusCode = 409, Message = "The Account not exist or Invalid email/password!!" });
            }
            if (user != null)
            {
                var hasuser = _context.Accounts.SingleOrDefault(p => (p.Email.ToUpper() == account.Email.ToUpper()) && account.Password == p.Password && p.Status == "active" && p.Role == "Member");
                if (hasuser == null)
                {
                    return StatusCode(409, new { StatusCode = 409, Message = "The Account not exist(Member) or Invalid email/password!!" });
                }

                if (hasuser != null)
                {
                    if (user.Status == "Inactive") return StatusCode(409, new { StatusCode = 409, Message = "The Account has inactive" });
                    var token = GenerateJwtToken(hasuser);
                    //  string tokenfb = authen.FirebaseToken;


                    return Ok(new { StatusCode = 200, Message = "Login with role Member successful", AccountId = user.Id, FullName = user.FullName, JWTTOKEN = token, Role = user.Role });


                }
            }
            return StatusCode(409, new { StatusCode = 409, Message = "The Account not exist or Invalid email/password!!" });
        }



        /*  [HttpPost("LoginAdmin")]
          public async Task<IActionResult> LoginToSystemByAdmin(Login account)
          {
              var user = GetAccount_byEmail(account.Email);
              if (user == null) return StatusCode(409, new { StatusCode = 409, Message = "The Account not exist or Invalid email/password!!" });
              var hasusertest = _context.Accounts.SingleOrDefault(p => (p.Email.ToUpper() == account.Email.ToUpper()) && account.Password == p.Password && p.Status == "active");
              if (hasusertest == null)
              {
                  return StatusCode(409, new { StatusCode = 409, Message = "The Account not exist or Invalid email/password!!" });
              }
              if (user != null)
              {
                  var hasuser = _context.Accounts.SingleOrDefault(p => (p.Email.ToUpper() == account.Email.ToUpper()) && account.Password == p.Password && p.Status == "active" && p.Role=="Admin");
                  if (hasuser == null)
                  {
                      return StatusCode(409, new { StatusCode = 409, Message = "The Account not exist or Invalid email/password!!" });
                  }

                  if (hasuser != null)
                  {
                      if (user.Status == "inactive") return StatusCode(409, new { StatusCode = 409, Message = "The Account is not exited or not verified" });
                      var token = GenerateJwtToken(hasuser);
                      //  string tokenfb = authen.FirebaseToken;


                      return Ok(new { StatusCode = 200, Message = "Login with role admin successful", AccountId = user.Id, JWTTOKEN = token, Role = user.Role, Name = user.FullName });


                  }
              }
              return StatusCode(409, new { StatusCode = 409, Message = "The Account not exist or Invalid email/password!!" });
          }
  */
       


        public class HostAccountInfor
        {

            public string Email { get; set; }
            public string Password { get; set; }
            public string Name { get; set; }
            public string ImageUrl { get; set; }
            public string Description { get; set; }

        }
        [HttpPost("ResendCode")]  //email  
        public async Task<IActionResult> ResendCode(string email)
        {
            try
            {
                email = addTailEmail(email);
                if (!Validate.isEmail(email))
                {
                    return StatusCode(409, new { StatusCode = 409, message = "Exception Email format" });
                }                     
                var a = GetAccount_byEmail(email);
                if (a != null)
                {
                    await sendCodeEmail(a);
                    return StatusCode(200, new { StatusCode = 200, message = "Email re-send" });
                }
             
               // await sendCodeEmail(a);
                return StatusCode(409, new { StatusCode = 409, message = "Account resend failed" });
            }
            catch (Exception ex)
            {
                return StatusCode(409, new { StatusCode = 409, message = "Account resend failed (" + ex.Message + ")" });
            }
        }
        public class ConfirmModel
        {
            public string Email { get; set; }
            public string Code { get; set; }

        
        }




        [HttpPut("ConfirmCode")]
        public async Task<IActionResult> submitCode(ConfirmModel account)
        {
            try
            {
                account.Email = addTailEmail(account.Email);
                if (!Validate.isEmail(account.Email))
                {
                    return StatusCode(409, new { StatusCode = 409, message = "Exception Email format" });//ok
                }

                var a = GetAccount_byEmail(account.Email);
                if (a != null)
                {
                    if (a.Status == "Active")
                    {
                        return StatusCode(409, new { StatusCode = 409, message = "Account aldready active" });//ok
                    }
                    else
                    {
                        if (a.Code == account.Code)
                        {
                            a.Status = "Active";
                            a.Email = account.Email;
                            _context.Accounts.Update(a);

                            await _context.SaveChangesAsync();

                            return Ok(new { StatusCode = 200, Message = "Email verification successfully" });
                        }
                        else
                        {
                            return StatusCode(400, new { StatusCode = 400, message = "Email verification failed (incorrect code)" }); //ok
                        }
                    }
                }
                return StatusCode(400, new { StatusCode = 400, message = "Email verification failed (account does not exist)" });//ok
            }
            catch (Exception ex)
            {
                return StatusCode(409, new { StatusCode = 409, message = "Account registered failed (" + ex.Message + ")" });//ok
            }
        }
        public class ForgotPassword
        {
            public string Email { get; set; }
            public string Code { get; set; }

            public string Password { get; set; }

        }


        [HttpPut("ForgotPassword")] //email, passhash, code   ok
        public async Task<IActionResult> changePass(ForgotPassword account)
        {
            try
            {
             
                account.Email = addTailEmail(account.Email);
                if (!Validate.isEmail(account.Email))
                {
                    return StatusCode(409, new { StatusCode = 409, message = "Exception Email format" });//ok
                }

                var a = GetAccount_byEmail(account.Email);
             
                if (a != null)
                {

                    await sendCodeEmail(a);

                    if (a.Code == account.Code)
                    {
                      
                        a.Password = account.Password;
                        _context.Accounts.Update(a);

                        await _context.SaveChangesAsync();

                        return Ok(new { StatusCode = 200, Message = "Password change successfully" });//ok
                    }
                    else
                    {
                        return StatusCode(400, new { StatusCode = 400, message = "Password change failed (incorrect code)" });//ok
                    }
                }
                return StatusCode(400, new { StatusCode = 400, message = "Password changen failed (account does not exist)" });//ok
            }
            catch (Exception ex)
            {
                return StatusCode(409, new { StatusCode = 409, message = "Password changen failed (" + ex.Message + ")" });//ok
            }
        }




        public class UserInfor
        {
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }

           // public int Id { get; set; }
        }

      /*  [HttpPut("ChangePassById")]
        public async Task<IActionResult> ChangePassWordUserByUserName(int id ,[FromBody] UserInfor userinfor)
        {
            var account = _context.Accounts.Where(a => a.Id == id).FirstOrDefault();
            if (account == null || id < 1)
            {
                return StatusCode(400, new { StatusCode = 400, Message = "Not Found Account!" });
            }
            if (account.Password == userinfor.OldPassword)
            {
                account.Password = userinfor.NewPassword;
                _context.Accounts.Update(account);
                await _context.SaveChangesAsync();
            }
            else return StatusCode(400, new { StatusCode = 400, Message = "Wrong information format, old password is incorrect!" });

            return Ok(new { StatusCode = 200, Content = "The Password of Account was changed successfully!!" });
        }*/




        // DELETE: api/Accounts/5
        [HttpPut("ChangeStatus")]
        public async Task<IActionResult> ChangeStatusAccount(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            if (account.Status == "Active") 
            {
                account.Status = "Inactive";
                _context.Accounts.Update(account);
                await _context.SaveChangesAsync();
                return Ok(new { StatusCode = 200, Content = "The account was changes status inactive successfully!!" });
            }
            else { 
                account.Status = "Active";
                _context.Accounts.Update(account);
                await _context.SaveChangesAsync();
                return Ok(new { StatusCode = 200, Content = "The account was changes status active successfully!!" });
            }
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();

            return Ok(new { StatusCode = 200, Content = "The account was changes status successfully!!" });
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.Id == id);
        }

        private Models.Account GetAccount_byEmail(string email)
        {
            var account = _context.Accounts.Where(a => a.Email.ToUpper() == email.ToUpper()).FirstOrDefault();

            if (account == null)
            {
                return null;
            }

            return account;
        }


        private async Task<bool> sendCodeEmail(Models.Account a)
        {
            try
            {

                a.Code = randomGenerator.Next(100000, 999999).ToString(); //moi lan resend la reset code
                _context.Accounts.Update(a);
                await _context.SaveChangesAsync();

                var mailRequest = new MailRequest();
                mailRequest.ToEmail = a.Email;
                var username = mailRequest.ToEmail.Split('@')[0];
                mailRequest.Subject = " Welcome to Bird Boarding System";
                mailRequest.Description = "Your Verify code: " + "    " + a.Code;
                mailRequest.Value = a.Code;
                await _mailService.SendEmailAsync(mailRequest.ToEmail, mailRequest.Subject, mailRequest.Description, mailRequest.Value);
                Console.WriteLine(mailRequest.ToEmail);
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }


        private string addTailEmail(string email)
        {
            if (!email.Contains("@")) //auto add ".com"              
            {
                return email + "@gmail.com";
            }
            return email;
        }


        private string GenerateJwtToken(Models.Account user)
        {
            //string role = "manager";
            var useracc = GetAccount_byEmail(user.Email);


            var securitykey = Encoding.UTF8.GetBytes(_config["Jwt:Secret"]);
            var claims = new Claim[] {
                new Claim(ClaimTypes.Name, useracc.Id.ToString()),
                new Claim(ClaimTypes.Email, useracc.Email),
                new Claim(ClaimTypes.Role, useracc.Role),

            };

            var credentials = new SigningCredentials(new SymmetricSecurityKey(securitykey), SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
