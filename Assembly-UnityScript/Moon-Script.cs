using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Boo.Lang;
using Boo.Lang.Runtime;
using UnityEngine;
using UnityScript.Lang;

[Serializable]
public class Moon_Script : MonoBehaviour
{
	[Serializable]
	[CompilerGenerated]
	internal sealed class _0024Check_002442 : GenericGenerator<WaitForSeconds>
	{
		[Serializable]
		[CompilerGenerated]
		internal sealed class _0024 : GenericGeneratorEnumerator<WaitForSeconds>, IEnumerator
		{
			internal float _0024distance_002443;

			internal float _0024_002420_002444;

			internal Color _0024_002421_002445;

			internal float _0024_002422_002446;

			internal Color _0024_002423_002447;

			internal float _0024_002424_002448;

			internal Color _0024_002425_002449;

			internal Moon_Script _0024self__002450;

			public _0024(Moon_Script self_)
			{
				_0024self__002450 = self_;
			}

			public override bool MoveNext()
			{
				int result;
				switch (_state)
				{
				default:
					_0024distance_002443 = Vector3.Distance(_0024self__002450.transform.position, _0024self__002450.player.position);
					if (!(_0024distance_002443 >= 40000f))
					{
						_0024self__002450.highMoon.renderer.active = true;
						_0024self__002450.lowMoon.renderer.enabled = false;
					}
					else
					{
						_0024self__002450.highMoon.renderer.active = false;
						_0024self__002450.lowMoon.renderer.enabled = true;
					}
					if (!(_0024distance_002443 >= 65000f))
					{
						float num = (_0024_002420_002444 = ((_0024distance_002443 - 6060f) / 746f * 1.61f + 17f) / 255f);
						Color color = (_0024_002421_002445 = Camera.mainCamera.backgroundColor);
						float num2 = (_0024_002421_002445.r = _0024_002420_002444);
						Color color3 = (Camera.mainCamera.backgroundColor = _0024_002421_002445);
						float num3 = (_0024_002422_002446 = ((_0024distance_002443 - 6060f) / 746f * 2.71f + 17f) / 255f);
						Color color4 = (_0024_002423_002447 = Camera.mainCamera.backgroundColor);
						float num4 = (_0024_002423_002447.g = _0024_002422_002446);
						Color color6 = (Camera.mainCamera.backgroundColor = _0024_002423_002447);
						float num5 = (_0024_002424_002448 = ((_0024distance_002443 - 6060f) / 746f * 1f + 17f) / 255f);
						Color color7 = (_0024_002425_002449 = Camera.mainCamera.backgroundColor);
						float num6 = (_0024_002425_002449.b = _0024_002424_002448);
						Color color9 = (Camera.mainCamera.backgroundColor = _0024_002425_002449);
						RuntimeServices.SetProperty(Camera.mainCamera.transform.GetComponent<AudioLowPassFilter>(), "cutoffFrequency", (_0024distance_002443 - 6060f) / 746f * 59.82f + 274f);
						RuntimeServices.SetProperty(Camera.mainCamera.transform.GetComponent<Vignetting>(), "intensity", (_0024distance_002443 - 6060f) / 746f * 0.05797f);
						result = (YieldDefault(2) ? 1 : 0);
					}
					else
					{
						result = (Yield(3, new WaitForSeconds(0.5f)) ? 1 : 0);
					}
					break;
				case 1:
					result = 0;
					break;
				}
				return (byte)result != 0;
			}
		}

		internal Moon_Script _0024self__002451;

		public _0024Check_002442(Moon_Script self_)
		{
			_0024self__002451 = self_;
		}

		public override IEnumerator<WaitForSeconds> GetEnumerator()
		{
			return new _0024(_0024self__002451);
		}
	}

	[Serializable]
	[CompilerGenerated]
	internal sealed class _0024Leave_002452 : GenericGenerator<WaitForSeconds>
	{
		[Serializable]
		[CompilerGenerated]
		internal sealed class _0024 : GenericGeneratorEnumerator<WaitForSeconds>, IEnumerator
		{
			internal int _0024i_002453;

			internal int _0024j_002454;

			internal Moon_Script _0024self__002455;

			public _0024(Moon_Script self_)
			{
				_0024self__002455 = self_;
			}

