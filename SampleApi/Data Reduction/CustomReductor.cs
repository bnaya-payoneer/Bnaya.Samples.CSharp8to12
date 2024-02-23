using Microsoft.Extensions.Compliance.Redaction;

namespace Bnaya.Samples;

internal class CustomReductor : Redactor
{
    public override int GetRedactedLength(ReadOnlySpan<char> input) => input.Length;

    public override int Redact(ReadOnlySpan<char> source, Span<char> destination)
    {
        destination.Fill('*');
        return destination.Length;
    }
}
