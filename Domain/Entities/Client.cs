using Domain.Validator;
using System;

namespace Domain.Entities
{
    public class Client : Entity
    {
        public Guid OrderId { get; set; }
        public Guid SubscriptionId { get; set; }
        public int IntegrationApiKey { get; set; }
        public string Domain { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }


        public void Validate()
        {
            Assert When = DomainValidator.Validate;

            When(string.IsNullOrEmpty(Name), "Nome do representante legal é obrigatório");
            When(string.IsNullOrEmpty(Domain), "Informar o dominio é obrigatório");
            When(string.IsNullOrEmpty(Address), "Endereço deve ser informado");
            When(OrderId == default, "Ordem do evento deve ser informada");
            When(SubscriptionId == default, "Chave de inscrição deve ser informada");
        }
    }
}