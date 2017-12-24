using System.ComponentModel.DataAnnotations;

namespace Passaword.Validation
{
    public class SecretValidationRule
    {
        public int Id { get; set; }
        [MaxLength(32)]
        [Required]
        public string SecretId { get; set; }
        public Secret Secret { get; set; }

        [MaxLength(50)]
        [Required]
        public string Validator { get; set; }

        public string ValidationData { get; set; }
    }
}
