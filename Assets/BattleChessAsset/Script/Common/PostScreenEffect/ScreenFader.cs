using UnityEngine;
using System.Collections;

using System;


// unity game object singleton
public class ScreenFaderException : System.Exception
{	
	public ScreenFaderException()	{
	}	
	
	public ScreenFaderException(string message)
	 : base(message) {
	}
		
	public ScreenFaderException(System.Exception innerException)
		: base(null, innerException) {
	}	
	
	public ScreenFaderException(string message, System.Exception innerException)
	 : base(message, innerException) {
	}
}





public class ScreenFader : MonoBehaviour	
{	
	public enum eFade {
		
		E_FADE_NONE = -1,
		E_FADE_OUT,
		E_FADE_IN		
	}
	
	public GUIStyle backGroundStyle;			// Style for background tiling
	public Texture2D fadeTexture;				// 1x1 pixel texture used for fading
	
	private Color currColor;					// default starting color: black and fully transparrent
	public Color startColor;
	public Color targetColor;					// default target color: black and fully transparrent
	
	public Color StartColor { 
		
		get {
			
			return startColor;
		}
		
		set {
			
			startColor = value;
		}
	}
	
	public Color TargetColor { 
		
		get {
			
			return targetColor;
		}
		
		set {
			
			targetColor = value;
		}
	}
	
	//public Color m_DeltaColor = Color.clear;				// the delta-color is basically the "speed / second" at which the current color should change
	public int fFadeGUIDepth;					// make sure this texture is drawn on top of everything 
	
	private float fFadeDurTime;
	private float fFadeStartTime;
	
	public float fFadeStartDelayTime;
	public float fFadeEndDelayTime;

	public Func<System.Object, bool> OnFadeInFinish;
	public Func<System.Object, bool> OnFadeOutFinish;
	
	private System.Object funcFadeInParam;
	private System.Object funcFadeOutParam;
	
	public bool IsFading { get; private set; }

	private eFade currFadeMode;
	
	private bool bPauseAtEnd;
	
	
	public bool IsFadeInState {
		
		get {
			
			if( currFadeMode == eFade.E_FADE_IN )
				return true;
			
			return false;
		}
	}
	
	public bool IsFadeOutState {
		
		get {
			
			if( currFadeMode == eFade.E_FADE_OUT )
				return true;
			
			return false;
		}
	}
	
	
	

	
	
	// Initialize the texture, background-style and initial color:
	public void Init()
	{
		IsFading = false;	
		
		backGroundStyle = new GUIStyle();		
		
		currColor = Color.clear;					
		startColor = Color.black;
		targetColor = Color.clear;					
		
		fFadeGUIDepth = -1000;	 
		
		fFadeDurTime = 0.0f;		
		fFadeStartTime = 0.0f;
		
		fFadeStartDelayTime = 0.0f;
		fFadeEndDelayTime = 0.2f;		
		
		OnFadeInFinish = null;
		OnFadeOutFinish = null;
		
		fadeTexture = new Texture2D(1, 1);        
        backGroundStyle.normal.background = fadeTexture;
		
		IsFading = false;
		currFadeMode = eFade.E_FADE_NONE;		
		
		funcFadeInParam = null;
		funcFadeOutParam = null;
		
		bPauseAtEnd = false;
	}
	
	
	void Awake()
	{
		// enroll game object singleton
		SingletonGameObject<ScreenFader>.AddSingleton( this );
		
		ScreenFader.Instance.Init();	
		
		//UnityEngine.Debug.Log("ScreenFader::Awake() called!!!!");
	}	
 
