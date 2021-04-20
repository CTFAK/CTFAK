namespace CTFAK_Runtime_Tools
{
    public static class Pointers
    {
        public const int PAMU_HEADER_OLD_PTR = 0xAC9AC;
        public const int PAMU_HEADER_NEW_PTR = 0xB49CC;
        public const int CURRENT_FRAME_OFFSET = 0x1D8;
        public const int OBJ_LIST_OFFSET = 0x190;
        public const int SND_LIST_OFFSET = 0x1C4;
        public const int KEY_DECODE_OFFSET = 0x26C;
        public static bool New=false;

        public static int PamuHeaderOff
        {
            get
            {
                if (New) return PAMU_HEADER_NEW_PTR;
                else return PAMU_HEADER_OLD_PTR;
             }
        }
    }
}