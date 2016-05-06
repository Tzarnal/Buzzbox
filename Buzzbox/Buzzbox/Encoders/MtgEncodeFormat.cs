using System;
using System.Collections.Generic;
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
            var encodedCard = string.Format("",
                card.Name,
                card.PlayerClass,
                card.Race,
                card.Type,
                card.Rarity,
                card.Cost,
                card.Attack,
                card.Health,
                card.Text);

            return encodedCard;
        }

        private string EncodeWeapon(Card card)
        {
            throw new NotImplementedException();
        }

        private string EncodeSpell(Card card)
        {
            throw new NotImplementedException();
        }
    }
}
