using Code.Boot.GlobalEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Boot.GlobalEvents.DataObjects
{
    class EnemyDanceDto : GlobalEventData
    {
        public GameObject enemy;
        public float duration;
        public float damage;
        public float damagePeriod;
    }
}
