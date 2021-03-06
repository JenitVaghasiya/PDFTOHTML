USE [SamplePDFToHTML]
GO
/****** Object:  Table [dbo].[tblBriefs]    Script Date: 4/15/2021 1:03:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblBriefs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FileName] [varchar](200) NULL,
	[FilePath] [varchar](max) NULL,
	[CreatedDate] [datetime] NOT NULL,
 CONSTRAINT [PK__tblBrief__3214EC07EE56C431] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[tblBriefs] ON 

INSERT [dbo].[tblBriefs] ([Id], [FileName], [FilePath], [CreatedDate]) VALUES (1, N'AngularChallenge637540879011392498', N'~/htmlfiles/brief_1/', CAST(N'2021-04-15T07:21:41.387' AS DateTime))
SET IDENTITY_INSERT [dbo].[tblBriefs] OFF
