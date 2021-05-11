using BlazorMovies.Shared.DTOs;
using BlazorMovies.Shared.Repositories;
using BlazorMovies.SharedBackend.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BlazorMovies.SharedBackend.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<IdentityUser> userManager;

        public UsersRepository(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }
        
        public async Task<List<RoleDTO>> GetRoles()
        {
            return await context.Roles
                .Select(x => new RoleDTO { RoleName = x.Name }).ToListAsync();
        }

        public async Task<PaginatedResponse<List<UserDTO>>> GetUsers(PaginationDTO paginationDTO)
        {
            var queryable = context.Users.AsQueryable();
            var paginatedResponse = await queryable.GetPaginatedResponseAsync(paginationDTO);
                
            var usersDto=paginatedResponse.Response
                .Select(x => new UserDTO { Email = x.Email, UserId = x.Id }).ToList();

            var paginatedResponseDto = new PaginatedResponse<List<UserDTO>>
            {
                Response = usersDto,
                TotalAmountPages = paginatedResponse.TotalAmountPages
            };

            return paginatedResponseDto;
        }

        public async Task AssignRole(EditRoleDTO editRoleDto)
        {
            var user = await userManager.FindByIdAsync(editRoleDto.UserId);
            await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, editRoleDto.RoleName));
        }

        public async Task RemoveRole(EditRoleDTO editRoleDto)
        {
            var user = await userManager.FindByIdAsync(editRoleDto.UserId);
            await userManager.RemoveClaimAsync(user, new Claim(ClaimTypes.Role, editRoleDto.RoleName));
        }
    }
}
