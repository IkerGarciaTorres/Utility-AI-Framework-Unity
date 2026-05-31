using UnityEngine;

namespace UtilityAI
{
    public interface IUtilityNode
    {
        string Name { get; }
        void Contextualize(GameObject gm);
        float Evaluate();
        void Clear();
    }
}