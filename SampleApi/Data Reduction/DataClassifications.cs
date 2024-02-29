using Microsoft.Extensions.Compliance.Classification;
// credit: https://andrewlock.net/redacting-sensitive-data-with-microsoft-extensions-compliance/

namespace Bnaya.Samples;

public static class DataClassifications
{
    public static DataClassificationSet Sensitive { get; } = new DataClassificationSet(DataTaxonomy.SensitiveData);

    public static DataClassificationSet Personal { get; } = new DataClassificationSet(DataTaxonomy.PersonalData);
}

