using UnityEngine;
namespace XGame
{
    public interface ILuaBehaviorManager
    {
        // lua Behaviour ------
        int InitLuaBehaviour(MonoBehaviour behaviour);
        void LuaBehaiourUpdate(int id);

        void LuaBehaiourStart(int id);
        void LuaBehaiourDestory(int id);
    }
}

