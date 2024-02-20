namespace RinhaBackendAPI2024Q1.Exceptions;

public class UnboundClientLimitException() : Exception(MESSAGE)
{
    private const string MESSAGE = "The client's limit is not bound to the client's account.";
}