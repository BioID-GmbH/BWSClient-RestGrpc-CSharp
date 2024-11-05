using BioID.RestGrpcForwarder.Auth;
using BioID.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

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
    var gprcEndpoint = builder.Configuration["BwsGrpcApiSettings:Endpoint"] ?? throw new InvalidOperationException("The gRPC endpoint is not specified or is incorrect.");
    o.Address = new Uri(gprcEndpoint);
})
    .AddCallCredentials((context, metadata, serviceProvider) =>
    {
        // Generate Json Web Token 
        var key = builder.Configuration["BwsGrpcApiSettings:AccessKey"] ?? throw new InvalidOperationException("The grpc access key could not be found.");
        var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var clientId = builder.Configuration["BwsGrpcApiSettings:ClientId"] ?? throw new InvalidOperationException("The grpc clientId could not be found.");
        var claims = new Dictionary<string, object> { [JwtRegisteredClaimNames.Sub] = clientId, }; 
        var descriptor = new SecurityTokenDescriptor { Claims = claims, Issuer = clientId, Audience = "BWS", SigningCredentials = credentials };
        var handler = new JsonWebTokenHandler { SetDefaultTimesOnTokenCreation = true, TokenLifetimeInMinutes = 5 };
        string jwt = handler.CreateToken(descriptor);
        metadata.Add("Authorization", $"Bearer {jwt}");
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
