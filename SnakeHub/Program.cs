using Microsoft.AspNetCore.Authentication;
using SnakeHub.Handlers;
using SnakeHub.Services;

namespace SnakeHub
{
    public class Program
	{
		public static void Main(string[] args)
		{
			WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllersWithViews();
            builder.WebHost.UseStaticWebAssets();

			builder.Services.AddScoped<JwtService>();
            builder.Services.AddAuthentication("JwtScheme").AddScheme<AuthenticationSchemeOptions, JwtAuthHandler>("JwtScheme", options => { });
            builder.Services.AddAuthorization();

            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();

            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
			{
				app.UseStatusCodePagesWithReExecute("/Error/Index");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();
            app.MapBlazorHub();

            app.Run();
		}
    }
}
