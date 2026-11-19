using Cinema.Business;
using FluentValidation;
using WebApplication1.Contracts;
using WebApplication1.Validations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddAutoMapper(cfg => { }, typeof(Program).Assembly);

builder.Services.AddBusiness(builder.Configuration.GetConnectionString("DefaultConnection"));

builder.Services.AddScoped<IValidator<IdentityRegisterRequest>, RegisterRequestValidator>();
builder.Services.AddScoped<IValidator<IdentityLoginRequest>, IdentityLoginRequestValidator>();
builder.Services.AddScoped<IValidator<CreateMovieRequest>, CreateMovieRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateMovieRequest>, UpdateMovieRequestValidator>();
builder.Services.AddScoped<IValidator<CreateMovieCommentRequest>, CreateMovieCommentRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateMovieCommentRequest>, UpdateMovieCommentRequestValidator>();
builder.Services.AddScoped<IValidator<CreateMovieSessionRequest>, CreateMovieSessionRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateMovieSessionRequest>, UpdateMovieSessionRequestValidator>();

builder.Services.AddCors(options =>
    options.AddPolicy("ReactClient", policy =>
        policy.WithOrigins(builder.Configuration.GetSection("ClientOrigins").Get<string[]>() ?? ["http://localhost:5173"])
              .AllowAnyHeader()
              .AllowAnyMethod()));

var app = builder.Build();
app.MapOpenApi();
app.UseSwaggerUI(o => o.SwaggerEndpoint("/openapi/v1.json", "EFDiExample"));
app.UseHttpsRedirection();
app.UseCors("ReactClient");
app.UseAuthorization();
app.MapControllers();

app.Run();