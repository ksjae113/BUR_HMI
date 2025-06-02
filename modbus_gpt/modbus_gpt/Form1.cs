using System;
using System.IO.Ports;
using System.Windows.Forms;
using Modbus.Device;
using Modbus.IO;

namespace modbus_gpt        //Turn on 하면 0받아서 켜지고 off하면 1받아서 꺼지기 성공
{


    public partial class Form1 : Form       
    {
        private SerialPort _serialPort;
        private IModbusSerialMaster _modbusMaster;

        const char _00 = (char)0x00;
        const char _01 = (char)0x01;
        const char _02 = (char)0x02;
        const char _03 = (char)0x03;
        const char _04 = (char)0x04;
        const char _05 = (char)0x05;
        const char _06 = (char)0x06;
        const char _07 = (char)0x07;
        const char _08 = (char)0x08;
        const char _09 = (char)0x09;


        public Form1()
        {

            InitializeComponent();

        }

        private void btnToggleDO_Click_Click(object sender, EventArgs e)
        {
            byte slaveId = 1;
            ushort address = 0x0000; // DO 포트 주소
            ushort value = chkOutput.Checked ? (ushort)0x0000 : (ushort)0x0001;

            _modbusMaster.WriteSingleRegister(slaveId, address, value);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // ① 시리얼 포트 설정
            _serialPort = new SerialPort("COM103", 115200, Parity.None, 8, StopBits.One);
            _serialPort.Open();

            // ② Modbus RTU 마스터 생성
            _modbusMaster = ModbusSerialMaster.CreateRtu(_serialPort);
            _modbusMaster.Transport.ReadTimeout = 1000;
        }

        private void btnReadDI_Click(object sender, EventArgs e)
        {
            byte slaveId = 1;
            ushort startAddress = 0x0000;
            ushort numInputs = 4;

            ushort[] inputData = _modbusMaster.ReadInputRegisters(slaveId, startAddress, numInputs);

            // 예시 출력
            lblDI1.Text = inputData[0].ToString(); // DI1 상태
            lblDI2.Text = inputData[1].ToString(); // DI2 상태
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _serialPort?.Close();
        }

        private void btnTurnOn_Click(object sender, EventArgs e)
        {
                  WritePort3Led(2); // 0 → LED 켜짐
           /* byte slaveId = Convert.ToByte(1);
            ushort startAddress = Convert.ToUInt16(0);
            ushort quantity = Convert.ToUInt16(17);
            byte[] frame = BuildReadHoldingRegistersFrame(slaveId, startAddress, quantity);
            _serialPort.Write(frame, 0, frame.Length);*/
        }

       /* private byte[] BuildReadHoldingRegistersFrame(byte slaveId, ushort startAddress, ushort quantity)
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
        }*/

      /*  private byte[] BuildWriteSingleRegisterFrame(byte slaveId, ushort registerAddress, ushort value)
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
        }*/

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

        private void btnTurnOff_Click(object sender, EventArgs e)   //꺼짐기준 - 1
        {
            WritePort3Led(1); // 1 → LED 꺼짐
        }

        private void WritePort3Led(ushort value)
        {
            try
            {
                byte slaveId = 1;
                ushort address = 0x0000; // Port3 LED 제어 주소

                _modbusMaster.WriteSingleRegister(slaveId, address, value);

                lblStatus.Text = value == 0 ? "Port3 LED ON (켜짐)" : "Port3 LED OFF (꺼짐)";

               
            }
            catch (Exception ex)
            {
                MessageBox.Show("통신 에러: " + ex.Message);
            }
        }
        private void WritePort4Led(ushort value)
        {
            try
            {
                byte slaveId = 1;
                ushort address = 0x0010; // Port4 LED 제어 주소

                _modbusMaster.WriteSingleRegister(slaveId, address, value);

                lblStatus.Text = value == 1 ? "Port4 LED ON (켜짐)" : "Port4 LED OFF (꺼짐)";


            }
            catch (Exception ex)
            {
                MessageBox.Show("통신 에러: " + ex.Message);
            }
        }


    }
}
