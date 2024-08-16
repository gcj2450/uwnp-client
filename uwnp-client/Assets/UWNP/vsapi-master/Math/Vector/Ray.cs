﻿namespace Vintagestory.API.MathTools
{
    public class Ray
    {
        public Vec3d origin;
        public Vec3d dir;

        public double Length
        {
            get { return dir.Length(); }
        }

        public Ray()
        {
            origin = new Vec3d();
            dir = new Vec3d();
        }

        public Ray(Vec3d o, Vec3d d)
        {
            origin = o;
            dir = d;
        }

        public void LimitToWalls(int minx, int miny, int minz, int maxx, int maxy, int maxz)
        {
            origin.X = GameMath.Clamp(origin.X, minx, maxx);
            origin.Y = GameMath.Clamp(origin.Y, miny, maxy);
            origin.Z = GameMath.Clamp(origin.Z, minz, maxz);

            dir.X = GameMath.Clamp(origin.X + dir.X, minx, maxx) - origin.X;
            dir.Y = GameMath.Clamp(origin.Y + dir.Y, miny, maxy) - origin.Y;
            dir.Z = GameMath.Clamp(origin.Z + dir.Z, minz, maxz) - origin.Z;
        }
    }
}
