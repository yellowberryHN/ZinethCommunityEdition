using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using HTTP;
using UnityEngine;

namespace Twitter
{
	public class API
	{
		private static readonly string RequestTokenURL = "https://api.twitter.com/oauth/request_token?oauth_callback=oob";

		private static readonly string AuthorizationURL = "http://api.twitter.com/oauth/authorize?oauth_token={0}";

		private static readonly string AccessTokenURL = "https://api.twitter.com/oauth/access_token";

		private static readonly string PostTweetURL = "http://api.twitter.com/1/statuses/update.xml?status={0}&include_entities=true";

		private static readonly string GetSearchResultsURL = "http://search.twitter.com/search.atom";

		private static readonly string GetSingleTweetURL = "http://api.twitter.com/1/statuses/show.xml?id={0}&include_entities=true";

		private static readonly string GetMentionsURL = "http://api.twitter.com/1/statuses/mentions.xml";

		private static readonly string GetTimeLineURL = "http://api.twitter.com/1/statuses/home_timeline.xml";

		private static readonly string imgur_key = "7578d06f2ac1c6e904c4775db47b2984";

		private static readonly string imgur_url = "http://api.imgur.com/2/upload.xml";

		private static readonly string[] OAuthParametersToIncludeInHeader = new string[7] { "oauth_version", "oauth_nonce", "oauth_timestamp", "oauth_signature_method", "oauth_consumer_key", "oauth_token", "oauth_verifier" };

		private static readonly string[] SecretParameters = new string[3] { "oauth_consumer_secret", "oauth_token_secret", "oauth_signature" };

		public static IEnumerator GetRequestToken(string consumerKey, string consumerSecret, RequestTokenCallback callback)
		{
			WWW web = WWWRequestToken(consumerKey, consumerSecret);
			yield return web;
			if (!string.IsNullOrEmpty(web.error))
			{
				Debug.Log(string.Format("GetRequestToken - failed. error : {0}", web.error));
				callback(false, null);
				yield break;
			}
			RequestTokenResponse response = new RequestTokenResponse
			{
				Token = Regex.Match(web.text, "oauth_token=([^&]+)").Groups[1].Value,
				TokenSecret = Regex.Match(web.text, "oauth_token_secret=([^&]+)").Groups[1].Value
			};
			if (!string.IsNullOrEmpty(response.Token) && !string.IsNullOrEmpty(response.TokenSecret))
			{
				callback(true, response);
				yield break;
			}
			Debug.Log(string.Format("GetRequestToken - failed. response : {0}", web.text));
			callback(false, null);
		}

		public static void OpenAuthorizationPage(string requestToken)
		{
			Application.OpenURL(string.Format(AuthorizationURL, requestToken));
		}

		public static IEnumerator GetAccessToken(string consumerKey, string consumerSecret, string requestToken, string pin, AccessTokenCallback callback)
		{
			WWW web = WWWAccessToken(consumerKey, consumerSecret, requestToken, pin);
			yield return web;
			if (!string.IsNullOrEmpty(web.error))
			{
				Debug.Log(string.Format("GetAccessToken - failed. error : {0}", web.error));
				callback(false, null);
				yield break;
			}
			AccessTokenResponse response = new AccessTokenResponse
			{
				Token = Regex.Match(web.text, "oauth_token=([^&]+)").Groups[1].Value,
				TokenSecret = Regex.Match(web.text, "oauth_token_secret=([^&]+)").Groups[1].Value,
				UserId = Regex.Match(web.text, "user_id=([^&]+)").Groups[1].Value,
				ScreenName = Regex.Match(web.text, "screen_name=([^&]+)").Groups[1].Value
			};
			if (!string.IsNullOrEmpty(response.Token) && !string.IsNullOrEmpty(response.TokenSecret) && !string.IsNullOrEmpty(response.UserId) && !string.IsNullOrEmpty(response.ScreenName))
			{
				callback(true, response);
				yield break;
			}
			Debug.Log(string.Format("GetAccessToken - failed. response : {0}", web.text));
			callback(false, null);
		}

		private static WWW WWWRequestToken(string consumerKey, string consumerSecret)
		{
			byte[] postData = new byte[1] { 0 };
			Hashtable hashtable = new Hashtable();
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			AddDefaultOAuthParams(dictionary, consumerKey, consumerSecret);
			dictionary.Add("oauth_callback", "oob");
			hashtable["Authorization"] = GetFinalOAuthHeader("POST", RequestTokenURL, dictionary);
			return new WWW(RequestTokenURL, postData, hashtable);
		}

