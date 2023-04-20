using UnityEngine;
using System.Data;
using Mono.Data.SqliteClient;
using System.IO;
using Battle.Units.Equip;

namespace Game
{
    public class EquipDB : MonoBehaviour
    {
        private static EquipDB _instance = null;
		public static EquipDB Instance => _instance;
		public bool DebugMode = true;

		private static string _sqlDBLocation = "";

		private const string SQL_DB_NAME = "EquipDB";
		private const string SQL_DB_PATH = "Assets/Resources/Data/Databases/";

		private IDbConnection _connection = null;
		private IDbCommand _command = null;
		private IDataReader _reader = null;

		void Awake()
		{
			if (DebugMode)
				Debug.Log("--- Awake ---");

			// here is where we set the file location
			// ------------ IMPORTANT ---------
			// - during builds, this is located in the project root - same level as Assets/Library/obj/ProjectSettings
			// - during runtime (Windows at least), this is located in the SAME directory as the executable
			// you can play around with the path if you like, but build-vs-run locations need to be taken into account
			_sqlDBLocation = "URI=file:" + SQL_DB_PATH + SQL_DB_NAME + ".db";

			Debug.Log(_sqlDBLocation);
			_instance = this;
			SQLiteInit();
		}
		void OnDestroy()
		{
			SQLiteClose();
		}
		
		public Armor ReadArmor(string name)
        {
			return new Armor(ReadEquipAttr(name, "Body"));
        }
		public Helm ReadHelm(string name)
        {
			return new Helm(ReadEquipAttr(name, "Head"));
        }
		public Shield ReadShield(string name)
        {
			return new Shield(ReadEquipAttr(name, "Shield"));
        }
		public Weapon ReadWeapon(string name)
        {
			return new Weapon(ReadEquipAttr(name, "Weapon"), ReadWeaponAttr(name));
        }
		private EquipAttributes ReadEquipAttr(string equipName, string db)
		{
			_connection.Open();

			_command.CommandText = "SELECT `prefabName`, `stabb`, `chop`, `crush`, `title`, `weight` FROM " + db + " WHERE name='" + equipName + "';";
			_reader = _command.ExecuteReader();
			EquipAttributes attr = new();
			while (_reader.Read())
            {
				attr.prefabName = _reader.GetString(0);
				attr.stabb = _reader.GetInt32(1);
				attr.chop = _reader.GetInt32(2);
				attr.crush = _reader.GetInt32(3);
				attr.title = _reader.GetString(4);
				attr.weight = _reader.GetInt32(5);
            }
			
			_reader.Close();
			_connection.Close();
			return attr;
		}
		private WeaponAttributes ReadWeaponAttr(string equipName)
        {
			_connection.Open();

			_command.CommandText = "SELECT `distance` FROM Weapon WHERE name='" + equipName + "'";
			_reader = _command.ExecuteReader();
			WeaponAttributes attr = new();
			while (_reader.Read())
			{
				attr.distance = _reader.GetFloat(0);
			}

			_reader.Close();
			_connection.Close();
			return attr;
		}

		private void SQLiteInit()
		{
			Debug.Log("SQLiter - Opening SQLite Connection");
			_connection = new SqliteConnection(_sqlDBLocation);
			_command = _connection.CreateCommand();
			_connection.Open();

			// WAL = write ahead logging, very huge speed increase
			_command.CommandText = "PRAGMA journal_mode = WAL;";
			_command.ExecuteNonQuery();

			// journal mode = look it up on google, I don't remember
			_command.CommandText = "PRAGMA journal_mode";
			_reader = _command.ExecuteReader();
			if (DebugMode && _reader.Read())
				Debug.Log("SQLiter - WAL value is: " + _reader.GetString(0));
			_reader.Close();

			// more speed increases
			_command.CommandText = "PRAGMA synchronous = OFF";
			_command.ExecuteNonQuery();

			// and some more
			_command.CommandText = "PRAGMA synchronous";
			_reader = _command.ExecuteReader();
			if (DebugMode && _reader.Read())
				Debug.Log("SQLiter - synchronous value is: " + _reader.GetInt32(0));
			_reader.Close();
			// implement test
			// close connection
			_connection.Close();
		}
		private void SQLiteClose()
		{
			if (_reader != null && !_reader.IsClosed)
				_reader.Close();
			_reader = null;

			if (_command != null)
				_command.Dispose();
			_command = null;

			if (_connection != null && _connection.State != ConnectionState.Closed)
				_connection.Close();
			_connection = null;
		}
	}
}
