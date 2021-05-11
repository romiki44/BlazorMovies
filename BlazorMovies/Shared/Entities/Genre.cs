using BlazorMovies.Shared.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorMovies.Shared.Entities
{
    public class Genre
    {
        public Genre()
        {
            MoviesGenres = new List<MovieGenre>();
        }

        public int Id { get; set; }

        [Required(ErrorMessageResourceType =typeof(Resource), ErrorMessageResourceName =nameof(Resource.required))]
        public string Name { get; set; }

        public List<MovieGenre> MoviesGenres { get; set; }
    }
}
