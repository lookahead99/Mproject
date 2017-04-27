
namespace XFrameWork
{
    public interface ILuaManager
    {
        void Init();

        void SetLuaPath(string luaPath);

        void InitLuaLoader();
        object[] DoFile(string filename);

        object[] DoString(string _chunk);

        void LuaGc();

        void Release();
    }
}

