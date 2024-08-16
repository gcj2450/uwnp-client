﻿namespace Vintagestory.API.MathTools
{

    /// <summary>
    /// Represents a vector of 2 doubles. Go bug Tyron of you need more utility methods in this class.
    /// </summary>
    public class Size3f
    {
        public float Width;
        public float Height;
        public float Length;


        public Size3f()
        {

        }

        public Size3f(float width, float height, float length)
        {
            this.Width = width;
            this.Height = height;
            this.Length = length;
        }

        public Size3f Clone()
        {
            return new Size3f(Width, Height, Length);
        }

        public bool CanContain(Size3f obj)
        {
            return Width >= obj.Width && Height >= obj.Height && Length >= obj.Length;
        }
    }
    
    /// <summary>
    /// Represents a vector of 2 doubles. Go bug Tyron of you need more utility methods in this class.
    /// </summary>
    public class Size3d
    {
        public double Width;
        public double Height;
        public double Length;


        public Size3d()
        {

        }

        public Size3d(double width, double height, double length)
        {
            this.Width = width;
            this.Height = height;
            this.Length = length;
        }

        public Size3d Clone()
        {
            return new Size3d(Width, Height, Length);
        }

        public bool CanContain(Size3d obj)
        {
            return Width >= obj.Width && Height >= obj.Height && Length >= obj.Length;
        }
    }



    /// <summary>
    /// Represents a vector of 2 doubles. Go bug Tyron of you need more utility methods in this class.
    /// </summary>
    public class Size3i
    {
        public int Width;
        public int Height;
        public int Length;


        public Size3i()
        {

        }

        public Size3i(int width, int height, int length)
        {
            this.Width = width;
            this.Height = height;
            this.Length = length;
        }

        public Size3i Clone()
        {
            return new Size3i(Width, Height, Length);
        }

        public bool CanContain(Size3i obj)
        {
            return Width >= obj.Width && Height >= obj.Height && Length >= obj.Length;
        }
    }
}
