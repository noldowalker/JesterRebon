using System;
using System.Collections.Generic;
using System.Linq;
using Code.Helpers.GlobalExtensions;
using UnityEngine;

namespace Code.Vfx
{
    public class VfxSystem : MonoBehaviour
    {
        private List<AbstractVfxSetup> effects;
        private List<AbstractVfxSetup> activeEffects;
        private List<AbstractVfxSetup> stoppingEffects;

        public void Init()
        {
            effects = GetComponentsInChildren<AbstractVfxSetup>().ToList();
            activeEffects = new List<AbstractVfxSetup>(effects.Count);
            stoppingEffects = new List<AbstractVfxSetup>(effects.Count);
            effects.ForEach(e => e.Init());
        }
        
        public void TryTurnOnEffect(string effectName)
        {
            var effect = effects.FirstOrDefault(e => e.Name == effectName);
            if (effect.IsNull())
                return;
            
            effect.Start();
            activeEffects.Add(effect);
        }

        public void ObserveActiveEffects()
        {
            if (activeEffects.Count == 0)
                return;
            
            activeEffects.ForEach(e => e.Observe());

            if (activeEffects.All(e => e.IsActive)) 
                return;
            
            stoppingEffects = activeEffects.Where(e => !e.IsActive).ToList();
            stoppingEffects.ForEach(e => activeEffects.Remove(e));
            stoppingEffects.Clear();
        }

        private void OnDestroy()
        {
            effects.ForEach(e => e.Stop());
            effects.Clear();
        }
    }
}