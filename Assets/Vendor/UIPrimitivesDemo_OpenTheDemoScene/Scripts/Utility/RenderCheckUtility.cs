using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace VRB.Utility {
	public class RenderCheckUtility : MonoBehaviour {

		[SerializeField]
		protected bool isCanvas;

		protected CanvasRenderer canRender;
		protected Renderer render;
		protected Vector3 lastWorldPos;

		[HideInInspector]
		public bool isVisible;
		private bool hasSwitched;

		public bool HasSwitched() {
			bool hs = hasSwitched;
			hasSwitched = false;
			return hs;
		}
		// Use this for initialization
		public void Start () {
			lastWorldPos = transform.position;
			if (isCanvas) {
				canRender = GetComponent<CanvasRenderer> ();
			} else {
				if (render == null)
				render = GetComponent<Renderer> ();
			}
			CheckRender ();
		}
	
		private bool isRendering(){
			if (isCanvas) {
				return false;
			} else {
				return render.isVisible;
			}
		}

		private bool hasMoved(){
			if (!Vector3.Equals (transform.position, lastWorldPos)) {
				lastWorldPos = transform.position;
				return true;
			} else
				return false;
		}

		void CheckRender ()
		{
			if (hasMoved ()) {
				if (isRendering ()) {
					if (!isVisible) {
						isVisible = true;
						hasSwitched = true;
					}
				}
				else {
					if (isVisible) {
						isVisible = false;
						hasSwitched = true;
					}
				}
			}
		}

		void InternalTest ()
		{
			if (HasSwitched ()) {
				if (isVisible) {
					print ("VISIBLE!");
				}
				else {
					print ("NOT VISIBLE!");
				}
			}
		}

		// Update is called once per frame
		public void Update () {
			CheckRender ();
			//InternalTest ();
		}
	}
}
