using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// UI的渐入渐出
/// </summary>
public class UI_FadeInOut : MonoBehaviour {
    private float UI_Alpha = 1;             //初始化时让UI显示
    public float FadeTime = 2f;          //渐隐渐显的速度
    private float currentTime = 0f;
    private CanvasGroup canvasGroup;
    private bool startFadeAnim = false;
 
	// Use this for initialization
	void Awake () {
        canvasGroup = this.GetComponent<CanvasGroup>();
    }
	
	// Update is called once per frame
	void Update () {
        if (canvasGroup == null)
        {
            return;
        }
 
        if (startFadeAnim)
        {
            currentTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, UI_Alpha, currentTime / FadeTime);
            if (Mathf.Abs(UI_Alpha - canvasGroup.alpha) <= 0.01f)
            {
                canvasGroup.alpha = UI_Alpha;
                startFadeAnim = false;
            }
        }
	}
 
    public void UI_FadeIn_Event()
    {
        UI_Alpha = 1;
        canvasGroup.blocksRaycasts = true;      //可以和该对象交互
        currentTime = 0f;
        startFadeAnim = true;
    }
 
    public void UI_FadeOut_Event()
    {
        UI_Alpha = 0;
        canvasGroup.blocksRaycasts = false;     //不可以和该对象交互
        currentTime = 0f;
        startFadeAnim = true;
    }
 
 
}