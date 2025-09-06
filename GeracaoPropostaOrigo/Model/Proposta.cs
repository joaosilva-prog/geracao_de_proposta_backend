using System.ComponentModel.DataAnnotations;
using GeracaoPropostaOrigo.Enums;

namespace GeracaoPropostaOrigo.Model;

public class Proposta
{
    [Required(ErrorMessage = "Erro, este campo é obrigatório.")]
    public string? RazaoSocial { get; set; }

    [Required(ErrorMessage = "Erro, este campo é obrigatório.")]
    [MinLength(0, ErrorMessage = "Erro, valor Total de Consumo do Cliente inválido.")]
    public decimal TotalConsumo { get; set; }

    [Required(ErrorMessage = "Erro, este campo é obrigatório.")]
    public ClasseCliente ClasseCliente { get; set; }

    [MinLength(0, ErrorMessage = "Erro, valor de Placa do Cliente inválido.")]
    public decimal? PlacaCliente { get; set; }

    [Required(ErrorMessage = "Erro, este campo é obrigatório.")]
    [Range(0, 100, ErrorMessage = "Erro, valor de Desconto do Mês inválido.")]
    public decimal DescontoMes { get; set; }

    [Required(ErrorMessage = "Erro, este campo é obrigatório.")]
    [MinLength(0, ErrorMessage = "Erro, valor Unitário por KWH inválido.")]
    public decimal KwhUnit { get; set; }

}
