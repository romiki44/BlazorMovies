using AutoMapper;
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
    public class PersonRepositoiry : IPersonRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IFileStorageService fileStorageService;
        private readonly IMapper mapper;
        private readonly string containerName = "people";

        public PersonRepositoiry(ApplicationDbContext context, IFileStorageService storageService, IMapper mapper)
        {
            this.context = context;
            this.fileStorageService = storageService;
            this.mapper = mapper;
        }

        public async Task<PaginatedResponse<List<Person>>> GetPeople(PaginationDTO paginationDTO)
        {
            var queryable = context.People.AsQueryable();
            return await queryable.GetPaginatedResponseAsync(paginationDTO);
        }

        public async Task<List<Person>> GetPeopleByName(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
                return new List<Person>();

            return await context.People.Where(x => x.Name.Contains(searchText))
                .Take(5)
                .ToListAsync();
        }

        public async Task<Person> GetPersonById(int id)
        {
            return await context.People.FindAsync(id);
        }

        public async Task CreatePerson(Person person)
        {
            if (!string.IsNullOrEmpty(person.Picture))
            {
                var personPicture = Convert.FromBase64String(person.Picture);
                person.Picture = await fileStorageService.SaveFile(personPicture, "jpg", containerName);
            }

            context.Add(person);
            await context.SaveChangesAsync();
        }

        public async Task DeletePerson(int id)
        {
            var personDb = await GetPersonById(id);
            if (personDb == null)
                return;

            context.Remove(personDb);
            await context.SaveChangesAsync();
        }        

        public async Task UpdatePerson(Person person)
        {
            context.Entry(person).State = EntityState.Detached;

            var personDb = await GetPersonById(person.Id);
            if (personDb == null)
                return;

            personDb = mapper.Map(person, personDb);

            if (!string.IsNullOrWhiteSpace(person.Picture))
            {
                var personPicture = Convert.FromBase64String(person.Picture);
                personDb.Picture = await fileStorageService.EditFile(personPicture, "jpg", containerName, personDb.Picture);
            }

            await context.SaveChangesAsync();
        }
    }
}
