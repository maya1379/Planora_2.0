using AutoMapper;
using Planora.Services.DTOs;
using Planora.Domain.Entities;

namespace Planora.Services.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {

        CreateMap<Groups, GroupDto>();
        CreateMap<CreateGroupDto, Groups>();
        CreateMap<UpdateGroupDto, Groups>();

        CreateMap<Building, BuildingDto>()
            .ForMember(dest => dest.ClassroomCount, opt => opt.MapFrom(src => src.Classrooms.Count));
        CreateMap<CreateBuildingDto, Building>();
        CreateMap<UpdateBuildingDto, Building>();

        CreateMap<Classrooms, ClassroomDto>()
            .ForMember(dest => dest.BuildingName, opt => opt.MapFrom(src => src.Building.Name));
        CreateMap<CreateClassroomDto, Classrooms>();
        CreateMap<UpdateClassroomDto, Classrooms>();

        CreateMap<Subjects, SubjectDto>();
        CreateMap<CreateSubjectDto, Subjects>();
        CreateMap<UpdateSubjectDto, Subjects>();

        CreateMap<TeachingAssignment, TeachingAssignmentDto>()
            .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher.FullName))
            .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subjects.Name));
        CreateMap<CreateTeachingAssignmentDto, TeachingAssignment>();
        CreateMap<UpdateTeachingAssignmentDto, TeachingAssignment>();

        CreateMap<GroupDisciplineList, GroupSubjectDto>()
            .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Groups.Name))
            .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subjects.Name));
        CreateMap<CreateGroupSubjectDto, GroupDisciplineList>();
        CreateMap<UpdateGroupSubjectDto, GroupDisciplineList>();

        CreateMap<TimeSlot, TimeSlotDto>();
        CreateMap<CreateTimeSlotDto, TimeSlot>();

        CreateMap<Schedule, ScheduleEntryDto>()
            .ForMember(dest => dest.TimeSlotNumber, opt => opt.MapFrom(src => src.TimeSlot.Number))
            .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.TimeSlot.StartTime))
            .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.TimeSlot.EndTime))
            .ForMember(dest => dest.ClassroomNumber, opt => opt.MapFrom(src => src.Classrooms.Number))
            .ForMember(dest => dest.BuildingName, opt => opt.MapFrom(src => src.Classrooms.Building.Name))
            .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher.FullName))
            .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subjects.Name))
            .ForMember(dest => dest.LessonType, opt => opt.MapFrom(src => src.Subjects.Type))
            .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Groups.Name));
        CreateMap<CreateScheduleEntryDto, Schedule>();
    }
}
