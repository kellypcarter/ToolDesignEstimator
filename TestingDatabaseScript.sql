/*
	Database Creation Script and Test Data insert

	Created 09.28.17 | Tarron Lane

	Creates the databases and indexes for the Boeing Tool Estimation on-campus internship project
	and inserts a set of dummy data to be used for testing. The databases and tables will follow
	the following naming convention:

		* No Pluralized names (eg Customer, not Customers)
		* No Underscores
		* Upper Camel Case (ie UpperCamelCase)
		* No abbreviations except universally understood abbreviations
		* Avoid using keywords for table or field names
	[Insert more naming conventions here]

	TODO
	- add short descritions on each create-table to show what each field means

*/

-- =================================
-- === Create the database and tables
-- =================================
-- Drop the database if it exists
if exists (
	select top 1 * 
	from master.dbo.sysdatabases sd
	where sd.name = 'ToolApp'
)
begin
	drop database [ToolApp];
end

go

-- Create the database
create database [ToolApp];

-- Grant sa ownership of the database
alter authorization on database::[ToolApp] to sa;

go

-- Create the Estimate Table
create table [ToolApp].[dbo].[Estimate] (
	[EstimateID] int identity(1,1) primary key not null
	,[DesignOrderSID] int not null -- FK Ties the estimate to a design order. Surrogate because design order not always known at insert time
	
	-- Fields for Tracking status of design order when estimate was made (all these fields affect estimate number)
	,[NeedsSurfacing] bit not null
	,[IsStressIncluded] bit not null -- If this is true, then we ignore the stressworktype field in all calculations
	,[StressWorkTypeID] int null -- FK
	,[ComplexityLevel] int not null -- FK
	,[EngineeringReleased] bit not null
	,[FamilyClassID] int null -- FK, identifies if this tool is part of a family, or the first of its kind, etc
	
	-- Estimate details
	,[HoursEstimate] float(24) not null
	,[DesignHour] float(24) not null
	,[StressHour] float(24) not null 
	,[CheckHour] float(24) not null
	,[ReleaseHour] float(24) not null
	,[DesignFlow] float(24) not null
	,[KBEBreakEvenNum] float(24) null
	,[IsLatestEstimate] bit not null
	,[Comment] nvarchar(2000) null
	,[ReasonForEstimateChange] nvarchar(2000) not null

	-- Audit fields
	,[CreatedByUserID] int null -- FK
	,[CreatedDate] datetime2(2) not null constraint Estimate_CreatedDate_Default DEFAULT GETDATE()
	,[UpdatedDate] datetime2(2) null
	,IsDeleted bit null constraint Estimate_IsDeleted_Default default 0

);

-- Create DesignOrder table
create table [ToolApp].[dbo].[DesignOrder] (
	[DesignOrderSID] int identity(1,1) primary key not null
	,[DesignOrderNumber] nvarchar(500) null -- Actual real-world design order number for this order
	,[DescriptiveName] varchar(500) not null -- Descriptive name to describe this design order
	
	-- Tool fields
	,[ToolTypeID] int not null -- FK
	,[PartNumber] nvarchar(500) null

	,[IsCompleted] bit not null -- Flags the design order as completed, meaning no estimates need made for it
	,[ActualHours] float(24) null -- The final number of hours that were worked on the design order

	-- Audit fields
	,[CreatedByUserID] int null -- FK
	,[CreatedDate] datetime2(2) null constraint DesignOrder_CreatedDate_Default DEFAULT GETDATE()
	,[UpdatedDate] datetime2(2) null
	,IsDeleted bit null constraint DesignOrder_IsDeleted_Default default 0
);

-- Create StressWorkType table (this will have an option that is "stress included in data")
create table [ToolApp].[dbo].[StressWorkType] (
	[StressWorkTypeID] int primary key identity(1,1) not null
	,[StressWorkTypeName] nvarchar(100) not null
	,[StressWorkTypeDescription] nvarchar(2000) null

	-- Audit fields
	,[CreatedByUserID] int null -- FK
	,[CreatedDate] datetime2(2) not null constraint StressWorkType_CreatedDate_Default DEFAULT GETDATE()
	,[UpdatedDate] datetime2(2) null
);

