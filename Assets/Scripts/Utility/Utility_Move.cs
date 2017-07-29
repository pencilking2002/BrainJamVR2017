using UnityEngine;
using System.Collections;

public class Utility_Move : MonoBehaviour {
	public Vector3 direction= new Vector3(0,0,1f);
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
		Vector3 updatedDirection = direction;
		if(space == Space.Self)
			updatedDirection = transform.TransformDirection(updatedDirection);
		transform.position += updatedDirection*speed*Time.deltaTime;
	}

	void FlipDirection(){
		speed*=-1f;
	}
}
