using Assets.Code.Boot.GlobalEvents.DataObjects;
using Assets.Code.Boot.GlobalEvents.Enum;
using Code.Boot.GlobalEvents;
using Code.Boot.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Code.Boot.Systems
{
    public partial class LevelSystem : BaseSystem
    {
        protected override void Subscribe()
        {
            GlobalEventsSystem<EnemyHitDto>.Sub(GlobalEventType.ENEMY_HIT, OnEnemyHit);
            GlobalEventsSystem<HitDto>.Sub(GlobalEventType.PLAYER_HIT, OnPlayerHit);
            GlobalEventsSystem<EnemyDanceDto>.Sub(GlobalEventType.ENEMY_DANCING, OnEnemyAffectDancingAura);
            GlobalEventsSystem<SkillDto>.Sub(GlobalEventType.ACTIVATE_SKILL, OnActivateSkill);
            base.Subscribe();
        }

        protected override void Unsubscribe()
        {
            GlobalEventsSystem<EnemyHitDto>.Unsub(GlobalEventType.ENEMY_HIT, OnEnemyHit);
            GlobalEventsSystem<HitDto>.Unsub(GlobalEventType.PLAYER_HIT, OnPlayerHit);
            GlobalEventsSystem<EnemyDanceDto>.Unsub(GlobalEventType.ENEMY_DANCING, OnEnemyAffectDancingAura);
            GlobalEventsSystem<SkillDto>.Unsub(GlobalEventType.ACTIVATE_SKILL, OnActivateSkill);
            base.Unsubscribe();
        }

        private void OnActivateSkill(SkillDto dto)
        {
            _playerSkillHandling.ActivateSkill(dto.button, dto.playerTransform);
        }
        private void OnEnemyHit(EnemyHitDto dto)
        {
            _npcActorsSystem.HandleEnemyHit(dto.enemy, dto.damage, dto.force, dto.timeOfStun, dto.direction);
        }
        private void OnPlayerHit(HitDto dto)
        {
            _playerCharacterControlSystem.HandlePlayerHit(dto.damage, dto.force, dto.pushTime , dto.direction, dto.timeOfStun);
        }

        private void OnEnemyAffectDancingAura(EnemyDanceDto dto)
        {
            _npcActorsSystem.StartDancing(dto.enemy, dto.duration, dto.damage, dto.damagePeriod);
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }
    }
}
