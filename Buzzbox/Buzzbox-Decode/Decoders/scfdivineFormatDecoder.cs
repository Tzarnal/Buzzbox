using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Buzzbox_Common;

namespace Buzzbox_Decode.Decoders
{
    public class scfdivineFormatDecoder : IDecoderInterface
    {
        private ConsoleLog _ConsoleLog;

        public scfdivineFormatDecoder()
        {
            _ConsoleLog = ConsoleLog.Instance;
        }

        public Card DecodeCard(string cardLine)
        {
            if (string.IsNullOrWhiteSpace(cardLine))
            {
                return null;
            }

            var splitLine = cardLine.Split('|');
            Card card;

            var cardType = FindCardType(splitLine);

            switch (cardType)
            {
                case "Minion":
                    card = DecodeMinion(splitLine);
                    break;

                case "Spell":
                    card = DecodeSpell(splitLine);
                    break;

                case "Weapon":
                    card = DecodeWeapon(splitLine);
                    break;

                case "Hero":
                    card = DecodeHero(splitLine);
                    break;

                default:
                    _ConsoleLog.VerboseWriteLine("Could not recognize card type.");
                    return null;
            }

            return card;
        }

        private Card DecodeMinion(string[] splitLine)
        {
            var newCard = new Card {Type = "MINION"};
                     
            //Assign Card name
            var nameClass = splitLine[0].Split('@');
            newCard.Name = nameClass[0].Trim();

            //Check for card class and assign, or fail out if not a real class.
            var className = DecodeClass(nameClass[1]);
            if (className != null && className == "Unknown")
            {
                _ConsoleLog.VerboseWriteLine("${nameClass[1]} is not a recognized Class in Hearthstone.");
                return null;
            }

            newCard.PlayerClass = className;

            //Assign card race/tribe
            var race = splitLine[1].Trim().ToUpper();
            if (!string.IsNullOrWhiteSpace(race))
            {
                newCard.Race = race;
            }

            //Assign Rarity
            var rarity = DecodeRarity(splitLine[3]);
            if (rarity == "Unknown")
            {
                _ConsoleLog.VerboseWriteLine($"{splitLine[3]} is not a recognized rarity in Hearthstone.");
                return null;
            }

            newCard.Rarity = rarity;

            //try to parse Manacost
            int manaCost;
            if (!int.TryParse(splitLine[4].Trim(), out manaCost))
            {
                _ConsoleLog.VerboseWriteLine($"{splitLine[4]} is not convertable to a number for manacost.");
                return null;
            }

            //split up attack and health, then try to parse them
            var AttackHealth = splitLine[5].Trim().Split('/');

            int attack;
            if (!int.TryParse(AttackHealth[0], out attack))
            {
                _ConsoleLog.VerboseWriteLine($"{AttackHealth[0]} is not convertable to a number for attack.");
                return null;
            }

            int health;
            if (!int.TryParse(AttackHealth[1], out health))
            {
                _ConsoleLog.VerboseWriteLine($"{AttackHealth[1]} is not convertable to a number for attack.");
                return null;
            }

            //assign all the numbers at once, why not
            newCard.Cost = manaCost;
            newCard.Attack = attack;
            newCard.Health = health;

            var cardText = DecodeText(splitLine[7]);

            newCard.Text = cardText;

            return newCard;
        }

        private Card DecodeHero(string[] splitLine)
        {            
            var newCard = new Card { Type = "HERO" };

            //Assign Card name
            var nameClass = splitLine[0].Split('@');
            newCard.Name = nameClass[0].Trim();

            //Check for card class and assign, or fail out if not a real class.
            var className = DecodeClass(nameClass[1]);
            if (className != null && className == "Unknown")
            {
                _ConsoleLog.VerboseWriteLine("${nameClass[1]} is not a recognized Class in Hearthstone.");
                return null;
            }            
            newCard.PlayerClass = className;
            
            //Assign Rarity
            var rarity = DecodeRarity(splitLine[2]);
            if (rarity == "Unknown")
            {
                _ConsoleLog.VerboseWriteLine($"{splitLine[2]} is not a recognized rarity in Hearthstone.");
                return null;
            }
            
            newCard.Rarity = rarity;

            //try to parse Manacost
            int manaCost;
            if (!int.TryParse(splitLine[3].Trim(), out manaCost))
            {
                _ConsoleLog.VerboseWriteLine($"{splitLine[3]} is not convertable to a number for manacost.");
                return null;
            }            

            int health;
            if (!int.TryParse(splitLine[4].Trim(), out health))
            {
                _ConsoleLog.VerboseWriteLine($"{splitLine[4]} is not convertable to a number for attack.");
                return null;
            }            

            //assign all the numbers at once, why not
            newCard.Cost = manaCost;            
            newCard.Health = health;

            var cardText = DecodeText(splitLine[6]);

            newCard.Text = cardText;
            
            return newCard;
        }

