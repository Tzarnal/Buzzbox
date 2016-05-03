using Buzzbox_Common;

namespace Buzzbox_Decode.Decoders
{
    public interface IDecoderInterface
    {
        Card DecodeCard(string cardLine);
    }
}
