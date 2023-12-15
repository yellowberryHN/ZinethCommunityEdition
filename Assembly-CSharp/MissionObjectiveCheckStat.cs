using System;

public class MissionObjectiveCheckStat : MissionObjective
{
	[Serializable]
	public enum Comparison
	{
		less,
		lessOrEqual,
		equal,
		greaterOrEqual,
		greater
	}

	public string statName = string.Empty;

	private float currentStat;

	public float requiredStat;

	public Comparison comparison = Comparison.greater;

	private void Awake()
	{
		Setup();
	}

	public override bool CheckCompleted()
	{
		return base.CheckCompleted() && CheckStat();
	}

	protected virtual bool CheckStat()
	{
		currentStat = PhoneInterface.GetStat(statName);
		if (comparison == Comparison.greaterOrEqual)
		{
			return currentStat >= requiredStat;
		}
		if (comparison == Comparison.greater)
		{
			return currentStat > requiredStat;
		}
		if (comparison == Comparison.less)
		{
			return currentStat < requiredStat;
		}
		if (comparison == Comparison.lessOrEqual)
		{
			return currentStat <= requiredStat;
		}
		if (comparison == Comparison.equal)
		{
			return currentStat == requiredStat;
		}
		return false;
	}

	public override string ParseGUIString(string guistring)
	{
		guistring = guistring.Replace("{statName}", statName);
		guistring = guistring.Replace("{currentStat}", currentStat.ToString());
		guistring = guistring.Replace("{requiredStat}", requiredStat.ToString());
		return base.ParseGUIString(guistring);
	}
}
