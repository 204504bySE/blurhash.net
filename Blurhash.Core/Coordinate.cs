namespace Blurhash.Core
{
    /// <summary>
    /// Represents a 2D-coordinate 
    /// </summary>
    public readonly struct Coordinate
    {
        public int X { get; }
        public int Y { get; }

        public Coordinate(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}