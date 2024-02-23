using Microsoft.Extensions.Compliance.Classification;
// credit: https://andrewlock.net/redacting-sensitive-data-with-microsoft-extensions-compliance/

namespace Bnaya.Samples;

public class SensitiveDataAttribute : DataClassificationAttribute
{
    public SensitiveDataAttribute() : base(DataTaxonomy.SensitiveData) { }
}
