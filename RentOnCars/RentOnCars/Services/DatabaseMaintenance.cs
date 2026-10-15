using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

namespace RentOnCars.Services
{
    public static class DatabaseMaintenance
    {
        // Disabled unless an operator explicitly supplies a policy period and applies it.
        public static int AnonymizeClosedRecords(string connectionString,int days,bool apply)
        {
            if(days<1||days>36500)throw new ArgumentException("Specify an approved retention period from 1 to 36500 days.");
            using(var con=new SqlConnection(connectionString))
            {
                con.Open();
                using(var tx=con.BeginTransaction())
                {
                    const string filter="Status<>'Active' AND IsAnonymized=0 AND COALESCE(ReturnedAt,CreatedAt)<@cutoff";
                    int count;
                    using(var cmd=new SqlCommand("SELECT COUNT(*) FROM dbo.Bookings WHERE "+filter,con,tx))
                    {cmd.Parameters.Add("@cutoff",SqlDbType.DateTime2).Value=DateTime.UtcNow.AddDays(-days);count=Convert.ToInt32(cmd.ExecuteScalar());}
                    if(apply)
                    {
                        using(var cmd=new SqlCommand("UPDATE dbo.Bookings SET CustomerFirstName='Redacted',CustomerSurname='Customer',CustomerAddress='Removed under retention policy',CustomerAge=18,IsAnonymized=1 WHERE "+filter,con,tx))
                        {cmd.Parameters.Add("@cutoff",SqlDbType.DateTime2).Value=DateTime.UtcNow.AddDays(-days);count=cmd.ExecuteNonQuery();}
                        using(var cmd=new SqlCommand("INSERT dbo.AuditEvents(Actor,Action,Detail) VALUES(@actor,'PiiAnonymized',@detail)",con,tx))
                        {
                            string actor=Environment.UserName;
                            cmd.Parameters.Add("@actor",SqlDbType.NVarChar,50).Value=actor.Substring(0,Math.Min(actor.Length,50));
                            cmd.Parameters.Add("@detail",SqlDbType.NVarChar,300).Value=count+" closed records; approved period "+days+" days";cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();return count;
                }
            }
        }
        public static string Backup(string connectionString,string destination)
        {
            var source=new SqlConnectionStringBuilder(connectionString);
            string file=Path.GetFullPath(destination);
            if(!file.EndsWith(".bak",StringComparison.OrdinalIgnoreCase)||File.Exists(file))throw new ArgumentException("Choose a new .bak file; existing backups are never overwritten.");
            Directory.CreateDirectory(Path.GetDirectoryName(file));
            source.InitialCatalog="master";
            using(var con=new SqlConnection(source.ConnectionString))
            {
                con.Open();string catalog=new SqlConnectionStringBuilder(connectionString).InitialCatalog;
                using(var cmd=new SqlCommand("BACKUP DATABASE ["+catalog.Replace("]","]]")+"] TO DISK=@file WITH COPY_ONLY,CHECKSUM",con))
                {cmd.CommandTimeout=120;cmd.Parameters.Add("@file",SqlDbType.NVarChar,4000).Value=file;cmd.ExecuteNonQuery();}
                using(var cmd=new SqlCommand("RESTORE VERIFYONLY FROM DISK=@file WITH CHECKSUM",con))
                {cmd.CommandTimeout=120;cmd.Parameters.Add("@file",SqlDbType.NVarChar,4000).Value=file;cmd.ExecuteNonQuery();}
            }
            return file;
        }
        public static void RestoreToNewCatalog(string connectionString,string backup,string newCatalog)
        {
            if(!Regex.IsMatch(newCatalog??"",@"^[A-Za-z][A-Za-z0-9_]{2,100}$"))throw new ArgumentException("Use a valid new catalog name.");
            string file=Path.GetFullPath(backup);if(!File.Exists(file))throw new FileNotFoundException("Backup not found.");
            var source=new SqlConnectionStringBuilder(connectionString){InitialCatalog="master"};
            using(var con=new SqlConnection(source.ConnectionString))
            {
                con.Open();
                using(var cmd=new SqlCommand("SELECT DB_ID(@catalog)",con))
                {cmd.Parameters.Add("@catalog",SqlDbType.NVarChar,128).Value=newCatalog;if(cmd.ExecuteScalar()!=DBNull.Value)throw new InvalidOperationException("Target catalog exists. Restore refuses to replace any database.");}
                string dataDirectory;
                using(var cmd=new SqlCommand("SELECT CONVERT(nvarchar(4000),SERVERPROPERTY('InstanceDefaultDataPath'))",con))dataDirectory=Convert.ToString(cmd.ExecuteScalar());
                if(string.IsNullOrEmpty(dataDirectory))throw new InvalidOperationException("Database data directory could not be determined.");
                var moves=new List<string>();
                using(var cmd=new SqlCommand("RESTORE FILELISTONLY FROM DISK=@file",con))
                {
                    cmd.Parameters.Add("@file",SqlDbType.NVarChar,4000).Value=file;
                    using(var rd=cmd.ExecuteReader())while(rd.Read())
                    {
                        string logical=Convert.ToString(rd["LogicalName"]).Replace("'","''");
                        string extension=Convert.ToString(rd["Type"])=="L"?".ldf":".mdf";
                        string physical=Path.Combine(dataDirectory,newCatalog+"_"+Guid.NewGuid().ToString("N")+extension).Replace("'","''");
                        moves.Add("MOVE N'"+logical+"' TO N'"+physical+"'");
                    }
                }
                using(var cmd=new SqlCommand("RESTORE DATABASE ["+newCatalog+"] FROM DISK=@file WITH CHECKSUM,"+string.Join(",",moves),con))
                {cmd.CommandTimeout=120;cmd.Parameters.Add("@file",SqlDbType.NVarChar,4000).Value=file;cmd.ExecuteNonQuery();}
            }
        }
    }
}