			public override bool MoveNext()
			{
				int result;
				switch (_state)
				{
				default:
					if (_0024self__002455.off)
					{
						_0024self__002455.off = false;
						GameObject.Find("Stars").particleEmitter.emit = true;
						GameObject.Find("Stars").renderer.enabled = true;
						RuntimeServices.SetProperty(Camera.mainCamera.transform.GetComponent<BloomAndLensFlares>(), "enabled", true);
						RuntimeServices.SetProperty(Camera.mainCamera.transform.GetComponent<Vignetting>(), "enabled", true);
						RuntimeServices.SetProperty(Camera.mainCamera.transform.GetComponent<AudioLowPassFilter>(), "enabled", true);
						for (_0024i_002453 = 0; _0024i_002453 < Extensions.get_length((System.Array)_0024self__002455.earth); _0024i_002453++)
						{
							MonoBehaviour.print(_0024self__002455.earth[_0024i_002453].name);
							_0024self__002455.TurnOff(_0024self__002455.earth[_0024i_002453]);
						}
						for (_0024j_002454 = 0; _0024j_002454 < Extensions.get_length((System.Array)_0024self__002455.space); _0024j_002454++)
						{
							_0024self__002455.TurnOn(_0024self__002455.space[_0024j_002454]);
						}
						result = (Yield(2, new WaitForSeconds(0.5f)) ? 1 : 0);
						break;
					}
					goto IL_01d1;
				case 2:
					UnityRuntimeServices.Invoke(GameObject.Find("SpawnPoint").GetComponent<SpawnPointScript>(), "ClearSpawns", new object[0], typeof(MonoBehaviour));
					goto IL_01d1;
				case 1:
					{
						result = 0;
						break;
					}
					IL_01d1:
					YieldDefault(1);
					goto case 1;
				}
				return (byte)result != 0;
			}
		}

		internal Moon_Script _0024self__002456;

		public _0024Leave_002452(Moon_Script self_)
		{
			_0024self__002456 = self_;
		}

		public override IEnumerator<WaitForSeconds> GetEnumerator()
		{
			return new _0024(_0024self__002456);
		}
	}

	[Serializable]
	[CompilerGenerated]
	internal sealed class _0024Enter_002457 : GenericGenerator<WaitForSeconds>
	{
		[Serializable]
		[CompilerGenerated]
		internal sealed class _0024 : GenericGeneratorEnumerator<WaitForSeconds>, IEnumerator
		{
			internal Moon_Script _0024self__002458;

			public _0024(Moon_Script self_)
			{
				_0024self__002458 = self_;
			}

			public override bool MoveNext()
			{
				int result;
				switch (_state)
				{
				default:
					if (!_0024self__002458.off)
					{
						GameObject.Find("Stars").particleEmitter.emit = false;
						GameObject.Find("Stars").renderer.enabled = false;
						RuntimeServices.SetProperty(GameObject.Find("Particle System").GetComponent<ParticleSystem>(), "enableEmission", true);
						result = (Yield(2, new WaitForSeconds(0.5f)) ? 1 : 0);
						break;
					}
					goto IL_00bb;
				case 2:
					UnityRuntimeServices.Invoke(GameObject.Find("SpawnPoint").GetComponent<SpawnPointScript>(), "ClearSpawns", new object[0], typeof(MonoBehaviour));
					goto IL_00bb;
				case 1:
					{
						result = 0;
						break;
					}
					IL_00bb:
					YieldDefault(1);
					goto case 1;
				}
				return (byte)result != 0;
			}
		}

		internal Moon_Script _0024self__002459;

		public _0024Enter_002457(Moon_Script self_)
		{
			_0024self__002459 = self_;
		}

		public override IEnumerator<WaitForSeconds> GetEnumerator()
		{
			return new _0024(_0024self__002459);
		}
	}

	[Serializable]
	[CompilerGenerated]
	internal sealed class _0024Exit_002460 : GenericGenerator<WaitForSeconds>
	{
		[Serializable]
		[CompilerGenerated]
		internal sealed class _0024 : GenericGeneratorEnumerator<WaitForSeconds>, IEnumerator
		{
			internal int _0024i2_002461;

			internal int _0024j2_002462;

			internal Moon_Script _0024self__002463;

			public _0024(Moon_Script self_)
			{
				_0024self__002463 = self_;
			}

