using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace SocketTcpServer
{
    class Server
    {
        static string address = "127.0.0.1"; // адрес сервера (наш)
        static int port = 8005; // порт сервера (наш порт для приема входящих запросов)
        static void Main(string[] args)
        {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port); // конечная точка соед. (IP-адрес, номер порта)

            // 1. Создание сокета 
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // 2. Установка локальн. точки для будущего прослушивания подключений
                // Связываем (Bind) сокет с локальной точкой, по которой будем принимать данные
                listenSocket.Bind(ipPoint);

                // 3. Запуск прослушивания входящих (внешних) подключений (метод Listen). Вызывается только после метода Bind
                listenSocket.Listen(10); // слушает подключение по порту 8005 на локальном адресе 127.0.0.1.т.е.клиент должен будет подключаться к локальному адресу и порту 8005
                                         // Параметр:  количество входящих подключений, которые могут быть поставлены в очередь сокета.
                Console.WriteLine("Сервер запущен. Ожидание подключений...");


                // 4. Прием входящего подключения (метод Accept)
                Socket handler = listenSocket.Accept();
                // Метод Accept извлекает из очереди первый запрос и создает для его обработки объект типа Socket. 
                while (true)
                {
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0; // количество полученных байтов
                    byte[] data = new byte[256]; // буфер для получаемых данных

                    do
                    {
                        bytes = handler.Receive(data); // получаем данные с помощью метода Receive
                        // Метод Receive в качестве параметра принимает массив байтов, в который считываются полученные данные, и возвращает количество полученных байтов.
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (handler.Available > 0); // количество полученных из сети и доступных для чтения данных

                    if (builder.ToString().Equals("stop"))
                    {
                        string message = "передача остановлена";
                        data = Encoding.Unicode.GetBytes(message);
                        handler.Send(data);

                        handler.Shutdown(SocketShutdown.Both); // отключение передачи и приема на сокет
                        handler.Close(); // закрываем сокет
                        handler = listenSocket.Accept();
                    }
                    else
                    {
                        Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + builder.ToString()); // вывод времени

                        // отправка ответа клиенту
                        string message = "сообщение доставлено";
                        data = Encoding.Unicode.GetBytes(message);
                        handler.Send(data);
                        // После получения данных клиенту посылается ответное сообщение с помощью метода Send(), 
                        // который в качестве параметра принимает массив байтов
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}