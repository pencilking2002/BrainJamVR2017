using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowScale : MonoBehaviour {
	private MeshRenderer rend;
	private Material mat;
	public RectTransform glow;

	public float speed = 1;

	// Use this for initialization
	void Start () 
	{
		rend = GetComponent <MeshRenderer> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		float scale = Mathf.Sin (Time.time * speed);
		scale = 0.9f + 0.2f * scale;
		glow.localScale = Vector3.one * scale;

		//print (Time.time); 
	}
}
