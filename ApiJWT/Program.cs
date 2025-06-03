using ApiJWT.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Carrega a chave pelo appsettings
var apiKey = builder.Configuration["Jwt:ApiKey"];
if (string.IsNullOrEmpty(apiKey))
{
    throw new ArgumentNullException("Jwt:ApiKey", "ApiKey não pode ser nulo ou vazio.");
}

var key = Encoding.ASCII.GetBytes(apiKey);

// Add services to the container.
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Se o token não vier no Header, pegar do Cookie
                var token = context.Request.Cookies["jwt"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }

                return Task.CompletedTask;
            }
        };

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(apiKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero // Evita atrasos na expiração do token
        };
    });



    builder.Services.AddAuthorization();

    // Add services to the container.
    //builder.Services.AddScoped<SessaoService>();
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();


    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: "CorsPoliticyAllHosts",
            builder =>
            {
                builder.AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowAnyOrigin();
            });
    });



builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "JWTApi",
        Version = "v1",
        Description = "JWTApi é uma API de estudo que implementa uma autenticação JWT",
        Contact = new OpenApiContact
        {
            Name = "JWTApi - Guilherme Santana",
            Url = new System.Uri("https://github.com/Guilhermesanttana")
        }
    });

    // Configurar autenticação no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Insira o token JWT no formato: Bearer {seu_token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });

    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

app.UseCors("CorsPoliticyAllHosts");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("swagger/v1/swagger.json", "ApiJWT - Guilherme Santana");
    c.RoutePrefix = string.Empty;
});

//app.UseMiddleware<JwtMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

Common.ConnDB = builder.Configuration.GetConnectionString("ConnDB") ?? string.Empty;
Common.PathLog = builder.Configuration.GetSection("Params")["PathLog"]?.ToString() ?? string.Empty;
Common.ApiKey = builder.Configuration.GetSection("Jwt")["ApiKey"]?.ToString() ?? string.Empty;

app.Run();
