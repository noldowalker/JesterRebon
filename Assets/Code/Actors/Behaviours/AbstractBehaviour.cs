using System;
using Code.Actors.Behaviours.BehaviourSettings;
using UnityEditor;
using UnityEngine;

namespace Code.Actors.Behaviours
{
    [RequireComponent(typeof(NpcActor))]
    public abstract class AbstractBehaviour : MonoBehaviour
    {
        [SerializeField] protected int priority;
        [SerializeField] protected NpcActor actor;
        public Action onBehaviourEnd;

        public virtual BehaviourType Type => BehaviourType.Idle;
        public int Priority => priority;
        
        public abstract void Act();
        public abstract void OnStart<T>(T settings) where T : AbstractBehaviourSettings;
        public abstract void OnEnd();


        private void OnValidate()
        {
            actor = GetComponent<NpcActor>();
        }
    }
    
    [CustomEditor(typeof(AbstractBehaviour), true)]
    public class AbstractBehaviourEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var behaviour = target as AbstractBehaviour;

            if (behaviour != null)
            {
                EditorGUILayout.LabelField("Type", behaviour.Type.ToString());
                DrawDefaultInspector();
            }
        }
    }
}