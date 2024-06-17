public class HashPasswordByBC
{
    public string HashPassword(string password)
    {
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        return hashedPassword;
    }
    public bool VerifyPassword(string password, string hashedPassword)
    {
        bool isMatch = BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        return isMatch;
    }
}