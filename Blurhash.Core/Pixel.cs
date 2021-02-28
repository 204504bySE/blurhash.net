namespace Blurhash.Core
{
    /// <summary>
    /// Represents a pixel within the Blurhash algorithm
    /// </summary>
    public struct Pixel
    {
        public float Blue;
        public float Green;
        public float Red;

        public Pixel(float red, float green, float blue)
        {
            this.Blue = blue;
            this.Green = green;
            this.Red = red;
        }
    }
}