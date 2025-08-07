using System.Text.Json.Serialization;
using AutoInsightAPI.models;
using AutoInsightAPI.Profiles;
using AutoInsightAPI.Repositories;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;

namespace AutoInsightAPI.configs;

public static class ServicesConfigurator
{
    public static void Configure(IServiceCollection services)
    {
        services.AddAutoMapper(typeof(YardProfile));
        services.AddAutoMapper(typeof(YardEmployeeProfile));
        services.AddAutoMapper(typeof(YardVehicleProfile));
        services.AddAutoMapper(typeof(AddressProfile));
        services.AddAutoMapper(typeof(PagedResponseProfile));
        services.AddAutoMapper(typeof(QRCodeProfile));
        services.AddAutoMapper(typeof(VehicleProfile));

        services.AddScoped<IYardRepository, YardRepository>();
        services.AddScoped<IYardEmployeeRepository, YardEmployeeRepository>();
        services.AddScoped<IYardVehicleRepository, YardVehicleRepository>();
        services.AddScoped<IVehicleRepository, VehicleRepository>();

        services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        var oracleConnectionString = Environment.GetEnvironmentVariable("ORACLE_CONNECTION_STRING");
        services.AddDbContext<AutoInsightDb>(opt
            => opt.UseOracle(oracleConnectionString));

        services.AddOpenApi();
    }
}