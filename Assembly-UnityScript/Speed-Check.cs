using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Boo.Lang;
using UnityEngine;
using UnityScript.Lang;

[Serializable]
public class Speed_0020Check : MonoBehaviour
{
	[Serializable]
	[CompilerGenerated]
	internal sealed class _0024Check_002468 : GenericGenerator<object>
	{
		[Serializable]
		[CompilerGenerated]
		internal sealed class _0024 : GenericGeneratorEnumerator<object>, IEnumerator
		{
			internal Speed_0020Check _0024self__002469;

			public _0024(Speed_0020Check self_)
			{
				_0024self__002469 = self_;
			}

			public override bool MoveNext()
			{
				int result;
				switch (_state)
				{
				default:
					if (!(_0024self__002469.player.InverseTransformDirection(_0024self__002469.player.rigidbody.velocity).z < _0024self__002469.speed))
					{
						_0024self__002469.ReachedSpeed();
						goto case 1;
					}
					result = (YieldDefault(2) ? 1 : 0);
					break;
				case 1:
					result = 0;
					break;
				}
				return (byte)result != 0;
			}
		}

		internal Speed_0020Check _0024self__002470;

		public _0024Check_002468(Speed_0020Check self_)
		{
			_0024self__002470 = self_;
		}

		public override IEnumerator<object> GetEnumerator()
		{
			return new _0024(_0024self__002470);
		}
	}

	private Transform player;

	public float speed;

	public virtual void Start()
	{
		player = GameObject.Find("Player").transform;
		StartCoroutine("Check");
	}

	public virtual IEnumerator Check()
	{
		return new _0024Check_002468(this).GetEnumerator();
	}

	public virtual void ReachedSpeed()
	{
		UnityRuntimeServices.Invoke(GameObject.Find("Door One Trigger").GetComponent<DoorScript>(), "Open2", new object[0], typeof(MonoBehaviour));
		UnityRuntimeServices.Invoke(transform.GetComponent<DoCommandTrigger>(), "Activate", new object[0], typeof(MonoBehaviour));
	}

	public virtual void Main()
	{
	}
}
