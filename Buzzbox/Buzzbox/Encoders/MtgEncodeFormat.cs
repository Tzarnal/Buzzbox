using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
         * 4: Class / Neutral
         * 5: Rarity
         * 6: Mana Cost
         * 7: Attack
         * 8: Health/Durability
         * 
         * all text in lowercase, type/class/rarity not abreviated
         * numbers replaced with &^^^ format. amount of ^ indicates number
         * fields seperated by | with leading and closing |
         * no additional spaces
         * keywords replaced by $KW$ tokens. tokens remain in upper case.
        */

        public string EncodeCard(Card card)
        {
            throw new NotImplementedException();
        }
    }
}
