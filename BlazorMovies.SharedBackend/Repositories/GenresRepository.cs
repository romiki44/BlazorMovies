using BlazorMovies.Shared.Entities;
using BlazorMovies.Shared.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorMovies.SharedBackend.Repositories
{
    public class GenresRepository : IGenreRepository
    {
        private readonly ApplicationDbContext context;

        public GenresRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<Genre> GetGenre(int id)
        {
            //return await context.Genres.FirstOrDefaultAsync(x => x.Id == id);

            // ak uz nasiel objekt s id v ramci daneho contextu, nerobi opatovny requeat do databazy!
            // vyhoda oproti firstordefault, vzdy urobi request do db
            return await context.Genres.FindAsync(id);
        }

        public async Task<List<Genre>> GetGenres()
        {
            return await context.Genres.ToListAsync();
        }

        public async Task CreateGenre(Genre genre)
        {
            context.Add(genre);
            await context.SaveChangesAsync();
        }

        public async Task DeleteGenre(int id)
        {
            var genre = await GetGenre(id);
            context.Remove(genre);
            await context.SaveChangesAsync();
        }        

        public async Task UpdateGenre(Genre genre)
        {
            context.Attach(genre).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }
    }
}
