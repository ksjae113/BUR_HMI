using System;
using System.IO.Ports;

class Program
{
    static SerialPort port;

     static void Main()    ///raw read 확인됨
     {
         port = new SerialPort("COM103", 115200, Parity.None, 8, StopBits.One);
         port.DataReceived += Port_DataReceived;
         port.Open();

         Console.WriteLine("수신 대기 중... 아무 키나 누르면 종료합니다.");
         Console.ReadKey();
         port.Close();
     }

     static void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
     {
        byte[] command = new byte[] { 0x04, 0x03, 0x01 };   //Raw Write 성공  // 0x04(DO) 0x03(PORT NUM) 0x01(LED ON)
        port.Write(command, 0, command.Length);
        Console.Write("PORT3 ON 송신");

        int bytesToRead = port.BytesToRead;
         byte[] buffer = new byte[bytesToRead];
         port.Read(buffer, 0, bytesToRead);

      

        Console.Write("수신 데이터: ");
         foreach (byte b in buffer)
         {
             Console.Write($"{b:X2} ");
         }
         Console.WriteLine();

         // 예시: 12바이트씩 파싱
         for (int i = 0; i + 11 < buffer.Length; i++)
         {
             if (buffer[i] == 0x05 && buffer[i + 1] == 0x05)
             {
                 byte[] frame = new byte[12];
                 Array.Copy(buffer, i, frame, 0, 12);
                 ParseFrame(frame);
                 i += 11;
             }
         }
     }

     static void ParseFrame(byte[] frame)
     {
         // 예시: DI 상태 추출 (추정 위치: 바이트 2~9)
         Console.WriteLine("[프레임 분석]");
         for (int i = 2; i < 10; i += 2)
         {
             ushort di = (ushort)((frame[i] << 8) | frame[i + 1]);
             Console.WriteLine($" - 데이터{i / 2 - 1}: 0x{di:X4}");
         }
         Console.WriteLine("------------------");
     }

   

   /* static void Main()    //송신
    {
        port = new SerialPort("COM103", 115200, Parity.None, 8, StopBits.One);
        port.DataReceived += Port_DataReceived;

        try
        {
            port.Open();
            Console.WriteLine("포트를 열었습니다.");

            // 명령 전송: DO3 ON → 04 03 01
            byte[] command = new byte[] { 0x04, 0x03, 0x01 };   //Raw Write 성공
            port.Write(command, 0, command.Length);
            Console.WriteLine("명령 송신: 04 03 01 (DO3 ON)");

            Console.WriteLine("아무 키나 누르면 종료합니다...");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"에러: {ex.Message}");
        }
        finally
        {
            if (port.IsOpen)
                port.Close();
        }
    }

    static void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        int bytesToRead = port.BytesToRead;
        byte[] buffer = new byte[bytesToRead];
        port.Read(buffer, 0, bytesToRead);

        Console.Write("수신 데이터: ");
        foreach (byte b in buffer)
        {
            Console.Write($"{b:X2} ");
        }
        Console.WriteLine();
    }


 

    static void ParseFrame(byte[] frame)
    {
        Console.WriteLine("프레임분석");

        // 데이터0 ~ 데이터3 총 4개 (2바이트씩 8바이트 = 바이트 2 ~ 9)
        for (int i = 0; i < 4; i++)
        {
            int index = 2 + i * 2;
            ushort value = (ushort)((frame[index] << 8) | frame[index + 1]);
            Console.WriteLine($"데이터{i}:0x{value:X4}");
        }

        Console.WriteLine();
    }*/

}
