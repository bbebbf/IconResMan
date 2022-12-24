using System.Runtime.InteropServices;

namespace IconResMan
{
    public class WinapiTypes
    {
        public const uint LOAD_LIBRARY_AS_DATAFILE = 0x00000002;

        public const uint FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100;
        public const uint FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200;
        public const uint FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;
        public const uint FORMAT_MESSAGE_ARGUMENT_ARRAY = 0x00002000;
        public const uint FORMAT_MESSAGE_FROM_HMODULE = 0x00000800;
        public const uint FORMAT_MESSAGE_FROM_STRING = 0x00000400;

        public const uint ERROR_RESOURCE_DATA_NOT_FOUND = 1812;
        public const uint ERROR_RESOURCE_TYPE_NOT_FOUND = 1813;
        public const uint ERROR_RESOURCE_NAME_NOT_FOUND = 1814;
        public const uint ERROR_RESOURCE_LANG_NOT_FOUND = 1815;

        public enum ResourceType
        {
            CURSOR = 1,
            BITMAP = 2,
            ICON = 3,
            MENU = 4,
            DIALOG = 5,
            STRING = 6,
            FONTDIR = 7,
            FONT = 8,
            ACCELERATOR = 9,
            RCDATA = 10,
            MESSAGETABLE = 11,
            GROUP_CURSOR = 12,
            GROUP_ICON = 14,
            VERSION = 16,
            DLGINCLUDE = 17,
            PLUGPLAY = 19,
            VXD = 20,
            ANICURSOR = 21,
            ANIICON = 22,
            HTML = 23,
            MANIFEST = 24
        }

        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct GroupIconRsrcHeader
        {
            public ushort wReserved; // 0
            public ushort wTyp;      // 1 for icons
            public ushort wCount;    // number of icons in this group, each has a following TGroupIconRsrcEntry
        }

        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct GroupIconRsrcEntry
        {
            public byte bWidth;          // 0 means 256, which is the maximum
            public byte bHeight;         // 0 means 256, which is the maximum
            public byte bColorCount;     // number of colors in image (0 if wBitCount > 8)
            public byte bReserved;       // 0
            public ushort wPlanes;       // 1
            public ushort wBitCount;     // number of bits per pixel
            public uint dwSize;          // size of the icon data, in bytes
            public ushort wID;           // resource ID of the icon (for RT_ICON entry)
        }
    }
}
