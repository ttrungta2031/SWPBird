using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPBirdBoarding.Models;

namespace SWPBirdBoarding.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly SWPBirdBoardingContext _context;

        public ServicesController(SWPBirdBoardingContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Lấy thông tin danh sách dịch vụ bởi chủ lưu trú
        /// </summary>
        [HttpGet("GetServiceList")]

        public async Task<ActionResult<IEnumerable<Service>>> GetService(int id,string search, int pagesize = 10, int pagenumber = 1)
        {

            var isHasAccount = await _context.Accounts.FindAsync(id);



            if (isHasAccount == null || id < 1)
            {
                return StatusCode(409, new { StatusCode = 409, Message = "The Account not exist" });
            }

            var hostAccount = _context.BirdShelters.SingleOrDefault(p => p.AccountId == id);

            var result = (from s in _context.Services
                      where s.BirdShelterId == hostAccount.Id
                          select new
                          {
                              Id = s.Id,
                              Name = s.Name,
              Type = s.Type,
            Unit = s.Unit,
            Price = s.Price,
            Amount = s.Amount,
            Status = s.Status,
            DateChange = s.DateChange,
            Description = s.Description
            
                          }).ToList();
            if (!string.IsNullOrEmpty(search))
            {
                result = (from s in _context.Services
                          where s.BirdShelterId == hostAccount.Id && s.Name.Contains(search)
                          select new
                          {
                              Id = s.Id,
                              Name = s.Name,
                              Type = s.Type,
                              Unit = s.Unit,
                              Price = s.Price,
                              Amount = s.Amount,
                              Status = s.Status,
                              DateChange = s.DateChange,
                              Description = s.Description
                          }).ToList();
            }

            var paging = result.Skip((pagenumber - 1) * pagesize).Take(pagesize).ToList();
            double totalpage1 = (double)result.Count() / pagesize;
            totalpage1 = Math.Ceiling(totalpage1);
            return Ok(new { StatusCode = 200, Content = "Load successful", Data = paging, totalpage = totalpage1, pagesize = pagesize, pagenumber = pagenumber });



        }
        public class Servicebyhost
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public string Type { get; set; }
            public string Unit { get; set; }
            public double Price { get; set; }

            public int Amount { get; set; }

            public string Description { get; set; }

        }
        /// <summary>
        /// thêm mới dịch vụ bởi chủ lưu trú
        /// </summary>
        [HttpPost("CreateService")]
        public async Task<IActionResult> CreateAccountHost(Servicebyhost host)
        {
            try
            {
                var isHasAccount = await _context.Accounts.FindAsync(host.Id);
             


                if (isHasAccount == null || host.Id < 1)
                {
                    return StatusCode(409, new { StatusCode = 409, Message = "The Account not exist" });
                }

                var hostAccount = _context.BirdShelters.SingleOrDefault(p => p.AccountId == host.Id);

                var newService = new Models.Service();
                {
                    newService.Name = host.Name;
                    newService.Type = host.Type;
                    newService.Unit = host.Unit;
                    newService.Price = host.Price;
                    newService.Amount = host.Amount;
                    newService.Status = "active";
                    newService.DateChange = DateTime.UtcNow.AddHours(7);
                    newService.Description = host.Description;
                    newService.BirdShelterId = hostAccount.Id;
                }
                _context.Services.Add(newService);
                await _context.SaveChangesAsync();
                return Ok(new { StatusCode = 200, Message = "Create new Service Successfully" });

            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }




        public class Hostdetail
        {
            public int HostId { get; set; }
            public string Name { get; set; }
            public string ImageUrl { get; set; }
            public string Type { get; set; }
            public string Description { get; set; }

        }
        [HttpPut("UpdateHostDetail")]
        public async Task<IActionResult> UpdateReport(Hostdetail host)
        {
            try
            {
                //   var isbookingid = await _context.Bookings.FindAsync(report.BookingId);
                var ishostid = _context.BirdShelters.SingleOrDefault(p => p.Id == host.HostId);


                if (ishostid == null || host.HostId < 1)
                {
                    return StatusCode(409, new { StatusCode = 409, Message = "The Hostid is not exist" });
                }

                ishostid.Name = host.Name == null ? ishostid.Name : host.Name;
                ishostid.ImageUrl = host.ImageUrl == null ? ishostid.ImageUrl : host.ImageUrl;
                ishostid.Type = host.Type == null? ishostid.Type : host.Type;
                ishostid.Description = host.Description == null ? ishostid.Description : host.Description;


                _context.BirdShelters.Update(ishostid);
                await _context.SaveChangesAsync();
                return Ok(new { StatusCode = 200, Message = "update the information Host Successfully" });

            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }
        private bool ServiceExists(int id)
        {
            return _context.Services.Any(e => e.Id == id);
        }
    }
}
