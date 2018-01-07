using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Passaword.Encryption
{
    public class RsaStringParameters
    {
        public virtual RSAParameters ToRsa()
        {
            throw new NotImplementedException();
        }
    }

    public class RsaPublicParameters : RsaStringParameters
    {
        public RsaPublicParameters() { }
        public RsaPublicParameters(RSAParameters parameters)
        {
            Exponent = Convert.ToBase64String(parameters.Exponent);
            Modulus = Convert.ToBase64String(parameters.Modulus);
        }

        public string Exponent { get; set; }
        public string Modulus { get; set; }

        public override RSAParameters ToRsa()
        {
            return new RSAParameters
            {
                Exponent = Convert.FromBase64String(Exponent),
                Modulus = Convert.FromBase64String(Modulus)
            };
        }
    }

    public class RsaPrivateParameters : RsaStringParameters
    {
        public RsaPrivateParameters() { }
        public RsaPrivateParameters(RSAParameters parameters)
        {
            D = Convert.ToBase64String(parameters.D);
            DP = Convert.ToBase64String(parameters.DP);
            DQ = Convert.ToBase64String(parameters.DQ);
            InverseQ = Convert.ToBase64String(parameters.InverseQ);
            P = Convert.ToBase64String(parameters.P);
            Q = Convert.ToBase64String(parameters.Q);
        }

        public string D { get; set; }
        public string DP { get; set; }
        public string DQ { get; set; }
        public string InverseQ { get; set; }
        public string P { get; set; }
        public string Q { get; set; }

        public override RSAParameters ToRsa()
        {
            return new RSAParameters
            {
                D = Convert.FromBase64String(D),
                DP = Convert.FromBase64String(DP),
                DQ = Convert.FromBase64String(DQ),
                InverseQ = Convert.FromBase64String(InverseQ),
                P = Convert.FromBase64String(P),
                Q = Convert.FromBase64String(Q)
            };
        }
    }
}
