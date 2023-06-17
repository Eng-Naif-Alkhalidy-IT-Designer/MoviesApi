


namespace MoviesApi.Model
{
    public class Genre
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte Id { get; set; }

        [MaxLength]
        public string Name { get; set; }

    }
}
