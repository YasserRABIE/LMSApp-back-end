# LMS (Learning Management System) - ERD Documentation v2.0

**Version:** 2.0 (Improved)  
**Last Updated:** 2026-02-03  
**Status:** Production-Ready  
**Changes:** Performance optimizations, referential integrity fixes, validation constraints, indexing strategy

---

## Table of Contents

1. [Overview](#1-overview)
2. [Architecture](#2-architecture)
3. [Design Decisions](#3-design-decisions)
4. [Core Domain: Users & Auth](#4-core-domain-users--auth)
5. [Content Domain](#5-content-domain)
6. [Assessment Domain](#6-assessment-domain)
7. [Study Plan Domain](#7-study-plan-domain)
8. [Follow-Up Domain](#8-follow-up-domain)
9. [Gamification Domain](#9-gamification-domain)
10. [Purchasing Domain](#10-purchasing-domain)
11. [Notifications Domain](#11-notifications-domain)
12. [Support & Analytics Domain](#12-support--analytics-domain)
13. [Configuration](#13-configuration)
14. [Redis Strategy](#14-redis-strategy)
15. [Data Retention](#15-data-retention)
16. [Enums Reference](#16-enums-reference)
17. [ERD Diagrams](#17-erd-diagrams)

---

## 1. Overview

### 1.1 Description

Interactive LMS for Egyptian secondary education. Gamified learning with human follow-up support.

### 1.2 Scale Parameters

| Metric | Value |
|--------|-------|
| Monthly Active Users | 60,000 |
| Daily Active Users | 12,000 - 18,000 |
| Peak Concurrent | 2,500 - 4,500 |
| Total Bandwidth/Month | ~50 TB |

### 1.3 User Types

| Type | Code | Description |
|------|------|-------------|
| Student | 1 | Primary learners |
| Teacher | 2 | Content creators |
| Assistant | 3 | Follow-up support |
| Parent | 4 | Progress monitors |
| Admin | 5 | Platform admins |

---

## 2. Architecture

### 2.1 Technology Stack

| Layer | Technology |
|-------|------------|
| Backend | .NET 8 (Clean Architecture) |
| Database | PostgreSQL 16 (AWS RDS) |
| Cache | Redis 7 (AWS ElastiCache) |
| Video | VdoCipher (abstracted) |
| Storage | AWS S3 + CloudFront |
| Payments | Paymob |
| Push | OneSignal |
| Email | AWS SES |

### 2.2 Content Hierarchy

```
Course
└── Module (monthly unit)
    └── Stage (chapter)
        └── ContentItem
            ├── Video
            ├── File
            ├── Quiz
            ├── Assignment
            └── Workshop
```

---

## 3. Design Decisions

### 3.1 Key Changes from v1.0

| Issue | Solution |
|-------|----------|
| GUID fragmentation | Sequential UUIDs (uuid_generate_v7) for all PKs |
| High-volume tables | BIGINT PKs for transactions/activity tables |
| Loose FK on Product | Explicit nullable FKs with CHECK constraint |
| JSON overuse | Normalized junction tables for queryable data |
| Missing constraints | CHECK constraints on all business rules |
| No content versioning | Snapshot tables for purchased content |
| Inconsistent cascades | Explicit ON DELETE rules documented |

### 3.2 Primary Key Strategy

```sql
-- For most tables: Sequential UUID (PostgreSQL 17+ native, or extension)
-- Prevents index fragmentation while maintaining distribution benefits
CREATE EXTENSION IF NOT EXISTS pg_uuidv7;

-- For high-volume tables (millions of rows): BIGINT
-- Tables: XPTransaction, PointsTransaction, UserActivity, NotificationDelivery
```

### 3.3 Naming Conventions

| Element | Convention | Example |
|---------|------------|---------|
| Tables | PascalCase, singular | `StudentProfile` |
| Columns | PascalCase | `CreatedAtUtc` |
| Foreign Keys | `{Table}Id` | `StudentId` |
| Indexes | `IX_{Table}_{Columns}` | `IX_Order_StudentId_CreatedAtUtc` |
| Unique | `UQ_{Table}_{Columns}` | `UQ_StudentProfile_UserId` |
| Check | `CK_{Table}_{Rule}` | `CK_Assessment_PassingMarks` |

### 3.4 Standard Audit Columns

All tables include:
```sql
CreatedAtUtc    TIMESTAMPTZ NOT NULL DEFAULT NOW(),
UpdatedAtUtc    TIMESTAMPTZ NOT NULL DEFAULT NOW()
```

---

## 4. Core Domain: Users & Auth

### 4.1 StudyLevel

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK, DEFAULT uuid_generate_v7() |
| Name | VARCHAR(100) | NOT NULL |
| NameEn | VARCHAR(100) | NULL |
| DisplayOrder | SMALLINT | NOT NULL |
| IsActive | BOOLEAN | NOT NULL DEFAULT TRUE |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
CREATE INDEX IX_StudyLevel_DisplayOrder ON StudyLevel(DisplayOrder) WHERE IsActive = TRUE;
```

### 4.2 Track

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| Name | VARCHAR(100) | NOT NULL |
| NameEn | VARCHAR(100) | NULL |
| ParentTrackId | UUID | FK → Track, NULL, ON DELETE SET NULL |
| IsActive | BOOLEAN | NOT NULL DEFAULT TRUE |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

### 4.3 StudyLevelTrack (Valid Combinations)

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| StudyLevelId | UUID | FK → StudyLevel, NOT NULL, ON DELETE CASCADE |
| TrackId | UUID | FK → Track, NOT NULL, ON DELETE CASCADE |
| IsActive | BOOLEAN | NOT NULL DEFAULT TRUE |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
ALTER TABLE StudyLevelTrack 
  ADD CONSTRAINT UQ_StudyLevelTrack_Combo UNIQUE (StudyLevelId, TrackId);

CREATE INDEX IX_StudyLevelTrack_Active ON StudyLevelTrack(StudyLevelId, TrackId) 
  WHERE IsActive = TRUE;
```

### 4.4 User

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| Phone | VARCHAR(20) | NOT NULL, UNIQUE |
| PasswordHash | VARCHAR(500) | NOT NULL |
| FullName | VARCHAR(200) | NOT NULL |
| AvatarUrl | VARCHAR(500) | NULL |
| UserType | SMALLINT | NOT NULL |
| IsPhoneVerified | BOOLEAN | NOT NULL DEFAULT FALSE |
| IsActive | BOOLEAN | NOT NULL DEFAULT TRUE |
| PreferredTimezone | VARCHAR(50) | NOT NULL DEFAULT 'Africa/Cairo' |
| PreferredLanguage | CHAR(2) | NOT NULL DEFAULT 'ar' |
| LastLoginAtUtc | TIMESTAMPTZ | NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
ALTER TABLE "User" ADD CONSTRAINT CK_User_UserType 
  CHECK (UserType BETWEEN 1 AND 5);

CREATE INDEX IX_User_Phone ON "User"(Phone);
CREATE INDEX IX_User_Type_Active ON "User"(UserType) WHERE IsActive = TRUE;
```

### 4.5 StudentProfile

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| UserId | UUID | FK → User, NOT NULL, UNIQUE, ON DELETE CASCADE |
| StudyLevelTrackId | UUID | FK → StudyLevelTrack, NOT NULL |
| SchoolName | VARCHAR(200) | NULL |
| BirthDate | DATE | NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
-- This ensures valid StudyLevel+Track combination
CREATE INDEX IX_StudentProfile_StudyLevelTrack ON StudentProfile(StudyLevelTrackId);
```

### 4.6 TeacherProfile

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| UserId | UUID | FK → User, NOT NULL, UNIQUE, ON DELETE CASCADE |
| Bio | TEXT | NULL |
| Specialization | VARCHAR(200) | NULL |
| ProfileVideoUrl | VARCHAR(500) | NULL |
| IsVerified | BOOLEAN | NOT NULL DEFAULT FALSE |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

### 4.7 TeacherSocialLink

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| TeacherId | UUID | FK → TeacherProfile, NOT NULL, ON DELETE CASCADE |
| Platform | SMALLINT | NOT NULL |
| Url | VARCHAR(500) | NOT NULL |
| DisplayOrder | SMALLINT | NOT NULL DEFAULT 0 |

```sql
-- Platform: 1=Facebook, 2=YouTube, 3=Instagram, 4=Twitter, 5=LinkedIn, 6=TikTok, 7=Website
ALTER TABLE TeacherSocialLink ADD CONSTRAINT CK_TeacherSocialLink_Platform 
  CHECK (Platform BETWEEN 1 AND 7);
```

### 4.8 AssistantProfile

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| UserId | UUID | FK → User, NOT NULL, UNIQUE, ON DELETE CASCADE |
| CreatedByUserId | UUID | FK → User, NOT NULL |
| MaxStudents | INT | NOT NULL DEFAULT 100 |
| CurrentStudentCount | INT | NOT NULL DEFAULT 0 |
| AverageRating | DECIMAL(3,2) | NULL |
| TotalRatings | INT | NOT NULL DEFAULT 0 |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
ALTER TABLE AssistantProfile ADD CONSTRAINT CK_AssistantProfile_Rating 
  CHECK (AverageRating IS NULL OR AverageRating BETWEEN 1.00 AND 5.00);
```

### 4.9 ParentProfile

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| UserId | UUID | FK → User, NOT NULL, UNIQUE, ON DELETE CASCADE |
| Relation | SMALLINT | NOT NULL |
| IsVerified | BOOLEAN | NOT NULL DEFAULT FALSE |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
-- Relation: 1=Father, 2=Mother, 3=Guardian
ALTER TABLE ParentProfile ADD CONSTRAINT CK_ParentProfile_Relation 
  CHECK (Relation BETWEEN 1 AND 3);
```

### 4.10 StudentParent

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| StudentProfileId | UUID | FK → StudentProfile, NOT NULL, ON DELETE CASCADE |
| ParentProfileId | UUID | FK → ParentProfile, NOT NULL, ON DELETE CASCADE |
| IsPrimary | BOOLEAN | NOT NULL DEFAULT FALSE |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
ALTER TABLE StudentParent 
  ADD CONSTRAINT UQ_StudentParent_Combo UNIQUE (StudentProfileId, ParentProfileId);

CREATE INDEX IX_StudentParent_Student ON StudentParent(StudentProfileId);
CREATE INDEX IX_StudentParent_Parent ON StudentParent(ParentProfileId);
```

### 4.11 DeviceSession

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| UserId | UUID | FK → User, NOT NULL, ON DELETE CASCADE |
| DeviceFingerprintHash | CHAR(64) | NOT NULL |
| DeviceName | VARCHAR(200) | NULL |
| Platform | SMALLINT | NOT NULL |
| IpAddress | VARCHAR(45) | NOT NULL |
| RefreshToken | VARCHAR(500) | NOT NULL |
| RefreshTokenExpiresAtUtc | TIMESTAMPTZ | NOT NULL |
| IsActive | BOOLEAN | NOT NULL DEFAULT TRUE |
| LastActiveAtUtc | TIMESTAMPTZ | NOT NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
-- Platform: 1=Web, 2=iOS, 3=Android
ALTER TABLE DeviceSession ADD CONSTRAINT CK_DeviceSession_Platform 
  CHECK (Platform BETWEEN 1 AND 3);

CREATE INDEX IX_DeviceSession_User_Active ON DeviceSession(UserId) WHERE IsActive = TRUE;
CREATE INDEX IX_DeviceSession_Fingerprint ON DeviceSession(DeviceFingerprintHash);
CREATE INDEX IX_DeviceSession_RefreshToken ON DeviceSession(RefreshToken) WHERE IsActive = TRUE;
```

---

## 5. Content Domain

### 5.1 Subject

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| Name | VARCHAR(100) | NOT NULL |
| NameEn | VARCHAR(100) | NULL |
| Icon | VARCHAR(500) | NULL |
| Color | CHAR(7) | NULL |
| IsCore | BOOLEAN | NOT NULL DEFAULT FALSE |
| IsActive | BOOLEAN | NOT NULL DEFAULT TRUE |
| DisplayOrder | SMALLINT | NOT NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
ALTER TABLE Subject ADD CONSTRAINT CK_Subject_Color 
  CHECK (Color IS NULL OR Color ~ '^#[0-9A-Fa-f]{6}$');
```

### 5.2 Course

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| StudyLevelTrackId | UUID | FK → StudyLevelTrack, NOT NULL |
| SubjectId | UUID | FK → Subject, NOT NULL |
| TeacherId | UUID | FK → TeacherProfile, NOT NULL |
| Title | VARCHAR(300) | NOT NULL |
| Description | TEXT | NULL |
| ThumbnailUrl | VARCHAR(500) | NULL |
| IntroVideoUrl | VARCHAR(500) | NULL |
| FullPrice | DECIMAL(10,2) | NOT NULL |
| DisplayOrder | SMALLINT | NOT NULL |
| Visibility | SMALLINT | NOT NULL DEFAULT 0 |
| IsActive | BOOLEAN | NOT NULL DEFAULT TRUE |
| CurrentVersion | INT | NOT NULL DEFAULT 1 |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
-- Visibility: 0=Hidden, 1=Draft, 2=Published
ALTER TABLE Course ADD CONSTRAINT CK_Course_Visibility 
  CHECK (Visibility BETWEEN 0 AND 2);
ALTER TABLE Course ADD CONSTRAINT CK_Course_Price 
  CHECK (FullPrice >= 0);

CREATE INDEX IX_Course_StudyLevelTrack ON Course(StudyLevelTrackId);
CREATE INDEX IX_Course_Subject ON Course(SubjectId);
CREATE INDEX IX_Course_Teacher ON Course(TeacherId);
CREATE INDEX IX_Course_Published ON Course(StudyLevelTrackId, SubjectId) 
  WHERE Visibility = 2 AND IsActive = TRUE;
```

### 5.3 Module

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| CourseId | UUID | FK → Course, NOT NULL, ON DELETE CASCADE |
| Title | VARCHAR(300) | NOT NULL |
| Description | TEXT | NULL |
| ThumbnailUrl | VARCHAR(500) | NULL |
| Price | DECIMAL(10,2) | NOT NULL |
| PriceWithFollowUp | DECIMAL(10,2) | NULL |
| HasFollowUpOption | BOOLEAN | NOT NULL DEFAULT FALSE |
| DisplayOrder | SMALLINT | NOT NULL |
| Visibility | SMALLINT | NOT NULL DEFAULT 0 |
| IsActive | BOOLEAN | NOT NULL DEFAULT TRUE |
| ExpectedDurationDays | SMALLINT | NOT NULL DEFAULT 30 |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
ALTER TABLE Module ADD CONSTRAINT CK_Module_Price 
  CHECK (Price >= 0);
ALTER TABLE Module ADD CONSTRAINT CK_Module_FollowUpPrice 
  CHECK (PriceWithFollowUp IS NULL OR PriceWithFollowUp >= Price);
ALTER TABLE Module ADD CONSTRAINT CK_Module_Visibility 
  CHECK (Visibility BETWEEN 0 AND 2);

CREATE INDEX IX_Module_Course ON Module(CourseId);
CREATE INDEX IX_Module_Published ON Module(CourseId, DisplayOrder) 
  WHERE Visibility = 2 AND IsActive = TRUE;
```

### 5.4 Stage

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| ModuleId | UUID | FK → Module, NOT NULL, ON DELETE CASCADE |
| Title | VARCHAR(300) | NOT NULL |
| Description | TEXT | NULL |
| DisplayOrder | SMALLINT | NOT NULL |
| Visibility | SMALLINT | NOT NULL DEFAULT 0 |
| IsActive | BOOLEAN | NOT NULL DEFAULT TRUE |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
CREATE INDEX IX_Stage_Module ON Stage(ModuleId, DisplayOrder);
```

### 5.5 ContentItem

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| StageId | UUID | FK → Stage, NULL, ON DELETE SET NULL |
| CourseId | UUID | FK → Course, NOT NULL |
| ContentType | SMALLINT | NOT NULL |
| Title | VARCHAR(300) | NOT NULL |
| Description | TEXT | NULL |
| ThumbnailUrl | VARCHAR(500) | NULL |
| DisplayOrder | SMALLINT | NOT NULL |
| IsFreePreview | BOOLEAN | NOT NULL DEFAULT FALSE |
| IsStandalone | BOOLEAN | NOT NULL DEFAULT FALSE |
| StandalonePrice | DECIMAL(10,2) | NULL |
| XpReward | SMALLINT | NOT NULL DEFAULT 0 |
| PointsReward | SMALLINT | NOT NULL DEFAULT 0 |
| EstimatedMinutes | SMALLINT | NULL |
| Visibility | SMALLINT | NOT NULL DEFAULT 0 |
| IsActive | BOOLEAN | NOT NULL DEFAULT TRUE |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
-- ContentType: 1=Video, 2=File, 3=Quiz, 4=Assignment, 5=Workshop
ALTER TABLE ContentItem ADD CONSTRAINT CK_ContentItem_Type 
  CHECK (ContentType BETWEEN 1 AND 5);
ALTER TABLE ContentItem ADD CONSTRAINT CK_ContentItem_StandalonePrice 
  CHECK (NOT IsStandalone OR StandalonePrice IS NOT NULL);
ALTER TABLE ContentItem ADD CONSTRAINT CK_ContentItem_Rewards 
  CHECK (XpReward >= 0 AND PointsReward >= 0);

CREATE INDEX IX_ContentItem_Stage ON ContentItem(StageId, DisplayOrder);
CREATE INDEX IX_ContentItem_Course ON ContentItem(CourseId);
CREATE INDEX IX_ContentItem_Standalone ON ContentItem(CourseId) 
  WHERE IsStandalone = TRUE AND IsActive = TRUE;
```

### 5.6 VideoContent

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| ContentItemId | UUID | FK → ContentItem, NOT NULL, UNIQUE, ON DELETE CASCADE |
| ProviderId | VARCHAR(50) | NOT NULL |
| ExternalVideoId | VARCHAR(500) | NOT NULL |
| DurationSeconds | INT | NOT NULL |
| OriginalFileName | VARCHAR(300) | NULL |
| FileSize | BIGINT | NULL |
| Resolution | VARCHAR(10) | NULL |
| Status | SMALLINT | NOT NULL DEFAULT 0 |
| TranscriptUrl | VARCHAR(500) | NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
-- Status: 0=Processing, 1=Ready, 2=Failed
ALTER TABLE VideoContent ADD CONSTRAINT CK_VideoContent_Status 
  CHECK (Status BETWEEN 0 AND 2);
ALTER TABLE VideoContent ADD CONSTRAINT CK_VideoContent_Duration 
  CHECK (DurationSeconds > 0);

CREATE INDEX IX_VideoContent_Provider ON VideoContent(ProviderId, ExternalVideoId);
```

### 5.7 VideoProviderMetadata

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| VideoContentId | UUID | FK → VideoContent, NOT NULL, ON DELETE CASCADE |
| MetadataKey | VARCHAR(100) | NOT NULL |
| MetadataValue | TEXT | NOT NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
ALTER TABLE VideoProviderMetadata 
  ADD CONSTRAINT UQ_VideoProviderMetadata UNIQUE (VideoContentId, MetadataKey);
```

### 5.8 FileContent

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| ContentItemId | UUID | FK → ContentItem, NOT NULL, UNIQUE, ON DELETE CASCADE |
| StorageProvider | VARCHAR(20) | NOT NULL DEFAULT 's3' |
| StoragePath | VARCHAR(1000) | NOT NULL |
| PublicUrl | VARCHAR(1000) | NULL |
| OriginalFileName | VARCHAR(300) | NOT NULL |
| FileExtension | VARCHAR(20) | NOT NULL |
| MimeType | VARCHAR(100) | NOT NULL |
| FileSize | BIGINT | NOT NULL |
| AllowDownload | BOOLEAN | NOT NULL DEFAULT FALSE |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

### 5.9 ContentPrerequisite

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| ContentItemId | UUID | FK → ContentItem, NOT NULL, ON DELETE CASCADE |
| PrerequisiteType | SMALLINT | NOT NULL |
| TargetContentId | UUID | FK → ContentItem, NULL, ON DELETE CASCADE |
| TargetStageId | UUID | FK → Stage, NULL, ON DELETE CASCADE |
| TargetModuleId | UUID | FK → Module, NULL, ON DELETE CASCADE |
| TargetAssessmentId | UUID | FK → Assessment, NULL, ON DELETE CASCADE |
| RequiredScore | SMALLINT | NULL |
| DelayDays | SMALLINT | NULL |
| GroupOperator | SMALLINT | NOT NULL DEFAULT 1 |
| GroupId | SMALLINT | NOT NULL DEFAULT 0 |
| DisplayOrder | SMALLINT | NOT NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
-- PrerequisiteType: 1=CompleteContent, 2=CompleteStage, 3=CompleteModule, 
--                   4=PassAssessment, 5=FollowUpApproval, 6=Purchase, 7=TimeDelay
-- GroupOperator: 1=AND, 2=OR

ALTER TABLE ContentPrerequisite ADD CONSTRAINT CK_ContentPrerequisite_Type 
  CHECK (PrerequisiteType BETWEEN 1 AND 7);
ALTER TABLE ContentPrerequisite ADD CONSTRAINT CK_ContentPrerequisite_Target 
  CHECK (
    (PrerequisiteType = 1 AND TargetContentId IS NOT NULL) OR
    (PrerequisiteType = 2 AND TargetStageId IS NOT NULL) OR
    (PrerequisiteType = 3 AND TargetModuleId IS NOT NULL) OR
    (PrerequisiteType = 4 AND TargetAssessmentId IS NOT NULL) OR
    (PrerequisiteType IN (5, 6)) OR
    (PrerequisiteType = 7 AND DelayDays IS NOT NULL)
  );

CREATE INDEX IX_ContentPrerequisite_Content ON ContentPrerequisite(ContentItemId);
```

### 5.10 StudentContentProgress

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| StudentId | UUID | FK → StudentProfile, NOT NULL, ON DELETE CASCADE |
| ContentItemId | UUID | FK → ContentItem, NOT NULL, ON DELETE CASCADE |
| Status | SMALLINT | NOT NULL DEFAULT 0 |
| ProgressPercent | DECIMAL(5,2) | NOT NULL DEFAULT 0 |
| LastPosition | INT | NULL |
| TimeSpentSeconds | INT | NOT NULL DEFAULT 0 |
| StartedAtUtc | TIMESTAMPTZ | NULL |
| CompletedAtUtc | TIMESTAMPTZ | NULL |
| LastAccessedAtUtc | TIMESTAMPTZ | NOT NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
-- Status: 0=NotStarted, 1=InProgress, 2=Completed
ALTER TABLE StudentContentProgress ADD CONSTRAINT CK_StudentContentProgress_Status 
  CHECK (Status BETWEEN 0 AND 2);
ALTER TABLE StudentContentProgress ADD CONSTRAINT CK_StudentContentProgress_Progress 
  CHECK (ProgressPercent BETWEEN 0 AND 100);

ALTER TABLE StudentContentProgress 
  ADD CONSTRAINT UQ_StudentContentProgress UNIQUE (StudentId, ContentItemId);

CREATE INDEX IX_StudentContentProgress_Student ON StudentContentProgress(StudentId, Status);
CREATE INDEX IX_StudentContentProgress_Content ON StudentContentProgress(ContentItemId);
CREATE INDEX IX_StudentContentProgress_Recent ON StudentContentProgress(StudentId, LastAccessedAtUtc DESC);
```

### 5.11 StudentBookmark

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| StudentId | UUID | FK → StudentProfile, NOT NULL, ON DELETE CASCADE |
| ContentItemId | UUID | FK → ContentItem, NOT NULL, ON DELETE CASCADE |
| Notes | VARCHAR(1000) | NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
ALTER TABLE StudentBookmark 
  ADD CONSTRAINT UQ_StudentBookmark UNIQUE (StudentId, ContentItemId);
```

---

## 6. Assessment Domain

### 6.1 QuestionBank

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| CourseId | UUID | FK → Course, NOT NULL, ON DELETE CASCADE |
| Name | VARCHAR(200) | NOT NULL |
| Description | TEXT | NULL |
| IsActive | BOOLEAN | NOT NULL DEFAULT TRUE |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

### 6.2 Question

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| QuestionBankId | UUID | FK → QuestionBank, NOT NULL, ON DELETE CASCADE |
| QuestionType | SMALLINT | NOT NULL |
| QuestionText | TEXT | NOT NULL |
| QuestionImageUrl | VARCHAR(500) | NULL |
| Difficulty | SMALLINT | NOT NULL DEFAULT 2 |
| DefaultMarks | DECIMAL(5,2) | NOT NULL |
| Explanation | TEXT | NULL |
| IsActive | BOOLEAN | NOT NULL DEFAULT TRUE |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
-- QuestionType: 1=MCQ, 2=TrueFalse, 3=FileUpload
-- Difficulty: 1=Easy, 2=Medium, 3=Hard
ALTER TABLE Question ADD CONSTRAINT CK_Question_Type 
  CHECK (QuestionType BETWEEN 1 AND 3);
ALTER TABLE Question ADD CONSTRAINT CK_Question_Difficulty 
  CHECK (Difficulty BETWEEN 1 AND 3);
ALTER TABLE Question ADD CONSTRAINT CK_Question_Marks 
  CHECK (DefaultMarks > 0);

CREATE INDEX IX_Question_Bank ON Question(QuestionBankId);
CREATE INDEX IX_Question_Bank_Difficulty ON Question(QuestionBankId, Difficulty) 
  WHERE IsActive = TRUE;
```

### 6.3 QuestionTag

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| QuestionId | UUID | FK → Question, NOT NULL, ON DELETE CASCADE |
| Tag | VARCHAR(50) | NOT NULL |

```sql
ALTER TABLE QuestionTag 
  ADD CONSTRAINT UQ_QuestionTag UNIQUE (QuestionId, Tag);
CREATE INDEX IX_QuestionTag_Tag ON QuestionTag(Tag);
```

### 6.4 QuestionOption

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| QuestionId | UUID | FK → Question, NOT NULL, ON DELETE CASCADE |
| OptionText | TEXT | NOT NULL |
| OptionImageUrl | VARCHAR(500) | NULL |
| IsCorrect | BOOLEAN | NOT NULL |
| DisplayOrder | SMALLINT | NOT NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
CREATE INDEX IX_QuestionOption_Question ON QuestionOption(QuestionId, DisplayOrder);
```

### 6.5 GradingRubric

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| Name | VARCHAR(200) | NOT NULL |
| Description | TEXT | NULL |
| IsDefault | BOOLEAN | NOT NULL DEFAULT FALSE |
| IsActive | BOOLEAN | NOT NULL DEFAULT TRUE |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

### 6.6 RubricCriteria

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| RubricId | UUID | FK → GradingRubric, NOT NULL, ON DELETE CASCADE |
| Name | VARCHAR(200) | NOT NULL |
| Description | TEXT | NULL |
| MaxScore | DECIMAL(5,2) | NOT NULL |
| DisplayOrder | SMALLINT | NOT NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
ALTER TABLE RubricCriteria ADD CONSTRAINT CK_RubricCriteria_Score 
  CHECK (MaxScore > 0);
```

### 6.7 Assessment

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| ContentItemId | UUID | FK → ContentItem, NULL, ON DELETE SET NULL |
| CourseId | UUID | FK → Course, NOT NULL |
| ModuleId | UUID | FK → Module, NULL |
| AssessmentType | SMALLINT | NOT NULL |
| Title | VARCHAR(300) | NOT NULL |
| Instructions | TEXT | NULL |
| TotalMarks | DECIMAL(5,2) | NOT NULL |
| PassingMarks | DECIMAL(5,2) | NOT NULL |
| DurationMinutes | SMALLINT | NULL |
| MaxRetakes | SMALLINT | NOT NULL DEFAULT 5 |
| RetakeCooldownMinutes | SMALLINT | NOT NULL DEFAULT 5 |
| ShuffleQuestions | BOOLEAN | NOT NULL DEFAULT TRUE |
| ShuffleOptions | BOOLEAN | NOT NULL DEFAULT TRUE |
| ShowCorrectAnswers | BOOLEAN | NOT NULL DEFAULT TRUE |
| AllowLateSubmission | BOOLEAN | NOT NULL DEFAULT TRUE |
| AvailableFromUtc | TIMESTAMPTZ | NULL |
| DeadlineUtc | TIMESTAMPTZ | NULL |
| XpReward | SMALLINT | NOT NULL DEFAULT 0 |
| PointsReward | SMALLINT | NOT NULL DEFAULT 0 |
| XpRewardIfLate | SMALLINT | NOT NULL DEFAULT 0 |
| PointsRewardIfLate | SMALLINT | NOT NULL DEFAULT 0 |
| GradingRubricId | UUID | FK → GradingRubric, NULL |
| Visibility | SMALLINT | NOT NULL DEFAULT 0 |
| IsActive | BOOLEAN | NOT NULL DEFAULT TRUE |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
-- AssessmentType: 1=Quiz, 2=Assignment, 3=FinalExam
ALTER TABLE Assessment ADD CONSTRAINT CK_Assessment_Type 
  CHECK (AssessmentType BETWEEN 1 AND 3);
ALTER TABLE Assessment ADD CONSTRAINT CK_Assessment_PassingMarks 
  CHECK (PassingMarks > 0 AND PassingMarks <= TotalMarks);
ALTER TABLE Assessment ADD CONSTRAINT CK_Assessment_Retakes 
  CHECK (MaxRetakes >= 1);
ALTER TABLE Assessment ADD CONSTRAINT CK_Assessment_Rewards 
  CHECK (XpReward >= 0 AND PointsReward >= 0 AND XpRewardIfLate >= 0 AND PointsRewardIfLate >= 0);

CREATE INDEX IX_Assessment_Course ON Assessment(CourseId);
CREATE INDEX IX_Assessment_Module ON Assessment(ModuleId);
CREATE INDEX IX_Assessment_Content ON Assessment(ContentItemId);
```

### 6.8 AssessmentQuestion

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| AssessmentId | UUID | FK → Assessment, NOT NULL, ON DELETE CASCADE |
| QuestionId | UUID | FK → Question, NOT NULL |
| Marks | DECIMAL(5,2) | NOT NULL |
| DisplayOrder | SMALLINT | NOT NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
ALTER TABLE AssessmentQuestion 
  ADD CONSTRAINT UQ_AssessmentQuestion UNIQUE (AssessmentId, QuestionId);
ALTER TABLE AssessmentQuestion ADD CONSTRAINT CK_AssessmentQuestion_Marks 
  CHECK (Marks > 0);
```

### 6.9 StudentAttempt

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| StudentId | UUID | FK → StudentProfile, NOT NULL, ON DELETE CASCADE |
| AssessmentId | UUID | FK → Assessment, NOT NULL |
| AttemptNumber | SMALLINT | NOT NULL |
| Status | SMALLINT | NOT NULL DEFAULT 0 |
| StartedAtUtc | TIMESTAMPTZ | NOT NULL |
| SubmittedAtUtc | TIMESTAMPTZ | NULL |
| DueAtUtc | TIMESTAMPTZ | NULL |
| IsLateSubmission | BOOLEAN | NOT NULL DEFAULT FALSE |
| TotalScore | DECIMAL(5,2) | NULL |
| PercentageScore | DECIMAL(5,2) | NULL |
| IsPassed | BOOLEAN | NULL |
| GradedByAssistantId | UUID | FK → AssistantProfile, NULL |
| GradedAtUtc | TIMESTAMPTZ | NULL |
| XpAwarded | SMALLINT | NOT NULL DEFAULT 0 |
| PointsAwarded | SMALLINT | NOT NULL DEFAULT 0 |
| GraderNotes | TEXT | NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
-- Status: 0=InProgress, 1=Submitted, 2=Graded, 3=Abandoned, 4=TimedOut
ALTER TABLE StudentAttempt ADD CONSTRAINT CK_StudentAttempt_Status 
  CHECK (Status BETWEEN 0 AND 4);
ALTER TABLE StudentAttempt ADD CONSTRAINT CK_StudentAttempt_Percentage 
  CHECK (PercentageScore IS NULL OR PercentageScore BETWEEN 0 AND 100);
ALTER TABLE StudentAttempt ADD CONSTRAINT CK_StudentAttempt_AttemptNumber 
  CHECK (AttemptNumber >= 1);

CREATE INDEX IX_StudentAttempt_Student ON StudentAttempt(StudentId, AssessmentId);
CREATE INDEX IX_StudentAttempt_Assessment ON StudentAttempt(AssessmentId, Status);
CREATE INDEX IX_StudentAttempt_Grading ON StudentAttempt(AssessmentId, Status) 
  WHERE Status = 1; -- Needs grading queue
```

### 6.10 StudentAnswer

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| AttemptId | UUID | FK → StudentAttempt, NOT NULL, ON DELETE CASCADE |
| QuestionId | UUID | FK → Question, NOT NULL |
| SelectedOptionId | UUID | FK → QuestionOption, NULL |
| BooleanAnswer | BOOLEAN | NULL |
| UploadedFileUrl | VARCHAR(1000) | NULL |
| UploadedFileName | VARCHAR(300) | NULL |
| IsCorrect | BOOLEAN | NULL |
| Score | DECIMAL(5,2) | NULL |
| Feedback | TEXT | NULL |
| AnsweredAtUtc | TIMESTAMPTZ | NOT NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
ALTER TABLE StudentAnswer 
  ADD CONSTRAINT UQ_StudentAnswer UNIQUE (AttemptId, QuestionId);

CREATE INDEX IX_StudentAnswer_Attempt ON StudentAnswer(AttemptId);
```

### 6.11 AttemptRubricScore

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| AttemptId | UUID | FK → StudentAttempt, NOT NULL, ON DELETE CASCADE |
| CriteriaId | UUID | FK → RubricCriteria, NOT NULL |
| Score | DECIMAL(5,2) | NOT NULL |
| Notes | TEXT | NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
ALTER TABLE AttemptRubricScore 
  ADD CONSTRAINT UQ_AttemptRubricScore UNIQUE (AttemptId, CriteriaId);
ALTER TABLE AttemptRubricScore ADD CONSTRAINT CK_AttemptRubricScore_Score 
  CHECK (Score >= 0);
```

### 6.12 StudentAssessmentScore (Cached)

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| StudentId | UUID | FK → StudentProfile, NOT NULL, ON DELETE CASCADE |
| AssessmentId | UUID | FK → Assessment, NOT NULL |
| BestScore | DECIMAL(5,2) | NOT NULL |
| BestAttemptId | UUID | FK → StudentAttempt, NOT NULL |
| LastScore | DECIMAL(5,2) | NOT NULL |
| LastAttemptId | UUID | FK → StudentAttempt, NOT NULL |
| TotalAttempts | SMALLINT | NOT NULL |
| LastAttemptAtUtc | TIMESTAMPTZ | NOT NULL |
| NextRetryAvailableAtUtc | TIMESTAMPTZ | NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
ALTER TABLE StudentAssessmentScore 
  ADD CONSTRAINT UQ_StudentAssessmentScore UNIQUE (StudentId, AssessmentId);

CREATE INDEX IX_StudentAssessmentScore_Assessment ON StudentAssessmentScore(AssessmentId);
```

---

## 7. Study Plan Domain

### 7.1 StudyPlanSettings

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| StudentId | UUID | FK → StudentProfile, NOT NULL, ON DELETE CASCADE |
| ModuleId | UUID | FK → Module, NOT NULL |
| DailyStudyMinutes | SMALLINT | NOT NULL |
| PreferredStartTime | TIME | NULL |
| IncludeReviewDays | BOOLEAN | NOT NULL DEFAULT TRUE |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
ALTER TABLE StudyPlanSettings 
  ADD CONSTRAINT UQ_StudyPlanSettings UNIQUE (StudentId, ModuleId);
ALTER TABLE StudyPlanSettings ADD CONSTRAINT CK_StudyPlanSettings_Minutes 
  CHECK (DailyStudyMinutes BETWEEN 30 AND 480);
```

### 7.2 StudyPlanStudyDay

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| SettingsId | UUID | FK → StudyPlanSettings, NOT NULL, ON DELETE CASCADE |
| DayOfWeek | SMALLINT | NOT NULL |

```sql
-- DayOfWeek: 0=Sunday, 1=Monday, ..., 6=Saturday
ALTER TABLE StudyPlanStudyDay ADD CONSTRAINT CK_StudyPlanStudyDay_Day 
  CHECK (DayOfWeek BETWEEN 0 AND 6);
ALTER TABLE StudyPlanStudyDay 
  ADD CONSTRAINT UQ_StudyPlanStudyDay UNIQUE (SettingsId, DayOfWeek);
```

### 7.3 StudyPlan

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| StudentId | UUID | FK → StudentProfile, NOT NULL, ON DELETE CASCADE |
| ModuleId | UUID | FK → Module, NOT NULL |
| SettingsId | UUID | FK → StudyPlanSettings, NOT NULL |
| StartDate | DATE | NOT NULL |
| EndDate | DATE | NOT NULL |
| ModuleReleaseDate | DATE | NOT NULL |
| IsLatePurchase | BOOLEAN | NOT NULL DEFAULT FALSE |
| Status | SMALLINT | NOT NULL DEFAULT 0 |
| CompletionPercent | DECIMAL(5,2) | NOT NULL DEFAULT 0 |
| TotalPlannedTasks | SMALLINT | NOT NULL |
| CompletedTasks | SMALLINT | NOT NULL DEFAULT 0 |
| MissedTasks | SMALLINT | NOT NULL DEFAULT 0 |
| RescheduledCount | SMALLINT | NOT NULL DEFAULT 0 |
| GeneratedAtUtc | TIMESTAMPTZ | NOT NULL |
| LastAdjustedAtUtc | TIMESTAMPTZ | NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
-- Status: 0=Active, 1=Completed, 2=Abandoned
ALTER TABLE StudyPlan ADD CONSTRAINT CK_StudyPlan_Status 
  CHECK (Status BETWEEN 0 AND 2);
ALTER TABLE StudyPlan ADD CONSTRAINT CK_StudyPlan_Dates 
  CHECK (EndDate >= StartDate);
ALTER TABLE StudyPlan ADD CONSTRAINT CK_StudyPlan_Completion 
  CHECK (CompletionPercent BETWEEN 0 AND 100);

CREATE INDEX IX_StudyPlan_Student_Status ON StudyPlan(StudentId, Status);
CREATE INDEX IX_StudyPlan_Module ON StudyPlan(ModuleId);
```

### 7.4 PlanDay

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| StudyPlanId | UUID | FK → StudyPlan, NOT NULL, ON DELETE CASCADE |
| Date | DATE | NOT NULL |
| DayType | SMALLINT | NOT NULL |
| Status | SMALLINT | NOT NULL DEFAULT 0 |
| PlannedMinutes | SMALLINT | NOT NULL |
| ActualMinutes | SMALLINT | NULL |
| Notes | VARCHAR(500) | NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
-- DayType: 1=Study, 2=Review, 3=Rest, 4=FollowUp
-- Status: 0=Upcoming, 1=Completed, 2=Missed, 3=Rescheduled
ALTER TABLE PlanDay 
  ADD CONSTRAINT UQ_PlanDay UNIQUE (StudyPlanId, Date);
ALTER TABLE PlanDay ADD CONSTRAINT CK_PlanDay_DayType 
  CHECK (DayType BETWEEN 1 AND 4);
ALTER TABLE PlanDay ADD CONSTRAINT CK_PlanDay_Status 
  CHECK (Status BETWEEN 0 AND 3);

CREATE INDEX IX_PlanDay_Date ON PlanDay(Date, Status);
```

### 7.5 PlanTask

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| PlanDayId | UUID | FK → PlanDay, NOT NULL, ON DELETE CASCADE |
| ContentItemId | UUID | FK → ContentItem, NULL |
| TaskType | SMALLINT | NOT NULL |
| Title | VARCHAR(300) | NOT NULL |
| Description | TEXT | NULL |
| EstimatedMinutes | SMALLINT | NOT NULL |
| DisplayOrder | SMALLINT | NOT NULL |
| Status | SMALLINT | NOT NULL DEFAULT 0 |
| CompletedAtUtc | TIMESTAMPTZ | NULL |
| ActualMinutes | SMALLINT | NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
-- TaskType: 1=Lecture, 2=Quiz, 3=Assignment, 4=Review, 5=ReviewQuiz
-- Status: 0=Todo, 1=InProgress, 2=Completed, 3=Missed, 4=Skipped
ALTER TABLE PlanTask ADD CONSTRAINT CK_PlanTask_TaskType 
  CHECK (TaskType BETWEEN 1 AND 5);
ALTER TABLE PlanTask ADD CONSTRAINT CK_PlanTask_Status 
  CHECK (Status BETWEEN 0 AND 4);

CREATE INDEX IX_PlanTask_Day ON PlanTask(PlanDayId, DisplayOrder);
CREATE INDEX IX_PlanTask_Content ON PlanTask(ContentItemId);
```

### 7.6 ReviewContent

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| PlanTaskId | UUID | FK → PlanTask, NOT NULL, ON DELETE CASCADE |
| ContentItemId | UUID | FK → ContentItem, NOT NULL |
| DisplayOrder | SMALLINT | NOT NULL |
| IsCompleted | BOOLEAN | NOT NULL DEFAULT FALSE |
| CompletedAtUtc | TIMESTAMPTZ | NULL |

### 7.7 ReviewQuiz

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| PlanTaskId | UUID | FK → PlanTask, NOT NULL, UNIQUE, ON DELETE CASCADE |
| StageId | UUID | FK → Stage, NOT NULL |
| TotalQuestions | SMALLINT | NOT NULL |
| PassingScore | DECIMAL(5,2) | NOT NULL |
| GeneratedAtUtc | TIMESTAMPTZ | NOT NULL |

### 7.8 ReviewQuizQuestion

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| ReviewQuizId | UUID | FK → ReviewQuiz, NOT NULL, ON DELETE CASCADE |
| QuestionId | UUID | FK → Question, NOT NULL |
| DisplayOrder | SMALLINT | NOT NULL |

### 7.9 PlanAdjustmentLog

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| StudyPlanId | UUID | FK → StudyPlan, NOT NULL, ON DELETE CASCADE |
| AdjustmentType | SMALLINT | NOT NULL |
| Reason | VARCHAR(500) | NOT NULL |
| AdjustedByUserId | UUID | FK → User, NULL |
| TasksRescheduled | SMALLINT | NOT NULL DEFAULT 0 |
| ReviewTasksAdded | SMALLINT | NOT NULL DEFAULT 0 |
| PreviousEndDate | DATE | NULL |
| NewEndDate | DATE | NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
-- AdjustmentType: 1=MissedDay, 2=LowScore, 3=FollowUpFeedback, 4=Manual, 5=Reschedule
ALTER TABLE PlanAdjustmentLog ADD CONSTRAINT CK_PlanAdjustmentLog_Type 
  CHECK (AdjustmentType BETWEEN 1 AND 5);

CREATE INDEX IX_PlanAdjustmentLog_Plan ON PlanAdjustmentLog(StudyPlanId, CreatedAtUtc DESC);
```

---

## 8. Follow-Up Domain

### 8.1 FollowUpGroup

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| Name | VARCHAR(200) | NOT NULL |
| CourseId | UUID | FK → Course, NOT NULL |
| ModuleId | UUID | FK → Module, NOT NULL |
| StudyLevelTrackId | UUID | FK → StudyLevelTrack, NOT NULL |
| AssistantId | UUID | FK → AssistantProfile, NOT NULL |
| MaxStudents | SMALLINT | NOT NULL |
| CurrentStudentCount | SMALLINT | NOT NULL DEFAULT 0 |
| IsAcceptingNewStudents | BOOLEAN | NOT NULL DEFAULT TRUE |
| IsActive | BOOLEAN | NOT NULL DEFAULT TRUE |
| CreatedByUserId | UUID | FK → User, NOT NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
ALTER TABLE FollowUpGroup 
  ADD CONSTRAINT UQ_FollowUpGroup_ModuleAssistant UNIQUE (ModuleId, AssistantId);
ALTER TABLE FollowUpGroup ADD CONSTRAINT CK_FollowUpGroup_Count 
  CHECK (CurrentStudentCount >= 0 AND CurrentStudentCount <= MaxStudents);

CREATE INDEX IX_FollowUpGroup_Course_Module ON FollowUpGroup(CourseId, ModuleId);
CREATE INDEX IX_FollowUpGroup_Assistant ON FollowUpGroup(AssistantId) WHERE IsActive = TRUE;
CREATE INDEX IX_FollowUpGroup_Available ON FollowUpGroup(ModuleId, StudyLevelTrackId) 
  WHERE IsAcceptingNewStudents = TRUE AND IsActive = TRUE;
```

### 8.2 FollowUpWaitingList

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| StudentId | UUID | FK → StudentProfile, NOT NULL, ON DELETE CASCADE |
| ModuleId | UUID | FK → Module, NOT NULL |
| EnrollmentId | UUID | FK → StudentEnrollment, NOT NULL |
| PreferredAssistantId | UUID | FK → AssistantProfile, NULL |
| Position | SMALLINT | NOT NULL |
| Status | SMALLINT | NOT NULL DEFAULT 0 |
| AssignedGroupId | UUID | FK → FollowUpGroup, NULL |
| AssignedAtUtc | TIMESTAMPTZ | NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
-- Status: 0=Waiting, 1=Assigned, 2=Cancelled
ALTER TABLE FollowUpWaitingList ADD CONSTRAINT CK_FollowUpWaitingList_Status 
  CHECK (Status BETWEEN 0 AND 2);

CREATE INDEX IX_FollowUpWaitingList_Module ON FollowUpWaitingList(ModuleId, Status, Position) 
  WHERE Status = 0;
```

### 8.3 StudentFollowUpEnrollment

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| StudentId | UUID | FK → StudentProfile, NOT NULL, ON DELETE CASCADE |
| GroupId | UUID | FK → FollowUpGroup, NOT NULL |
| EnrollmentId | UUID | FK → StudentEnrollment, NOT NULL |
| Status | SMALLINT | NOT NULL DEFAULT 0 |
| StartDateUtc | TIMESTAMPTZ | NOT NULL |
| EndDateUtc | TIMESTAMPTZ | NULL |
| TotalSessionsScheduled | SMALLINT | NOT NULL DEFAULT 0 |
| TotalSessionsCompleted | SMALLINT | NOT NULL DEFAULT 0 |
| TotalSessionsMissed | SMALLINT | NOT NULL DEFAULT 0 |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
-- Status: 0=Active, 1=Paused, 2=Cancelled, 3=Completed
ALTER TABLE StudentFollowUpEnrollment 
  ADD CONSTRAINT UQ_StudentFollowUpEnrollment UNIQUE (StudentId, GroupId);
ALTER TABLE StudentFollowUpEnrollment ADD CONSTRAINT CK_StudentFollowUpEnrollment_Status 
  CHECK (Status BETWEEN 0 AND 3);

CREATE INDEX IX_StudentFollowUpEnrollment_Group ON StudentFollowUpEnrollment(GroupId, Status);
CREATE INDEX IX_StudentFollowUpEnrollment_Student ON StudentFollowUpEnrollment(StudentId, Status);
```

### 8.4 StudentAssistantHistory

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| StudentId | UUID | FK → StudentProfile, NOT NULL, ON DELETE CASCADE |
| AssistantId | UUID | FK → AssistantProfile, NOT NULL |
| CourseId | UUID | FK → Course, NOT NULL |
| ModuleId | UUID | FK → Module, NOT NULL |
| GroupId | UUID | FK → FollowUpGroup, NOT NULL |
| IsLatest | BOOLEAN | NOT NULL DEFAULT TRUE |
| AssignedAtUtc | TIMESTAMPTZ | NOT NULL |
| EndedAtUtc | TIMESTAMPTZ | NULL |

```sql
CREATE INDEX IX_StudentAssistantHistory_Student ON StudentAssistantHistory(StudentId, CourseId, IsLatest);
CREATE INDEX IX_StudentAssistantHistory_Assistant ON StudentAssistantHistory(AssistantId);
```

### 8.5 FollowUpSession

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| StudentFollowUpEnrollmentId | UUID | FK → StudentFollowUpEnrollment, NOT NULL, ON DELETE CASCADE |
| AssistantId | UUID | FK → AssistantProfile, NOT NULL |
| TriggerType | SMALLINT | NOT NULL |
| TriggerStageId | UUID | FK → Stage, NULL |
| ScheduledAtUtc | TIMESTAMPTZ | NULL |
| StartedAtUtc | TIMESTAMPTZ | NULL |
| EndedAtUtc | TIMESTAMPTZ | NULL |
| DurationMinutes | SMALLINT | NULL |
| CallAttempts | SMALLINT | NOT NULL DEFAULT 0 |
| LastCallAttemptAtUtc | TIMESTAMPTZ | NULL |
| CallStatus | SMALLINT | NOT NULL DEFAULT 0 |
| CallNotes | TEXT | NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
-- TriggerType: 1=AfterStage, 2=Scheduled, 3=Manual
-- CallStatus: 0=Pending, 1=Scheduled, 2=InProgress, 3=Completed, 4=NoAnswer, 5=Rescheduled
ALTER TABLE FollowUpSession ADD CONSTRAINT CK_FollowUpSession_TriggerType 
  CHECK (TriggerType BETWEEN 1 AND 3);
ALTER TABLE FollowUpSession ADD CONSTRAINT CK_FollowUpSession_CallStatus 
  CHECK (CallStatus BETWEEN 0 AND 5);

CREATE INDEX IX_FollowUpSession_Enrollment ON FollowUpSession(StudentFollowUpEnrollmentId);
CREATE INDEX IX_FollowUpSession_Assistant ON FollowUpSession(AssistantId, CallStatus);
CREATE INDEX IX_FollowUpSession_Pending ON FollowUpSession(AssistantId, ScheduledAtUtc) 
  WHERE CallStatus IN (0, 1);
```

### 8.6 FollowUpEvaluation

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| SessionId | UUID | FK → FollowUpSession, NOT NULL, UNIQUE, ON DELETE CASCADE |
| CommitmentScore | SMALLINT | NOT NULL |
| UnderstandingScore | SMALLINT | NOT NULL |
| OverallScore | DECIMAL(3,2) | NOT NULL |
| StudentStatus | SMALLINT | NOT NULL |
| PrivateNotes | TEXT | NULL |
| ParentNotes | TEXT | NULL |
| RecommendedAction | SMALLINT | NOT NULL |
| UnlockNextStage | BOOLEAN | NOT NULL DEFAULT FALSE |
| NextStageId | UUID | FK → Stage, NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
-- StudentStatus: 1=Excellent, 2=Good, 3=NeedsImprovement, 4=AtRisk
-- RecommendedAction: 1=Continue, 2=AddReview, 3=Warning, 4=ContactParent
ALTER TABLE FollowUpEvaluation ADD CONSTRAINT CK_FollowUpEvaluation_CommitmentScore 
  CHECK (CommitmentScore BETWEEN 1 AND 5);
ALTER TABLE FollowUpEvaluation ADD CONSTRAINT CK_FollowUpEvaluation_UnderstandingScore 
  CHECK (UnderstandingScore BETWEEN 1 AND 5);
ALTER TABLE FollowUpEvaluation ADD CONSTRAINT CK_FollowUpEvaluation_OverallScore 
  CHECK (OverallScore BETWEEN 1.00 AND 5.00);
ALTER TABLE FollowUpEvaluation ADD CONSTRAINT CK_FollowUpEvaluation_StudentStatus 
  CHECK (StudentStatus BETWEEN 1 AND 4);
ALTER TABLE FollowUpEvaluation ADD CONSTRAINT CK_FollowUpEvaluation_RecommendedAction 
  CHECK (RecommendedAction BETWEEN 1 AND 4);
```

### 8.7 StudentWarning

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| StudentId | UUID | FK → StudentProfile, NOT NULL, ON DELETE CASCADE |
| IssuedByUserId | UUID | FK → User, NOT NULL |
| SessionId | UUID | FK → FollowUpSession, NULL |
| WarningType | SMALLINT | NOT NULL |
| Severity | SMALLINT | NOT NULL |
| Message | VARCHAR(1000) | NOT NULL |
| IsAcknowledged | BOOLEAN | NOT NULL DEFAULT FALSE |
| AcknowledgedAtUtc | TIMESTAMPTZ | NULL |
| NotifyParent | BOOLEAN | NOT NULL DEFAULT FALSE |
| ParentNotifiedAtUtc | TIMESTAMPTZ | NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
-- WarningType: 1=LowCommitment, 2=MissedSessions, 3=PoorPerformance, 4=Other
-- Severity: 1=Mild, 2=Moderate, 3=Severe
ALTER TABLE StudentWarning ADD CONSTRAINT CK_StudentWarning_Type 
  CHECK (WarningType BETWEEN 1 AND 4);
ALTER TABLE StudentWarning ADD CONSTRAINT CK_StudentWarning_Severity 
  CHECK (Severity BETWEEN 1 AND 3);

CREATE INDEX IX_StudentWarning_Student ON StudentWarning(StudentId, CreatedAtUtc DESC);
CREATE INDEX IX_StudentWarning_Unacknowledged ON StudentWarning(StudentId) 
  WHERE IsAcknowledged = FALSE;
```

### 8.8 AssistantRating

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| StudentId | UUID | FK → StudentProfile, NOT NULL, ON DELETE CASCADE |
| AssistantId | UUID | FK → AssistantProfile, NOT NULL |
| SessionId | UUID | FK → FollowUpSession, NOT NULL |
| Rating | SMALLINT | NOT NULL |
| Comment | VARCHAR(500) | NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
ALTER TABLE AssistantRating 
  ADD CONSTRAINT UQ_AssistantRating UNIQUE (StudentId, SessionId);
ALTER TABLE AssistantRating ADD CONSTRAINT CK_AssistantRating_Rating 
  CHECK (Rating BETWEEN 1 AND 5);

CREATE INDEX IX_AssistantRating_Assistant ON AssistantRating(AssistantId);
```

### 8.9 GroupTransferRequest

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| StudentId | UUID | FK → StudentProfile, NOT NULL, ON DELETE CASCADE |
| CurrentGroupId | UUID | FK → FollowUpGroup, NOT NULL |
| PreferredAssistantId | UUID | FK → AssistantProfile, NULL |
| Reason | VARCHAR(500) | NULL |
| Status | SMALLINT | NOT NULL DEFAULT 0 |
| NewGroupId | UUID | FK → FollowUpGroup, NULL |
| ProcessedByUserId | UUID | FK → User, NULL |
| ProcessedAtUtc | TIMESTAMPTZ | NULL |
| ProcessingNotes | VARCHAR(500) | NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
-- Status: 0=Pending, 1=Approved, 2=Rejected
ALTER TABLE GroupTransferRequest ADD CONSTRAINT CK_GroupTransferRequest_Status 
  CHECK (Status BETWEEN 0 AND 2);

CREATE INDEX IX_GroupTransferRequest_Pending ON GroupTransferRequest(Status, CreatedAtUtc) 
  WHERE Status = 0;
```

---

## 9. Gamification Domain

### 9.1 Level

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| Number | SMALLINT | NOT NULL, UNIQUE |
| Name | VARCHAR(100) | NOT NULL |
| NameEn | VARCHAR(100) | NULL |
| RequiredXp | INT | NOT NULL |
| BadgeImageUrl | VARCHAR(500) | NOT NULL |
| ShortMessage | VARCHAR(200) | NOT NULL |
| LongMessage | TEXT | NOT NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
ALTER TABLE Level ADD CONSTRAINT CK_Level_Number 
  CHECK (Number >= 1);
ALTER TABLE Level ADD CONSTRAINT CK_Level_RequiredXp 
  CHECK (RequiredXp >= 0);

-- Seed data
INSERT INTO Level (Id, Number, Name, NameEn, RequiredXp, BadgeImageUrl, ShortMessage, LongMessage, CreatedAtUtc)
VALUES 
  (uuid_generate_v7(), 1, 'المبتدئ', 'Beginner', 0, '/badges/level1.png', 'مرحباً بك!', 'بداية رحلة التعلم', NOW()),
  (uuid_generate_v7(), 2, 'الملتزم', 'Committed', 500, '/badges/level2.png', 'أحسنت!', 'أثبت التزامك', NOW()),
  (uuid_generate_v7(), 3, 'المجتهد', 'Diligent', 1500, '/badges/level3.png', 'ممتاز!', 'مجهود رائع', NOW()),
  (uuid_generate_v7(), 4, 'الوحش', 'Beast', 3000, '/badges/level4.png', 'رهيب!', 'أداء استثنائي', NOW()),
  (uuid_generate_v7(), 5, 'ملك المنهج', 'Curriculum King', 6000, '/badges/level5.png', 'ملكي!', 'سيطرت على المنهج', NOW()),
  (uuid_generate_v7(), 6, 'الأسطوري', 'Legendary', 9000, '/badges/level6.png', 'أسطورة!', 'مستوى لا يصدق', NOW());
```

### 9.2 LevelBenefit

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| LevelId | UUID | FK → Level, NOT NULL, ON DELETE CASCADE |
| BenefitType | SMALLINT | NOT NULL |
| BenefitValue | VARCHAR(200) | NOT NULL |
| DisplayOrder | SMALLINT | NOT NULL |

```sql
-- BenefitType: 1=Badge, 2=Discount, 3=Feature, 4=ExtraPoints
```

### 9.3 StudentGamification

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| StudentId | UUID | FK → StudentProfile, NOT NULL, UNIQUE, ON DELETE CASCADE |
| TotalXp | INT | NOT NULL DEFAULT 0 |
| CurrentLevelId | UUID | FK → Level, NOT NULL |
| PointsBalance | INT | NOT NULL DEFAULT 0 |
| TotalPointsEarned | INT | NOT NULL DEFAULT 0 |
| TotalPointsSpent | INT | NOT NULL DEFAULT 0 |
| CurrentStreak | SMALLINT | NOT NULL DEFAULT 0 |
| LongestStreak | SMALLINT | NOT NULL DEFAULT 0 |
| LastStreakDateUtc | DATE | NULL |
| StreakFreezeAvailable | SMALLINT | NOT NULL DEFAULT 0 |
| StreakFreezeUsedToday | BOOLEAN | NOT NULL DEFAULT FALSE |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
ALTER TABLE StudentGamification ADD CONSTRAINT CK_StudentGamification_Xp 
  CHECK (TotalXp >= 0);
ALTER TABLE StudentGamification ADD CONSTRAINT CK_StudentGamification_Points 
  CHECK (PointsBalance >= 0 AND TotalPointsEarned >= 0 AND TotalPointsSpent >= 0);
ALTER TABLE StudentGamification ADD CONSTRAINT CK_StudentGamification_Streak 
  CHECK (CurrentStreak >= 0 AND LongestStreak >= CurrentStreak);
```

### 9.4 StreakHistory

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| StudentId | UUID | FK → StudentProfile, NOT NULL, ON DELETE CASCADE |
| Date | DATE | NOT NULL |
| HadPlannedTasks | BOOLEAN | NOT NULL |
| CompletedAllTasks | BOOLEAN | NOT NULL |
| UsedFreeze | BOOLEAN | NOT NULL DEFAULT FALSE |
| StreakCountOnDate | SMALLINT | NOT NULL |
| WasStreakBroken | BOOLEAN | NOT NULL DEFAULT FALSE |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
ALTER TABLE StreakHistory 
  ADD CONSTRAINT UQ_StreakHistory UNIQUE (StudentId, Date);

CREATE INDEX IX_StreakHistory_Student ON StreakHistory(StudentId, Date DESC);
```

### 9.5 XpTransaction (High Volume - BIGINT PK)

| Column | Type | Constraints |
|--------|------|-------------|
| Id | BIGINT | PK, GENERATED ALWAYS AS IDENTITY |
| StudentId | UUID | FK → StudentProfile, NOT NULL, ON DELETE CASCADE |
| Amount | INT | NOT NULL |
| Source | SMALLINT | NOT NULL |
| SourceEntityType | VARCHAR(50) | NULL |
| SourceEntityId | UUID | NULL |
| Description | VARCHAR(300) | NOT NULL |
| XpBefore | INT | NOT NULL |
| XpAfter | INT | NOT NULL |
| LevelUpTriggered | BOOLEAN | NOT NULL DEFAULT FALSE |
| NewLevelId | UUID | FK → Level, NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
-- Source: 1=Lecture, 2=Quiz, 3=Assignment, 4=Challenge, 5=Streak, 6=FollowUp, 
--         7=Achievement, 8=LevelUp, 9=Bonus
ALTER TABLE XpTransaction ADD CONSTRAINT CK_XpTransaction_Source 
  CHECK (Source BETWEEN 1 AND 9);
ALTER TABLE XpTransaction ADD CONSTRAINT CK_XpTransaction_Amount 
  CHECK (Amount > 0);

CREATE INDEX IX_XpTransaction_Student ON XpTransaction(StudentId, CreatedAtUtc DESC);
-- Partition by month for large scale
```

### 9.6 PointsTransaction (High Volume - BIGINT PK)

| Column | Type | Constraints |
|--------|------|-------------|
| Id | BIGINT | PK, GENERATED ALWAYS AS IDENTITY |
| StudentId | UUID | FK → StudentProfile, NOT NULL, ON DELETE CASCADE |
| Amount | INT | NOT NULL |
| TransactionType | SMALLINT | NOT NULL |
| Source | SMALLINT | NOT NULL |
| SourceEntityType | VARCHAR(50) | NULL |
| SourceEntityId | UUID | NULL |
| Description | VARCHAR(300) | NOT NULL |
| BalanceBefore | INT | NOT NULL |
| BalanceAfter | INT | NOT NULL |
| RelatedOrderId | UUID | FK → "Order", NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
-- TransactionType: 1=Earned, 2=Spent, 3=Purchased, 4=Refund, 5=Gift, 6=Promo
ALTER TABLE PointsTransaction ADD CONSTRAINT CK_PointsTransaction_Type 
  CHECK (TransactionType BETWEEN 1 AND 6);

CREATE INDEX IX_PointsTransaction_Student ON PointsTransaction(StudentId, CreatedAtUtc DESC);
```

### 9.7 Achievement

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| Code | VARCHAR(50) | NOT NULL, UNIQUE |
| Name | VARCHAR(100) | NOT NULL |
| NameEn | VARCHAR(100) | NULL |
| Description | VARCHAR(500) | NOT NULL |
| ShortMessage | VARCHAR(200) | NOT NULL |
| LongMessage | TEXT | NOT NULL |
| BadgeImageUrl | VARCHAR(500) | NOT NULL |
| BadgeImageLockedUrl | VARCHAR(500) | NULL |
| XpReward | SMALLINT | NOT NULL DEFAULT 0 |
| PointsReward | SMALLINT | NOT NULL DEFAULT 0 |
| CriteriaType | SMALLINT | NOT NULL |
| CriteriaValue | INT | NULL |
| IsRepeatable | BOOLEAN | NOT NULL DEFAULT FALSE |
| IsActive | BOOLEAN | NOT NULL DEFAULT TRUE |
| DisplayOrder | SMALLINT | NOT NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
-- CriteriaType: 1=Streak, 2=QuizScore, 3=Completion, 4=SameDayCompletion, 
--               5=ReviewOnTime, 6=NoMissedAssignments, 7=TopOfClass, 99=Custom
ALTER TABLE Achievement ADD CONSTRAINT CK_Achievement_CriteriaType 
  CHECK (CriteriaType IN (1, 2, 3, 4, 5, 6, 7, 99));
ALTER TABLE Achievement ADD CONSTRAINT CK_Achievement_Rewards 
  CHECK (XpReward >= 0 AND PointsReward >= 0);
```

### 9.8 AchievementCriteria

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| AchievementId | UUID | FK → Achievement, NOT NULL, ON DELETE CASCADE |
| CriteriaKey | VARCHAR(50) | NOT NULL |
| CriteriaValue | VARCHAR(200) | NOT NULL |

```sql
-- For complex criteria that don't fit in single CriteriaValue
ALTER TABLE AchievementCriteria 
  ADD CONSTRAINT UQ_AchievementCriteria UNIQUE (AchievementId, CriteriaKey);
```

### 9.9 StudentAchievement

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| StudentId | UUID | FK → StudentProfile, NOT NULL, ON DELETE CASCADE |
| AchievementId | UUID | FK → Achievement, NOT NULL |
| EarnedAtUtc | TIMESTAMPTZ | NOT NULL |
| EarnedCount | SMALLINT | NOT NULL DEFAULT 1 |
| CurrentProgress | INT | NULL |
| TargetProgress | INT | NULL |
| IsNotified | BOOLEAN | NOT NULL DEFAULT FALSE |
| NotifiedAtUtc | TIMESTAMPTZ | NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
-- For non-repeatable achievements
CREATE UNIQUE INDEX UQ_StudentAchievement_NonRepeatable 
  ON StudentAchievement(StudentId, AchievementId) 
  WHERE EarnedCount = 1;

CREATE INDEX IX_StudentAchievement_Student ON StudentAchievement(StudentId);
```

### 9.10 Challenge

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| Type | SMALLINT | NOT NULL |
| Title | VARCHAR(200) | NOT NULL |
| TitleEn | VARCHAR(200) | NULL |
| Description | VARCHAR(500) | NOT NULL |
| TargetType | SMALLINT | NOT NULL |
| TargetValue | INT | NOT NULL |
| XpReward | SMALLINT | NOT NULL DEFAULT 0 |
| PointsReward | SMALLINT | NOT NULL DEFAULT 0 |
| BadgeImageUrl | VARCHAR(500) | NULL |
| StartDateUtc | TIMESTAMPTZ | NOT NULL |
| EndDateUtc | TIMESTAMPTZ | NOT NULL |
| IsActive | BOOLEAN | NOT NULL DEFAULT TRUE |
| CreatedByUserId | UUID | FK → User, NOT NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
-- Type: 1=Daily, 2=Weekly, 3=Special
-- TargetType: 1=StudyMinutes, 2=LecturesComplete, 3=QuizzesComplete, 
--             4=QuizScoreAverage, 5=StreakDays, 6=PointsEarned
ALTER TABLE Challenge ADD CONSTRAINT CK_Challenge_Type 
  CHECK (Type BETWEEN 1 AND 3);
ALTER TABLE Challenge ADD CONSTRAINT CK_Challenge_TargetType 
  CHECK (TargetType BETWEEN 1 AND 6);
ALTER TABLE Challenge ADD CONSTRAINT CK_Challenge_Dates 
  CHECK (EndDateUtc > StartDateUtc);

CREATE INDEX IX_Challenge_Active ON Challenge(StartDateUtc, EndDateUtc) 
  WHERE IsActive = TRUE;
```

### 9.11 StudentChallenge

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| StudentId | UUID | FK → StudentProfile, NOT NULL, ON DELETE CASCADE |
| ChallengeId | UUID | FK → Challenge, NOT NULL |
| CurrentProgress | INT | NOT NULL DEFAULT 0 |
| Status | SMALLINT | NOT NULL DEFAULT 0 |
| CompletedAtUtc | TIMESTAMPTZ | NULL |
| XpAwarded | SMALLINT | NOT NULL DEFAULT 0 |
| PointsAwarded | SMALLINT | NOT NULL DEFAULT 0 |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
-- Status: 0=InProgress, 1=Completed, 2=Failed
ALTER TABLE StudentChallenge 
  ADD CONSTRAINT UQ_StudentChallenge UNIQUE (StudentId, ChallengeId);
ALTER TABLE StudentChallenge ADD CONSTRAINT CK_StudentChallenge_Status 
  CHECK (Status BETWEEN 0 AND 2);

CREATE INDEX IX_StudentChallenge_Challenge ON StudentChallenge(ChallengeId, Status);
```

### 9.12 Leaderboard (Cached)

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| Type | SMALLINT | NOT NULL |
| Scope | SMALLINT | NOT NULL |
| ScopeEntityId | UUID | NULL |
| Period | SMALLINT | NOT NULL |
| TotalParticipants | INT | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
-- Type: 1=XP, 2=Streak, 3=CourseProgress
-- Scope: 1=Global, 2=Course, 3=StudyLevel
-- Period: 1=Weekly, 2=Monthly, 3=AllTime
ALTER TABLE Leaderboard 
  ADD CONSTRAINT UQ_Leaderboard UNIQUE (Type, Scope, ScopeEntityId, Period);
```

### 9.13 LeaderboardEntry

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| LeaderboardId | UUID | FK → Leaderboard, NOT NULL, ON DELETE CASCADE |
| StudentId | UUID | FK → StudentProfile, NOT NULL, ON DELETE CASCADE |
| Rank | INT | NOT NULL |
| Score | INT | NOT NULL |
| DisplayRank | VARCHAR(50) | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
ALTER TABLE LeaderboardEntry 
  ADD CONSTRAINT UQ_LeaderboardEntry UNIQUE (LeaderboardId, StudentId);
ALTER TABLE LeaderboardEntry ADD CONSTRAINT CK_LeaderboardEntry_Rank 
  CHECK (Rank >= 1);

CREATE INDEX IX_LeaderboardEntry_Leaderboard ON LeaderboardEntry(LeaderboardId, Rank);
CREATE INDEX IX_LeaderboardEntry_Student ON LeaderboardEntry(StudentId);
```

---

## 10. Purchasing Domain

### 10.1 AcademicYear

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| Year | VARCHAR(9) | NOT NULL, UNIQUE |
| StartDate | DATE | NOT NULL |
| EndDate | DATE | NOT NULL |
| IsCurrent | BOOLEAN | NOT NULL DEFAULT FALSE |
| IsActive | BOOLEAN | NOT NULL DEFAULT TRUE |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
ALTER TABLE AcademicYear ADD CONSTRAINT CK_AcademicYear_Dates 
  CHECK (EndDate > StartDate);

-- Only one current year
CREATE UNIQUE INDEX UQ_AcademicYear_Current ON AcademicYear(IsCurrent) 
  WHERE IsCurrent = TRUE;
```

### 10.2 Product

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| ProductType | SMALLINT | NOT NULL |
| ModuleId | UUID | FK → Module, NULL |
| ContentItemId | UUID | FK → ContentItem, NULL |
| PointsPackageId | UUID | FK → PointsPackage, NULL |
| Name | VARCHAR(300) | NOT NULL |
| Description | TEXT | NULL |
| ThumbnailUrl | VARCHAR(500) | NULL |
| PriceEgp | DECIMAL(10,2) | NOT NULL |
| PointsPrice | INT | NOT NULL |
| AllowPointsPurchase | BOOLEAN | NOT NULL DEFAULT TRUE |
| DiscountedPriceEgp | DECIMAL(10,2) | NULL |
| DiscountStartUtc | TIMESTAMPTZ | NULL |
| DiscountEndUtc | TIMESTAMPTZ | NULL |
| ParentProductId | UUID | FK → Product, NULL |
| IsActive | BOOLEAN | NOT NULL DEFAULT TRUE |
| DisplayOrder | SMALLINT | NOT NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
-- ProductType: 1=Course, 2=Module, 3=ModuleFollowUp, 4=Content, 5=PointsPackage
ALTER TABLE Product ADD CONSTRAINT CK_Product_Type 
  CHECK (ProductType BETWEEN 1 AND 5);
ALTER TABLE Product ADD CONSTRAINT CK_Product_Price 
  CHECK (PriceEgp >= 0 AND PointsPrice >= 0);
ALTER TABLE Product ADD CONSTRAINT CK_Product_Discount 
  CHECK (DiscountedPriceEgp IS NULL OR DiscountedPriceEgp < PriceEgp);

-- Exactly one reference must be set based on ProductType
ALTER TABLE Product ADD CONSTRAINT CK_Product_Reference 
  CHECK (
    (ProductType IN (1, 2, 3) AND ModuleId IS NOT NULL AND ContentItemId IS NULL AND PointsPackageId IS NULL) OR
    (ProductType = 4 AND ContentItemId IS NOT NULL AND ModuleId IS NULL AND PointsPackageId IS NULL) OR
    (ProductType = 5 AND PointsPackageId IS NOT NULL AND ModuleId IS NULL AND ContentItemId IS NULL)
  );

CREATE INDEX IX_Product_Type ON Product(ProductType) WHERE IsActive = TRUE;
CREATE INDEX IX_Product_Module ON Product(ModuleId) WHERE ModuleId IS NOT NULL;
```

### 10.3 PointsPackage

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| Name | VARCHAR(100) | NOT NULL |
| PointsAmount | INT | NOT NULL |
| BonusPoints | INT | NOT NULL DEFAULT 0 |
| TotalPoints | INT | GENERATED ALWAYS AS (PointsAmount + BonusPoints) STORED |
| PriceEgp | DECIMAL(10,2) | NOT NULL |
| IsActive | BOOLEAN | NOT NULL DEFAULT TRUE |
| DisplayOrder | SMALLINT | NOT NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
ALTER TABLE PointsPackage ADD CONSTRAINT CK_PointsPackage_Points 
  CHECK (PointsAmount > 0 AND BonusPoints >= 0);
ALTER TABLE PointsPackage ADD CONSTRAINT CK_PointsPackage_Price 
  CHECK (PriceEgp > 0);
```

### 10.4 Cart

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| StudentId | UUID | FK → StudentProfile, NOT NULL, UNIQUE, ON DELETE CASCADE |
| ItemCount | SMALLINT | NOT NULL DEFAULT 0 |
| SubTotalEgp | DECIMAL(10,2) | NOT NULL DEFAULT 0 |
| LastActivityAtUtc | TIMESTAMPTZ | NOT NULL |
| LastReminderSentAtUtc | TIMESTAMPTZ | NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

### 10.5 CartItem

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| CartId | UUID | FK → Cart, NOT NULL, ON DELETE CASCADE |
| ProductId | UUID | FK → Product, NOT NULL |
| Quantity | SMALLINT | NOT NULL DEFAULT 1 |
| UnitPriceEgp | DECIMAL(10,2) | NOT NULL |
| AddedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
ALTER TABLE CartItem 
  ADD CONSTRAINT UQ_CartItem UNIQUE (CartId, ProductId);
ALTER TABLE CartItem ADD CONSTRAINT CK_CartItem_Quantity 
  CHECK (Quantity >= 1);
```

### 10.6 PromoCode

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| Code | VARCHAR(50) | NOT NULL, UNIQUE |
| DiscountType | SMALLINT | NOT NULL |
| DiscountValue | DECIMAL(10,2) | NOT NULL |
| MaxTotalUses | INT | NULL |
| CurrentTotalUses | INT | NOT NULL DEFAULT 0 |
| MaxUsesPerUser | SMALLINT | NOT NULL DEFAULT 1 |
| MinOrderAmountEgp | DECIMAL(10,2) | NULL |
| ValidFromUtc | TIMESTAMPTZ | NOT NULL |
| ValidUntilUtc | TIMESTAMPTZ | NOT NULL |
| IsActive | BOOLEAN | NOT NULL DEFAULT TRUE |
| CreatedByUserId | UUID | FK → User, NOT NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
-- DiscountType: 1=Percentage, 2=FixedAmount, 3=FreePoints
ALTER TABLE PromoCode ADD CONSTRAINT CK_PromoCode_DiscountType 
  CHECK (DiscountType BETWEEN 1 AND 3);
ALTER TABLE PromoCode ADD CONSTRAINT CK_PromoCode_DiscountValue 
  CHECK (DiscountValue > 0);
ALTER TABLE PromoCode ADD CONSTRAINT CK_PromoCode_Percentage 
  CHECK (DiscountType != 1 OR DiscountValue <= 100);
ALTER TABLE PromoCode ADD CONSTRAINT CK_PromoCode_Dates 
  CHECK (ValidUntilUtc > ValidFromUtc);

CREATE INDEX IX_PromoCode_Code ON PromoCode(Code) WHERE IsActive = TRUE;
CREATE INDEX IX_PromoCode_Valid ON PromoCode(ValidFromUtc, ValidUntilUtc) WHERE IsActive = TRUE;
```

### 10.7 PromoCodeProduct

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| PromoCodeId | UUID | FK → PromoCode, NOT NULL, ON DELETE CASCADE |
| ProductId | UUID | FK → Product, NULL |
| ProductType | SMALLINT | NULL |
| IsExcluded | BOOLEAN | NOT NULL DEFAULT FALSE |

```sql
-- Either ProductId or ProductType must be set
ALTER TABLE PromoCodeProduct ADD CONSTRAINT CK_PromoCodeProduct_Target 
  CHECK ((ProductId IS NOT NULL) != (ProductType IS NOT NULL));
```

### 10.8 PromoCodeUsage

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| PromoCodeId | UUID | FK → PromoCode, NOT NULL |
| StudentId | UUID | FK → StudentProfile, NOT NULL, ON DELETE CASCADE |
| OrderId | UUID | FK → "Order", NOT NULL |
| DiscountAppliedEgp | DECIMAL(10,2) | NOT NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
CREATE INDEX IX_PromoCodeUsage_Code_Student ON PromoCodeUsage(PromoCodeId, StudentId);
```

### 10.9 Order

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| OrderNumber | VARCHAR(20) | NOT NULL, UNIQUE |
| StudentId | UUID | FK → StudentProfile, NOT NULL |
| Status | SMALLINT | NOT NULL DEFAULT 0 |
| SubTotalEgp | DECIMAL(10,2) | NOT NULL |
| DiscountAmountEgp | DECIMAL(10,2) | NOT NULL DEFAULT 0 |
| PointsUsed | INT | NOT NULL DEFAULT 0 |
| PointsValueEgp | DECIMAL(10,2) | NOT NULL DEFAULT 0 |
| TotalAmountEgp | DECIMAL(10,2) | NOT NULL |
| TotalAmountPaidEgp | DECIMAL(10,2) | NOT NULL |
| PromoCodeId | UUID | FK → PromoCode, NULL |
| PaymentMethod | SMALLINT | NULL |
| PaymobOrderId | VARCHAR(100) | NULL |
| PaymobTransactionId | VARCHAR(100) | NULL |
| PaidAtUtc | TIMESTAMPTZ | NULL |
| ExpiresAtUtc | TIMESTAMPTZ | NULL |
| Notes | VARCHAR(500) | NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
-- Status: 0=Pending, 1=Processing, 2=Paid, 3=Failed, 4=Refunded, 5=PartiallyRefunded, 6=Cancelled, 7=Expired
-- PaymentMethod: 1=Card, 2=MobileWallet, 3=Fawry, 4=PointsOnly, 5=Mixed
ALTER TABLE "Order" ADD CONSTRAINT CK_Order_Status 
  CHECK (Status BETWEEN 0 AND 7);
ALTER TABLE "Order" ADD CONSTRAINT CK_Order_PaymentMethod 
  CHECK (PaymentMethod IS NULL OR PaymentMethod BETWEEN 1 AND 5);
ALTER TABLE "Order" ADD CONSTRAINT CK_Order_Amounts 
  CHECK (SubTotalEgp >= 0 AND DiscountAmountEgp >= 0 AND TotalAmountEgp >= 0 AND TotalAmountPaidEgp >= 0);

CREATE INDEX IX_Order_Student ON "Order"(StudentId, CreatedAtUtc DESC);
CREATE INDEX IX_Order_Number ON "Order"(OrderNumber);
CREATE INDEX IX_Order_Paymob ON "Order"(PaymobTransactionId) WHERE PaymobTransactionId IS NOT NULL;
CREATE INDEX IX_Order_Pending ON "Order"(ExpiresAtUtc) WHERE Status = 0;
```

### 10.10 OrderItem

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| OrderId | UUID | FK → "Order", NOT NULL, ON DELETE CASCADE |
| ProductId | UUID | FK → Product, NOT NULL |
| Quantity | SMALLINT | NOT NULL DEFAULT 1 |
| UnitPriceEgp | DECIMAL(10,2) | NOT NULL |
| DiscountAmountEgp | DECIMAL(10,2) | NOT NULL DEFAULT 0 |
| TotalEgp | DECIMAL(10,2) | NOT NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
CREATE INDEX IX_OrderItem_Order ON OrderItem(OrderId);
```

### 10.11 OrderItemSnapshot

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| OrderItemId | UUID | FK → OrderItem, NOT NULL, UNIQUE, ON DELETE CASCADE |
| ProductName | VARCHAR(300) | NOT NULL |
| ProductType | SMALLINT | NOT NULL |
| ProductDescription | TEXT | NULL |
| OriginalPriceEgp | DECIMAL(10,2) | NOT NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |

### 10.12 StudentEnrollment

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| StudentId | UUID | FK → StudentProfile, NOT NULL, ON DELETE CASCADE |
| CourseId | UUID | FK → Course, NOT NULL |
| ModuleId | UUID | FK → Module, NULL |
| EnrollmentType | SMALLINT | NOT NULL |
| OrderId | UUID | FK → "Order", NOT NULL |
| OrderItemId | UUID | FK → OrderItem, NOT NULL |
| HasFollowUp | BOOLEAN | NOT NULL DEFAULT FALSE |
| FollowUpOrderItemId | UUID | FK → OrderItem, NULL |
| AcademicYearId | UUID | FK → AcademicYear, NOT NULL |
| StartDateUtc | TIMESTAMPTZ | NOT NULL |
| ExpiryDateUtc | TIMESTAMPTZ | NOT NULL |
| Status | SMALLINT | NOT NULL DEFAULT 0 |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
-- EnrollmentType: 1=FullCourse, 2=Module, 3=ModuleWithFollowUp
-- Status: 0=Active, 1=Expired, 2=Cancelled, 3=Suspended
ALTER TABLE StudentEnrollment ADD CONSTRAINT CK_StudentEnrollment_Type 
  CHECK (EnrollmentType BETWEEN 1 AND 3);
ALTER TABLE StudentEnrollment ADD CONSTRAINT CK_StudentEnrollment_Status 
  CHECK (Status BETWEEN 0 AND 3);

CREATE UNIQUE INDEX UQ_StudentEnrollment_Module 
  ON StudentEnrollment(StudentId, ModuleId, AcademicYearId) 
  WHERE ModuleId IS NOT NULL;

CREATE INDEX IX_StudentEnrollment_Student ON StudentEnrollment(StudentId, Status);
CREATE INDEX IX_StudentEnrollment_Course ON StudentEnrollment(CourseId);
CREATE INDEX IX_StudentEnrollment_Expiry ON StudentEnrollment(ExpiryDateUtc) WHERE Status = 0;
```

### 10.13 RefundRequest

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| OrderId | UUID | FK → "Order", NOT NULL |
| StudentId | UUID | FK → StudentProfile, NOT NULL, ON DELETE CASCADE |
| RequestedAmountEgp | DECIMAL(10,2) | NOT NULL |
| Reason | TEXT | NOT NULL |
| Status | SMALLINT | NOT NULL DEFAULT 0 |
| ProcessedByUserId | UUID | FK → User, NULL |
| ProcessedAtUtc | TIMESTAMPTZ | NULL |
| ProcessingNotes | TEXT | NULL |
| RefundedAmountEgp | DECIMAL(10,2) | NULL |
| RefundMethod | SMALLINT | NULL |
| PointsRefunded | INT | NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
-- Status: 0=Pending, 1=Approved, 2=Rejected, 3=Processed
-- RefundMethod: 1=OriginalPayment, 2=Points, 3=Manual
ALTER TABLE RefundRequest ADD CONSTRAINT CK_RefundRequest_Status 
  CHECK (Status BETWEEN 0 AND 3);
ALTER TABLE RefundRequest ADD CONSTRAINT CK_RefundRequest_Method 
  CHECK (RefundMethod IS NULL OR RefundMethod BETWEEN 1 AND 3);

CREATE INDEX IX_RefundRequest_Pending ON RefundRequest(Status, CreatedAtUtc) WHERE Status = 0;
```

### 10.14 PaymentWebhookLog

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| Provider | VARCHAR(50) | NOT NULL |
| EventType | VARCHAR(100) | NOT NULL |
| TransactionId | VARCHAR(100) | NULL |
| ExternalOrderId | VARCHAR(100) | NULL |
| OrderId | UUID | FK → "Order", NULL |
| RawPayload | TEXT | NOT NULL |
| IsProcessed | BOOLEAN | NOT NULL DEFAULT FALSE |
| ProcessingError | TEXT | NULL |
| ProcessedAtUtc | TIMESTAMPTZ | NULL |
| ReceivedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
CREATE INDEX IX_PaymentWebhookLog_Transaction ON PaymentWebhookLog(TransactionId);
CREATE INDEX IX_PaymentWebhookLog_Received ON PaymentWebhookLog(ReceivedAtUtc DESC);
CREATE INDEX IX_PaymentWebhookLog_Unprocessed ON PaymentWebhookLog(ReceivedAtUtc) 
  WHERE IsProcessed = FALSE;
```

---

## 11. Notifications Domain

### 11.1 NotificationTemplate

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| Code | VARCHAR(50) | NOT NULL, UNIQUE |
| Name | VARCHAR(100) | NOT NULL |
| Type | SMALLINT | NOT NULL |
| TitleTemplate | VARCHAR(200) | NOT NULL |
| BodyTemplate | TEXT | NOT NULL |
| TitleTemplateEn | VARCHAR(200) | NULL |
| BodyTemplateEn | TEXT | NULL |
| PushEnabled | BOOLEAN | NOT NULL DEFAULT TRUE |
| EmailEnabled | BOOLEAN | NOT NULL DEFAULT FALSE |
| SmsEnabled | BOOLEAN | NOT NULL DEFAULT FALSE |
| InAppEnabled | BOOLEAN | NOT NULL DEFAULT TRUE |
| Priority | SMALLINT | NOT NULL DEFAULT 2 |
| IconUrl | VARCHAR(500) | NULL |
| ActionUrl | VARCHAR(500) | NULL |
| IsActive | BOOLEAN | NOT NULL DEFAULT TRUE |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
-- Priority: 1=Low, 2=Normal, 3=High
ALTER TABLE NotificationTemplate ADD CONSTRAINT CK_NotificationTemplate_Priority 
  CHECK (Priority BETWEEN 1 AND 3);
```

### 11.2 Notification

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| UserId | UUID | FK → User, NOT NULL, ON DELETE CASCADE |
| TemplateId | UUID | FK → NotificationTemplate, NULL |
| Type | SMALLINT | NOT NULL |
| Title | VARCHAR(200) | NOT NULL |
| Body | TEXT | NOT NULL |
| ActionUrl | VARCHAR(500) | NULL |
| ImageUrl | VARCHAR(500) | NULL |
| Priority | SMALLINT | NOT NULL DEFAULT 2 |
| IsRead | BOOLEAN | NOT NULL DEFAULT FALSE |
| ReadAtUtc | TIMESTAMPTZ | NULL |
| ScheduledAtUtc | TIMESTAMPTZ | NULL |
| SentAtUtc | TIMESTAMPTZ | NULL |
| ExpiresAtUtc | TIMESTAMPTZ | NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
CREATE INDEX IX_Notification_User ON Notification(UserId, IsRead, CreatedAtUtc DESC);
CREATE INDEX IX_Notification_Scheduled ON Notification(ScheduledAtUtc) 
  WHERE ScheduledAtUtc IS NOT NULL AND SentAtUtc IS NULL;
```

### 11.3 NotificationData

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| NotificationId | UUID | FK → Notification, NOT NULL, ON DELETE CASCADE |
| DataKey | VARCHAR(50) | NOT NULL |
| DataValue | VARCHAR(500) | NOT NULL |

```sql
ALTER TABLE NotificationData 
  ADD CONSTRAINT UQ_NotificationData UNIQUE (NotificationId, DataKey);
```

### 11.4 NotificationDelivery (High Volume - BIGINT PK)

| Column | Type | Constraints |
|--------|------|-------------|
| Id | BIGINT | PK, GENERATED ALWAYS AS IDENTITY |
| NotificationId | UUID | FK → Notification, NOT NULL, ON DELETE CASCADE |
| Channel | SMALLINT | NOT NULL |
| Status | SMALLINT | NOT NULL DEFAULT 0 |
| ExternalId | VARCHAR(200) | NULL |
| ErrorMessage | VARCHAR(500) | NULL |
| SentAtUtc | TIMESTAMPTZ | NULL |
| DeliveredAtUtc | TIMESTAMPTZ | NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
-- Channel: 1=Push, 2=Email, 3=Sms, 4=InApp
-- Status: 0=Pending, 1=Sent, 2=Delivered, 3=Failed
ALTER TABLE NotificationDelivery ADD CONSTRAINT CK_NotificationDelivery_Channel 
  CHECK (Channel BETWEEN 1 AND 4);
ALTER TABLE NotificationDelivery ADD CONSTRAINT CK_NotificationDelivery_Status 
  CHECK (Status BETWEEN 0 AND 3);

CREATE INDEX IX_NotificationDelivery_Notification ON NotificationDelivery(NotificationId);
CREATE INDEX IX_NotificationDelivery_Pending ON NotificationDelivery(CreatedAtUtc) 
  WHERE Status = 0;
```

### 11.5 UserNotificationSettings

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| UserId | UUID | FK → User, NOT NULL, UNIQUE, ON DELETE CASCADE |
| PushEnabled | BOOLEAN | NOT NULL DEFAULT TRUE |
| EmailEnabled | BOOLEAN | NOT NULL DEFAULT TRUE |
| SmsEnabled | BOOLEAN | NOT NULL DEFAULT FALSE |
| DigestEmailEnabled | BOOLEAN | NOT NULL DEFAULT TRUE |
| DigestEmailTime | TIME | NOT NULL DEFAULT '08:00' |
| QuietHoursEnabled | BOOLEAN | NOT NULL DEFAULT FALSE |
| QuietHoursStart | TIME | NULL |
| QuietHoursEnd | TIME | NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

### 11.6 UserNotificationDisabled

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| SettingsId | UUID | FK → UserNotificationSettings, NOT NULL, ON DELETE CASCADE |
| NotificationType | SMALLINT | NOT NULL |

```sql
ALTER TABLE UserNotificationDisabled 
  ADD CONSTRAINT UQ_UserNotificationDisabled UNIQUE (SettingsId, NotificationType);
```

### 11.7 UserDevice

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| UserId | UUID | FK → User, NOT NULL, ON DELETE CASCADE |
| DeviceToken | VARCHAR(500) | NOT NULL |
| Platform | SMALLINT | NOT NULL |
| Provider | VARCHAR(50) | NOT NULL |
| ExternalPlayerId | VARCHAR(200) | NULL |
| IsActive | BOOLEAN | NOT NULL DEFAULT TRUE |
| LastActiveAtUtc | TIMESTAMPTZ | NOT NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
CREATE INDEX IX_UserDevice_User ON UserDevice(UserId) WHERE IsActive = TRUE;
CREATE INDEX IX_UserDevice_Token ON UserDevice(DeviceToken);
```

---

## 12. Support & Analytics Domain

### 12.1 SupportTicket

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| TicketNumber | VARCHAR(20) | NOT NULL, UNIQUE |
| UserId | UUID | FK → User, NOT NULL |
| Category | SMALLINT | NOT NULL |
| Subject | VARCHAR(200) | NOT NULL |
| Description | TEXT | NOT NULL |
| Priority | SMALLINT | NOT NULL DEFAULT 2 |
| Status | SMALLINT | NOT NULL DEFAULT 0 |
| AssignedToUserId | UUID | FK → User, NULL |
| RelatedEntityType | VARCHAR(50) | NULL |
| RelatedEntityId | UUID | NULL |
| ResolvedAtUtc | TIMESTAMPTZ | NULL |
| ResolutionNotes | TEXT | NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
-- Category: 1=Technical, 2=Payment, 3=Content, 4=Account, 5=FollowUp, 6=Refund, 99=Other
-- Priority: 1=Low, 2=Normal, 3=High, 4=Urgent
-- Status: 0=Open, 1=InProgress, 2=WaitingOnCustomer, 3=WaitingOnThirdParty, 4=Resolved, 5=Closed, 6=Reopened
ALTER TABLE SupportTicket ADD CONSTRAINT CK_SupportTicket_Category 
  CHECK (Category IN (1, 2, 3, 4, 5, 6, 99));
ALTER TABLE SupportTicket ADD CONSTRAINT CK_SupportTicket_Priority 
  CHECK (Priority BETWEEN 1 AND 4);
ALTER TABLE SupportTicket ADD CONSTRAINT CK_SupportTicket_Status 
  CHECK (Status BETWEEN 0 AND 6);

CREATE INDEX IX_SupportTicket_User ON SupportTicket(UserId, CreatedAtUtc DESC);
CREATE INDEX IX_SupportTicket_Open ON SupportTicket(Status, Priority DESC, CreatedAtUtc) 
  WHERE Status IN (0, 1, 6);
CREATE INDEX IX_SupportTicket_Assigned ON SupportTicket(AssignedToUserId, Status) 
  WHERE AssignedToUserId IS NOT NULL;
```

### 12.2 TicketMessage

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| TicketId | UUID | FK → SupportTicket, NOT NULL, ON DELETE CASCADE |
| SenderUserId | UUID | FK → User, NOT NULL |
| Message | TEXT | NOT NULL |
| AttachmentUrl | VARCHAR(500) | NULL |
| AttachmentName | VARCHAR(200) | NULL |
| IsInternal | BOOLEAN | NOT NULL DEFAULT FALSE |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
CREATE INDEX IX_TicketMessage_Ticket ON TicketMessage(TicketId, CreatedAtUtc);
```

### 12.3 TicketStatusHistory

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| TicketId | UUID | FK → SupportTicket, NOT NULL, ON DELETE CASCADE |
| OldStatus | SMALLINT | NULL |
| NewStatus | SMALLINT | NOT NULL |
| ChangedByUserId | UUID | FK → User, NOT NULL |
| Notes | VARCHAR(500) | NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |

### 12.4 FaqCategory

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| Name | VARCHAR(100) | NOT NULL |
| NameEn | VARCHAR(100) | NULL |
| IconUrl | VARCHAR(500) | NULL |
| DisplayOrder | SMALLINT | NOT NULL |
| IsActive | BOOLEAN | NOT NULL DEFAULT TRUE |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |

### 12.5 FaqItem

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| CategoryId | UUID | FK → FaqCategory, NOT NULL, ON DELETE CASCADE |
| Question | VARCHAR(500) | NOT NULL |
| QuestionEn | VARCHAR(500) | NULL |
| Answer | TEXT | NOT NULL |
| AnswerEn | TEXT | NULL |
| ViewCount | INT | NOT NULL DEFAULT 0 |
| HelpfulCount | INT | NOT NULL DEFAULT 0 |
| NotHelpfulCount | INT | NOT NULL DEFAULT 0 |
| DisplayOrder | SMALLINT | NOT NULL |
| IsActive | BOOLEAN | NOT NULL DEFAULT TRUE |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |
| UpdatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
CREATE INDEX IX_FaqItem_Category ON FaqItem(CategoryId, DisplayOrder) WHERE IsActive = TRUE;
```

### 12.6 DailyAnalytics

| Column | Type | Constraints |
|--------|------|-------------|
| Id | UUID | PK |
| Date | DATE | NOT NULL |
| EntityType | VARCHAR(50) | NOT NULL |
| EntityId | UUID | NULL |
| MetricType | VARCHAR(50) | NOT NULL |
| Value | DECIMAL(18,2) | NOT NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
ALTER TABLE DailyAnalytics 
  ADD CONSTRAINT UQ_DailyAnalytics UNIQUE (Date, EntityType, EntityId, MetricType);

CREATE INDEX IX_DailyAnalytics_Date ON DailyAnalytics(Date DESC, MetricType);
CREATE INDEX IX_DailyAnalytics_Entity ON DailyAnalytics(EntityType, EntityId, Date DESC);
```

### 12.7 UserActivity (High Volume - BIGINT PK)

| Column | Type | Constraints |
|--------|------|-------------|
| Id | BIGINT | PK, GENERATED ALWAYS AS IDENTITY |
| UserId | UUID | FK → User, NOT NULL, ON DELETE CASCADE |
| ActivityType | SMALLINT | NOT NULL |
| EntityType | VARCHAR(50) | NULL |
| EntityId | UUID | NULL |
| IpAddress | VARCHAR(45) | NULL |
| UserAgent | VARCHAR(500) | NULL |
| CreatedAtUtc | TIMESTAMPTZ | NOT NULL |

```sql
-- ActivityType: 1=Login, 2=Logout, 3=PageView, 4=VideoStart, 5=VideoComplete, 
--               6=QuizStart, 7=QuizSubmit, 8=FileDownload, 9=Purchase, 10=Search
ALTER TABLE UserActivity ADD CONSTRAINT CK_UserActivity_Type 
  CHECK (ActivityType BETWEEN 1 AND 10);

CREATE INDEX IX_UserActivity_User ON UserActivity(UserId, CreatedAtUtc DESC);
CREATE INDEX IX_UserActivity_Type ON UserActivity(ActivityType, CreatedAtUtc DESC);

-- Partition by month for performance
-- CREATE TABLE UserActivity (
--   ...
-- ) PARTITION BY RANGE (CreatedAtUtc);
```

---

## 13. Configuration

```json
{
  "LmsSettings": {
    "Authentication": {
      "OtpExpiryMinutes": 10,
      "OtpMaxAttempts": 3,
      "OtpCooldownMinutes": 2,
      "OtpLength": 6,
      "DeviceLimit": 1,
      "JwtExpiryMinutes": 60,
      "RefreshTokenExpiryDays": 30,
      "PasswordMinLength": 8,
      "RequirePhoneVerification": true
    },
    
    "Assessment": {
      "QuizMaxRetakes": 5,
      "QuizCooldownMinutes": 5,
      "TempAnswerTtlHours": 1,
      "AutoSubmitGracePeriodMinutes": 2,
      "LateSubmissionXpMultiplier": 0.0,
      "LateSubmissionPointsMultiplier": 0.0,
      "DefaultQuizDurationMinutes": 30,
      "DefaultAssignmentDurationDays": 3,
      "ShuffleQuestionsDefault": true,
      "ShuffleOptionsDefault": true
    },
    
    "Gamification": {
      "StreakResetHourUtc": 3,
      "DefaultXpPerLecture": 10,
      "DefaultXpPerQuiz": 20,
      "DefaultXpPerAssignment": 30,
      "DefaultPointsPerLecture": 5,
      "DefaultPointsPerQuiz": 10,
      "DefaultPointsPerAssignment": 15,
      "StreakBonusXpPerDay": 5,
      "StreakBonusPointsPerDay": 2,
      "LeaderboardUpdateIntervalMinutes": 15,
      "LeaderboardTop100ShowRank": true,
      "LeaderboardShowPercentageAfterRank": 500,
      "StreakFreezeRules": [
        { "MinStreak": 1, "MaxStreak": 7, "Freezes": 0 },
        { "MinStreak": 8, "MaxStreak": 14, "Freezes": 1 },
        { "MinStreak": 15, "MaxStreak": 21, "Freezes": 2 },
        { "MinStreak": 22, "MaxStreak": 30, "Freezes": 3 },
        { "MinStreak": 31, "MaxStreak": 999, "Freezes": 4 }
      ]
    },
    
    "StudyPlan": {
      "DefaultDailyMinutes": 120,
      "MinDailyMinutes": 30,
      "MaxDailyMinutes": 480,
      "MaxReschedulesPerWeek": 3,
      "AutoAdjustOnMissedDays": true,
      "AutoAdjustOnLowScore": true,
      "LowScoreThresholdPercent": 60,
      "ReviewDayIntervalDays": 5,
      "ReviewQuizQuestionsCount": 10,
      "ModuleDurationDays": 30,
      "CrampTasksIfLatePurchase": true
    },
    
    "FollowUp": {
      "MaxCallAttempts": 3,
      "CallAttemptIntervalHours": 24,
      "SessionDefaultDurationMinutes": 15,
      "DefaultGroupMaxStudents": 30,
      "StageUnlockMinScore": 3.0,
      "AutoAssignToLastAssistant": true,
      "EnableWaitingList": true,
      "EnableAssistantRating": true,
      "EnableGroupTransferRequest": true
    },
    
    "Purchasing": {
      "PointsPerEgp": 10,
      "MinPointsForPurchase": 100,
      "MaxPointsPercentagePerOrder": 50,
      "PendingOrderExpiryMinutes": 30,
      "EnablePromoCode": true,
      "EnableRefunds": true,
      "RefundWindowDays": 7
    },
    
    "Cart": {
      "ItemExpirationDays": 30,
      "AbandonmentReminderIntervalDays": 7,
      "MaxReminderCount": 4,
      "MaxItemsPerCart": 20
    },
    
    "Notifications": {
      "EnablePush": true,
      "EnableEmail": true,
      "EnableSms": false,
      "PushProvider": "onesignal",
      "EmailProvider": "ses",
      "SmsProvider": "twilio",
      "DigestEmailHourUtc": 8,
      "MaxNotificationsPerDay": 20
    },
    
    "Cache": {
      "DefaultExpiryMinutes": 60,
      "LeaderboardExpiryMinutes": 15,
      "StudentGamificationExpiryMinutes": 5,
      "CourseListExpiryMinutes": 30
    }
  }
}
```

---

## 14. Redis Strategy

### 14.1 Authentication & Security

| Key Pattern | Purpose | TTL |
|-------------|---------|-----|
| `otp:{phone}:{purpose}` | OTP verification | 10 min |
| `otp:attempts:{phone}` | OTP attempt counter | 10 min |
| `session:{userId}:{deviceId}` | Active session | 24 hours |
| `blacklist:token:{jti}` | Revoked tokens | Token expiry |
| `ratelimit:{ip}:{endpoint}` | Rate limiting | 1 min |

### 14.2 Assessment

| Key Pattern | Purpose | TTL |
|-------------|---------|-----|
| `attempt:{attemptId}:answers` | Temp exam answers | 1hr + duration |
| `attempt:{attemptId}:meta` | Attempt metadata | 1hr + duration |
| `cooldown:quiz:{studentId}:{assessmentId}` | Retake cooldown | 5 min |

### 14.3 Gamification

| Key Pattern | Purpose | TTL |
|-------------|---------|-----|
| `student:{id}:gamification` | XP, Points, Streak | 5 min |
| `leaderboard:{type}:{scope}:{period}` | Cached rankings | 15 min |
| `streak:check:{studentId}:{date}` | Streak calculation lock | 1 day |
| `achievement:check:{studentId}:{code}` | Prevent duplicate awards | 1 hour |

### 14.4 Content & Progress

| Key Pattern | Purpose | TTL |
|-------------|---------|-----|
| `course:{id}:details` | Course info | 30 min |
| `module:{id}:content` | Module content list | 30 min |
| `student:{id}:progress:{courseId}` | Progress summary | 5 min |
| `video:playback:{studentId}:{contentId}` | Resume position | 24 hours |

---

## 15. Data Retention

### 15.1 Retention Policies

| Data Type | Active | Archive | Delete |
|-----------|--------|---------|--------|
| User accounts | Indefinite | - | On request |
| Content progress | Indefinite | - | With user |
| XP/Points transactions | 2 years | 5 years | After archive |
| User activity | 90 days | 2 years | After archive |
| Notification delivery | 30 days | 1 year | After archive |
| Support tickets | 2 years | 5 years | After archive |
| Payment webhooks | 90 days | 7 years | After archive |
| Study plan history | 1 year | 3 years | After archive |

### 15.2 Archive Strategy

```sql
-- Monthly archival job for UserActivity
CREATE TABLE UserActivity_Archive (
  LIKE UserActivity INCLUDING ALL
) PARTITION BY RANGE (CreatedAtUtc);

-- Move old data
INSERT INTO UserActivity_Archive
SELECT * FROM UserActivity
WHERE CreatedAtUtc < NOW() - INTERVAL '90 days';

DELETE FROM UserActivity
WHERE CreatedAtUtc < NOW() - INTERVAL '90 days';
```

### 15.3 Aggregation Jobs

```sql
-- Daily aggregation for analytics
INSERT INTO DailyAnalytics (Id, Date, EntityType, EntityId, MetricType, Value, CreatedAtUtc)
SELECT 
  uuid_generate_v7(),
  CURRENT_DATE - 1,
  'global',
  NULL,
  'ActiveStudents',
  COUNT(DISTINCT UserId),
  NOW()
FROM UserActivity
WHERE CreatedAtUtc >= CURRENT_DATE - 1
  AND CreatedAtUtc < CURRENT_DATE;
```

---

## 16. Enums Reference

### 16.1 User Domain

```csharp
public enum UserType : byte
{
    Student = 1,
    Teacher = 2,
    Assistant = 3,
    Parent = 4,
    Admin = 5
}

public enum ParentRelation : byte
{
    Father = 1,
    Mother = 2,
    Guardian = 3
}

public enum DevicePlatform : byte
{
    Web = 1,
    iOS = 2,
    Android = 3
}

public enum SocialPlatform : byte
{
    Facebook = 1,
    YouTube = 2,
    Instagram = 3,
    Twitter = 4,
    LinkedIn = 5,
    TikTok = 6,
    Website = 7
}
```

### 16.2 Content Domain

```csharp
public enum Visibility : byte
{
    Hidden = 0,
    Draft = 1,
    Published = 2
}

public enum ContentType : byte
{
    Video = 1,
    File = 2,
    Quiz = 3,
    Assignment = 4,
    Workshop = 5
}

public enum VideoStatus : byte
{
    Processing = 0,
    Ready = 1,
    Failed = 2
}

public enum PrerequisiteType : byte
{
    CompleteContent = 1,
    CompleteStage = 2,
    CompleteModule = 3,
    PassAssessment = 4,
    FollowUpApproval = 5,
    Purchase = 6,
    TimeDelay = 7
}

public enum ProgressStatus : byte
{
    NotStarted = 0,
    InProgress = 1,
    Completed = 2
}
```

### 16.3 Assessment Domain

```csharp
public enum QuestionType : byte
{
    MCQ = 1,
    TrueFalse = 2,
    FileUpload = 3
}

public enum Difficulty : byte
{
    Easy = 1,
    Medium = 2,
    Hard = 3
}

public enum AssessmentType : byte
{
    Quiz = 1,
    Assignment = 2,
    FinalExam = 3
}

public enum AttemptStatus : byte
{
    InProgress = 0,
    Submitted = 1,
    Graded = 2,
    Abandoned = 3,
    TimedOut = 4
}
```

### 16.4 Study Plan Domain

```csharp
public enum StudyPlanStatus : byte
{
    Active = 0,
    Completed = 1,
    Abandoned = 2
}

public enum PlanDayType : byte
{
    Study = 1,
    Review = 2,
    Rest = 3,
    FollowUp = 4
}

public enum PlanDayStatus : byte
{
    Upcoming = 0,
    Completed = 1,
    Missed = 2,
    Rescheduled = 3
}

public enum PlanTaskType : byte
{
    Lecture = 1,
    Quiz = 2,
    Assignment = 3,
    Review = 4,
    ReviewQuiz = 5
}

public enum PlanTaskStatus : byte
{
    Todo = 0,
    InProgress = 1,
    Completed = 2,
    Missed = 3,
    Skipped = 4
}

public enum PlanAdjustmentType : byte
{
    MissedDay = 1,
    LowScore = 2,
    FollowUpFeedback = 3,
    Manual = 4,
    Reschedule = 5
}
```

### 16.5 Follow-Up Domain

```csharp
public enum FollowUpEnrollmentStatus : byte
{
    Active = 0,
    Paused = 1,
    Cancelled = 2,
    Completed = 3
}

public enum SessionTriggerType : byte
{
    AfterStage = 1,
    Scheduled = 2,
    Manual = 3
}

public enum CallStatus : byte
{
    Pending = 0,
    Scheduled = 1,
    InProgress = 2,
    Completed = 3,
    NoAnswer = 4,
    Rescheduled = 5
}

public enum StudentStatus : byte
{
    Excellent = 1,
    Good = 2,
    NeedsImprovement = 3,
    AtRisk = 4
}

public enum RecommendedAction : byte
{
    Continue = 1,
    AddReview = 2,
    Warning = 3,
    ContactParent = 4
}

public enum WarningSeverity : byte
{
    Mild = 1,
    Moderate = 2,
    Severe = 3
}

public enum WarningType : byte
{
    LowCommitment = 1,
    MissedSessions = 2,
    PoorPerformance = 3,
    Other = 4
}

public enum WaitingListStatus : byte
{
    Waiting = 0,
    Assigned = 1,
    Cancelled = 2
}

public enum TransferRequestStatus : byte
{
    Pending = 0,
    Approved = 1,
    Rejected = 2
}
```

### 16.6 Gamification Domain

```csharp
public enum GamificationSource : byte
{
    Lecture = 1,
    Quiz = 2,
    Assignment = 3,
    Challenge = 4,
    Streak = 5,
    FollowUp = 6,
    Achievement = 7,
    LevelUp = 8,
    Bonus = 9
}

public enum PointsTransactionType : byte
{
    Earned = 1,
    Spent = 2,
    Purchased = 3,
    Refund = 4,
    Gift = 5,
    Promo = 6
}

public enum AchievementCriteriaType : byte
{
    Streak = 1,
    QuizScore = 2,
    Completion = 3,
    SameDayCompletion = 4,
    ReviewOnTime = 5,
    NoMissedAssignments = 6,
    TopOfClass = 7,
    Custom = 99
}

public enum ChallengeType : byte
{
    Daily = 1,
    Weekly = 2,
    Special = 3
}

public enum ChallengeTargetType : byte
{
    StudyMinutes = 1,
    LecturesComplete = 2,
    QuizzesComplete = 3,
    QuizScoreAverage = 4,
    StreakDays = 5,
    PointsEarned = 6
}

public enum ChallengeStatus : byte
{
    InProgress = 0,
    Completed = 1,
    Failed = 2
}

public enum LeaderboardType : byte
{
    XP = 1,
    Streak = 2,
    CourseProgress = 3
}

public enum LeaderboardScope : byte
{
    Global = 1,
    Course = 2,
    StudyLevel = 3
}

public enum LeaderboardPeriod : byte
{
    Weekly = 1,
    Monthly = 2,
    AllTime = 3
}
```

### 16.7 Purchasing Domain

```csharp
public enum ProductType : byte
{
    Course = 1,
    Module = 2,
    ModuleFollowUp = 3,
    Content = 4,
    PointsPackage = 5
}

public enum DiscountType : byte
{
    Percentage = 1,
    FixedAmount = 2,
    FreePoints = 3
}

public enum OrderStatus : byte
{
    Pending = 0,
    Processing = 1,
    Paid = 2,
    Failed = 3,
    Refunded = 4,
    PartiallyRefunded = 5,
    Cancelled = 6,
    Expired = 7
}

public enum PaymentMethod : byte
{
    Card = 1,
    MobileWallet = 2,
    Fawry = 3,
    PointsOnly = 4,
    Mixed = 5
}

public enum EnrollmentType : byte
{
    FullCourse = 1,
    Module = 2,
    ModuleWithFollowUp = 3
}

public enum EnrollmentStatus : byte
{
    Active = 0,
    Expired = 1,
    Cancelled = 2,
    Suspended = 3
}

public enum RefundStatus : byte
{
    Pending = 0,
    Approved = 1,
    Rejected = 2,
    Processed = 3
}

public enum RefundMethod : byte
{
    OriginalPayment = 1,
    Points = 2,
    Manual = 3
}
```

### 16.8 Notifications Domain

```csharp
public enum NotificationType : byte
{
    Welcome = 1,
    Otp = 2,
    PurchaseComplete = 3,
    NewContent = 4,
    TaskReminder = 5,
    StreakWarning = 6,
    StreakLost = 7,
    Achievement = 8,
    LevelUp = 9,
    FollowUpScheduled = 10,
    FollowUpReminder = 11,
    AssessmentAvailable = 12,
    AssessmentGraded = 13,
    WarningIssued = 14,
    ParentReport = 15,
    LeaderboardUpdate = 16,
    SystemAnnouncement = 17,
    Custom = 99
}

public enum NotificationChannel : byte
{
    Push = 1,
    Email = 2,
    Sms = 3,
    InApp = 4
}

public enum NotificationPriority : byte
{
    Low = 1,
    Normal = 2,
    High = 3
}

public enum DeliveryStatus : byte
{
    Pending = 0,
    Sent = 1,
    Delivered = 2,
    Failed = 3
}
```

### 16.9 Support Domain

```csharp
public enum TicketCategory : byte
{
    Technical = 1,
    Payment = 2,
    Content = 3,
    Account = 4,
    FollowUp = 5,
    Refund = 6,
    Other = 99
}

public enum TicketStatus : byte
{
    Open = 0,
    InProgress = 1,
    WaitingOnCustomer = 2,
    WaitingOnThirdParty = 3,
    Resolved = 4,
    Closed = 5,
    Reopened = 6
}

public enum TicketPriority : byte
{
    Low = 1,
    Normal = 2,
    High = 3,
    Urgent = 4
}

public enum UserActivityType : byte
{
    Login = 1,
    Logout = 2,
    PageView = 3,
    VideoStart = 4,
    VideoComplete = 5,
    QuizStart = 6,
    QuizSubmit = 7,
    FileDownload = 8,
    Purchase = 9,
    Search = 10
}
```

---

## 17. ERD Diagrams

### 17.1 Core User Domain

```
┌─────────────────┐     ┌─────────────────┐     ┌─────────────────┐
│   StudyLevel    │────┤│ StudyLevelTrack │├────│     Track       │
└─────────────────┘     └────────┬────────┘     └─────────────────┘
                                 │
                                 │
                        ┌────────┴────────┐
                        │  StudentProfile │
                        └────────┬────────┘
                                 │
┌─────────────────┐     ┌────────┴────────┐     ┌─────────────────┐
│ TeacherProfile  │     │      User       │     │AssistantProfile │
└────────┬────────┘     └────────┬────────┘     └─────────────────┘
         │                       │
         │              ┌────────┴────────┐
         │              │  DeviceSession  │
         │              └─────────────────┘
         │
┌────────┴────────┐     ┌─────────────────┐     ┌─────────────────┐
│TeacherSocialLink│     │  ParentProfile  │────┤│  StudentParent  │
└─────────────────┘     └─────────────────┘     └─────────────────┘
```

### 17.2 Content Domain

```
┌─────────────────┐
│     Subject     │
└────────┬────────┘
         │
┌────────┴────────┐     ┌─────────────────┐
│     Course      │────┤│     Module      │
└────────┬────────┘     └────────┬────────┘
         │                       │
         │              ┌────────┴────────┐
         │              │      Stage      │
         │              └────────┬────────┘
         │                       │
         │              ┌────────┴────────┐
         └──────────────│   ContentItem   │
                        └────────┬────────┘
                                 │
         ┌───────────────────────┼───────────────────────┐
         │                       │                       │
┌────────┴────────┐     ┌────────┴────────┐     ┌────────┴────────┐
│  VideoContent   │     │   FileContent   │     │ContentPrerequisite│
└────────┬────────┘     └─────────────────┘     └─────────────────┘
         │
┌────────┴────────┐
│VideoProviderMeta│
└─────────────────┘
```

### 17.3 Assessment Domain

```
┌─────────────────┐     ┌─────────────────┐
│  QuestionBank   │────┤│    Question     │
└─────────────────┘     └────────┬────────┘
                                 │
                        ┌────────┴────────┐
                        │ QuestionOption  │
                        └─────────────────┘

┌─────────────────┐     ┌─────────────────┐
│ GradingRubric   │────┤│ RubricCriteria  │
└────────┬────────┘     └─────────────────┘
         │
┌────────┴────────┐     ┌─────────────────┐
│   Assessment    │────┤│AssessmentQuestion│
└────────┬────────┘     └─────────────────┘
         │
┌────────┴────────┐     ┌─────────────────┐
│ StudentAttempt  │────┤│  StudentAnswer  │
└────────┬────────┘     └─────────────────┘
         │
         ├─────────────────┬─────────────────┐
         │                 │                 │
┌────────┴────────┐ ┌──────┴──────┐ ┌────────┴────────┐
│AttemptRubricScore│ │StudentAssess│ │                 │
└─────────────────┘ │  mentScore  │ │                 │
                    └─────────────┘ └─────────────────┘
```

### 17.4 Relationships Summary

| Parent Table | Child Table | Relationship | ON DELETE |
|--------------|-------------|--------------|-----------|
| User | StudentProfile | 1:1 | CASCADE |
| User | TeacherProfile | 1:1 | CASCADE |
| User | AssistantProfile | 1:1 | CASCADE |
| User | ParentProfile | 1:1 | CASCADE |
| User | DeviceSession | 1:N | CASCADE |
| StudentProfile | StudentParent | 1:N | CASCADE |
| ParentProfile | StudentParent | 1:N | CASCADE |
| Course | Module | 1:N | CASCADE |
| Module | Stage | 1:N | CASCADE |
| Stage | ContentItem | 1:N | SET NULL |
| Course | ContentItem | 1:N | (no delete) |
| ContentItem | VideoContent | 1:1 | CASCADE |
| ContentItem | FileContent | 1:1 | CASCADE |
| ContentItem | StudentContentProgress | 1:N | CASCADE |
| QuestionBank | Question | 1:N | CASCADE |
| Question | QuestionOption | 1:N | CASCADE |
| Assessment | AssessmentQuestion | 1:N | CASCADE |
| Assessment | StudentAttempt | 1:N | (no delete) |
| StudentAttempt | StudentAnswer | 1:N | CASCADE |
| StudentProfile | StudyPlan | 1:N | CASCADE |
| StudyPlan | PlanDay | 1:N | CASCADE |
| PlanDay | PlanTask | 1:N | CASCADE |
| FollowUpGroup | StudentFollowUpEnrollment | 1:N | (no delete) |
| StudentFollowUpEnrollment | FollowUpSession | 1:N | CASCADE |
| StudentProfile | StudentGamification | 1:1 | CASCADE |
| StudentProfile | XpTransaction | 1:N | CASCADE |
| StudentProfile | PointsTransaction | 1:N | CASCADE |
| StudentProfile | Cart | 1:1 | CASCADE |
| Cart | CartItem | 1:N | CASCADE |
| Order | OrderItem | 1:N | CASCADE |
| StudentProfile | StudentEnrollment | 1:N | CASCADE |
| User | Notification | 1:N | CASCADE |
| Notification | NotificationDelivery | 1:N | CASCADE |
| User | SupportTicket | 1:N | (no delete) |
| SupportTicket | TicketMessage | 1:N | CASCADE |

---

## Document Complete ✅

**Version 2.0 Changes Summary:**

1. ✅ Sequential UUIDs for all tables (prevents fragmentation)
2. ✅ BIGINT PKs for high-volume tables (XP, Points, Activity, Notifications)
3. ✅ Fixed Product table with explicit nullable FKs + CHECK constraint
4. ✅ Added OrderItemSnapshot for purchase history integrity
5. ✅ Normalized JSON columns (Tags → QuestionTag, SocialLinks → TeacherSocialLink, etc.)
6. ✅ Added CHECK constraints for all business rules
7. ✅ Fixed StudyLevel+Track validation via StudyLevelTrackId FK
8. ✅ Added timezone support (User.PreferredTimezone)
9. ✅ Converted ParentProfile.Relation to enum
10. ✅ Added comprehensive indexes on FK columns
11. ✅ Optimized DeviceFingerprint to hash (64 chars)
12. ✅ Documented ON DELETE behavior for all FKs
13. ✅ Added data retention policies
14. ✅ Added archive strategy for high-volume tables
15. ✅ Cleaner, more consistent formatting throughout
