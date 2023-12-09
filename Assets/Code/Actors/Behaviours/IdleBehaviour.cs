using UnityEngine;

namespace Code.Actors.Behaviours
{
    public class IdleBehaviour : AbstractBehaviour
    {
        [Tooltip("Time before NPC will decide what to do next")]
        [SerializeField] private float reactionTime;
        public override BehaviourType Type  => BehaviourType.Idle;
        
        private float _timePastSinceStart;
        
        public override void Act()
        {
            _timePastSinceStart += Time.deltaTime;
            
            if (_timePastSinceStart < reactionTime)
                return;
            
            actor.MakeDecision();
        }

        public override void OnStart()
        {
            _timePastSinceStart = 0;
        }

        public override void OnEnd()
        {
        }
    }
}