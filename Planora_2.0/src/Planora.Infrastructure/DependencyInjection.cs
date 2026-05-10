using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Planora.Services.Interfaces;
using Planora.Services.Services.Interfaces;
using Planora.Domain.Entities;
using Planora.Infrastructure.Data;
using Planora.Infrastructure.Repositories;

namespace Planora.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        services.AddDbContext<PlanoraDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(PlanoraDbContext).Assembly.FullName)));

        services.AddIdentity<User, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 6;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<PlanoraDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<IBuildingRepository, BuildingRepository>();
        services.AddScoped<IClassroomRepository, ClassroomRepository>();
        services.AddScoped<ISubjectRepository, SubjectRepository>();
        services.AddScoped<ITeachingAssignmentRepository, TeachingAssignmentRepository>();
        services.AddScoped<IGroupSubjectRepository, GroupSubjectRepository>();
        services.AddScoped<ITimeSlotRepository, TimeSlotRepository>();
        services.AddScoped<IScheduleEntryRepository, ScheduleEntryRepository>();
        services.AddScoped<IScheduleNoteRepository, ScheduleNoteRepository>();

        services.AddScoped<IEmailService, Planora.Infrastructure.Services.EmailService>();

        return services;
    }
}
