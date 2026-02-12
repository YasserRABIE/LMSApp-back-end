using AutoMapper;
using LMS.Application.Common;
using LMS.Application.Common.Interfaces;
using LMS.Application.Features.Content.DTOs;
using LMS.Domain.Content;
using LMS.Domain.Users;
using MediatR;

namespace LMS.Application.Features.Content.Queries.Courses.GetCoursesList;

public sealed class GetCoursesListQueryHandler : IRequestHandler<GetCoursesListQuery, ApiResult<List<CourseListDto>>>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IModuleRepository _moduleRepository;
    private readonly IUserRepository _userRepository;
    private readonly ISubjectRepository _subjectRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetCoursesListQueryHandler(
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

    public async Task<ApiResult<List<CourseListDto>>> Handle(GetCoursesListQuery request, CancellationToken cancellationToken)
    {
        // Get courses based on filters
        IReadOnlyList<Course> courses;

        if (request.PublishedOnly)
        {
            courses = await _courseRepository.GetPublishedCoursesAsync(cancellationToken);
        }
        else if (request.TeacherId.HasValue)
        {
            courses = await _courseRepository.GetCoursesByTeacherAsync(
                UserId.From(request.TeacherId.Value),
                cancellationToken);
        }
        else if (request.SubjectId.HasValue)
        {
            courses = await _courseRepository.GetCoursesBySubjectAsync(
                SubjectId.From(request.SubjectId.Value),
                cancellationToken);
        }
        else
        {
            courses = await _courseRepository.GetAllAsync(cancellationToken);
        }

        // Map to DTOs
        var courseDtos = _mapper.Map<List<CourseListDto>>(courses);

        // Enrich with additional data
        foreach (var (courseDto, course) in courseDtos.Zip(courses))
        {
            // Get teacher name
            var teacher = await _userRepository.GetByIdAsync(course.TeacherId, cancellationToken);

            // Get subject name
            var subject = await _subjectRepository.GetByIdAsync(course.SubjectId, cancellationToken);

            // Get modules count
            var modules = await _moduleRepository.GetModulesByCourseAsync(course.Id, cancellationToken);

            // Get price
            decimal? price = null;
            if (course.ProductId != null)
            {
                var product = await _productRepository.GetByIdAsync(course.ProductId, cancellationToken);
                price = product?.CashPrice;
            }

            var index = courseDtos.IndexOf(courseDto);
            courseDtos[index] = courseDto with
            {
                TeacherName = teacher?.GetFullName() ?? "Unknown",
                SubjectName = subject?.Name ?? "Unknown",
                ModulesCount = modules.Count,
                Price = price
            };
        }

        return ApiResult<List<CourseListDto>>.Ok(courseDtos);
    }
}
