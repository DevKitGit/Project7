using UnityEngine;
using UnityEngine.Events;

namespace Devkit.Modularis.Events
{
    public class GameEventListener : MonoBehaviour
    {
        [Tooltip("GameEvent to register with.")]
        [SerializeField] private GameEvent gameEvent;
        
        [Tooltip("Response to invoke when GameEvent is raised.")]
        [SerializeField] private UnityEvent response;

        
        private void OnEnable()
        {
            if (gameEvent == null) return;
            gameEvent.RegisterListener(this);
        }

        private void OnDisable()
        {
            if (gameEvent == null) return;
            gameEvent.UnregisterListener(this);
        }

        public void SetResponseEvent(UnityEvent newUnityEvent)
        {
            response = newUnityEvent;
        }

        public void WipeResponseInvocationList()
        {
            response.RemoveAllListeners();
        }
        
        public void SetListeningEvent(GameEvent gameEvent)
        {
            if (this.gameEvent == null)
            {
                this.gameEvent = gameEvent;
                return;
            }
            this.gameEvent.UnregisterListener(this);
            this.gameEvent = gameEvent;
            this.gameEvent.RegisterListener(this);
        }
        public void OnGameEventInvoked()
        {
            response.Invoke();
        }
    }
}