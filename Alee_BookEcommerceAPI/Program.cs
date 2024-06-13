using Alee_BookEcommerceAPI;
using Alee_BookEcommerceAPI.Data;
using Alee_BookEcommerceAPI.Repository;
using Alee_BookEcommerceAPI.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddAutoMapper(typeof(MappingConfig));

// add versioning to services
builder.Services.AddApiVersioning(options =>
{
    // Thiết lập này giả định rằng phiên bản API mặc định sẽ được sử dụng khi một client không chỉ định phiên bản
    options.AssumeDefaultVersionWhenUnspecified = true;
    // Thiết lập này xác định phiên bản API mặc định là 1.0.
    options.DefaultApiVersion = new ApiVersion(1, 0);
    // show the available API version in response header
    options.ReportApiVersions = true;
});
builder.Services.AddVersionedApiExplorer(options =>
{
    // Thiết lập này định dạng tên nhóm cho các phiên bản API. 'v'VVV sẽ định dạng các phiên bản API dưới dạng "v1", "v2",...
    options.GroupNameFormat = "'v'VVV";
    // Auto choose default version
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "Alee_VillaV1"); });
}

app.UseStaticFiles(); // render wwwroot

app.UseHttpsRedirection();

app.MapControllers();

app.Run();