		private static WWW WWWAccessToken(string consumerKey, string consumerSecret, string requestToken, string pin)
		{
			byte[] postData = new byte[1] { 0 };
			Hashtable hashtable = new Hashtable();
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			AddDefaultOAuthParams(dictionary, consumerKey, consumerSecret);
			dictionary.Add("oauth_token", requestToken);
			dictionary.Add("oauth_verifier", pin);
			hashtable["Authorization"] = GetFinalOAuthHeader("POST", AccessTokenURL, dictionary);
			return new WWW(AccessTokenURL, postData, hashtable);
		}

		private static string GetHeaderWithAccessToken(string httpRequestType, string apiURL, string consumerKey, string consumerSecret, AccessTokenResponse response, Dictionary<string, string> parameters)
		{
			AddDefaultOAuthParams(parameters, consumerKey, consumerSecret);
			parameters.Add("oauth_token", response.Token);
			parameters.Add("oauth_token_secret", response.TokenSecret);
			return GetFinalOAuthHeader(httpRequestType, apiURL, parameters);
		}

		public static IEnumerator PostTweet(string text, string consumerKey, string consumerSecret, AccessTokenResponse response, PostTweetCallback callback)
		{
			string pos_lat = string.Empty;
			string pos_long = string.Empty;
			if (text.StartsWith("pos:"))
			{
				int startind2 = text.IndexOf(":") + 1;
				int endind2 = text.IndexOf(";");
				string ttext = text.Substring(startind2, endind2 - startind2);
				text = text.Substring(endind2 + 1);
				pos_lat = ttext.Split('/')[0];
				pos_long = ttext.Split('/')[1];
			}
			string replyto = string.Empty;
			if (text.StartsWith("replyto:"))
			{
				int startind = text.IndexOf(":") + 1;
				int endind = text.IndexOf(";");
				replyto = text.Substring(startind, endind - startind);
				text = text.Substring(endind + 1);
			}
			if (text.Length > 140)
			{
				Debug.LogWarning("tweet too long... cutting it off");
				text = text.Substring(0, 140);
			}
			if (string.IsNullOrEmpty(text) || text.Length > 140)
			{
				Debug.Log(string.Format("PostTweet - text[{0}] is empty or too long.", text));
				callback(false, string.Empty);
				yield break;
			}
			Dictionary<string, string> parameters = new Dictionary<string, string>();
			string url = string.Format(PostTweetURL, UrlEncode(text));
			parameters.Add("status", text);
			parameters.Add("include_entities", "true");
			if (replyto != string.Empty)
			{
				string turl3 = url + "&in_reply_to_status_id={0}";
				url = string.Format(turl3, UrlEncode(replyto));
				parameters.Add("in_reply_to_status_id", replyto);
			}
			if (pos_lat != string.Empty && pos_long != string.Empty)
			{
				string turl2 = url + "&lat={0}&long={1}&display_coordinates=true";
				url = string.Format(turl2, UrlEncode(pos_lat), UrlEncode(pos_long));
				parameters.Add("display_coordinates", "true");
				parameters.Add("lat", pos_lat);
				parameters.Add("long", pos_long);
			}
			else if (TwitterDemo.has_coord_pos)
			{
				string turl = url + "&lat={0}&long={1}&display_coordinates=true";
				url = string.Format(turl, UrlEncode(TwitterDemo.pos_lat), UrlEncode(TwitterDemo.pos_long));
				parameters.Add("display_coordinates", "true");
				parameters.Add("lat", TwitterDemo.pos_lat);
				parameters.Add("long", TwitterDemo.pos_long);
			}
			byte[] dummmy = new byte[1] { 0 };
			Hashtable headers = new Hashtable();
			headers["Authorization"] = GetHeaderWithAccessToken("POST", url, consumerKey, consumerSecret, response, parameters);
			WWW web = new WWW(url, dummmy, headers);
			yield return web;
			if (!string.IsNullOrEmpty(web.error))
			{
				Debug.Log(string.Format("PostTweet - failed. {0}", web.error));
				callback(false, string.Empty);
				yield break;
			}
			string error = Regex.Match(web.text, "<error>([^&]+)</error>").Groups[1].Value;
			if (!string.IsNullOrEmpty(error))
			{
				Debug.Log(string.Format("PostTweet - failed. {0}", error));
				callback(false, web.text);
			}
			else
			{
				callback(true, web.text);
			}
		}

		public static IEnumerator GetSearchResults(string search, GetMentionsCallback callback)
		{
			Dictionary<string, string> parameters = new Dictionary<string, string>();
			string url3 = GetSearchResultsURL;
			url3 = url3 + "?q=" + UrlEncode(search);
			url3 += "&include_entities=true";
			Debug.Log(url3);
			Request req = new Request("get", url3);
			req.Send();
			while (!req.isDone)
			{
				yield return null;
			}
			yield return null;
			Response res = req.response;
			if (res == null)
			{
				Debug.LogWarning("Hey! The twitter response is null!");
			}
			else if (res.status != 200)
			{
				Debug.Log("Twitter Search - failed - " + res.message + " - " + res.status);
				callback(false, null);
			}
			else
			{
				callback(true, res.Text);
			}
		}

