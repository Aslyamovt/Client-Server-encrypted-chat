using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Security.Cryptography;

namespace Сети_Лаба1_КлиентV2
{
    class SocketClient
    {
        private Socket client;
        private IPEndPoint ip;
        private Thread th;

        private static byte[] key = new byte[8] { 1, 2, 3, 4, 5, 6, 7, 8 };
        private static byte[] iv = new byte[8] { 1, 2, 3, 4, 5, 6, 7, 8 };

        public SocketClient(string ip, Int32 port)
        {
            this.ip = new IPEndPoint(IPAddress.Parse(ip), port);
            this.client = new Socket(this.ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this.client.Connect(this.ip);
        }

        public void Start()
        {
            th = new Thread(new ThreadStart(Sending));
            th.Start();
        }

        private void Sending()
        {
            string messagesd = String.Empty;
            do
            {
                messagesd = Console.ReadLine();
                Send(messagesd);
                string messagersv = Receive();
                Console.WriteLine("Собеседник: " + messagersv);
            } while (messagesd != "Конец связи");
            Disconnect();
        }

        public void Send(string message)
        {
            byte[] tosend = Encoding.UTF8.GetBytes(message);
            byte[] EnBytes = Crypt(tosend);
            this.client.Send(EnBytes, 0, EnBytes.Length, SocketFlags.None);
        }

        public string Receive()
        {
            string message = String.Empty;
            byte[] GetBytes = new byte[4096];
            int b = client.Receive(GetBytes);
            byte[] DeBytes = Decrypt(GetBytes, b);
            message = Encoding.UTF8.GetString(DeBytes, 0, DeBytes.Length);
            return message;
        }


        public static byte[] Crypt(byte[] text)
        {
            SymmetricAlgorithm algorithm = DES.Create();
            ICryptoTransform transform = algorithm.CreateEncryptor(key, iv);
            byte[] outputBuffer = transform.TransformFinalBlock(text, 0, text.Length);
            return outputBuffer;
        }

        public static byte[] Decrypt(byte[] text, int b)
        {
            SymmetricAlgorithm algorithm = DES.Create();
            ICryptoTransform transform = algorithm.CreateDecryptor(key, iv);
            byte[] outputBuffer = transform.TransformFinalBlock(text, 0, b);
            return outputBuffer;
        }

        public void Disconnect()
        {
            this.client.Disconnect(false);
        }
    }

class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Клиент");
                Console.WriteLine("Введите ip-адрес:");
                string adr = Console.ReadLine();
                Console.WriteLine("***************************************************\n");
                SocketClient client = new SocketClient(adr, 5001);
                client.Start();
            }
            catch
            {
                Console.WriteLine("Связь с клиентом потеряна");
            }
        }
    }
}
