using System.Globalization;
using System.Text;
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
                    form.SetFieldProperty($"Text{i}", "textsize", 21f, null);
                    form.SetFieldProperty($"Text{i}", "textcolor", BaseColor.BLACK, null);
                    form.SetFieldProperty($"Text{i}", "font", BaseFont.CreateFont(BaseFont.TIMES_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED), null);
                }

                // Define o valor
                form.SetField("Text1", RemoverAcentos(propostaFinal.RazaoSocial));
                form.SetField("Text2", propostaFinal.ValorSemOrigoMensal.ToString("C", new CultureInfo("pt-BR")));
                form.SetField("Text3", propostaFinal.ValorSemOrigoAnual.ToString("C", new CultureInfo("pt-BR")));
                form.SetField("Text4", propostaFinal.ValorComOrigoMensal.ToString("C", new CultureInfo("pt-BR")));
                form.SetField("Text5", propostaFinal.ValorComOrigoAnual.ToString("C", new CultureInfo("pt-BR")));
                form.SetField("Text6", propostaFinal.EconomiaMensal.ToString("C", new CultureInfo("pt-BR")));
                form.SetField("Text7", propostaFinal.EconomiaAnual.ToString("C", new CultureInfo("pt-BR")));

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

        string totalString = prop.TotalConsumo.ToString(CultureInfo.InvariantCulture);
        decimal total = decimal.Parse(totalString);

        if (prop.PlacaCliente == 0)
        {
            valorSemOrigoMensal = (total - (int)prop.ClasseCliente) * prop.KwhUnit;
        }
        else
        {
            string placaString = prop.PlacaCliente.ToString(CultureInfo.InvariantCulture);
            decimal placa = decimal.Parse(placaString);

            valorSemOrigoMensal = (total - (int)prop.ClasseCliente - placa) * prop.KwhUnit;
        }

        var descontoMes = (prop.DescontoMes / 100) * valorSemOrigoMensal;

        var valorComOrigoMensal = valorSemOrigoMensal - descontoMes;

        var valorSemOrigoAnual = valorSemOrigoMensal * 12;

        var valorComOrigoAnual = valorComOrigoMensal * 12;

        var economiaMensal = valorSemOrigoMensal - valorComOrigoMensal;

        var economiaAnual = valorSemOrigoAnual - valorComOrigoAnual;

        return new PropostaDto(prop.RazaoSocial, valorSemOrigoMensal, valorComOrigoMensal, valorSemOrigoAnual, valorComOrigoAnual, economiaMensal, economiaAnual);
    }

    public static string RemoverAcentos(string texto)
    {
        if (string.IsNullOrEmpty(texto)) return texto;

        var normalized = texto.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();

        foreach (var c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        return sb.ToString().Normalize(NormalizationForm.FormC);
    }
}
