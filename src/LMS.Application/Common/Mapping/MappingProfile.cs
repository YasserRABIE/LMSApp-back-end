using AutoMapper;
using LMS.Application.Features.Content.DTOs;
using LMS.Domain.Content;

namespace LMS.Application.Common.Mapping;

/// <summary>
/// AutoMapper profile for entity to DTO mappings
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Reference data mappings
        CreateMap<Subject, SubjectDto>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id.Value));

        CreateMap<Subject, SubjectListDto>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id.Value));

        CreateMap<Tag, TagDto>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id.Value));

        CreateMap<SchoolType, SchoolTypeDto>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id.Value));

        CreateMap<CourseCategory, CourseCategoryDto>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id.Value));

        // Course mappings
        CreateMap<Course, CourseDto>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id.Value))
            .ForMember(d => d.TeacherId, opt => opt.MapFrom(s => s.TeacherId.Value))
            .ForMember(d => d.SubjectId, opt => opt.MapFrom(s => s.SubjectId.Value))
            .ForMember(d => d.SchoolTypeId, opt => opt.MapFrom(s => s.SchoolTypeId.Value))
            .ForMember(d => d.CourseCategoryId, opt => opt.MapFrom(s => s.CourseCategoryId.Value))
            .ForMember(d => d.ProductId, opt => opt.MapFrom(s => s.ProductId != null ? (Guid?)s.ProductId.Value : null))
            .ForMember(d => d.Visibility, opt => opt.MapFrom(s => s.Visibility.ToString()))
            .ForMember(d => d.ModulesCount, opt => opt.Ignore())
            .ForMember(d => d.Modules, opt => opt.Ignore())
            .ForMember(d => d.TeacherName, opt => opt.Ignore())
            .ForMember(d => d.SubjectName, opt => opt.Ignore())
            .ForMember(d => d.Price, opt => opt.Ignore());

        CreateMap<Course, CourseListDto>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id.Value))
            .ForMember(d => d.Visibility, opt => opt.MapFrom(s => s.Visibility.ToString()))
            .ForMember(d => d.ModulesCount, opt => opt.Ignore())
            .ForMember(d => d.TeacherName, opt => opt.Ignore())
            .ForMember(d => d.SubjectName, opt => opt.Ignore())
            .ForMember(d => d.Price, opt => opt.Ignore());

        // Module mappings
        CreateMap<Module, ModuleDto>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id.Value))
            .ForMember(d => d.CourseId, opt => opt.MapFrom(s => s.CourseId.Value))
            .ForMember(d => d.ProductId, opt => opt.MapFrom(s => s.ProductId != null ? (Guid?)s.ProductId.Value : null))
            .ForMember(d => d.StagesCount, opt => opt.Ignore())
            .ForMember(d => d.Stages, opt => opt.Ignore())
            .ForMember(d => d.Price, opt => opt.Ignore());

        CreateMap<Module, ModuleListDto>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id.Value))
            .ForMember(d => d.StagesCount, opt => opt.Ignore())
            .ForMember(d => d.Price, opt => opt.Ignore());

        // Stage mappings
        CreateMap<Stage, StageDto>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id.Value))
            .ForMember(d => d.ModuleId, opt => opt.MapFrom(s => s.ModuleId.Value))
            .ForMember(d => d.Visibility, opt => opt.MapFrom(s => s.Visibility.ToString()))
            .ForMember(d => d.ProductId, opt => opt.MapFrom(s => s.ProductId != null ? (Guid?)s.ProductId.Value : null))
            .ForMember(d => d.ContentItemsCount, opt => opt.Ignore())
            .ForMember(d => d.ContentItems, opt => opt.Ignore())
            .ForMember(d => d.Price, opt => opt.Ignore());

        CreateMap<Stage, StageListDto>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id.Value))
            .ForMember(d => d.Visibility, opt => opt.MapFrom(s => s.Visibility.ToString()))
            .ForMember(d => d.ContentItemsCount, opt => opt.Ignore())
            .ForMember(d => d.Price, opt => opt.Ignore());

        // ContentItem mappings
        CreateMap<ContentItem, ContentItemDto>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id.Value))
            .ForMember(d => d.StageId, opt => opt.MapFrom(s => s.StageId.Value))
            .ForMember(d => d.ContentType, opt => opt.MapFrom(s => s.ContentType.ToString()))
            .ForMember(d => d.ProductId, opt => opt.MapFrom(s => s.ProductId != null ? (Guid?)s.ProductId.Value : null))
            .ForMember(d => d.Price, opt => opt.Ignore())
            .ForMember(d => d.ExternalVideoId, opt => opt.Ignore())
            .ForMember(d => d.DurationSeconds, opt => opt.Ignore())
            .ForMember(d => d.VideoStatus, opt => opt.Ignore())
            .ForMember(d => d.FileUrl, opt => opt.Ignore())
            .ForMember(d => d.FileSizeBytes, opt => opt.Ignore())
            .ForMember(d => d.FileType, opt => opt.Ignore());

        CreateMap<ContentItem, ContentItemListDto>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id.Value))
            .ForMember(d => d.ContentType, opt => opt.MapFrom(s => s.ContentType.ToString()))
            .ForMember(d => d.Price, opt => opt.Ignore())
            .ForMember(d => d.DurationSeconds, opt => opt.Ignore());

        // Video and File content handled separately in query handlers

        // Prerequisite mappings
        CreateMap<Prerequisite, PrerequisiteDto>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id.Value))
            .ForMember(d => d.LogicOperator, opt => opt.MapFrom(s => s.LogicOperator.ToString()));
    }
}
