using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Twitter
{
	public class Parser
	{
		private static string iconUrl = "http://api.twitter.com/1/users/profile_image/";

		public static string GetIconURL(string userID)
		{
			return iconUrl + userID;
		}

		public static string ParseImgurResponse(string text)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(text);
			XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("original");
			if (elementsByTagName.Count > 0)
			{
				return elementsByTagName[0].InnerText;
			}
			Debug.LogWarning("Could not parse the dang imgur response..." + text);
			return string.Empty;
		}

		public static List<PhoneMail> ParseAtomToMail(string atomtext)
		{
			List<PhoneMail> list = new List<PhoneMail>();
			atomtext = atomtext.Replace("georss:point>", "point>");
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(atomtext);
			XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("entry");
			foreach (XmlNode item2 in elementsByTagName)
			{
				PhoneMail item = AtomTweetToMail(item2);
				list.Add(item);
			}
			return list;
		}

		public static PhoneMail AtomTweetToMail(XmlNode node)
		{
			PhoneMail phoneMail = new PhoneMail();
			string innerText = node["id"].InnerText;
			string[] array = innerText.Split(':');
			innerText = array[array.Length - 1];
			phoneMail.id = innerText;
			string innerText2 = node["title"].InnerText;
			phoneMail.body = innerText2;
			string innerText3 = node["author"]["name"].InnerText;
			phoneMail.sender = innerText3;
			phoneMail.subject = string.Empty;
			if (node["twitter:geo"] != null && node["twitter:geo"]["point"] != null)
			{
				string innerText4 = node["twitter:geo"]["point"].InnerText;
				//phoneMail.position = PlaytomicController.TranslateGPSStringToPos(innerText4);
				Debug.Log(phoneMail.position);
			}
			return phoneMail;
		}

		public static List<PhoneMail> ParseToMail(string text)
		{
			List<PhoneMail> list = new List<PhoneMail>();
			XmlDocument xmlDocument = new XmlDocument();
			text = text.Replace("georss:point>", "point>");
			xmlDocument.LoadXml(text);
			XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("status");
			foreach (XmlNode item2 in elementsByTagName)
			{
				PhoneMail item = TweetToMail(item2);
				list.Add(item);
			}
			return list;
		}

		public static PhoneMail TweetToMail(XmlNode node)
		{
			string innerText = node["created_at"].InnerText;
			if (node["retweeted_status"] != null)
			{
				node = node["retweeted_status"];
			}
			string innerText2 = node["id"].InnerText;
			PhoneMail phoneMail = MailController.FindMail("tw_" + innerText2);
			if (phoneMail != null)
			{
				return phoneMail;
			}
			phoneMail = new PhoneMail();
			phoneMail.id = "tw_" + innerText2;
			ulong result;
			if (ulong.TryParse(innerText2, out result))
			{
				phoneMail.id_number = result;
			}
			else
			{
				phoneMail.id_number = 0uL;
				Debug.LogWarning("could not parse the id: " + innerText2);
			}
			string innerText3 = node["text"].InnerText;
			phoneMail.body = UrlDecode(innerText3);
			phoneMail.body = phoneMail.body.Replace('\n', ' ');
			phoneMail.sender = node["user"]["name"].InnerText;
			phoneMail.image_url = node["user"]["profile_image_url"].InnerText;
			phoneMail.subject = "@" + node["user"]["screen_name"].InnerXml;
			phoneMail.time = TweetTimeToDateTime(innerText);
			if (node["entities"] != null)
			{
				XmlNode xmlNode = node["entities"];
				if (xmlNode["media"] != null)
				{
					XmlNode xmlNode2 = xmlNode["media"];
					if (xmlNode2.ChildNodes.Count > 0)
					{
						XmlNode xmlNode3 = xmlNode2.ChildNodes[0];
						if (xmlNode3["media_url"] != null)
						{
							string item = xmlNode3["media_url"].InnerText + ":small";
							phoneMail.media_urls.Add(item);
						}
					}
				}
				if (xmlNode["urls"] != null)
				{
					XmlNode xmlNode4 = xmlNode["urls"];
					foreach (XmlNode childNode in xmlNode4.ChildNodes)
					{
						if (childNode["expanded_url"] != null)
						{
							string text = childNode["display_url"].InnerText;
							if (text.Length > 25)
							{
								text = text.Remove(22) + "...";
							}
							phoneMail.body = phoneMail.body.Replace(childNode["url"].InnerText, text);
							string innerText4 = childNode["expanded_url"].InnerText;
							if (innerText4.EndsWith(".jpg") || innerText4.EndsWith(".png"))
							{
								phoneMail.media_urls.Add(innerText4);
							}
							else if (innerText4.StartsWith("http://instagram.com/p/") || innerText4.StartsWith("http://instagr.am/p/"))
							{
								phoneMail.media_urls.Add(innerText4 + "media");
							}
							else if (innerText4.StartsWith("http://yfrog.com/"))
							{
								phoneMail.media_urls.Add(innerText4 + ":iphone");
							}
							else if (innerText4.StartsWith("http://twitpic.com/"))
							{
								phoneMail.media_urls.Add(innerText4.Replace("http://twitpic.com/", "http://twitpic.com/show/iphone/"));
							}
							else
							{
								phoneMail.link_urls.Add(innerText4);
							}
						}
					}
				}
			}
			XmlElement xmlElement = node["geo"];
			MailController.AddMail(phoneMail);
			return phoneMail;
		}

		public static DateTime TweetTimeToDateTime(string timestring)
		{
			string[] array = timestring.Split(' ');
			int month;
			switch (array[1])
			{
			case "Jan":
				month = 1;
				break;
			case "Feb":
				month = 2;
				break;
			case "Mar":
				month = 3;
				break;
			case "Apr":
				month = 4;
				break;
			case "May":
				month = 5;
				break;
			case "Jun":
				month = 6;
				break;
			case "Jul":
				month = 7;
				break;
			case "Aug":
				month = 8;
				break;
			case "Sep":
				month = 9;
				break;
			case "Oct":
				month = 10;
				break;
			case "Nov":
				month = 11;
				break;
			case "Dec":
				month = 12;
				break;
			default:
				month = 1;
				break;
			}
			int result;
			if (!int.TryParse(array[2], out result))
			{
				Debug.LogWarning("Could not parse tweet day: " + array[2]);
				result = 1;
			}
			int result2;
			if (!int.TryParse(array[5], out result2))
			{
				Debug.LogWarning("Could not parse tweet year: " + array[5]);
				result2 = 2012;
			}
			string[] array2 = array[3].Split(':');
			int result3;
			int.TryParse(array2[0], out result3);
			int result4;
			int.TryParse(array2[1], out result4);
			int result5;
			int.TryParse(array2[2], out result5);
			int num = 5;
			DateTime result6 = new DateTime(result2, month, result, result3, result4, result5).AddHours(-num);
			if (DateTime.Now.IsDaylightSavingTime())
			{
				result6 = result6.AddHours(1.0);
			}
			return result6;
		}

		public static string UrlDecode(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return string.Empty;
			}
			value = Uri.UnescapeDataString(value);
			value = value.Replace("&quot;", "\"").Replace("&lt;", "<").Replace("&gt;", ">")
				.Replace("&amp;", "&")
				.Replace("&apos;", "'");
			return value;
		}
	}
}
