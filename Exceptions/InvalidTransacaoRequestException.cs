namespace RinhaBackendAPI2024Q1.Exceptions;

public class InvalidTransacaoRequestException() : Exception(MESSAGE)
{
    private const string MESSAGE = "The request body is invalid.";
}