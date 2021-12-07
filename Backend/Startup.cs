using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.OpenApi.Models;
using Auth0.ManagementApi;

namespace Backend
{
    public static class Startup
    {
        internal static void ConfigureServices(IServiceCollection services, WebApplicationBuilder builder)
        {
            var Configuration = builder.Services.BuildServiceProvider().GetRequiredService<IConfiguration>();
            // Add services to the container.
            services.AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DataContext")));
            services.TryAddTransient<MovieRepository>();
            services.AddSingleton<WeatherForecastService>();
            services.AddSingleton<IManagementConnection, HttpClientManagementConnection>();
            services.TryAddTransient<Auth0Service>();

            services.AddControllers();

            services.AddRazorPages();
            services.AddServerSideBlazor();

            ConfigureAuth(services, Configuration);

            services.AddHttpContextAccessor();

            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(2, 0);
            });

            services.AddVersionedApiExplorer(setup =>
            {
                setup.GroupNameFormat = "'v'VVV";
                setup.SubstituteApiVersionInUrl = true;
            });
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options => SetupSwagger(options));

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/build"; });
        }

        internal static void ConfigureAuth(IServiceCollection services, IConfiguration Configuration)
        {
            var domain = $"https://{Configuration["Auth0:Domain"]}";
            services.Configure<CookiePolicyOptions>(options =>
            {
            // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // Add authentication services
            services.AddAuthentication(options =>
            {
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = "smart";
                options.DefaultChallengeScheme = "smart";
            }).
            AddPolicyScheme("smart", "Authorization Bearer or OIDC", options =>
            {
                options.ForwardDefaultSelector = context =>
                {
                    var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

                    if (authHeader?.StartsWith("Bearer ") == true) return JwtBearerDefaults.AuthenticationScheme;
                    return CookieAuthenticationDefaults.AuthenticationScheme;
                };
            })
            .AddJwtBearer(options =>
            {
                options.Authority = domain;
                options.Audience = Configuration["Auth0:Audience"];
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidAudience = Configuration["Auth0:Audience"],
                    ValidIssuer = domain,
                    NameClaimType = ClaimTypes.NameIdentifier,
                };
            })
            .AddCookie()
            .AddOpenIdConnect("Auth0", options =>
            {
            // Set the authority to your Auth0 domain
            options.Authority = domain;

            // Configure the Auth0 Client ID and Client Secret
            options.ClientId = Configuration["Auth0:ClientId"];
                options.ClientSecret = Configuration["Auth0:ClientSecret"];

            // Set response type to code
            options.ResponseType = "code";

            // Configure the scope
            options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");

            // Set the callback path, so Auth0 will call back to http://localhost:3000/admin/auth/callback
            // Also ensure that you have added the URL as an Allowed Callback URL in your Auth0 dashboard
            options.CallbackPath = new PathString("/callback");

            // Configure the Claims Issuer to be Auth0
            options.ClaimsIssuer = "Auth0";

                options.Events = new OpenIdConnectEvents
                {
                // handle the logout redirection
                OnRedirectToIdentityProviderForSignOut = (context) =>
            {
                        var logoutUri = $"{domain}/v2/logout?client_id={Configuration["Auth0:ClientId"]}";

                        var postLogoutUri = context.Properties.RedirectUri;
                        if (!string.IsNullOrEmpty(postLogoutUri))
                        {
                            if (postLogoutUri.StartsWith("/"))
                            {
                            // transform to absolute
                            var request = context.Request;
                                postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + postLogoutUri;
                            }
                            logoutUri += $"&returnTo={ Uri.EscapeDataString(postLogoutUri)}";
                        }

                        context.Response.Redirect(logoutUri);
                        context.HandleResponse();

                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyRole.WRITER, policy => policy.RequireClaim("permissions", "perm:writer"));
                options.AddPolicy(PolicyRole.EDITOR, policy => policy.RequireClaim("permissions", "perm:editor"));
                options.AddPolicy(PolicyRole.MANAGER, policy => policy.RequireClaim("permissions", "perm:manager"));
                options.AddPolicy(PolicyRole.ADMIN, policy => policy.RequireClaim("permissions", "perm:admin"));
            });
        }

        internal static void SetupSwagger(SwaggerGenOptions options)
        {
            // Set the comments path for the Swagger JSON and UI.
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);

            var securitySchema = new OpenApiSecurityScheme
            {
                Description = "Using the Authorization header with the Bearer scheme.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };

            options.AddSecurityDefinition("Bearer", securitySchema);

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { securitySchema, new[] { "Bearer" } }
                });
        }

        public static void ConfigureSwagger(WebApplication app, WebApplicationBuilder builder)
        {
            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                var provider = builder.Services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    string version = description.GroupName.ToLowerInvariant();
                    string name = version + " - Demo API";
                    if (description.IsDeprecated)
                    {
                        name += " (deprecated)";
                    }
                    options.SwaggerEndpoint($"/swagger/{version}/swagger.json", name);
                }
            });
        }
    }
}