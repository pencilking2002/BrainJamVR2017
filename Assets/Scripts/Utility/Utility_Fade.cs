using UnityEngine;
using System.Collections;
using Neuromancers.Utility;


public class Utility_Fade : MonoBehaviour {

	//readonly
	protected readonly Color FADE_IN_COLOR = new Color(0,0,0,1f);
	protected readonly Color FADE_OUT_COLOR = new Color(0,0,0,0);

	//Serialized
	[SerializeField]
	protected float fadeDuration=3f;

	/////Protected/////
	//References
	protected Material myMaterial;
	protected AnimationCurve animationCurve;
	//Primitives
	protected bool isFading;
	protected Color startColor,endColor;
	protected float animationStartTime;

	
	/////Properties/////
	/// 
	/// 

	public float FadeDuration
	{
		get {
			return this.fadeDuration;
		}
		set {
			this.fadeDuration = value;
		}
	}
	
	///////////////////////////////////////////////////////////////////////////
	//
	// Inherited from MonoBehaviour
	//

	protected void Awake()
	{
		this.myMaterial=GetComponent<Renderer>().material;
	}

	protected void Start ()
	{
		StartFadeOut();
	}

	protected void Update ()
	{
		UpdateAnimation();
	}

	///////////////////////////////////////////////////////////////////////////
	//
	// Utility_Fade Functions
	//
	
	public float StartFadeIn()
	{
//		StartCoroutine(SetRendererEnabled(true));
		this.startColor = GetColor();
		this.endColor = FADE_IN_COLOR;
		StartFade();

		StartCoroutine(SetRendererEnabled(true));

		return this.fadeDuration;
	}

	public float StartFadeOut()
	{
		
		StartCoroutine(SetRendererEnabled(true));

		this.startColor = FADE_IN_COLOR;
		this.endColor = FADE_OUT_COLOR;
		StartFade();

		StartCoroutine(SetRendererEnabled(false,this.fadeDuration));
		
		return this.fadeDuration;
	}


	protected void StartFade()
	{
		this.isFading = true;
		this.animationCurve	= Utility.GetCurve_Ease(this.fadeDuration);
	}

	protected void UpdateAnimation()
	{
		if(this.isFading)
		{
			float percent = animationCurve.Evaluate(Time.time);

			 SetColor(Color.Lerp(this.startColor,this.endColor,percent));
			this.isFading = percent<1f;
		}
	}

	protected IEnumerator SetRendererEnabled(bool value, float delay=0f)
	{
		if(delay!=0)
			yield return new WaitForSeconds(delay);

		this.GetComponent<Renderer>().enabled=value;
	}

	protected Color GetColor()
	{
		return this.myMaterial.color;
	}

	protected void SetColor(Color color)
	{
		this.myMaterial.color = color;
	}
	
}

