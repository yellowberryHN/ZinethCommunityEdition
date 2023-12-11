using UnityEngine;

public class MissionObjectiveTimed : MissionObjective
{
	public float startTime = 10f;

	private float currentTime = -1f;

	public bool fail_on_end = true;

	private void Awake()
	{
		Setup();
	}

	public override void OnBegin()
	{
		base.OnBegin();
		currentTime = startTime;
	}

	private void FixedUpdate()
	{
		currentTime -= Time.fixedDeltaTime;
		if (currentTime <= 0f && fail_on_end)
		{
			failed = true;
			SendMessageUpwards("FailMission");
		}
	}

	public override bool CheckCompleted()
	{
		if (!fail_on_end)
		{
			return base.CheckCompleted() && currentTime <= 0f;
		}
		return base.CheckCompleted();
	}

	public override string GetText()
	{
		return "Time Left: " + currentTime.ToString("0.00") + "s";
	}
}
