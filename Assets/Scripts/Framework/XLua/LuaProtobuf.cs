using System.Runtime.InteropServices;

namespace XLua.LuaDLL
{
    public partial class Lua
    {
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_pb(System.IntPtr L);

        [MonoPInvokeCallback(typeof(lua_CSFunction))]
        public static int LoadLuaProtobuf(System.IntPtr L)
        {
            return luaopen_pb(L);
        }
    }

}
