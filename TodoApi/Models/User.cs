namespace TodoApi.Models;
public class User
{
    public User(string username, string password, string token)
    {
        Id = Guid.NewGuid();
        Username = username;
        Password = password;
        Token = token;
    }

    public User(string username, string password)
    {
        Id = Guid.NewGuid();
        Username = username;
        Password = password;
        Token = Guid.NewGuid().ToString();
    }

    public User()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; private set; }
    public string Username { get; private set; }
    public string Password { get; private set; }
    public string Token { get; private set; }
}