-- Create the FamilyClass table
create table [ToolApp].[dbo].[FamilyClass] (
	[FamilyClassID] int primary key identity(1,1) not null
	,[FamilyClassName] nvarchar(256) not null -- First in family, second in family, etc.
	,[FamilyClassDescription] nvarchar(2000) null

	-- Audit fields
	,[CreatedByUserID] int null -- FK
	,[CreatedDate] datetime2(2) not null constraint FamilyClass_CreatedDate_Default DEFAULT GETDATE()
	,[UpdatedDate] datetime2(2) null
);

-- Create the Users Table
create table [ToolApp].[dbo].[AppUser] ( -- Changed from "User" to avoid using keyword
	[AppUserID] int primary key identity(1,1) not null
	,[UserEmail] nvarchar(100) not null
	,[FirstName] nvarchar(100) not null
	,[LastName] nvarchar(100) not null
	,[PasswordHash] nvarchar(200) not null
	,[PasswordSalt] nvarchar(200) not null
	,[AccessLevelID] int not null -- FK

	-- Audit fields
	,[CreatedByUserID] int null -- FK
	,[CreatedDate] datetime2(2) not null constraint AppUser_CreatedDate_Default DEFAULT GETDATE()
	,[UpdatedDate] datetime2(2) null
    ,IsDeleted bit null constraint User_IsDeleted_Default default 0
);

-- Create the Access Level Table
create table [ToolApp].[dbo].[AccessLevel] (
	[AccessLevelID] int primary key identity(1,1) not null
	,[AccessLevelName] nvarchar(100) not null
	,[AccessLevelDescription] nvarchar(2000) not null

	-- Audit fields
	,[CreatedByUserID] int null -- FK
	,[CreatedDate] datetime2(2) not null constraint AccessLevel_CreatedDate_Default DEFAULT GETDATE()
	,[UpdatedDate] datetime2(2) null
);

-- Create the Tool Type table
create table [ToolApp].[dbo].[ToolType] (
	[ToolTypeID] int primary key identity(1,1) not null
	,[ToolCode] nvarchar(10) not null
	,[ToolTypeName] nvarchar(100) null
	,[ToolTypeDescription] nvarchar(2000) null
	,[IsStressRequiredDefault] bit not null
	,[StandardFlow] float(24) null

	-- Audit fields
	,[CreatedByUserID] int null -- FK
	,[CreatedDate] datetime2(2) not null constraint ToolType_CreatedDate_Default DEFAULT GETDATE()
	,[UpdatedDate] datetime2(2) null
);

-- Create the statistics table
create table [ToolApp].[dbo].[Statistic] (
	[StatisticID] int primary key identity(1,1) not null
	,[ToolTypeID] int not null -- FK
	,[ComplexityLevel] int not null -- FK
	,[StandardDeviation] float(48) not null
	,[AverageHours] float(24) not null
	,[ImageFilePath] nvarchar(200) null
	,[ReleaseHours] float(24) not null
	,[StatisticNotes] nvarchar(1000) null
	
	-- Table versioned information
	,[EffectiveStartDate] datetime2(2) not null
	,[EffectiveEndDate] datetime2(2) null
	,[isCurrent] bit not null

	-- Audit fields
	,[CreatedByUserID] int null -- FK
	,[CreatedDate] datetime2(2) not null constraint Statistic_CreatedDate_Default DEFAULT GETDATE()
	,[UpdatedDate] datetime2(2) null
);

-- Create the Adjustment Factors table
create table [ToolApp].[dbo].[AdjustmentFactor] (
	[AdjustmentFactorID] int primary key identity(1,1) not null
	,[NSAR] float(24) null
	,[NSRR] float(24) null
	,[StressAnalysis] float(24) null
	,[SurfacingRequired] float(24) null
	,[EngineeringReleased] float(24) null
	,[FirstToolInFamily] float(24) null
	,[SecondToolInFamily] float(24) null
	,[KBEDevelopment] float(24) null
	,[KBEFollowOnTooling] float(24) null

	-- Audit fields
	,[CreatedByUserID] int null -- FK
	,[CreatedDate] datetime2(2) not null constraint AdjustmentFactor_CreatedDate_Default DEFAULT GETDATE()
	,[UpdatedDate] datetime2(2) null

);

