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
    0x85, 0xC8  // CRC (계산 필요하거나 라이브러리 이용)
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
            /* try   //일단 basso가 데이터 받기는 됨   //QModBus에서 function 3,6 되는거확인
             {
                 serialPort.WriteLine(txtSend.Text);
                 // 예외 없으면 일단 전송 시도는 성공
             }
             catch (Exception ex)
             {
                 MessageBox.Show("전송 실패: " + ex.Message);
             }*/

            /*   string data = serialPort.ReadExisting();
             MessageBox.Show(data.ToString());

               this.Invoke(new Action(() =>
               {
                   txtReceive.AppendText(data);

                   if (data.Contains("00"))  // 또는 다른 장비가 보내는 성공 응답 문자열
                   {
                       MessageBox.Show("전송 성공 확인됨!");
                   }
               }));*/

            /*   System.Threading.Thread.Sleep(50); // 필요 시 늘리세요   //read 기준 성공
               int len = serialPort.BytesToRead;
               byte[] buffer = new byte[len];
               serialPort.Read(buffer, 0, len);

               string hex = BitConverter.ToString(buffer).Replace("-", " ");

               Invoke(new Action(() => {
                   txtReceive.AppendText($"[수신] {hex}{Environment.NewLine}");
               }));*/


            /*   int bytesToRead = serialPort.BytesToRead; //write 일단 성공인데 수신 GUI에서 잘림 
               byte[] buffer = new byte[bytesToRead];

               serialPort.Read(buffer, 0, bytesToRead);

               string receivedHex = BitConverter.ToString(buffer);
               txtReceive.AppendText($"[수신] {receivedHex}{Environment.NewLine}");*/

            /*    int bytesToRead = serialPort.BytesToRead; //수신 GUI잘리지 않지만 문제는 address 0말고 1넣을때.
                byte[] buffer = new byte[bytesToRead];
                serialPort.Read(buffer, 0, bytesToRead);

                while (serialPort.BytesToRead > 0)
                {
                    byte data = (byte)serialPort.ReadByte();
                    receiveBuffer.Add(data);
                }

                // 누적 저장
                receiveBuffer.AddRange(buffer);

                if (receiveBuffer.Count >= 8)
                {
                    // 여기서 CRC 검증 등도 할 수 있음
                    byte[] fullPacket = receiveBuffer.Take(8).ToArray();
                    // CRC 체크
                    ushort crc = CalculateCRC(fullPacket, 6);
                    if (fullPacket[6] == (byte)(crc & 0xFF) && fullPacket[7] == (byte)(crc >> 8))
                    {
                        string hex = BitConverter.ToString(fullPacket);
                        txtReceive.AppendText($"[수신] {hex}{Environment.NewLine}");
                        receiveBuffer.RemoveRange(0, 8);
                    }
                    else
                    {
                        // CRC 오류 처리 (버퍼 비우거나 다시 시도)
                        receiveBuffer.Clear();
                        Invoke(new Action(() => txtReceive.AppendText("[오류] CRC 불일치\r\n")));
                    }
                }*/
            while (serialPort.BytesToRead > 0)
            {
                byte data = (byte)serialPort.ReadByte();
                receiveBuffer.Add(data);
            }

            Invoke(new Action(() => {
                string fullHex = BitConverter.ToString(receiveBuffer.ToArray());
                txtReceive.AppendText($"[버퍼 전체] {fullHex}{Environment.NewLine}");
            }));

            ProcessReceiveBuffer();

        }

        private void ProcessReceiveBuffer()
        {
            while (true)
            {
                if (receiveBuffer.Count < 5) break;  // 최소 Exception 패킷 크기

                byte slaveId = receiveBuffer[0];
                byte functionCode = receiveBuffer[1];

                int packetLength = 0;

                if ((functionCode & 0x80) != 0) // Exception 응답
                    packetLength = 5;
                else if (functionCode == 0x06)  // 정상 Write Single Register 응답
                    packetLength = 8;
                else
                {
                    // 알 수 없는 패킷이라 가정, 버퍼 첫 바이트 제거 후 다시 시도
                    receiveBuffer.RemoveAt(0);
                    continue;
                }

                if (receiveBuffer.Count < packetLength) break;  // 패킷 다 안 받음

                byte[] fullPacket = receiveBuffer.GetRange(0, packetLength).ToArray();
                ushort receivedCRC = (ushort)((fullPacket[packetLength - 1] << 8) | fullPacket[packetLength - 2]);
                ushort calculatedCRC = CalculateCRC(fullPacket, packetLength - 2);

                string hex = BitConverter.ToString(fullPacket);
                Invoke(new Action(() =>
                {
                    txtReceive.AppendText($"[수신] {hex}{Environment.NewLine}");
                    if (receivedCRC != calculatedCRC)
                        txtReceive.AppendText("[오류] CRC 불일치" + Environment.NewLine);
                    else
                        txtReceive.AppendText("[정상] CRC 일치" + Environment.NewLine);
                }));

                receiveBuffer.RemoveRange(0, packetLength);  // 처리한 패킷 제거
            }
        }

        private bool IsLikelyModbusPacket(List<byte> buffer, int index)
        {
            if (buffer.Count < index + 8)
                return false;

            byte slaveId = buffer[index];
            byte functionCode = buffer[index + 1];

            // Modbus 기본 조건: 주소 1~247, 기능코드 0x06
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


        /* private void SendRequest(byte[] request)  //read기준 성공
         {
             if (serialPort.IsOpen)
             {
                 serialPort.Write(request, 0, request.Length);
                 txtSend.AppendText("[전송] " + BitConverter.ToString(request).Replace("-", " ") + Environment.NewLine);
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
                 MessageBox.Show("보낸메세지 : " + txtSend.Text);
             }*/
            /*  byte[] request = new byte[] { 0x01, 0x03, 0x00, 0x00, 0x00, 0x01 }; //  read 기준 성공
              ushort crc = CalculateCRC(request); // CRC 계산

              byte[] fullRequest = new byte[request.Length + 2];
              Array.Copy(request, fullRequest, request.Length);
              fullRequest[request.Length] = (byte)(crc & 0xFF);           // CRC Low
              fullRequest[request.Length + 1] = (byte)((crc >> 8) & 0xFF); // CRC High

              SendRequest(fullRequest); // 여기서 실제 전송됨*/

            if (serialPort.IsOpen)
            {
                byte slaveId = 0x01;
                   ushort registerAddress = 0x0000;
                //   ushort valueToWrite = 0x0001;
            //    ushort registerAddress = Convert.ToUInt16(txtAddress.Text);   주소 0만됨
                ushort valueToWrite = Convert.ToUInt16(txtValue.Text);

                byte[] request = BuildWriteSingleRegisterFrame(slaveId, registerAddress, valueToWrite);
                serialPort.Write(request, 0, request.Length);
                //   txtSend.AppendText("[전송] " + BitConverter.ToString(request).Replace("-", " ") + Environment.NewLine);
                txtSend.AppendText("[전송] " + BitConverter.ToString(request).Replace("-", " ") + Environment.NewLine);

            }
            else
            {
                MessageBox.Show("연결 우선 필요 ");
            }
            


        }

        /* private ushort CalculateCRC(byte[] data)  //read 기준 성공
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
