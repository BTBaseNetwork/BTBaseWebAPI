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
using JwtUtils;
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
        private readonly string SERVER_NAME = "BTBaseWebAPI";
        private readonly string VALID_ISSUER = "BTBaseAuth";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IApplicationBuilder app, IServiceCollection services)
        {

            // 配置其他
            services.AddMvc(ac => { })
            .AddJsonOptions(op =>
            {
                op.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                op.SerializerSettings.Formatting = Formatting.None;
            });

            services.AddSingleton<AccountService>();
            services.AddSingleton<MemberService>();
            services.AddSingleton<SessionService>();
            services.AddDbContextPool<BTBaseDbContext>(builder =>
            {
                builder.UseMySQL(Environment.GetEnvironmentVariable("MYSQL_CONSTR"));
            });
            AddAuthentication(app, services);
        }

        private void TryConnectDB(IApplicationBuilder app)
        {
            using (var sc = app.ApplicationServices.CreateScope())
            {
                try
                {
                    var dbContext = sc.ServiceProvider.GetService<BTBaseDbContext>();
                    dbContext.Database.EnsureCreated();
                    Console.WriteLine("Connect DB Success");
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine("Connect DB Error:" + ex.ToString());
                }
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMvc();
            TryConnectDB(app);
        }

        private void AddAuthentication(IApplicationBuilder app, IServiceCollection services)
        {
            BTWebServerAuthKey authKey;
            var securityKey = GetAuthenticationKey(app.ApplicationServices, out authKey);
            services.AddAuthentication().AddJwtBearer(jwtOptions =>
            {
                jwtOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = securityKey,
                    ValidateAudience = true,
                    ValidAudience = SERVER_NAME,
                    ValidateIssuer = true,
                    ValidIssuer = VALID_ISSUER,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5)
                };
            });
        }

        private SecurityKey GetAuthenticationKey(IServiceProvider serviceProvider, out BTWebServerAuthKey authKey)
        {
            using (var sc = serviceProvider.CreateScope())
            {
                var dbContext = sc.ServiceProvider.GetService<BTBaseDbContext>();
                try
                {
                    authKey = dbContext.BTWebServerAuthKey.First(x => x.ServerName == SERVER_NAME);
                    return ServerAuthKeyUtils.ConvertToKey<SecurityKey>(authKey);
                }
                catch (System.InvalidOperationException)
                {
                    var skey = ServerAuthKeyUtils.CreateNewSecurityKey<SecurityKey>(ServerAuthKeyUtils.ALGORITHM_RSA);
                    var res = dbContext.BTWebServerAuthKey.Add(ServerAuthKeyUtils.GenerateAuthKey(SERVER_NAME, skey));
                    authKey = res.Entity;
                    return skey;
                }
            }
        }
    }
}
