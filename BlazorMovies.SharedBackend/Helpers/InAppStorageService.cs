using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;

namespace BlazorMovies.SharedBackend.Helpers
{
    public class InAppStorageService : IFileStorageService
    {
        private readonly IHostingEnvironment env;
        //private readonly IWebHostEnvironment env;
        private readonly IHttpContextAccessor httpContextAccessor;

        public InAppStorageService(IHostingEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            env = webHostEnvironment;
            //this.webHostEnvironment = webHostEnvironment;
            this.httpContextAccessor = httpContextAccessor;
        }

        public Task DeleteFile(string fileRoute, string containerName)
        {
            var fileName = Path.GetFileName(fileRoute);
            string filePath = Path.Combine(env.WebRootPath, containerName, fileName);
            if (File.Exists(filePath))
                File.Delete(filePath);

            return Task.FromResult(0);
        }

        public async Task<string> EditFile(byte[] content, string extension, string containerName, string fileRoute)
        {
            if (!string.IsNullOrEmpty(fileRoute))
                await DeleteFile(fileRoute, containerName);

            return await SaveFile(content, extension, containerName);
        }

        public async Task<string> SaveFile(byte[] content, string extension, string containerName)
        {
            var fileName = $"{Guid.NewGuid()}.{extension}";
            string folder = Path.Combine(env.WebRootPath, containerName);

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string savingPath = Path.Combine(folder, fileName);
            await File.WriteAllBytesAsync(savingPath, content);

            // pri dual-architekture to robi zmatky, lebo hlavna url sa vzdy zmeni
            // mala by sa ukladat len cesta bez hlavnej url, ale potom to treab osetrit aj inde
            // zatial to necham tak....
            var currentUrl = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}";
            var pathForDatabase = Path.Combine(currentUrl, containerName, fileName);
            return pathForDatabase;
        }
    }
}
