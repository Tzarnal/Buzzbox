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

        [TestMethod()]
        public void EncodeMinionCardTest()
        {
            var encoder = new scfdivineFormatEncoder();

            var output = encoder.EncodeCard(testMinion);
            var expectedOutput =
                "Tirion Fordring @ Paladin |  | Minion | L | 8 | 6/6 || $DV$. $T$. $DR$ Equip a 5/3 Ashbringer. &";

            Assert.AreEqual(expectedOutput, output);
        }
    }
}