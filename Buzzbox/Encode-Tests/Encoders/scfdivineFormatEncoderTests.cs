using Microsoft.VisualStudio.TestTools.UnitTesting;
using Buzzbox_Common;

namespace Buzzbox.Encoders.Tests
{
    [TestClass()]
    public class scfdivineFormatEncoderTests
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
            var encoder = new scfdivineFormatEncoder();

            var output = encoder.EncodeCard(testMinion);
            var expectedOutput =
                "Tirion Fordring @ Paladin |  | Minion | L | 8 | 6/6 || $DV$. $T$. $DR$: Equip a 5/3 Ashbringer. &";

            Assert.AreEqual(expectedOutput, output);
        }

        [TestMethod()]
        public void EncodeSpellCardTest()
        {
            var encoder = new scfdivineFormatEncoder();

            var output = encoder.EncodeCard(testSpell);
            var expectedOutput =
                "Fireball @ Mage | Spell | C | 4 || Deal $6 damage. &";

            Assert.AreEqual(expectedOutput, output);
        }


        [TestMethod()]
        public void EncodeWeaponCardTest()
        {
            var encoder = new scfdivineFormatEncoder();

            var output = encoder.EncodeCard(testWeapon);
            var expectedOutput =
                "Death's Bite @ Warrior | Weapon | C | 4 | 4/2 || $DR$: Deal 1 damage to all minions. &";

            Assert.AreEqual(expectedOutput, output);
        }
    }
}