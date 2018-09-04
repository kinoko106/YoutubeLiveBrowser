CREATE TABLE Channels(
	ChannelId	nvarchar(30) NOT NULL PRIMARY KEY,
	ChannelName	nvarchar(30)
);

CREATE TABLE YoutubeLiveStreamInfo(
	VideoId				nvarchar(30) NOT NULL PRIMARY KEY,
	ChannelId			nvarchar(30) NOT NULL,
	LiveChatId			nvarchar(30) NOT NULL,
	LiveStreamStartDate	Datetime,
	LiveStreamEndDate	Datetime,
	LiveStreamTimeSpan	int
);

CREATE TABLE YoutubeLiveComment(
	VideoId				nvarchar(30) NOT NULL,
	LiveChatId			nvarchar(30) NOT NULL,
	CommentId			nvarchar(30) NOT NULL,
	PublishedAt			Datetime,
	DisplayName			nvarchar(100),
	DisplayMessage		nvarchar(100),
	IsChatOwner			Bit,
	IsChatSponsor		Bit,
	IsChatModerator		Bit,
	CONSTRAINT PK_YoutubeLiveComment PRIMARY KEY CLUSTERED (VideoId,LiveChatId,CommentId) 
);
