namespace SClient
{
    using System.Net.Http.Headers;
    using System.Net.Http.Json;
    using System.Threading.Tasks;

    class Client {
        private static readonly HttpClient client = new HttpClient();
        private const string BaseUrl = "http://localhost:5297";
        private static string? token = null;
        private static string? login = null;
        static async Task Main(string[] args) {
            try
            {
                while (true)
                {
                    Console.Clear();
                    if (string.IsNullOrEmpty(token))
                    {
                        await ShowGuestMenu();
                    }
                    else
                    {
                        await ShowUserMenu();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                PressAnyKeyToContinue();
            }
        }

        static async Task ShowGuestMenu() {
            Console.WriteLine("Menu:");
            Console.WriteLine("1. Register");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Exit");
            Console.Write("Choose an option: ");
            var choice = Console.ReadLine();
            Console.Clear();
            switch (choice) {
                case "1":
                    await ServerRegister();
                    PressAnyKeyToContinue();
                    break;
                case "2":
                    ServerLogin();
                    PressAnyKeyToContinue();
                    break;
                case "3":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid key. Try again.");
                    PressAnyKeyToContinue();
                    break;
            }
        }

        static async Task ShowUserMenu()
        {
            Console.WriteLine("Menu:");
            Console.WriteLine("1. Add elements to Array");
            Console.WriteLine("2. Sort Array");
            Console.WriteLine("3. Get Array");
            Console.WriteLine("4. Generate Random Array");
            Console.WriteLine("5. Clear Array");
            Console.WriteLine("6. Change Password");
            Console.WriteLine("7. Delete User");
            Console.WriteLine("8. Logout");
            Console.Write("Choose an option: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await SendElementsToServer();
                    PressAnyKeyToContinue();
                    break;
                case "2":
                    await SortArray();
                    PressAnyKeyToContinue();
                    break;
                case "3":
                    await GetArray();
                    PressAnyKeyToContinue();
                    break;
                case "4":
                    await GenerateRandomArray();
                    PressAnyKeyToContinue();
                    break;
                case "5":
                    await ClearArray();
                    PressAnyKeyToContinue();
                    break;
                case "6":
                    await ChangePassword();
                    PressAnyKeyToContinue();
                    break;
                case "7":
                    DeleteUser();
                    PressAnyKeyToContinue();
                    break;
                case "8":
                    token = null;
                    login = null;
                    Console.WriteLine("Logged out.");
                    PressAnyKeyToContinue();
                    break;
                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    PressAnyKeyToContinue();
                    break;
            }
        }
        static async Task ServerRegister() {
            Console.Clear();
            Console.WriteLine("enter username:");
            string? username = Console.ReadLine();
            Console.Clear();
            Console.WriteLine("enter password:");
            string? password = Console.ReadLine();
            Console.Clear();
            if (username == null || username.Length == 0 || password == null || password.Length == 0) {
                Console.WriteLine("Please enter correct username and password");
                return;
            }
            string request = BaseUrl + "/signup";
            var form = new MultipartFormDataContent();
            form.Add(new StringContent(username), "login");
            form.Add(new StringContent(password), "password");
            var response = client.PostAsync(request, form).Result;
            if (response.IsSuccessStatusCode){
                Console.WriteLine("User registered succesfully!");
                return;
            }
            Console.WriteLine("Registraition Failure");
            return;
        }
        static void DeleteUser() {
            Console.Clear();
            Console.WriteLine("enter username:");
            string? username = Console.ReadLine();
            Console.Clear();
            Console.WriteLine("enter password:");
            string? password = Console.ReadLine();
            Console.Clear();
            if (username == null || username.Length == 0 || password == null || password.Length == 0) {
                Console.WriteLine("Please enter correct username and password");
                return;
            }
            string request = BaseUrl + "/DeleteUser";
            var form = new MultipartFormDataContent();
            form.Add(new StringContent(username), "login");
            form.Add(new StringContent(password), "password");
            var response = client.PostAsync(request, form).Result;
            if (response.IsSuccessStatusCode){
                Console.WriteLine("User terminated succesfully!");
                token = null;
                login = null;
                return;
            }
            Console.WriteLine("Termination Failure");
            return;
        }
        static void ServerLogin() {
            Console.Clear();
            Console.WriteLine("Please enter your username:");
            string? username = Console.ReadLine();
            Console.WriteLine("Please enter your password:");
            string? password = Console.ReadLine();
            if (username == null || username.Length == 0 || password == null || password.Length == 0) {
                Console.WriteLine("Please enter correct username and password");
                return;
            }
            string request = BaseUrl + "/login";
            var form = new MultipartFormDataContent();
            form.Add(new StringContent(username), "login");
            form.Add(new StringContent(password), "password");
            var response = client.PostAsync(request, form).Result;
            if (response.IsSuccessStatusCode){
                Console.WriteLine("Authorization Success!");
                login = username;
                token = response.Content.ReadFromJsonAsync<UserToken>().Result.access_token;
                return;
            }
            Console.WriteLine("Authorization Failure");
            return;
        }

        static async Task ChangePassword()
        {
            try
            {
                Console.Clear();
                Console.Write("Enter old password: ");
                var oldPassword = Console.ReadLine();
                Console.Write("Enter new password: ");
                var newPassword = Console.ReadLine();

                if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword))
                {
                    Console.WriteLine("Old and new passwords cannot be empty.");
                    return;
                }

                if (oldPassword == newPassword)
                {
                    Console.WriteLine("New password must be different from the old password.");
                    return;
                }
                string request = BaseUrl + "/changePassword";
                var form = new MultipartFormDataContent();
                form.Add(new StringContent(login), "login");
                form.Add(new StringContent(oldPassword), "oldPassword");
                form.Add(new StringContent(newPassword), "newPassword");
                var response = await client.PostAsync(request, form);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Password changed successfully.");
                    token = null;
                    login = null;
                    Console.WriteLine("Session ended. Please log in again with the new password.");
                    return;
                }
                else
                {
                    Console.WriteLine($"Password change error: {response}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during password change: {ex.Message}");
            }
        }

