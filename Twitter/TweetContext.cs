using System.Collections.Generic;
using UnityEngine;

namespace Twitter
{
	public class TweetContext
	{
		public string text;

		public Texture2D texture;

		public string textureURL = string.Empty;

		public string replyTo = string.Empty;

		public List<string> mentions = new List<string>();

		public string posLat = string.Empty;

		public string posLong = string.Empty;
	}
}