        private Card DecodeSpell(string[] splitLine)
        {
            var newCard = new Card { Type = "SPELL" };

            //Assign Card name
            var nameClass = splitLine[0].Split('@');
            newCard.Name = nameClass[0].Trim();

            //Check for card class and assign, or fail out if not a real class.
            var className = DecodeClass(nameClass[1]);
            if (className != null && className == "Unknown")
            {
               _ConsoleLog.VerboseWriteLine($"{nameClass[1]} is not a regcognized Class in Hearthstone.");
                return null;
            }

            newCard.PlayerClass = className;


            //Assign Rarity
            var rarity = DecodeRarity(splitLine[2]);
            if (rarity == "Unknown")
            {
               _ConsoleLog.VerboseWriteLine($"{splitLine[2]} is not a regcognized rarity in Hearthstone.");
                return null;
            }

            newCard.Rarity = rarity;

            //try to parse Manacost
            int manaCost;
            if (!int.TryParse(splitLine[3].Trim(), out manaCost))
            {
               _ConsoleLog.VerboseWriteLine($"{splitLine[3]} is not convertable to a number for manacost.");
                return null;
            }

            //assign all the numbers at once, why not
            newCard.Cost = manaCost;

            var cardText = DecodeText(splitLine[5]);

            newCard.Text = cardText;

            return newCard;
        }

        private Card DecodeWeapon(string[] splitLine)
        {
            var newCard = new Card { Type = "WEAPON" };

            //Assign Card name
            var nameClass = splitLine[0].Split('@');
            newCard.Name = nameClass[0].Trim();

            //Check for card class and assign, or fail out if not a real class.
            var className = DecodeClass(nameClass[1]);
            if (className != null && className == "Unknown")
            {
               _ConsoleLog.VerboseWriteLine($"{nameClass[1]} is not a regcognized Class in Hearthstone.");
                return null;
            }

            newCard.PlayerClass = className;

            //Assign Rarity
            var rarity = DecodeRarity(splitLine[2]);
            if (rarity == "Unknown")
            {
               _ConsoleLog.VerboseWriteLine($"{splitLine[2]} is not a regcognized rarity in Hearthstone.");
                return null;
            }

            newCard.Rarity = rarity;

            //try to parse Manacost
            int manaCost;
            if (!int.TryParse(splitLine[3].Trim(), out manaCost))
            {
               _ConsoleLog.VerboseWriteLine($"{splitLine[3]} is not convertable to a number for manacost.");
                return null;
            }

            //split up attack and health, then try to parse them
            var AttackHealth = splitLine[4].Trim().Split('/');

            int attack;
            if (!int.TryParse(AttackHealth[0], out attack))
            {
               _ConsoleLog.VerboseWriteLine($"{AttackHealth[0]} is not convertable to a number for attack.");
                return null;
            }

            int health;
            if (!int.TryParse(AttackHealth[1], out health))
            {
               _ConsoleLog.VerboseWriteLine($"{AttackHealth[1]} is not convertable to a number for attack.");
                return null;
            }

            //assign all the numbers at once, why not
            newCard.Cost = manaCost;
            newCard.Attack = attack;
            newCard.Durability = health;

            var cardText = DecodeText(splitLine[6]);

            newCard.Text = cardText;

            return newCard;
        }

        private string DecodeText(string cardText)
        {
            
            if (string.IsNullOrWhiteSpace(cardText))
            {
                return "";
            }

            //clean up the start and end of the string
            cardText = cardText.TrimEnd('&');
            cardText = cardText.Trim();

            //Ensure at least one space between keyword symbols
            cardText = cardText.Replace("$$", "$ $");

            //Replace keywords symbols with markup text
            foreach (KeyValuePair<string, string> replacement in Collections.ReverseKeywordReplacements)
            {
                cardText = cardText.Replace(replacement.Key, replacement.Value);
            }

            //Italicise things between () that is not a single character. Mostly (wherever it is).
            cardText = Regex.Replace(cardText, @"\((..+?)\)", "(<i>$1</i>)");

            return cardText;
        }

        private string DecodeRarity(string rarity)
        {
            rarity = rarity.Trim();

            var legalRarities = new Dictionary<string, string>
            {
                {"C", "COMMON"},
                {"R", "RARE"},
                {"E", "EPIC"},
                {"L", "LEGENDARY"}
            };

            if (legalRarities.ContainsKey(rarity))
            {
                return legalRarities[rarity];
            }

            return "Unknown";
        }

        private string DecodeClass(string className)
        {
            className = className.Trim();
            className = className.ToUpper();

            if (className == "NEUTRAL")
            {
                return null;
            }

            var legalClasses = new[]
            {
                "PALADIN",
                "HUNTER",
                "MAGE",
                "WARRIOR",
                "ROGUE",
                "DRUID",
                "PRIEST",
                "WARLOCK",
                "SHAMAN"

            };

            if (!legalClasses.Contains(className))
            {
                return "Unknown";
            }

            return className;
        }

        private string FindCardType(string[] splitLine)
        {            
            if (splitLine.Count() == 8)
            {
                if (splitLine[2].Trim() == "Minion")
                {
                    return "Minion";
                }
            }

            if (splitLine.Count() == 6)
            {
                if (splitLine[1].Trim() == "Spell")
                {
                    return "Spell";
                }

            }

            if (splitLine.Count() == 7)
            {
                if (splitLine[1].Trim() == "Weapon")
                {
                    return "Weapon";
                }

                if (splitLine[1].Trim() == "Hero")
                {
                    return "Hero";
                }
            }

            //No clue what it is            
            return "Unknown";
        }
    }
}