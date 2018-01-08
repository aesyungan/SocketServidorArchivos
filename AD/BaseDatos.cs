using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AD
{
    public class BaseDatos
    {
        private DbTransaction Tran = null;
        private DbConnection conexion = null;
        private DbCommand comando = null;
        private DbDataAdapter adapter = null;
        private string cadenaConexion;
        private static DbProviderFactory factory = null;
        public BaseDatos()
        {
            Configurar();
        }

        //metodo AccesoDatoss
        private void Configurar()
        {
            try
            {

                string proveedor = AD.Properties.Settings.Default.PROVEEDOR_POSTGRESQL;
                //local
                cadenaConexion = AD.Properties.Settings.Default.CADENA_CONEXION;
                //remote
                // cadenaConexion = Properties.Settings.Default.CADENA_CONEXIONAzure;

                factory = DbProviderFactories.GetFactory(proveedor);
            }
            catch (ConfigurationException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //metodo para descopnectar datos
        public void Desconectar()
        {
            if (conexion.State.Equals(ConnectionState.Open))
            {
                conexion.Close();
            }
        }
        //metodo de conectar
        public void Conectar()
        {
            if (conexion != null && !conexion.State.Equals(ConnectionState.Closed))
            {

            }
            try
            {
                if (conexion == null)
                {
                    conexion = factory.CreateConnection();
                    conexion.ConnectionString = cadenaConexion;
                }
                conexion.Open();
            }
            catch (DataException ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //metodo para comando tipo texto
        public void CrearComandoStrSql(string sentenciaSQL)
        {
            comando = factory.CreateCommand();
            comando.Connection = conexion;
            comando.CommandType = CommandType.Text;
            comando.CommandText = sentenciaSQL;
        }

        public void CrearComandoTran(string sentenciaSQL)
        {
            comando = factory.CreateCommand();
            comando.Connection = conexion;
            comando.CommandType = CommandType.StoredProcedure;
            comando.CommandText = sentenciaSQL;
            comando.Transaction = Tran;
        }


        public DataSet ConstruirAdapter()
        {
            DataSet ds = new DataSet();
            adapter = factory.CreateDataAdapter();
            adapter.SelectCommand = comando;
            adapter.Fill(ds, "Articulo");
            return ds;

        }

        public DataSet ConstruirAdapter(String strDataSet, String strName)
        {
            DataSet ds = new DataSet(strDataSet);
            adapter = factory.CreateDataAdapter();
            adapter.SelectCommand = comando;
            adapter.Fill(ds, strName);
            return ds;

        }


        public void EjecutarComando()
        {
            comando.ExecuteNonQuery();
        }


        public void AsignarParametro(string nombreparam, string valor)
        {
            DbParameter parametro = factory.CreateParameter();
            parametro.ParameterName = nombreparam;
            parametro.Value = valor;
            comando.Parameters.Add(parametro);
        }
        public void AsignarParametroInt(string nombreparam, int valor)
        {
            DbParameter parametro = factory.CreateParameter();
            parametro.ParameterName = nombreparam;
            parametro.DbType = DbType.Int32;
            parametro.Value = valor;
            comando.Parameters.Add(parametro);
        }

        public void AsignarParametroDecimal(string nombreparam, Object valor)
        {
            DbParameter parametro = factory.CreateParameter();
            parametro.ParameterName = nombreparam;
            parametro.DbType = DbType.Decimal;
            parametro.Value = valor;
            comando.Parameters.Add(parametro);
        }

        public void AsignarParametroFecha(string nombreparam, DateTime valor)
        {
            DbParameter parametro = factory.CreateParameter();
            parametro.ParameterName = nombreparam;
            parametro.DbType = DbType.DateTime;
            parametro.Value = valor;
            comando.Parameters.Add(parametro);
        }

        public void AsignarParametroBoolean(string nombreparam, Boolean valor)
        {
            DbParameter parametro = factory.CreateParameter();
            parametro.ParameterName = nombreparam;
            parametro.DbType = DbType.Boolean;
            parametro.Value = valor;
            comando.Parameters.Add(parametro);
        }
        public void AsignarParametrobyte(string nombreparam, byte valor)
        {
            DbParameter parametro = factory.CreateParameter();
            parametro.ParameterName = nombreparam;
            parametro.DbType = DbType.Byte;
            parametro.Value = valor;
            comando.Parameters.Add(parametro);
        }

        public void AsignarParametroImagen(string nombreparam, Object valor)
        {
            DbParameter parametro = factory.CreateParameter();
            parametro.ParameterName = nombreparam;
            parametro.DbType = DbType.Byte;
            parametro.Value = valor;
            comando.Parameters.Add(parametro);
        }
        public DbDataReader EjecutarConsulta()
        {

            return (comando.ExecuteReader());

        }
        public int EjecutarComandoSqlReturnID()
        {
            //retornara el id que fue insertado serialmente
            int id = 0;
            System.Data.Common.DbDataReader Datos = comando.ExecuteReader();
            while (Datos.Read())
            {
                id = Convert.ToInt32(Datos.GetValue(0));
            }
            return id;
        }

    }
}
