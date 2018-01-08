using AD;
using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LN
{
    public class LNArchivos
    {
        #region statico
        private static LNArchivos instance;
        public static LNArchivos Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LNArchivos();
                }
                return instance;
            }
        }
        #endregion
        #region funciones
        public List<Archivos> Listar()
        {
            List<Archivos> list = new List<Archivos>();
            BaseDatos bd = new BaseDatos();
            bd.Conectar();
            bd.CrearComandoStrSql("select *from public.archivos");
            list = Mapear(bd.EjecutarConsulta());
            bd.Desconectar();
            return list;
        }
        public List<Archivos> Listar(Usuarios item)
        {
            List<Archivos> list = new List<Archivos>();
            BaseDatos bd = new BaseDatos();
            bd.Conectar();
            bd.CrearComandoStrSql("select *from archivos as ar where ar.id_usuario=@id_usuario ");
            bd.AsignarParametroInt("@id_usuario", item.id);
            list = Mapear(bd.EjecutarConsulta());
            bd.Desconectar();
            return list;
        }
        public Archivos ListarId(Archivos item)
        {
            Archivos itemRes = new Archivos();
            BaseDatos bd = new BaseDatos();
            bd.Conectar();
            bd.CrearComandoStrSql("select *from archivos as ar where ar.id=@id");
            bd.AsignarParametroInt("@id", item.id);
            foreach (Archivos i in Mapear(bd.EjecutarConsulta()))
            {
                itemRes = i;
            }
            bd.Desconectar();
            return itemRes;
        }

        public void Eliminar(Archivos item)
        {
            BaseDatos bd = new BaseDatos();
            bd.Conectar();
            bd.CrearComandoStrSql("delete from archivos as ar where ar.id=@id");
            bd.AsignarParametroInt("@id", item.id);
            bd.EjecutarComando();
            bd.Desconectar();


        }
        public void Insertar(Archivos item)
        {
            BaseDatos bd = new BaseDatos();
            bd.Conectar();
            bd.CrearComandoStrSql("INSERT INTO public.archivos(id_usuario, nombre, fecha, ubicacion) VALUES (@id_usuario, @nombre, @fecha, @ubicacion)");
            bd.AsignarParametroInt("@id_usuario", item.usuarios.id);
            bd.AsignarParametro("@nombre", item.nombre);
            bd.AsignarParametro("@fecha", item.fecha);
            bd.AsignarParametro("@ubicacion", item.ubicacion);
            bd.EjecutarComando();
            bd.Desconectar();

        }
        public void Actualizar(Archivos item)
        {
            BaseDatos bd = new BaseDatos();
            bd.Conectar();
            bd.CrearComandoStrSql("UPDATE public.archivos SET  id_usuario=@id_usuario, nombre=@nombre, fecha=@fecha, ubicacion=@ubicacion WHERE  id=@id");
            bd.AsignarParametroInt("@id_usuario", item.usuarios.id);
            bd.AsignarParametro("@nombre", item.nombre);
            bd.AsignarParametro("@fecha", item.fecha);
            bd.AsignarParametro("@ubicacion", item.ubicacion);
            bd.EjecutarComando();
            bd.Desconectar();
          
        }

        public List<Archivos> Mapear(System.Data.Common.DbDataReader Datos)
        {
            List<Archivos> list = new List<Archivos>();
            while (Datos.Read())
            {
                Archivos item = new Archivos();
                item.id = Convert.ToInt32(Datos.GetValue(0));
                item.usuarios.id = Convert.ToInt32(Datos.GetValue(1));
                item.nombre = Convert.ToString(Datos.GetValue(2));
                item.fecha = Convert.ToString(Datos.GetValue(3));
                item.ubicacion = Convert.ToString(Datos.GetValue(4));
                list.Add(item);
            }
            return list;
        }
        #endregion
    }
}
