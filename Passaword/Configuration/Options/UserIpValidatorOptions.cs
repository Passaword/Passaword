using System;
using System.Collections.Generic;
using System.Text;
using Passaword.Constants;

namespace Passaword.Configuration.Options
{
    public class UserIpValidatorOptions
    {
        public bool IsRequired { get; set; } = false;
        public ValidationStage ValidationStage { get; set; } = ValidationStage.AfterGet;
    }
}