        static async Task SendElementsToServer()
        {
            try
            {
                Console.Clear();
                Console.Write("Enter numbers separated by commas: ");
                var input = Console.ReadLine();
                if (input == null || input.Length == 0){
                    Console.WriteLine("input may not be null");
                    return;
                }
                try{
                    int[] c = input.Split(',').Select(x => int.Parse(x)).ToArray();
                }catch(Exception ex){
                    Console.WriteLine($"Invalid input: {ex.Message}");
                    return;
                }
                string request = BaseUrl + "/sort/AddElements";
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, request);
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var form = new MultipartFormDataContent();
                form.Add(new StringContent(input), "addItem");
                req.Content = form;
                HttpResponseMessage response = await client.SendAsync(req);
                var responseData = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Array successfuly sended");
                }
                else
                {
                    Console.WriteLine($"Error: {responseData}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during sending array: {ex.Message}");
            }
        }

        static async Task SortArray()
        {
            try
            {
                Console.Clear();
                string request = BaseUrl + "/sort/SortArray";
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, request);
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                Console.WriteLine("Sending sorting request...");
                HttpResponseMessage response = await client.SendAsync(req);
                var responseData = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine(responseData);
                }
                else
                {
                    Console.WriteLine($"Sorting error: {responseData}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during sorting: {ex.Message}");
            }
        }

        static async Task GetArray()
        {
            try
            {
                Console.Clear();
                string request = BaseUrl + "/sort/ShowList";
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, request);
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                Console.WriteLine("Requesting sorted array...");
                HttpResponseMessage response = await client.SendAsync(req);
                var responseData = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Sorted array: {string.Join(",", responseData)}");
                }
                else
                {
                    Console.WriteLine($"Error: {responseData}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during getting sorted array: {ex.Message}");
            }
        }
        static async Task GenerateRandomArray()
        {
            try
            {
                Console.Clear();
                Console.Write("Enter size of the random array: ");
                var size = Console.ReadLine();
                Console.Write("Enter random minium: ");
                var min = Console.ReadLine();
                Console.Write("Enter random maxium: ");
                var max = Console.ReadLine();
                
                Console.WriteLine("Generating random array...");
                string request = BaseUrl + "/sort/RandArray";
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, request);
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var form = new MultipartFormDataContent();
                form.Add(new StringContent(size), "len");
                form.Add(new StringContent(min), "min");
                form.Add(new StringContent(max), "max");
                req.Content = form;
                HttpResponseMessage response = await client.SendAsync(req);
                var responseData = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Generated array: {string.Join(",", responseData)}");
                }
                else
                {
                    Console.WriteLine($"Error: {responseData}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during generating random array: {ex.Message}");
            }
        }

        static async Task ClearArray()
        {
            try
            {
                Console.Clear();
                string request = BaseUrl + "/sort/ClearArray";
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, request);
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                Console.WriteLine("Clearing array...");
                HttpResponseMessage response = await client.SendAsync(req);
                var responseData = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Array cleared successfully.");
                }
                else
                {
                    Console.WriteLine($"Error: {responseData}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during clearing array: {ex.Message}");
            }
        }
        static void PressAnyKeyToContinue()
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();
        }
    }

    public struct UserToken
    {
        public string access_token { get; set; }
        public string username { get; set; }
    }
}