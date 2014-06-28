using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace We
{
	// gestisce l'input per valori booleani
	public static class Input
	{
		public static bool MoveUp
		{
			get { return UnityEngine.Input.GetAxis("Vertical") > 0; }
		}
		public static bool MoveDown
		{
			get { return UnityEngine.Input.GetAxis("Vertical") < 0; }
		}
		public static bool MoveLeft
		{
			get { return UnityEngine.Input.GetAxis("Horizontal") < 0; }
		}
		public static bool MoveRight
		{
			get { return UnityEngine.Input.GetAxis("Horizontal") > 0; }
		}
		public static int MoveHorizontal
		{
			get { return MoveLeft ? -1 : MoveRight ? +1 : 0; }
		}
		public static int MoveVertical
		{
			get { return MoveUp ? -1 : MoveDown ? +1 : 0; }
		}
		public static bool Jump
		{
			get { return UnityEngine.Input.GetButton ("Jump"); }
		}
		public static bool Attack
		{
			get { return UnityEngine.Input.GetButton ("Fire1"); }
		}
		public static bool Attack2
		{
			get { return UnityEngine.Input.GetButton ("Fire2"); }
		}
	}
}