using System;
using System.Text;
using Buzzbox.Encoders;
using Buzzbox_Common;

namespace Buzzbox
{
    public class Encode
    {
        public Encode()
        {
            
        }

        //Dispatch cards to be encoded to an encoder selected by command line option. then stitch them together in a stringbuilder. 
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

        //Return an encoder based on the requested encodingFormat
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
