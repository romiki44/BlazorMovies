using BlazorMovies.Client.Auth;
using BlazorMovies.Client.Helpers;
using BlazorMovies.Client.Repository;
using BlazorMovies.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BlazorMovies.Shared.Repositories;
using BlazorMovies.Components.Helpers;
using Microsoft.JSInterop;
using System.Globalization;

namespace BlazorMovies.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            ConfigureServices(builder.Services);

            var host = builder.Build();

            var js = host.Services.GetRequiredService<IJSRuntime>();
            var culture = await js.InvokeAsync<string>("getFromLocalStorage", "culture");
            if(culture==null)
            { 
                var selectedCulture = new CultureInfo("en-US");
                CultureInfo.DefaultThreadCurrentCulture = selectedCulture;
                CultureInfo.DefaultThreadCurrentUICulture = selectedCulture;
            }
            else
            {
                var selectedCulture = new CultureInfo(culture);
                // tyka sa meny, datumov, formatu cisiel a pod...
                // nesatci ale iba zmenit new CultureInfo("en-US");...to potom nefunguje!!!
                // treba nastavit v MainLayout....CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
                // a v CultureSelectore....return CultureInfo.CurrentUICulture;  nie iba CurrentCulture!!
                CultureInfo.DefaultThreadCurrentCulture = selectedCulture; 
                // len pouzivanie recource fajlov
                CultureInfo.DefaultThreadCurrentUICulture = selectedCulture;
            }

            await host.RunAsync();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddLocalization();

            services.AddTransient<IRepository, RepositoryInMemory>();
            services.AddScoped<IHttpService, HttpService>();
            services.AddScoped<IGenreRepository, GenreRepository>();
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<IMoviesRepository, MoviesRepository>();
            services.AddScoped<IAccountsRepository, AccountsRepository>();
            services.AddScoped<IRatingRepository, RatingRepository>();
            services.AddScoped<IDisplayMessage, DisplayMessage>();
            services.AddScoped<IUsersRepository, UserRepository>();

            services.AddAuthorizationCore();

            services.AddScoped<JwtAuthenticationStateProvider>();
            services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>(provider=>
                provider.GetRequiredService<JwtAuthenticationStateProvider>());
            services.AddScoped<ILoginService, JwtAuthenticationStateProvider>(provider =>
                provider.GetRequiredService<JwtAuthenticationStateProvider>());

            services.AddScoped<TokenRenewer>();

            services.AddScoped<ExampleJsInterop>();
        }
    }
}
