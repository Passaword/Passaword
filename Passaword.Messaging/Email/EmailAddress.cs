using System;
using System.Collections.Generic;
using System.Text;

namespace Passaword.Messaging.Email
{
    public class EmailAddress
    {
        public EmailAddress(string address, string name = null)
        {
            Address = address;
            Name = name ?? address;
        }

        public string Name { get; set; }
        public string Address { get; set; }
    }
}
