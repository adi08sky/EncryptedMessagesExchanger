using System;
using System.Data.SqlClient;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace BackendConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            const string connectionString = "Server={localDataBase}\\SQLEXPRESS;Database=MessagesDb;Trusted_Connection=True;";

            try
            {
                for (int i = 1; i < 100; i++)
                {
                    string massage = "When I sent this encrypted message, it was: " + DateTime.Now;
                    byte[] key = null;

                    string encryptedMessage = CipherUtility.Encrypt(ref key, massage);
                    int id = InsertMessageToDb(connectionString, encryptedMessage);

                    var result = GetMessageAsync(id, key).GetAwaiter().GetResult();

                    Console.WriteLine($"Cycle number: {i}");
                    Console.WriteLine("Encrypted Message: " + encryptedMessage);
                    Console.WriteLine("Key: " + Convert.ToBase64String(key));
                    Console.WriteLine("Decrypted Message: " + result);
                    Console.WriteLine();

                    Thread.Sleep(15000);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        static int InsertMessageToDb(string connectionString, string message)
        {
            var sqlConnection = new SqlConnection(connectionString);

            try
            {
                sqlConnection.Open();

                string insertQuery = $"INSERT INTO [MessagesDb].[dbo].[EncryptedMessages] (Payload) " +
                                     $"output INSERTED.ID " +
                                     $"values ('{message}')";

                SqlCommand insertCommand = new SqlCommand(insertQuery, sqlConnection);
                int id = (int)insertCommand.ExecuteScalar();
                
                sqlConnection.Close();
                return id;
            }
            catch (Exception e)
            {
                sqlConnection.Close();
                Console.WriteLine(e.Message);
                throw;
            }
        }
        
        static async Task<string> GetMessageAsync(int id, byte[] key)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("http://localhost:5001/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            string path = "https://localhost:5001/api/message/" + id;

            try
            {
                string keyString = Convert.ToBase64String(key);
                if (string.IsNullOrEmpty(keyString))
                    throw new Exception("Failed to convert byte[] to string");

                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(path),
                    Content = JsonContent.Create(keyString)
                };

                HttpResponseMessage response = await client.SendAsync(request);
                if ((int)response.StatusCode != 418)
                    throw new Exception($"Bad Response: {response.StatusCode}, it should be 418");

                string message = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(message))
                    throw new Exception($"No message or an empty message from API");

                return message;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}
