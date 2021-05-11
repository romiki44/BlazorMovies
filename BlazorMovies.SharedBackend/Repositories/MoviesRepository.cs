﻿using AutoMapper;
using BlazorMovies.Shared.DTOs;
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
    public class MoviesRepository : IMoviesRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IAuthenticationStateService authenticationStateService;
        private readonly IFileStorageService fileStorageService;
        private readonly IMapper mapper;
        private readonly string containerName = "movies";

        public MoviesRepository(ApplicationDbContext context, 
                                IAuthenticationStateService authenticationStateService, 
                                IFileStorageService fileStorageService,
                                IMapper mapper)
        {
            this.context = context;
            this.authenticationStateService = authenticationStateService;
            this.fileStorageService = fileStorageService;
            this.mapper = mapper;
        }

        public async Task<int> CreateMovie(Movie movie)
        {
            if (!string.IsNullOrWhiteSpace(movie.Poster))
            {
                var poster = Convert.FromBase64String(movie.Poster);
                movie.Poster = await fileStorageService.SaveFile(poster, "jpg", containerName);
            }

            if (movie.MoviesActors != null)
            {
                for (int i = 0; i < movie.MoviesActors.Count; i++)
                {
                    movie.MoviesActors[i].Order = i + 1;
                }
            }

            context.Add(movie);
            await context.SaveChangesAsync();
            return movie.Id;
        }

        public async Task DeleteMovie(int id)
        {
            var movie = await context.Movies.FindAsync(id);
            if (movie == null)
                return;

            context.Remove(movie);
            await context.SaveChangesAsync();
        }

        public async Task<DetailsMovieDTO> GetDetailsMovieDTO(int id)
        {
            var movie = await context.Movies.Where(x => x.Id == id)
                .Include(x => x.MoviesGenres).ThenInclude(x => x.Genre)
                .Include(x => x.MoviesActors).ThenInclude(x => x.Person)
                .FirstOrDefaultAsync();

            if (movie == null)
                return null;

            var voteAverage = 0.0;
            var userVote = 0;
            if (await context.MovieRatings.AnyAsync(x => x.MovieId == id))
            {
                voteAverage = await context.MovieRatings.Where(x => x.MovieId == id).AverageAsync(x => x.Rate);

                var userId = await authenticationStateService.GetCurrentUserId();

                if (userId!=null)
                {
                    var userVoteDb = await context.MovieRatings.FirstOrDefaultAsync(x => x.MovieId == id && x.UserId == userId);
                    if (userVoteDb != null)
                        userVote = userVoteDb.Rate;
                }
            }

            movie.MoviesActors = movie.MoviesActors.OrderBy(x => x.Order).ToList();

            var model = new DetailsMovieDTO();
            model.Movie = movie;
            model.Genres = movie.MoviesGenres.Select(x => x.Genre).ToList();
            model.Actors = movie.MoviesActors.Select(x =>
                new Person
                {
                    Name = x.Person.Name,
                    Picture = x.Person.Picture,
                    Character = x.Character,
                    Id = x.PersonId

                }).ToList();

            model.UserVote = userVote;
            model.AverageVote = voteAverage;
            return model;
        }

        public async Task<IndexPageDTO> GetIndexPageDTO()
        {
            var userId = await authenticationStateService.GetCurrentUserId();

            var limit = 6;

            var moviesInTheaters = await context.Movies
                .Where(x => x.InTheater).Take(limit)
                .OrderByDescending(x => x.ReleaseDate)
                .ToListAsync();

            var todaysDate = DateTime.Today;

            var upcomingReleases = await context.Movies
                .Where(x => x.ReleaseDate > todaysDate)
                .OrderBy(x => x.ReleaseDate).Take(limit)
                .ToListAsync();

            var response = new IndexPageDTO();
            response.Intheaters = moviesInTheaters;
            response.UpcomingReleases = upcomingReleases;

            return response;
        }

        public async Task<MovieUpdateDTO> GetMovieForUpdate(int id)
        {
            var movieDetailDTO = await GetDetailsMovieDTO(id);
            if (movieDetailDTO == null)
                return null;

            var selectedGenresIds = movieDetailDTO.Genres.Select(x => x.Id).ToList();
            var notSelectedGenres = await context.Genres
                .Where(x => !selectedGenresIds.Contains(x.Id))
                .ToListAsync();

            var model = new MovieUpdateDTO();
            model.Movie = movieDetailDTO.Movie;
            model.SelectedGenres = movieDetailDTO.Genres;
            model.NotSelectedGenres = notSelectedGenres;
            model.Actors = movieDetailDTO.Actors;
            return model;
        }

        public async Task<PaginatedResponse<List<Movie>>> GetMoviesFiltered(FilterMoviesDTO filterMoviesDTO)
        {
            var moviesQueryable = context.Movies.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filterMoviesDTO.Title))
            {
                moviesQueryable = moviesQueryable
                    .Where(x => x.Title.Contains(filterMoviesDTO.Title));
            }

            if (filterMoviesDTO.InTheaters)
            {
                moviesQueryable = moviesQueryable.Where(x => x.InTheater);
            }

            if (filterMoviesDTO.UpcomingReleases)
            {
                var today = DateTime.Today;
                moviesQueryable = moviesQueryable.Where(x => x.ReleaseDate > today);
            }

            if (filterMoviesDTO.GenreId != 0)
            {
                moviesQueryable = moviesQueryable
                    .Where(x => x.MoviesGenres.Select(y => y.GenreId)
                    .Contains(filterMoviesDTO.GenreId));
            }

            var movies = await moviesQueryable.GetPaginatedResponseAsync(filterMoviesDTO.Pagination);

            return movies;
        }

        public async Task UpdateMovie(Movie movie)
        {
            context.Entry(movie).State = EntityState.Detached;

            var movieDB = await context.Movies
                .Include(x=>x.MoviesActors)
                .Include(x=>x.MoviesGenres)
                .FirstOrDefaultAsync(x => x.Id == movie.Id);

            if (movieDB == null)
                return;

            mapper.Map(movie, movieDB);

            if (!string.IsNullOrWhiteSpace(movie.Poster))
            {
                var moviePoster = Convert.FromBase64String(movie.Poster);
                movieDB.Poster = await fileStorageService.EditFile(moviePoster,
                    "jpg", containerName, movieDB.Poster);
            }

            await context.Database.ExecuteSqlInterpolatedAsync($"delete from MovieActors where MovieId = {movie.Id}; delete from MovieGenres where MovieId = {movie.Id}");

            if (movie.MoviesActors != null)
            {
                for (int i = 0; i < movie.MoviesActors.Count; i++)
                {
                    movie.MoviesActors[i].Order = i + 1;
                }
            }

            movieDB.MoviesActors = movie.MoviesActors;
            movieDB.MoviesGenres = movie.MoviesGenres;

            await context.SaveChangesAsync();
        }
    }
}
