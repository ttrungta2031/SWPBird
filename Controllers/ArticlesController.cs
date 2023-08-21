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
    public class ArticlesController : ControllerBase
    {
        private readonly SWPBirdBoardingContext _context;

        public ArticlesController(SWPBirdBoardingContext context)
        {
            _context = context;
        }

        // GET: api/Articles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Article>>> GetArticles()
        {
            return await _context.Articles.ToListAsync();
        }

        // GET: api/Articles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Article>> GetArticle(int id)
        {
            var article = await _context.Articles.FindAsync(id);

            if (article == null)
            {
                return NotFound();
            }

            return article;
        }

        // PUT: api/Articles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("UpdateArticle")]
        public async Task<IActionResult> UpdateArticle(DetailArticle2 article)
        {
            try
            {
                //   var isbookingid = await _context.Bookings.FindAsync(report.BookingId);
                var isarticle = _context.Articles.SingleOrDefault(p => p.Id == article.Id);


                if (isarticle == null || article.Id < 1)
                {
                    return StatusCode(409, new { StatusCode = 409, Message = "The Article is not exist" });
                }

                isarticle.ImageUrl = article.ImageUrl == null ? isarticle.ImageUrl : article.ImageUrl;
                isarticle.Title = article.Title == null ? isarticle.Title : article.Title;
                isarticle.Description = article.Description == null ? isarticle.Description : article.Description;


                _context.Articles.Update(isarticle);
                await _context.SaveChangesAsync();
                return Ok(new { StatusCode = 200, Message = "Update Successfully" });

            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }


        public class DetailArticle2
        {
            public int Id { get; set; }
            public string ImageUrl { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }


        }
        public class DetailArticle
        {
            public string ImageUrl { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }


        }
        // POST: api/Articles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("CreateArticle")]
        public async Task<IActionResult> CreateBirdProfile(DetailArticle article)
        {
            try
            {
          

                var newArticle = new Models.Article();
                {
                    newArticle.ImageUrl = article.ImageUrl;
                    newArticle.Title = article.Title;
                    newArticle.Description = article.Description;
                    newArticle.DateCreate = DateTime.UtcNow.AddHours(7);
                    newArticle.Status = "Active";


                }
                _context.Articles.Add(newArticle);
                await _context.SaveChangesAsync();





                return Ok(new { StatusCode = 200, Message = "Create new Article Successfully" });

            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException.Message);
                return StatusCode(409, new { StatusCode = 409, Message = e.Message });
            }
        }
        // DELETE: api/Articles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();

            return Ok(new { StatusCode = 200, Message = "Delete the Article Successfully" });
        }

        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.Id == id);
        }
    }
}
