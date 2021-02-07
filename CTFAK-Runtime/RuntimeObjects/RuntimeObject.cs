using System;
using System.Windows.Forms;
using CTFAK.MMFParser.MFA.Loaders;

namespace CTFAK_Runtime.RuntimeObjects
{
    public class RuntimeObject:PictureBox
    {
        public int ID;
        public int Flags;
        public static RuntimeObject Create(FrameItem item)
        {
            RuntimeObject newObject;
            if (item.ObjectType == 1)
            {
                newObject = Backdrop.Create(item.Loader as CTFAK.MMFParser.MFA.Loaders.mfachunks.Backdrop);
            }
            else
            {
                throw new NotImplementedException("Unsupported Object");
            }

            newObject.ID = item.Handle;
            return newObject;
        }

        public virtual void Spawn(Form window)
        {

        }
        
    }
}