-- Create ComplexityLevel Table
create table [ToolApp].[dbo].[ComplexityLevel] (
	[ComplexityLevel] int primary key not null
	,[ComplexityLevelName] nvarchar(50) not null
	,[ComplexityLevelDescription] nvarchar(2000) null

	-- Audit fields
	,[CreatedByUserID] int null -- FK
	,[CreatedDate] datetime2(2) not null constraint ComplexityLevel_CreatedDate_Default DEFAULT GETDATE()
	,[UpdatedDate] datetime2(2) null
);


-- ===============================
-- === Insert Test Data
-- ===============================
-- AppUser Table
set identity_insert ToolApp.dbo.AppUser on;

insert into [ToolApp].[dbo].[AppUser] (AppUserID, [UserEmail], [FirstName], [LastName], [PasswordHash], [PasswordSalt], AccessLevelID, CreatedByUserID, IsDeleted)
values 
	(1, 'tarronlane@hotmail.com', 'Tarron', 'Lane', 'EE73E86147937A4D6C0D0D67BBE309CEBD990D82D63347FB707A69FDC38501A5', 'mj1IdcoA5UUxBpOA3Ec6gQ==', 1, 1, 0),
	(2, 'wesley.z.chow@boeing.com', 'Wes', 'Chow', '2E95889F203C29A23FFE796985FA0CAD735F49674515809EEA74BD6918F97DAF', 's3Dvbt6aCeOWXqWGSAxDXg====', 1, 1, 0)
	
	;

--insert into [ToolApp].[dbo].[AppUser] (AppUserID, [UserEmail], [UserName], [PasswordHash], [PasswordSalt], AccessLevelID, CreatedByUserID, IsDeleted)
--values (2, 'campbell3alex@gmail.com', 'AlexC', , , 1, 1, 0);

--insert into [ToolApp].[dbo].[AppUser] (AppUserID, [UserEmail], [UserName], [PasswordHash], [PasswordSalt], AccessLevelID, CreatedByUserID, IsDeleted)
--values (3, 'nikneue.ash@gmail.com', 'AshlanN', , , 1, 1, 0);

--insert into [ToolApp].[dbo].[AppUser] (AppUserID, [UserEmail], [UserName], [PasswordHash], [PasswordSalt], AccessLevelID, CreatedByUserID, IsDeleted)
--values (4, 'andypetern@gmail.com', 'AndrewN', , , 1, 1, 0);

--insert into [ToolApp].[dbo].[AppUser] (AppUserID, [UserEmail], [UserName], [PasswordHash], [PasswordSalt], AccessLevelID, CreatedByUserID, IsDeleted)
--values (5, 'brian.r.otte20@gmail.com', 'BrianO', , , 1, 1, 0);

--insert into [ToolApp].[dbo].[AppUser] (AppUserID, [UserEmail], [UserName], [PasswordHash], [PasswordSalt], AccessLevelID, CreatedByUserID)
--values (6, 'yuchen.v.v.v@gmail.com', 'ChenY', , , 1, 1);

set identity_insert ToolApp.dbo.AppUser off;

-- Tool Type Table
set identity_insert ToolApp.dbo.ToolType on;

insert into ToolApp.dbo.ToolType (ToolTypeID, ToolCode, ToolTypeName, ToolTypeDescription, IsStressRequiredDefault, CreatedByUserID, StandardFlow)
values (1, 'AJ', 'AJ Name', 'AJ Description', 0, 1, 6.5),
	(2, 'CF', 'CF Name', 'CF Description', 0, 1, 6.5),
	(3, 'DJ', 'DJ Name', 'DJ Description', 0, 1, 6.5),
	(4, 'FAJ', 'FAJ Name', 'FAJ Description', 0, 1, 6.5),
	(5, 'FME', 'FME Name', 'FME Description', 1, 1, 6.5),
	(6, 'HF', 'HF Name', 'HF Description', 0, 1, 6.5),
	(7, 'LM', 'LM Name', 'LM Description', 0, 1, 6.5),
	(8, 'ME', 'ME Name', 'ME Description', 1, 1, 6.5),
	(9, 'NCMF', 'NCMF Name', 'NCMF Description', 0, 1, 6.5),
	(10, 'OHME', 'OHME Name', 'OHME Description', 1, 1, 6.5),
	(11, 'PME', 'PME Name', 'PME Description', 1, 1, 6.5),
	(12, 'SME', 'SME Name', 'SME Description', 1, 1, 6.5);

