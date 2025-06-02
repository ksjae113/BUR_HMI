using System;
using System.IO.Ports;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Timers;


namespace Serial_gpt
{
    public partial class Form1 : Form
    {

        private SerialPort serialPort;
        List<byte> receiveBuffer = new List<byte>();
        /// <summary>
        private System.Timers.Timer pollTimer;
        /// </summary>

        public Form1()
        {
            InitializeComponent();
            serialPort = new SerialPort();
            serialPort.DataReceived += SerialPort_DataReceived;
            serialPort.BaudRate = 115200;
            serialPort.DataBits = 8;
            serialPort.Parity = Parity.None;
            serialPort.StopBits = StopBits.One;
            txtPortName.Text = "COM2";

            ///
            pollTimer = new System.Timers.Timer(500);   //0.5초마다 DI 읽기
            pollTimer.Elapsed += PollTimer_Elapsed;
            ///

        }



        /*   private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)    //원래쓰던거
           {
               int bytesToRead = serialPort.BytesToRead;
               byte[] buffer = new byte[bytesToRead];
               serialPort.Read(buffer, 0, bytesToRead);

               receiveBuffer.AddRange(buffer);

               // 간단한 Modbus 응답 파싱 (수신된 데이터 길이, CRC 체크 등)
               while (receiveBuffer.Count >= 5)
               {
                   // 여기서는 DI 2개 읽은 응답 길이 계산:  
                   // Slave(1) + Function(1) + ByteCount(1) + Data(2*2) + CRC(2) = 9 바이트
                   int expectedLength = 9;
                   if (receiveBuffer.Count < expectedLength)
                       break;

                   byte[] packet = receiveBuffer.GetRange(0, expectedLength).ToArray();

                   ushort receivedCRC = (ushort)((packet[expectedLength - 1] << 8) | packet[expectedLength - 2]);
                   ushort calculatedCRC = CalculateCRC(packet, expectedLength - 2);

                   if (receivedCRC == calculatedCRC)
                   {
                       byte functionCode = packet[1];
                       if (functionCode == 0x03)
                       {
                           byte byteCount = packet[2];
                           // Data는 packet[3] ~ packet[3 + byteCount -1]
                           // 여기서 DI #1 상태 추출 (첫 2바이트가 DI #1, #2)
                           ushort di1 = (ushort)((packet[3] << 8) | packet[4]);

                           Invoke(new Action(() =>
                           {
                               AppendText("[수신 DI] DI#1 상태: " + di1);

                               // DO #1 제어: DI #1이 0이 아니면 DO #1 ON, 0이면 OFF
                               ushort doValue = (ushort)(di1 != 0 ? 1 : 0);

                               // DO #1은 0x04 주소에 있으므로 쓰기
                               byte slaveId = 1;
                               ushort doAddress = 0x04;

                               byte[] writeFrame = BuildWriteSingleRegisterFrame(slaveId, doAddress, doValue);
                               serialPort.Write(writeFrame, 0, writeFrame.Length);
                               AppendText("[전송 DO] " + BitConverter.ToString(writeFrame));
                           }));

                           receiveBuffer.RemoveRange(0, expectedLength);
                       }
                       else
                       {
                           // 함수 코드 다를 경우 버퍼 정리
                           receiveBuffer.Clear();
                       }
                   }
                   else
                   {
                       Invoke(new Action(() =>
                       {
                           AppendText("[오류] CRC 불일치, 버퍼 초기화");
                       }));
                       receiveBuffer.Clear();
                       break;
                   }
               }
           }*/



         private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)    //원래쓰던거
         {
             int bytesToRead = serialPort.BytesToRead;
             byte[] buffer = new byte[bytesToRead];
             serialPort.Read(buffer, 0, bytesToRead);

             receiveBuffer.AddRange(buffer);

             // Modbus RTU는 최소 5바이트 이상부터 패킷 가능 (주소1, 기능1, 데이터N, CRC2)
             // 여기서는 간단하게 8바이트 이상일 때 검사 및 출력 처리 (필요에 따라 수정)
             while (receiveBuffer.Count >= 5)
             {

                 // 최소 길이 검증
                 byte functionCode = receiveBuffer[1];
                 int expectedLength = 0;

                 if ((functionCode & 0x80) != 0)
                 {
                     // 예외 응답 (Error): [Slave][Function|0x80][Exception Code][CRC Lo][CRC Hi]
                     expectedLength = 5;
                 }
                 else if (functionCode == 0x03)
                 {
                     if (receiveBuffer.Count < 3)
                         break;

                     byte byteCount = receiveBuffer[2];
                     expectedLength = 3 + byteCount + 2; // Header + Data + CRC

                     if (receiveBuffer.Count < expectedLength)
                         break;
                 }
                 else if (functionCode == 0x06)
                 {
                     expectedLength = 8;
                     if (receiveBuffer.Count < expectedLength)
                         break;
                 }
                 else
                 {
                     AppendText($"[오류] 알 수 없는 기능 코드: 0x{functionCode:X2}");
                     receiveBuffer.Clear();
                     return;
                 }
                 byte[] packet = receiveBuffer.GetRange(0, expectedLength).ToArray();

                 ushort receivedCRC = (ushort)((packet[expectedLength - 1] << 8) | packet[expectedLength - 2]);
                 ushort calculatedCRC = CalculateCRC(packet, expectedLength - 2);
                 if (receivedCRC == calculatedCRC)
                 {
                     Invoke(new Action(() =>
                     {
                         AppendText($"[수신] {BitConverter.ToString(packet)}");

                         if ((functionCode & 0x80) != 0)
                         {
                             byte errorCode = packet[2];
                             AppendText($"[예외] Function: 0x{functionCode:X2}, 예외 코드: {errorCode}");
                         }
                         else
                         {
                             AppendText("[정상] CRC 일치");
                         }
                     }));

                     receiveBuffer.RemoveRange(0, expectedLength);
                 }
                 else
                 {
                     Invoke(new Action(() =>
                     {
                         AppendText($"[수신] {BitConverter.ToString(packet)}");
                         AppendText("[오류] CRC 불일치 - 버퍼 정리");
                     }));

                     receiveBuffer.Clear();
                     break;
                 }
                 // 예외 응답 처리 (0x83 등)도 위처럼 분기해서 넣어줘야 함
             }

         }

