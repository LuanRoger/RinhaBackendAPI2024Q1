using RinhaBackendAPI2024Q1.Exceptions;

namespace RinhaBackendAPI2024Q1.Utils.Extensions;

internal static class TransacaoTypeExtension
{ 
    internal static short ConvertCharToIntBasedOnTransacaoType(this char transacaoType) =>
        transacaoType switch
        {
            'c' => 0,
            'd' => 1,
            _ => throw new TransacaoNotSupportedException()
        }; 
    internal  static char ConvertIntToCharBasedOnTransacaoType(this short transacaoType) =>
        transacaoType switch
        {
            0 => 'c',
            1 => 'd',
            _ => throw new TransacaoNotSupportedException()
        };
}