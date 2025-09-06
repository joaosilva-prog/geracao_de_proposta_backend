using System.Data;
using FluentValidation;
using GeracaoPropostaOrigo.Model;

namespace GeracaoPropostaOrigo.PostValidator
{
    public class PropostaValidator : AbstractValidator<Proposta>
    {
        public PropostaValidator() 
        {
            RuleFor(x => x.TotalConsumo > 0).NotEmpty();
            RuleFor(x => x.ClasseCliente).NotEmpty();
            RuleFor(x => x.PlacaCliente > 0);
            RuleFor(x => x.KwhUnit > 0).NotEmpty();
        }
    }
}
