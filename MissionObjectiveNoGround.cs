public class MissionObjectiveNoGround : MissionObjective
{
	private float lastAirTime = -1f;

	private void Awake()
	{
		Setup();
	}

	public override void OnBegin()
	{
		base.OnBegin();
		lastAirTime = MissionObjective._playerMove.airTime;
	}

	public override bool CheckCompleted()
	{
		if (GetAirTime() < lastAirTime && GetAirTime() >= 0f && GetAirTime() <= 0.1f)
		{
			failed = true;
		}
		lastAirTime = GetAirTime();
		return base.CheckCompleted();
	}

	public override string GetText()
	{
		return "Don't Touch the Ground!";
	}

	public virtual float GetAirTime()
	{
		return MissionObjective._playerMove.airTime;
	}
}
