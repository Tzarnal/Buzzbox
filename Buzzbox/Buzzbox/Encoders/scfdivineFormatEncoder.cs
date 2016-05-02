using System;
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

            var encodedCard = string.Format("{0} @ {1} | {2} | {3} | {4} | {5} | {6}/{7} | {8} &",
                card.Name,
                encodedClass,
                encodedRace,
                encodedType,
                encodedRarity,
                card.Cost,
                card.Attack,
                card.Health,
                card.Text);

            return encodedCard;
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
                
            if (type == "FREE")
            {
                type = "COMMON";
            }

            return type.CapitalizeOnlyFirstLetter();
        }

        private string EncodeCardRarity(string rarity)
        {
            if (string.IsNullOrWhiteSpace(rarity))
                return "";

            return rarity[0].ToString();
        }

        private string EncodeSpell(Card card)
        {
            return "";
        }

        private string EncodeWeapon(Card card)
        {
            return "";
        }
    }
}