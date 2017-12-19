using System;
using System.Collections.Generic;
using System.Text;
using Passaword.Constants;

namespace Passaword.Configuration.Options
{
    public class PassphraseValidatorOptions
    {
        public bool IsRequired { get; set; }
        public ValidationStage ValidationStage { get; set; } = ValidationStage.BeforeDecrypt;
    }
}
