using CFT_Solutions.Core.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CFT_Solutions.Core.Repository
{
    public partial class BaseRepository<T> : IRepository<T> where T : BaseEntity
    {
        private SqlConnection _connection;
        private readonly ConnectionStrings _connectionStrings;
        private readonly IConfiguration _iConfig;

        /// <summary>
        /// Constructor to init MySQL connection
        /// </summary>
        public BaseRepository(IOptions<ConnectionStrings> connectionStrings,
            IConfiguration iConfig)
        {
            _iConfig = iConfig;
            string dataSource = _iConfig.GetSection("ConnectionStrings:DataSource").Value;
            string database = _iConfig.GetSection("ConnectionStrings:Database").Value;
            string userName = _iConfig.GetSection("ConnectionStrings:UserName").Value;
            string password = _iConfig.GetSection("ConnectionStrings:Password").Value;
            string connectionString = $"server={dataSource};database={database};user={userName};password={password};";

       //     string connectionString =
       //$"Server={dataSource};Database={database};Trusted_Connection=True;MultipleActiveResultSets=True;";
            _connection = new SqlConnection(connectionString);
          
        }
        /// <summary>
        /// map result set to Entity
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="propertyKeyMap"></param>
        /// <returns></returns>
        public virtual T PopulateRecord(SqlDataReader reader, IDictionary<string, string> propertyKeyMap = null)
        {
            if (reader != null)
            {
                var entity = GetInstance<T>();
                if (propertyKeyMap == null)
                {
                    foreach (var prop in entity.GetType().GetProperties())
                    {
                        if (HasColumn(reader, prop.Name))
                        {
                            if (reader[prop.Name] != DBNull.Value)
                            {
                                if (prop != null)
                                {
                                    Type t = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                                    object safeValue = (reader[prop.Name] == null) ? null : Convert.ChangeType(reader[prop.Name], t);

                                    prop.SetValue(entity, safeValue, null);
                                }

                                //Type propType = prop.PropertyType;
                                //prop.SetValue(entity, Convert.ChangeType(reader[prop.Name], propType), null);
                            }
                        }
                    }
                    return entity;
                }
                else
                {
                    foreach (var propkey in propertyKeyMap)
                    {
                        var prop = entity.GetType().GetProperties().Where(m => m.Name.ToLower() == propkey.Key.ToLower()).FirstOrDefault();
                        if (HasColumn(reader, propkey.Value) && prop != null)
                        {
                            if (reader[propkey.Value] != DBNull.Value)
                            {
                                if (prop != null)
                                {
                                    Type t = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                                    object safeValue = (reader[prop.Name] == null) ? null : Convert.ChangeType(reader[prop.Name], t);

                                    prop.SetValue(entity, safeValue, null);
                                }

                                //Type propType = prop.PropertyType;
                                //prop.SetValue(entity, Convert.ChangeType(reader[propkey.Value], propType), null);
                            }
                        }
                    }
                    return entity;
                }
            }
            return GetInstance<T>();
        }


        /// <summary>
        /// Get the istance of the entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected To GetInstance<To>()
        {
            return (To)FormatterServices.GetUninitializedObject(typeof(T));
        }


        /// <summary>
        /// Check if the coloum exsist in the Datareader
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        protected bool HasColumn(IDataRecord dr, string columnName)
        {
            for (int i = 0; i < dr.FieldCount; i++)
            {
                if (dr.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// get array of records for a entity
        /// </summary>
        /// <param name="command">Sql Command with parameters or query.</param>
        /// <returns></returns>
        public IEnumerable<T> GetRecords(SqlCommand command)
        {
            var list = new List<T>();
            command.Connection = _connection;
            if (_connection.State == ConnectionState.Closed)
                _connection.Open();
            try
            {
                var reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                        list.Add(PopulateRecord(reader));
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
            }
            finally
            {
                _connection.Close();
            }
            return list;
        }

        /// <summary>
        /// get record for a entity
        /// </summary>
        /// <param name="command">Sql Command with parameters or query.</param>
        /// <returns></returns>
        public T GetRecord(SqlCommand command)
        {
            T record = null;
            command.Connection = _connection;
            if (_connection.State == ConnectionState.Closed)
                _connection.Open();
            try
            {
                var reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        record = PopulateRecord(reader);
                        break;
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
            }
            finally
            {
                _connection.Close();
            }
            return record;
        }

        /// <summary>
        /// Execute A procedure
        /// </summary>
        /// <param name="command">Sql Command with parameters or query.</param>
        /// <returns></returns>
        public IEnumerable<T> ExecuteStoredProc(SqlCommand command)
        {
            var list = new List<T>();
            command.Connection = _connection;
            command.CommandType = CommandType.StoredProcedure;
            if (_connection.State == ConnectionState.Closed)
                _connection.Open();
            try
            {
                var reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        var record = PopulateRecord(reader);
                        if (record != null) list.Add(record);
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
            }
            finally
            {
                _connection.Close();
            }
            return list;
        }

        /// <summary>
        /// get array of records for a entity
        /// </summary>
        /// <param name="command">Sql Command with parameters or query.</param>
        /// <param name="propertyMap">Maping of Class property to reader coloum.</param>
        /// <returns></returns>
        public IEnumerable<T> GetRecords(SqlCommand command, IDictionary<string, string> propertyMap)
        {
            var list = new List<T>();
            command.Connection = _connection;
            if (_connection.State == ConnectionState.Closed)
                _connection.Open();
            try
            {
                var reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                        list.Add(PopulateRecord(reader, propertyMap));
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
            }
            finally
            {
                _connection.Close();
            }
            return list;
        }

        /// <summary>
        /// get record for a entity
        /// </summary>
        /// <param name="command">Sql Command with parameters or query.</param>
        /// <param name="propertyMap">Maping of Class property to reader coloum.</param>
        /// <returns></returns>
        public T GetRecord(SqlCommand command, IDictionary<string, string> propertyMap)
        {
            T record = null;
            command.Connection = _connection;
            if (_connection.State == ConnectionState.Closed)
                _connection.Open();

            try
            {
                var reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        record = PopulateRecord(reader, propertyMap);
                        break;
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
            }
            finally
            {
                _connection.Close();
            }
            return record;
        }

        /// <summary>
        /// Execute A procedure
        /// </summary>
        /// <param name="command">Sql Command with parameters or query.</param>
        /// <returns>Return Datatable with all records</returns>
        public DataTable ExecuteStoredProcedure(SqlCommand command)
        {
            IDataReader reader = null;
            DataTable table = new DataTable();
            command.Connection = _connection;
            command.CommandType = CommandType.StoredProcedure;
            _connection.Open();
            try
            {
                try
                {
                    using (reader = command.ExecuteReader())
                    {
                        table.Load(reader);
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
            }
            finally
            {
                _connection.Close();
            }
            return table;
        }


        public DataSet ExecuteStoredProcedureDS(SqlCommand command)
        {
            SqlDataAdapter dbAdapter = new SqlDataAdapter();
            DataSet ds = new DataSet();
            command.Connection = _connection;
            command.CommandType = CommandType.StoredProcedure;
            dbAdapter.SelectCommand = command;
            _connection.Open();
            try
            {
                dbAdapter.Fill(ds);

            }
            finally
            {
                _connection.Close();
            }
            return ds;
        }

        /// <summary>
        /// Execute A procedure
        /// </summary>
        /// <param name="command">Sql Command with parameters or query.</param>
        /// <param name="propertyMap">Maping of Class property to reader coloum.</param>
        /// <returns></returns>
        public IEnumerable<T> ExecuteStoredProc(SqlCommand command, IDictionary<string, string> propertyMap)
        {
            var list = new List<T>();
            command.Connection = _connection;
            command.CommandType = CommandType.StoredProcedure;
            if (_connection.State == ConnectionState.Closed)
                _connection.Open();
            try
            {
                var reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        var record = PopulateRecord(reader, propertyMap);
                        if (record != null) list.Add(record);
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
            }
            finally
            {
                _connection.Close();
            }
            return list;
        }

        /// <summary>
        /// Execute A procedure
        /// </summary>
        /// <param name="command">Sql Command with parameters or query.</param>
        /// <returns>Return affected records count</returns>
        public int ExecuteProc(SqlCommand command)
        {
            int rowsAffected;
            IDataReader reader = null;
            DataTable table = new DataTable();
            command.Connection = _connection;
            command.CommandType = CommandType.StoredProcedure;
            //_connection.Open();
            if (_connection.State == ConnectionState.Closed)
                _connection.Open();
            try
            {
                rowsAffected = Convert.ToInt32(command.ExecuteScalar());
            }
            finally
            {
                _connection.Close();
            }

            return rowsAffected;
        }

        /// <summary>
        /// Execute A procedure
        /// </summary>
        /// <param name="command">Sql Command with parameters or query.</param>
        /// <returns>Return affected records count</returns>
        public int ExecuteNonQueryProc(SqlCommand command)
        {
            int rowsAffected;
            IDataReader reader = null;
            DataTable table = new DataTable();
            command.Connection = _connection;
            command.CommandType = CommandType.StoredProcedure;
            //_connection.Open();
            if (_connection.State == ConnectionState.Closed)
                _connection.Open();
            try
            {
                rowsAffected = command.ExecuteNonQuery();
            }
            finally
            {
                _connection.Close();
            }

            return rowsAffected;
        }

        /// <summary>
        /// Execute A procedure
        /// </summary>
        /// <param name="command">Sql Command with parameters or query.</param>
        /// <returns>Return affected records count</returns>
        public object ExecuteProcedure(SqlCommand command)
        {
            object returnObj;
            IDataReader reader = null;
            DataTable table = new DataTable();
            command.Connection = _connection;
            command.CommandType = CommandType.StoredProcedure;
            //_connection.Open();
            if (_connection.State == ConnectionState.Closed)
                _connection.Open();
            try
            {
                returnObj = command.ExecuteScalar();
            }
            finally
            {
                _connection.Close();
            }

            return returnObj;
        }

        /// <summary>
        /// Execute A query
        /// </summary>
        /// <param name="command"> or query.</param>
        /// <returns>Return object</returns>
        public object ExecuteQuery(SqlCommand command)
        {
            object returnObj;
            //IDataReader reader = null;
            command.Connection = _connection;
            //_connection.Open();
            if (_connection.State == ConnectionState.Closed)
                _connection.Open();
            try
            {
                returnObj = command.ExecuteScalar();
            }
            finally
            {
                _connection.Close();
            }

            return returnObj;
        }

        ///Below 2 methods Added by Keshaw
        ///
        /// <summary>
        /// Execute A Query
        /// </summary>
        /// <param name="command">Sql Command with parameters or query.</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string query)
        {
            //var list = new List<T>();
            int rowsAdded;
            SqlCommand command = new SqlCommand(query);
            command.Connection = _connection;
            //command.CommandType = CommandType.;
            if (_connection.State == ConnectionState.Closed)
                _connection.Open();
            // _connection.Open();
            try
            {
                rowsAdded = command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _connection.Close();
            }
            return rowsAdded;
        }

        //public object ExecuteReader(MySqlCommand command)
        //{
        //    object returnObj;
        //    //IDataReader reader = null;
        //    command.Connection = _connection;
        //    //_connection.Open();
        //    if (_connection.State == ConnectionState.Closed)
        //        _connection.Open();
        //    try
        //    {
        //        MySqlDataReader rdr = command.ExecuteReader();
        //    }
        //    finally
        //    {
        //        _connection.Close();
        //    }

        //    return returnObj;
        //}
    }
}
