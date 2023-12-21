using Assets.Code.Boot.GlobalEvents.DataObjects;
using Assets.Code.Boot.GlobalEvents.Enum;
using Code.Boot.GlobalEvents;
using Code.Boot.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Code.Boot.Systems
{
    public partial class LevelSystem : BaseSystem
    {
        protected override void Subscribe()
        {
            GlobalEventsSystem<EnemyHitDto>.Sub(GlobalEventType.ENEMY_HIT, OnEnemyHit);
            GlobalEventsSystem<HitDto>.Sub(GlobalEventType.PLAYER_HIT, OnPlayerHit);
            base.Subscribe();
        }

        protected override void Unsubscribe()
        {
            GlobalEventsSystem<EnemyHitDto>.Unsub(GlobalEventType.ENEMY_HIT, OnEnemyHit);
            GlobalEventsSystem<HitDto>.Unsub(GlobalEventType.PLAYER_HIT, OnPlayerHit);
            base.Unsubscribe();
        }

        private void OnEnemyHit(EnemyHitDto dto)
        {
            _npcActorsSystem.HandleEnemyHit(dto.enemy, dto.damage, dto.force, dto.timeOfStun, dto.direction);
        }
        private void OnPlayerHit(HitDto dto)
        {
            _playerCharacterControlSystem.HandlePlayerHit(dto.damage, dto.force, dto.pushTime , dto.direction, dto.timeOfStun);
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }
    }
}
