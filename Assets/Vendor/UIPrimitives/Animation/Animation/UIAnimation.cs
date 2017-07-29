using UnityEngine;
using System.Collections;
using UIPrimitives;

namespace UIPrimitives
{
	public abstract class UIAnimation : PrimitiveAnimation
	{
		/////Ease/////
		//EaseType
		public UIAnimationUtility.EaseType easeType = UIAnimationUtility.EaseType.easeOutSine;
		//LastEaseType
		public UIAnimationUtility.EaseType lastEaseType;
		//EaseCurve
		public AnimationCurve easeCurve;

		/////Loop/////
		//LoopCount
		[Tooltip("-1 For Endless Loop")]
		public int loopCount =0;
		//LoopType
		[Tooltip("Ping Pong For Reversed Animation")]
		public UIAnimation_Base.LoopType loopType = UIAnimation_Base.LoopType.PingPong;


		/////Sequence/////
		//PlayOnAwake
		public bool playOnAwake = true;
		//SetValueAtStart
		[Tooltip("Sets The Initial Value On Start, Helpful When You Want To Use A Different Value In Editor Mode")]
		public bool setValueAtStart = false;

		/////Value/////
		//TweenType
		[Tooltip("Type of tween to use.")]
		public UIAnimation_Base.TweenType tweenType = UIAnimation_Base.TweenType.Start;
		
		/////Editor/////
		//CurrentCurve
		protected Animation currentCurve;
		//IsCurveMinimized
		public bool isCurveMinimized = true;
		
		///////////////////////////////////////////////////////////////////////////
		//
		// Inherited from MonoBehaviour
		//

		protected virtual void Start()
		{
			if (setValueAtStart) {
				SetStartValues ();
			}
			if (playOnAwake) {
				if (startDelay == 0) {
					StartAnimation ();
				} else {
					StartAnimationWithDelay ();
				}
			}
		}
		
		///////////////////////////////////////////////////////////////////////////
		//
		// UIAnimation Functions
		//

		public abstract void SetStartValues ();
		public abstract void StartAnimation ();
		public abstract void StartAnimationReversed ();
		
		public void StartAnimationWithDelay ()
		{
			Invoke ("StartAnimation", startDelay);
		}
		
		public void StartAnimationReversedWithDelay ()
		{
			Invoke ("StartAnimationReversed", startDelay);
		}
	}
}