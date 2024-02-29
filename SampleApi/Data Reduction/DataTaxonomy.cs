using Microsoft.Extensions.Compliance.Classification;
// credit: https://andrewlock.net/redacting-sensitive-data-with-microsoft-extensions-compliance/

namespace Bnaya.Samples;

public static class DataTaxonomy
{
    public static DataClassification SensitiveData { get; } = 
                        new(nameof(DataTaxonomy), nameof(SensitiveData));

    public static DataClassification PersonalData { get; } =
                        new(nameof(DataTaxonomy), nameof(PersonalData));
}

