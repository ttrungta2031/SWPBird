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
    public class BookingsController : ControllerBase
    {
        private readonly SWPBirdBoardingContext _context;

        public BookingsController(SWPBirdBoardingContext context)
        {
            _context = context;
        }


        // GET: api/GetBookingList/5
        /// <summary>
        /// lấy thông tin danh sách các đơn booking
        /// </summary>
        [HttpGet("GetBookingList")]
        public async Task<ActionResult<IEnumerable<Account>>> GetBookingList(string search, int accountid)
        {
            if(accountid == null) return StatusCode(409, new { StatusCode = 409, message = "AccountId null!!" });
            var hostaccount = _context.BirdShelters.SingleOrDefault(p => p.AccountId == accountid);
            if (hostaccount == null) return StatusCode(409, new { StatusCode = 409, message = "Not Found Information of the AccountId" });
            var result = (from s in _context.Bookings
                         where s.BirdShelterId == hostaccount.Id
                          select new
                          {
                              Id = s.Id,
                              DateBooking = s.DateBooking,
                              CustomerName = s.Account.FullName,
                              BirdOfCustomer = s.BirdProfile.Name,
                              DateStart = s.DateStart,
                              DateEnd = s.DateEnd,
                              Status = s.Status

                          }).ToList();
            if (!string.IsNullOrEmpty(search))
            {
                result = (from s in _context.Bookings
                          where s.BirdShelterId == hostaccount.Id && s.Account.FullName.Contains(search) || s.BirdShelterId == hostaccount.Id && s.DateStart.ToString().Contains(search) || s.BirdShelterId == hostaccount.Id && s.Status.Contains(search)
                          select new
                          {
                              Id = s.Id,
                              DateBooking = s.DateBooking,
                              CustomerName = s.Account.FullName,
                              BirdOfCustomer = s.BirdProfile.Name,
                              DateStart = s.DateStart,
                              DateEnd = s.DateEnd,
                              Status = s.Status
                          }).ToList();
            }

          
         
            return Ok(new { StatusCode = 200, Content = "Load successful", Data = result });



        }

        [HttpGet("GetBookingReportList")]
        public async Task<ActionResult<IEnumerable<BookingReport>>> GetBookingReportList( int bookingid, int pagesize = 10, int pagenumber = 1)
        {
            if (bookingid == null) return StatusCode(409, new { StatusCode = 409, message = "BookingId null!!" });
            var isbookingid = _context.Bookings.SingleOrDefault(p => p.Id == bookingid);
            if (isbookingid == null) return StatusCode(409, new { StatusCode = 409, message = "Not Found Information of the Booking" });
            var result = (from s in _context.BookingReports
                          where s.BookingId == isbookingid.Id
                          select new
                          {
                              Id = s.Id,
                              Date =s.Date,
                              Description = s.Description,
                              ImageUrl = s.ImageUrl,
                              VideoUrl = s.VideoUrl,
                              MessageCustomer = s.MsgCustomer,
                              MessageHost = s.MsgHost,
                              Status = s.Status

                          }).ToList();
           

            var paging = result.Skip((pagenumber - 1) * pagesize).Take(pagesize).ToList();
            double totalpage1 = (double)result.Count() / pagesize;
            totalpage1 = Math.Ceiling(totalpage1);
            return Ok(new { StatusCode = 200, Content = "Load successful", Data = paging, totalpage = totalpage1, pagesize = pagesize, pagenumber = pagenumber });



        }

        public class BillBooking
        {
            public int BookingId { get; set; }
            public DateTime? DateBooking { get; set; }
            public string CustomerName { get; set; }
            public string BirdOfCustomer { get; set; }
            public string TypeOfBird { get; set; }
            public DateTime DateStart { get; set; }
            public DateTime DateEnd { get; set; }
            public int AmountDay { get; set; }
            public List<string> Service { get; set; }
            public string Status { get; set; }
            public double total { get; set; }


        }

        [HttpGet("GetBillBooking")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBillBooking(int bookingid)
        {
            if (bookingid == null) return StatusCode(409, new { StatusCode = 409, message = "BookingId null!!" });
            var isbookingid = _context.Bookings.SingleOrDefault(p => p.Id == bookingid);
            if (isbookingid == null) return StatusCode(409, new { StatusCode = 409, message = "Not Found Information of the Booking" });

            var priceofbill = _context.BookingDetails.Where(a => a.BookingId == isbookingid.Id).Select(a => a.Service.Price);
            var servicename = _context.BookingDetails.Where(a => a.BookingId == isbookingid.Id).Select(a => a.Service.Name);



            var newtotal = (from s in _context.BookingDetails
                          where s.BookingId == bookingid
                          select new
                          {
                              id = s.Id,
                              ServiceName = s.Service.Name,
                              Unit = s.Service.Unit,
                              Amount = s.Amount,
                              Price = s.Service.Price,
                              Total = s.Service.Price * s.Amount

                          }).ToList();
            double finaltotal = 0;
            foreach (var item in newtotal)
            {
                finaltotal += (double)item.Total;
            }


            /*double total = 0;
            foreach(var item in priceofbill)
            {
                total += item.GetValueOrDefault();

            }*/
            List<string> listservice = new List<string>();

            foreach (var item in servicename)
            {
              
                listservice.Add(item);
            }



            var billbookingdetail = _context.Bookings.Where(a => a.Id == isbookingid.Id).FirstOrDefault();
            var result = (from s in _context.Bookings
                          where s.Id == bookingid
                          select new
                          {
                              CustomerName = s.Account.FullName,
                              BirdOfCustomer = s.BirdProfile.Name,
                              TypeOfBird = s.BirdProfile.Type

                          });

          
            // var CustomerName = _context.Bookings.Where(a => a.Id == isbookingid.Id).Select(s => s.Account.FullName);


            var bill = new BillBooking();
            bill.Service = listservice;
            bill.BookingId = billbookingdetail.Id;
            bill.DateBooking = billbookingdetail.DateBooking;
          //  bill.CustomerName = account.Account.FullName;
          //  bill.BirdOfCustomer = account.BirdProfile.Name;
          //  bill.TypeOfBird = account.BirdProfile.Type;
            bill.DateStart = (DateTime)billbookingdetail.DateStart;
            bill.DateEnd = (DateTime)billbookingdetail.DateEnd;
            bill.Status = billbookingdetail.Status;
        //    bill.total = total;


            bill.AmountDay = (int)bill.DateEnd.Subtract(bill.DateStart).TotalDays;
            bill.total = finaltotal;


            foreach (var item in result)
            {
                bill.CustomerName = item.CustomerName;
                bill.BirdOfCustomer = item.BirdOfCustomer;
                bill.TypeOfBird = item.TypeOfBird;
            }

            return Ok(new { StatusCode = 200, Content = "Load successful", Data = bill });



        }






        public class ReportDetail
        {
            public int BookingId { get; set; }
            public DateTime? Date { get; set; }
            public string Description { get; set; }
            public string ImageUrl { get; set; }
            public string VideoUrl { get; set; }

            public string MsgHost { get; set; }

        }
        [HttpPost("CreateReport")]
        public async Task<IActionResult> CreateReport(ReportDetail report)
        {
            try
            {
             //   var isbookingid = await _context.Bookings.FindAsync(report.BookingId);
                var isbookingid = _context.Bookings.SingleOrDefault(p => p.Id == report.BookingId);


                if (isbookingid == null || report.BookingId < 1)
                {
                    return StatusCode(409, new { StatusCode = 409, Message = "The Booking is not exist" });
                }

                var newReport = new Models.BookingReport();
                {
                    newReport.BookingId = report.BookingId;
                    newReport.Date = report.Date;
                    newReport.Description = report.Description;
                    newReport.ImageUrl = report.ImageUrl;
                    newReport.VideoUrl = report.VideoUrl;
                    newReport.MsgHost = report.MsgHost;
                    newReport.Status = "active";


                }
                _context.BookingReports.Add(newReport);
                await _context.SaveChangesAsync();
                return Ok(new { StatusCode = 200, Message = "Create new Report Successfully" });

            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }

        public class ReportDetail3
        {
            public int BookingId { get; set; }
            public DateTime? DateStart { get; set; }

            public DateTime? DateEnd { get; set; }

        }

        [HttpPut("UpdateBooking")]
        public async Task<IActionResult> UpdateBooking(ReportDetail3 booking)
        {
            try
            {
                //   var isbookingid = await _context.Bookings.FindAsync(report.BookingId);
                var isbookingid = _context.Bookings.SingleOrDefault(p => p.Id == booking.BookingId);


                if (isbookingid == null || booking.BookingId < 1)
                {
                    return StatusCode(409, new { StatusCode = 409, Message = "The Booking is not exist" });
                }

                isbookingid.DateStart = booking.DateStart;
                isbookingid.DateEnd = booking.DateEnd;



                _context.Bookings.Update(isbookingid);
                await _context.SaveChangesAsync();
                return Ok(new { StatusCode = 200, Message = "update the booking Successfully" });

            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }




        public class ReportDetail2
        {
            public int ReportId { get; set; }
            public DateTime? Date { get; set; }
            public string Description { get; set; }
            public string ImageUrl { get; set; }
            public string VideoUrl { get; set; }
            public string MsgHost { get; set; }

        }

        public class UpdateImage
        {
            public int ReportId { get; set; }       
            public string ImageUrl { get; set; }

        }

        public class UpdateVideo
        {
            public int ReportId { get; set; }
            public string VideoUrl { get; set; }

        }


        [HttpPut("UpdateReport")]
        public async Task<IActionResult> UpdateReport(ReportDetail2 report)
        {
            try
            {
                //   var isbookingid = await _context.Bookings.FindAsync(report.BookingId);
                var isreportbookingid = _context.BookingReports.SingleOrDefault(p => p.Id == report.ReportId);


                if (isreportbookingid == null || report.ReportId < 1)
                {
                    return StatusCode(409, new { StatusCode = 409, Message = "The Report Booking is not exist" });
                }


                isreportbookingid.Date = isreportbookingid.Date;
                isreportbookingid.Description = isreportbookingid.Description;
                isreportbookingid.ImageUrl = isreportbookingid.ImageUrl;
                isreportbookingid.VideoUrl = isreportbookingid.VideoUrl;
                isreportbookingid.MsgHost += "\n" + report.MsgHost;


                _context.BookingReports.Update(isreportbookingid);
                await _context.SaveChangesAsync();
                return Ok(new { StatusCode = 200, Message = "update the Report Successfully" });

            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }



        [HttpPut("UpdateImage")]
        public async Task<IActionResult> UpdateImaging(UpdateImage report)
        {
            try
            {
                //   var isbookingid = await _context.Bookings.FindAsync(report.BookingId);
                var isreportbookingid = _context.BookingReports.SingleOrDefault(p => p.Id == report.ReportId);


                if (isreportbookingid == null || report.ReportId < 1)
                {
                    return StatusCode(409, new { StatusCode = 409, Message = "The Report Booking is not exist" });
                }
                isreportbookingid.ImageUrl = report.ImageUrl;
          
                _context.BookingReports.Update(isreportbookingid);
                await _context.SaveChangesAsync();
                return Ok(new { StatusCode = 200, Message = "update image the Report Successfully" });

            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }

        [HttpPut("UpdateVideo")]
        public async Task<IActionResult> UpdateVideon(UpdateVideo report)
        {
            try
            {
                //   var isbookingid = await _context.Bookings.FindAsync(report.BookingId);
                var isreportbookingid = _context.BookingReports.SingleOrDefault(p => p.Id == report.ReportId);


                if (isreportbookingid == null || report.ReportId < 1)
                {
                    return StatusCode(409, new { StatusCode = 409, Message = "The Report Booking is not exist" });
                }
                isreportbookingid.VideoUrl = report.VideoUrl;

                _context.BookingReports.Update(isreportbookingid);
                await _context.SaveChangesAsync();
                return Ok(new { StatusCode = 200, Message = "update image the Report Successfully" });

            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }






        /// <summary>
        /// lấy thông tin chi tiết đơn booking.
        /// </summary>
        [HttpGet("GetBookingDetail")]
      
        public async Task<ActionResult<IEnumerable<Account>>> GetBookingDetail(int id)
        {
            if (id <=0) return StatusCode(409, new { StatusCode = 409, message = "Id null!!" });

            var result = (from s in _context.Bookings
                          where s.Id == id
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
                              Service = s.BookingDetails.Where(p=>p.BookingId == id).Select(a => a.Service),
                              Status = s.Status

                          }).ToList();
           
         
            return Ok(new { StatusCode = 200, Content = "Load successful", Data = result });




        }


        public class ServiceBody
        {
            public int BookingId { get; set; }
            public List<int> SerId { get; set; }
        }

        /// <summary>
        /// cập nhật dịch vụ vào đơn booking
        /// </summary>
        [HttpPost("UpdateServiceBooking")]

        public async Task<ActionResult<Service>> UpdateServiceBooking([FromBody] ServiceBody option)
        {
            try
            {
                var result = (from s in _context.BookingDetails
                              where s.BookingId == option.BookingId
                              select new
                              {
                                  Id = s.Id,
                                  ServiceId = s.ServiceId
                              }).ToList();
                if (result.Count > 0)
                {
                    foreach (var item in result)
                    {
                      //  var service = _context.BookingDetails.Where(a => a.ServiceId == item.ServiceId);
                      var bookingdetailid = _context.BookingDetails.Find(item.Id);
                     //   var service =  _context.BookingDetails.Find(bookingdetailid.ServiceId);
                        if (bookingdetailid != null)
                        {
                            //  return StatusCode(409, new { StatusCode = 409, Message = "Không tìm thấy loại dịch vụ hoặc chọn sai định dạng!" });
                            _context.BookingDetails.Remove(bookingdetailid);
                            await _context.SaveChangesAsync();
                        }

                       

                    }
                }

             
                for (int i = 0; i < option.SerId.Count; i++)
                {
                    var existservice = _context.BookingDetails.Where(a => a.BookingId == option.BookingId).Where(b => b.ServiceId == option.SerId[i]).FirstOrDefault();
                //    var existspec = _context.Specializations.Where(a => a.ConsultantId == option.ConsultantId).Where(b => b.SpecializationTypeId == option.SpecId[i]).FirstOrDefault();
                 //   var existspecname = _context.SpecializationTypes.Where(b => b.Id == option.SpecId[i]).FirstOrDefault();
                    var existservicename = _context.Services.Where(b => b.Id == option.SerId[i]).FirstOrDefault();
                    if (existservicename == null) { return StatusCode(409, new { StatusCode = 409, Message = "Không tìm thấy loại dịch vụ hoặc chọn sai định dạng!" }); }
                    if (existservice != null) { return StatusCode(409, new { StatusCode = 409, Message = "Đã tồn tại Loại dịch vụ:  {" + existservicename.Name + "} Vui lòng kiểm tra lại! " }); }

                }

                foreach (var item in option.SerId)
                {
                    var addservice = new BookingDetail();
                    {
                        addservice.BookingId = option.BookingId;
                        addservice.ServiceId = item;
                    }
                    _context.BookingDetails.Add(addservice);
                    await _context.SaveChangesAsync();
                }

                return Ok(new { StatusCode = 201, Message = "Add Successfull" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }
        /// <summary>
        /// chấp nhận đơn booking
        /// </summary>
        [HttpPut("AcceptBooking")]
        public async Task<IActionResult> AcceptBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            if (booking.Status == "waiting")
            {
                booking.Status = "accepted";
                _context.Bookings.Update(booking);
                await _context.SaveChangesAsync();
                return Ok(new { StatusCode = 200, Content = "Xác nhận thành công" });
            }

            return StatusCode(409, new { StatusCode = 409, message = "Xác nhận thất bại" });
        }


        [HttpPut("SuccessBooking")]
        public async Task<IActionResult> SuccessBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            if (booking.Status == "processing")
            {
                booking.Status = "success";
                booking.DateFinish = DateTime.UtcNow.AddHours(7);
                _context.Bookings.Update(booking);
                await _context.SaveChangesAsync();
                return Ok(new { StatusCode = 200, Content = "Xác nhận thanh toán thành công" });
            }

            return StatusCode(409, new { StatusCode = 409, message = "Xác nhận thất bại" });
        }


        [HttpPut("checkinBooking")]
        public async Task<IActionResult> CheckinBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            if (booking.Status == "accepted")
            {
                booking.Status = "processing";
                booking.DateFinish = DateTime.UtcNow.AddHours(7);
                _context.Bookings.Update(booking);
                await _context.SaveChangesAsync();
                return Ok(new { StatusCode = 200, Content = "Xác nhận Checkin thành công" });
            }

            return StatusCode(409, new { StatusCode = 409, message = "Xác nhận thất bại" });
        }


        [HttpPut("cancelBooking")]
        public async Task<IActionResult>CancelBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            if (booking.Status == "accepted" || booking.Status == "waiting")
            {
                booking.Status = "cancel";
                booking.DateFinish = DateTime.UtcNow.AddHours(7);
                _context.Bookings.Update(booking);
                await _context.SaveChangesAsync();
                return Ok(new { StatusCode = 200, Content = "Từ chối thành công" });
            }

            return StatusCode(409, new { StatusCode = 409, message = "Từ chối thất bại" });
        }




        // PUT: api/Bookings/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754


        // POST: api/Bookings
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754


        // DELETE: api/Bookings/5


        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.Id == id);
        }
    }
}
