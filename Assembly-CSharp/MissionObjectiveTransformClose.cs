using UnityEngine;

public class MissionObjectiveTransformClose : MissionObjective
{
	public float distance = 5f;

	private void Awake()
	{
		Setup();
	}

	public override bool CheckCompleted()
	{
		return base.CheckCompleted() && Vector3.Distance(MissionObjective.player.transform.position, base.objectivePosition) < distance;
	}

	public override void OnBegin()
	{
		base.OnBegin();
	}

	public override void OnEnd()
	{
		base.OnEnd();
	}
}