		public static IEnumerator GetSingleTweet(string tweet_id, GetMentionsCallback callback)
		{
			Dictionary<string, string> parameters = new Dictionary<string, string>();
			string url2 = GetSingleTweetURL;
			url2 = string.Format(url2, UrlEncode(tweet_id));
			Debug.Log(url2);
			Request req = new Request("get", url2);
			req.Send();
			while (!req.isDone)
			{
				yield return null;
			}
			yield return null;
			Response res = req.response;
			if (res == null)
			{
				Debug.LogWarning("Hey! The twitter response is null!");
			}
			else if (res.status != 200)
			{
				Debug.Log("Twitter Search - failed - " + res.message + " - " + res.status);
				callback(false, null);
			}
			else
			{
				callback(true, res.Text);
			}
		}

		public static IEnumerator GetMentions(string consumerKey, string consumerSecret, AccessTokenResponse response, GetMentionsCallback callback)
		{
			return GenericGetTweets(GetMentionsURL, consumerKey, consumerSecret, response, callback);
		}

		public static IEnumerator GetTimeLine(string consumerKey, string consumerSecret, AccessTokenResponse response, GetMentionsCallback callback)
		{
			return GenericGetTweets(GetTimeLineURL, consumerKey, consumerSecret, response, callback);
		}

		public static IEnumerator GenericGetTweets(string url, string consumerKey, string consumerSecret, AccessTokenResponse response, GetMentionsCallback callback)
		{
			Dictionary<string, string> parameters = new Dictionary<string, string>();
			url = ((!url.Contains("?")) ? (url + "?") : (url + "&"));
			url += "include_entities=true";
			parameters.Add("include_entities", "true");
			if (TwitterDemo.newest_id != 0 && !url.StartsWith(GetMentionsURL))
			{
				string id_str = UrlEncode(TwitterDemo.newest_id.ToString("F0"));
				url = url + "&since_id=" + id_str;
				parameters.Add("since_id", id_str);
			}
			Request req = new Request("get", url);
			req.AddHeader("Authorization", GetHeaderWithAccessToken("GET", url, consumerKey, consumerSecret, response, parameters));
			req.Send();
			while (!req.isDone)
			{
				yield return null;
			}
			yield return null;
			Response res = req.response;
			if (res == null)
			{
				Debug.LogWarning("Hey! The twitter response is null!");
			}
			else if (res.status != 200)
			{
				Debug.Log("GetMentions - failed - " + res.message + " - " + res.status);
				callback(false, null);
			}
			else
			{
				callback(true, res.Text);
			}
		}

		public static IEnumerator UploadImage(GetMentionsCallback callback)
		{
			yield return new WaitForEndOfFrame();
			int width = Screen.width;
			int height = Screen.height;
			Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
			tex.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
			tex.Apply();
			byte[] bytes = tex.EncodeToPNG();
			yield return null;
			UnityEngine.Object.Destroy(tex);
			WWWForm form = new WWWForm();
			form.AddField("key", imgur_key);
			form.AddBinaryData("image", bytes, "test.png");
			WWW web = new WWW(imgur_url, form);
			yield return web;
			if (web.error != null)
			{
				callback(false, web.error);
			}
			else
			{
				callback(true, web.text);
			}
		}

		public static IEnumerator UploadImage(Texture2D tex, GetMentionsCallback callback)
		{
			byte[] bytes = tex.EncodeToPNG();
			yield return null;
			UnityEngine.Object.Destroy(tex);
			WWWForm form = new WWWForm();
			form.AddField("key", imgur_key);
			form.AddBinaryData("image", bytes, "test.png");
			WWW web = new WWW(imgur_url, form);
			yield return web;
			if (web.error != null)
			{
				callback(false, web.error);
			}
			else
			{
				callback(true, web.text);
			}
		}

		public static IEnumerator UploadImage(TweetContext tweet, TweetContextCallback callback)
		{
			if (tweet.texture == null)
			{
				int width = Screen.width;
				int height = Screen.height;
				yield return new WaitForEndOfFrame();
				tweet.texture = new Texture2D(width, height, TextureFormat.RGB24, false);
				tweet.texture.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
				tweet.texture.Apply();
				List<NetPlayer> players = PhoneCamControl.GetPlayersInView(Camera.main);
				foreach (NetPlayer player in players)
				{
					string nam = player.userName;
					if (nam.Contains(" "))
					{
						nam = "gamsfest";
					}
					if (!tweet.mentions.Contains(nam))
					{
						tweet.mentions.Add(nam);
					}
				}
				yield return null;
			}
			byte[] bytes = tweet.texture.EncodeToPNG();
			yield return null;
			UnityEngine.Object.Destroy(tweet.texture);
			WWWForm form = new WWWForm();
			form.AddField("key", imgur_key);
			form.AddBinaryData("image", bytes, "test.png");
			WWW web = new WWW(imgur_url, form);
			yield return web;
			if (web.error != null)
			{
				callback(false, web.error, tweet);
			}
			else
			{
				callback(true, web.text, tweet);
			}
		}

