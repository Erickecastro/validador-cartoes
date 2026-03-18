using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace ValidadorCartoes
{
    public static class CartaoValidator
    {
        public static string IdentificarBandeira(string numeroCartao)
        {
            if (string.IsNullOrWhiteSpace(numeroCartao))
                return "Número inválido";

            string numero = LimparNumero(numeroCartao);

            if (!Regex.IsMatch(numero, @"^\d+$"))
                return "Número inválido";

            if (!ValidarLuhn(numero))
                return "Cartão inválido";

            if (EhVisaElectron(numero)) return "Visa Electron";
            if (EhVisa(numero)) return "Visa";
            if (EhMastercard(numero)) return "Mastercard";
            if (EhAmericanExpress(numero)) return "American Express";
            if (EhDinersClub(numero)) return "Diners Club";
            if (EhEnRoute(numero)) return "enRoute";
            if (EhDiscover(numero)) return "Discover";
            if (EhJcb(numero)) return "JCB";
            if (EhMaestro(numero)) return "Maestro";
            if (EhSolo(numero)) return "Solo";
            if (EhSwitch(numero)) return "Switch";
            if (EhLaser(numero)) return "Laser";

            return "Bandeira não identificada";
        }

        public static bool ValidarCartao(string numeroCartao)
        {
            if (string.IsNullOrWhiteSpace(numeroCartao))
                return false;

            string numero = LimparNumero(numeroCartao);

            if (!Regex.IsMatch(numero, @"^\d+$"))
                return false;

            return ValidarLuhn(numero);
        }

        private static string LimparNumero(string numero)
        {
            return Regex.Replace(numero, @"[\s-]", "");
        }

        private static bool ValidarLuhn(string numero)
        {
            int soma = 0;
            bool dobrar = false;

            for (int i = numero.Length - 1; i >= 0; i--)
            {
                int digito = numero[i] - '0';

                if (dobrar)
                {
                    digito *= 2;
                    if (digito > 9)
                        digito -= 9;
                }

                soma += digito;
                dobrar = !dobrar;
            }

            return soma % 10 == 0;
        }

        private static bool ComecaCom(string numero, params string[] prefixos)
        {
            return prefixos.Any(numero.StartsWith);
        }

        private static bool TamanhoEntre(string numero, params int[] tamanhos)
        {
            return tamanhos.Contains(numero.Length);
        }

        private static int PrefixoInt(string numero, int quantidade)
        {
            if (numero.Length < quantidade)
                return -1;

            return int.Parse(numero.Substring(0, quantidade));
        }

        private static bool EhVisa(string numero)
        {
            return numero.StartsWith("4") && TamanhoEntre(numero, 13, 16, 19);
        }

        private static bool EhVisaElectron(string numero)
        {
            string[] prefixos = { "4026", "417500", "4508", "4844", "4913", "4917" };
            return ComecaCom(numero, prefixos) && TamanhoEntre(numero, 16);
        }

        private static bool EhMastercard(string numero)
        {
            int p2 = PrefixoInt(numero, 2);
            int p4 = PrefixoInt(numero, 4);

            bool faixaAntiga = p2 >= 51 && p2 <= 55;
            bool faixaNova = p4 >= 2221 && p4 <= 2720;

            return TamanhoEntre(numero, 16) && (faixaAntiga || faixaNova);
        }

        private static bool EhAmericanExpress(string numero)
        {
            return ComecaCom(numero, "34", "37") && TamanhoEntre(numero, 15);
        }

        private static bool EhDinersClub(string numero)
        {
            int p3 = PrefixoInt(numero, 3);
            int p2 = PrefixoInt(numero, 2);

            bool faixa1 = p3 >= 300 && p3 <= 305;
            bool faixa2 = ComecaCom(numero, "36", "38", "39");

            return TamanhoEntre(numero, 14) && (faixa1 || faixa2);
        }

        private static bool EhEnRoute(string numero)
        {
            return ComecaCom(numero, "2014", "2149") && TamanhoEntre(numero, 15);
        }

        private static bool EhDiscover(string numero)
        {
            int p3 = PrefixoInt(numero, 3);
            int p6 = PrefixoInt(numero, 6);

            bool prefixoFixo = ComecaCom(numero, "6011", "65", "644", "645", "646", "647", "648", "649");
            bool faixa622 = p6 >= 622126 && p6 <= 622925;

            return TamanhoEntre(numero, 16, 19) && (prefixoFixo || faixa622);
        }

        private static bool EhJcb(string numero)
        {
            int p4 = PrefixoInt(numero, 4);
            return p4 >= 3528 && p4 <= 3589 && TamanhoEntre(numero, 16, 17, 18, 19);
        }

        private static bool EhMaestro(string numero)
        {
            string[] prefixos =
            {
                "5018", "5020", "5038", "5893", "6304", "6759", "6761", "6762"
            };

            return ComecaCom(numero, prefixos) && numero.Length >= 12 && numero.Length <= 19;
        }

        private static bool EhSolo(string numero)
        {
            return ComecaCom(numero, "6334", "6767") && TamanhoEntre(numero, 16, 18, 19);
        }

        private static bool EhSwitch(string numero)
        {
            string[] prefixos =
            {
                "4903", "4905", "4911", "4936",
                "564182", "633110", "6333", "6759"
            };

            return ComecaCom(numero, prefixos) && TamanhoEntre(numero, 16, 18, 19);
        }

        private static bool EhLaser(string numero)
        {
            return ComecaCom(numero, "6304", "6706", "6771", "6709") && numero.Length >= 16 && numero.Length <= 19;
        }
    }

    class Program
    {
        static void Main()
        {
            Console.Write("Digite o número do cartão: ");
            string numero = Console.ReadLine();

            bool valido = CartaoValidator.ValidarCartao(numero);
            string bandeira = CartaoValidator.IdentificarBandeira(numero);

            Console.WriteLine($"\nVálido: {valido}");
            Console.WriteLine($"Bandeira: {bandeira}");
        }
    }
}