namespace Portfolio.Infrastructure.Authentication.Models;

/// <summary>
/// Represents a model for credential information used in authentication processes.
/// </summary>
/// <remarks>
/// This class encapsulates various properties related to credentials, such as algorithm, configuration, 
/// hashed and salted values, and other metadata required for authentication.
/// </remarks>
internal sealed class CredentialRepresentationModel
{
    public string Algorithm { get; set; }
    public string Config { get; set; }
    public int Counter { get; set; }
    public long CreatedDate { get; set; }
    public string Device { get; set; }
    public int Digits { get; set; }
    public int HashIterations { get; set; }
    public string HashedSaltedValue { get; set; }
    public int Period { get; set; }
    public string Salt { get; set; }
    public bool Temporary { get; set; }
    public string Type { get; set; }
    public string Value { get; set; }
}
