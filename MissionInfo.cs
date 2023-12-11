public class MissionInfo
{
	public string title = "Default Title";

	public string id = "zzzzz";

	public int status;

	public string description = "This describes the mission. The player will love to read this description.";

	public string introText = "This is the text that you see when receiving a mission.";

	public string outroText = "This is the text that you see when completing a mission.";

	public MissionInfo(string titletext, string idtext, int statustext, string descriptiontext, string introtext, string outrotext)
	{
		title = titletext;
		id = idtext;
		status = statustext;
		description = descriptiontext;
		introText = introtext;
		outroText = outrotext;
	}
}
