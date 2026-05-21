using System;
using System.Reflection;
using System.Text.Json.Serialization;
using CsvHelper;
using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using mbt.webapi;
using mbt.webapi.Configuration;
using mbt.webapi.Configuration.Validation;
using mbt.webapi.Middleware;
using mbt.webapi.Migrations;
using mbt.webapi.Services;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDBMigrations;
using NLog;
using NLog.Web;
using Prometheus;
using Prometheus.HttpMetrics;
using WebApplication = Microsoft.AspNetCore.Builder.WebApplication;


// Early init of NLog to allow startup and exception logging, before host is built
var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Configuration.AddJsonFile("opt/config.json");


    // NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    builder.Services.AddMediatrConfiguration();

    builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

    var objectSerializer = new ObjectSerializer(_ => true);
    BsonSerializer.RegisterSerializer(objectSerializer);

    var dbConfigSection = builder.Configuration.GetRequiredSection(nameof(MbtDatabaseSettings));
    builder.Services.Configure<MbtDatabaseSettings>(dbConfigSection);
    var dbConfig = dbConfigSection.Get<MbtDatabaseSettings>();

    builder.Services.AddWorkflowCore(builder.Configuration);

    builder.Services.AddProblemDetails(x =>
        {
            x.IncludeExceptionDetails =
                (_, _) => false; //builder.Environment.IsDevelopment() || builder.Environment.IsStaging();

            x.Map<InvalidOperationException>(ex => new ApiValidationErrorProblemDetails(ex));
            x.Map<FluentValidation.ValidationException>(ex => new ApiValidationErrorProblemDetails(ex));
            x.Map<AccessDeniedException>(ex => new AccessDeniedProblemDetails(ex));
            x.Map<ApiException>(ex => new ApiValidationErrorProblemDetails(ex));
            x.Map<DuplicateItemException>(ex => new ApiValidationErrorProblemDetails(ex));
            x.Map<CsvHelperException>(ex => new InvalidFileProblemDetails(ex));
            x.Map<Exception>(ex => new ApiValidationErrorProblemDetails(ex));
        }).AddMvcCore()
        .AddJsonOptions(x => x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);


    builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), ServiceLifetime.Singleton);

    builder.Services.AddSingleton<ICurrentUserContext, CurrentUserContext>();

    builder.Services.AddHttpContextAccessor();

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApi(builder.Configuration)
        .EnableTokenAcquisitionToCallDownstreamApi()
        .AddMicrosoftGraph(builder.Configuration.GetSection("DownstreamApi"))
        .AddInMemoryTokenCaches();

    builder.Services.AddAuthorization(options =>
    {
        options.FallbackPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
    });

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("default", policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });

    builder.Services.Configure<SmtpSettings>(
        builder.Configuration.GetSection(nameof(SmtpSettings)));

    builder.Services.AddSingleton<IMbtDatabaseSettings>(sp =>
        sp.GetRequiredService<IOptions<MbtDatabaseSettings>>().Value);

    builder.Services.AddSingleton<ISmtpSettings>(sp =>
        sp.GetRequiredService<IOptions<SmtpSettings>>().Value);

    builder.Services.AddRepositories();
    builder.Services.AddServices();

    builder.Services.AddHostedService<RemoveOldItemsTimedService>();

    builder.Services.AddHealthChecks();

    builder.Services.AddControllers()
        .AddJsonOptions(
            options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

    var azureConfigOptions = new AzureConfigOptions();
    builder.Configuration.GetSection(AzureConfigOptions.SectionName).Bind(azureConfigOptions);
    builder.Services.Configure<AzureConfigOptions>(builder.Configuration.GetSection(AzureConfigOptions.SectionName));


    builder.Services.AddSwaggerGenService(azureConfigOptions);

    builder.AddQuartsConfiguration();

    var app = builder.Build();

    app.UseSwaggerUiConfig(azureConfigOptions.ClientId, azureConfigOptions.ClientSecret);

    app.UseRouting();
    app.UseHttpMetrics(new HttpMiddlewareExporterOptions()
    {
        RequestDuration = new HttpRequestDurationOptions()
        {
            Enabled = false
        }
    });

    app.Services.UseWorkflowCore();

    app.UseCors("default");

    app.UseProblemDetails();

    app.UseAuthentication();

    var isDevMode = builder.Configuration.GetValue("IsDevMode", false);
    if (isDevMode)
    {
        app.UseMiddleware<ExtendClaimsMiddleware>();
    }

    app.UseAuthorization();

    app.UseMiddleware<EnsureCurrentUserMiddleware>();

    app.MapControllers();
    app.MapMetrics().AllowAnonymous();
    app.MapHealthChecks("/health").AllowAnonymous();

    DbSeeder.SeedDatabase(app);
    DbSeeder.CreateIndexAsync(dbConfig.ConnectionString, dbConfig.DatabaseName);

    var notificationService = app.Services.GetRequiredService<INotificationService>();
    notificationService.Init();

    //mongo-db-migrations
    new MigrationEngine()
        .UseDatabase(dbConfig.ConnectionString, dbConfig.DatabaseName) //Required to use specific db
        .UseAssembly(Assembly.GetExecutingAssembly())
        .UseSchemeValidation(false).Run();
    //

    app.Run();
}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    LogManager.Shutdown();
}
