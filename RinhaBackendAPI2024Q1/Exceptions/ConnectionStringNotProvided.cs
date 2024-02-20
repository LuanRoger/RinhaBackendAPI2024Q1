namespace RinhaBackendAPI2024Q1.Exceptions;

public class ConnectionStringNotProvided() : Exception(MESSAGE)
{
    private const string MESSAGE = "A connection string for the database was not provided.";
}