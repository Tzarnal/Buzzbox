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

        private MtgEncodeFormatEncoder _encoder;        
        private CardCollection _cardCollection;
        private StreamWriter _stream;

        public StreamEncode(CardCollection cardCollection, StreamWriter stream)
        {
            _encoder = new MtgEncodeFormatEncoder();

            _cardCollection = cardCollection;
            _stream = stream;
        }

        public void ThreadEntry()
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

                    //actually try to output
                    try
                    {
                        _stream.Write(outputLine);
                    }
                    catch (Exception)
                    {
                        //fd was probably closed or otherwise no longer available
                        return;
                    }                    
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