        private void btnOpenPort_Click(object sender, EventArgs e)
        {

            try
            {
                if (!serialPort.IsOpen)
                {
                    serialPort.PortName = txtPortName.Text; // 예: "COM2"
                    serialPort.Open();
                    AppendText("[포트 열림] " + serialPort.PortName);
                    ///
                    pollTimer.Start();
                    ///

                }
                else
                {
                    AppendText("[포트 이미 열림]");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("포트 열기 실패: " + ex.Message);
            }
        }

        private void btnClosePort_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                /////////
                pollTimer.Stop();
                ///

                serialPort.Close();
                Log("포트 닫힘");
            }
        }

        private void PollTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!serialPort.IsOpen) return;

            // DI #1, #2는 PortTable 0x05번 주소부터 (0x05)
            // 여기서 startAddress=0x05, quantity=2로 DI 읽기 (Modbus 기능코드 0x03 사용)
            byte slaveId = 1;
            ushort startAddress = 0x05;
            ushort quantity = 2;

            byte[] frame = BuildReadHoldingRegistersFrame(slaveId, startAddress, quantity);

            try
            {
                serialPort.Write(frame, 0, frame.Length);
                AppendText("[전송] " + BitConverter.ToString(frame));
            }
            catch (Exception ex)
            {
                AppendText("[전송 오류] " + ex.Message);
            }
        }

        private void btnWriteDO_Click(object sender, EventArgs e)
        {
            if (!serialPort.IsOpen)
            {
                MessageBox.Show("포트를 먼저 열어주세요.");
                return;
            }

            byte slaveId = Convert.ToByte(1);
            ushort registerAddress = Convert.ToUInt16(0);
            ushort valueToWrite = Convert.ToUInt16(txtWriteValue.Text);

            byte[] frame = BuildWriteSingleRegisterFrame(slaveId, registerAddress, valueToWrite);
            serialPort.Write(frame, 0, frame.Length);
            AppendText("[전송] " + BitConverter.ToString(frame));
        }

        private void btnReadDI_Click(object sender, EventArgs e)
        {
            if (!serialPort.IsOpen)
            {
                MessageBox.Show("포트를 먼저 열어주세요.");
                return;
            }

            byte slaveId = Convert.ToByte(1);
            ushort startAddress = Convert.ToUInt16(0);
            ushort quantity = Convert.ToUInt16(18);

            byte[] frame = BuildReadHoldingRegistersFrame(slaveId, startAddress, quantity);
            serialPort.Write(frame, 0, frame.Length);
            AppendText("[전송] " + BitConverter.ToString(frame));
        }

        private void Log(string message)
        {
            txtLog.AppendText(message + Environment.NewLine);
        }

        private byte[] BuildWriteSingleRegisterFrame(byte slaveId, ushort registerAddress, ushort value)
        {
            byte[] frame = new byte[8];
            frame[0] = slaveId;
            frame[1] = 0x06; // Function code Write Single Register
            frame[2] = (byte)(registerAddress >> 8);
            frame[3] = (byte)(registerAddress & 0xFF);
            frame[4] = (byte)(value >> 8);
            frame[5] = (byte)(value & 0xFF);

            ushort crc = CalculateCRC(frame, 6);
            frame[6] = (byte)(crc & 0xFF);       // CRC Low byte
            frame[7] = (byte)(crc >> 8);         // CRC High byte

            return frame;
        }
        private byte[] BuildReadHoldingRegistersFrame(byte slaveId, ushort startAddress, ushort quantity)
        {
            byte[] frame = new byte[8];
            frame[0] = slaveId;
            frame[1] = 0x03; // Function code Read Holding Registers
            frame[2] = (byte)(startAddress >> 8);
            frame[3] = (byte)(startAddress & 0xFF);
            frame[4] = (byte)(quantity >> 8);
            frame[5] = (byte)(quantity & 0xFF);

            ushort crc = CalculateCRC(frame, 6);
            frame[6] = (byte)(crc & 0xFF);       // CRC Low byte
            frame[7] = (byte)(crc >> 8);         // CRC High byte

            return frame;
        }

        private ushort CalculateCRC(byte[] data, int length)
        {
            ushort crc = 0xFFFF;
            for (int pos = 0; pos < length; pos++)
            {
                crc ^= data[pos];
                for (int i = 0; i < 8; i++)
                {
                    if ((crc & 0x0001) != 0)
                        crc = (ushort)((crc >> 1) ^ 0xA001);
                    else
                        crc >>= 1;
                }
            }
            return crc;
        }

        private bool CheckCRC(byte[] data)
        {
            if (data.Length < 3)
                return false;

            int length = data.Length;
            ushort receivedCrc = (ushort)((data[length - 1] << 8) | data[length - 2]); // LSB 먼저, MSB 나중

            ushort calculatedCrc = CalculateCRC(data, length - 2); // CRC 계산은 마지막 2바이트 제외

            return receivedCrc == calculatedCrc;
        }


        private void AppendText(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AppendText(text)));
                return;
            }

            txtReceive.AppendText(text + Environment.NewLine);
        }

        private void checkBoxDO_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
