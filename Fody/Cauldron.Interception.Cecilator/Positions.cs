namespace Cauldron.Interception.Cecilator
{
    public struct Positions
    {
        public Position Beginning;

        public Position End;

        public Positions(Position beginning, Position end)
        {
            this.Beginning = beginning;
            this.End = end;
        }
    }
}