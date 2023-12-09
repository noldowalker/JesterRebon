using UnityEngine;

namespace Code.Boot.Systems
{
    public class BaseSystem : MonoBehaviour
    {
        protected void Subscribe()
        {
            // подписки на события
        }

        protected void Unsubscribe()
        {
            // отписки от событий
        }
        
        private void OnDestroy()
        {
            Unsubscribe();
        }
    }
}