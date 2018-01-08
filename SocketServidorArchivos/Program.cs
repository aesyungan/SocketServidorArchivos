using Entidades;
using LN;
using socketServidor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketServidorArchivos
{
    class Program
    {
        static void Main(string[] args)
        {
            //sample app
            //Console.Write("Datos");
            //  Console.Write(LNUsuarios.Instance.Listar().Count);
            //socket
            Socket listenSocket = new Socket(AddressFamily.InterNetwork,
                                    SocketType.Stream,
                                    ProtocolType.Tcp);

            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5656);
            listenSocket.Bind(ep);

            // start listening
            listenSocket.Listen(100);

            FileEncryption fe = new FileEncryption();

            Console.WriteLine("Servidor escuchando.. ");
            String filedirectorykey = "claves/";
            String filedirectory = "recibidos/";

            while (true)
            {
                Console.WriteLine("\nWaiting for a connection...");

                Socket client = listenSocket.Accept();
                Console.WriteLine("cliente conectado");
                byte[] bytesFrom = new byte[1024];

                int bytesReceived = 1;
                var buffer = new byte[1024];

                do
                {
                    bytesReceived = client.Receive(buffer);
                } while (!(bytesReceived > 0));

                String filenamekey = Encoding.UTF8.GetString(buffer, 0, bytesReceived);

                Console.WriteLine("Recibido: " + filenamekey);
                client.Send(Encoding.ASCII.GetBytes("Recibiendo clave.."));

                //Recibo de archivo

                var output = File.Create(filedirectorykey + filenamekey);

                Console.WriteLine("Recibiendo clave");
                buffer = new byte[1024];
                int bytesRead;
                int ava;
                do
                {
                    //bytesRead = client.Receive(buffer);
                    //bytesRead = client.Receive(buffer, 0, client.Available, SocketFlags.None);
                    bytesRead = client.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                    output.Write(buffer, 0, bytesRead);
                    ava = client.Available;
                } while (bytesRead > 0 && ava > 0);
                output.Close();

                //Confirmación de recibo de clave
                client.Send(Encoding.ASCII.GetBytes("Descifrando clave.."));
                //--------------------------------------------------------------------------
                //accion a Realizar
                //que hacer 1 insserta 2 update 3 descardar
                buffer = new byte[1024];
                bytesReceived = 1;
                do
                {
                    bytesReceived = client.Receive(buffer);
                } while (!(bytesReceived > 0) && bytesReceived > buffer.Length);

                string tipoDeInstruccion = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
               
                int idAccion = Convert.ToInt32(tipoDeInstruccion);
                switch (idAccion)
                {
                    case 1:
                        Console.WriteLine("Action :" + tipoDeInstruccion + "->Insertando..");
                        //--------------------------------------------------------------------------
                        //Recibir nombre archivo cifrado
                        buffer = new byte[1024];
                        bytesReceived = 1;
                        do
                        {
                            bytesReceived = client.Receive(buffer);
                        } while (!(bytesReceived > 0) && bytesReceived > buffer.Length);

                        String filename = Encoding.UTF8.GetString(buffer, 0, bytesReceived);

                        Console.WriteLine("Recibido: " + filename);
                        client.Send(Encoding.ASCII.GetBytes("Recibiendo criptograma"));

                        buffer = new byte[8192];
                        bytesReceived = 1;
                        do
                        {
                            bytesReceived = client.Receive(buffer);
                        } while (!(bytesReceived > 0) && bytesReceived > buffer.Length);

                        long tamano = Convert.ToInt64(Encoding.UTF8.GetString(buffer, 0, bytesReceived));

                        Console.WriteLine("Recibido:" + filename);
                        client.Send(Encoding.ASCII.GetBytes("recibido tamano"));

                        //Recibir archivo cifrado
                        output = File.Create(filedirectory + "cifrados/" + filename);

                        Console.WriteLine("Recibiendo criptograma");
                        buffer = new byte[8192];

                        int contador = 0;
                        do
                        {
                            bytesRead = 0;
                            //System.Threading.Thread.Sleep(1);
                            //if (client.Available > 0) {
                            bytesRead = client.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                            output.Write(buffer, 0, bytesRead);
                            //}
                            contador += bytesRead;
                        } while (contador < tamano);
                        output.Close();

                        Console.WriteLine("Recibido criptograma: " + filename);
                        //client.Send(Encoding.ASCII.GetBytes("criptograma ok"));
                        // client.Close();

                        //descifrar
                        // inicia tiempo de desincrifrado
                        var stopwatch2 = new Stopwatch();
                        stopwatch2.Start();
                        DateTime tiempo1 = DateTime.Now;
                        // Console.WriteLine("INICIO tiempo: " + tiempo1.Ticks);
                        client.Send(Encoding.ASCII.GetBytes(tiempo1.Ticks.ToString()));
                        Console.WriteLine("Descifrando criptograma...");
                        fe.loadKey(filedirectorykey + filenamekey, "private.pem");
                        fe.DecryptFile(filedirectory + "cifrados/" + filename, filedirectory + "descifrados/" + filename);
                        //finaliza tiempo de descrifrado
                        stopwatch2.Stop();
                        //DateTime tiempo2 = DateTime.Now;
                        //Console.WriteLine("FIN tiempo: " + tiempo2.Ticks);
                        // long tDesCifrado = (tiempo2.Ticks - tiempo1.Ticks);
                        //TimeSpan tDesCifrado = new TimeSpan(tiempo2.Ticks - tiempo1.Ticks);
                        // TimeSpan tDesCifrado = (tiempo2.TimeOfDay);
                        // TimeSpan tDesCifrado1 = (tiempo1.TimeOfDay);
                        double tDesCifrado = stopwatch2.ElapsedMilliseconds;
                        Console.WriteLine("TIEMPO DE ENVIO DE CRIPTOGRAMA:  {0} ms", tDesCifrado);
                        //Console.Write("TIEMPO DE DESCIFRADO DEL ARCHIVO: {0} ms", tDesCifrado.TotalMilliseconds);
                        client.Send(Encoding.ASCII.GetBytes(tDesCifrado.ToString()));
                        //-------------------------------------------------------------------------------
                        //recibe id Usuario 
                        int bytesReceivedIdUser = 1;
                        var bufferIdUser = new byte[1024];

                        do
                        {
                            bytesReceivedIdUser = client.Receive(bufferIdUser);
                        } while (!(bytesReceivedIdUser > 0));

                        String id_usuario = Encoding.UTF8.GetString(bufferIdUser, 0, bytesReceivedIdUser);
                        Console.WriteLine("Id Usuario:" + id_usuario);

                        //fin recibe id Usuario
                        //inserta DB
                        InsertarArchivo(Convert.ToInt32(id_usuario), filename);
                        //envia q se Inserto Corectamente
                        client.Send(Encoding.ASCII.GetBytes("Insertado en DB"));
                        //-------------------------------------------------------------------------------

                        client.Close();


                        Console.WriteLine("\nProceso completado!!");
                        break;
                    case 2:
                        Console.WriteLine("Action :" + tipoDeInstruccion + "->Actualizando..");
                      
                        //--------------------------------------------------------------------------
                        //Recibir nombre archivo cifrado
                        buffer = new byte[1024];
                        bytesReceived = 1;
                        do
                        {
                            bytesReceived = client.Receive(buffer);
                        } while (!(bytesReceived > 0) && bytesReceived > buffer.Length);

                        String filenameUpdate = Encoding.UTF8.GetString(buffer, 0, bytesReceived);

                        Console.WriteLine("Recibido: " + filenameUpdate);
                        client.Send(Encoding.ASCII.GetBytes("Recibiendo criptograma"));

                        buffer = new byte[8192];
                        bytesReceived = 1;
                        do
                        {
                            bytesReceived = client.Receive(buffer);
                        } while (!(bytesReceived > 0) && bytesReceived > buffer.Length);

                        long tamanoUpdate = Convert.ToInt64(Encoding.UTF8.GetString(buffer, 0, bytesReceived));

                        Console.WriteLine("Recibido:" + filenameUpdate);
                        client.Send(Encoding.ASCII.GetBytes("recibido tamano"));

                        //Recibir archivo cifrado
                        output = File.Create(filedirectory + "cifrados/" + filenameUpdate);

                        Console.WriteLine("Recibiendo criptograma");
                        buffer = new byte[8192];

                        int contadorUpdate = 0;
                        do
                        {
                            bytesRead = 0;
                            //System.Threading.Thread.Sleep(1);
                            //if (client.Available > 0) {
                            bytesRead = client.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                            output.Write(buffer, 0, bytesRead);
                            //}
                            contadorUpdate += bytesRead;
                        } while (contadorUpdate < tamanoUpdate);
                        output.Close();

                        Console.WriteLine("Recibido criptograma: " + filenameUpdate);
                        //client.Send(Encoding.ASCII.GetBytes("criptograma ok"));
                        // client.Close();

                        //descifrar
                        // inicia tiempo de desincrifrado
                        var stopwatch2Update = new Stopwatch();
                        stopwatch2Update.Start();
                        DateTime tiempo1Update = DateTime.Now;
                        // Console.WriteLine("INICIO tiempo: " + tiempo1.Ticks);
                        client.Send(Encoding.ASCII.GetBytes(tiempo1Update.Ticks.ToString()));
                        Console.WriteLine("Descifrando criptograma...");
                        fe.loadKey(filedirectorykey + filenamekey, "private.pem");
                        fe.DecryptFile(filedirectory + "cifrados/" + filenameUpdate, filedirectory + "descifrados/" + filenameUpdate);
                        //finaliza tiempo de descrifrado
                        stopwatch2Update.Stop();
                        //DateTime tiempo2 = DateTime.Now;
                        //Console.WriteLine("FIN tiempo: " + tiempo2.Ticks);
                        // long tDesCifrado = (tiempo2.Ticks - tiempo1.Ticks);
                        //TimeSpan tDesCifrado = new TimeSpan(tiempo2.Ticks - tiempo1.Ticks);
                        // TimeSpan tDesCifrado = (tiempo2.TimeOfDay);
                        // TimeSpan tDesCifrado1 = (tiempo1.TimeOfDay);
                        double tDesCifradoUpdate = stopwatch2Update.ElapsedMilliseconds;
                        Console.WriteLine("TIEMPO DE ENVIO DE CRIPTOGRAMA:  {0} ms", tDesCifradoUpdate);
                        //Console.Write("TIEMPO DE DESCIFRADO DEL ARCHIVO: {0} ms", tDesCifrado.TotalMilliseconds);
                        client.Send(Encoding.ASCII.GetBytes(tDesCifradoUpdate.ToString()));
                        //-------------------------------------------------------------------------------
                        //recibe id Usuario 
                        int bytesReceivedIdUserUpdate = 1;
                        var bufferIdUserUpdate = new byte[1024];

                        do
                        {
                            bytesReceivedIdUser = client.Receive(bufferIdUserUpdate);
                        } while (!(bytesReceivedIdUser > 0));

                        string id_archivoUpdate = Encoding.UTF8.GetString(bufferIdUserUpdate, 0, bytesReceivedIdUser);
                        Console.WriteLine("Id Archivo:" + id_archivoUpdate);

                        //fin recibe id Usuario
                        //inserta DB
                        ActualizarArchivo(Convert.ToInt32(id_archivoUpdate), filenameUpdate);
                        //envia q se Inserto Corectamente
                        client.Send(Encoding.ASCII.GetBytes("Actualizado en DB"));
                        //-------------------------------------------------------------------------------

                        client.Close();
                        break;
                    case 3:
                        Console.WriteLine("Action :" + tipoDeInstruccion + "Descargando..");
                        break;
                }


            }

        }
        public static void InsertarArchivo(int id_Usuario, string nombre)
        {
            Archivos item = new Archivos();
            item.usuarios.id = id_Usuario;
            item.nombre = nombre;
            item.fecha = DateTime.Now.ToString("M-d-yyyy");
            item.ubicacion = "\\recibidos\\descifrados\\" + nombre;
            LNArchivos.Instance.Insertar(item);
            Console.WriteLine("Insert in DB Correct");
        }

        public static void ActualizarArchivo(int id_Archivo, string nombre)
        {
            Archivos item = new Archivos();
            item.id = id_Archivo;
            item = LNArchivos.Instance.ListarId(item);
            
            item.nombre = nombre;
            item.fecha = DateTime.Now.ToString("M-d-yyyy");
            item.ubicacion = "\\recibidos\\descifrados\\" + nombre;
            LNArchivos.Instance.Actualizar(item);
            Console.WriteLine("Update in DB Correct");
        }
    }
}
