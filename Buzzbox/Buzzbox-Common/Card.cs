using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace Buzzbox_Common
{
    public class Card
    {
        public int Health { get; set; }
        public int Attack { get; set; }
        public string Artist { get; set; }
        public List<string> Mechanics { get; set; }
        public string Rarity { get; set; }
        public string Race { get; set; }
        public string Set { get; set; }
        public string HowToEarnGolden { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
        public string Texture { get; set; }
        public string Id { get; set; }
        public bool Collectible { get; set; }
        public string Name { get; set; }
        public int Cost { get; set; }
        public string HowToEarn { get; set; }
        public string Flavor { get; set; }
        public List<int?> Dust { get; set; }
        public string PlayerClass { get; set; }
        public int? Durability { get; set; }
        public string Source { get; set; }

        public override string ToString()
        {
            switch (Type)
            {
                case "MINION":
                    return DisplayMinion();
                
                case "SPELL":
                    return DisplaySpell();

                case "WEAPON":
                    return DisplayWeapon();

                default:
                    return base.ToString();
            }            
        }

        private string DisplayMinion()
        {
            var text = Text.RemoveMarkup();
            text = Regex.Replace(text, @"\r\n?|\n", " ");

            return $"{(PlayerClass ?? "Neutral").CapitalizeOnlyFirstLetter()} {(Race ?? "")} Minion: {Name} - {Attack}/{Health} for {Cost} - {text}";
        }

        private string DisplaySpell()
        {
            var text = Text.RemoveMarkup();
            text = Regex.Replace(text, @"\r\n?|\n", " ");

            return $"{(PlayerClass ?? "Neutral").CapitalizeOnlyFirstLetter()} Spell: {Name} - {Cost} mana - {text}";
        }

        private string DisplayWeapon()
        {
            var text = Text.RemoveMarkup();
            text = Regex.Replace(text, @"\r\n?|\n", " ");

            return $"{(PlayerClass ?? "Neutral").CapitalizeOnlyFirstLetter()} Weapon: {Name} - {Attack}/{Durability} for {Cost} - {text}";
        }
    }
    
}
