using Code.Actors.Behaviours.BehaviourSettings;
using UnityEngine;

namespace Code.Actors.Behaviours
{
    public class IdleBehaviour : AbstractBehaviour
    {
        [Tooltip("Time before NPC will decide what to do next")]
        [SerializeField] private float reactionTime;

        private bool _disableAi;

        public override BehaviourType Type  => BehaviourType.Idle;
        
        private float _timePastSinceStart;
        
        public override void Act()
        {
            if (_disableAi)
                return;
            
            _timePastSinceStart += Time.deltaTime;
            
            if (_timePastSinceStart < reactionTime)
                return;
            
            actor.MakeDecision();
        }

        public override void OnStart<T>(T settings)
        {
            var idleSettings = settings as IdleBehaviourSettings;
            _disableAi = idleSettings.disableAI;
            _timePastSinceStart = 0;
        }

        public override void OnEnd()
        {
        }
    }
}