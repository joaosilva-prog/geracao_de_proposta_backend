using System.Globalization;
using GeracaoPropostaOrigo.DTOs;
using GeracaoPropostaOrigo.Model;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;

namespace GeracaoPropostaOrigo.Services;

public class GeracaoDeProposta
{
    public async Task<IResult> GerarPdf(Proposta dadosBrutos)
    {
        var propostaFinal = CalculaProposta(dadosBrutos);

        string basePath = AppDomain.CurrentDomain.BaseDirectory;
        string caminhoPdf = Path.Combine(basePath, "Resources", "proposta final.pdf");

        string caminhoSaida = Path.Combine(Path.GetDirectoryName(caminhoPdf)!, "Proposta.pdf");

        using (PdfReader reader = new PdfReader(caminhoPdf))
        using (MemoryStream ms = new MemoryStream())
        {
            using (PdfStamper stamper = new PdfStamper(reader, ms))
            {
                var form = stamper.AcroFields;

                var formFields = reader.AcroFields.Fields;

                for (int i = 1; i <= formFields.Count; i++)
                {
                    form.SetFieldProperty($"Text{i}", "bordercolor", BaseColor.WHITE, null);
                    form.SetFieldProperty($"Text{i}", "borderwidth", 0f, null);

                    // Ajusta propriedades do campo existente
                    form.SetFieldProperty($"Text{i}", "textsize", 22f, null);
                    form.SetFieldProperty($"Text{i}", "textcolor", BaseColor.BLACK, null);
                    form.SetFieldProperty($"Text{i}", "font", BaseFont.CreateFont(BaseFont.TIMES_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED), null);
                }

                // Define o valor
                form.SetField("Text1", propostaFinal.RazaoSocial);
                form.SetField("Text2", "R$" + propostaFinal.ValorSemOrigoMensal.ToString("F2"));
                form.SetField("Text3", "R$" + propostaFinal.ValorSemOrigoAnual.ToString("F2"));
                form.SetField("Text4", "R$" + propostaFinal.ValorComOrigoMensal.ToString("F2"));
                form.SetField("Text5", "R$" + propostaFinal.ValorComOrigoAnual.ToString("F2"));
                form.SetField("Text6", "R$" + propostaFinal.EconomiaMensal.ToString("F2"));
                form.SetField("Text7", "R$" + propostaFinal.EconomiaAnual.ToString("F2"));

                // Achata para que o campo vire texto fixo e a caixa desapareça
                stamper.FormFlattening = true;
            }

            File.WriteAllBytes(caminhoSaida, ms.ToArray());
        }
        return Results.Ok();
    }

    public static PropostaDto CalculaProposta(Proposta prop)
    {
        decimal valorSemOrigoMensal;

        if (prop.PlacaCliente == 0)
        {
            valorSemOrigoMensal = (prop.TotalConsumo - ((int)prop.ClasseCliente)) * prop.KwhUnit;
        }
        else
        {
            valorSemOrigoMensal = (decimal)((prop.TotalConsumo - ((int)prop.ClasseCliente) - prop.PlacaCliente) * prop.KwhUnit);
        }

        var descontoMes = (prop.DescontoMes / 100) * valorSemOrigoMensal;

        var valorComOrigoMensal = valorSemOrigoMensal - descontoMes;

        var valorSemOrigoAnual = valorSemOrigoMensal * 12;

        var valorComOrigoAnual = valorComOrigoMensal * 12;

        var economiaMensal = valorSemOrigoMensal - valorComOrigoMensal;

        var economiaAnual = valorSemOrigoAnual - valorComOrigoAnual;

        return new PropostaDto(prop.RazaoSocial, valorSemOrigoMensal, valorComOrigoMensal, valorSemOrigoAnual, valorComOrigoAnual, economiaMensal, economiaAnual);
    }
}
