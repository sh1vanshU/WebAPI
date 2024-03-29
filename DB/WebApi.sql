USE [WebApi]
GO
/****** Object:  Table [dbo].[ProfileTable]    Script Date: 9/27/2019 2:45:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProfileTable](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [varchar](50) NULL,
	[LastName] [varchar](50) NULL,
	[DOB] [date] NULL,
	[Language] [varchar](50) NULL,
	[UserID] [int] NULL,
 CONSTRAINT [PK_Profile] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RegistrationTable]    Script Date: 9/27/2019 2:45:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RegistrationTable](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[Username] [varchar](50) NOT NULL,
	[Password] [varchar](50) NOT NULL,
	[UserRole] [varchar](50) NULL,
	[Token] [varchar](max) NULL,
 CONSTRAINT [PK_Registration] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[ProfileTable] ON 

INSERT [dbo].[ProfileTable] ([ID], [FirstName], [LastName], [DOB], [Language], [UserID]) VALUES (1002, N'Shivanshu', N'Gupta', CAST(N'1997-12-24' AS Date), N'India', 1)
SET IDENTITY_INSERT [dbo].[ProfileTable] OFF
SET IDENTITY_INSERT [dbo].[RegistrationTable] ON 

INSERT [dbo].[RegistrationTable] ([UserID], [Username], [Password], [UserRole], [Token]) VALUES (1, N'shivanshu.gupta4@gmail.com', N'12345', NULL, N'shivanshu.gupta4@gmail.com-12345-9/20/2019 10:06:12 AM-1')
SET IDENTITY_INSERT [dbo].[RegistrationTable] OFF
ALTER TABLE [dbo].[ProfileTable]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [dbo].[RegistrationTable] ([UserID])
GO
