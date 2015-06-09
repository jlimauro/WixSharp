using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace WixSharp
{

    public class SqlDatabase : WixEntity
    {

        #region Constructors

        public SqlDatabase() { }

        public SqlDatabase(string database, string server, params WixEntity[] items)
        {
            if (string.IsNullOrEmpty(database)) throw new ArgumentNullException("database", "database is a null reference or empty");
            if (string.IsNullOrEmpty(server)) throw new ArgumentNullException("server", "server is a null reference or empty");

            Name = database;
            Database = database;
            Server = server;

            AddItems(items);
        }

        public SqlDatabase(Id id, string database, string server, params WixEntity[] items)
            : this(database, server, items)
        {
            Id = id;
        }

        public SqlDatabase(Feature feature, string database, string server, params WixEntity[] items)
            : this(database, server, items)
        {
            Feature = feature;
        }

        public SqlDatabase(Id id, Feature feature, string database, string server, params WixEntity[] items)
            : this(database, server, items)
        {
            Id = id;
            Feature = feature;
        }

        #endregion

        public Feature Feature { get; set; }

        public SqlScript[] SqlScripts = new SqlScript[0];
        public SqlString[] SqlStrings = new SqlString[0];

        #region Wix SqlDatabase attributes

        public bool? ConfirmOverwrite { get; set; }
        public bool? ContinueOnError { get; set; }
        public bool? CreateOnInstall { get; set; }
        public bool? CreateOnReinstall { get; set; }
        public bool? CreateOnUninstall { get; set; }
        public string Database { get; set; } //required
        public bool? DropOnInstall { get; set; }
        public bool? DropOnReinstall { get; set; }
        public bool? DropOnUninstall { get; set; }
        public string Instance { get; set; }
        public string Server { get; set; } //required
        public string User { get; set; }

        #endregion

        private void AddItems(IEnumerable<WixEntity> items)
        {
            SqlStrings = items.OfType<SqlString>().ToArray();
            SqlScripts = items.OfType<SqlScript>().ToArray();

            var unexpectedItem =
                items.Except(SqlStrings)
                     .Except(SqlScripts)
                     .FirstOrDefault();

            if (unexpectedItem != null)
                throw new ApplicationException(
                    string.Format("{0} is unexpected. Only {1} and {2} items can be added to {3}",
                        unexpectedItem,
                        typeof(SqlScript),
                        typeof(SqlString),
                        this.GetType()));
        }

        internal bool MustDescendFromComponent
        {
            get
            {
                return CreateOnInstall.HasValue
                       || CreateOnReinstall.HasValue
                       || CreateOnUninstall.HasValue
                       || DropOnInstall.HasValue
                       || DropOnReinstall.HasValue
                       || DropOnUninstall.HasValue;
            }
        }
    }

    public class SqlString : WixEntity
    {

        #region Constructors

        public SqlString() { }

        public SqlString(string sql)
        {
            if (string.IsNullOrEmpty(sql)) throw new ArgumentNullException("sql", "sql is a null reference or empty");

            Name = "SqlString";
            Sql = sql;
        }

        public SqlString(Id id, string sql)
            : this(sql)
        {
            Id = id;
        }

        public SqlString(Feature feature, string sql)
            : this(sql)
        {
            Feature = feature;
        }

        public SqlString(Id id, Feature feature, string sql)
            : this(sql)
        {
            Id = id;
            Feature = feature;
        }

        #endregion 

        public Feature Feature { get; set; }

        #region Wix SqlString attributes

        public bool? ContinueOnError { get; set; }
        public bool? ExecuteOnInstall { get; set; } //partially required
        public bool? ExecuteOnReinstall { get; set; } //partially required
        public bool? ExecuteOnUninstall { get; set; } //partially required
        public bool? RollbackOnInstall { get; set; } //partially required
        public bool? RollbackOnReinstall { get; set; } //partially required
        public bool? RollbackOnUninstall { get; set; } //partially required
        public int? Sequence { get; set; }
        public string Sql { get; set; } //required
        internal string SqlDb { get; set; } //required when not under a SqlDatabase element
        public string User { get; set; }

        #endregion

    }

    public class SqlScript : WixEntity
    {

        #region Constructors

        public SqlScript() { }

        public SqlScript(string binaryKey)
        {
            if (string.IsNullOrEmpty(binaryKey)) throw new ArgumentNullException("binaryKey", "binaryKey is a null reference or empty");

            BinaryKey = binaryKey;
            Name = binaryKey;
        }

        public SqlScript(Id id, string binaryKey)
            : this(binaryKey)
        {
            Id = id;       
        }

        public SqlScript(Feature feature, string binaryKey)
            : this(binaryKey)
        {
            Feature = feature;
        }

        public SqlScript(Id id, Feature feature, string binaryKey)
            : this(id, binaryKey)
        {
            Feature = feature;
        }

        #endregion

        public Feature Feature { get; set; }

        #region Wix SqlScript attributes

        public string BinaryKey { get; set; } //required
        public bool? ContinueOnError { get; set; }
        public bool? ExecuteOnInstall { get; set; } //partially required
        public bool? ExecuteOnReinstall { get; set; } //partially required
        public bool? ExecuteOnUninstall { get; set; } //partially required
        public bool? RollbackOnInstall { get; set; } //partially required
        public bool? RollbackOnReinstall { get; set; } //partially required
        public bool? RollbackOnUninstall { get; set; } //partially required
        public int? Sequence { get; set; }
        internal string SqlDb { get; set; } //required if and only if not under a SqlDatabase
        public string User { get; set; }

        #endregion

    }

    internal static class SqlEx
    {

        static void Do<T>(this T? nullable, Action<T> action) where T : struct
        {
            if (!nullable.HasValue) return;
            action(nullable.Value);
        }

        public static void EmitAttributes(this SqlDatabase sqlDb, XElement sqlDbElement)
        {
            sqlDbElement.SetAttributeValue("Id", sqlDb.Id);
            sqlDbElement.SetAttributeValue("Database", sqlDb.Database);
            sqlDbElement.SetAttributeValue("Server", sqlDb.Server);

            sqlDb.ConfirmOverwrite.Do(b => sqlDbElement.SetAttributeValue("ConfirmOverwrite", b.ToYesNo()));
            sqlDb.ContinueOnError.Do(b => sqlDbElement.SetAttributeValue("ContinueOnError", b.ToYesNo()));
            sqlDb.CreateOnInstall.Do(b => sqlDbElement.SetAttributeValue("CreateOnInstall", b.ToYesNo()));
            sqlDb.CreateOnReinstall.Do(b => sqlDbElement.SetAttributeValue("CreateOnReinstall", b.ToYesNo()));
            sqlDb.CreateOnUninstall.Do(b => sqlDbElement.SetAttributeValue("CreateOnUninstall", b.ToYesNo()));
            sqlDb.DropOnInstall.Do(b => sqlDbElement.SetAttributeValue("DropOnInstall", b.ToYesNo()));
            sqlDb.DropOnReinstall.Do(b => sqlDbElement.SetAttributeValue("DropOnReinstall", b.ToYesNo()));
            sqlDb.DropOnUninstall.Do(b => sqlDbElement.SetAttributeValue("DropOnUninstall", b.ToYesNo()));
            if (!string.IsNullOrEmpty(sqlDb.Instance)) sqlDbElement.SetAttributeValue("Instance", sqlDb.Instance);
            if (!string.IsNullOrEmpty(sqlDb.User)) sqlDbElement.SetAttributeValue("User", sqlDb.User);
        }

        public static void EmitAttributes(this SqlString sqlString, XElement sqlStringElement)
        {
            sqlStringElement.SetAttributeValue("Id", sqlString.Id);
            sqlStringElement.SetAttributeValue("SQL", sqlString.Sql);

            sqlString.ContinueOnError.Do(b => sqlStringElement.SetAttributeValue("ContinueOnError", b.ToYesNo()));
            sqlString.ExecuteOnInstall.Do(b => sqlStringElement.SetAttributeValue("ExecuteOnInstall", b.ToYesNo()));
            sqlString.ExecuteOnReinstall.Do(b => sqlStringElement.SetAttributeValue("ExecuteOnReinstall", b.ToYesNo()));
            sqlString.ExecuteOnUninstall.Do(b => sqlStringElement.SetAttributeValue("ExecuteOnUninstall", b.ToYesNo()));
            sqlString.RollbackOnInstall.Do(b => sqlStringElement.SetAttributeValue("RollbackOnInstall", b.ToYesNo()));
            sqlString.RollbackOnReinstall.Do(b => sqlStringElement.SetAttributeValue("RollbackOnReinstall", b.ToYesNo()));
            sqlString.RollbackOnUninstall.Do(b => sqlStringElement.SetAttributeValue("RollbackOnUninstall", b.ToYesNo()));
            sqlString.Sequence.Do(i => sqlStringElement.SetAttributeValue("Sequence", i));
            if (!string.IsNullOrEmpty(sqlString.SqlDb)) sqlStringElement.SetAttributeValue("SqlDb", sqlString.SqlDb);
            if (!string.IsNullOrEmpty(sqlString.User)) sqlStringElement.SetAttributeValue("User", sqlString.User);
        }

        public static void EmitAttributes(this SqlScript sqlScript, XElement sqlScriptElement)
        {
            sqlScriptElement.SetAttributeValue("Id", sqlScript.Id);
            sqlScriptElement.SetAttributeValue("BinaryKey", sqlScript.BinaryKey);

            sqlScript.ContinueOnError.Do(b => sqlScriptElement.SetAttributeValue("ContinueOnError", b.ToYesNo()));
            sqlScript.ExecuteOnInstall.Do(b => sqlScriptElement.SetAttributeValue("ExecuteOnInstall", b.ToYesNo()));
            sqlScript.ExecuteOnReinstall.Do(b => sqlScriptElement.SetAttributeValue("ExecuteOnReinstall", b.ToYesNo()));
            sqlScript.ExecuteOnUninstall.Do(b => sqlScriptElement.SetAttributeValue("ExecuteOnUninstall", b.ToYesNo()));
            sqlScript.RollbackOnInstall.Do(b => sqlScriptElement.SetAttributeValue("RollbackOnInstall", b.ToYesNo()));
            sqlScript.RollbackOnReinstall.Do(b => sqlScriptElement.SetAttributeValue("RollbackOnReinstall", b.ToYesNo()));
            sqlScript.RollbackOnUninstall.Do(b => sqlScriptElement.SetAttributeValue("RollbackOnUninstall", b.ToYesNo()));
            sqlScript.Sequence.Do(i => sqlScriptElement.SetAttributeValue("Sequence", i));
            if (!string.IsNullOrEmpty(sqlScript.SqlDb)) sqlScriptElement.SetAttributeValue("SqlDb", sqlScript.SqlDb);
            if (!string.IsNullOrEmpty(sqlScript.User)) sqlScriptElement.SetAttributeValue("User", sqlScript.User);

        }

    }

}
