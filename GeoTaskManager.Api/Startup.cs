using FluentValidation.AspNetCore;
using GeoTaskManager.Api.Core.Filters;
using GeoTaskManager.Api.GeoTasks.Models;
using GeoTaskManager.Application.Core.Data;
using GeoTaskManager.MongoDb;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace GeoTaskManager.Api
{
    internal class Startup
    {
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. 
        // Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Use Seq server for log analysis in development
            if (_env.IsDevelopment())
            {
                services.AddLogging(loggingBuilder =>
                {
                    loggingBuilder.AddSeq();
                });
            }
            else
            {
                services.AddLogging();
            }


            services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme =
                            JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme =
                            JwtBearerDefaults.AuthenticationScheme;
                    }
            ).AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = Configuration["OAuth20:Issuer"],
                            ValidAudience = Configuration["OAuth20:Audience"],
                            IssuerSigningKey = new SymmetricSecurityKey
                                (Encoding.UTF8.GetBytes
                                    (Configuration["OAuth20:Secret"])),
                            NameClaimType = Configuration["OAuth20:UserNameClaim"],
                            RoleClaimType = Configuration["OAuth20:UserRoleClaim"],

                        };
                        if (_env.IsDevelopment())
                        {
                            options.RequireHttpsMetadata = false;
                        }
                    }
            );

            // If configuration sets e-mail verification rules then add policy 
            // or don't use e-mail verification rule in the policy
            var emailVerificationClaimName = Configuration["OAuth20:EmailVerificationClaimName"];
            var emailVerificationClaimValue = Configuration["OAuth20:EmailVerificationClaimValue"];
            if (!String.IsNullOrWhiteSpace(emailVerificationClaimName)
                && !String.IsNullOrWhiteSpace(emailVerificationClaimValue))
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy("EmailVerified",
                        policy => policy.RequireClaim(emailVerificationClaimName,
                            emailVerificationClaimValue));
                });

            }
            else
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy("EmailVerified",
                        policy => policy.RequireAuthenticatedUser());
                });
            }


            services.AddControllers()
                .AddNewtonsoftJson();

            services
                .AddMvc(options => options.Filters.Add<ApiValidationFilter>())
                .AddFluentValidation
                (
                    config =>
                    {
                        config
                            .RegisterValidatorsFromAssemblyContaining
                                <Startup>();
                        config
                            .RegisterValidatorsFromAssemblyContaining
                                <IGeoTaskManagerDbContext>();
                    }
                );

            services.AddCors(
                options => options.AddDefaultPolicy
                (
                    builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .Build()
                )
            );

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "GeoTaskManager API",
                    Description = "The back-end for task management " +
                        "with geoposition visualization",
                    Contact = new OpenApiContact
                    {
                        Name = "Artem Kuskov",
                        Email = "a.kuskov@bk.ru",
                        Url = new Uri("https://linkedin.com/in/artem-kuskov"),
                    },
                });

                c.MapType<JsonPatchDocument<ApiGeoTask>>(
                    () => new OpenApiSchema
                    {
                        Type = "array",
                        Items = new OpenApiSchema
                        {
                            Type = "object",
                            Items = new OpenApiSchema
                            {
                                Type = "string",
                                Title = "op"
                            },
                        }
                    });

                // Swagger authentication by JWT token
                var jwtSchema = new OpenApiSecurityScheme
                {
                    Scheme = "Bearer",
                    Type = SecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Description = "Insert 'Bearer YOUR_JWT_TOKEN'"
                };
                c.AddSecurityDefinition("Bearer", jwtSchema);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                // Set the comments path for the Swagger JSON and UI.
                var assemblyName = Assembly
                    .GetExecutingAssembly()
                    .GetName()
                    .Name;
                var xmlFile = $"{assemblyName}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddMediatR(new Assembly[]
                {
                    Assembly.GetExecutingAssembly()
                });
            services.AddGeoTaskApplication();
            services.AddGeoTaskMongoDb();
        }

        // This method gets called by the runtime. 
        // Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json",
                    "GeoTaskManager API V1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseCors();
        }
    }
}
