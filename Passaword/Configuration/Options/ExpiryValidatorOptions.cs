using System;
using System.Collections.Generic;
using System.Text;
using Passaword.Constants;

namespace Passaword.Configuration.Options
{
    public class ExpiryValidatorOptions
    {
        public bool IsRequired { get; set; } = true;
        public ValidationStage ValidationStage { get; set; } = ValidationStage.AfterGet;
    }
}
