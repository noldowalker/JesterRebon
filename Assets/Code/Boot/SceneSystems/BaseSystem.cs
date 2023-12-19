using UnityEngine;

namespace Code.Boot.Systems
{
    public class BaseSystem : MonoBehaviour
    {
        protected virtual void Subscribe()
        {
            // подписки на события
        }

        protected virtual void Unsubscribe()
        {
            // отписки от событий
        }
        
        private void OnDestroy()
        {
            Unsubscribe();
        }
    }
}