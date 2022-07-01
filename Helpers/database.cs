﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PinPadWebApi.Helpers
{
    class Database
    {
        IDataReader rd = null;
        int rowsAfected = 0;
        public SqlConnection conn;

        public Database()
        {
            String conectionString = "Server=ayt.database.windows.net;Database=aytProd;User Id=aytuser;Password=$r%ER2aY#wBD3cDP;";
            conn = new SqlConnection(conectionString);
            conn.Close();
        }

        public async Task<IDataReader> Query(String queryStr)
        {
            SqlCommand command = new SqlCommand(queryStr, conn);
            if (conn.State == ConnectionState.Closed)
               conn.Open();
            try
            {
                rd = command.ExecuteReader();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return rd;
        }

        public async Task<int> QueryInsert(String queryStr)
        {
            SqlCommand command = new SqlCommand(queryStr, conn);
            if (conn.State == ConnectionState.Closed)
                await conn.OpenAsync();
            try
            {
                rowsAfected = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return -2;
            }
            return rowsAfected;
        }

        public async Task<int> QueryStroredProcedure(String storedProcedure, List<parametrosSP> parametros)
        {
            using (conn)
            {
                using (SqlCommand cmd = new SqlCommand(storedProcedure, conn))
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    foreach (parametrosSP param in parametros)
                    {
                        cmd.Parameters.AddWithValue(param.nombre, param.valor);
                    }
                    try
                    {
                        rowsAfected = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            return rowsAfected;
        }
    }
}
