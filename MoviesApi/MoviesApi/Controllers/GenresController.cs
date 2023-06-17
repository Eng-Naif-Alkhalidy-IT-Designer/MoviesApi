using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GenresController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var genres = await _context.genres.OrderBy(g=>g.Name).ToListAsync();
            return Ok(genres);
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateGenreDto dto)
       
        {
          var genre=new  Genre { Name=dto.Name }; 
         await   _context.genres.AddAsync(genre);
            _context.SaveChanges();
            return Ok(dto);
        }
        [HttpPut("{id}")]
        //api/geners/1
     public async Task<IActionResult> UpdateAsync(int id,[FromBody] CreateGenreDto dto)
        {
            var genre=await _context.genres.SingleOrDefaultAsync(g=>g.Id==id);
            if (genre == null)
                return NotFound($"No Genre was found with ID:{id}");
            genre.Name=dto.Name;
            _context.SaveChanges();
            return Ok(genre);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var genere=await _context.genres.SingleOrDefaultAsync(g=>g.Id==id);
            if (genere == null)
                return NotFound($"not id {id}");
            _context.genres.Remove(genere);
            _context.SaveChanges();
            var f=await _context.genres.ToListAsync();
            return Ok(f);
        }

    }
    
}
