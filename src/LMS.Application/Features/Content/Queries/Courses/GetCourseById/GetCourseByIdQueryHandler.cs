using AutoMapper;
using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Application.Features.Content.DTOs;
using LMS.Domain.Common;
using LMS.Domain.Content;
using MediatR;

namespace LMS.Application.Features.Content.Queries.Courses.GetCourseById;

public sealed class GetCourseByIdQueryHandler : IRequestHandler<GetCourseByIdQuery, ApiResult<CourseDto>>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IModuleRepository _moduleRepository;
    private readonly IUserRepository _userRepository;
    private readonly ISubjectRepository _subjectRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetCourseByIdQueryHandler(
        ICourseRepository courseRepository,
        IModuleRepository moduleRepository,
        IUserRepository userRepository,
        ISubjectRepository subjectRepository,
        IProductRepository productRepository,
        IMapper mapper)
    {
        _courseRepository = courseRepository;
        _moduleRepository = moduleRepository;
        _userRepository = userRepository;
        _subjectRepository = subjectRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<ApiResult<CourseDto>> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
    {
        // Get the course
        var course = await _courseRepository.GetByIdAsync(CourseId.From(request.CourseId), cancellationToken);
        if (course == null)
        {
            return ApiResult<CourseDto>.Fail(
                ErrorCodes.Course.NotFound,
                ErrorMessages.GetMessage(ErrorCodes.Course.NotFound),
                HttpStatusCodes.NotFound);
        }

        // Map to DTO
        var courseDto = _mapper.Map<CourseDto>(course);

        // Get teacher name
        var teacher = await _userRepository.GetByIdAsync(course.TeacherId, cancellationToken);
        courseDto = courseDto with { TeacherName = teacher?.GetFullName() ?? "Unknown" };

        // Get subject name
        var subject = await _subjectRepository.GetByIdAsync(course.SubjectId, cancellationToken);
        courseDto = courseDto with { SubjectName = subject?.Name ?? "Unknown" };

        // Get modules
        var modules = await _moduleRepository.GetModulesByCourseAsync(course.Id, cancellationToken);
        courseDto = courseDto with { ModulesCount = modules.Count };

        // Get modules list
        var moduleDtos = _mapper.Map<List<ModuleListDto>>(modules);
        courseDto = courseDto with { Modules = moduleDtos };

        // Get price if product exists
        if (course.ProductId != null)
        {
            var product = await _productRepository.GetByIdAsync(course.ProductId, cancellationToken);
            courseDto = courseDto with { Price = product?.CashPrice };
        }

        return ApiResult<CourseDto>.Ok(courseDto);
    }
}
