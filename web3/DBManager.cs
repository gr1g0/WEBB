using Microsoft.Data.Sqlite;
public class DBManager {
    private SqliteConnection? connection = null; 
    public bool ConnectToDB(string path) {
        try{
            Console.WriteLine("Connecting to DB...");
            connection = new SqliteConnection("Data Source=/home/kali/Documents/WEBB/web3/Users.db");
            connection.Open();
            if (connection.State != System.Data.ConnectionState.Open){
                Console.WriteLine("Connection failed!");
                return false;
            }
            Console.WriteLine("Connection succesful!");
            return true;
        }
        catch(Exception exp){
            Console.WriteLine(exp.Message);
            return false;
        }
    }
    public void Disconnect() {
        if (null == connection)
            return;
        connection.Close();
        Console.WriteLine("Disconected from DB");
    }
    public bool AddUser(string login, string password) {
        if (null == connection)
            return false;
        if (connection.State != System.Data.ConnectionState.Open)
            return false;
            
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        string REQUEST = "INSERT INTO users (Login, Password) VALUES ('" + login + "', '" + hashedPassword + "')";
        var command = new SqliteCommand(REQUEST, connection);
        int result = 0;
        try{
            result =command.ExecuteNonQuery();
        }
        catch(Exception exp){
            Console.WriteLine(exp.Message);
            return false;
        }
        if (result == 1){
            return true;
        }
        return false;
    }
    public bool CheckUser(string login, string password) {
        if (null == connection)
            return false;
        if (connection.State != System.Data.ConnectionState.Open)
            return false;

        string REQUEST = "SELECT Password FROM users WHERE Login='" + login + "'";
        var command = new SqliteCommand(REQUEST,connection);
        try{
            var reader = command.ExecuteReader();
            if (reader.Read()){
                if (BCrypt.Net.BCrypt.Verify(password, reader["Password"].ToString())){
                    return true;
                }
            }
        }
        catch(Exception exp){
            Console.WriteLine(exp.Message);
            return false;
        }
        return false;
    }
    public bool ChangePassword(string login, string password) {
        if (null == connection)
            return false;
        if (connection.State != System.Data.ConnectionState.Open)
            return false;
        
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        string REQUEST = "UPDATE users SET Password = '" + hashedPassword + "' WHERE Login = '" + login + "'";
        var command = new SqliteCommand(REQUEST,connection);
        try{
            var reader = command.ExecuteReader();
            if (reader.HasRows){
                return false;
            }
            else{
                return true;
            }
        }
        catch(Exception exp){
            Console.WriteLine(exp.Message);
            return false;
        }
    }
    public bool DeleteUser(string login) {
        if (null == connection)
            return false;
        if (connection.State != System.Data.ConnectionState.Open)
            return false;

        string REQUEST = "DELETE FROM users WHERE Login = '" + login + "'";
        var command = new SqliteCommand(REQUEST,connection);
        try{
            var reader = command.ExecuteReader();
            if (reader.HasRows){
                return false;
            }
            else{
                return true;
            }
        }
        catch(Exception exp){
            Console.WriteLine(exp.Message);
            return false;
        }
    }
}
public interface IDBManager{
    bool ConnectToDB(string path);
    void Disconnect();
    bool AddUser(string login, string password);
    bool CheckUser(string login, string password);
    bool DeleteUser(string login);
    bool ChangePassword(string login, string newPassword);
}