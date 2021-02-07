using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CTFAK_Runtime
{
    public class NativeUtils
    {
        [DllImport("user32.dll")]
        static extern ushort GetAsyncKeyState(int vKey);
        
        public static bool IsKeyPushedDown(Keys keyData)
        {
           return 0 != (GetAsyncKeyState((int)keyData) & 0x8000);
        }
    }
}