set identity_insert ToolApp.dbo.ToolType off;

-- Insert Test data for Complexity Level Table
insert into ToolApp.dbo.ComplexityLevel (ComplexityLevel, ComplexityLevelName, ComplexityLevelDescription, CreatedByUserID)
values (1, 'Low Complexity', 'Tools that are not very complex', 1),
	(2, 'Medium-Low Complexity', 'Tools that are slightly complex', 1),
	(3, 'Medium Complexity', 'Tools that are moderately complex', 1),
	(4, 'Medium-High Complexity', 'Tools that are significantly complex', 1),
	(5, 'High Complexity', 'Tools that are very complex', 1);

-- Insert Test data for Adjustment Factors table
set identity_insert ToolApp.dbo.AdjustmentFactor on;

insert into ToolApp.dbo.AdjustmentFactor (AdjustmentFactorID, NSAR, NSRR, StressAnalysis, SurfacingRequired, EngineeringReleased, FirstToolInFamily, SecondToolInFamily, KBEDevelopment, KBEFollowOnTooling, CreatedByUserID)
values (1, 0.1, 0, 1, 1, 1, 4, 2, 2, 2, 1);

set identity_insert ToolApp.dbo.AdjustmentFactor off;

-- Insert Test data for statistics
set identity_insert ToolApp.dbo.Statistic on;

