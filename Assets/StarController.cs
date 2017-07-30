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
			ScaleUp ();		
		} 
		if (GUI.Button (new Rect (10,10, 100, 160), "Sun glow"))
		{
			ScaleUp ();		
		} 
	}
		

	//[EditorButtonAttribute]
	public void ScaleUp()
	{
		var animator = GetComponent<UIPrimitives.UITransformAnimator> ();
		animator.AddScaleEndAnimation(Vector3.one * 7.0f, 2.0f, UIPrimitives.UIAnimationUtility.EaseType.easeInOutSine);

		LeanTween.value (gameObject, (Vector3 val) => {
			mat.SetColor ("_RimColor", new Color(val.x/255, val.y/255, val.z/255, 1));
		}, new Vector3(255,185,0), new Vector3(255,255,255), 2);

		LeanTween.value (gameObject, (Vector3 val) => {
			mat.SetColor ("_Color", new Color(val.x/255, val.y/255, val.z/255, 1));
		}, new Vector3(255,185,0), new Vector3(255,255,255), 2);
	}

	public void ScaleDown()
	{
		var animator = GetComponent<UIPrimitives.UITransformAnimator> ();
		animator.AddScaleEndAnimation(Vector3.one * 7.0f, 2.0f, UIPrimitives.UIAnimationUtility.EaseType.easeInOutSine);

		LeanTween.value (gameObject, (Vector3 val) => {
			mat.SetColor ("_RimColor", new Color(val.x/255, val.y/255, val.z/255, 1));
		}, new Vector3(255,185,0), new Vector3(255,255,255), 2);

		LeanTween.value (gameObject, (Vector3 val) => {
			mat.SetColor ("_Color", new Color(val.x/255, val.y/255, val.z/255, 1));
		}, new Vector3(255,185,0), new Vector3(255,255,255), 2);
	}

}
