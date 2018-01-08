using LN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketServidorArchivos
{
    class Program
    {
        static void Main(string[] args)
        {
            //sample
            Console.Write("Datos");
            Console.Write(LNUsuarios.Instance.Listar().Count);
            Console.Read();
        }
    }
}
