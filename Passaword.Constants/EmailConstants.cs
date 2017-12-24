using System;

namespace Passaword.Constants
{
    public class EmailConstants
    {
        public const string Subject = "Subject";
        public const string ReplyTo = "ReplyTo";
        public const string To = "To";
        public const string From = "From";
        public const string TextFormat = "TextFormat";

        public enum TextFormats
        {
            PlainText,
            Html
        }

        public class MessageTypes
        {
            public const string Encrypted = "Encrypted";
            public const string Decrypted = "Decrypted";
        }
    }
}