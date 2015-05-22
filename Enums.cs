using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RQ_HunoController
{
    public enum RemoteControlFunctions
    {
        Up = 0x04,
        Down = 0x0A,
        Right = 0x08,
        Left = 0x06,
        PivotLeft= 0x03,
        PivotRight = 0x05,
        GetUpFaceDown = 0x02,
        GetUpSupine = 0x01,
        One = 0x0C,
        Two = 0x0D,
        Three = 0x0E,
        Four = 0x0F,
        Five = 0x10,
        Six = 0x11,
        Seven = 0x12,
        Eight = 0x13,
        Nine = 0x14

    }
}
