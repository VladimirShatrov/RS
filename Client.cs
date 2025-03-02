// 2. КЛИЕНТ, который отправляет одно сообщение и затем отключает передачу и прием на сокет
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace SocketTcpClient
{
    class Client
    {
        // адрес и порт сервера, к которому будем подключаться
        static string address = "127.0.0.1"; // адрес сервера
        static int port = 8005; // порт сервера

        static void Main(string[] args)
        {
            try
            {
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port); // конечная точка соед. (IP-адрес, номер порта)

                // 1. Создание сокета
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // 2. Подключение к серверу (удаленному хосту). Метод Connect
                socket.Connect(ipPoint);


                string message = "";
                Console.WriteLine("stop - прекратить отправку сообщений");
                while (!message.Equals("stop"))
                {
                    Console.WriteLine("Введите сообщение:");
                    message = Console.ReadLine();
                    byte[] data = Encoding.Unicode.GetBytes(message); // перекодировка текста в последовательность байтов

                    // 3. Отправка данных (параметр: массив байтов)
                    socket.Send(data);

                    // 4. Получение ответа от сервера
                    data = new byte[256]; // буфер для ответа
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0; // количество полученных байт

                    do
                    {
                        bytes = socket.Receive(data, data.Length, 0); //  Получение данных
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (socket.Available > 0); // количество байтов полученных из сети и доступных для чтения
                    Console.WriteLine("ответ сервера: " + builder.ToString());
                }

                // 5. Закрытие сокета
                socket.Shutdown(SocketShutdown.Both); // отключение передачи и приема на сокет
                socket.Close(); // закрываем сокет
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();
        }
    }
}