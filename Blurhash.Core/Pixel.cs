namespace Blurhash.Core
{
    /// <summary>
    /// Represents a pixel within the Blurhash algorithm
    /// </summary>
    public struct Pixel
    {
        public float Red;
        public float Green;
        public float Blue;

        public Pixel(float red, float green, float blue)
        {
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
        }
    }
}