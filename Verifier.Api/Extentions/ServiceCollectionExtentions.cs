using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using Application.Interfaces.Services.Identity;
using Domain.Identity;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Verifier.Application.Configurations;
using Verifier.Application.Interfaces.Db;
using Verifier.Application.Interfaces.Services;
using Verifier.Application.Interfaces.Services.Identity;
using Verifier.Domain.Entities.User;
using Verifier.Infrastructure.Services;
using Verifier.Infrastructure.Services.Email;
using Verifier.Shared.WrappersCore.Wrappers;

namespace Verifier.Api.Extentions
{
    public static class ServiceCollectionExtentions
    {
        internal static void AllowSpecificOrigins(this IServiceCollection services, IConfiguration configuration, string specificOrigins)
        {
            var AllowCorsForList = configuration.GetSection("EnableCorsFor").Value.Split(',');

            services.AddCors(options =>
            {
                options.AddPolicy(specificOrigins,
                builder =>
                {
                    builder.WithOrigins(AllowCorsForList)
                                        .AllowAnyHeader()
                                        .AllowAnyMethod()
                                        .WithExposedHeaders("Content-Disposition", "Content-Length"); ;
                });
            });
        }

        internal static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
              x => x.MigrationsAssembly("Infrastructure")))
                   .AddTransient<IDatabaseSeeder, DatabaseSeeder>();

            services.AddScoped<IDatabaseService, ApplicationDbContext>();
        }

        internal static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IRoleClaimService, RoleClaimService>();
            services.AddTransient<ITokenService, IdentityService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IUserService, UserService>();
            services.AddScoped<IAppSettingsService, AppSettingsService>();
            services.AddScoped<IRedisService, RedisService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IAzureBlobStorageService, AzureBlobStorageService>();
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration["RedisCache:Configuration"];
                options.InstanceName = configuration["RedisCache:InstanceName"];
            });

            return services;
        }

        internal static void AddSharedInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SendGridConfiguration>(configuration.GetSection("SendGridConfiguration"));
            services.AddScoped<EmailFactory>();
            services.AddScoped<SendGridMailService>()
                   .AddScoped<IMailService, SendGridMailService>(s => s.GetService<SendGridMailService>());

            services.Configure<SocialConfiguration>(configuration.GetSection("SocialConfiguration"));
            services.AddScoped<SocialProviderFactory>();
            services.AddScoped<ISocialService, SocialService>();

        }


        internal static void AddCurrentUserService(this IServiceCollection services)
        {
            services.AddScoped<ICurrentUserService, CurrentUserService>();
        }

        internal static void RegisterSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(async c =>
            {
                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (!assembly.IsDynamic)
                    {
                        var xmlFile = $"{assembly.GetName().Name}.xml";
                        var xmlPath = Path.Combine(baseDirectory, xmlFile);
                        if (File.Exists(xmlPath))
                        {
                            c.IncludeXmlComments(xmlPath);
                        }
                    }
                }

                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "The Terminal Api",
                    License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    }
                });

                var localizer = await GetRegisteredServerLocalizerAsync<ServerCommonResources>(services);

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = localizer["Input your Bearer token in this format - Bearer {your token here} to access this API"],
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                            Scheme = "Bearer",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        }, new List<string>()
                    },
                });
            });
        }

        internal static void AddHttpService(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddScoped<IHttpService, HttpService>();
        }


        internal static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            services
                .AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
                .AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>()
                .AddIdentity<VerifierUser, VerifierRole>(options =>
                {
                    options.Password.RequiredLength = 6;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            return services;
        }

        internal static void AddJwtAuthentication(this IServiceCollection services, IConfiguration config, IWebHostEnvironment webHostingEnv)
        {
            var identityConfig = services.GetApplicationSettings(config);

            var key = Encoding.ASCII.GetBytes(identityConfig.Secret);
            services
                .AddAuthentication(authentication =>
                {
                    authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                    authentication.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(async bearer =>
                {
                    bearer.RequireHttpsMetadata = false;
                    bearer.SaveToken = true;
                    bearer.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        RoleClaimType = ClaimTypes.Role,
                        ClockSkew = TimeSpan.Zero
                    };

                    var localizer = await GetRegisteredServerLocalizerAsync<ServerCommonResources>(services);

                    bearer.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = c =>
                        {
                            if (c.Exception is SecurityTokenExpiredException)
                            {
                                c.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                c.Response.ContentType = "application/json";
                                var result = JsonConvert.SerializeObject(Result.Fail(localizer["The Token is expired."]));
                                return c.Response.WriteAsync(result);
                            }
                            else
                            {
                                c.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                c.Response.ContentType = "application/json";
                                var result = JsonConvert.SerializeObject(Result.Fail(localizer["An unhandled error has occurred."]));
                                return c.Response.WriteAsync(result);
                            }
                        },
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            if (!context.Response.HasStarted)
                            {
                                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                context.Response.ContentType = "application/json";
                                var result = JsonConvert.SerializeObject(Result.Fail(localizer["You are not Authorized."]));
                                return context.Response.WriteAsync(result);
                            }

                            return Task.CompletedTask;
                        },
                        OnForbidden = context =>
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                            context.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(Result.Fail(localizer["You are not authorized to access this resource."]));
                            return context.Response.WriteAsync(result);
                        },
                    };
                })
                .AddCookie()
                .AddGoogle(googleOptions =>
                {
                    IConfigurationSection googleAuthNSection = config.GetSection("ThirdPartAuthentication:Google");
                    googleOptions.ClientId = googleAuthNSection["ClientId"];
                    googleOptions.ClientSecret = googleAuthNSection["ClientSecret"];
                    googleOptions.SaveTokens = true;


                })
                .AddMicrosoftAccount(microsoftOptions =>
                {
                    microsoftOptions.ClientId = config["ThirdPartAuthentication:Microsoft:ClientId"];
                    microsoftOptions.ClientSecret = config["ThirdPartAuthentication:Microsoft:ClientSecret"];
                    microsoftOptions.SaveTokens = true;

                })
                .AddApple(appleOptions =>
                 {

                     appleOptions.ClientId = config["ThirdPartAuthentication:Apple:ClientId"];
                     appleOptions.KeyId = config["ThirdPartAuthentication:Apple:KeyId"];
                     appleOptions.TeamId = config["ThirdPartAuthentication:Apple:TeamId"];
                     appleOptions.UsePrivateKey(keyId
                          => webHostingEnv.ContentRootFileProvider.GetFileInfo($"AuthKey_{keyId}.p8"));
                     appleOptions.SaveTokens = true;

                 });


            services.AddAuthorization(options =>
            {
                foreach (var prop in typeof(Permissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
                {
                    var propertyValue = prop.GetValue(null);
                    if (propertyValue is not null)
                    {
                        options.AddPolicy(propertyValue.ToString(), policy => policy.RequireClaim(ApplicationClaimTypes.Permission, propertyValue.ToString()));
                    }
                }
            });
        }

        internal static IdentityConfiguration GetApplicationSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var applicationSettingsConfiguration = configuration.GetSection(nameof(IdentityConfiguration));
            services.Configure<IdentityConfiguration>(applicationSettingsConfiguration);
            return applicationSettingsConfiguration.Get<IdentityConfiguration>();
        }

        internal static async Task<IStringLocalizer> GetRegisteredServerLocalizerAsync<T>(this IServiceCollection services) where T : class
        {
            var serviceProvider = services.BuildServiceProvider();
            await SetCultureFromServerPreferenceAsync(serviceProvider);
            var localizer = serviceProvider.GetService<IStringLocalizer<T>>();
            await serviceProvider.DisposeAsync();
            return localizer;
        }

        private static async Task SetCultureFromServerPreferenceAsync(IServiceProvider serviceProvider)
        {
            var storageService = serviceProvider.GetService<ServerPreferenceManager>();
            if (storageService != null)
            {
                CultureInfo culture;
                var preference = await storageService.GetPreference() as ServerPreference;
                if (preference != null)
                    culture = new CultureInfo(preference.LanguageCode);
                else
                    culture = new CultureInfo(LocalizationConstants.SupportedLanguages.FirstOrDefault()?.Code ?? "en-US");
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
            }
        }

        public static void RegisterCustomAuthorization(this IServiceCollection services)
        {

            services.AddScoped<IAuthorizationHandler, ChatGroupAdministrationHandler>();

            services.AddAuthorization(authorizationOptions =>
            {
                authorizationOptions.AddPolicy(
                   ChatSettingsConstants.CanAddUsers,
                   policyBuilder =>
                   {
                       policyBuilder.RequireAuthenticatedUser();
                       policyBuilder.AddRequirements(
                             new ChatGroupAdministrationRequirments(ChatSettingsConstants.CanAddUsers));
                   });

                authorizationOptions.AddPolicy(
                  ChatSettingsConstants.CanRemoveUsers,
                  policyBuilder =>
                  {
                      policyBuilder.RequireAuthenticatedUser();
                      policyBuilder.AddRequirements(
                            new ChatGroupAdministrationRequirments(ChatSettingsConstants.CanRemoveUsers));
                  });

                authorizationOptions.AddPolicy(
                  ChatSettingsConstants.CanChangeGroupName,
                  policyBuilder =>
                  {
                      policyBuilder.RequireAuthenticatedUser();
                      policyBuilder.AddRequirements(
                            new ChatGroupAdministrationRequirments(ChatSettingsConstants.CanChangeGroupName));
                  });

                authorizationOptions.AddPolicy(
                  ChatSettingsConstants.CanChangeGroupPicture,
                  policyBuilder =>
                  {
                      policyBuilder.RequireAuthenticatedUser();
                      policyBuilder.AddRequirements(
                            new ChatGroupAdministrationRequirments(ChatSettingsConstants.CanChangeGroupPicture));
                  });

                authorizationOptions.AddPolicy(
                 ChatSettingsConstants.CanChangeSettings,
                 policyBuilder =>
                 {
                     policyBuilder.RequireAuthenticatedUser();
                     policyBuilder.AddRequirements(
                           new ChatGroupAdministrationRequirments(ChatSettingsConstants.CanChangeSettings));
                 });

                authorizationOptions.AddPolicy(
                ChatGroupPermissionsConstants.Member,
                policyBuilder =>
                {
                    policyBuilder.RequireAuthenticatedUser();
                    policyBuilder.AddRequirements(
                          new ChatGroupAdministrationRequirments(ChatGroupPermissionsConstants.Member));
                });



            });



        }



    }
}