		private static void AddDefaultOAuthParams(Dictionary<string, string> parameters, string consumerKey, string consumerSecret)
		{
			parameters.Add("oauth_version", "1.0");
			parameters.Add("oauth_nonce", GenerateNonce());
			parameters.Add("oauth_timestamp", GenerateTimeStamp());
			parameters.Add("oauth_signature_method", "HMAC-SHA1");
			parameters.Add("oauth_consumer_key", consumerKey);
			parameters.Add("oauth_consumer_secret", consumerSecret);
		}

		private static string GetFinalOAuthHeader(string HTTPRequestType, string URL, Dictionary<string, string> parameters)
		{
			string value = GenerateSignature(HTTPRequestType, URL, parameters);
			parameters.Add("oauth_signature", value);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("OAuth realm=\"{0}\"", "Twitter API");
			IOrderedEnumerable<KeyValuePair<string, string>> orderedEnumerable = from p in parameters
				where OAuthParametersToIncludeInHeader.Contains(p.Key)
				orderby p.Key, UrlEncode(p.Value)
				select p;
			foreach (KeyValuePair<string, string> item in orderedEnumerable)
			{
				stringBuilder.AppendFormat(",{0}=\"{1}\"", UrlEncode(item.Key), UrlEncode(item.Value));
			}
			stringBuilder.AppendFormat(",oauth_signature=\"{0}\"", UrlEncode(parameters["oauth_signature"]));
			return stringBuilder.ToString();
		}

		private static string GenerateSignature(string httpMethod, string url, Dictionary<string, string> parameters)
		{
			IEnumerable<KeyValuePair<string, string>> parameters2 = parameters.Where((KeyValuePair<string, string> p) => !SecretParameters.Contains(p.Key));
			string s = string.Format(CultureInfo.InvariantCulture, "{0}&{1}&{2}", httpMethod, UrlEncode(NormalizeUrl(new Uri(url))), UrlEncode(parameters2));
			string s2 = string.Format(CultureInfo.InvariantCulture, "{0}&{1}", UrlEncode(parameters["oauth_consumer_secret"]), (!parameters.ContainsKey("oauth_token_secret")) ? string.Empty : UrlEncode(parameters["oauth_token_secret"]));
			HMACSHA1 hMACSHA = new HMACSHA1(Encoding.ASCII.GetBytes(s2));
			byte[] inArray = hMACSHA.ComputeHash(Encoding.ASCII.GetBytes(s));
			return Convert.ToBase64String(inArray);
		}

		private static string GenerateTimeStamp()
		{
			return Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds, CultureInfo.CurrentCulture).ToString(CultureInfo.CurrentCulture);
		}

		private static string GenerateNonce()
		{
			return new System.Random().Next(123400, int.MaxValue).ToString("X", CultureInfo.InvariantCulture);
		}

		private static string NormalizeUrl(Uri url)
		{
			string text = string.Format(CultureInfo.InvariantCulture, "{0}://{1}", url.Scheme, url.Host);
			if ((!(url.Scheme == "http") || url.Port != 80) && (!(url.Scheme == "https") || url.Port != 443))
			{
				text = text + ":" + url.Port;
			}
			return text + url.AbsolutePath;
		}

		public static string UrlEncode(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return string.Empty;
			}
			value = Uri.EscapeDataString(value);
			value = Regex.Replace(value, "(%[0-9a-f][0-9a-f])", (Match c) => c.Value.ToUpper());
			value = value.Replace("(", "%28").Replace(")", "%29").Replace("$", "%24")
				.Replace("!", "%21")
				.Replace("*", "%2A")
				.Replace("'", "%27");
			value = value.Replace("%7E", "~");
			return value;
		}

		private static string UrlEncode(IEnumerable<KeyValuePair<string, string>> parameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			IOrderedEnumerable<KeyValuePair<string, string>> orderedEnumerable = from p in parameters
				orderby p.Key, p.Value
				select p;
			foreach (KeyValuePair<string, string> item in orderedEnumerable)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append("&");
				}
				stringBuilder.Append(string.Format(CultureInfo.InvariantCulture, "{0}={1}", UrlEncode(item.Key), UrlEncode(item.Value)));
			}
			return UrlEncode(stringBuilder.ToString());
		}
	}
}
