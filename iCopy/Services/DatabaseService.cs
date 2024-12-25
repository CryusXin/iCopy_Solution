using System;
using System.Collections.Generic;
using System.IO;
using iCopy.Models;
using Microsoft.Data.Sqlite;

namespace iCopy.Services
{
    public class DatabaseService
    {
        private static DatabaseService _instance;
        private readonly string _dbPath;
        private readonly string _connectionString;

        public static DatabaseService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DatabaseService();
                }
                return _instance;
            }
        }

        private DatabaseService()
        {
            try
            {
                // 确保SQLite被正确初始化
                SQLitePCL.Batteries_V2.Init();
                SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_dynamic_cdecl());

                string appDataPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "iCopy");

                if (!Directory.Exists(appDataPath))
                {
                    Directory.CreateDirectory(appDataPath);
                }

                _dbPath = Path.Combine(appDataPath, "clipboard.db");
                System.Diagnostics.Debug.WriteLine($"Database path: {_dbPath}");

                _connectionString = new SqliteConnectionStringBuilder
                {
                    DataSource = _dbPath,
                    Mode = SqliteOpenMode.ReadWriteCreate,
                    Cache = SqliteCacheMode.Private
                }.ToString();

                // 测试连接
                using (var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();
                    System.Diagnostics.Debug.WriteLine("Database connection test successful");
                }

                InitializeDatabase();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing DatabaseService: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw; // 重新抛出异常以便上层处理
            }
        }

        private void InitializeDatabase()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        CREATE TABLE IF NOT EXISTS ClipboardItems (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Content TEXT NOT NULL,
                            CreateTime TEXT NOT NULL
                        )";
                    command.ExecuteNonQuery();
                }
            }
        }

        public void AddClipboardItem(ClipboardItem item)
        {
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO ClipboardItems (Content, CreateTime) VALUES (@Content, @CreateTime)";
                        command.Parameters.AddWithValue("@Content", item.Content);
                        command.Parameters.AddWithValue("@CreateTime", item.CreateTime.ToString("O"));
                        command.ExecuteNonQuery();

                        command.CommandText = "SELECT last_insert_rowid()";
                        item.Id = Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in AddClipboardItem: {ex.Message}");
            }
        }

        public List<ClipboardItem> GetClipboardItems(int limit = 50)
        {
            var items = new List<ClipboardItem>();
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT Id, Content, CreateTime FROM ClipboardItems ORDER BY CreateTime DESC LIMIT @Limit";
                        command.Parameters.AddWithValue("@Limit", limit);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                items.Add(new ClipboardItem
                                {
                                    Id = reader.GetInt32(0),
                                    Content = reader.GetString(1),
                                    CreateTime = DateTime.Parse(reader.GetString(2))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetClipboardItems: {ex.Message}");
            }
            return items;
        }

        public void DeleteClipboardItem(int id)
        {
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "DELETE FROM ClipboardItems WHERE Id = @Id";
                        command.Parameters.AddWithValue("@Id", id);
                        command.ExecuteNonQuery();
                        //System.Diagnostics.Debug.WriteLine($"Deleted clipboard item with id: {id}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in DeleteClipboardItem: {ex.Message}");
            }
        }
    }
}