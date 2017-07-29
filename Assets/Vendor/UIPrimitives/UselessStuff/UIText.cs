using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UIPrimitives;

namespace UIPrimitives
{
	[RequireComponent(typeof(Text))]
	public class UIText : PrimitiveAnimation
	{
		public enum UITextAnimationType
		{
			EaseInOut,
			Linear
		}
		public UITextAnimationType myUITextAnimationType;
		Text myText;
//		public float animationDuration = 3f;
		public bool playOnStart = true;
		public bool setValueOnStart;
//		public float startDelay = 0;
		// Use this for initialization
		void Awake ()
		{
			myText = GetComponent<Text> ();
		}

		void Start ()
		{
			if (playOnStart) {
				if (startDelay == 0) {
					nextAnimationText = myText.text;
					ExpandText ();
				} else {
					nextAnimationText = myText.text;
					Invoke ("ExpandText", startDelay);
					myText.text = "";
				}
			}
			if (setValueOnStart) {
			
				nextAnimationText = myText.text;
				myText.text = "";
			}
		}

		bool isAnimating, isExpanding;
		int textLength;
		AnimationCurve animationCurve;
		string animationText, nextAnimationText;

		public void ExpandTextWithDelay ()
		{
			Invoke ("ExpandText", startDelay);
		}

		public void ExpandText ()
		{
			animationText = nextAnimationText;
			if (myUITextAnimationType == UITextAnimationType.EaseInOut) {
				animationCurve = AnimationCurve.EaseInOut (Time.time, 0, Time.time + duration, 1f);
			} else if (myUITextAnimationType == UITextAnimationType.Linear) {
				animationCurve = AnimationCurve.Linear (Time.time, 0, Time.time + duration, 1f);
			}

			textLength = nextAnimationText.Length;
			isAnimating = true;
			isExpanding = true;
		}

		public void RetractText ()
		{
			animationText = myText.text;
			if (myUITextAnimationType == UITextAnimationType.EaseInOut) {
				animationCurve = AnimationCurve.EaseInOut (Time.time, 0, Time.time + duration, 1f);
			} else if (myUITextAnimationType == UITextAnimationType.Linear) {
				animationCurve = AnimationCurve.Linear (Time.time, 0, Time.time + duration, 1f);
			}
			textLength = animationText.Length;
			isAnimating = true;
			isExpanding = false;
		}

		public void SetText (string newText)
		{
			nextAnimationText = newText;
			if (myText.text == "") {
				ExpandText ();
			} else {
				RetractText ();
				Invoke ("ExpandText", duration);
			}
		}

		protected void Update ()
		{
			if (isAnimating) {
				UpdateAnimation ();
			}
		}

		void UpdateAnimation ()
		{
			float percent = animationCurve.Evaluate (Time.time);
			int animatedTextLength = (int)(textLength * percent);
			if (!isExpanding) {
				animatedTextLength = (textLength) - animatedTextLength;
			}
			string animatedText = animationText.Substring (0, animatedTextLength);
			myText.text = animatedText;
			isAnimating = percent < 1f;
		}
	}
}