
using ASPHW3_Services__DI_.Data;
using ASPHW3_Services__DI_.Formatters;
using ASPHW3_Services__DI_.Repository.Abstract;
using ASPHW3_Services__DI_.Repository.Concrete;
using ASPHW3_Services__DI_.Services.Abstract;
using ASPHW3_Services__DI_.Services.Concrete;
using Microsoft.EntityFrameworkCore;

namespace ASPHW3_Services__DI_
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddControllers(options =>
            {
                options.OutputFormatters.Add(new BookVCardOutputFormatter());
                options.InputFormatters.Insert(0, new BookVCardInputFormatter());
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var connection = builder.Configuration.GetConnectionString("BookConnection");
            builder.Services.AddDbContext<BookContext>(options => options.UseSqlServer(connection));

            builder.Services.AddScoped<IBookService, BookService>();
            builder.Services.AddScoped<IBookRepository, BookRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
