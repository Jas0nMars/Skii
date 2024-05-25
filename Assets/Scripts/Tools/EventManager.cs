using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
 
 
public enum EventKey
{
    //SDK
    KeyTest,
    PlayerDie,
    PlayerReborn,
    UpdateProgress,
    UpdateScore,
    ShowTips,
    LevelFinish,
    ResetLevel,
}
 
 
public class EventManager
{
    private class EventNode : UnityEvent<object>
    {
 
 
    }
 
 
    private Dictionary<EventKey, EventNode> eventDictionary = new Dictionary<EventKey, EventNode>();
    private static EventManager eventManager = new EventManager();
 
 
    public static EventManager Instance
    {
        get
        {
            return eventManager;
        }
    }
    public void RegisterEvent(EventKey eventKey, UnityAction<object> listener)
    {
        EventNode thisEvent = null;
        if (eventManager.eventDictionary.TryGetValue(eventKey, out thisEvent))
        {
            
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new EventNode();
            thisEvent.AddListener(listener);
            eventManager.eventDictionary.Add(eventKey, thisEvent);
        }
    }
 
 
    public void RemoveListening(EventKey eventKey, UnityAction<object> listener)
    {
        if (eventManager == null) return;
        EventNode thisEvent = null;
        if (eventManager.eventDictionary.TryGetValue(eventKey, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }
 
 
    public void TriggerEvent(EventKey eventKey,object mess)
    {
        EventNode thisEvent = null;
        if (eventManager.eventDictionary.TryGetValue(eventKey, out thisEvent))
        {
            thisEvent.Invoke(mess);
        }
    }
}