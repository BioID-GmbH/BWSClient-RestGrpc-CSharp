namespace BioID.RestGrpcForwarder
{
    using BioID.RestGrpcForwarder.Auth;
    using BioID.Services;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using Serilog;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;

    public class Program
    {
        public static void Main(string[] args)
        {
            // App Configuration
            IConfiguration appConfiguration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(appConfiguration)
                .Enrich.FromLogContext().Enrich.WithMachineName()
                .CreateLogger();
            try
            {
                Log.Information("Application is starting up.");
                // create webapp
                var builder = WebApplication.CreateBuilder(args);

                //Add support to logging with SERILOG
                builder.Services.AddSerilog();

                // Add api key header name for authentication.
                builder.Services.AddAuthentication("ApiKey")
                    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthSchemeHandler>("ApiKey", null);

                // Add settings for accessing API authentication.
                builder.Services.Configure<ApiAuthConfiguration>(appConfiguration.GetSection("ApiAuth"));

                // Add grpc client to service collection.
                builder.Services.AddGrpcClient<BioIDWebService.BioIDWebServiceClient>(o =>
                {
                    // Configure grpc server endpoint from appsettings.json
                    o.Address = new Uri(appConfiguration["BwsGrpcApiSettings:Endpoint"]!);
                })
                    .AddCallCredentials((context, metadata, serviceProvider) =>
                    {
                        // Generate JWT token 
                        var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(appConfiguration["BwsGrpcApiSettings:AccessKey"]!));
                        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);
                        List<Claim> claims = [new Claim(JwtRegisteredClaimNames.Sub, appConfiguration["BwsGrpcApiSettings:ClientId"]!)];
                        var now = DateTime.UtcNow;
                        string token = new JwtSecurityTokenHandler().CreateEncodedJwt("RestGrpcForwarder", "bws", new ClaimsIdentity(claims), now, now.AddMinutes(10), now, credentials);
                        metadata.Add("Authorization", $"Bearer {token}");
                        return Task.CompletedTask;
                    });

                // Add service for controller to service collection.
                builder.Services.AddControllers();

                // Add Authorization to service collection.
                builder.Services.AddAuthorization();

                var app = builder.Build();

                // Configure the HTTP request pipeline.

                app.UseSerilogRequestLogging();
                app.UseHttpsRedirection();

                app.UseAuthorization();
                app.UseAuthorization();

                // use controller for rest request
                app.MapControllers();

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "The application failed to start.");
                Log.CloseAndFlush();
            }
           
        }
    }
}
