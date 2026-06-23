using BCrypt.Net;


string password = "Admin@AmazeCare@123";
string hash = BCrypt.Net.BCrypt.HashPassword(password);

Console.WriteLine("Plaintext: " + password);
Console.WriteLine("BCrypt hash:");
Console.WriteLine(hash);