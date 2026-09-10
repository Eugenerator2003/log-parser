using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogParser.Data
{
    public abstract class SqliteDatabase : IDisposable
    {
        protected SqliteConnection Connection { get; }

        private string _connectionString;

        public SqliteDatabase(string connectionString)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(connectionString);
            _connectionString = connectionString;
            Connection = new SqliteConnection(_connectionString);
            Connection.Open();
            Connection.Execute("PRAGMA journal_mode=WAL;");
            Connection.Execute("PRAGMA synchronous=NORMAL;");
        }

        public IEnumerable<dynamic> ExecuteQuery(string query)
        {
            return Connection.Query(query);
        }

        public void Dispose()
        {
            Connection.Dispose();
        }
    }
}
