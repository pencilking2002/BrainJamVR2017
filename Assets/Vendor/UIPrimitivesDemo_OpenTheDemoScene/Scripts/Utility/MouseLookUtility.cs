using UnityEngine;
using System.Collections;

namespace App
{
	public class MouseLookUtility : MonoBehaviour
	{

		public enum RotationAxes
		{
			MouseXAndY = 0,
			MouseX = 1,
			MouseY = 2
		}
		public RotationAxes axes = RotationAxes.MouseXAndY;
		public float sensitivityX = 4f;
		public float sensitivityY = 4f;
		public float minimumX = -360F;
		public float maximumX = 360F;
		public float minimumY = -60F;
		public float maximumY = 60F;
		public float movementSpeed=1f;
		float rotationY = 0F;
		public bool rotationEnabled=true;
		void Update ()
		{
			if(UnityEngine.Input.GetKeyDown(KeyCode.N)){
				rotationEnabled=!rotationEnabled;
			} 

			if (UnityEngine.Input.GetMouseButtonDown (0)) {
				return;
			}

			if(!rotationEnabled)
				return;
			if (axes == RotationAxes.MouseXAndY) {
				float rotationX = transform.localEulerAngles.y + UnityEngine.Input.GetAxis ("Mouse X") * sensitivityX;
			
				rotationY += UnityEngine.Input.GetAxis ("Mouse Y") * sensitivityY;
				rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
				transform.localEulerAngles = new Vector3 (-rotationY, rotationX, 0);
			} else if (axes == RotationAxes.MouseX) {
				transform.Rotate (0, UnityEngine.Input.GetAxis ("Mouse X") * sensitivityX, 0);
			} else {
				rotationY += UnityEngine.Input.GetAxis ("Mouse Y") * sensitivityY;
				rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
				transform.localEulerAngles = new Vector3 (-rotationY, transform.localEulerAngles.y, 0);
			}
			float multiplier = .03f*movementSpeed;
			if(UnityEngine.Input.GetKey(KeyCode.LeftShift)||UnityEngine.Input.GetKey(KeyCode.RightShift))
				multiplier*=2f;
			if(multiplier==0)
				return;
			transform.position += transform.forward * UnityEngine.Input.GetAxis ("Vertical") * multiplier;
			transform.position += transform.up * UnityEngine.Input.GetAxis ("Lateral") * multiplier;
			transform.position += transform.right * UnityEngine.Input.GetAxis ("Horizontal") * multiplier;
		}
	
		void Start ()
		{
			// Make the rigid body not change rotation
			if (GetComponent<Rigidbody> ())
				GetComponent<Rigidbody> ().freezeRotation = true;
		}
	}
}