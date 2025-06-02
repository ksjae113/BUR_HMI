using System;
using System.IO.Ports;
using System.Net;
using Modbus.Device;

namespace BassoRegisterTest
{
    class Program
    {

        static void Main()
        {
            string portName = "COM103"; // 실제 포트 이름으로 바꿔주세요
            int baudRate = 115200;
            byte slaveId = 1;

            using (SerialPort port = new SerialPort(portName))
            {
                port.BaudRate = baudRate;
                port.DataBits = 8;
                port.Parity = Parity.None;
                port.StopBits = StopBits.One;
                port.Open();

                IModbusSerialMaster master = ModbusSerialMaster.CreateRtu(port);

                for ( int i = 0; i < 10; i ++)
                {
                    master.WriteSingleRegister(slaveId, 0x000A, 1);
                    master.WriteSingleRegister(slaveId, 0x000A, 0);
                }

                for (ushort address = 0x0000; address <= 0x007F; address++)
                {
                    try
                    {
                     //   ushort[] result = master.ReadHoldingRegisters(slaveId, address, 1);
                        ushort[] result = master.ReadInputRegisters(slaveId, address, 1);

                        Console.WriteLine($"주소 0x{address:X4} → 값: {result[0]}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"주소 0x{address:X4} → 읽기 실패: {ex.Message}");
                    }
                }
            }

            Console.WriteLine("완료. 아무 키나 누르세요...");
            Console.ReadKey();
        }

        /* static void Main(string[] args)   Write 확인
         {
             // 시리얼 포트 설정
             var serialPort = new SerialPort("COM103", 115200, Parity.None, 8, StopBits.One);

             try
             {
                 serialPort.Open();

                 // Modbus 마스터 생성
                 var modbusMaster = ModbusSerialMaster.CreateRtu(serialPort);
                 modbusMaster.Transport.ReadTimeout = 1000;

                 byte slaveId = 1;

                 // 테스트할 레지스터 주소 범위
                 for (ushort address = 0x0000; address <= 0x0010; address++)
                 {
                     Console.WriteLine($"\n--- 주소 0x{address:X4} 테스트 시작 ---");

                     for (ushort value = 0; value < 100; value++)
                     {
                         try
                         {
                             modbusMaster.WriteSingleRegister(slaveId, address, value);
                             Console.WriteLine($"주소 0x{address:X4} - 값 {value} → 성공");
                         }
                         catch (Exception ex)
                         {
                             Console.WriteLine($"주소 0x{address:X4} - 값 {value} → 실패 ({ex.Message})");
                             break; // 더 큰 값도 실패할 가능성 높으므로 중단
                         }
                     }
                 }

                 serialPort.Close();
             }
             catch (Exception ex)
             {
                 Console.WriteLine($"예외 발생: {ex.Message}");
             }
         }*/
    }
}
