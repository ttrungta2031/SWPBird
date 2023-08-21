using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWPBirdBoarding.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWPBirdBoarding.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly SWPBirdBoardingContext _context;

        public HomeController(SWPBirdBoardingContext context)
        {
            _context = context;

        }

      


        /// <summary>
        /// lấy danh sách các chỗ lưu trú
        /// </summary>
        [HttpGet("GetHostList")]

        public async Task<ActionResult<IEnumerable<Service>>> GetHostList( string search, int pagesize = 10, int pagenumber = 1)
        {  
            var result = (from s in _context.Services
                          where s.Type.Contains("Lưu Trú")
                          select new
                          {
                              HostId = s.BirdShelterId,
                              HostName = s.BirdShelter.Name,
                              HostImage = s.BirdShelter.ImageUrl,                    
                              Price = s.Price,
                              Unit = s.Unit,
                              Amount = s.Amount,
                              Description = s.BirdShelter.Description,
                              Service = s.BirdShelter.Services.Select(a=> a.Type)

                          }).ToList();
            if (!string.IsNullOrEmpty(search))
            {
                result = (from s in _context.Services
                          where s.Type.Contains("Lưu Trú") && s.Name.Contains(search) || s.Type.Contains("Lưu Trú") && s.Price <= double.Parse(search)
                          select new
                          {
                              HostId = s.BirdShelterId,
                              HostName = s.BirdShelter.Name,
                              HostImage = s.BirdShelter.ImageUrl,
                              Price = s.Price,
                              Unit = s.Unit,
                              Amount = s.Amount,
                              Description = s.BirdShelter.Description,
                              Service = s.BirdShelter.Services.Select(a => a.Type)
                          }).ToList();
            }

       

            var paging = result.Skip((pagenumber - 1) * pagesize).Take(pagesize).ToList();
            double totalpage1 = (double)result.Count() / pagesize;
            totalpage1 = Math.Ceiling(totalpage1);
            return Ok(new { StatusCode = 200, Content = "Load successful", Data = paging, totalpage = totalpage1, pagesize = pagesize, pagenumber = pagenumber });
        }

        [HttpGet("GetHostDetail")]

        public async Task<ActionResult<IEnumerable<Service>>> GetHostDetail(int hostid)
        {



            var result = (from s in _context.Services
                          where s.Type.Contains("Lưu Trú") && s.BirdShelterId == hostid
                          select new
                          {
                              HostId = s.BirdShelterId,
                              HostName = s.BirdShelter.Name,
                              HostImage = s.BirdShelter.ImageUrl,
                              Price = s.Price,
                              Unit = s.Unit,
                              Amount = s.Amount,
                              Description = s.BirdShelter.Description,
                              Service = s.BirdShelter.Services.Select(a => a.Type)

                          }).ToList();
         
            return Ok(new { StatusCode = 200, Content = "Load successful", Data = result });
        }





        /// <summary>
        /// lấy thông tin danh sách về profile chim
        /// </summary>
        [HttpGet("GetBirdProfileList")]

        public async Task<ActionResult<IEnumerable<Service>>> GetBirdProfileList(int accountid, string search, int pagesize = 10, int pagenumber = 1)
        {
            var result = (from s in _context.BirdProfiles
                          where s.AccountId == accountid
                          select new
                          {
                              BirdProfileId = s.Id,
                            Name = s.Name,
                            ImageUrl = s.ImageUrl,
                            Type = s.Type,
                            Description = s.Description,
                            Status = s.Status,


                          }).ToList();
            if (!string.IsNullOrEmpty(search))
            {
                result = (from s in _context.BirdProfiles
                          where s.AccountId == accountid && s.Name.Contains(search)
                          select new
                          {
                              BirdProfileId = s.Id,
                              Name = s.Name,
                              ImageUrl = s.ImageUrl,
                              Type = s.Type,
                              Description = s.Description,
                              Status = s.Status,

                          }).ToList();
            }

            var paging = result.Skip((pagenumber - 1) * pagesize).Take(pagesize).ToList();
            double totalpage1 = (double)result.Count() / pagesize;
            totalpage1 = Math.Ceiling(totalpage1);
            return Ok(new { StatusCode = 200, Content = "Load successful", Data = paging, totalpage = totalpage1, pagesize = pagesize, pagenumber = pagenumber });
        }



        public class CommentDetail
        {
            public int ReportId { get; set; }
            public string MsgCustomer { get; set; }

        }

        [HttpPut("CommentReport")]
        public async Task<IActionResult> CommentReport(CommentDetail report)
        {
            try
            {
                //   var isbookingid = await _context.Bookings.FindAsync(report.BookingId);
                var isreportbookingid = _context.BookingReports.SingleOrDefault(p => p.Id == report.ReportId);


                if (isreportbookingid == null || report.ReportId < 1)
                {
                    return StatusCode(409, new { StatusCode = 409, Message = "The Report Booking is not exist" });
                }

                isreportbookingid.MsgCustomer += "\n" +report.MsgCustomer;
                _context.BookingReports.Update(isreportbookingid);
                await _context.SaveChangesAsync();
                return Ok(new { StatusCode = 200, Message = "Comment to the Report Successfully" });

            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }

        public class DetailBooking
        {
            public DateTime DateStart { get; set; }
            public DateTime DateEnd { get; set; }
            public int HostId { get; set; }
            public int AccountId { get; set; }
            public int BirdProfileId { get; set; }

        }

        public class DetailBirdProfile
        {
            public string Name { get; set; }
            public string ImageUrl { get; set; }
            public string Type { get; set; }
            public string Description { get; set; }
            public int AccountId { get; set; }


        }
        /// <summary>
        /// thêm mới 1 profile chim
        /// </summary>
        [HttpPost("CreateBirdProfile")]
        public async Task<IActionResult> CreateBirdProfile(DetailBirdProfile profile)
        {
            try
            {
                var isHasAccount = await _context.Accounts.FindAsync(profile.AccountId);



                if (isHasAccount == null || profile.AccountId < 1)
                {
                    return StatusCode(409, new { StatusCode = 409, Message = "The Account not exist" });
                }

                var newBirdProfile = new Models.BirdProfile();
                {
                    newBirdProfile.Name = profile.Name;
                    newBirdProfile.ImageUrl = profile.ImageUrl;
                    newBirdProfile.Type = profile.Type;
                    newBirdProfile.Description = profile.Description;
                    newBirdProfile.Status = "active";
                    newBirdProfile.AccountId = profile.AccountId;


                }
                _context.BirdProfiles.Add(newBirdProfile);
                await _context.SaveChangesAsync();





                return Ok(new { StatusCode = 200, Message = "Create Bird Profile Successfully" });

            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }
        /// <summary>
        /// lấy thông tin lịch sử đặt 
        /// </summary>
        [HttpGet("HistoryBooking")]

        public async Task<ActionResult<IEnumerable<Account>>> HistoryBooking(int accountid, string search, int pagesize = 10, int pagenumber = 1)
        {
            var result = (from s in _context.Bookings
                          where s.AccountId == accountid
                          select new
                          {
                              BookingId = s.Id,
                              DateBooking  = s.DateBooking,
           DateStart = s.DateStart,
           DateEnd = s.DateEnd,
           BirdName = s.BirdProfile.Name,
           BirdShelter = s.BirdShelter.Name,
         
            Status = s.Status,
           Total =s.Total

                          }).ToList();
            if (!string.IsNullOrEmpty(search))
            {
                result = (from s in _context.Bookings
                          where s.AccountId == accountid && s.BirdProfile.Name.Contains(search) || s.AccountId == accountid && s.BirdShelter.Name.Contains(search)
                          select new
                          {
                              BookingId = s.Id,
                              DateBooking = s.DateBooking,
                              DateStart = s.DateStart,
                              DateEnd = s.DateEnd,
                              BirdName = s.BirdProfile.Name,
                              BirdShelter = s.BirdShelter.Name,

                              Status = s.Status,
                              Total = s.Total

                          }).ToList();
            }

            var paging = result.Skip((pagenumber - 1) * pagesize).Take(pagesize).ToList();
            double totalpage1 = (double)result.Count() / pagesize;
            totalpage1 = Math.Ceiling(totalpage1);
            return Ok(new { StatusCode = 200, Content = "Load successful", Data = paging, totalpage = totalpage1, pagesize = pagesize, pagenumber = pagenumber });



        }
        /// <summary>
        /// lấy thông tin chi tiết đơn booking.
        /// </summary>
        [HttpGet("BookingDetail")]
        public async Task<ActionResult<IEnumerable<Account>>> BookingDetail(int bookingid)
        {
            if (bookingid <= 0) return StatusCode(409, new { StatusCode = 409, message = "Id null!!" });



            var result = (from s in _context.Bookings
                          where s.Id == bookingid
                          select new
                          {
                              Id = s.Id,
                              DateBooking = s.DateBooking,
                              CustomerName = s.Account.FullName,
                              BirdOfCustomer = s.BirdProfile.Name,
                              ImageOfBird = s.BirdProfile.ImageUrl,
                              TypeOfBird = s.BirdProfile.Type,
                              InfoOfBird = s.BirdProfile.Description,
                              DateStart = s.DateStart,
                              DateEnd = s.DateEnd,
                              Service = s.BookingDetails.Where(p => p.BookingId == bookingid).Select(a => a.Service),
                              Status = s.Status

                          }).ToList();


            return Ok(new { StatusCode = 200, Content = "Load successful", Data = result });




        }







        [HttpGet("ServiceDetail")]
        public async Task<ActionResult<IEnumerable<Account>>> ServiceDetail(int bookingid)
        {
            if (bookingid <= 0) return StatusCode(409, new { StatusCode = 409, message = "Id null!!" });


            var result = (from s in _context.BookingDetails
                          where s.BookingId == bookingid
                          select new
                          {
                              id = s.Id,
                              ServiceName = s.Service.Name,
                              Unit = s.Service.Unit,
                              Amount = s.Amount,
                              Price = s.Service.Price,
                              Total = s.Service.Price*s.Amount

                          }).ToList();
            double finaltotal = 0;
            foreach (var item in result)
            {
                finaltotal += (double)item.Total;
            }
          

            return Ok(new { StatusCode = 200, Content = "Load successful", Data = result, finaltotal});
        }

        public class ServiceAdd
        {

            public int ServiceId { get; set; }
            public int? Amount { get; set; }
            public int? BookingId { get; set; }


        }

        [HttpPost("AddService")]
        public async Task<IActionResult> AddService(ServiceAdd service)
        {
           
            try
            {
                var serviceunit = _context.Services.Where(p => p.Id == service.ServiceId).FirstOrDefault();

                var hasservice = _context.BookingDetails.Where(p => p.ServiceId == serviceunit.Id && p.BookingId == service.BookingId).FirstOrDefault();
                if (hasservice != null)
                {
                    return StatusCode(409, new { StatusCode = 409, Message = "The service is has already" });
                }



                int  newamount = 1;
                if (serviceunit.Unit == "Gói")
                {
                    newamount = 1;
                }
                else newamount = (int)service.Amount;



                var bookingdetail = new Models.BookingDetail();
                {
                    bookingdetail.Amount = newamount;
                    bookingdetail.BookingId = service.BookingId;
                    bookingdetail.ServiceId = service.ServiceId;

                }
                _context.BookingDetails.Add(bookingdetail);
                await _context.SaveChangesAsync();



                return Ok(new { StatusCode = 200, Message = "Add Service Successfully" });

            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }



        public class ServiceUpdate
        {

            public int Id { get; set; }
            public int? Amount { get; set; }


        }
        [HttpPut("UpdateServiceDetail")]
        public async Task<IActionResult> UpdateService(ServiceUpdate service)
        {

            try
            {
                var bookingdetail = await _context.BookingDetails.FindAsync(service.Id);
                if (bookingdetail == null)
                {
                    return NotFound();
                }

                var serviceunit = _context.Services.Where(p => p.Id == bookingdetail.ServiceId).FirstOrDefault();

                int newamount = 1;
                if (serviceunit.Unit == "Gói")
                {
                    newamount = 1;
                }
                else if((int)service.Amount >0) newamount = (int)service.Amount;

                bookingdetail.Amount = newamount;

               
                _context.BookingDetails.Update(bookingdetail);
                await _context.SaveChangesAsync();



                return Ok(new { StatusCode = 200, Message = "Update Service Successfully" });

            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }



        [HttpDelete("DeleteServiceDetail")]
        public async Task<IActionResult> DeleteService(int id)
        {

            try
            {
                var bookingdetail = await _context.BookingDetails.FindAsync(id);
                if (bookingdetail == null)
                {
                    return NotFound();
                }

                _context.BookingDetails.Remove(bookingdetail);
                await _context.SaveChangesAsync();




                return Ok(new { StatusCode = 200, Message = "Delete this Service Successfully" });

            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }









        /// <summary>
        /// Đặt đơn booking bởi member
        /// </summary>

        [HttpPost("BookingByMember")]
        public async Task<IActionResult> BookingByMember(DetailBooking booking)
        {
            //update totalamount
            try
            {
                var isHasAccount = await _context.Accounts.FindAsync(booking.AccountId);



                if (isHasAccount == null || booking.AccountId < 1)
                {
                    return StatusCode(409, new { StatusCode = 409, Message = "The Account not exist" });
                }

              
                var servicedefault = _context.Services.Where(p => p.Type.Contains("Lưu Trú") && p.BirdShelterId==booking.HostId).Select(a => a.Id).FirstOrDefault();
                var servicedefaultprice = _context.Services.Where(p => p.Type.Contains("Lưu Trú") && p.BirdShelterId == booking.HostId).Select(a => a.Price).FirstOrDefault();


                var newBooking = new Models.Booking();
                {
                    newBooking.DateBooking = DateTime.UtcNow.AddHours(7);
                    newBooking.DateStart = booking.DateStart;
                    newBooking.DateEnd = booking.DateEnd;
                    newBooking.BirdProfileId = booking.BirdProfileId;
                    newBooking.BirdShelterId = booking.HostId;
                    newBooking.AccountId = booking.AccountId;
                    newBooking.Status = "waiting";
                    newBooking.Total = servicedefaultprice;


                }
                _context.Bookings.Add(newBooking);
                await _context.SaveChangesAsync();

                var bookingnew = _context.Bookings.Max(it => it.Id);


              var totalday = (int)booking.DateEnd.Subtract(booking.DateStart).TotalDays;
                var bookingdetail = new Models.BookingDetail();
                {
                    bookingdetail.Amount = totalday;
                    bookingdetail.BookingId = bookingnew;
                    bookingdetail.ServiceId = servicedefault;

                }
                _context.BookingDetails.Add(bookingdetail);
                await _context.SaveChangesAsync();



                return Ok(new { StatusCode = 200, Message = "Booking Successfully" });

            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }

        [HttpGet("Gettotaldashboard")]

        public IActionResult GetTopByBooking(int accountid)
        {
            var hostaccount = _context.BirdShelters.SingleOrDefault(p => p.AccountId == accountid);
            if (hostaccount == null) return StatusCode(409, new { StatusCode = 409, message = "Not Found Information of the AccountId" });

            var customer = (from s in _context.Bookings
                            where s.BirdShelterId == hostaccount.Id
                            select new
                              {                            
                                Amount = s.Account.FullName.Count()
                            }).Distinct().ToList();
            var countcustomer = 0;
            /*foreach (var item in customer)
            {
                int count = 1;
                var oldname = item.CustomerName;
                if (!item.CustomerName.Contains(oldname)) count += 1;
              
                countcustomer = count;
            }*/
            int count = 1;
            /*  for (int i =1; i<customer.Count;i++)
              {

                  var oldname = customer[i-1].CustomerName;
                  if (!customer[i].CustomerName.Contains(oldname))
                  {
                      count += 1;
                  }

                  countcustomer = count;

              }*/
        
              var service = (from s in _context.BookingDetails
                            where  s.Booking.BirdShelterId == hostaccount.Id
                            select new
                            {
                                Id = s.Id,
                                ServiceId =s.ServiceId,
                                ServiceName = s.Service.Name

                            }).ToList();


          


            //    var countcustomer = customer.Count();
            var countservice = service.Count();
            countcustomer = customer.Count();
            return Ok(new { StatusCode = 200, Message = "Load successful", customer = countcustomer, service = countservice});
        }


        [HttpGet("Gettopservice")]

        public IActionResult Gettopservice(int accountid)
        {
            var hostaccount = _context.BirdShelters.SingleOrDefault(p => p.AccountId == accountid);
            if (hostaccount == null) return StatusCode(409, new { StatusCode = 409, message = "Not Found Information of the AccountId" });

            var service = (from s in _context.BookingDetails
                            where  s.Booking.BirdShelterId == hostaccount.Id && s.Booking.Status =="success"
                            select new
                            {
       
                                ServiceId =s.ServiceId,
                                ServiceName = s.Service.Name,
                                Amount = s.Service.Name.Count()
                                

                            }).Distinct().ToList();
            var sort = service.OrderByDescending(x => x.ServiceName).Take(10).ToList();
            return Ok(new { StatusCode = 200, Message = "Load successful", data = sort });
        }

        public class DetailReport
        {
            public string Name { get; set; }
            public string Month { get; set; }
            public double? Total { get; set; }
        }

        [HttpGet("ReportByYear")]
        public ActionResult DetailProfitReport(int accountid, int year = 2023)
        {
            List<DetailReport> detail = new List<DetailReport>();
            for (int month = 1; month <= 12; month++)
            {

                 var firstDayOfMonth = new DateTime(year, month, 1);
                var lastDayOfMonth = new DateTime(year, month, DateTime.DaysInMonth(year, month));
                double total = 0;
                double finaltotal = 0;
                var hostaccount = _context.BirdShelters.SingleOrDefault(p => p.AccountId == accountid);
                if (hostaccount == null) return StatusCode(409, new { StatusCode = 409, message = "Not Found Information of the AccountId" });


                var bookingid = (from s in _context.Bookings
                                 where s.BirdShelterId == hostaccount.Id && s.Status == "success" && s.DateFinish >= firstDayOfMonth && s.DateFinish <= lastDayOfMonth
                                 select new
                                 {
                                     Id = s.Id,
                                     DateFinish = s.DateFinish

                                 }).ToList();
                foreach (var item1 in bookingid)
                {
                    /* var priceofbill = _context.BookingDetails.Where(a => a.BookingId == item1.Id).Select(a => a.Service.Price);
                     foreach (var item in priceofbill)
                     {
                         total += item.GetValueOrDefault();

                     }*/
                    DateTime Datefinishnew = (DateTime)item1.DateFinish;
                    int datemonth = Datefinishnew.Month;

                    if(item1.DateFinish >= firstDayOfMonth && item1.DateFinish <= lastDayOfMonth && datemonth == month)
                    {
                        var newtotal = (from s in _context.BookingDetails
                                        where s.BookingId == item1.Id
                                        select new
                                        {
                                            id = s.Id,
                                            ServiceName = s.Service.Name,
                                            Unit = s.Service.Unit,
                                            Amount = s.Amount,
                                            Price = s.Service.Price,
                                            Total = s.Service.Price * s.Amount

                                        }).ToList();

                        foreach (var item in newtotal)
                        {
                            finaltotal += (double)item.Total;
                        }


                    }

                   

                }
                DetailReport depo = new DetailReport();
                {
                    depo.Month = "Tháng " + month.ToString();
                    depo.Name = "Doanh Thu";
                    depo.Total = finaltotal;
                }
                detail.Add(depo);




            }



               
            var resultnew = detail.ToList();


            return Ok(new { StatusCode = 200, Content = "Load successful", Data = resultnew });

            }


        [HttpGet("GetFeedback")]

        public async Task<ActionResult<IEnumerable<Service>>> GetFeedback(int hostid)
        {

            var hostaccount = _context.BirdShelters.SingleOrDefault(p => p.Id == hostid);



            var result = (from s in _context.Feedbacks
                          where s.Booking.BirdShelterId == hostaccount.Id
                          select new
                          {
                              CustomerName = s.Account.FullName,
                              Rating = s.Rating,
                              Feedback = s.Description
                          }).ToList();

            return Ok(new { StatusCode = 200, Content = "Load successful", Data = result });
        }


        [HttpGet("GetArticle")]

        public async Task<ActionResult<IEnumerable<Service>>> GetArticle()
        {

            var result = (from s in _context.Articles
                          where s.Status =="active"
                          select new
                          {
                             Id = s.Id,
                             ImageUrl = s.ImageUrl,
                             Title = s.Title,
                              Status = s.Status
                          }).ToList();

            return Ok(new { StatusCode = 200, Content = "Load successful", Data = result });
        }


        [HttpGet("GetArticleDetail")]

        public async Task<ActionResult<IEnumerable<Service>>> GetArticleDetail(int id)
        {

            var result = (from s in _context.Articles
                          where s.Status == "active" && s.Id == id
                          select new
                          {
                              Id = s.Id,
                              ImageUrl = s.ImageUrl,
                              Title = s.Title,
                              Description = s.Description,
                              DateCreate = s.DateCreate,
                              Status = s.Status
                          }).ToList();

            return Ok(new { StatusCode = 200, Content = "Load successful", Data = result });
        }






    }
}
