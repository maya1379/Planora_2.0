using Microsoft.Extensions.DependencyInjection;
using Planora.Services.Services;
using Planora.Services.Services.Interfaces;

namespace Planora.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {

        services.AddAutoMapper(typeof(DependencyInjection).Assembly);

        services.AddScoped<IGroupService, GroupService>();
        services.AddScoped<IBuildingService, BuildingService>();
        services.AddScoped<IClassroomService, ClassroomService>();
        services.AddScoped<ISubjectService, SubjectService>();
        services.AddScoped<ITeachingAssignmentService, TeachingAssignmentService>();
        services.AddScoped<IGroupSubjectService, GroupSubjectService>();
        services.AddScoped<ITimeSlotService, TimeSlotService>();
        services.AddScoped<IScheduleService, ScheduleService>();
        services.AddScoped<IScheduleGenerationService, ScheduleGenerationService>();
        services.AddScoped<IExportService, ExportService>();

        return services;
    }
}
