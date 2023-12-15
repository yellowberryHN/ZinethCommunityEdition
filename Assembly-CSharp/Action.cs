public class Action
{
	public stateEnum mood;

	public float length;

	public float pause;

	public Action(stateEnum newMood, float newLength, float newPause)
	{
		mood = newMood;
		length = newLength;
		pause = newPause;
	}
}
