using JwtWabApiTutorial;
using JwtWabApiTutorial.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

var myAllowSpecificOriginns = "_myAllowSpesificOrigin";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});
builder.Services.AddDbContext<DataContext>(option =>
{
    option.UseInMemoryDatabase(builder.Configuration.GetConnectionString("DefaultConnection"));

});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.HttpContext.Request.Cookies["token"];
                context.Token = token;
                return Task.CompletedTask;

            },

        };
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
//enable CORS
builder.Services.AddCors(option =>
{
    option.AddPolicy(name: myAllowSpecificOriginns, builder =>
    {
        builder.WithOrigins("http://localhost:3000")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});
var app = builder.Build();
var testUsers = new List<User>
    {
        new User { UserId = 1, Username = "admin", Password = "admin", Role = "Admin" },
        new User { UserId = 2, Username = "user", Password = "user", Role = "User" }
    };
var graphData = new List<GraphValue>
    {
        new GraphValue { GraphValueId = 1, Value = 25.5},
        new GraphValue { GraphValueId = 2, Value = 15.9},
        new GraphValue { GraphValueId = 3, Value = 12.0},
        new GraphValue { GraphValueId = 4, Value = 35.0},
        new GraphValue { GraphValueId = 5, Value = 22.3}
    };
using var scope = app.Services.CreateScope();
var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
dataContext.Users.AddRange(testUsers);
dataContext.SaveChanges();

dataContext.graphValues.AddRange(graphData);
dataContext.SaveChanges();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(myAllowSpecificOriginns);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
