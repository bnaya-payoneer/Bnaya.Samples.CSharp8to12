using Microsoft.Extensions.Compliance.Classification;
using Microsoft.Extensions.Compliance.Redaction;

namespace Bnaya.Samples;

internal class CustomReductor : Redactor
{
    public static readonly Redactor Default = new CustomReductor();
    public override int GetRedactedLength(ReadOnlySpan<char> input) => input.Length;

    public override int Redact(ReadOnlySpan<char> source, Span<char> destination)
    {
        destination.Fill('*');
        return destination.Length;
    }
}

//internal class CustomReductorProvider : IRedactorProvider
//{

//}