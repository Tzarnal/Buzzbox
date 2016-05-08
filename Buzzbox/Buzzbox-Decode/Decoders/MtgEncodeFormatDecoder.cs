using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Buzzbox_Common;
using System.Globalization;
using System.Linq;

namespace Buzzbox_Decode.Decoders
{
    public class MtgEncodeFormatDecoder : IDecoderInterface
    {
        /*
         * Format Explained: 
         * 
         * Field Labels 
         * 1: Name
         * 2: Text
         * 3: Type ( Minion/Spell/Weapon )
         * 4: Class or Neutral
         * 5: Race/Tribe or None
         * 6: Rarity
         * 7: Mana Cost
         * 8: Attack
         * 9: Health/Durability
         * 
         * 
         * all text in lowercase, type/class/rarity not abreviated
         * numbers replaced with &^^^ format. amount of ^ indicates number
         * fields seperated by | with leading and closing |
         * no additional spaces
         * keywords replaced by $KW$ tokens. tokens remain in upper case.
         */

        private TextInfo _textInfo;

        public MtgEncodeFormatDecoder()
        {
            _textInfo = new CultureInfo("en-US", false).TextInfo;
        }

        public Card DecodeCard(string cardLine)
        {
            if (string.IsNullOrWhiteSpace(cardLine))
            {
                return null;
            }

            Card card;
            var cardType = FindField(cardLine, 3);

            if (string.IsNullOrWhiteSpace(cardType))
            {
                Console.WriteLine("Could not find the card type.");
            }

            switch (cardType)
            {
                case "minion":
                    card = DecodeMinion(cardLine);
                    break;

                case "spell":
                    card = DecodeSpell(cardLine);
                    break;

                case "weapon":
                    card = DecodeWeapon(cardLine);
                    break;

                default:
                    return null;
            }

            return card;
        }

        private Card DecodeMinion(string cardLine)
        {
            var newCard = new Card { Type = "MINION" };

            //Assign card class
            var cardClass = DecodeClass(cardLine);
            if (cardClass == "Unknown")
            {                
                return null;
            }
            newCard.PlayerClass = cardClass;

            //Assign card rarity
            var cardRarity = DecodeRarity(cardLine);
            if (cardRarity == "Unknown")
            {
                return null;
            }
            newCard.Rarity = cardRarity;

            //Assign card race/tribe
            var cardRace = FindField(cardLine, 5);
            if (string.IsNullOrWhiteSpace(cardRace))
            {
                Console.WriteLine("Could not find a field for Race/Tribe.");
                return null;
            }
            if (cardRace != "none")
            {
                newCard.Race = cardRace.ToUpper();
            }

            //Assign card name
            var cardName = DecodeName(cardLine);
            if (cardName == "Unknown")
            {
                return null;
            }
            newCard.Name = cardName;

            var cardCost = DecodeNumberField(cardLine, 7);
            if (cardCost == null)
            {
                Console.WriteLine("Could not convert Cost field to a number.");
                return null;
            }
            newCard.Cost = (int) cardCost;

            var cardAttack = DecodeNumberField(cardLine, 8);
            if (cardAttack == null)
            {
                Console.WriteLine("Could not convert Attack field to a number.");
                return null;
            }
            newCard.Attack = (int)cardAttack;

            var cardHealth = DecodeNumberField(cardLine, 9);
            if (cardHealth == null)
            {
                Console.WriteLine("Could not convert Health field to a number.");
                return null;
            }
            newCard.Health = (int)cardHealth;

            var cardText = DecodeText(cardLine);
            if (cardClass == "Unknown")
            {
                return null;
            }
            newCard.Text = cardText;

            return newCard;
        }

        private Card DecodeSpell(string cardLine)
        {
            var newCard = new Card { Type = "SPELL" };

            //Assign card class
            var cardClass = DecodeClass(cardLine);
            if (cardClass == "Unknown")
            {
                return null;
            }
            newCard.PlayerClass = cardClass;

            //Assign card rarity
            var cardRarity = DecodeRarity(cardLine);
            if (cardRarity == "Unknown")
            {
                return null;
            }
            newCard.Rarity = cardRarity;

            //Assign card name
            var cardName = DecodeName(cardLine);
            if (cardName == "Unknown")
            {
                return null;
            }
            newCard.Name = cardName;

            var cardCost = DecodeNumberField(cardLine, 7);
            if (cardCost == null)
            {
                Console.WriteLine("Could not convert Cost field to a number.");
                return null;
            }
            newCard.Cost = (int)cardCost;

            var cardText = DecodeText(cardLine);
            if (cardText == "Unknown")
            {
                return null;
            }
            newCard.Text = cardText;

            return newCard;
        }
        
