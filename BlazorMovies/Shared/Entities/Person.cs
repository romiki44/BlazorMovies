using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorMovies.Shared.Entities
{
    public class Person
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Biography { get; set; }
        public string Picture { get; set; } //= "tom_holland2.jpg";

        [Required]
        public DateTime? DateOfBirth { get; set; }
        public List<MovieActor> MoviesActors { get; set; } = new List<MovieActor>();

        [NotMapped]
        public string Character { get; set; }

        public override bool Equals(object obj)
        {
            if(obj is Person p2)
            {
                return Id == p2.Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