insert into ToolApp.dbo.Statistic (StatisticID, ToolTypeID, ComplexityLevel, StandardDeviation, AverageHours, ImageFilePath, ReleaseHours, EffectiveStartDate, CreatedByUserID, isCurrent)
values 
	   -- AJ
	   (1, 1, 1, 4.32429780832538, 57, '../../content/toolpictures/AJ_COMPLEX_1.jpg', 8, getdate(), 1, 1),
	   (2, 1, 2, 5.49439097961203, 74, '../../content/toolpictures/AJ_COMPLEX_2.jpg', 8, getdate(), 1, 1),
	   (3, 1, 3, 7.60880921453746, 110, '../../content/toolpictures/AJ_COMPLEX_3.jpg', 8, getdate(), 1, 1),
	   (4, 1, 4, 29.5806017332897, 181, '../../content/toolpictures/AJ_COMPLEX_4.jpg', 8, getdate(), 1, 1),
	   (5, 1, 5, 168.302598537106, 375.5, '../../content/toolpictures/AJ_COMPLEX_5.jpg', 8, getdate(), 1, 1),
	   -- CF
	   (6, 2, 1, 6.50411957366801, 53, '../../content/toolpictures/CF_COMPLEX_1.jpg', 8, getdate(), 1, 1),
	   (7, 2, 2, 3.24605988267388, 66, '../../content/toolpictures/CF_COMPLEX_2.jpg', 8, getdate(), 1, 1),
	   (8, 2, 3, 6.43583716388163, 89.2, '../../content/toolpictures/CF_COMPLEX_3.jpg', 8, getdate(), 1, 1),
	   (9, 2, 4, 27.0744801647322, 147.7, '../../content/toolpictures/CF_COMPLEX_4.jpg', 8, getdate(), 1, 1),
	   (10, 2, 5, 102.459574524613, 311, '../../content/toolpictures/CF_COMPLEX_5.jpg', 8, getdate(), 1, 1),
	   -- DJ
	   (11, 3, 1, 2.71746925071088, 46, '../../content/toolpictures/DJ_COMPLEX_1.jpg', 8, getdate(), 1, 1),
	   (12, 3, 2, 3.91194437546634, 57, '../../content/toolpictures/DJ_COMPLEX_2.jpg', 8, getdate(), 1, 1),
	   (13, 3, 3, 6.1762672877725, 82.5, '../../content/toolpictures/DJ_COMPLEX_3.jpg', 8, getdate(), 1, 1),
	   (14, 3, 4, 15.7260630120582, 137, '../../content/toolpictures/DJ_COMPLEX_4.jpg', 8, getdate(), 1, 1),
	   (15, 3, 5, 50.0463404367696, 216.1, '../../content/toolpictures/DJ_COMPLEX_5.jpg', 8, getdate(), 1, 1),
	   -- FAJ
	   (16, 4, 1, 45.336785364016, 161.5, '../../content/toolpictures/FAJ_COMPLEX_1.jpg', 8, getdate(), 1, 1),
	   (17, 4, 2, 35.3148379265397, 306, '../../content/toolpictures/FAJ_COMPLEX_2.jpg', 8, getdate(), 1, 1),
	   (18, 4, 3, 52.5675602711131, 459.3, '../../content/toolpictures/FAJ_COMPLEX_3.jpg', 8, getdate(), 1, 1),
	   (19, 4, 4, 265.117233502463, 814, '../../content/toolpictures/FAJ_COMPLEX_4.jpg', 8, getdate(), 1, 1),
	   (20, 4, 5, 434.74997959966, 1755.5, '../../content/toolpictures/FAJ_COMPLEX_5.jpg', 8, getdate(), 1, 1),
	   -- FME
	   (21, 5, 1, 8.28366148626606, 148, '../../content/toolpictures/FME_COMPLEX_1.jpg', 8, getdate(), 1, 1),
	   (22, 5, 2, 38.5447328252698, 214, '../../content/toolpictures/FME_COMPLEX_2.jpg', 8, getdate(), 1, 1),
	   (23, 5, 3, 25.1419825334894, 342, '../../content/toolpictures/FME_COMPLEX_3.jpg', 8, getdate(), 1, 1),
	   (24, 5, 4, 117.029903840246, 509, '../../content/toolpictures/FME_COMPLEX_4.jpg', 8, getdate(), 1, 1),
	   (25, 5, 5, 206.816766728426, 803.5, '../../content/toolpictures/FME_COMPLEX_5.jpg', 8, getdate(), 1, 1),
	   -- HF
	   (26, 6, 1, 2.72144263213465, 47, '../../content/toolpictures/HF_COMPLEX_1.jpg', 8, getdate(), 1, 1),
	   (27, 6, 2, 8.15577505002846, 60, '../../content/toolpictures/HF_COMPLEX_2.jpg', 8, getdate(), 1, 1),
	   (28, 6, 3, 17.5618172883841, 110, '../../content/toolpictures/HF_COMPLEX_3.jpg', 8, getdate(), 1, 1),
	   (29, 6, 4, 46.2651776897771, 182.5, '../../content/toolpictures/HF_COMPLEX_4.jpg', 8, getdate(), 1, 1),
	   (30, 6, 5, 232.437315051976, 443.5, '../../content/toolpictures/HF_COMPLEX_5.jpg', 8, getdate(), 1, 1),
	   -- LM
	   (31, 7, 1, 4.922228733422, 92, '../../content/toolpictures/LM_COMPLEX_1.jpg', 8, getdate(), 1, 1),
	   (32, 7, 2, 4.18364529916242, 107, '../../content/toolpictures/LM_COMPLEX_2.jpg', 8, getdate(), 1, 1),
	   (33, 7, 3, 12.4091592497605, 147, '../../content/toolpictures/LM_COMPLEX_3.jpg', 8, getdate(), 1, 1),
	   (34, 7, 4, 20.6365917820232, 220, '../../content/toolpictures/LM_COMPLEX_4.jpg', 8, getdate(), 1, 1),
	   (35, 7, 5, 185.857048972903, 352.4, '../../content/toolpictures/LM_COMPLEX_5.jpg', 8, getdate(), 1, 1),
	   -- ME
	   (36, 8, 1, 7.15250107339644, 64, '../../content/toolpictures/ME_COMPLEX_1.jpg', 8, getdate(), 1, 1),
	   (37, 8, 2, 7.86299879180965, 87, '../../content/toolpictures/ME_COMPLEX_2.jpg', 8, getdate(), 1, 1),
	   (38, 8, 3, 10.8630179152871, 133.1, '../../content/toolpictures/ME_COMPLEX_3.jpg', 8, getdate(), 1, 1),
	   (39, 8, 4, 29.8617990101647, 230.6, '../../content/toolpictures/ME_COMPLEX_4.jpg', 8, getdate(), 1, 1),
	   (40, 8, 5, 255.633485032288, 441.5, '../../content/toolpictures/ME_COMPLEX_5.jpg', 8, getdate(), 1, 1),
	   -- NCMF
	   (41, 9, 1, 3.79473254867785, 51, '../../content/toolpictures/NCMF_COMPLEX_1.jpg', 8, getdate(), 1, 1),
	   (42, 9, 2, 4.29990402255383, 64, '../../content/toolpictures/NCMF_COMPLEX_2.jpg', 8, getdate(), 1, 1),
	   (43, 9, 3, 7.2468424347936, 100, '../../content/toolpictures/NCMF_COMPLEX_3.jpg', 8, getdate(), 1, 1),
	   (44, 9, 4, 22.4185441838028, 152.2, '../../content/toolpictures/NCMF_COMPLEX_4.jpg', 8, getdate(), 1, 1),
	   (45, 9, 5, 83.6387221111482, 272.5, '../../content/toolpictures/NCMF_COMPLEX_5.jpg', 8, getdate(), 1, 1),
	   -- OHME
	   (46, 10, 1, 8.29075517777469, 82, '../../content/toolpictures/OHME_COMPLEX_1.jpg', 8, getdate(), 1, 1),
	   (47, 10, 2, 7.80108932912355, 103, '../../content/toolpictures/OHME_COMPLEX_2.jpg', 8, getdate(), 1, 1),
	   (48, 10, 3, 11.8823396782478, 159, '../../content/toolpictures/OHME_COMPLEX_3.jpg', 8, getdate(), 1, 1),
	   (49, 10, 4, 39.8097633113029, 268, '../../content/toolpictures/OHME_COMPLEX_4.jpg', 8, getdate(), 1, 1),
	   (50, 10, 5, 202.493149689543, 526, '../../content/toolpictures/OHME_COMPLEX_5.jpg', 8, getdate(), 1, 1),
	   -- PME
	   (51, 11, 1, 12.9124476513102, 96, '../../content/toolpictures/PME_COMPLEX_1.jpg', 8, getdate(), 1, 1),
	   (52, 11, 2, 10.6080975360021, 123.2, '../../content/toolpictures/PME_COMPLEX_2.jpg', 8, getdate(), 1, 1),
	   (53, 11, 3, 24.0755062252074, 233.5, '../../content/toolpictures/PME_COMPLEX_3.jpg', 8, getdate(), 1, 1),
	   (54, 11, 4, 57.8082038583912, 372.5, '../../content/toolpictures/PME_COMPLEX_4.jpg', 8, getdate(), 1, 1),
	   (55, 11, 5, 316.506597654292, 720.5, '../../content/toolpictures/PME_COMPLEX_5.jpg', 8, getdate(), 1, 1),
	   -- SME
	   (56, 12, 1, 11.418969180397, 143, '../../content/toolpictures/SME_COMPLEX_1.jpg', 8, getdate(), 1, 1),
	   (57, 12, 2, 20.0024165206776, 170, '../../content/toolpictures/SME_COMPLEX_2.jpg', 8, getdate(), 1, 1),
	   (58, 12, 3, 33.9815916553149, 259, '../../content/toolpictures/SME_COMPLEX_3.jpg', 8, getdate(), 1, 1),
	   (59, 12, 4, 97.734073114851, 512.5, '../../content/toolpictures/SME_COMPLEX_4.jpg', 8, getdate(), 1, 1),
	   (60, 12, 5, 441.083446375702, 707.3, '../../content/toolpictures/SME_COMPLEX_5.jpg', 8, getdate(), 1, 1);
	   set identity_insert ToolApp.dbo.Statistic off;

