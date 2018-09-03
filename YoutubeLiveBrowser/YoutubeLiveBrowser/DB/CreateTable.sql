CREATE TABLE Channels(
	ChannelId	nvarchar(30) NOT NULL PRIMARY KEY,
	ChannelName	nvarchar(30)
);

CREATE TABLE YoutubeLiveStreamInfo(
	VidelId				nvarchar(30) NOT NULL PRIMARY KEY,
	ChannelID			nvarchar(30) NOT NULL,
	LiveChatId			nvarchar(30) NOT NULL,
	LivaStreamStartDate	Datetime,
	LivaStreamEndDate	Datetime,
	LivaStreamTimeSpan	int
);

CREATE TABLE YoutubeLiveComment(
	VidelId				nvarchar(30) NOT NULL,
	ChannelID			nvarchar(30) NOT NULL,
	CommentId			nvarchar(30) NOT NULL,
	PublishedAt			Datetime,
	DisplayName			nvarchar(100),
	IsChatOwner			Bit,
	IsChatSponsor		Bit,
	IsChatModerator		Bit,
	CONSTRAINT PK_YoutubeLiveComment PRIMARY KEY CLUSTERED (VidelId,ChannelID,CommentId) 
);
