
using KASHOP.BLL.Mapping;
using KASHOP.BLL.Service;
using KASHOP.DAL.Data;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repositry;
using KASHOP.DAL.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.PL
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddDbContext<ApplicationDbContext>(option =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            // Language
            builder.Services.AddLocalization(options => options.ResourcesPath = "");
            const string defaultCulture = "en";

            var supportedCultures = new[]
            {
            new CultureInfo(defaultCulture),
            new CultureInfo("ar")
            };

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(defaultCulture);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                // بدي اغبير هاي https://localhost:7189/api/categories?Culture=en الى https://localhost:7189/api/categories?lan=en

                options.RequestCultureProviders.Clear();
                //options.RequestCultureProviders.Add(new QueryStringRequestCultureProvider
                //{
                //    QueryStringKey = "lan"
                //});


                //افضل طريقه ببعت الداتا في header
                options.RequestCultureProviders.Add(new AcceptLanguageHeaderRequestCultureProvider());

            });
            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  policy =>
                                  {
                                      policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                                  });
            });
            builder.Services.AddScoped<ICategoryRepository,CategoryRepository>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IAuthenticationService,AuthenticationService>();
            builder.Services.AddScoped<ISeedData, RolesSeedData>();
            builder.Services.AddTransient<IEmailSender, EmailSender>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IFileService, FileService>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IBrandService, BrandService>();
            builder.Services.AddScoped<IBrandRepository, BrandRepository>();

            using var scoped = builder.Services.BuildServiceProvider().CreateScope();
            var fileService = scoped.ServiceProvider.GetRequiredService<IFileService>();
            MapsterConfig.MapesterConfigRegister(fileService);

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireDigit = true;

                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })

        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,//name of project
                ValidateAudience = true,//domain
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
            };
        });
            var app = builder.Build();

            app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseCors(MyAllowSpecificOrigins);

            builder.Services.AddAuthorization();

            app.UseAuthorization();


            app.UseStaticFiles();
            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var seeders = services.GetServices<ISeedData>();
                foreach (var seeder in seeders)
                {
                    await seeder.DataSeed();
                }
            }

            app.Run();
        }
    }
}
