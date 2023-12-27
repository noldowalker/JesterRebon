using System;
using Code.Boot.Logging;
using UnityEngine;

namespace Code.Vfx
{
    public class FollowVfxSetup : AbstractVfxSetup
    {
        [SerializeField] private float playtime;
        
        public override VfxType Type => VfxType.Follow;

        private float _timePast;
        
        public override void Start()
        {
            DebugExtension.DebugNotice($"+++ Start {Name}");
            IsActive = true;
            _timePast = 0;
            effect.enabled = true;
            effect.Reinit();
            effect.Play();
        }

        public override void Observe()
        {
            DebugExtension.DebugNotice($"+++ Observe {Name}");
            _timePast += Time.deltaTime;
            if (_timePast < playtime)
                return;

            Stop();
        }

        public override void Stop()
        {
            DebugExtension.DebugNotice($"+++ Stop {Name}");
            IsActive = false;
            effect.Stop();
            effect.enabled = false;
        }
    }
}