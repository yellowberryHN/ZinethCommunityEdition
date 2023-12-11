using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
	public MissionObjective pointObjective;

	public MissionObjective GetTarget()
	{
		if ((bool)MissionController.focus_mission)
		{
			List<MissionObjective> currentObjectives = MissionController.focus_mission.GetCurrentObjectives();
			for (int i = 0; i < currentObjectives.Count; i++)
			{
				if (currentObjectives[i].use_position)
				{
					return currentObjectives[i];
				}
			}
			return null;
		}
		return null;
	}

	private void Update()
	{
		CheckAndPoint();
	}

	public void CheckAndPoint()
	{
		pointObjective = GetTarget();
		if (!pointObjective || pointObjective.completed)
		{
			MissionController.GetInstance().arrowActive = false;
			base.gameObject.SetActiveRecursively(false);
			Invoke("CheckAndPoint", 0.5f);
			return;
		}
		if (!base.gameObject.active)
		{
			base.gameObject.SetActiveRecursively(true);
		}
		base.transform.LookAt(pointObjective.objectivePosition);
		CancelInvoke("CheckAndPoint");
	}
}
