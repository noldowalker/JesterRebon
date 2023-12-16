using System;
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
        public abstract void OnStart();
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
            var behaviour = (AbstractBehaviour)target;

            EditorGUILayout.LabelField("Type", behaviour.Type.ToString());

            DrawDefaultInspector();
        }
    }
}