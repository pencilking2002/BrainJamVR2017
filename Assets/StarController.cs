using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarController : MonoBehaviour {

	private Material mat;
	private Renderer rend;

	private void Awake()
	{
		rend = GetComponent <Renderer> ();
		mat = rend.material;
	}

	private void OnGUI()
	{
		if (GUI.Button (new Rect (10,10, 100, 60), "Sun glow"))
		{
			Animate ();		
		} 
	}
		

	//[EditorButtonAttribute]
	public void Animate()
	{
		var animator = GetComponent<UIPrimitives.UITransformAnimator> ();
		animator.AddScaleEndAnimation(Vector3.one * 7.0f, 2.0f, UIPrimitives.UIAnimationUtility.EaseType.easeInOutSine);

	}

}
