using Code.Actors.Player;
using Code.Actors.Player.Settings;
using UnityEngine;

namespace Code.ScriptableObjects
{
    [CreateAssetMenu(fileName = "UntitledPlayerControlledActorContainer", menuName = "Scriptable Objects/Settings/Create Player Controlled Actor Collection", order = 0)]
    public class PlayerControlledActorContainer : ScriptableObject
    {
        [SerializeField] private PlayerControlledActor prefab;

        [SerializeField] private PlayerCharacterSettings settings;

        public PlayerControlledActor CreateInstance()
        {
            var instance = GameObject.Instantiate(prefab);
            instance.Init(settings);
            
            return instance;
        }
    }
}