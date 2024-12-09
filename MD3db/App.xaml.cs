using System.Diagnostics;

namespace MD3db
{
    public partial class App : Application
    {
        public App()
        {

            InitializeComponent();
            Debug.WriteLine($" InitializeComponent(); STRĀDĀ");
            try
            {
                // Attempt to wait for the database initialization to complete
                DatabaseService.GetDatabaseAsync();
            }
            catch (Exception ex)
            {
                // Log the error if the operation fails
                Debug.WriteLine($"DatabaseService.GetDatabaseAsync().Wait(); NESTRĀDĀ - Error: {ex.Message}");
            }







            MainPage = new MainPage();
            Debug.WriteLine($" MainPage = new MainPage(); STRĀDĀ");

        }
    }
}
