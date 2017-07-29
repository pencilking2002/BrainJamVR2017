using UnityEngine;
using System.Collections;

public class RotateUtility : MonoBehaviour {
	public Vector3 direction= new Vector3(0,30f,0);
	public float speed=1f;
	public Space space=Space.World;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(direction*speed*Time.deltaTime,space);
	}
}
