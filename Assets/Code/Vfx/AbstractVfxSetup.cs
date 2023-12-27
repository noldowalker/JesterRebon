using System;
using Code.Actors.Behaviours;
using Code.Boot.Logging;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;

namespace Code.Vfx
{
    [RequireComponent(typeof(VisualEffect))]
    public abstract class AbstractVfxSetup : MonoBehaviour
    {
        [SerializeField] protected VisualEffect effect;
        [SerializeField] protected string effectName;
        
        public virtual VfxType Type => VfxType.Undefined;
        public string Name => effectName;
        public bool IsActive { get; protected set; }

        public abstract void Start();
        public abstract void Observe();
        public abstract void Stop();

        public virtual void Init()
        {
            DebugExtension.DebugNotice($"+++ Init {Name}");
            IsActive = false;
            effect.Stop();
            effect.enabled = false;
        }
        
        private void OnValidate()
        {
            effect = GetComponent<VisualEffect>();
        }
    }
    
    [CustomEditor(typeof(AbstractVfxSetup), true)]
    public class AbstractVfxSetupEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var setup = target as AbstractVfxSetup;

            if (setup != null)
            {
                EditorGUILayout.LabelField("Type", setup.Type.ToString());
                DrawDefaultInspector();
            }
        }
    }
}