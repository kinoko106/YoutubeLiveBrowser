﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeLiveBrowser.Models
{
	class YoutubeLive
	{
		public static string RequestBase = "https://www.googleapis.com/youtube/v3";
		public static string APIPath = "https://accounts.google.com/o/oauth2/v2/auth?response_type=code";
		public static string ClientId = "681645994603-504ct38mbada3ltl3n8oe7g9bja6chnc.apps.googleusercontent.com";//自分で生成したやつ
		public static string ClientSecert = "iEk0qX2un9EKnee4y6DvXmlz";//自分で生成したやつ
		public static string RequestUrl = "http://localhost:8080";
		public static string TokenUrl = "https://www.googleapis.com/oauth2/v4/token";
		public static string APIKey = "";//自分で生成したやつ

		//partに指定するパラメータのリスト
		public enum YoutubePartParameters
		{
			id,
			snippet,
			contentDetails,
			fileDetails,
			liveStreamingDetails,
			player,
			processingDetails,
			recordingDetails,
			statistics,
			status,
			suggestions,
			topicDetails,
			authorDetails
		}
	}
}
