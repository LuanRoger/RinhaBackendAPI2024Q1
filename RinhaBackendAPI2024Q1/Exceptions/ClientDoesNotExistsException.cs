namespace RinhaBackendAPI2024Q1.Exceptions;

public class ClientDoesNotExistsException(int clienteId) : 
    Exception(string.Format(MESSAGE, clienteId))
{
    private const string MESSAGE = "The client does not exists with the ID[{0}]";
}