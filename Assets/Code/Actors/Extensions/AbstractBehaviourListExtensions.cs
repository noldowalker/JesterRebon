using System.Collections.Generic;
using System.Linq;
using Code.Actors.Behaviours;
using Code.Actors.Behaviours.BehaviourSettings;

namespace Code.Actors.Extensions
{
    public static class AbstractBehaviourListExtensions
    {
        public static AbstractBehaviour GetHighestPriorityBehaviour(this List<AbstractBehaviour> behaviours, BehaviourType type)
        {
            return behaviours
                .Where(b => b.Type == type)
                .OrderByDescending(b => b.Priority)
                .FirstOrDefault();
        }
    }
}