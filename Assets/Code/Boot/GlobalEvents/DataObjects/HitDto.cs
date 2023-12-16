using Code.Boot.GlobalEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Boot.GlobalEvents.DataObjects
{
    class HitDto : GlobalEventData
    {
        public float damage;
        public Vector3 direction;
        public float force;
        public float timeOfStun;
    }
}
