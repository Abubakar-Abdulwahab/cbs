USE [CBSPaymentEngine]
GO
/****** Object:  Table [dbo].[UserProfile]    Script Date: 10/2/2018 5:02:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserProfile](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](255) NOT NULL,
	[PasswordHash] [nvarchar](255) NOT NULL
) ON [PRIMARY]

GO
SET IDENTITY_INSERT [dbo].[UserProfile] ON 

INSERT [dbo].[UserProfile] ([Id], [UserName], [PasswordHash]) VALUES (1, N'EIRSAdmin', N'AFjYg76lZ6MFGjbPQyW3ZiNts+OQaACfTqzjT6BDvwSqsMYy35Mrivt32AUwq69Djg==')
SET IDENTITY_INSERT [dbo].[UserProfile] OFF
