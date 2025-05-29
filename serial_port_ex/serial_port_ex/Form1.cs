using System;
using System.IO.Ports;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace serial_port_ex
{
    public partial class Form1 : Form
    {
        private SerialPort serialPort;

        byte[] request = new byte[] {
    0x01,       // Slave Address
    0x03,       // Function Code
    0x00, 0x10, // Starting Address
    0x00, 0x01, // Quantity of Registers
    0x85, 0xC8  // CRC (��� �ʿ��ϰų� ���̺귯�� �̿�)
};

        public Form1()
        {
            InitializeComponent();
               serialPort = new SerialPort("COM2", 115200, Parity.None, 8, StopBits.One); 
            serialPort.DataReceived += SerialPort_DataReceived;
            try
            {
             //   serialPort.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Serial port open error: " + ex.Message);
            }
        }

        List<byte> receiveBuffer = new List<byte>();

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            /* try   //�ϴ� basso�� ������ �ޱ�� ��   //QModBus���� function 3,6 �Ǵ°�Ȯ��
             {
                 serialPort.WriteLine(txtSend.Text);
                 // ���� ������ �ϴ� ���� �õ��� ����
             }
             catch (Exception ex)
             {
                 MessageBox.Show("���� ����: " + ex.Message);
             }*/

            /*   string data = serialPort.ReadExisting();
             MessageBox.Show(data.ToString());

               this.Invoke(new Action(() =>
               {
                   txtReceive.AppendText(data);

                   if (data.Contains("00"))  // �Ǵ� �ٸ� ��� ������ ���� ���� ���ڿ�
                   {
                       MessageBox.Show("���� ���� Ȯ�ε�!");
                   }
               }));*/

            /*   System.Threading.Thread.Sleep(50); // �ʿ� �� �ø�����   //read ���� ����
               int len = serialPort.BytesToRead;
               byte[] buffer = new byte[len];
               serialPort.Read(buffer, 0, len);

               string hex = BitConverter.ToString(buffer).Replace("-", " ");

               Invoke(new Action(() => {
                   txtReceive.AppendText($"[����] {hex}{Environment.NewLine}");
               }));*/


            /*   int bytesToRead = serialPort.BytesToRead; //write �ϴ� �����ε� ���� GUI���� �߸� 
               byte[] buffer = new byte[bytesToRead];

               serialPort.Read(buffer, 0, bytesToRead);

               string receivedHex = BitConverter.ToString(buffer);
               txtReceive.AppendText($"[����] {receivedHex}{Environment.NewLine}");*/

            /*    int bytesToRead = serialPort.BytesToRead; //���� GUI�߸��� ������ ������ address 0���� 1������.
                byte[] buffer = new byte[bytesToRead];
                serialPort.Read(buffer, 0, bytesToRead);

                while (serialPort.BytesToRead > 0)
                {
                    byte data = (byte)serialPort.ReadByte();
                    receiveBuffer.Add(data);
                }

                // ���� ����
                receiveBuffer.AddRange(buffer);

                if (receiveBuffer.Count >= 8)
                {
                    // ���⼭ CRC ���� � �� �� ����
                    byte[] fullPacket = receiveBuffer.Take(8).ToArray();
                    // CRC üũ
                    ushort crc = CalculateCRC(fullPacket, 6);
                    if (fullPacket[6] == (byte)(crc & 0xFF) && fullPacket[7] == (byte)(crc >> 8))
                    {
                        string hex = BitConverter.ToString(fullPacket);
                        txtReceive.AppendText($"[����] {hex}{Environment.NewLine}");
                        receiveBuffer.RemoveRange(0, 8);
                    }
                    else
                    {
                        // CRC ���� ó�� (���� ���ų� �ٽ� �õ�)
                        receiveBuffer.Clear();
                        Invoke(new Action(() => txtReceive.AppendText("[����] CRC ����ġ\r\n")));
                    }
                }*/
            while (serialPort.BytesToRead > 0)
            {
                byte data = (byte)serialPort.ReadByte();
                receiveBuffer.Add(data);
            }

            Invoke(new Action(() => {
                string fullHex = BitConverter.ToString(receiveBuffer.ToArray());
                txtReceive.AppendText($"[���� ��ü] {fullHex}{Environment.NewLine}");
            }));

            ProcessReceiveBuffer();

        }

        private void ProcessReceiveBuffer()
        {
            while (true)
            {
                if (receiveBuffer.Count < 5) break;  // �ּ� Exception ��Ŷ ũ��

                byte slaveId = receiveBuffer[0];
                byte functionCode = receiveBuffer[1];

                int packetLength = 0;

                if ((functionCode & 0x80) != 0) // Exception ����
                    packetLength = 5;
                else if (functionCode == 0x06)  // ���� Write Single Register ����
                    packetLength = 8;
                else
                {
                    // �� �� ���� ��Ŷ�̶� ����, ���� ù ����Ʈ ���� �� �ٽ� �õ�
                    receiveBuffer.RemoveAt(0);
                    continue;
                }

                if (receiveBuffer.Count < packetLength) break;  // ��Ŷ �� �� ����

                byte[] fullPacket = receiveBuffer.GetRange(0, packetLength).ToArray();
                ushort receivedCRC = (ushort)((fullPacket[packetLength - 1] << 8) | fullPacket[packetLength - 2]);
                ushort calculatedCRC = CalculateCRC(fullPacket, packetLength - 2);

                string hex = BitConverter.ToString(fullPacket);
                Invoke(new Action(() =>
                {
                    txtReceive.AppendText($"[����] {hex}{Environment.NewLine}");
                    if (receivedCRC != calculatedCRC)
                        txtReceive.AppendText("[����] CRC ����ġ" + Environment.NewLine);
                    else
                        txtReceive.AppendText("[����] CRC ��ġ" + Environment.NewLine);
                }));

                receiveBuffer.RemoveRange(0, packetLength);  // ó���� ��Ŷ ����
            }
        }

        private bool IsLikelyModbusPacket(List<byte> buffer, int index)
        {
            if (buffer.Count < index + 8)
                return false;

            byte slaveId = buffer[index];
            byte functionCode = buffer[index + 1];

            // Modbus �⺻ ����: �ּ� 1~247, ����ڵ� 0x06
            return (slaveId >= 1 && slaveId <= 247) && functionCode == 0x06;
        }

        private byte[] BuildWriteSingleRegisterFrame(byte slaveId, ushort address, ushort value)
        {
            byte[] frame = new byte[8];
            frame[0] = slaveId;
            frame[1] = 0x06;
            frame[2] = (byte)(address >> 8);
            frame[3] = (byte)(address & 0xFF);
            frame[4] = (byte)(value >> 8);
            frame[5] = (byte)(value & 0xFF);

            ushort crc = CalculateCRC(frame, 6);
            frame[6] = (byte)(crc & 0xFF); // CRC Low
            frame[7] = (byte)(crc >> 8);   // CRC High

            return frame;
        }


        /* private void SendRequest(byte[] request)  //read���� ����
         {
             if (serialPort.IsOpen)
             {
                 serialPort.Write(request, 0, request.Length);
                 txtSend.AppendText("[����] " + BitConverter.ToString(request).Replace("-", " ") + Environment.NewLine);
             }
         }*/

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort.IsOpen)
                serialPort.Close();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (!serialPort.IsOpen)
                {
                    serialPort.Open();
                    MessageBox.Show("Connected to COM2");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection error: " + ex.Message);
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            /* if (serialPort.IsOpen)
             {
                 serialPort.WriteLine(txtSend.Text);
                 MessageBox.Show("�����޼��� : " + txtSend.Text);
             }*/
            /*  byte[] request = new byte[] { 0x01, 0x03, 0x00, 0x00, 0x00, 0x01 }; //  read ���� ����
              ushort crc = CalculateCRC(request); // CRC ���

              byte[] fullRequest = new byte[request.Length + 2];
              Array.Copy(request, fullRequest, request.Length);
              fullRequest[request.Length] = (byte)(crc & 0xFF);           // CRC Low
              fullRequest[request.Length + 1] = (byte)((crc >> 8) & 0xFF); // CRC High

              SendRequest(fullRequest); // ���⼭ ���� ���۵�*/

            if (serialPort.IsOpen)
            {
                byte slaveId = 0x01;
                   ushort registerAddress = 0x0000;
                //   ushort valueToWrite = 0x0001;
            //    ushort registerAddress = Convert.ToUInt16(txtAddress.Text);   �ּ� 0����
                ushort valueToWrite = Convert.ToUInt16(txtValue.Text);

                byte[] request = BuildWriteSingleRegisterFrame(slaveId, registerAddress, valueToWrite);
                serialPort.Write(request, 0, request.Length);
                //   txtSend.AppendText("[����] " + BitConverter.ToString(request).Replace("-", " ") + Environment.NewLine);
                txtSend.AppendText("[����] " + BitConverter.ToString(request).Replace("-", " ") + Environment.NewLine);

            }
            else
            {
                MessageBox.Show("���� �켱 �ʿ� ");
            }
            


        }

        /* private ushort CalculateCRC(byte[] data)  //read ���� ����
         {
             ushort crc = 0xFFFF;

             foreach (byte b in data)
             {
                 crc ^= b;
                 for (int i = 0; i < 8; i++)
                 {
                     if ((crc & 0x0001) != 0)
                     {
                         crc >>= 1;
                         crc ^= 0xA001;
                     }
                     else
                     {
                         crc >>= 1;
                     }
                 }
             }

             return crc;
         }*/
        private ushort CalculateCRC(byte[] data, int length)
        {
            ushort crc = 0xFFFF;
            for (int pos = 0; pos < length; pos++)
            {
                crc ^= data[pos];
                for (int i = 0; i < 8; i++)
                {
                    if ((crc & 1) != 0)
                        crc = (ushort)((crc >> 1) ^ 0xA001);
                    else
                        crc >>= 1;
                }
            }
            return crc;
        }
    }
}
