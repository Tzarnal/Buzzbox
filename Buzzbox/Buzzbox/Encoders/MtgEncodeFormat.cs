using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Buzzbox_Common;

namespace Buzzbox.Encoders
{
    public class MtgEncodeFormatEncoder : IEncoderInterface
    {
        /*
         * Format Explained: 
         * 
         * Field Labels 
         * 1: Name
         * 2: Text
         * 3: Type ( Minion/Spell/Weapon )
         * 4: Race/Tribe or None
         * 5: Class or Neutral
         * 6: Rarity
         * 7: Mana Cost
         * 8: Attack
         * 9: Health/Durability
         * 
         * 
         * all text in lowercase, type/class/rarity not abreviated
         * numbers replaced with &^^^ format. amount of ^ indicates number
         * fields seperated by | with leading and closing |
         * no additional spaces
         * keywords replaced by $KW$ tokens. tokens remain in upper case.
        */

        public string EncodeCard(Card card)
        {
            if (card == null)
            {
                return string.Empty;
            }

            string output;

            switch (card.Type)
            {
                case "SPELL":
                    output = EncodeSpell(card);
                    break;

                case "WEAPON":
                    output = EncodeWeapon(card);
                    break;

                case "MINION":
                    output = EncodeMinion(card);
                    break;

                default:
                    output = EncodeSpell(card);
                    break;
            }

            return output;
        }

        private string EncodeMinion(Card card)
        {
            var encodedCard = string.Format("|3{3}|4{1}|5{2}|6{4}|7{5}|8{6}|9{7}|2{8}|1{0}|",
                card.Name.ToLower(),
                (card.PlayerClass ?? "neutral").ToLower(),
                (card.Race ?? "none").ToLower(),
                card.Type.ToLower(),
                EncodeCardRarity(card.Rarity),
                EncodeNumbers(card.Cost),
                EncodeNumbers(card.Attack),
                EncodeNumbers(card.Health),
                EncodeCardText(card.Text));

            return encodedCard;
        }

        private string EncodeWeapon(Card card)
        {
            var encodedCard = string.Format("|3{2}|4{1}|6{3}|7{4}|8{5}|9{6}|2{7}|1{0}|",
                card.Name.ToLower(),
                (card.PlayerClass ?? "neutral").ToLower(),
                card.Type.ToLower(),
                EncodeCardRarity(card.Rarity),
                EncodeNumbers(card.Cost),
                EncodeNumbers(card.Attack),
                EncodeNumbers(card.Durability),
                EncodeCardText(card.Text));

            return encodedCard;
        }

        private string EncodeSpell(Card card)
        {
            throw new NotImplementedException();
        }

        //Processes a string replacing all numbers with encoded number markup like &^^^
        private string EncodeNumbers(string input)
        {
            var numberMatches = Regex.Matches(input, @"\b[0-9]+\b");

            foreach (Match numberMatch in numberMatches)
            {
                var numberString = numberMatch.Value;
                int number;

                if (int.TryParse(numberString, out number))
                {
                    var replacement = EncodeNumbers(number);
                    input = input.Replace(numberString, replacement);
                }
            }

            return input;
        }

        private string EncodeNumbers(int? input)
        {
            return "&" + new string('^', input ?? 0);
        }

        private string EncodeCardRarity(string rarity)
        {
            if (string.IsNullOrWhiteSpace(rarity))
                return "";

            if (rarity == "FREE")
            {
                rarity = "COMMON";
            }

            return rarity.ToLower();
        }

        private string EncodeCardText(string cardText)
        {
            if (string.IsNullOrWhiteSpace(cardText))
            {
                return "";
            }

            var output = cardText.RemoveMarkup().ToLower();

            //Also strip newlines.
            output = Regex.Replace(output, @"\r\n?|\n", " ");

            //Replace keywords with shorter symbols.
            foreach (KeyValuePair<string, string> replacement in Collections.KeywordReplacements)
            {
                output = output.Replace(replacement.Key.ToLower(), replacement.Value);
            }
            
            return EncodeNumbers(output);
        }
    }
}
