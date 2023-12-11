public class MissionObjectiveAirtime : MissionObjective
{
	public float requiredAirtime = 3f;

	private void Awake()
	{
		Setup();
	}

	public override void Setup()
	{
		base.Setup();
	}

	public override bool CheckCompleted()
	{
		return base.CheckCompleted() && GetAirTime() >= requiredAirtime;
	}

	public override string GetText()
	{
		return "Stay in the air!\n" + GetAirTime().ToString("0.00") + "s / " + requiredAirtime.ToString("0.00") + "s";
	}

	protected virtual float GetAirTime()
	{
		return MissionObjective._playerMove.airTime;
	}
}