	// Draw the texture and perform the fade:
	void OnGUI()
    {   
		// fading
		if( IsFading ) {	
			
			if( Time.time > fFadeStartTime + fFadeStartDelayTime ) {
				
				float fTotalFadingTime;
				fTotalFadingTime = fFadeStartTime + fFadeDurTime + fFadeEndDelayTime;				
				
				// fading time over
				if( Time.time > fTotalFadingTime ) {				
					
					SetCurrentColor(targetColor);					
					IsFading = false;							
				}
				// in fading
				else {
					// because multiple call on OnGUI
					float fColorRatio = Mathf.Clamp01( (Time.time - fFadeStartTime) / fFadeDurTime );				
					
					// Fade!				
					SetCurrentColor( Color.Lerp( startColor, targetColor, fColorRatio ) );				
				}	
				
				// Only draw the texture when the alpha value is greater than 0:
				if (currColor.a > 0)
				{			
		    		GUI.depth = fFadeGUIDepth;
		    		GUI.Label(new Rect(-10, -10, Screen.width + 10, Screen.height + 10), fadeTexture, backGroundStyle);
				}
			}
		}
		// fade end
		else {
			
			// is Not SAFE Deactive GameObject on OnGUI!!!!!!
			//if( bDestroyAtEnd )
			//	DeActivateFader();
			
			if( bPauseAtEnd ) {
				
				SetCurrentColor(targetColor);				
				
				// Only draw the texture when the alpha value is greater than 0:
				if (currColor.a > 0)
				{			
		    		GUI.depth = fFadeGUIDepth;
		    		GUI.Label(new Rect(-10, -10, Screen.width + 10, Screen.height + 10), fadeTexture, backGroundStyle);
				}
			}
		}		
    }
 
	private void CallFadeCallback() {
		
		if( IsFadeInState ) {
			
			if( OnFadeInFinish != null )
				OnFadeInFinish( funcFadeInParam );			
		}
		else if( IsFadeOutState ) {
			
			if( OnFadeOutFinish != null )
				OnFadeOutFinish( funcFadeOutParam );
		}		
	}
 
	/// <summary>
	/// Sets the color of the screen overlay instantly.  Useful to start a fade.
	/// </summary>
	/// <param name='newScreenOverlayColor'>
	/// New screen overlay color.
	/// </param>
	private void SetCurrentColor( Color color )
	{
		currColor = color;
		fadeTexture.SetPixel(0, 0, currColor);
		fadeTexture.Apply();
	}
	
	
 
	
	
	
	
	/// <summary>
	/// Starts the fade from color newScreenOverlayColor. If isFadeIn, start fully opaque, else start transparent, after a delay, with Func<Object> OnFadeFinish.
	/// </summary>
	/// <param name='newScreenOverlayColor'>
	/// New screen overlay color.
	/// </param>
	/// <param name='fadeDuration'>
	/// Fade duration.
	/// </param>
	/// <param name='fadeDelay'>
	/// Fade delay.
	/// </param>
	/// <param name='OnFadeFinish'>
	/// On fade finish, doWork().
	/// </param>
	
	
	public IEnumerator WaitingForFadeEnd() {
		
		while( IsFading )
			yield return null;
	}
	
	public void StartFade( bool isFadeIn, float fFadeDuration,
							Func<System.Object, bool> OnFadeFinish = null, 
							System.Object cbParam = null, bool bWillPauseAtEnd = false )
	{		
		if (fFadeDuration <= 0.0f) {
			
			DeActivateFader();
			return;
		}	
		
		StartCoroutine( StartFadeInternal( isFadeIn, fFadeDuration, OnFadeFinish, cbParam, bWillPauseAtEnd ) );		
	}	
	
	
	public void StartFadeInOut( float fFadeInDuration, float fFadeOutDuration, 								
								Func<System.Object, bool> OnFadeInFinish = null, System.Object cbFadeInParam = null,
								Func<System.Object, bool> OnFadeOutFinish = null, System.Object cbFadeOutParam = null,
								bool bWillPauseAtEnd = false )
	{	
		if (fFadeInDuration <= 0.0f || fFadeOutDuration <= 0.0f) {
			
			DeActivateFader();
			return;
		}
		
		StartCoroutine( StartFadeInternal( true, fFadeInDuration, OnFadeInFinish, cbFadeInParam, false ) );
		
		StartCoroutine( WaitAndStartFadeInternal( false, fFadeOutDuration, OnFadeOutFinish, cbFadeOutParam, bWillPauseAtEnd ) );
	}	
	
	public void StartFadeOutIn( float fFadeOutDuration, float fFadeInDuration, 							
								Func<System.Object, bool> OnFadeOutFinish = null, System.Object cbFadeOutParam = null,
								Func<System.Object, bool> OnFadeInFinish = null, System.Object cbFadeInParam = null,
								bool bWillPauseAtEnd = false )
	{		
		if (fFadeOutDuration <= 0.0f || fFadeInDuration <= 0.0f) {
			
			DeActivateFader();
			return;
		}		
		
		StartCoroutine( StartFadeInternal( false, fFadeOutDuration, OnFadeOutFinish, cbFadeOutParam, false ) );
		
		StartCoroutine( WaitAndStartFadeInternal( true, fFadeInDuration, OnFadeInFinish, cbFadeInParam, bWillPauseAtEnd ) );					
	}
	
