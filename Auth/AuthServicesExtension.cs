using API.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Shinetech.Common;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace API.Auth
{
    public static class AuthServicesExtension
    {
        public static IServiceCollection AddAuthServices(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            //Get the token security key from config settings
            string environmentKeyValue = configuration[Const.TokenSecurityKey];

            //Add JWT authentication scheme with options
            serviceCollection.AddAuthentication(a =>
            {
                a.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                a.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                a.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ClockSkew = TimeSpan.FromSeconds(30),
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(environmentKeyValue))
                };
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = handleJWTChallenge,
                    OnAuthenticationFailed = handleJWTAuthFailed,
                    OnTokenValidated = handleJWTTokenValidated,
                    OnForbidden = handleJWTAuthForbidden
                };
            });

            //Add authorization policies
            serviceCollection.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                                            .RequireAuthenticatedUser()
                                            .AddRequirements(new AuthRequirement())
                                            .Build();
            });

            serviceCollection.AddScoped<IAuthorizationHandler, AuthHandler>();
            serviceCollection.AddTransient<ICurrentUser, UserAccount>();
            return serviceCollection;

            async Task handleJWTChallenge(JwtBearerChallengeContext context)
            {
                if (!context.Response.HasStarted)
                {
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new CommonResponse() { code = (int)ResponseCode.Unauthorized }));
                }
                context.HandleResponse();
            }

            Task handleJWTAuthFailed(AuthenticationFailedContext context)
            {
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    context.Response.ContentType = "application/json";
                    context.Response.WriteAsync(JsonSerializer.Serialize(new CommonResponse() { code = (int)ResponseCode.TokenExpired }));
                }
                return Task.CompletedTask;
            }

            async Task handleJWTAuthForbidden(ForbiddenContext context)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync(JsonSerializer.Serialize(new CommonResponse() { code = (int)ResponseCode.Forbidden, message = "You are not authorised to perform this action" }));
            }

            Task handleJWTTokenValidated(TokenValidatedContext context)
            {
                return Task.CompletedTask;
            }


        }

    }

}
