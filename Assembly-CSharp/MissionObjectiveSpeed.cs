public class MissionObjectiveSpeed : MissionObjective
{
	public bool lessThan;

	public float requiredSpeed = 120f;

	private void Awake()
	{
		Setup();
	}

	public override bool CheckCompleted()
	{
		return base.CheckCompleted() && CheckSpeed();
	}

	protected virtual bool CheckSpeed()
	{
		if (lessThan)
		{
			return GetSpeed() <= requiredSpeed;
		}
		return GetSpeed() >= requiredSpeed;
	}

	protected virtual float GetSpeed()
	{
		return MissionObjective.player.InverseTransformDirection(MissionObjective.player.rigidbody.velocity).z;
	}

	public override string GetText()
	{
		if (lessThan)
		{
			if (requiredSpeed <= 1f && requiredSpeed >= 0f)
			{
				if (requireTrigger)
				{
					return "Stop in the Zone!";
				}
				return "Stop!";
			}
			return "Slow Down:\n" + GetSpeed().ToString("0") + " / " + requiredSpeed.ToString("0");
		}
		return "Reach the speed:\n" + GetSpeed().ToString("0") + " / " + requiredSpeed.ToString("0");
	}
}
