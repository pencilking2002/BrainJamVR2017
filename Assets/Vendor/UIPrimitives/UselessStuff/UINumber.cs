using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using UIPrimitives;

namespace UIPrimitives
{
	public class UINumber : PrimitiveAnimation
	{

		public float minFloat, maxFloat = 99f;
//		public float duration = 10f;
		public bool pingPong=true;
		public bool playOnStart=true;
		float currentValueFloat;
		bool isAnimating;
		float startTime;
		Text myText;


		protected void Start ()
		{
			myText = GetComponent<Text> ();
			if(playOnStart){
				startTime=Time.time;
				isAnimating=true;
			}
		}

		public void StartAnimation()
		{
			startTime=Time.time;
			isAnimating=true;
		}
	

		protected void Update ()
		{
			if(!isAnimating)
				return;

			float range = maxFloat - minFloat;
			float timePassed = Time.time-startTime;
			float rangeByDuration = (range / duration);
			if(pingPong)
				currentValueFloat = minFloat + Mathf.PingPong (timePassed * rangeByDuration, range);
			else
				currentValueFloat = minFloat + Mathf.Clamp (timePassed * rangeByDuration,0, range);
			string formattedString = string.Format("{0:n0}",currentValueFloat);
			myText.text = formattedString;

			if(currentValueFloat>=maxFloat&&pingPong==false)
				isAnimating=false;

		}
	}
}