using BlazorMovies.Shared.Entities;
using BlazorMovies.Shared.Repositories;
using BlazorMovies.SharedBackend.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorMovies.SharedBackend.Repositories
{
    public class RatingRepository : IRatingRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IAuthenticationStateService authenticationStateService;

        public RatingRepository(ApplicationDbContext context, IAuthenticationStateService authenticationStateService)
        {
            this.context = context;
            this.authenticationStateService = authenticationStateService;
        }

        public async Task Vote(MovieRating movieRating)
        {
            var userId = await authenticationStateService.GetCurrentUserId();
            if (userId == null)
                return;

            var currentRating = await context.MovieRatings
                .FirstOrDefaultAsync(x => x.MovieId == movieRating.MovieId &&
                x.UserId == userId);

            if (currentRating == null)
            {
                movieRating.UserId = userId;
                movieRating.RatingDate = DateTime.Today;
                context.Add(movieRating);
                await context.SaveChangesAsync();
            }
            else
            {
                currentRating.Rate = movieRating.Rate;
                await context.SaveChangesAsync();
            }
        }
    }
}
