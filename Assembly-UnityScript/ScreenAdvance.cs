using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Boo.Lang;
using UnityEngine;

[Serializable]
public class ScreenAdvance : MonoBehaviour
{
	[Serializable]
	[CompilerGenerated]
	internal sealed class _0024Lock_002436 : GenericGenerator<WaitForSeconds>
	{
		[Serializable]
		[CompilerGenerated]
		internal sealed class _0024 : GenericGeneratorEnumerator<WaitForSeconds>, IEnumerator
		{
			internal ScreenAdvance _0024self__002437;

			public _0024(ScreenAdvance self_)
			{
				_0024self__002437 = self_;
			}

			public override bool MoveNext()
			{
				int result;
				switch (_state)
				{
				default:
					result = (Yield(2, new WaitForSeconds(1f)) ? 1 : 0);
					break;
				case 2:
					_0024self__002437.locked = 1;
					_0024self__002437.StartCoroutine("Input2");
					YieldDefault(1);
					goto case 1;
				case 1:
					result = 0;
					break;
				}
				return (byte)result != 0;
			}
		}

		internal ScreenAdvance _0024self__002438;

		public _0024Lock_002436(ScreenAdvance self_)
		{
			_0024self__002438 = self_;
		}

		public override IEnumerator<WaitForSeconds> GetEnumerator()
		{
			return new _0024(_0024self__002438);
		}
	}

	[Serializable]
	[CompilerGenerated]
	internal sealed class _0024Input2_002439 : GenericGenerator<object>
	{
		[Serializable]
		[CompilerGenerated]
		internal sealed class _0024 : GenericGeneratorEnumerator<object>, IEnumerator
		{
			internal ScreenAdvance _0024self__002440;

			public _0024(ScreenAdvance self_)
			{
				_0024self__002440 = self_;
			}

			public override bool MoveNext()
			{
				int result;
				switch (_state)
				{
				default:
					if (_0024self__002440.advance == 1)
					{
						Application.LoadLevel(_0024self__002440.levelToLoad);
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

		internal ScreenAdvance _0024self__002441;

		public _0024Input2_002439(ScreenAdvance self_)
		{
			_0024self__002441 = self_;
		}

		public override IEnumerator<object> GetEnumerator()
		{
			return new _0024(_0024self__002441);
		}
	}

	public string levelToLoad;

	private int advance;

	private int locked;

	public virtual void Start()
	{
		StartCoroutine("Lock");
	}

	public virtual IEnumerator Lock()
	{
		return new _0024Lock_002436(this).GetEnumerator();
	}

	public virtual IEnumerator Input2()
	{
		return new _0024Input2_002439(this).GetEnumerator();
	}

	public virtual void Update()
	{
		if (Input.anyKeyDown && locked == 1)
		{
			advance = 1;
		}
	}

	public virtual void Main()
	{
	}
}
