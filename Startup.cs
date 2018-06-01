using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BahamutCommon.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using BTBaseServices.DAL;
using BTBaseServices.Services;
using BTBaseServices;
using BTBaseServices.Models;

namespace BTBaseWebAPI
{
    public class Startup
    {
        private readonly string AppName = "BTBaseWebAPI";
        private readonly string ValidIssuer = "BTBaseAuth";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public IServiceCollection ServiceCollection { get; private set; }
        public IApplicationBuilder ApplicatonBuilder { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            this.ServiceCollection = services;

            services.AddMvc(ac => { })
            .AddJsonOptions(op =>
            {
                op.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                op.SerializerSettings.Formatting = Formatting.None;
            });

            services.AddSingleton<AccountService>();
            services.AddSingleton<MemberService>();
            services.AddSingleton<SessionService>();
            services.AddSingleton<SecurityCodeService>();
            services.AddDbContextPool<BTBaseDbContext>(builder =>
            {
                builder.UseMySQL(Environment.GetEnvironmentVariable("MYSQL_CONSTR"));
            });
            AddAuthentication(this.ServiceCollection);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            ApplicatonBuilder = app;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            TryConnectDB(env.IsDevelopment());
            app.UseAuthentication();
            app.UseMvc();
        }

        private void TryConnectDB(bool isDevelopment)
        {
            using (var sc = ApplicatonBuilder.ApplicationServices.CreateScope())
            {
                try
                {
                    var dbContext = sc.ServiceProvider.GetService<BTBaseDbContext>();
                    if (isDevelopment)
                    {
                        dbContext.Database.EnsureCreated();
                    }
                    Console.WriteLine("Connect DB Success");
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine("Connect DB Error:" + ex.ToString());
                }
            }
        }

        private void AddAuthentication(IServiceCollection services)
        {
            services.AddAuthentication("Bearer").AddJwtBearer(jwtOptions =>
            {
                var securityKey = GetIssuerSigningKey(ApplicatonBuilder.ApplicationServices);
                jwtOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = securityKey,
                    ValidateAudience = true,
                    ValidAudience = AppName,
                    ValidateIssuer = true,
                    ValidIssuer = ValidIssuer,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5)
                };
            });
        }

        private SecurityKey GetIssuerSigningKey(IServiceProvider serviceProvider)
        {
            var pubkeyBase64 = Environment.GetEnvironmentVariable("ISSUER_SIGNING_KEY") ?? Configuration.GetValue<string>("issuer_signing_key");
            var rsaParam =
            new SecurityKeychain { PublicKey = pubkeyBase64, Algorithm = SecurityKeychainRSAExtensions.ALGORITHM_RSA }
            .ReadRSAParameters(false);
            return new RsaSecurityKey(rsaParam);
        }
    }
}
