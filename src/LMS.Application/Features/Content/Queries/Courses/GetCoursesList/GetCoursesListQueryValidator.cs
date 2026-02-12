using FluentValidation;

namespace LMS.Application.Features.Content.Queries.Courses.GetCoursesList;

/// <summary>
/// Validator for GetCoursesListQuery
/// </summary>
public sealed class GetCoursesListQueryValidator : AbstractValidator<GetCoursesListQuery>
{
    public GetCoursesListQueryValidator()
    {
        // No required validations - all parameters are optional filters
    }
}
