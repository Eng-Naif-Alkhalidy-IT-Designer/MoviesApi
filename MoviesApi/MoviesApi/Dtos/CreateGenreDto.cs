namespace MoviesApi.Dtos
{
    public class CreateGenreDto
    {
        [MaxLength]
        public string  Name { get; set; }
    }
}