	public void WaitAndStartFade( bool isFadeIn, float fFadeDuration,
									Func<System.Object, bool> OnFadeFinish = null, System.Object cbParam = null,
									bool bWillPauseAtEnd = false ) {
		
		StartCoroutine( WaitAndStartFadeInternal( isFadeIn, fFadeDuration, OnFadeFinish, cbParam, bWillPauseAtEnd ) );
	}	
	
	public IEnumerator StartFadeAndWait( bool isFadeIn, float fFadeDuration,
										Func<System.Object, bool> OnFadeFinish = null, System.Object cbParam = null,
										bool bWillPauseAtEnd = false ) {		
		
		//yield return StartCoroutine( StartFadeAndWaitInternal( isFadeIn, fFadeDuration, OnFadeFinish, cbParam, bWillPauseAtEnd ) );
		yield return StartFadeAndWaitInternal( isFadeIn, fFadeDuration, OnFadeFinish, cbParam, bWillPauseAtEnd );
	}	
	
	
	
	
	
	
	
	// internal method
	IEnumerator StartFadeInternal( bool isFadeIn, float fFadeDuration,
									Func<System.Object, bool> OnFadeFinish = null, 
									System.Object cbParam = null, bool bWillPauseAtEnd = false )
	{		
		IsFading = true;
		bPauseAtEnd = bWillPauseAtEnd;

		if( isFadeIn ) {			
			
			currFadeMode = eFade.E_FADE_IN;
			OnFadeInFinish = OnFadeFinish;		
			funcFadeInParam = cbParam;		
			
			startColor.a = 1;			
			targetColor.a = 0;
			
			SetCurrentColor( startColor );
		} 
		else {
			
			currFadeMode = eFade.E_FADE_OUT;
			OnFadeOutFinish = OnFadeFinish;
			funcFadeOutParam = cbParam;			
			
			startColor.a = 0;			
			targetColor.a = 1;
			
			SetCurrentColor( startColor );
		}
		
		fFadeStartTime = Time.time;
		fFadeDurTime = fFadeDuration;			
	
		
		while( IsFading ) {
			
			yield return null;
		}
		
		
		CallFadeCallback();
		
		currFadeMode = eFade.E_FADE_NONE;			
		
		if( bPauseAtEnd == false )
			DeActivateFader();
	}	
	
	IEnumerator WaitAndStartFadeInternal( bool isFadeIn, float fFadeDuration,
											Func<System.Object, bool> OnFadeFinish = null, 
											System.Object cbParam = null,
											bool bWillPauseAtEnd = false ) {
		
		while( IsFading ) {
			
			yield return null;
		}
		
		StartCoroutine( StartFadeInternal( isFadeIn, fFadeDuration, OnFadeFinish, cbParam, bWillPauseAtEnd ) );
	}	
	
	IEnumerator StartFadeAndWaitInternal( bool isFadeIn, float fFadeDuration,
											Func<System.Object, bool> OnFadeFinish = null, 
											System.Object cbParam = null,
											bool bWillPauseAtEnd = false ) {	
		
		StartCoroutine( StartFadeInternal( isFadeIn, fFadeDuration, OnFadeFinish, cbParam, true ) );		

		while( IsFading ) {
			
			yield return null;
		}	
		
		if( bWillPauseAtEnd == false )
			DeActivateFader();
	}
	
	
	
	
	void ActivateFader()
	{
		if( gameObject.activeSelf == false )
			gameObject.SetActive( true );			
	}
	
	void DeActivateFader()
	{	
		if( gameObject.activeSelf )
			gameObject.SetActive( false );		
	}

	
	
	
	
	
	// static
	public static ScreenFader Instance {
		
		get { 
			
			ScreenFader screenFader = SingletonGameObject<ScreenFader>.Instance;
			
			if( screenFader != null ) {
				screenFader.ActivateFader();
				return screenFader;
			}
			
			return null;
		} 
	}
}