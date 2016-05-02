using System;
using System.Text;
using Buzzbox.Encoders;
using Buzzbox_Common;

namespace Buzzbox
{
    class Encode
    {
        public Encode()
        {
            
        }

        public string EncodeCardCollection(CardCollection cardCollection, EncodingFormats encodingFormat)
        {
            var outputBuilder = new StringBuilder();
            var encoder = InstanciateEncoder(encodingFormat);

            foreach (var card in cardCollection.Cards)
            {
                var cardOutput = $"{encoder.EncodeCard(card)}\n\n";
                outputBuilder.Append(cardOutput);
            }

            return outputBuilder.ToString();
        }

        public IEncoderInterface InstanciateEncoder(EncodingFormats encodingFormat)
        {
            switch (encodingFormat)
            {
                case EncodingFormats.scfdivineFormat:
                    return new scfdivineFormatEncoder();
                default:
                    return new scfdivineFormatEncoder();
            }
        }
    }
}
