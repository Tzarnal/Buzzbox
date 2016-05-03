using System;
using System.IO;
using System.Text;
using Buzzbox_Decode.Decoders;
using Buzzbox_Common;

namespace Buzzbox_Decode
{
    public class Decode
    {
        public bool Verbal;
        public string Set;
        public string Source;

        private Random rnd;

        public Decode(bool verbal = true, string set = "RNN", string source = "hs-rnn")
        {
            Verbal = verbal;
            Set = set;
            Source = source;
            rnd = new Random();
        }

        //Dispatch lines to be decoded to a decoder selected by decodingFormat
        public CardCollection DecodeString(string inputData, EncodingFormats decodingFormat)
        {
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

                    var card = decoder.DecodeCard(line);

                    if (card != null)
                    {
                        SetAdditionalData(card);
                        cardCollection.Cards.Add(card);
                    }
                    else
                    {
                        Console.WriteLine("Dud: '{0}'\n",line);
                    }
                }
            }

            return cardCollection;
        }

        private void SetAdditionalData(Card card)
        {
            card.Set = Set;
            card.Source = Source;
            card.Texture = Set;
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
                default:
                    return new scfdivineFormatDecoder();
            }
        }
    }
}
