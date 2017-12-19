namespace Passaword.Validation.Passphrase
{
    public class PassphraseValidationData
    {
        public string Algorithm { get; set; }
        public string Salt { get; set; }
        public int IterationCount { get; set; }
    }
}
