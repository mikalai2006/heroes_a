using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "GameEvent", menuName = "Game/Event/GameEvent", order = 0)]
public class EntityEvent : ScriptableObject
{
    List<GameEventListener> listeners = new List<GameEventListener>();
    public void TriggerEvent()
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventTriggered();
        }
    }
    public void AddListener(GameEventListener listener)
    {
        listeners.Add(listener);
    }
    public void RemoveListener(GameEventListener listener)
    {
        listeners.Remove(listener);
    }
}