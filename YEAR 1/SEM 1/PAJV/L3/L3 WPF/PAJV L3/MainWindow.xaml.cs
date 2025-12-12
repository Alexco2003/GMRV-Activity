using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PAJV_L3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string sessionToken;
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Login(object sender, RoutedEventArgs e)
        {
            // Console.WriteLine("Login button clicked");

            var username = textBoxUsername.Text;
            var password = passwordBox.Password;

            sessionToken = await LoginUser(username, password);

        }

        private async Task<string> LoginUser(string username, string password)
        {
            // Initialize the HTTP client & set the required headers.
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Parse-Application-Id", "nAcQNDJGyZliOjiTMMVq8AAo61zufvrk2nhJwDk7");
            client.DefaultRequestHeaders.Add("X-Parse-REST-API-Key", "plL7RnDAjZwVeRD4CUMbpZR1VosgOu8tME9aRSHe");

            // Create the request message content.
            var content = new StringContent($"{{\"username\":\"{username}\",\"password\":\"{password}\"}}", System.Text.Encoding.UTF8, "application/json");

            // Send POST request to back4app's login endpoint.
            var response = await client.PostAsync("https://parseapi.back4app.com/login", content);

            if (response.IsSuccessStatusCode)
            {
                // Get response.
                string responseBody = await response.Content.ReadAsStringAsync();

                // Create JSON.
                var json = JsonObject.Parse(responseBody);

                var displayName = json["displayName"].ToString();
                loginStatus.Text = $"Login successful! Welcome, {displayName}.";
                gameStatus.Text = "";

                Console.WriteLine(json);

                // Return the `sessionToken` (get it from the JSON).
                return json["sessionToken"].ToString();
            }
            else
            {
                loginStatus.Text = "Login failed!";
                gameStatus.Text = "";
                Debug.WriteLine($"Login failed : {response.StatusCode}, {response.ReasonPhrase}");
                return null;
            }
        }

        private void startGame(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(sessionToken))
            {
                launchGame(sessionToken);
            }
            else
            {
                gameStatus.Text = "Please log in before starting the game.";
                loginStatus.Text = "";
            }
        }

        private void launchGame(string sessionToken)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = @"C:\Users\Alexco\Desktop\PAJV\L3\L3PAJV\build\L3PAJV.exe",
                Arguments = $"--sessionToken {sessionToken}",
                UseShellExecute = false
            };

            try 
            {
                Process gameProcess = Process.Start(startInfo);
                gameStatus.Text = "Game launched successfully!";
                loginStatus.Text = "";
            }
            catch (Exception ex)
            {
                gameStatus.Text = "Failed to launch the game.";
                loginStatus.Text = "";
                Console.WriteLine($"Error launching game: {ex.Message}");
            }

        }


    }
}