        private Card DecodeWeapon(string cardLine)
        {
            var newCard = new Card { Type = "WEAPON" };

            //Assign card class
            var cardClass = DecodeClass(cardLine);
            if (cardClass == "Unknown")
            {
                return null;
            }
            newCard.PlayerClass = cardClass;

            //Assign card rarity
            var cardRarity = DecodeRarity(cardLine);
            if (cardRarity == "Unknown")
            {
                return null;
            }
            newCard.Rarity = cardRarity;


            //Assign card name
            var cardName = DecodeName(cardLine);
            if (cardName == "Unknown")
            {
                return null;
            }
            newCard.Name = cardName;

            var cardCost = DecodeNumberField(cardLine, 7);
            if (cardCost == null)
            {
                Console.WriteLine("Could not convert Cost field to a number.");
                return null;
            }
            newCard.Cost = (int)cardCost;

            var cardAttack = DecodeNumberField(cardLine, 8);
            if (cardAttack == null)
            {
                Console.WriteLine("Could not convert Attack field to a number.");
                return null;
            }
            newCard.Attack = (int)cardAttack;

            var cardHealth = DecodeNumberField(cardLine, 9);
            if (cardHealth == null)
            {
                Console.WriteLine("Could not convert Health field to a number.");
                return null;
            }
            newCard.Durability = (int)cardHealth;

            var cardText = DecodeText(cardLine);
            if (cardText == "Unknown")
            {
                return null;
            }
            newCard.Text = cardText;

            return newCard;
        }

        private string DecodeText(string cardLine)
        {
            var cardText = FindField(cardLine, 2);
            if (cardText == null)
            {
                Console.WriteLine("Could not find a field for card text.");
                return "Unknown";
            }

            if (string.IsNullOrWhiteSpace(cardText))
            {
                return "";
            }

            cardText = cardText.Trim();

            //Ensure at least one space between keyword symbols
            cardText = cardText.Replace("$$", "$ $");

            cardText = Paragraph.ToSentenceCase(cardText);
            cardText = DecodeNumbers(cardText);
            
            //Replace keywords symbols with markup text
            foreach (KeyValuePair<string, string> replacement in Collections.ReverseKeywordReplacements)
            {
                cardText = cardText.Replace(replacement.Key, replacement.Value);
            }

            return cardText;
        }

        private string DecodeName(string cardLine)
        {
            var cardName = FindField(cardLine, 1);
            if (string.IsNullOrWhiteSpace(cardName))
            {
                Console.WriteLine("Could not find a field for name.");
                return "Unknown";
            }

            return _textInfo.ToTitleCase(cardName);
        }

        private string DecodeRarity(string cardLine)
        {
            var cardRarity = FindField(cardLine, 6);
            if (string.IsNullOrWhiteSpace(cardRarity))
            {
                Console.WriteLine("Could not find a field for rarity.");
                return "Unknown";
            }

            cardRarity = cardRarity.ToUpper();

            var legalRarities = new[] 
            {
                "COMMON",
                "RARE",
                "EPIC",
                "LEGENDARY"
            };

            if (legalRarities.Contains(cardRarity))
            {
                return cardRarity;
            }

            return "Unknown";
        }

        private string DecodeClass(string cardLine)
        {
            var cardClass = FindField(cardLine, 4);
            if (string.IsNullOrWhiteSpace(cardClass))
            {
                Console.WriteLine("Could not find a field for class.");
                return "Unknown";
            }

            cardClass = cardClass.ToUpper();

            if (cardClass == "NEUTRAL")
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

            if (!legalClasses.Contains(cardClass))
            {
                Console.WriteLine("{0} is not a recognized class in Hearthstone.", cardClass);
                return "Unknown";
            }

            return cardClass;
        }

       
        private string DecodeNumbers(string input)
        {
            var symbolMatches = Regex.Matches(input, @"&(\^*)");

            var numberSymbolsToReplace = new Dictionary<string, int>();

            foreach (Match symbolMatch in symbolMatches)
            {
                var symbolString = symbolMatch.Value;
                var count = symbolString.Length - 1;

                if(!numberSymbolsToReplace.ContainsKey(symbolString))
                    numberSymbolsToReplace.Add(symbolString, count);                    
            }

            //order by the longest first, this should avoid dangeling ^^^'s
            var sortedSymbols = from entry in numberSymbolsToReplace orderby entry.Value descending select entry;

            foreach (var entry in sortedSymbols)
            {
                
                input = input.Replace(entry.Key,  entry.Value.ToString() );
            }

            return input;
        }

        private int? DecodeNumberField(string cardLine, int fieldNumber)
        {
            var numberField = FindField(cardLine, fieldNumber);
            if (string.IsNullOrWhiteSpace(numberField))
            {
                return null;
            }

            numberField = DecodeNumbers(numberField);

            int number;
            if (!int.TryParse(numberField, out number))
            {                
                return null;
            }

            return number;
        }


        private string FindField(string cardLine,int fieldNumber)
        {
            var regex = @"\|" + fieldNumber + @"(.*?)\|";
            var matches = Regex.Matches(cardLine, regex);

            if (matches.Count == 0)
            {
                //Console.WriteLine("Could not find Field labeled {0}", fieldNumber);
                return null;
            }

            return matches[0].Groups[1].Value;
        }
    }
}
