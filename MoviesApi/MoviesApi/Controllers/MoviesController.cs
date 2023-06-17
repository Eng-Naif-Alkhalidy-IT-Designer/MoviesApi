using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        private new List<string> _allowedExtenstions = new List<string>
        {
            ".jpg",".png"
        };
        private long _MaxAllowedPosterSize = 1048576;

     

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var movies =await _context.Movies
                .OrderByDescending(x => x.Rate)
                .Include(m =>m.Genre)
                .Select(m=> new  MovieDetaiosDto
                {
                Id = m.Id,
                GenreId=m.GenreId,
                GenreName=m.Genre.Name,
                Poster=m.Poster,
                Rate=m.Rate,
                StoreLine=m.StoreLine,
                Title=m.Title,
                Year=m.Year,


                })
                .ToListAsync();
            return Ok(movies);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var movie =await _context.Movies.Include(m=>m.Genre).SingleOrDefaultAsync(m=>m.Id==id);
            if (movie == null)
                return BadRequest($"not found for id {id}");
            var dto = new MovieDetaiosDto
            {
                Id = movie.Id,
                GenreId = movie.GenreId,
                GenreName = movie.Genre.Name,
                Poster = movie.Poster,
                Rate = movie.Rate,
                StoreLine = movie.StoreLine,
                Title = movie.Title,
                Year = movie.Year,
            };
            return Ok(dto);
        }
        [HttpGet("GetByGenreId")]
        public async Task<IActionResult> GetByGenreIdAsync(byte genreId)
        {
            var movies = await _context.Movies
                .Where(m=>m.GenreId==genreId)
               .OrderByDescending(x => x.Rate) 
               .Include(m => m.Genre)
               .Select(m => new MovieDetaiosDto
               {
                   Id = m.Id,
                   GenreId = m.GenreId,
                   GenreName = m.Genre.Name,
                   Poster = m.Poster,
                   Rate = m.Rate,
                   StoreLine = m.StoreLine,
                   Title = m.Title,
                   Year = m.Year,


               })
               .ToListAsync();

            return Ok(movies);
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm]MovieDto dto)
        {
            if (dto.Poster == null)
                return BadRequest("no poster required");
            if (!_allowedExtenstions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                return BadRequest("Only .png and .jpg images are allowed!");
            if(dto.Poster.Length>_MaxAllowedPosterSize)
                return BadRequest("Only logger then 1mB!");
            var isvalidgenre=await _context.genres.AnyAsync(g=>g.Id==dto.GenreId);
            if(!isvalidgenre)
                return BadRequest("not found genre id !");

            using var datastream= new MemoryStream();   
            await dto.Poster.CopyToAsync(datastream);

            var movie = new Movie   
            {
                Poster=datastream.ToArray(),
                Rate=dto.Rate,
                StoreLine=dto.StoreLine,
                Year=dto.Year,
                 GenreId = dto.GenreId, 
                 Title = dto.Title,

            };
            await _context.AddAsync(movie);
            _context.SaveChanges();

            return Ok(movie);

        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id,[FromForm] MovieDto dto)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound($"NO movie was found with Id {id}");
            var isvalidgenre = await _context.genres.AnyAsync(g => g.Id == dto.GenreId);
            if (!isvalidgenre)
                return BadRequest("not found genre id !");
            if(dto.Poster!=null)
            {

                if (!_allowedExtenstions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                    return BadRequest("Only .png and .jpg images are allowed!");
                if (dto.Poster.Length > _MaxAllowedPosterSize)
                    return BadRequest("Only logger then 1mB!");

                using var datastream = new MemoryStream();
                await dto.Poster.CopyToAsync(datastream);
                movie.Poster = datastream.ToArray();
            }
             
            movie.Title=dto.Title;
            movie.GenreId=dto.GenreId;
            movie.Year=dto.Year;
            movie.StoreLine=dto.StoreLine;
            movie.Rate=dto.Rate;
            _context.SaveChangesAsync();

            return Ok(movie);

        }
       
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var movie=await _context.Movies.SingleOrDefaultAsync(g=>g.Id==id);
            if (movie == null)
                return BadRequest($"not found for id:{id}");
            _context.Remove(movie);
            await _context.SaveChangesAsync();
            return Ok(movie);
        }

    }
}
