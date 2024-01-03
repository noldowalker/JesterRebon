using Code.Boot.GlobalEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Boot.GlobalEvents.DataObjects
{
    class SkillDto : GlobalEventData
    {
        public string button;
        public Transform playerTransform;
    }
}
