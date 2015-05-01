using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RQ_HunoController
{
    public class SAM3SmartServo
    {
        byte header = 255;

        public SAM3SmartServo(int comingID, byte comingMaxPosition, byte comingMinPosition,byte comingInitPosition, int comingTorque = 2)
        {
            SetID(comingID);
            SetTorque(comingTorque);
            _maxPosition = comingMaxPosition;
            _minPosition = comingMinPosition;
            _initPosition = comingInitPosition;
        }        

        private int _id;

        public int ID
        {
            get { return _id; }
        }

        private byte _position;

        public byte Position
        {
            get { return _position; }
            set { _position = value; }
        }

        private int _torque;

        public int Torque
        {
            get { return _torque; }
            set 
            {
                _torque = SetTorque(value);                           
            }
        }

        private byte _maxPosition;

        public byte MaxPosition
        {
            get { return _maxPosition; }
        }
        private byte _minPosition;

        public byte MinPosition
        {
            get { return _minPosition; }
        }
        private byte _initPosition;

        public byte InitPosition
        {
            get { return  _initPosition; }
        }


        //ID must be in interval of 0 - 30
        private void SetID(int comingID)
        {
            if (comingID >= 0 && comingID <= 30)
            {
                _id = comingID;
            }
            else
            {
                throw new Exception("ID must be in interval of 0 - 30");
            }
        }
        //Torque must be in interval of 0 - 4
        public static int SetTorque(int value)
        {
            if (value >= 0 && value <= 4)
            {
                return value;
            }
            else
            {
                throw new Exception("Torque must be in interval of 0 - 4");
            }
        }
        //Returns two byte; first one for Load, second one for Position. 
        public Byte[] StatusRead(SerialPort port)
        {
            int mode = 5;
            byte data1 = (byte)(_id + mode + 32);
            byte data2 = 0; // An arbitrary value
            byte checksum = (byte)(data1 ^ data2);
            List<byte> buffer = new List<byte>();

            port.Write(new byte[] { header, data1, data2, checksum }, 0, 4);
            buffer = ReadByte(port, buffer);

            return buffer.ToArray();
        }
        //Returns two byte; first one for Load, second one for Position. 
        public void PositionMove(byte targetPosition, SerialPort port)
        {
            if (targetPosition> MaxPosition)
            {
                targetPosition = MaxPosition;
            }
            else if (targetPosition< MinPosition)
            {
                targetPosition = MinPosition;
            }
            byte data1 = (byte)(_id + (_torque * 32));
            byte checksum = (byte)((data1 ^ targetPosition)&(byte)0x7F);

            port.Write(new byte[] { header, data1, targetPosition, checksum }, 0, 4);
                      
        }

        private static List<byte> ReadByte(SerialPort port, List<byte> buffer)
        {
            while (true)
            {
                int readed = port.ReadByte();
                if (readed < 0)
                {
                    break;
                }
                else
                {
                    buffer.Add((byte)readed);
                }
            }
            return buffer;
        }
    }
}
