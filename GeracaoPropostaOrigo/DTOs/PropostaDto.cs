namespace GeracaoPropostaOrigo.DTOs;

public class PropostaDto
{
    public string? RazaoSocial { get; set; }
    public decimal ValorSemOrigoMensal { get; set; }
    public decimal ValorComOrigoMensal { get; set; }
    public decimal ValorSemOrigoAnual { get; set; }
    public decimal ValorComOrigoAnual { get; set; }
    public decimal EconomiaMensal { get; set; }
    public decimal EconomiaAnual { get; set; }
    public PropostaDto(string? razaoSocial, decimal valorSemOrigoMensal, decimal valorComOrigoMensal, decimal valorSemOrigoAnual, decimal valorComOrigoAnual, decimal economiaMensal, decimal economiaAnual)
    {
        RazaoSocial = razaoSocial;
        ValorSemOrigoMensal = valorSemOrigoMensal;
        ValorComOrigoMensal = valorComOrigoMensal;
        ValorSemOrigoAnual = valorSemOrigoAnual;
        ValorComOrigoAnual = valorComOrigoAnual;
        EconomiaMensal = economiaMensal;
        EconomiaAnual = economiaAnual;
    }
}