			public override bool MoveNext()
			{
				int result;
				switch (_state)
				{
				default:
					MonoBehaviour.print("sup");
					if (!_0024self__002463.off)
					{
						for (_0024i2_002461 = 0; _0024i2_002461 < Extensions.get_length((System.Array)_0024self__002463.earth); _0024i2_002461++)
						{
							_0024self__002463.TurnOn(_0024self__002463.earth[_0024i2_002461]);
						}
						for (_0024j2_002462 = 0; _0024j2_002462 < Extensions.get_length((System.Array)_0024self__002463.space); _0024j2_002462++)
						{
							_0024self__002463.TurnOff(_0024self__002463.space[_0024j2_002462]);
						}
						RuntimeServices.SetProperty(GameObject.Find("Particle System").GetComponent<ParticleSystem>(), "enableEmission", false);
						RuntimeServices.SetProperty(Camera.mainCamera.transform.GetComponent<BloomAndLensFlares>(), "enabled", false);
						RuntimeServices.SetProperty(Camera.mainCamera.transform.GetComponent<Vignetting>(), "enabled", false);
						RuntimeServices.SetProperty(Camera.mainCamera.transform.GetComponent<AudioLowPassFilter>(), "enabled", false);
						_0024self__002463.off = true;
						result = (Yield(2, new WaitForSeconds(0.5f)) ? 1 : 0);
						break;
					}
					goto IL_01ba;
				case 2:
					UnityRuntimeServices.Invoke(GameObject.Find("SpawnPoint").GetComponent<SpawnPointScript>(), "ClearSpawns", new object[0], typeof(MonoBehaviour));
					goto IL_01ba;
				case 1:
					{
						result = 0;
						break;
					}
					IL_01ba:
					YieldDefault(1);
					goto case 1;
				}
				return (byte)result != 0;
			}
		}

		internal Moon_Script _0024self__002464;

		public _0024Exit_002460(Moon_Script self_)
		{
			_0024self__002464 = self_;
		}

		public override IEnumerator<WaitForSeconds> GetEnumerator()
		{
			return new _0024(_0024self__002464);
		}
	}

	private Transform player;

	private new Camera camera;

	private bool onMoon;

	private Transform highMoon;

	private Transform lowMoon;

	public Transform[] earth;

	public Transform[] space;

	private bool off;

	public Moon_Script()
	{
		off = true;
	}

	public virtual void Start()
	{
		player = GameObject.Find("Player").transform;
		highMoon = transform.Find("Moon High");
		lowMoon = transform.Find("Moon Low");
		for (int i = 0; i < Extensions.get_length((System.Array)space); i++)
		{
			TurnOff(space[i]);
		}
		StartCoroutine("Check");
	}

	public virtual IEnumerator Check()
	{
		return new _0024Check_002442(this).GetEnumerator();
	}

	public virtual IEnumerator Leave()
	{
		return new _0024Leave_002452(this).GetEnumerator();
	}

	public virtual IEnumerator Enter()
	{
		return new _0024Enter_002457(this).GetEnumerator();
	}

	public virtual IEnumerator Exit()
	{
		return new _0024Exit_002460(this).GetEnumerator();
	}

	public virtual void Land()
	{
		onMoon = true;
		UnityRuntimeServices.Invoke(player.GetComponent<move>(), "LandOnMoon", new object[0], typeof(MonoBehaviour));
	}

	public virtual void OnCollisionEnter(Collision obj)
	{
		if (!onMoon)
		{
			Land();
		}
	}

	public virtual void TurnOff(Transform obj)
	{
		IEnumerator enumerator = UnityRuntimeServices.GetEnumerator(obj);
		while (enumerator.MoveNext())
		{
			object obj2 = enumerator.Current;
			if (!(obj2 is Transform))
			{
				obj2 = RuntimeServices.Coerce(obj2, typeof(Transform));
			}
			Transform transform = (Transform)obj2;
			TurnOff(transform);
			UnityRuntimeServices.Update(enumerator, transform);
		}
		obj.gameObject.active = false;
	}

	public virtual void TurnOn(Transform obj)
	{
		IEnumerator enumerator = UnityRuntimeServices.GetEnumerator(obj);
		while (enumerator.MoveNext())
		{
			object obj2 = enumerator.Current;
			if (!(obj2 is Transform))
			{
				obj2 = RuntimeServices.Coerce(obj2, typeof(Transform));
			}
			Transform transform = (Transform)obj2;
			TurnOn(transform);
			UnityRuntimeServices.Update(enumerator, transform);
		}
		obj.gameObject.active = true;
	}

	public virtual void Main()
	{
	}
}
