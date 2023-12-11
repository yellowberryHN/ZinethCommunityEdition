public class MissionObjectiveCactusBreaks : MissionObjective
{
	public int requiredBreaks = 10;

	public int currentBreaks;

	private void Awake()
	{
		Setup();
	}

	public override void OnBegin()
	{
		base.OnBegin();
		currentBreaks = 0;
	}

	public override bool CheckCompleted()
	{
		currentBreaks += CactusBehavior.recentCactusBreaks;
		return base.CheckCompleted() && CheckBreaks();
	}

	protected virtual bool CheckBreaks()
	{
		return currentBreaks >= requiredBreaks;
	}

	public override string GetText()
	{
		return "Break Cacti: " + currentBreaks + " / " + requiredBreaks;
	}
}