-- Insert Test data for Access Level
set identity_insert ToolApp.dbo.AccessLevel on;

insert into ToolApp.dbo.AccessLevel (AccessLevelID, AccessLevelName, AccessLevelDescription, CreatedByUserID)
values (1, 'Admin', 'All Permissions granted', 1);

set identity_insert ToolApp.dbo.AccessLevel off;

-- Insert Test data for FamilyClass table
set identity_insert ToolApp.dbo.FamilyClass on;

insert into ToolApp.dbo.FamilyClass (FamilyClassID, FamilyClassName, FamilyClassDescription, CreatedByUserID)
values (1, 'Initial Design', 'This estimate assumes this is a 1 off tool, or the first tool in a family of similar tools. Note: "Make similar to" is not considered part of a tool family if that tool was designed more than 2 years prior.', 1)
, (2, 'Second Tool in Family', 'This estimate assumes this is the second tool in a family of similar tools.', 1)
, (3, 'Additional Tools in Tool Family', 'This estimate assumes this is the third tool and beyond in a family of similar tools.', 1)
, (4, 'KBE Development Tool', 'This estimate assumes the additional hours required to develop a KBE template / seed model to handle a package of similar tools.  This estimate should be applied to the tool that will act as the representative for the family.', 1)
, (5, 'KBE Development (NSAR)', 'This estimate assumes the effort to design all follow on tooling after the KBE development tool has been created.  Stress effort is assumed to be NSAR.  Drawing checks only.', 1)
, (6, 'KBE Development (NSRR)', 'This estimate assumes the effort to design all follow on tooling after the KBE development tool has been created.  Stress effort is assumed to be NSRR. No stress hours required.', 1);

