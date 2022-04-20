using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
namespace Example
{
    class Program
    {
        // 서버 실행 Task 메소드
        static async Task RunServer(int port)
        {
            // Socket EndPoint 설정(서버의 경우는 Any로 설정하고 포트 번호만 설정한다.)
            var ipep = new IPEndPoint(IPAddress.Any, port);
            // 소켓 인스턴스 생성
            using (Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                // 서버 소켓에 EndPoint 설정
                server.Bind(ipep);
                // 클라이언트 소켓 대기 버퍼
                server.Listen(20);
                // 콘솔 출력
                Console.WriteLine($"Server Start... Listen port {ipep.Port}...");
                // server Accept를 Task로 병렬 처리(즉, 비동기를 만든다.)
                var task = new Task(() =>
                {
                    // 무한 루프
                    while (true)
                    {
                        // 클라이언트로부터 접속 대기
                        var client = server.Accept();
                        // 접속이 되면 Task로 병렬 처리
                        new Task(() =>
                        {
                            // 클라이언트 EndPoint 정보 취득
                            var ip = client.RemoteEndPoint as IPEndPoint;
                            // 콘솔 출력 - 접속 ip와 접속 시간
                            Console.WriteLine($"Client : (From: {ip.Address.ToString()}:{ip.Port}, Connection time: {DateTime.Now})");
                            // 클라이언트로 접속 메시지를 byte로 변환하여 송신
                            client.Send(Encoding.ASCII.GetBytes("Welcome YoungMin's Server!\r\n>"));
                            // 메시지 버퍼
                            var sb = new StringBuilder();
                            // 종료되면 자동 client 종료
                            using (client)
                            {
                                try 
                                { 
                                    // 무한 루프
                                    while (true)
                                    {
                                        // 통신 바이너리 버퍼
                                        var binary = new Byte[1024];
                                        // 클라이언트로부터 메시지 대기
                                        client.Receive(binary);
                                        // 클라이언트로 받은 메시지를 String으로 변환
                                        var data = Encoding.ASCII.GetString(binary);
                                        // 메시지 공백(\0)을 제거
                                        sb.Append(data.Trim('\0'));
                                        // 메시지 총 내용이 2글자 이상이고 개행(\r\n)이 발생하면
                                        if (sb.Length <= 2)
                                            continue;

                                        if (sb[0] != '{')
                                        {
                                            int size = sb[0];
                                            size |= sb[1];

                                            if(sb.Length == size)
                                            {
                                                int id = sb[2];
                                                id |= sb[3];

                                                Console.WriteLine("[Perfect] size = " + (size - 4) + ", id = " + id + ", data = " + sb.Remove(0,4));
                                            }
                                            else
                                            {
                                                Console.WriteLine("[Just Word] " + sb);
                                            }

                                            sb.Length = 0;

                                            var sendMsg = Encoding.ASCII.GetBytes("ECHO : " + data + "\r\n>");
                                            client.Send(sendMsg);

                                            continue;
                                        }
                                        else if (sb.Length > 2) //&& sb[sb.Length - 2] == '\r' && sb[sb.Length - 1] == '\n')
                                        {
                                            // 메시지 버퍼의 내용을 String으로 변환
                                            data = sb.ToString().Replace("\n", "").Replace("\r", "");
                                            // 메시지 내용이 공백이라면 계속 메시지 대기 상태로
                                            if (String.IsNullOrWhiteSpace(data))
                                            {
                                                continue;
                                            }
                                            // 메시지 내용이 exit라면 무한 루프 종료(즉, 서버 종료)
                                            if ("EXIT".Equals(data, StringComparison.OrdinalIgnoreCase))
                                            {
                                                break;
                                            }
                                            // 메시지 내용을 콘솔에 표시
                                            Console.WriteLine("[PPRK version] " + data);
                                            // 버퍼 초기화
                                            sb.Length = 0;
                                            // 메시지에 ECHO를 붙힘
                                            var sendMsg = Encoding.ASCII.GetBytes("ECHO : " + data + "\r\n>");
                                            // 클라이언트로 메시지 송신
                                            client.Send(sendMsg);
                                        }
                                    }
                                }
                                 catch (SocketException)
                                {
                                    // 접속 끝김이 발생하면 Exception이 발생
                                }
                            // 콘솔 출력 - 접속 종료 메시지
                            Console.WriteLine($"Disconnected : (From: {ip.Address.ToString()}:{ip.Port}, Connection time: {DateTime.Now})");
                            }
                            // Task 실행
                        }).Start();
                    }
                });
                // Task 실행
                task.Start();
                // 대기
                await task;
            }
        }
        // 실행 함수
        static void Main(string[] args)
        {
            // Task로 Socket 서버를 만듬(서버가 종료될 때까지 대기)
            RunServer(7777).Wait();
            // 아무 키나 누르면 종료
            Console.WriteLine("Press Any key...");
            Console.ReadLine();
        }
    }
}