using System;
using System.Text;
using Buzzbox_Decode.Decoders;
using Buzzbox_Common;

namespace Buzzbox_Decode
{
    public class Decode
    {
        public bool Verbal;

        public Decode(bool verbal = true)
        {
            Verbal = verbal;
        }

        //Dispatch lines to be decoded to a decoder selected by decodingFormat
        public CardCollection DecodeString(string inputData, EncodingFormats decodingFormat)
        {
            var cardCollection = new CardCollection();

            if (string.IsNullOrWhiteSpace(inputData))
            {
                Console.WriteLine("Input contained no data.");
                return cardCollection;
            }
            
            return cardCollection;
        }

        //Return an encoder based on the requested decodingFormat
        public IDecoderInterface InstanciateDecoder(EncodingFormats decodingFormat)
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
