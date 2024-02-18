namespace RinhaBackendAPI2024Q1.Exceptions;

public class TransacaoNotSupportedException() : Exception(MESSAGE)
{
    private const string MESSAGE = "The trnasaction {0} is not supported.";
}