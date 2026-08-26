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
    WHERE [MigrationId] = N'20260825202912_InitialSecurity'
)
BEGIN
    CREATE TABLE [Permisos] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [NormalizedName] nvarchar(100) NOT NULL,
        [Description] nvarchar(250) NOT NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_Permisos] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260825202912_InitialSecurity'
)
BEGIN
    CREATE TABLE [Roles] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(50) NOT NULL,
        [NormalizedName] nvarchar(50) NOT NULL,
        [Description] nvarchar(250) NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260825202912_InitialSecurity'
)
BEGIN
    CREATE TABLE [Usuarios] (
        [Id] uniqueidentifier NOT NULL,
        [Username] nvarchar(50) NOT NULL,
        [NormalizedUsername] nvarchar(50) NOT NULL,
        [Email] nvarchar(254) NOT NULL,
        [NormalizedEmail] nvarchar(254) NOT NULL,
        [PasswordHash] nvarchar(512) NOT NULL,
        [CreatedAt] datetimeoffset NOT NULL,
        [UpdatedAt] datetimeoffset NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_Usuarios] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260825202912_InitialSecurity'
)
BEGIN
    CREATE TABLE [RolesPermisos] (
        [RoleId] uniqueidentifier NOT NULL,
        [PermissionId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_RolesPermisos] PRIMARY KEY ([RoleId], [PermissionId]),
        CONSTRAINT [FK_RolesPermisos_Permisos_PermissionId] FOREIGN KEY ([PermissionId]) REFERENCES [Permisos] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_RolesPermisos_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260825202912_InitialSecurity'
)
BEGIN
    CREATE TABLE [UsuariosRoles] (
        [UserId] uniqueidentifier NOT NULL,
        [RoleId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_UsuariosRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_UsuariosRoles_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_UsuariosRoles_Usuarios_UserId] FOREIGN KEY ([UserId]) REFERENCES [Usuarios] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260825202912_InitialSecurity'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'Description', N'IsActive', N'Name', N'NormalizedName', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[Permisos]'))
        SET IDENTITY_INSERT [Permisos] ON;
    EXEC(N'INSERT INTO [Permisos] ([Id], [CreatedAt], [Description], [IsActive], [Name], [NormalizedName], [UpdatedAt])
    VALUES (''10000000-0000-0000-0000-000000000001'', ''2026-01-01T00:00:00.0000000+00:00'', N''Allows SECURITY.USER.VIEW.'', CAST(1 AS bit), N''SECURITY.USER.VIEW'', N''SECURITY.USER.VIEW'', NULL),
    (''10000000-0000-0000-0000-000000000002'', ''2026-01-01T00:00:00.0000000+00:00'', N''Allows SECURITY.USER.CREATE.'', CAST(1 AS bit), N''SECURITY.USER.CREATE'', N''SECURITY.USER.CREATE'', NULL),
    (''10000000-0000-0000-0000-000000000003'', ''2026-01-01T00:00:00.0000000+00:00'', N''Allows SECURITY.USER.UPDATE.'', CAST(1 AS bit), N''SECURITY.USER.UPDATE'', N''SECURITY.USER.UPDATE'', NULL),
    (''10000000-0000-0000-0000-000000000004'', ''2026-01-01T00:00:00.0000000+00:00'', N''Allows SECURITY.USER.DELETE.'', CAST(1 AS bit), N''SECURITY.USER.DELETE'', N''SECURITY.USER.DELETE'', NULL),
    (''10000000-0000-0000-0000-000000000005'', ''2026-01-01T00:00:00.0000000+00:00'', N''Allows SECURITY.USER.ASSIGN_ROLE.'', CAST(1 AS bit), N''SECURITY.USER.ASSIGN_ROLE'', N''SECURITY.USER.ASSIGN_ROLE'', NULL),
    (''10000000-0000-0000-0000-000000000006'', ''2026-01-01T00:00:00.0000000+00:00'', N''Allows SECURITY.ROLE.VIEW.'', CAST(1 AS bit), N''SECURITY.ROLE.VIEW'', N''SECURITY.ROLE.VIEW'', NULL),
    (''10000000-0000-0000-0000-000000000007'', ''2026-01-01T00:00:00.0000000+00:00'', N''Allows SECURITY.ROLE.CREATE.'', CAST(1 AS bit), N''SECURITY.ROLE.CREATE'', N''SECURITY.ROLE.CREATE'', NULL),
    (''10000000-0000-0000-0000-000000000008'', ''2026-01-01T00:00:00.0000000+00:00'', N''Allows SECURITY.ROLE.UPDATE.'', CAST(1 AS bit), N''SECURITY.ROLE.UPDATE'', N''SECURITY.ROLE.UPDATE'', NULL),
    (''10000000-0000-0000-0000-000000000009'', ''2026-01-01T00:00:00.0000000+00:00'', N''Allows SECURITY.ROLE.DELETE.'', CAST(1 AS bit), N''SECURITY.ROLE.DELETE'', N''SECURITY.ROLE.DELETE'', NULL),
    (''10000000-0000-0000-0000-000000000010'', ''2026-01-01T00:00:00.0000000+00:00'', N''Allows SECURITY.ROLE.ASSIGN_PERMISSION.'', CAST(1 AS bit), N''SECURITY.ROLE.ASSIGN_PERMISSION'', N''SECURITY.ROLE.ASSIGN_PERMISSION'', NULL),
    (''10000000-0000-0000-0000-000000000011'', ''2026-01-01T00:00:00.0000000+00:00'', N''Allows SECURITY.PERMISSION.VIEW.'', CAST(1 AS bit), N''SECURITY.PERMISSION.VIEW'', N''SECURITY.PERMISSION.VIEW'', NULL)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'Description', N'IsActive', N'Name', N'NormalizedName', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[Permisos]'))
        SET IDENTITY_INSERT [Permisos] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260825202912_InitialSecurity'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Permisos_NormalizedName] ON [Permisos] ([NormalizedName]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260825202912_InitialSecurity'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Roles_NormalizedName] ON [Roles] ([NormalizedName]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260825202912_InitialSecurity'
)
BEGIN
    CREATE INDEX [IX_RolesPermisos_PermissionId] ON [RolesPermisos] ([PermissionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260825202912_InitialSecurity'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Usuarios_NormalizedEmail] ON [Usuarios] ([NormalizedEmail]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260825202912_InitialSecurity'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Usuarios_NormalizedUsername] ON [Usuarios] ([NormalizedUsername]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260825202912_InitialSecurity'
)
BEGIN
    CREATE INDEX [IX_UsuariosRoles_RoleId] ON [UsuariosRoles] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260825202912_InitialSecurity'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260825202912_InitialSecurity', N'8.0.20');
END;
GO

COMMIT;
GO

