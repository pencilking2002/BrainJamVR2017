using UnityEngine;

/// <summary>
/// Allows you to toggle monoscopic rendering with a gamepad button press.
/// </summary>
public class OVRMonoscopic : MonoBehaviour
{
	/// <summary>
	/// The gamepad button that will toggle monoscopic rendering.
	/// </summary>
	public OVRInput.RawButton	toggleButton = OVRInput.RawButton.B;

	private bool						monoscopic = false;

	/// <summary>
	/// Check input and toggle monoscopic rendering mode if necessary
	/// See the input mapping setup in the Unity Integration guide
	/// </summary>
	void Update()
	{
		// NOTE: some of the buttons defined in OVRInput.RawButton are not available on the Android game pad controller
		if (OVRInput.GetDown(toggleButton))
		{
			//*************************
			// toggle monoscopic rendering mode
			//*************************
			monoscopic = !monoscopic;
			OVRManager.instance.monoscopic = monoscopic;
		}
	}

}
