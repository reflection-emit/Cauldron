namespace Cauldron.Interception.Cecilator.Extensions
{
    public class BooleanExpressionCoder
    {
        internal readonly Coder coder;

        internal BooleanExpressionCoder(Coder coder)
        {
            this.coder = coder;
        }

        public static explicit operator Coder(BooleanExpressionCoder coder) => coder.coder;

        public override int GetHashCode() => this.coder.GetHashCode();

        public Coder ToCoder() => this.coder;
    }
}