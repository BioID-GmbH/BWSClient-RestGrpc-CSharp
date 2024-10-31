using BioID.RestGrpcForwarder.Auth;
using BioID.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

// create webapp
var builder = WebApplication.CreateBuilder(args);

// Add api key header name for authentication.
builder.Services.AddAuthentication("ApiKey")
    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthSchemeHandler>("ApiKey", null);

// Add settings for accessing API authentication.
builder.Services.Configure<ApiAuthConfiguration>(builder.Configuration.GetSection("ApiAuth"));

// Add grpc client to service collection.
builder.Services.AddGrpcClient<BioIDWebService.BioIDWebServiceClient>(o =>
{
    // Configure grpc server endpoint from appsettings.json
    var gprcEndpoint = builder.Configuration.GetSection("BwsGrpcApiSettings")["Endpoint"] ?? throw new InvalidOperationException("The gRPC endpoint is not specified or is incorrect.");
    o.Address = new Uri(gprcEndpoint);
})
    .AddCallCredentials((context, metadata, serviceProvider) =>
    {
        // Generate JWT token 
        var key = builder.Configuration.GetSection("BwsGrpcApiSettings")["AccessKey"] ?? throw new InvalidOperationException("The grpc access key could not be found.");
        var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);
        var clientID = builder.Configuration.GetSection("BwsGrpcApiSettings")["ClientId"] ?? throw new InvalidOperationException("The grpc clientId could not be found.");
        List<Claim> claims = [new Claim(JwtRegisteredClaimNames.Sub, clientID)];
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
app.UseAuthorization();
app.UseAuthorization();

// use controller for rest request
app.MapControllers();

app.Run();
