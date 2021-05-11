using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorMovies.Shared.Entities
{
    public class Movie
    {
        public Movie()
        {
            MoviesGenres = new List<MovieGenre>();
        }

        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        public string Summary { get; set; }
        public bool InTheater { get; set; }
        public string Trailer { get; set; }
        
        [Required]
        public DateTime? ReleaseDate { get; set; }
        public string Poster { get; set; }
        public List<MovieGenre> MoviesGenres { get; set; } = new List<MovieGenre>();
        public List<MovieActor> MoviesActors { get; set; } = new List<MovieActor>();
        public string TitleBrief 
        {
            get 
            {
                if (string.IsNullOrEmpty(Title))
                    return null;

                if (Title.Length > 60)
                    return Title.Substring(0, 60) + "...";

                return Title;
            }
        }

    }
}
