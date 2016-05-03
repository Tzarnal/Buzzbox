using Microsoft.VisualStudio.TestTools.UnitTesting;
using Buzzbox_Common;

namespace Buzzbox_Decode.Decoders.Tests
{
    [TestClass()]
    public class scfdivineFormatDecoderTests
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
        public void DecodeMinionCardTest()
        {
            var decoder = new scfdivineFormatDecoder();
            var input = "Tirion Fordring @ Paladin |  | Minion | L | 8 | 6/6 || $DV$. $T$. $DR$ Equip a 5/3 Ashbringer. &";

            var output = decoder.DecodeCard(input);

            Assert.AreEqual(testMinion.Name, output.Name);
            Assert.AreEqual(testMinion.Type, output.Type);
            Assert.AreEqual(testMinion.PlayerClass, output.PlayerClass);
            Assert.AreEqual(testMinion.Rarity, output.Rarity);
            Assert.AreEqual(testMinion.Cost, output.Cost);
            Assert.AreEqual(testMinion.Attack, output.Attack);
            Assert.AreEqual(testMinion.Health, output.Health);
            Assert.AreEqual(testMinion.Text, output.Text);

        }

        [TestMethod()]
        public void DecodeSpellCardTest()
        {
            var decoder = new scfdivineFormatDecoder();
            var input = "Fireball @ Mage | Spell | C | 4 || Deal $6 damage. &";

            var output = decoder.DecodeCard(input);

            Assert.AreEqual(testMinion.Name, output.Name);
            Assert.AreEqual(testMinion.Type, output.Type);
            Assert.AreEqual(testMinion.PlayerClass, output.PlayerClass);
            Assert.AreEqual(testMinion.Rarity, output.Rarity);
            Assert.AreEqual(testMinion.Cost, output.Cost);
            Assert.AreEqual(testMinion.Text, output.Text);
        }

        [TestMethod()]
        public void DecodeWeaponCardTest()
        {
            var decoder = new scfdivineFormatDecoder();
            var input = "Death's Bite @ Warrior | Weapon | C | 4 | 4/2 || $DR$ Deal 1 damage to all minions. &";

            var output = decoder.DecodeCard(input);

            Assert.AreEqual(testMinion.Name, output.Name);
            Assert.AreEqual(testMinion.Type, output.Type);
            Assert.AreEqual(testMinion.PlayerClass, output.PlayerClass);
            Assert.AreEqual(testMinion.Rarity, output.Rarity);
            Assert.AreEqual(testMinion.Cost, output.Cost);
            Assert.AreEqual(testMinion.Attack, output.Attack);
            Assert.AreEqual(testMinion.Durability, output.Durability);
            Assert.AreEqual(testMinion.Text, output.Text);
        }
    }
}