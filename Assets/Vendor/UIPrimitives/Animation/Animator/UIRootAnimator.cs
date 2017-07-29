using UnityEngine;
using System;
using System.Collections;
using UIPrimitives;

namespace UIPrimitives {
	[DisallowMultipleComponent]
	public class UIRootAnimator : MonoBehaviour {
		public enum AnimationDirection {
			Forward,
			Reverse
		}

		public bool playOnAwake;
		public float startDelay;
		public bool ignoreChildDelay = false;
		public bool shouldTriggerChildRootAnimators = true;
		[Header ("Depth")]
		public bool shouldRestrictDepth = false;
		public int maxDepth = 1;
		[Header ("Duration")]
		public bool overrideDuration;
		public float duration = 2f;

		protected void Start () {
			if (playOnAwake) {

				Initialize ();
			}
		}

		public void Initialize () {

//			Debug.Log("Initialize: "+gameObject.name);

			if (startDelay == 0) {
				Animate ();
			} else {
				AnimateWithDelay ();
			}
		}


		/// <summary>
		/// Animate this instance.
		/// </summary>
		public float Animate (AnimationDirection animationDirection = AnimationDirection.Forward, float durationMultiplier = 1f) {
			bool reversed = animationDirection == AnimationDirection.Reverse;

			float totalAnimationLength = GetAnimationLength (this.transform);
			if(reversed) {
				ReverseDelays(this.transform,totalAnimationLength);
			}
			AnimateChildren (this.transform, reversed, durationMultiplier);

			return totalAnimationLength * durationMultiplier;
		}

		protected float GetAnimationLength (Transform startTransform) {

			float maxAnimationLength = float.MinValue;

			Action<Transform> lambda = (childTransform) => { 

				PrimitiveAnimation[] primitiveAnimations = childTransform.GetComponents<PrimitiveAnimation> ();
				for (int i = 0; i < primitiveAnimations.Length; ++i) {


					float animationLength = primitiveAnimations[i].startDelay + primitiveAnimations[i].duration;
					if(animationLength>maxAnimationLength)
						maxAnimationLength = animationLength;
				}
			};

			TraverseChildren (startTransform, lambda, 0,false);

			return maxAnimationLength;
		}

		protected void ReverseDelays (Transform startTransform,float totalAnimationLength) {

			Action<Transform> lambda = (childTransform) => { 

				PrimitiveAnimation[] primitiveAnimations = childTransform.GetComponents<PrimitiveAnimation> ();
				for (int i = 0; i < primitiveAnimations.Length; ++i) {

					float newDelay = totalAnimationLength - (primitiveAnimations[i].startDelay + primitiveAnimations[i].duration);
					primitiveAnimations[i].startDelay = newDelay;
				}
			};

			TraverseChildren (startTransform, lambda, 0,false);
		}

		protected void TraverseChildren (Transform child, Action<Transform> action, int depth, bool isAnimating) {

			//Depth restriction
			if(shouldRestrictDepth && (depth>maxDepth))
				return;

			//Trigger child root animator if we should
			UIRootAnimator rootAnimator = child.GetComponent<UIRootAnimator> ();
			if (rootAnimator != null && (depth!=0)) { 

				if (shouldTriggerChildRootAnimators && isAnimating)
					rootAnimator.Initialize ();
				return;
			}

			//Where the stuff happens
			if (action != null)
				action (child);

			for (int i = 0; i < child.childCount; i++) {

				Transform childChild = child.GetChild (i);
				TraverseChildren (childChild, action, depth + 1,isAnimating);
			}
		}

		protected void AnimateChildren (Transform startTransform, bool reversed, float durationMultiplier) {

			Action<Transform> lambda = (childTransform) => { 

				//PrimitiveAnimation
				PrimitiveAnimation[] primitiveAnimations = childTransform.GetComponents<PrimitiveAnimation> ();
				for (int i = 0; i < primitiveAnimations.Length; ++i) {

					if (overrideDuration)
						primitiveAnimations [i].duration = duration;


					primitiveAnimations [i].duration *= durationMultiplier;
					primitiveAnimations [i].startDelay *= durationMultiplier;
				}
				//UIAnimation
				UIAnimation[] myUIAnimations = childTransform.GetComponents<UIAnimation> ();
				for (int i = 0; i < myUIAnimations.Length; i++) {
					if (!myUIAnimations [i].enabled)
						continue;

					if (ignoreChildDelay) {
						if (reversed)
							myUIAnimations [i].StartAnimationReversed ();
						else
							myUIAnimations [i].StartAnimation ();
					} else {
						if (reversed)
							myUIAnimations [i].StartAnimationReversedWithDelay ();
						else
							myUIAnimations [i].StartAnimationWithDelay ();
					}
				}
				//UIText
				UIText[] myUITextAnimations = childTransform.GetComponents<UIText> ();
				for (int i = 0; i < myUITextAnimations.Length; i++) {
					if (!myUITextAnimations [i].enabled)
						continue;

					if (ignoreChildDelay) {
						myUITextAnimations [i].ExpandText ();
					} else {
						if (reversed)
							myUITextAnimations [i].RetractText ();
						else
							myUITextAnimations [i].ExpandTextWithDelay ();
					}

				}
				//UINumber
				UINumber[] myUINumberAnimations = childTransform.GetComponents<UINumber> ();
				for (int i = 0; i < myUINumberAnimations.Length; i++) {
					if (!myUINumberAnimations [i].enabled)
						continue;

					if (ignoreChildDelay) {
						myUINumberAnimations [i].StartAnimation ();
					} else {
						myUINumberAnimations [i].StartAnimation ();
					}
				}
			};

			TraverseChildren (startTransform, lambda, 0,true);
		}

		[EditorButtonAttribute]
		public void AnimateWithDelay () {
			
			StartCoroutine (AnimateCoroutine (startDelay));
		}

		[EditorButtonAttribute]
		protected void AnimateReversed () {
			Animate (AnimationDirection.Reverse);
		}

		protected IEnumerator AnimateCoroutine (float delay) {
			yield return new WaitForSeconds (delay);
			Animate ();
		}

	}
}