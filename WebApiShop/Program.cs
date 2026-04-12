using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Repositeries;
using Service;
using WebApiShop.Controllers;
using NLog.Web;
using Entities;
using System.Text.Json.Serialization;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddCors(options => {

//options.AddPolicy("CorsPolicy", 
//builder =>
//builder.WithOrigins("http://localhost:4200", "אתר פיתוח")
//.AllowAnyHeader()
//.AllowAnyMethod()
//    );


//});
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
    );
});


// Add services to the container.

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserPasswordRipository, UserPasswordRipository>();
builder.Services.AddScoped<IUserPasswordService, UserPasswordService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddAutoMapper(typeof(Service.AutoMappering));



builder.Services.AddDbContext<Store_215962135Context>(options => options.UseSqlServer( builder.Configuration.GetConnectionString("MyWebApiShop")));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // הופך את כל השמות (כמו Items, TotalCount) לאותיות קטנות בהתחלה (items, totalCount)
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

        // מתעלם מלולאות מעגליות במקום לקרוס ולשלוח תשובה ריקה
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });


builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Host.UseNLog();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "My API V1");
    });
}


// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("CorsPolicy");

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
