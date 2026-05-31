using UnityEngine;
using System.Collections.Generic;

namespace UtilityAI.Traits
{
    [CreateAssetMenu(menuName = "UtilityAI/Traits/Personality Profile")]
    public class PersonalityProfile : ScriptableObject
    {
        public string profileName = "New Personality";
        public List<Trait> traits = new List<Trait>();

        public bool TryGetModifiers(string considerationID, out float offset, out float multiplier)
        {
            offset = 0f;
            multiplier = 1f;

            if (string.IsNullOrEmpty(considerationID)) return false;

            foreach (Trait trait in traits)
            {
                if (trait.targetConsiderationID == considerationID)
                {
                    offset = trait.thresholdOffset;
                    multiplier = trait.importanceMultiplier;
                    return true;
                }
            }

            return false;
        }
    }
}