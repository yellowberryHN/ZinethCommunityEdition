using UnityEngine;

public class PhoneButtonMonsterDebug : PhoneButton
{
	private MonsterTester _tester;

	private MonsterTester tester
	{
		get
		{
			if (!_tester)
			{
				_tester = Object.FindObjectOfType(typeof(MonsterTester)) as MonsterTester;
			}
			return _tester;
		}
	}

	private bool is_on
	{
		get
		{
			return tester.enabled;
		}
	}

	private void Awake()
	{
		if (textmesh == null)
		{
			textmesh = base.gameObject.GetComponent<TextMesh>();
		}
		if (controller == null)
		{
			controller = Object.FindObjectOfType(typeof(PhoneController)) as PhoneController;
		}
		if (is_on)
		{
			textmesh.text = "Monster Debug(on)";
		}
		else
		{
			textmesh.text = "Monster Debug(off)";
		}
		Init();
	}

	private void Start()
	{
	}

	public override void OnPressed()
	{
		tester.enabled = !is_on;
		if (is_on)
		{
			textmesh.text = "Monster Debug(on)";
		}
		else
		{
			textmesh.text = "Monster Debug(off)";
		}
		tester.showgui = is_on;
	}
}
