using System.Collections.Generic;
using UnityEngine;

namespace Devkit.Modularis.Events
{
    [CreateAssetMenu]
    public class GameEvent : ScriptableObject
    {
        #if UNITY_EDITOR
        [Multiline]
        public string developerDescription = "";
        #endif
        
        private readonly List<GameEventListener> _eventListeners = new();
        
        public void Invoke()
        {
            for(var i = _eventListeners.Count -1; i >= 0; i--)
                _eventListeners[i].OnGameEventInvoked();
        }

        public void RegisterListener(GameEventListener listener)
        {
            if (!_eventListeners.Contains(listener))
                _eventListeners.Add(listener);
        }

        public void UnregisterListener(GameEventListener listener)
        {
            if (_eventListeners.Contains(listener))
                _eventListeners.Remove(listener);
        }
    }
}