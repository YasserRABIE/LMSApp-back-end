namespace LMS.Domain.Content;

public enum Visibility : byte
{
    Hidden = 1,
    Published = 2,
    Archived = 3
}

public enum ContentType : byte
{
    Video = 1,
    File = 2,
    Assessment = 3,
    Workshop = 4
}

public enum VideoStatus : byte
{
    Uploading = 1,
    Processing = 2,
    Ready = 3,
    Failed = 4
}

public enum ProgressStatus : byte
{
    NotStarted = 0,
    InProgress = 1,
    Completed = 2
}
