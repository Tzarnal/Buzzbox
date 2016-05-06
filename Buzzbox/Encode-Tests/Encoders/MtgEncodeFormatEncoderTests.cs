using Microsoft.VisualStudio.TestTools.UnitTesting;
using Buzzbox_Common;

namespace Buzzbox.Encoders.Tests
{
    [TestClass()]
    public class MtgEncodeFormatEncoderTests
    {
        Card testMinion = new Card
        {
            Name = "Tirion Fordring",
            Type = "MINION",
            PlayerClass = "PALADIN",
            Rarity = "LEGENDARY",
            Cost = 8,
            Attack = 6,
            Health = 6,
            Text = "<b>Divine Shield</b>. <b>Taunt</b>. <b>Deathrattle:</b> Equip a 5/3 Ashbringer."
        };

        Card testSpell = new Card
        {
            Name = "Fireball",
            Type = "SPELL",
            PlayerClass = "MAGE",
            Rarity = "FREE",
            Cost = 4,
            Text = "Deal $6 damage."
        };

        Card testWeapon = new Card
        {
            Name = "Death's Bite",
            Type = "WEAPON",
            PlayerClass = "WARRIOR",
            Rarity = "COMMON",
            Cost = 4,
            Attack = 4,
            Durability = 2,
            Text = "<b>Deathrattle:</b> Deal 1 damage to all minions."
        };

        [TestMethod()]
        public void EncodeMinionCardTest()
        {
            var encoder = new MtgEncodeFormatEncoder();

            var output = encoder.EncodeCard(testMinion);
            var expectedOutput =
                "|3minion|4paladin|5none|6legendary|7&^^^^^^^^|8&^^^^^^|9&^^^^^^|2$DV$. $T$. $DR$: equip a &^^^^^/&^^^ ashbringer.|1tirion fordring|";
            Assert.AreEqual(expectedOutput, output);
        }

        [TestMethod()]
        public void EncodeSpellCardTest()
        {
            var encoder = new MtgEncodeFormatEncoder();

            var output = encoder.EncodeCard(testSpell);
            var expectedOutput =
                "|3spell|4mage|6common|7&^^^^|2deal $6 damage.|1fireball|";            

            Assert.AreEqual(expectedOutput, output);
        }


        [TestMethod()]
        public void EncodeWeaponCardTest()
        {
            var encoder = new MtgEncodeFormatEncoder();

            var output = encoder.EncodeCard(testWeapon);
            var expectedOutput =
                "|3weapon|4warrior|6common|7&^^^^|8&^^^^|9&^^|2$DR$: deal 1 damage to all minions|1death's bite|";            

            Assert.AreEqual(expectedOutput, output);
        }
    }
}