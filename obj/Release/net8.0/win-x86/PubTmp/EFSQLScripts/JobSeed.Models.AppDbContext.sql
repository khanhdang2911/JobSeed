IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240616082541_InitModel'
)
BEGIN
    CREATE TABLE [JobType] (
        [Id] int NOT NULL IDENTITY,
        [JobTitle] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_JobType] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240616082541_InitModel'
)
BEGIN
    CREATE TABLE [Role] (
        [Id] int NOT NULL IDENTITY,
        [RoleName] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Role] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240616082541_InitModel'
)
BEGIN
    CREATE TABLE [Users] (
        [Id] int NOT NULL IDENTITY,
        [Email] nvarchar(max) NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Phone] nvarchar(max) NOT NULL,
        [Password] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240616082541_InitModel'
)
BEGIN
    CREATE TABLE [Job] (
        [Id] int NOT NULL IDENTITY,
        [JobName] nvarchar(max) NOT NULL,
        [JobDescription] nvarchar(max) NOT NULL,
        [PublishDate] datetime2 NOT NULL,
        [CompanyName] nvarchar(max) NOT NULL,
        [Location] nvarchar(max) NOT NULL,
        [FromSalary] decimal(18,2) NOT NULL,
        [ToSalary] decimal(18,2) NOT NULL,
        [JobTypeId] int NOT NULL,
        CONSTRAINT [PK_Job] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Job_JobType_JobTypeId] FOREIGN KEY ([JobTypeId]) REFERENCES [JobType] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240616082541_InitModel'
)
BEGIN
    CREATE TABLE [UsersRole] (
        [UsersId] int NOT NULL,
        [RoleId] int NOT NULL,
        CONSTRAINT [PK_UsersRole] PRIMARY KEY ([UsersId], [RoleId]),
        CONSTRAINT [FK_UsersRole_Role_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Role] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_UsersRole_Users_UsersId] FOREIGN KEY ([UsersId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240616082541_InitModel'
)
BEGIN
    CREATE TABLE [JobDetail] (
        [Id] int NOT NULL IDENTITY,
        [JobId] int NOT NULL,
        [Gender] bit NULL,
        [Experiences] nvarchar(max) NULL,
        [Qualifications] nvarchar(max) NULL,
        [Benefits] nvarchar(max) NULL,
        CONSTRAINT [PK_JobDetail] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_JobDetail_Job_JobId] FOREIGN KEY ([JobId]) REFERENCES [Job] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240616082541_InitModel'
)
BEGIN
    CREATE INDEX [IX_Job_JobTypeId] ON [Job] ([JobTypeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240616082541_InitModel'
)
BEGIN
    CREATE INDEX [IX_JobDetail_JobId] ON [JobDetail] ([JobId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240616082541_InitModel'
)
BEGIN
    CREATE INDEX [IX_UsersRole_RoleId] ON [UsersRole] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240616082541_InitModel'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240616082541_InitModel', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240616084025_AddStatus'
)
BEGIN
    ALTER TABLE [Job] ADD [Status] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240616084025_AddStatus'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240616084025_AddStatus', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240616090523_AddImageLink_JobTable'
)
BEGIN
    DROP INDEX [IX_JobDetail_JobId] ON [JobDetail];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240616090523_AddImageLink_JobTable'
)
BEGIN
    ALTER TABLE [Job] ADD [ImageLink] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240616090523_AddImageLink_JobTable'
)
BEGIN
    CREATE UNIQUE INDEX [IX_JobDetail_JobId] ON [JobDetail] ([JobId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240616090523_AddImageLink_JobTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240616090523_AddImageLink_JobTable', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240616094139_RemoveJobDetail'
)
BEGIN
    DROP TABLE [JobDetail];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240616094139_RemoveJobDetail'
)
BEGIN
    ALTER TABLE [Job] ADD [Benefits] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240616094139_RemoveJobDetail'
)
BEGIN
    ALTER TABLE [Job] ADD [Experiences] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240616094139_RemoveJobDetail'
)
BEGIN
    ALTER TABLE [Job] ADD [Gender] bit NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240616094139_RemoveJobDetail'
)
BEGIN
    ALTER TABLE [Job] ADD [Qualifications] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240616094139_RemoveJobDetail'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240616094139_RemoveJobDetail', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240616113202_UpdateFK_Job'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240616113202_UpdateFK_Job', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240617020505_AddCategory'
)
BEGIN
    CREATE TABLE [Category] (
        [Id] int NOT NULL IDENTITY,
        [CategoryName] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Category] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240617020505_AddCategory'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240617020505_AddCategory', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240617020815_UpdateCategory_job'
)
BEGIN
    ALTER TABLE [Job] ADD [CategoryId] int NOT NULL DEFAULT 1;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240617020815_UpdateCategory_job'
)
BEGIN
    CREATE INDEX [IX_Job_CategoryId] ON [Job] ([CategoryId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240617020815_UpdateCategory_job'
)
BEGIN
    ALTER TABLE [Job] ADD CONSTRAINT [FK_Job_Category_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Category] ([Id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240617020815_UpdateCategory_job'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240617020815_UpdateCategory_job', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240617092326_AddResponsibility_column'
)
BEGIN
    ALTER TABLE [Job] ADD [Responsibility] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240617092326_AddResponsibility_column'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240617092326_AddResponsibility_column', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240619125206_AddAvatar_User'
)
BEGIN
    ALTER TABLE [Users] ADD [ImageLink] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240619125206_AddAvatar_User'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240619125206_AddAvatar_User', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240620032914_AddUsersJob'
)
BEGIN
    ALTER TABLE [Job] ADD [EmployerId] int NOT NULL DEFAULT 1;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240620032914_AddUsersJob'
)
BEGIN
    CREATE TABLE [UsersJob] (
        [JobId] int NOT NULL,
        [UsersId] int NOT NULL,
        [ApplyDate] datetime2 NOT NULL,
        [ImageLink] nvarchar(max) NULL,
        CONSTRAINT [PK_UsersJob] PRIMARY KEY ([UsersId], [JobId]),
        CONSTRAINT [FK_UsersJob_Job_JobId] FOREIGN KEY ([JobId]) REFERENCES [Job] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_UsersJob_Users_UsersId] FOREIGN KEY ([UsersId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240620032914_AddUsersJob'
)
BEGIN
    CREATE INDEX [IX_Job_EmployerId] ON [Job] ([EmployerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240620032914_AddUsersJob'
)
BEGIN
    CREATE INDEX [IX_UsersJob_JobId] ON [UsersJob] ([JobId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240620032914_AddUsersJob'
)
BEGIN
    ALTER TABLE [Job] ADD CONSTRAINT [FK_Job_Users_EmployerId] FOREIGN KEY ([EmployerId]) REFERENCES [Users] ([Id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240620032914_AddUsersJob'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240620032914_AddUsersJob', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240620035636_AddState_Job'
)
BEGIN
    ALTER TABLE [Job] ADD [State] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240620035636_AddState_Job'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240620035636_AddState_Job', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240621084716_update_userjob'
)
BEGIN
    EXEC sp_rename N'[UsersJob].[ImageLink]', N'CV', N'COLUMN';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240621084716_update_userjob'
)
BEGIN
    ALTER TABLE [UsersJob] ADD [Coverletter] nvarchar(max) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240621084716_update_userjob'
)
BEGIN
    ALTER TABLE [UsersJob] ADD [SocialLinkAccount] nvarchar(max) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240621084716_update_userjob'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240621084716_update_userjob', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240622034328_AdState_UsersJob'
)
BEGIN
    ALTER TABLE [UsersJob] ADD [State] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240622034328_AdState_UsersJob'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240622034328_AdState_UsersJob', N'8.0.6');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240622064342_EditState'
)
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UsersJob]') AND [c].[name] = N'State');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [UsersJob] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [UsersJob] ALTER COLUMN [State] bit NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240622064342_EditState'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240622064342_EditState', N'8.0.6');
END;
GO

COMMIT;
GO

