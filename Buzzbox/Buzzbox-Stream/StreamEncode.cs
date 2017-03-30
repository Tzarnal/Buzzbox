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

            try
            {
                do
                {
                    _cardCollection.Cards.Shuffle();

                    foreach (var card in _cardCollection.Cards)
                    {
                        var outputLine = _encoder.EncodeCard(card) + "\n\n";

                        if (ShuffleFields)
                        {
                            outputLine = ShuffleCardFields(outputLine);
                        }

                        _stream.Write(outputLine);
                
                    }
               
                } while (LoopForever);
            }
            catch (Exception)
            {
                _consoleLog.WriteLine("Closing StreamText Thread, stream no longer available.");
                return;
            }

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
