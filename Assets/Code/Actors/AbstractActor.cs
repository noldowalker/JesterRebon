using System.Collections.Generic;
using System.Linq;
using Code.Actors.Behaviours;
using Code.Actors.Player.Settings;
using UnityEngine;

namespace Code.Actors
{
    public abstract class AbstractActor : MonoBehaviour
    {
        public abstract void Init(AbstractSettings settings);
    }
}