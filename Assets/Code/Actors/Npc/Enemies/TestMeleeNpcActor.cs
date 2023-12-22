using System.Linq;
using Code.Actors.Behaviours;
using Code.Actors.Extensions;
using Code.Actors.Npc.Enemies.Settings;
using Code.Actors.Player.Settings;
using Code.Boot.Logging;
using Code.Helpers.GlobalExtensions;
using UnityEngine;

namespace Code.Actors.Npc.Enemies
{
    public class TestMeleeNpcActor : NpcActor
    {
        public override void Init(AbstractSettings settings)
        {
            base.Init(settings);

            DebugExtension.InitNotice("TestMeleeNpcActor initiated");
        }

        public override void MakeDecision()
        {
            if (_playerTransform.IsNull())
            {
                ChangeBehaviourTo(BehaviourType.Search);
                return;
            }
            
            if (InAttackDistance(_playerTransform))
            {
                MeleeAttackTarget(_playerTransform);
                return;
            }
            
            ChangeBehaviourTo(BehaviourType.Move);
        }
        
        public void Act()
        {
            if (currentBehaviour.IsNull())
                return;
            
            currentBehaviour.Act();
        }
    }
}