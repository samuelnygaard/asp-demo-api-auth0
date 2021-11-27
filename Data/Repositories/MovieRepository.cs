namespace Data.Repositories
{
    public class MovieRepository : BaseRepository<DataContext, Models.Movie, Entities.Movie, int>
    {
        public MovieRepository(DataContext db) : base(db) { }
    }
}
