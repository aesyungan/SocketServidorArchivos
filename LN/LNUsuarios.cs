
using AD;
using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LN
{
    public class LNUsuarios
    {

        #region statico
        private static LNUsuarios instance;
        public static LNUsuarios Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LNUsuarios();
                }
                return instance;
            }
        }
        #endregion
        #region funciones
        public List<Usuarios> Listar()
        {
            List<Usuarios> list = new List<Usuarios>();
            BaseDatos bd = new BaseDatos();
            bd.Conectar();
            bd.CrearComandoStrSql("select *from public.usuarios");
            list = Mapear(bd.EjecutarConsulta());
            bd.Desconectar();
            return list;
        }
        public Usuarios ListarId(Usuarios item)
        {
            Usuarios itemRes = new Usuarios();
            BaseDatos bd = new BaseDatos();
            bd.Conectar();
            bd.CrearComandoStrSql("select *from public.usuarios where ar.id=@id");
            bd.AsignarParametroInt("@id", item.id);
            foreach (Usuarios i in Mapear(bd.EjecutarConsulta()))
            {
                itemRes = i;
            }
            bd.Desconectar();
            return itemRes;
        }
        public Usuarios Logear(Usuarios usuarios)
        {
            Usuarios item = null;
            BaseDatos bd = new BaseDatos();
            bd.Conectar();
            bd.CrearComandoStrSql("select *from public.usuarios as u where u.cedula=@cedula and u.clave=@clave");
            bd.AsignarParametro("@cedula", usuarios.cedula);
            bd.AsignarParametro("@clave", usuarios.clave);
            foreach (Usuarios ite in Mapear(bd.EjecutarConsulta()))
            {
                item = ite;
            }
           
            bd.Desconectar();
            return item;
        }
        public List<Usuarios> Mapear(System.Data.Common.DbDataReader Datos)
        {
            List<Usuarios> list = new List<Usuarios>();
            while (Datos.Read())
            {
                Usuarios item = new Usuarios();
                item.id = Convert.ToInt32(Datos.GetValue(0));
                item.nombre = Convert.ToString(Datos.GetValue(1));
                item.cedula = Convert.ToString(Datos.GetValue(2));
                item.clave = Convert.ToString(Datos.GetValue(3));
                list.Add(item);
            }
            return list;
        }
        #endregion

    }
}
