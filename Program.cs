using HotelBookingAPI.Connection;
using HotelBookingAPI.Repository;
using Serilog;

namespace HotelBookingAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddScoped<CancellationRepository>();
            builder.Services.AddScoped<HotelSearchRepository>();
            builder.Services.AddScoped<ReservationRepository>();
            builder.Services.AddScoped<RoomAmenityRepository>();
            builder.Services.AddScoped<AmenityRepository>();
            builder.Services.AddScoped<RoomTypeRepository>();
            builder.Services.AddScoped<RoomRepository>();
            builder.Services.AddScoped<UserRepository>();
            // Add services to the container.
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

            // Initialize Serilog from appsettings.json
            builder.Host.UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services));

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddTransient<SqlConnectionFactory>();

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