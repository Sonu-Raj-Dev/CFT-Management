/****** Object:  Table [dbo].[Permissions]    Script Date: 29-11-2025 22:47:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Permissions](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[PermissionName] [varchar](max) NULL,
	[IsActive] [bit] NULL,
	[CreatedBy] [bigint] NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [bigint] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_Permissions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RoleMaster]    Script Date: 29-11-2025 22:47:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RoleMaster](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[RoleName] [varchar](max) NULL,
	[IsActive] [bit] NULL,
	[CreatedBy] [bigint] NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [bigint] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_RoleMaster] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RolePermissionMapping]    Script Date: 29-11-2025 22:47:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RolePermissionMapping](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[RoleId] [bigint] NULL,
	[PermissionId] [bigint] NULL,
	[IsActive] [bit] NULL,
	[CreatedBy] [bigint] NULL,
	[CreatdDate] [datetime] NULL,
	[Modifiedby] [bigint] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_RolePermissionMapping] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserMaster]    Script Date: 29-11-2025 22:47:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserMaster](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[FirstName] [varchar](max) NULL,
	[MiddleName] [varchar](max) NULL,
	[LastName] [varchar](max) NULL,
	[LoginId] [varchar](max) NULL,
	[Password] [varchar](max) NULL,
	[IsActive] [bit] NULL,
	[CreatedBy] [bigint] NULL,
	[CreatedDate] [bigint] NULL,
	[ModifiedBy] [bigint] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_UserMaster] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserRoleMapping]    Script Date: 29-11-2025 22:47:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRoleMapping](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [bigint] NULL,
	[RoleId] [bigint] NULL,
	[IsActive] [bit] NULL,
	[CreatedBy] [bigint] NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [bigint] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_UserRoleMapping] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[RoleMaster] ON 
GO
INSERT [dbo].[RoleMaster] ([Id], [RoleName], [IsActive], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (1, N'Admin', 1, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[RoleMaster] ([Id], [RoleName], [IsActive], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (2, N'HR', 1, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[RoleMaster] ([Id], [RoleName], [IsActive], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (3, N'User', 1, NULL, NULL, NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[RoleMaster] OFF
GO
SET IDENTITY_INSERT [dbo].[UserMaster] ON 
GO
INSERT [dbo].[UserMaster] ([Id], [FirstName], [MiddleName], [LastName], [LoginId], [Password], [IsActive], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (1, N'Sonu', NULL, N'Rajbhar', N'sonu296702@gmail.com', N'1234', 1, NULL, NULL, NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[UserMaster] OFF
GO
SET IDENTITY_INSERT [dbo].[UserRoleMapping] ON 
GO
INSERT [dbo].[UserRoleMapping] ([Id], [UserId], [RoleId], [IsActive], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (1, 1, 1, 1, NULL, NULL, NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[UserRoleMapping] OFF
GO
/****** Object:  StoredProcedure [dbo].[spGetPermissionOnRoleID]    Script Date: 29-11-2025 22:47:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetPermissionOnRoleID] 
@RoleId bigint
AS
BEGIN
	select p.Id,p.PermissionName as SystemName from RolePermissionMapping as rpm
	left join [Permissions] as p on p.Id=rpm.PermissionId
	where rpm.RoleId=@RoleId
END
GO
/****** Object:  StoredProcedure [dbo].[stp_GetUserDetailsByLoginId]    Script Date: 29-11-2025 22:47:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[stp_GetUserDetailsByLoginId]
@LoginId varchar(MAX)=NULL
AS
BEGIN

if(@LoginId is not null)
begin
select Id,LoginId as EmailId from UserMaster where LoginId=LTRIM(rtrim(@LoginId))
end

END
GO
/****** Object:  StoredProcedure [dbo].[stp_GetUserRoleBYUserID]    Script Date: 29-11-2025 22:47:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[stp_GetUserRoleBYUserID]
@UserId bigint=0
AS
BEGIN

if(@UserId >0)
begin
select UM.Id as RoleId,R.RoleName  from UserRoleMapping UM
left join RoleMaster as R on R.Id=UM.RoleId
where UserId=LTRIM(rtrim(@UserId))
end

END
GO
/****** Object:  StoredProcedure [dbo].[stp_ValidateUserByEmailId]    Script Date: 29-11-2025 22:47:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:stp_ValidateUserByEmailId 'sonu296702@gmail.com'
-- =============================================
CREATE PROCEDURE [dbo].[stp_ValidateUserByEmailId]
@EmailId varchar(MAX)
AS
BEGIN

if(@EmailId is not null)
begin
select Id,LoginId as EmailId from UserMaster where LoginId=LTRIM(rtrim(@EmailId))
end

END
GO
