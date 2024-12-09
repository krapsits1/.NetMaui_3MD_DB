using SQLite;
using System.IO;
using System.Threading.Tasks;
using MD3db;
using System.Diagnostics;

namespace MD3db
{
    public class DatabaseService
    {
        private static SQLiteAsyncConnection _database;

        public static async Task<SQLiteAsyncConnection> GetDatabaseAsync()
        {
            try
            {
                Debug.WriteLine("Entering GetDatabaseAsync");

                // Pārbauda vai datubāze ir inicializēta
                if (_database == null)
                {
                    Debug.WriteLine("Database is null, initializing...");

                    // Nolasa connection string no faila
                    string connectionString = await File.ReadAllTextAsync(@"C:\Temp\ConnS.txt");

                    // Izveido savienojumu ar datubāzi
                    _database = new SQLiteAsyncConnection(connectionString);

                    Debug.WriteLine("Database connection established successfully.");

                    // Izveido tabulas, ja tādas vēl neeksistē
                    await CreateTablesAsync();
                }
            }
            catch (FileNotFoundException ex)
            {
                Debug.WriteLine($"File not found: {ex.Message}");
                throw;
            }
            catch (UnauthorizedAccessException ex)
            {
                Debug.WriteLine($"Access denied to file: {ex.Message}");
                throw;
            }
            catch (InvalidDataException ex)
            {
                Debug.WriteLine($"Invalid data in the connection string: {ex.Message}");
                throw;
            }
            catch (SQLiteException ex)
            {
                Debug.WriteLine($"SQLite error: {ex.Message}");
                throw;
            }
            catch (IOException ex)
            {
                Debug.WriteLine($"I/O error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An unexpected error occurred: {ex.Message}");
                throw;
            }

            return _database;
        }

        private static async Task CreateTablesAsync()
        {
            try
            {
                Debug.WriteLine("Creating tables...");

                await _database.CreateTableAsync<Teacher>();
                Debug.WriteLine("Teacher table created.");

                await _database.CreateTableAsync<Student>();
                Debug.WriteLine("Student table created.");

                await _database.CreateTableAsync<Course>();
                Debug.WriteLine("Course table created.");

                await _database.CreateTableAsync<Assignment>();
                Debug.WriteLine("Assignment table created.");

                await _database.CreateTableAsync<Submission>();
                Debug.WriteLine("Submission table created.");

                Debug.WriteLine("All tables created successfully.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creating tables: {ex.Message}");
                throw;
            }
        }
    }
}
