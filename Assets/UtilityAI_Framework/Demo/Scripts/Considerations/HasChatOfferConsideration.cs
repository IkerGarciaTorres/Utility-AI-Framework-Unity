using UnityEngine;

namespace UtilityAI.Considerations
{
    [CreateAssetMenu(fileName = "Cons_HasChatOffer", menuName = "UtilityAI/Considerations/Has Chat Offer")]
    public class HasChatOfferConsideration : Consideration
    {
        protected override float GetRawInput()
        {
            if (blackboard != null && blackboard.Exists("ChatOffer"))
            {
                if (blackboard.Get<GameObject>("ChatOffer") != null)
                    return 1f;
            }
            return 0f;
        }
    }
}