using Microsoft.VisualStudio.TestTools.UnitTesting;
using Buzzbox_Common;
using System;

namespace Buzzbox_Decode.Tests
{
    [TestClass()]
    public class DecodeTests
    {
        
        [TestMethod()]
        public void DecodeStringscfdivineFormatTest()
        {
            var decoder = new Decode();
            var result = decoder.DecodeString("Tirion Fordring @ Paladin |  | Minion | L | 8 | 6/6 || $DV$. $T$. $DR$ Equip a 5/3 Ashbringer. &\n\nFireball @ Mage | Spell | C | 4 || Deal $6 damage. &\n\nDeath's Bite @ Warrior | Weapon | C | 4 | 4/2 || $DR$ Deal 1 damage to all minions. &\n\n",EncodingFormats.scfdivineFormat);

            var expected = "Paladin - Minion: Tirion Fordring - 6/6 for 8 - Divine Shield. Taunt. Deathrattle Equip a 5/3 Ashbringer.\r\nMage Spell: Fireball - 4 mana - Deal $6 damage.\r\nWarrior Weapon: Death's Bite - 4/2 for 4 - Deathrattle Deal 1 damage to all minions.\r\n";

            var resultString = "";
            foreach (var card in result.Cards)
            {
                resultString += card.ToString() + Environment.NewLine;
            }

            Assert.AreEqual(expected, resultString);
        }

        [TestMethod()]
        public void DecodeStringMtgEncodeFormatTest()
        {
            var decoder = new Decode();
            var result = decoder.DecodeString("|3minion|4paladin|5none|6legendary|7&^^^^^^^^|8&^^^^^^|9&^^^^^^|2$DV$. $T$. $DR$: equip a &^^^^^/&^^^ ashbringer.|1tirion fordring|\n\n|3spell|4mage|6common|7&^^^^|2deal $&^^^^^^ damage.|1fireball|\n\n|3weapon|4warrior|6common|7&^^^^|8&^^^^|9&^^|2$DR$: deal &^ damage to all minions.|1death's bite|\n\n", EncodingFormats.MtgEncoderFormat);

            var expected = "Paladin - Minion: Tirion Fordring - 6/6 for 8 - Divine Shield. Taunt. Deathrattle: Equip a 5/3 ashbringer.\r\nMage Spell: Fireball - 4 mana - Deal $6 damage.\r\nWarrior Weapon: Death's Bite - 4/2 for 4 - Deathrattle: Deal 1 damage to all minions.\r\n";

            var resultString = "";
            foreach (var card in result.Cards)
            {
                resultString += card.ToString() + Environment.NewLine;
            }

            Assert.AreEqual(expected, resultString);
        }
    }
}