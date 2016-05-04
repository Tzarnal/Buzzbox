using Microsoft.VisualStudio.TestTools.UnitTesting;
using Buzzbox_Common;

namespace Buzzbox_Decode.Decoders.Tests
{
    [TestClass()]
    public class scfdivineFormatDecoderTests
    {
        private Card testMinion = new Card
        {
            Name = "Tirion Fordring",
            Type = "MINION",
            PlayerClass = "PALADIN",
            Rarity = "LEGENDARY",
            Cost = 8,
            Attack = 6,
            Health = 6,
            Text = "<b>Divine Shield</b>. <b>Taunt</b>. <b>Deathrattle</b>: Equip a 5/3 Ashbringer."
        };

        private Card testSpell = new Card
        {
            Name = "Fireball",
            Type = "SPELL",
            PlayerClass = "MAGE",
            Rarity = "COMMON",
            Cost = 4,
            Text = "Deal $6 damage."
        };

        private Card testWeapon = new Card
        {
            Name = "Death's Bite",
            Type = "WEAPON",
            PlayerClass = "WARRIOR",
            Rarity = "COMMON",
            Cost = 4,
            Attack = 4,
            Durability = 2,
            Text = "<b>Deathrattle</b>: Deal 1 damage to all minions."
        };

        [TestMethod()]
        public void DecodeMinionCardTest()
        {
            var decoder = new scfdivineFormatDecoder();
            var input =
                "Tirion Fordring @ Paladin |  | Minion | L | 8 | 6/6 || $DV$. $T$. $DR$: Equip a 5/3 Ashbringer. &";

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

            Assert.AreEqual(testSpell.Name, output.Name);
            Assert.AreEqual(testSpell.Type, output.Type);
            Assert.AreEqual(testSpell.PlayerClass, output.PlayerClass);
            Assert.AreEqual(testSpell.Rarity, output.Rarity);
            Assert.AreEqual(testSpell.Cost, output.Cost);
            Assert.AreEqual(testSpell.Text, output.Text);
        }

        [TestMethod()]
        public void DecodeWeaponCardTest()
        {
            var decoder = new scfdivineFormatDecoder();
            var input = "Death's Bite @ Warrior | Weapon | C | 4 | 4/2 || $DR$: Deal 1 damage to all minions. &";

            var output = decoder.DecodeCard(input);

            Assert.AreEqual(testWeapon.Name, output.Name);
            Assert.AreEqual(testWeapon.Type, output.Type);
            Assert.AreEqual(testWeapon.PlayerClass, output.PlayerClass);
            Assert.AreEqual(testWeapon.Rarity, output.Rarity);
            Assert.AreEqual(testWeapon.Cost, output.Cost);
            Assert.AreEqual(testWeapon.Attack, output.Attack);
            Assert.AreEqual(testWeapon.Durability, output.Durability);
            Assert.AreEqual(testWeapon.Text, output.Text);
        }
    }
}
