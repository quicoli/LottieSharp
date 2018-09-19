using SharpDX.Direct2D1;

namespace LottieSharp
{
    public static class PorterDuff
    {
        public enum Mode
        {
            Clear,
            DstIn,
            DstOut,
            SrcAtop
        }

        public static CompositeMode ToCanvasComposite(Mode mode)
        {
            switch (mode)
            {
                case Mode.SrcAtop:
                    return CompositeMode.SourceAtop;
                case Mode.DstIn:
                    return CompositeMode.DestinationIn;
                case Mode.DstOut:
                    return CompositeMode.DestinationOut;
                //case Mode.Clear:
                default:
                    return CompositeMode.SourceCopy;
            }
        }
    }
}