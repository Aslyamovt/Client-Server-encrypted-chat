using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace Сети_Лаба1_Сервер
{
    static class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                byte[] bytesRecv = new byte[4096];
                Console.WriteLine("Сервер\nВведите IP адрес: ");
                TcpListener listener = new TcpListener(IPAddress.Parse(Console.ReadLine()), 5001);
                Console.WriteLine("***\n");
                listener.Start();
                Socket tc = listener.AcceptSocket();
                //Получение
                string str="";
                do
                {
                    tc.Receive(bytesRecv);
                    string msg = Encoding.Unicode.GetString(bytesRecv);
                    Console.WriteLine(msg.Remove(msg.IndexOf('\0')).Decrypt());
                    //Отправка
                    str = Console.ReadLine();
                    tc.Send(Encoding.Unicode.GetBytes(Crypt(str)));
                } while (str != "Break");
                tc.Close();
            }
            catch (System.ArgumentOutOfRangeException ex)
            {
                Console.WriteLine("\n***\nПроизошла ошибка!\n***");
                System.Console.WriteLine(ex.Message);
            }
        }
        private static byte[] key = new byte[8] { 1, 2, 3, 4, 5, 6, 7, 8 };
        private static byte[] iv = new byte[8] { 1, 2, 3, 4, 5, 6, 7, 8 };
        public static string Crypt(this string text)
        {
            SymmetricAlgorithm algorithm = DES.Create();
            ICryptoTransform transform = algorithm.CreateEncryptor(key, iv);
            byte[] inputbuffer = Encoding.Unicode.GetBytes(text);
            byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return Encoding.Unicode.GetString(outputBuffer);
        }
        public static string Decrypt(this string text)
        {
            SymmetricAlgorithm algorithm = DES.Create();
            ICryptoTransform transform = algorithm.CreateDecryptor(key, iv);
            byte[] inputbuffer = Encoding.Unicode.GetBytes(text);
            byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return Encoding.Unicode.GetString(outputBuffer);
        }
    }
}



