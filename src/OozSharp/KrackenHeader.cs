namespace OozSharp
{
    public class KrackenHeader
    {
        internal DecoderTypes DecoderType { get; set; }
        internal bool RestartDecoder { get; set; }
        internal bool Uncompressed { get; set; }
        internal bool UseChecksums { get; set; }
    }
}
