using FluentAssertions;
using LMS.Domain.Common;
using LMS.Domain.Users;

namespace LMS.Domain.Tests.Users;

public sealed class StudentProfileTests
{
    private readonly UserId _userId;
    private readonly Guid _studyLevelTrackId;

    public StudentProfileTests()
    {
        _userId = UserId.New();
        _studyLevelTrackId = Guid.NewGuid();
    }

    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        // Act
        var result = StudentProfile.Create(
            _userId,
            _studyLevelTrackId,
            schoolName: "Cairo Secondary School",
            governorate: "Cairo");

        // Assert
        result.IsSuccess.Should().BeTrue();
        var profile = result.Value;
        profile.UserId.Should().Be(_userId);
        profile.StudyLevelTrackId.Should().Be(_studyLevelTrackId);
        profile.SchoolName.Should().Be("Cairo Secondary School");
        profile.Governorate.Should().Be("Cairo");
        profile.CreatedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        profile.UpdatedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_WithoutOptionalFields_ShouldSucceed()
    {
        // Act
        var result = StudentProfile.Create(_userId, _studyLevelTrackId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var profile = result.Value;
        profile.SchoolName.Should().BeNull();
        profile.Governorate.Should().BeNull();
    }

    [Fact]
    public void Create_WithEmptyStudyLevelTrackId_ShouldFail()
    {
        // Act
        var result = StudentProfile.Create(_userId, Guid.Empty);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(ErrorCodes.Validation.Required);
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public void Create_WithTooLongSchoolName_ShouldFail()
    {
        // Arrange
        var longSchoolName = new string('A', 201);

        // Act
        var result = StudentProfile.Create(
            _userId,
            _studyLevelTrackId,
            schoolName: longSchoolName);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(ErrorCodes.Validation.InvalidInput);
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public void Create_WithTooLongGovernorate_ShouldFail()
    {
        // Arrange
        var longGovernorate = new string('A', 101);

        // Act
        var result = StudentProfile.Create(
            _userId,
            _studyLevelTrackId,
            governorate: longGovernorate);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(ErrorCodes.Validation.InvalidInput);
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public void UpdateProfile_WithValidSchoolName_ShouldSucceed()
    {
        // Arrange
        var profile = StudentProfile.Create(_userId, _studyLevelTrackId).Value;
        const string newSchoolName = "Alexandria Secondary School";

        // Act
        var result = profile.UpdateProfile(schoolName: newSchoolName);

        // Assert
        result.IsSuccess.Should().BeTrue();
        profile.SchoolName.Should().Be(newSchoolName);
        profile.UpdatedAtUtc.Should().BeOnOrAfter(profile.CreatedAtUtc);
    }

    [Fact]
    public void UpdateProfile_WithValidGovernorate_ShouldSucceed()
    {
        // Arrange
        var profile = StudentProfile.Create(_userId, _studyLevelTrackId).Value;
        const string newGovernorate = "Alexandria";

        // Act
        var result = profile.UpdateProfile(governorate: newGovernorate);

        // Assert
        result.IsSuccess.Should().BeTrue();
        profile.Governorate.Should().Be(newGovernorate);
        profile.UpdatedAtUtc.Should().BeOnOrAfter(profile.CreatedAtUtc);
    }

    [Fact]
    public void UpdateProfile_WithNewStudyLevelTrack_ShouldSucceed()
    {
        // Arrange
        var profile = StudentProfile.Create(_userId, _studyLevelTrackId).Value;
        var newStudyLevelTrackId = Guid.NewGuid();

        // Act
        var result = profile.UpdateProfile(studyLevelTrackId: newStudyLevelTrackId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        profile.StudyLevelTrackId.Should().Be(newStudyLevelTrackId);
        profile.UpdatedAtUtc.Should().BeOnOrAfter(profile.CreatedAtUtc);
    }

    [Fact]
    public void UpdateProfile_WithTooLongSchoolName_ShouldFail()
    {
        // Arrange
        var profile = StudentProfile.Create(_userId, _studyLevelTrackId).Value;
        var longSchoolName = new string('A', 201);

        // Act
        var result = profile.UpdateProfile(schoolName: longSchoolName);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(ErrorCodes.Validation.InvalidInput);
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public void UpdateProfile_WithTooLongGovernorate_ShouldFail()
    {
        // Arrange
        var profile = StudentProfile.Create(_userId, _studyLevelTrackId).Value;
        var longGovernorate = new string('A', 101);

        // Act
        var result = profile.UpdateProfile(governorate: longGovernorate);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(ErrorCodes.Validation.InvalidInput);
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public void UpdateProfile_WithEmptyStudyLevelTrackId_ShouldNotUpdate()
    {
        // Arrange
        var profile = StudentProfile.Create(_userId, _studyLevelTrackId).Value;
        var originalTrackId = profile.StudyLevelTrackId;

        // Act
        var result = profile.UpdateProfile(studyLevelTrackId: Guid.Empty);

        // Assert
        result.IsSuccess.Should().BeTrue();
        profile.StudyLevelTrackId.Should().Be(originalTrackId); // Should remain unchanged
    }

    [Fact]
    public void UpdateProfile_WithAllFields_ShouldSucceed()
    {
        // Arrange
        var profile = StudentProfile.Create(_userId, _studyLevelTrackId).Value;
        var newTrackId = Guid.NewGuid();
        const string newSchoolName = "New School";
        const string newGovernorate = "Giza";

        // Act
        var result = profile.UpdateProfile(
            studyLevelTrackId: newTrackId,
            schoolName: newSchoolName,
            governorate: newGovernorate);

        // Assert
        result.IsSuccess.Should().BeTrue();
        profile.StudyLevelTrackId.Should().Be(newTrackId);
        profile.SchoolName.Should().Be(newSchoolName);
        profile.Governorate.Should().Be(newGovernorate);
        profile.UpdatedAtUtc.Should().BeOnOrAfter(profile.CreatedAtUtc);
    }
}
