using UnityEngine;
using System.Collections;

public class Utility_Rotate : MonoBehaviour {
	public Vector3 direction= new Vector3(0,30f,0);
	public float speed=1f;
	public Space space=Space.World;
	public bool pingPong;
	public float pingPongTime=3f;

	[Header("Randomize")]
	public bool randomizeSpeed;
	public float minSpeed=-1f,maxSpeed=1f;
	// Use this for initialization
	void Start () {
		if(randomizeSpeed)
		{
			this.speed = UnityEngine.Random.Range(minSpeed,maxSpeed);
		}
		if(pingPong){
			InvokeRepeating("FlipDirection",pingPongTime,pingPongTime);
		}
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(direction*speed*Time.deltaTime,space);
	}

	void FlipDirection(){
		speed*=-1f;
	}
}
