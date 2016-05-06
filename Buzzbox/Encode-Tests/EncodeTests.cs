using Microsoft.VisualStudio.TestTools.UnitTesting;
using Buzzbox_Common;

namespace Buzzbox.Tests
{
    [TestClass()]
    public class EncodeTests
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
        public void EncodeCardCollectionscfdivineFormatTest()
        {
            var collection = new CardCollection();
            collection.Cards.Add(testMinion);
            collection.Cards.Add(testSpell);
            collection.Cards.Add(testWeapon);

            var encoder = new Encode();
            var result = encoder.EncodeCardCollection(collection, EncodingFormats.scfdivineFormat);

            var expected =
                "Tirion Fordring @ Paladin |  | Minion | L | 8 | 6/6 || $DV$. $T$. $DR$ Equip a 5/3 Ashbringer. &\n\nFireball @ Mage | Spell | C | 4 || Deal $6 damage. &\n\nDeath's Bite @ Warrior | Weapon | C | 4 | 4/2 || $DR$: Deal 1 damage to all minions. &\n\n";

            Assert.AreEqual(expected,result);
        }

        [TestMethod()]
        public void EncodeCardCollectionMtgEncodeFormatTest()
        {
            var collection = new CardCollection();
            collection.Cards.Add(testMinion);
            collection.Cards.Add(testSpell);
            collection.Cards.Add(testWeapon);

            var encoder = new Encode();
            var result = encoder.EncodeCardCollection(collection, EncodingFormats.scfdivineFormat);

            var expected =
                "|3minion|4paladin|5legendary|6&^^^^^^^^|7&^^^^^^|8&^^^^^^|2$DV$. $T$. $DR$: equip a &^^^^^/&^^ ashbringer.|1tirion fordring|\n\n|3spell|4mage|5common|6&^^^^|2deal $6 damage.|1fireball|\n\n|3weapon|4warrior|5common|6&^^^^|7&^^^^|8&^^|2$DR$: deal 1 damage to all minions|1death's bite|\n\n";

            Assert.AreEqual(expected, result);
        }
    }
}