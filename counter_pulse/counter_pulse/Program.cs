// See https://aka.ms/new-console-template for more information
using System;
using System.Data;
using System.Diagnostics.Metrics;
using System.IO.Ports;
using System.Threading;
using Modbus.Device; // NModbus 라이브러리
using ScottPlot;
using ScottPlot.WinForms;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;


class Program
{
    [STAThread] // 중요: WinForms 창 사용 시 필요
    static void Main(string[] args)
    {
        // 시리얼 포트 설정
        SerialPort port = new SerialPort("COM103", 115200, Parity.None, 8, StopBits.One);
        port.Open();

        // Modbus 마스터 초기화
        var modbusMaster = ModbusSerialMaster.CreateRtu(port);
        modbusMaster.Transport.ReadTimeout = 1000;

        byte slaveId = 1;
        ushort port3CoilAddress = 0x000A;   // DO - PORT3
        ushort counterRegAddress = 0x0011;  // Counter 값 레지스터 (PORT2로 설정된 경우)

        Console.WriteLine("Modbus 시작됨. 아무 키나 누르면 펄스 발생 후 Counter 값 읽기...");
        Console.ReadKey();



        ushort port3RegisterAddress = 0x000A;
     
        for (int i =0;i < 10;i++)
        {
            // DO ON (PORT3)
            modbusMaster.WriteSingleRegister(slaveId, port3RegisterAddress, 1); // ON
            Thread.Sleep(100); // 펄스 유지 시간

            // DO OFF (PORT3)
            modbusMaster.WriteSingleRegister(slaveId, port3RegisterAddress, 0); // OFF
        }
       
        Console.WriteLine("펄스 출력 완료.");


        for (ushort address = 0x0000; address <= 0x007F; address++)
        {
            try
            {
                ushort[] res = modbusMaster.ReadHoldingRegisters(slaveId, address, 1);
                Console.WriteLine($"주소 0x{address:X4} → 값: {res[0]}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"주소 0x{address:X4} → 읽기 실패: {ex.Message}");
            }
        }

        var plt = new Plot();
        double[] xs = DataGen.Consecutive(100);
        double[] ys = DataGen.Sin(100);
        plt.AddScatter(xs, ys);

        // 팝업 윈도우로 보여줌
        Application.EnableVisualStyles();
        FormsPlotViewer viewer = new FormsPlotViewer(plt);
        Application.Run(viewer);

        // 카운터 값 읽기
        ushort[] result = modbusMaster.ReadHoldingRegisters(slaveId, counterRegAddress, 1);
        int counterValue = result[0];
        Console.WriteLine($"현재 Counter 값: {counterValue}");

        // 종료 처리
        port.Close();
        Console.WriteLine("시리얼 포트 닫힘. 종료하려면 아무 키나 누르세요.");
        Console.ReadKey();
    }
}

