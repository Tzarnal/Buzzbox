using System;
using System.IO;
using Buzzbox_Decode.Decoders;
using Buzzbox_Common;

namespace Buzzbox_Decode
{
    public class Decode
    {
        public string Set;
        public string Source;
        public string Texture;

        private Random rnd;
        private ConsoleLog _ConsoleLog;

        public int PotentialCards;
        public int ActualCards;

        public Decode(string set = "RNN", string source = "hs-rnn", string texture = "Default")
        {
            Set = set;
            Source = source;
            Texture = texture;
            rnd = new Random();

            _ConsoleLog = ConsoleLog.Instance;
        }

        //Dispatch lines to be decoded to a decoder selected by decodingFormat
        public CardCollection DecodeString(string inputData, EncodingFormats decodingFormat)
        {
            PotentialCards = 0;
            ActualCards = 0;

            var cardCollection = new CardCollection();
            var decoder = InstanciateDecoder(decodingFormat);

            if (string.IsNullOrWhiteSpace(inputData))
            {
                Console.WriteLine("Input contained no data.");
                return cardCollection;
            }

            using (StringReader reader = new StringReader(inputData))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {                    
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    PotentialCards++;
                    var card = decoder.DecodeCard(line);

                    if (card != null)
                    {
                        ActualCards++;
                        SetAdditionalData(card);
                        cardCollection.Cards.Add(card);

                        _ConsoleLog.VerboseWriteLine(card + Environment.NewLine);
                    }
                    else
                    {
                        _ConsoleLog.VerboseWriteLine($"Dud: '{line}'" + Environment.NewLine);
                    }
                }
            }

            return cardCollection;
        }

        private void SetAdditionalData(Card card)
        {
            card.Set = Set;
            card.Source = Source;
            card.Texture = Texture;
            card.Id = RandomCardId();
        }

        private string RandomCardId()
        {
            string id = "";
            id += (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            id += "-" +rnd.Next();
            return id;
        }

        //Return an encoder based on the requested decodingFormat
        private IDecoderInterface InstanciateDecoder(EncodingFormats decodingFormat)
        {
            switch (decodingFormat)
            {
                case EncodingFormats.scfdivineFormat:
                    return new scfdivineFormatDecoder();

                case EncodingFormats.MtgEncoderFormat:
                default:
                    return new MtgEncodeFormatDecoder();
            }
        }
    }
}
