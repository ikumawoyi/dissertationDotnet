using EmailService;
using FluentValidation;
using FluentValidation.Results;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TasksApi;
using TasksApi.Entities;
using TasksApi.Helpers;
using TasksApi.Interfaces;
using TasksApi.Services;
using Vonage;
using Vonage.Messaging;

var builder = WebApplication.CreateBuilder(args);

const string AllowAllHeadersPolicy = "AllowAllHeadersPolicy";

builder.Services.AddCors(options =>
{
    options.AddPolicy(AllowAllHeadersPolicy,
        builder =>
        {
            builder.WithOrigins("http://localhost:4200")
                    .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Add services to the container.


builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddTransient<IEventService, EventService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TasksDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("TasksDbConnectionString")));

builder.Services.AddValidatorsFromAssemblyContaining<SmsModel>(ServiceLifetime.Scoped);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = TokenHelper.Issuer,
                ValidAudience = TokenHelper.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(TokenHelper.Secret)),
                ClockSkew = TimeSpan.Zero
            };

        });

builder.Services.AddAuthorization();

builder.Services.AddHangfireServer();



builder.Services.AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(builder.Configuration.GetConnectionString("TasksDbConnectionString"), new SqlServerStorageOptions
        {
            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            QueuePollInterval = TimeSpan.Zero,
            UseRecommendedIsolationLevel = true,
            DisableGlobalLocks = true
        }));



builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<ITaskService, TaskService>();
builder.Services.AddTransient<IMemberService, MemberService>();
builder.Services.AddTransient<IOccasionService, OccasionService>();


var emailConfig = builder.Configuration
        .GetSection("EmailConfiguration")
        .Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);

builder.Services.AddSingleton<VonageClient>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var key = config.GetValue<string>("Vonage_key");
    var secret = config.GetValue<string>("Vonage_Secret");
    var credentials = Vonage.Request.Credentials.FromApiKeyAndSecret(key, secret);

    return new VonageClient(credentials);
});





var app = builder.Build();
app.UseCors(AllowAllHeadersPolicy);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHangfireDashboard();
//BackgroundJob.Enqueue(() => Console.WriteLine("Hello world from Hangfire!"));
//BackgroundJob.Schedule(() => Console.WriteLine("Hello, world from Schedule"), TimeSpan.FromSeconds(10));

//RecurringJob.AddOrUpdate("MyJobId", () => Console.Write("Powerful!"), "*/2 * * * *");


//app.MapPost("/sms", async (VonageClient vonageClient, SmsModel smsModel) =>
//{
//    var smsResponse = await vonageClient.SmsClient.SendAnSmsAsync(new SendSmsRequest
//    {
//        To = smsModel.To,
//        From = smsModel.From,
//        Text = smsModel.Text
//    });
//});



app.MapPost("/sms", async (VonageClient vonageClient, SmsModel smsModel, IValidator<SmsModel> validator) =>
{
    ValidationResult validationResult = validator.Validate(smsModel);
    if (!validationResult.IsValid)
    {
        return Results.ValidationProblem(validationResult.ToDictionary());
    }

    var smsResponse = await vonageClient.SmsClient.SendAnSmsAsync(new SendSmsRequest
    {

        To = smsModel.To,
        From = smsModel.From,
        Text = smsModel.Text
    }); ;

    return Results.Ok();
});


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.UseDefaultFiles();

app.MapControllers();

app.Run();
