using System;

namespace Domain.Validator
{
    public class DomainValidator : ApplicationException
    {
        public DomainValidator()
        {
        }

        public DomainValidator(string message) : base(message)
        {
        }

        public DomainValidator(string message, Exception innerException) : base(message, innerException)
        {
        }

        public static void Validate(bool error, string message)
        {
            if (error)
                throw new DomainValidator(message);
        }
    }
}
