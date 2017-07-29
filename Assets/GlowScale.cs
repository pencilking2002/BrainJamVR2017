using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowScale : MonoBehaviour {
	private MeshRenderer rend;
	private Material mat;
	public RectTransform glow;

	public float scalePower = 2;
	public float speed = 1;

	// Use this for initialization
	void Start () 
	{
		rend = GetComponent <MeshRenderer> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		float scale = Mathf.Sin (Time.time * speed) * scalePower * Time.deltaTime;
		glow.localScale = new Vector3(glow.localScale.x + scale, glow.localScale.y + scale, glow.localScale.z + scale);

		//print (Time.time); 
	}
}
