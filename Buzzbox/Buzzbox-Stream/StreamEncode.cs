using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Buzzbox_Common;
using Buzzbox_Common.Encoders;

namespace Buzzbox_Stream
{
    class StreamEncode
    {
        public bool LoopForever;
        public bool ShuffleFields;
        public bool IncludeFlavorText;

        public bool NameReplacement;
        public NameCollection NameCollection;

        private MtgEncodeFormatEncoder _encoder;        
        private CardCollection _cardCollection;
        private StreamWriter _stream;
        private ConsoleLog _consoleLog;

        public StreamEncode(CardCollection cardCollection, StreamWriter stream)
        {
            _encoder = new MtgEncodeFormatEncoder();
            

            _consoleLog = ConsoleLog.Instance;

            _cardCollection = cardCollection;
            _stream = stream;
        }

        public void ThreadEntry()
        {
            _encoder.IncludeFlavorText = IncludeFlavorText;
 
            do
            {
                _cardCollection.Cards.Shuffle();

                foreach (var card in _cardCollection.Cards)
                {
                    if (!(card.Type == "MINION" || card.Type == "SPELL" || card.Type == "WEAPON"))
                    {
                        //Console.WriteLine(card.Type);
                        continue;
                    }

                    //See if names should be replaced, roll dice, replace name.
                    if (card.Type != "HERO" && NameReplacement && NameCollection.ReplaceName())
                    {
                        string newName;
                        try
                        {
                            newName = NameCollection.RandomName(card.CardClass, card.Type);
                        }
                        catch (Exception e)
                        {
                            _consoleLog.VerboseWriteLine($"Error trying to find a new card name for {card.Name}[{card.CardClass}-{card.Type}]: {e.Message}");
                            newName = "None";
                        }

                        if (newName != "None")
                        {
                            card.Name = newName;
                        }                        
                    }

                    var outputLine = _encoder.EncodeCard(card);

                    if (string.IsNullOrWhiteSpace(outputLine))
                    {
                        continue;
                    }

                    if (ShuffleFields)
                    {
                        outputLine = ShuffleCardFields(outputLine);
                    }

                    _stream.Write(outputLine + "\n\n");
                
                }
               
            } while (LoopForever);
   

            _stream.Close();
        }

        private string ShuffleCardFields(string cardLine)
        {
            cardLine = cardLine.TrimEnd('|').TrimStart('|');
            List<string> fields = cardLine.Split('|').ToList();
            fields.Shuffle();
            return $"|{string.Join("|", fields)}|";
        }
    }
}