set identity_insert ToolApp.dbo.FamilyClass off;

-- Insert Test data for StressWorkType table
set identity_insert ToolApp.dbo.StressWorkType on;

insert into ToolApp.dbo.StressWorkType (StressWorkTypeID, StressWorkTypeName, StressWorkTypeDescription, CreatedByUserID)
values (1, 'NSRR', 'NSRR Description', 1)
	,(2, 'NSAR', 'NSAR Description', 1)
	,(3, 'Stress Analysis Required', 'Stress Required Description', 1);

set identity_insert ToolApp.dbo.StressWorkType off;

-- Insert Test data for DesignOrder table
set identity_insert ToolApp.dbo.DesignOrder on;

insert into ToolApp.dbo.DesignOrder (DesignOrderSID, DescriptiveName, DesignOrderNumber, 
										ToolTypeID, [PartNumber], IsCompleted, CreatedByUserID)
values (1, 'First Design Order', 455654, 1, '12345dbk', 0, 1)
	,(2, 'Second Design Order', 445665, 1, '12345dbk', 0, 1)
	,(3, 'Third Design Order', 445666, 1, '12345dbk', 0, 1);

set identity_insert ToolApp.dbo.DesignOrder off;

-- Insert Test data for Estimate Table
set identity_insert ToolApp.dbo.Estimate on;

insert into ToolApp.dbo.Estimate (EstimateID, DesignOrderSID, NeedsSurfacing, IsStressIncluded, StressWorkTypeID, 
	ComplexityLevel, EngineeringReleased, FamilyClassID, HoursEstimate,DesignHour, StressHour,CheckHour, ReleaseHour,DesignFlow, KBEBreakEvenNum, 
	IsLatestEstimate, Comment, ReasonForEstimateChange, CreatedByUserID)
values (1, 1, 1, 0, 1, 1, 1, 4, 56, 1,1,1,1,10,1.76, 0, 'Part of Design Order 1, Estimate 1', 'Original Estimate', 1)
		,(2, 1, 1, 0, 1, 1, 1, 1, 56, 2,2,2,2,11,null, 1, 'Part of Design Order 1, Estimate 2', 'Second Estimate', 1)
		,(3, 2, 1, 0, 1, 1, 1, 1, 56, 3,3,3,3,12,null, 0, 'Part of Design Order 2, Estimate 1', 'Original Estimate', 1)
		,(4, 2, 1, 0, 1, 1, 1, 1, 56, 4,4,4,4,13,null, 1, 'Part of Design Order 2, Estimate 2', 'Second Estimate', 1)
		,(5, 3, 1, 0, 1, 1, 1, 1, 56, 5,5,5,5,14,null, 0, 'Part of Design Order 3, Estimate 1', 'Original Estimate', 1)
		,(6, 3, 1, 0, 1, 1, 1, 1, 56, 6,6,6,6,15,null, 1, 'Part of Design Order 3, Estimate 2', 'Second Estimate', 1);

set identity_insert ToolApp.dbo.Estimate off;
