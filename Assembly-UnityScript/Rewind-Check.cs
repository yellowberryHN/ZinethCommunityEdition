using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Boo.Lang;
using Boo.Lang.Runtime;
using UnityEngine;
using UnityScript.Lang;

[Serializable]
public class Rewind_Check : MonoBehaviour
{
	[Serializable]
	[CompilerGenerated]
	internal sealed class _0024Check_002465 : GenericGenerator<WaitForSeconds>
	{
		[Serializable]
		[CompilerGenerated]
		internal sealed class _0024 : GenericGeneratorEnumerator<WaitForSeconds>, IEnumerator
		{
			internal Rewind_Check _0024self__002466;

			public _0024(Rewind_Check self_)
			{
				_0024self__002466 = self_;
			}

			public override bool MoveNext()
			{
				int result;
				switch (_state)
				{
				default:
					if (_0024self__002466.player.GetComponent<move>().freezeControls)
					{
						_0024self__002466.ReachedSpeed();
						goto case 1;
					}
					result = (Yield(2, new WaitForSeconds(0.2f)) ? 1 : 0);
					break;
				case 1:
					result = 0;
					break;
				}
				return (byte)result != 0;
			}
		}

		internal Rewind_Check _0024self__002467;

		public _0024Check_002465(Rewind_Check self_)
		{
			_0024self__002467 = self_;
		}

		public override IEnumerator<WaitForSeconds> GetEnumerator()
		{
			return new _0024(_0024self__002467);
		}
	}

	private Transform player;

	private bool dra;

	public bool check;

	public virtual void Start()
	{
		player = GameObject.Find("Player").transform;
		GameObject.Find("SpawnPoint").GetComponent<SpawnPointScript>().canRespawn = false;
	}

	public virtual void OnTriggerEnter(Collider obj)
	{
		if (!check)
		{
			GameObject.Find("SpawnPoint").GetComponent<SpawnPointScript>().canRespawn = true;
			player.GetComponent<move>().canRewind = true;
			StartCoroutine("Check");
		}
		else if (dra)
		{
			GameObject.Find("Bridge").animation.Play();
		}
	}

	public virtual IEnumerator Check()
	{
		return new _0024Check_002465(this).GetEnumerator();
	}

	public virtual void ReachedSpeed()
	{
		GameObject.Find("Draw").GetComponent<Rewind_Check>().Draw();
		transform.GetComponent<DoCommandTrigger>().Activate();
		UnityEngine.Object.Destroy(gameObject);
	}

	public virtual void Draw()
	{
		dra = true;
	}
}
