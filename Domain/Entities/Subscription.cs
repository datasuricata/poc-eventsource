using Domain.Validator;

namespace Domain.Entities
{
    public class Subscription : Entity
    {
        public string Name { get; set; }
        public string Domain { get; set; }
        public string Address { get; set; }

        public void Validate()
        {
            Assert When = DomainValidator.Validate;

            When(string.IsNullOrEmpty(Name), "Nome do representante legal é obrigatório");
            When(string.IsNullOrEmpty(Domain), "Informar o dominio é obrigatório");
            When(string.IsNullOrEmpty(Address), "Endereço deve ser informado");
        }
    }
}
