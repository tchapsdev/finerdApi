using Finerd.Api.Data;
using Finerd.Api.Hubs;
using Finerd.Api.Model;
using Finerd.Api.Model.Entities;
using Finerd.Api.PushNotification;
using Finerd.Api.Services;
using Finerd.Api.Services.Push;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

    var builder = WebApplication.CreateBuilder(args);

    // Build a config object, using env vars and JSON providers.
    IConfiguration config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddEnvironmentVariables()
        .Build();

    // Get values from the config given their key and their target type.
    var jwtSettings = config.GetRequiredSection("JwtSettings").Get<JwtSettings>();

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(jwtSettings.AccessTokenSecret))                    
                };

                // Configure the Authority to the expected value for
                // the authentication provider. This ensures the token
                // is appropriately validated.
               // options.Authority = "Authority URL"; // TODO: Update URL

                // We have to hook the OnMessageReceived event in order to
                // allow the JWT authentication handler to read the access
                // token from the query string when a WebSocket or 
                // Server-Sent Events request comes in.

                // Sending the access token in the query string is required due to
                // a limitation in Browser APIs. We restrict it to only calls to the
                // SignalR hub in this code.
                // See https://docs.microsoft.com/aspnet/core/signalr/security#access-token-logging
                // for more information about security considerations when using
                // the query string to transmit the access token.
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/finerdHub")))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
    builder.Services.AddAuthorization();

    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Finerd API", Version = "v1" });
        //c.EnableAnnotations();

        c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            BearerFormat = "JWT"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = JwtBearerDefaults.AuthenticationScheme
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    builder.Services.AddScoped<ITokenService, TokenService>();
    builder.Services.AddDbContext<ApplicationDbContext>(
        options => {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultDbConnectionString"));
        }
    );

    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<ITransactionService, TransactionService>();
    builder.Services.AddScoped<IPushNotificationService, PushServicePushNotificationService>();

    builder.Services.AddTransient<IPushSubscriptionStore, PushSubscriptionStore>();
    builder.Services.AddTransient<IPushSubscriptionStoreAccessor, PushSubscriptionStoreAccessor>();
    builder.Services.AddTransient<IPushSubscriptionStoreAccessorProvider, PushSubscriptionStoreAccessorProvider>();

    builder.Services.AddScoped<IGenericService<Category>, GenericService<Category>>();
    builder.Services.AddScoped<IGenericService<TransactionType>, GenericService<TransactionType>>();
    builder.Services.AddScoped<IGenericService<PaymentMethod>, GenericService<PaymentMethod>>();
    //builder.Services.AddScoped<IGenericService<PushSubscription>, GenericService<PushSubscription>>();

    builder.Services.Configure<PushNotificationServiceOptions>(builder.Configuration.GetSection("VAPID"));

    //builder.Services.AddPushServiceClient(options =>
    //{
    //    IConfigurationSection pushNotificationServiceConfigurationSection = builder.Configuration.GetSection("VAPID");

    //    options.Subject = pushNotificationServiceConfigurationSection.GetValue<string>(nameof(options.Subject));
    //    options.PublicKey = pushNotificationServiceConfigurationSection.GetValue<string>(nameof(options.PublicKey));
    //    options.PrivateKey = pushNotificationServiceConfigurationSection.GetValue<string>(nameof(options.PrivateKey));
    //});

// CORS with default policy and middleware
builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(
            policy =>
            {
                //policy.WithOrigins("http://localhost:3000",
                //                    "https://finerd-tchapsdev.vercel.app",
                //                    "http://finerd-api.tchapssolution.com",
                //                    "https://finerd-api.tchapssolution.com")
                //        .AllowAnyHeader()
                //        .AllowAnyMethod();
                policy.AllowAnyHeader()
                      .AllowAnyMethod()
                      .SetIsOriginAllowed(origin => true) // allow any origin
                      .AllowAnyOrigin();

            });
    });

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    //builder.Services.AddSwaggerGen();

    builder.Services.AddAutoMapper(typeof(Program));


    builder.Services.AddSignalR(hubOptions =>
    {
        // If true, detailed exception messages are returned to clients when an exception is thrown in a Hub method.
        hubOptions.EnableDetailedErrors = true;
        // If the server hasn't sent a message within this interval, a ping message is sent automatically to keep the connection open. 
        hubOptions.KeepAliveInterval = TimeSpan.FromMinutes(3);
        // The server considers the client disconnected if it hasn't received a message (including keep-alive) in this interval.
        // It could take longer than this timeout interval for the client to be marked disconnected due to how this is implemented.
        // The recommended value is double the KeepAliveInterval value.
        hubOptions.ClientTimeoutInterval = TimeSpan.FromMinutes(6);
    });

    //builder.Services.AddHostedService<FinerdHubService>();
    builder.Services.AddHostedService<FinerdNotificationHubService>();


    var app = builder.Build();


    app.MapHub<NotificationHub>("/finerdHub");

    // Configure the HTTP request pipeline.
    //if (app.Environment.IsDevelopment())
    //{
    //    app.UseSwagger();
    //    app.UseSwaggerUI();
    //}
    // Todo: Hide the Swagger after API fully tested. 

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

app.UseSwagger();
    app.UseSwaggerUI();

    app.UseHttpsRedirection();
//app.UseStaticFiles();

app.UseCors();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    // migrate any database changes on startup (includes initial db creation)
    using (var scope = app.Services.CreateScope())
    {
        var dataContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dataContext.Database.Migrate();
    }


app.Run();
