using System;
using Buzzbox_Common;

namespace Buzzbox_Decode.Decoders
{
    public class MtgEncodeFormatDecoder : IDecoderInterface
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

        public Card DecodeCard(string cardLine)
        {
            throw new NotImplementedException();
        }
    }
}
