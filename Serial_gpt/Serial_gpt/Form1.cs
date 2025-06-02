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
            pollTimer = new System.Timers.Timer(500);   //0.5�ʸ��� DI �б�
            pollTimer.Elapsed += PollTimer_Elapsed;
            ///

        }



        /*   private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)    //����������
           {
               int bytesToRead = serialPort.BytesToRead;
               byte[] buffer = new byte[bytesToRead];
               serialPort.Read(buffer, 0, bytesToRead);

               receiveBuffer.AddRange(buffer);

               // ������ Modbus ���� �Ľ� (���ŵ� ������ ����, CRC üũ ��)
               while (receiveBuffer.Count >= 5)
               {
                   // ���⼭�� DI 2�� ���� ���� ���� ���:  
                   // Slave(1) + Function(1) + ByteCount(1) + Data(2*2) + CRC(2) = 9 ����Ʈ
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
                           // Data�� packet[3] ~ packet[3 + byteCount -1]
                           // ���⼭ DI #1 ���� ���� (ù 2����Ʈ�� DI #1, #2)
                           ushort di1 = (ushort)((packet[3] << 8) | packet[4]);

                           Invoke(new Action(() =>
                           {
                               AppendText("[���� DI] DI#1 ����: " + di1);

                               // DO #1 ����: DI #1�� 0�� �ƴϸ� DO #1 ON, 0�̸� OFF
                               ushort doValue = (ushort)(di1 != 0 ? 1 : 0);

                               // DO #1�� 0x04 �ּҿ� �����Ƿ� ����
                               byte slaveId = 1;
                               ushort doAddress = 0x04;

                               byte[] writeFrame = BuildWriteSingleRegisterFrame(slaveId, doAddress, doValue);
                               serialPort.Write(writeFrame, 0, writeFrame.Length);
                               AppendText("[���� DO] " + BitConverter.ToString(writeFrame));
                           }));

                           receiveBuffer.RemoveRange(0, expectedLength);
                       }
                       else
                       {
                           // �Լ� �ڵ� �ٸ� ��� ���� ����
                           receiveBuffer.Clear();
                       }
                   }
                   else
                   {
                       Invoke(new Action(() =>
                       {
                           AppendText("[����] CRC ����ġ, ���� �ʱ�ȭ");
                       }));
                       receiveBuffer.Clear();
                       break;
                   }
               }
           }*/



         private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)    //����������
         {
             int bytesToRead = serialPort.BytesToRead;
             byte[] buffer = new byte[bytesToRead];
             serialPort.Read(buffer, 0, bytesToRead);

             receiveBuffer.AddRange(buffer);

             // Modbus RTU�� �ּ� 5����Ʈ �̻���� ��Ŷ ���� (�ּ�1, ���1, ������N, CRC2)
             // ���⼭�� �����ϰ� 8����Ʈ �̻��� �� �˻� �� ��� ó�� (�ʿ信 ���� ����)
             while (receiveBuffer.Count >= 5)
             {

                 // �ּ� ���� ����
                 byte functionCode = receiveBuffer[1];
                 int expectedLength = 0;

                 if ((functionCode & 0x80) != 0)
                 {
                     // ���� ���� (Error): [Slave][Function|0x80][Exception Code][CRC Lo][CRC Hi]
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
                     AppendText($"[����] �� �� ���� ��� �ڵ�: 0x{functionCode:X2}");
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
                         AppendText($"[����] {BitConverter.ToString(packet)}");

                         if ((functionCode & 0x80) != 0)
                         {
                             byte errorCode = packet[2];
                             AppendText($"[����] Function: 0x{functionCode:X2}, ���� �ڵ�: {errorCode}");
                         }
                         else
                         {
                             AppendText("[����] CRC ��ġ");
                         }
                     }));

                     receiveBuffer.RemoveRange(0, expectedLength);
                 }
                 else
                 {
                     Invoke(new Action(() =>
                     {
                         AppendText($"[����] {BitConverter.ToString(packet)}");
                         AppendText("[����] CRC ����ġ - ���� ����");
                     }));

                     receiveBuffer.Clear();
                     break;
                 }
                 // ���� ���� ó�� (0x83 ��)�� ��ó�� �б��ؼ� �־���� ��
             }

         }

        private void btnOpenPort_Click(object sender, EventArgs e)
        {

            try
            {
                if (!serialPort.IsOpen)
                {
                    serialPort.PortName = txtPortName.Text; // ��: "COM2"
                    serialPort.Open();
                    AppendText("[��Ʈ ����] " + serialPort.PortName);
                    ///
                    pollTimer.Start();
                    ///

                }
                else
                {
                    AppendText("[��Ʈ �̹� ����]");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("��Ʈ ���� ����: " + ex.Message);
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
                Log("��Ʈ ����");
            }
        }

        private void PollTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!serialPort.IsOpen) return;

            // DI #1, #2�� PortTable 0x05�� �ּҺ��� (0x05)
            // ���⼭ startAddress=0x05, quantity=2�� DI �б� (Modbus ����ڵ� 0x03 ���)
            byte slaveId = 1;
            ushort startAddress = 0x05;
            ushort quantity = 2;

            byte[] frame = BuildReadHoldingRegistersFrame(slaveId, startAddress, quantity);

            try
            {
                serialPort.Write(frame, 0, frame.Length);
                AppendText("[����] " + BitConverter.ToString(frame));
            }
            catch (Exception ex)
            {
                AppendText("[���� ����] " + ex.Message);
            }
        }

        private void btnWriteDO_Click(object sender, EventArgs e)
        {
            if (!serialPort.IsOpen)
            {
                MessageBox.Show("��Ʈ�� ���� �����ּ���.");
                return;
            }

            byte slaveId = Convert.ToByte(1);
            ushort registerAddress = Convert.ToUInt16(0);
            ushort valueToWrite = Convert.ToUInt16(txtWriteValue.Text);

            byte[] frame = BuildWriteSingleRegisterFrame(slaveId, registerAddress, valueToWrite);
            serialPort.Write(frame, 0, frame.Length);
            AppendText("[����] " + BitConverter.ToString(frame));
        }

        private void btnReadDI_Click(object sender, EventArgs e)
        {
            if (!serialPort.IsOpen)
            {
                MessageBox.Show("��Ʈ�� ���� �����ּ���.");
                return;
            }

            byte slaveId = Convert.ToByte(1);
            ushort startAddress = Convert.ToUInt16(0);
            ushort quantity = Convert.ToUInt16(18);

            byte[] frame = BuildReadHoldingRegistersFrame(slaveId, startAddress, quantity);
            serialPort.Write(frame, 0, frame.Length);
            AppendText("[����] " + BitConverter.ToString(frame));
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
            ushort receivedCrc = (ushort)((data[length - 1] << 8) | data[length - 2]); // LSB ����, MSB ����

            ushort calculatedCrc = CalculateCRC(data, length - 2); // CRC ����� ������ 2����Ʈ ����

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
