using UnityEngine;
using UnityEngine.EventSystems;
 
public class EventTriggerListener : EventTrigger
{
    public delegate void voidDelegate(GameObject obj);
    public voidDelegate OnBtnDown;
    public voidDelegate OnBtnUp;
    public voidDelegate OnEnter;
    public voidDelegate OnExit;
    public voidDelegate OnStay;
    public voidDelegate OnClick;
    public voidDelegate OnDoubleClick;
    private float t1, t2;
    private bool isEnter = false;
    public void Update()
    {
        if (isEnter)
        {
            if (OnStay != null)
            {
                OnStay(gameObject);
            }
        }
    }
    public static EventTriggerListener Get(GameObject obj)
    {
        EventTriggerListener eventTriggerListener = obj.GetComponent<EventTriggerListener>();
        if (eventTriggerListener == null)
        {
            eventTriggerListener = obj.AddComponent<EventTriggerListener>();
        }
        return eventTriggerListener;
    }
    /// <summary>
    /// 判断是否是第二次点击
    /// </summary>
    private bool isTwo = true;
    private bool isOver = true;
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (OnBtnDown != null)
        {
            OnBtnDown(gameObject);
        }
        isTwo = !isTwo;
        if (!isOver)
        {
            return;
        }
        isOver = false;
        Invoke("SelectClick", 0.3f);
       
    }
    public void SelectClick()
    {
        if (isTwo)
        {
            if (OnDoubleClick!=null)
            {
                OnDoubleClick(gameObject);
            }
        }
        else
        {
            if (OnClick!=null)
            {
                OnClick(gameObject);
            }
        }
        isTwo = true;
        isOver = true;
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        if (OnBtnUp != null)
        {
            OnBtnUp(gameObject);
        }
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (OnEnter != null)
        {
            OnEnter(gameObject);
        }
        isEnter = true;
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        if (OnExit != null)
        {
            OnExit(gameObject);
        }
        isEnter = false;
    }
}