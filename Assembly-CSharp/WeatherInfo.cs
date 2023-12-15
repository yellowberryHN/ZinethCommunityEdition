using System.Xml;
using UnityEngine;

public class WeatherInfo
{
	public string day_name;

	public string condition;

	public string temp_f;

	public string humidity;

	public string icon_url;

	public string wind_conditions;

	public string low;

	public string high;

	private WeatherInfo()
	{
	}

	public WeatherInfo(XmlNode node)
	{
		condition = GetData(node, "condition");
		temp_f = GetData(node, "temp_f");
		humidity = GetData(node, "humidity");
		wind_conditions = GetData(node, "wind_condition");
	}

	private string GetData(XmlNode node, string str)
	{
		Debug.Log(str);
		return node[str].GetAttribute("data");
	}
}
