using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RQ_HunoController
{
    public class Huno
    {
        public SAM3SmartServo[] servoArray;
        private byte[] maxPositionArray = new byte[16] { 200, 220, 220, 140, 215, 254, 175, 195, 215, 215, 254, 220, 220, 254, 220, 220 };
        private byte[] minPositionArray = new byte[16] { 1, 80, 60, 40, 40, 55, 25, 25, 115, 40, 1, 25, 25, 1, 25, 25 };
        private byte[] initPositionArray = new byte[16] { 123, 192, 124, 110, 126, 123, 60, 126, 145, 126, 49, 47, 40, 199, 204, 211 };
        public Huno()
        {           
            List<SAM3SmartServo> servos = new List<SAM3SmartServo>() ;
            for (int i = 0; i < 16; i++)
			{
                servos.Add(new SAM3SmartServo(i, maxPositionArray[i], minPositionArray[i], initPositionArray[i]));
			}
            servoArray = servos.ToArray();
            _generalTorque = 2;
        }

        private int _generalTorque;

        public int GeneralTorque
        {
            get { return _generalTorque; }
            set
            {
                _generalTorque = SAM3SmartServo.SetTorque(value);
            }
        }
        public void SendRemoteControlCommand(SerialPort port,RemoteControlFunctions rcf)
        {
            port.Write(new byte[6]{0xFF,0xE0,0xFB,0x01,0x00,0x1A}, 0, 6);
           
            byte[] move = new byte[16] { 0xFF, 0xFF, 0xAA, 0x55, 0xAA, 0x55, 0x37, 0xBA, 0x14, 0x01, 0x00, 0x00, 0x00, 0x01, 0x0A, 0x0A };
            move[14] = (byte)rcf;
            move[15] = (byte)rcf;
            
            port.Write(move, 0, 16);
          
        }
        public void SendOpeningProtocol(SerialPort port)
        {
            port.Write(new byte[] { 0xFF, 0xFF, 0xAA, 0x55, 0xAA, 0x55, 0x37, 0xBA, 0x10, 0x65, 0x00, 0x00, 0x00, 0x01, 0x01, 0x01 }, 0, 16);
        }
        public void MoveToInitPosition(SerialPort port)
        {
            foreach (var item in servoArray)
            {
                item.PositionMove(initPositionArray[item.ID], port);
            }
        }
        public void MoveAllServos(byte[] targetPositions, SerialPort port)
        {
            byte[] checksums = calculateChecksum(_generalTorque, targetPositions);

            port.Write(new byte[] { 
                  0xFF, (byte)(0 + (_generalTorque * 32)), targetPositions[0], checksums[0]
                , 0xFF, (byte)(1 + (_generalTorque * 32)), targetPositions[1], checksums[1] 
                , 0xFF, (byte)(2 + (_generalTorque * 32)), targetPositions[2], checksums[2] 
                , 0xFF, (byte)(3 + (_generalTorque * 32)), targetPositions[3], checksums[3] 
                , 0xFF, (byte)(4 + (_generalTorque * 32)), targetPositions[4], checksums[4] 
                , 0xFF, (byte)(5 + (_generalTorque * 32)), targetPositions[5], checksums[5] 
                , 0xFF, (byte)(6 + (_generalTorque * 32)), targetPositions[6], checksums[6] 
                , 0xFF, (byte)(7 + (_generalTorque * 32)), targetPositions[7], checksums[7] 
                , 0xFF, (byte)(8 + (_generalTorque * 32)), targetPositions[8], checksums[8] 
                , 0xFF, (byte)(9 + (_generalTorque * 32)), targetPositions[9], checksums[9] 
                , 0xFF, (byte)(10+ (_generalTorque * 32)), targetPositions[10], checksums[10] 
                , 0xFF, (byte)(11+ (_generalTorque * 32)), targetPositions[11], checksums[11] 
                , 0xFF, (byte)(12+ (_generalTorque * 32)), targetPositions[12], checksums[12] 
                , 0xFF, (byte)(13+ (_generalTorque * 32)), targetPositions[13], checksums[13] 
                , 0xFF, (byte)(14+ (_generalTorque * 32)), targetPositions[14], checksums[14] 
                , 0xFF, (byte)(15+ (_generalTorque * 32)), targetPositions[15], checksums[15] 
            }, 0, 64);
        }

        private byte[] calculateChecksum(int _generalTorque, byte[] targetPositions)
        {
            byte[] checksums = new byte[16];
            for (int i = 0; i < 16; i++)
            {
                checksums[i] = (byte)(((i + (_generalTorque*32)) ^ targetPositions[i]) & 0x7F);
            }
            return checksums;
         //   (byte)((data1 ^ targetPosition) & (byte)0x7F);
        }
        public void SynchronizedPositionMove(byte[] targetPositions, SerialPort port)
        {
            const byte header = 255;
            byte data1 = (byte)(31 + (_generalTorque << 5));
            int lastID = 15;
            byte data2 = (byte)(lastID +1 ) ;
            byte checksum = 0;
            for (int i = 0; i <= lastID; i++)
            {
                checksum =(byte) (checksum ^ targetPositions[i]);
            }
            checksum = (byte)(checksum & 0x7F);
            //byte checksum = (byte)((targetPositions[0] ^/* targetPositions[1] ^ targetPositions[2] ^ targetPositions[3] ^ targetPositions[4] ^ targetPositions[5] ^ targetPositions[6] ^ targetPositions[7] ^ targetPositions[8] ^ targetPositions[9] ^ targetPositions[10] ^ targetPositions[11] ^ targetPositions[12] ^ targetPositions[13] ^ targetPositions[14] ^*/ targetPositions[15]/* ^ (byte)(lastID+3) */) & (byte)0x7F);

            port.Write(new byte[] {header, data1,data2, targetPositions[0],targetPositions[1],targetPositions[2],targetPositions[3],targetPositions[4],targetPositions[5],targetPositions[6],targetPositions[7],targetPositions[8],targetPositions[9],targetPositions[10],targetPositions[11],targetPositions[12],targetPositions[13],targetPositions[14],targetPositions[15], checksum }, 0, 20);
   

        }
    }
}
