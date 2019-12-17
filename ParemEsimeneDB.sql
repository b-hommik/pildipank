alter table aspnetusers add FirstName sysname null
alter table aspnetusers add LaststName sysname null
alter table aspnetusers add BirthDate datetime null -- sa peaks oma kasutajate vanust kontrollima

-- kasutajad ONGI sul selles andmebaais tabelis AspNetUsers


create table DataFile
(
	Id int identity primary key,
	Name sysname not null,
	ContentType nvarchar(128) not null,
	Content varbinary(max)
)

create table UserPicture -- võimlaus kasutajale näopilt lisada
(
	UserId nvarchar(128) not null references aspnetusers(Id),
	FileId int not null references Datafile(Id)
)


create table Category
(
	Id int identity primary key,
	OrderId int not null default(0), -- ability to rearrange
	Name nvarchar(64) not null,
	Description nvarchar(max) null,
	UserId nvarchar(128) null references aspnetusers(Id), -- kas isiklik (kell) või üldine (null) kategooria
														  -- kasutaja saab oma kategooriaid lisada 
														  -- admin saab üldiseid kategooriaid lisada
)

create table Album
(
	Id int identity primary key,
	OrderId int not null default(0), -- ability to rearrange albums in list
	UserId nvarchar(128) not null references aspnetusers(Id), 
	Name nvarchar(64),
	Description nvarchar(max) null,
	Visibility bit not null default(1),		-- (1) true - visible for all users (0) - false - visible only for owner 
	CoverPictureId int null					-- PictureId - viide pildile, mis kaanele
)

create table Picture
(
	Id int identity primary key,
	UserId nvarchar(128) not null references aspnetusers(Id), 
	FileId int not null references Datafile(Id),
	Name nvarchar(128) null,
	Description nvarchar(max) null,
	Created datetime not null default(getdate()),
	Visibility bit not null default(1)		-- see Album visibility

)

create table Likes
(
	Id int identity primary key,
	PictureId int not null references Picture(Id),
	LikerId nvarchar(128) not null references aspnetusers(Id),
	Number int not null default(0)			-- for 5 star likes (1..5) or likes/dislikes (0,1) - future usage
)
