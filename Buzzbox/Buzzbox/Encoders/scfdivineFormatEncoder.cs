using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Buzzbox_Common;

namespace Buzzbox.Encoders
{
    public class scfdivineFormatEncoder : IEncoderInterface
    {
        public string EncodeCard(Card card)
        {
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
                default:
                    output = EncodeMinion(card);
                    break;
            }

            return output;
        }

        private string EncodeMinion(Card card)
        {
            var encodedClass = EncodeCardClass(card.PlayerClass);
            var encodedRace = EncodeCardRace(card.Race);
            var encodedType = EncodeCardType(card.Type);
            var encodedRarity = EncodeCardRarity(card.Rarity);
            var encodedText = EncodeCardText(card.Text);

            var encodedCard = string.Format("{0} @ {1} | {2} | {3} | {4} | {5} | {6}/{7} || {8} &",
                card.Name,
                encodedClass,
                encodedRace,
                encodedType,
                encodedRarity,
                card.Cost,
                card.Attack,
                card.Health,
                encodedText);

            return encodedCard;
        }

        private string EncodeWeapon(Card card)
        {
            var encodedClass = EncodeCardClass(card.PlayerClass);            
            var encodedType = EncodeCardType(card.Type);
            var encodedRarity = EncodeCardRarity(card.Rarity);
            var encodedText = EncodeCardText(card.Text);

            var encodedCard = string.Format("{0} @ {1} | {2} | {3} | {4} | {5}/{6} || {7} &",
                card.Name,
                encodedClass,
                encodedType,
                encodedRarity,
                card.Cost,
                card.Attack,
                card.Durability,
                encodedText);

            return encodedCard;
        }

        private string EncodeSpell(Card card)
        {
            var encodedClass = EncodeCardClass(card.PlayerClass);
            var encodedType = EncodeCardType(card.Type);
            var encodedRarity = EncodeCardRarity(card.Rarity);
            var encodedText = EncodeCardText(card.Text);

            var encodedCard = string.Format("{0} @ {1} | {2} | {3} | {4} || {5} &",
                card.Name,
                encodedClass,                
                encodedType,
                encodedRarity,
                card.Cost,
                encodedText);

            return encodedCard;
        }

        private string EncodeCardText(string cardText)
        {
            if (string.IsNullOrWhiteSpace(cardText))
            {
                return "";
            }

            var output = cardText.RemoveMarkup();

            //Also strip newlines.
            output = Regex.Replace(output, @"\r\n?|\n", "");

            //Get rid of : as well, it'll be put back in ( for 90 of cases ) at decode when keywords are put back in
            output = output.Replace(":", string.Empty);

            //Replace keywords with shorter symbols.
            foreach (KeyValuePair<string, string> replacement in Collections.KeywordReplacements)
            {
                output = output.Replace(replacement.Key, replacement.Value);
            }

            return output;
        }


        private string EncodeCardRace(string race)
        {
            if (string.IsNullOrWhiteSpace(race))
            {
                return "";
            }

            return race.CapitalizeOnlyFirstLetter();
        }

        private string EncodeCardClass(string cardClass)
        {
            if (string.IsNullOrWhiteSpace(cardClass))
            {
                return "Neutral";
            }

            return cardClass.CapitalizeOnlyFirstLetter();
        }

        private string EncodeCardType(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                return "";
            }

            return type.CapitalizeOnlyFirstLetter();
        }

        private string EncodeCardRarity(string rarity)
        {
            if (string.IsNullOrWhiteSpace(rarity))
                return "";

            if (rarity == "FREE")
            {
                rarity = "COMMON";
            }

            return rarity[0].ToString();
        }

    }
}