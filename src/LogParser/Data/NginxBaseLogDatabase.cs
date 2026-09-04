using Dapper;
using LogParser.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogParser.Data
{
    public class NginxBaseLogDatabase : SqliteDatabase
    {
        public NginxBaseLogDatabase(string connectionString) : base(connectionString)
        {
            CreateTables();
        }

        public void AddRemoteAddress(RemoteAddress remoteAddress)
        {
            string sql = @"
                Insert into RemoteAddresses 
                (Address)
                Values 
                (@Address);
                Select last_insert_rowid();
            ";

            int id = Convert.ToInt32(Connection.ExecuteScalar(sql, remoteAddress));
            remoteAddress.Id = id;
        }

        public void AddRemoteUser(RemoteUser remoteUser)
        {
            string sql = @"
                Insert into RemoteUsers
                (Name)
                Values 
                (@Name);
                Select last_insert_rowid();
            ";

            int id = Convert.ToInt32(Connection.ExecuteScalar(sql, remoteUser));
            remoteUser.Id = id;
        }

        public void AddUserAgent(UserAgent userAgent)
        {
            string sql = @"
                Insert into UserAgents
                (Name)
                Values 
                (@Name);
                Select last_insert_rowid();
            ";

            int id = Convert.ToInt32(Connection.ExecuteScalar(sql, userAgent));
            userAgent.Id = id;
        }

        public void AddLog(Log log)
        {
            string sql = @"
                Insert into LogEntries
                (RemoteAddressId, RemoteUserId, StatusCode, UserAgentId, Request, DateTimeOffset, BytesCount, HttpReferer)
                Values
                (@RemoteAddressId, @RemoteUserId, @StatusCode, @UserAgentId, @Request, @DateTimeOffset, @BytesCount, @HttpReferer)
            ";

            Connection.Execute(sql, log);
        }

        public void AddLogsBatch(IEnumerable<Log> logs)
        {
            using (var transaction = Connection.BeginTransaction())
            {
                string sql = @"
                    Insert into LogEntries
                    (RemoteAddressId, RemoteUserId, StatusCode, UserAgentId, Request, DateTimeOffset, BytesCount, HttpReferer)
                    Values
                    (@RemoteAddressId, @RemoteUserId, @StatusCode, @UserAgentId, @Request, @DateTimeOffset, @BytesCount, @HttpReferer)
                ";

                Connection.Execute(sql, logs, transaction);
                transaction.Commit();
            }
        }

        private void CreateTables()
        {
            Connection.Execute(@"
                Create table if not exists RemoteAddresses(
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Address TEXT NOT NULL UNIQUE
                );
            ");

            Connection.Execute(@"
                Create table if not exists RemoteUsers(
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL UNIQUE
                );
            ");

            Connection.Execute(@"
                Create table if not exists UserAgents(
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL UNIQUE
                );
            ");

            Connection.Execute(@"
                Create table if not exists LogEntries(
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    RemoteAddressId INTEGER NOT NULL,
                    RemoteUserId INTEGER NOT NULL,
                    StatusCode INTEGER NOT NULL,
                    UserAgentId INTEGER NOT NULL,
                    Request TEXT NOT NULL,
                    DateTimeOffset TEXT NOT NULL,
                    BytesCount INTEGER NOT NULL,
                    HttpReferer TEXT NOT NULL,

                    FOREIGN KEY (RemoteAddressId) REFERENCES RemoteAddresses(Id),
                    FOREIGN KEY (RemoteUserId) REFERENCES RemoteUsers(Id),
                    FOREIGN KEY (UserAgentId) REFERENCES UserAgents(Id)
                );
            ");
        }
    }
}
