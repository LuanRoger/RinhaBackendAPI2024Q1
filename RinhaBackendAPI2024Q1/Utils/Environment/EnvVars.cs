using RinhaBackendAPI2024Q1.Exceptions;

namespace RinhaBackendAPI2024Q1.Utils.Environment;

public static class EnvVars
{
    private const string POSTGRES_CONNECTION = "POSTGRES_CONNECTION";
    
    public static string GetConnectionString()
    {
        string? connString = System.Environment.GetEnvironmentVariable(POSTGRES_CONNECTION);
        if(connString is null)
            throw new ConnectionStringNotProvided();
        return connString;
    }
}