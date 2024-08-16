using System;

namespace Sfs2X.Entities.Data
{
	/// <summary>
	/// The Vec3D object represents a position in a 2D or 3D space.
	/// </summary>
	///
	/// <remarks>
	/// This class is used to express a position inside a virtual environment with no specific unit of measure (could be pixels, feet, meters, etc).
	/// <para />
	/// Positions along the X,Y,Z axes can be expressed as Integers or Floats, based on the game's coordinate system requirements.
	/// </remarks>
	public class Vec3D
	{
		private float fx;

		private float fy;

		private float fz;

		private int ix;

		private int iy;

		private int iz;

		private bool useFloat;

		/// <summary>
		/// Returns the position along the X axis as a float value.
		/// </summary>
		public float FloatX
		{
			get
			{
				return fx;
			}
		}

		/// <summary>
		/// Returns the position along the Y axis as a float value.
		/// </summary>
		public float FloatY
		{
			get
			{
				return fy;
			}
		}

		/// <summary>
		/// Returns the position along the Z axis as a float value.
		/// </summary>
		public float FloatZ
		{
			get
			{
				return fz;
			}
		}

		/// <summary>
		/// Returns the position along the X axis as an integer value.
		/// </summary>
		public int IntX
		{
			get
			{
				return ix;
			}
		}

		/// <summary>
		/// Returns the position along the Y axis as an integer value.
		/// </summary>
		public int IntY
		{
			get
			{
				return iy;
			}
		}

		/// <summary>
		/// Returns the position along the Z axis as an integer value.
		/// </summary>
		public int IntZ
		{
			get
			{
				return iz;
			}
		}

		/// <exclude />
		public static Vec3D fromArray(object array)
		{
			if (array is SFSArrayLite)
			{
				SFSArrayLite sFSArrayLite = array as SFSArrayLite;
				object elementAt = sFSArrayLite.GetElementAt(0);
				object elementAt2 = sFSArrayLite.GetElementAt(1);
				object value = ((sFSArrayLite.Size() > 2) ? sFSArrayLite.GetElementAt(2) : ((object)0));
				array = ((!(elementAt is double)) ? ((object)new int[3]
				{
					Convert.ToInt32(elementAt),
					Convert.ToInt32(elementAt2),
					Convert.ToInt32(value)
				}) : ((object)new float[3]
				{
					Convert.ToSingle(elementAt),
					Convert.ToSingle(elementAt2),
					Convert.ToSingle(value)
				}));
			}
			if (array is int[])
			{
				return fromIntArray((int[])array);
			}
			if (array is float[])
			{
				return fromFloatArray((float[])array);
			}
			throw new ArgumentException("Invalid Array Type, cannot convert to Vec3D!");
		}

		private static Vec3D fromIntArray(int[] array)
		{
			if (array.Length != 3)
			{
				throw new ArgumentException("Wrong array size. Vec3D requires an array with 3 parameters (x,y,z)");
			}
			return new Vec3D(array[0], array[1], array[2]);
		}

		private static Vec3D fromFloatArray(float[] array)
		{
			if (array.Length != 3)
			{
				throw new ArgumentException("Wrong array size. Vec3D requires an array with 3 parameters (x,y,z)");
			}
			return new Vec3D(array[0], array[1], array[2]);
		}

		private Vec3D()
		{
		}

		/// <summary>
		/// Creates a new Vec3D instance for a 3D coordinates system with integer values.
		/// </summary>
		///
		/// <param name="px">The position along the X axis.</param>
		/// <param name="py">The position along the Y axis.</param>
		/// <param name="pz">The position along the Z axis.</param>
		public Vec3D(int px, int py, int pz)
		{
			ix = px;
			iy = py;
			iz = pz;
			useFloat = false;
		}

		/// <summary>
		/// Creates a new Vec3D instance for a 2D coordinates system with integer values.
		/// </summary>
		///
		/// <param name="px">The position along the X axis.</param>
		/// <param name="py">The position along the Y axis.</param>
		public Vec3D(int px, int py)
			: this(px, py, 0)
		{
		}

		/// <summary>
		/// Creates a new Vec3D instance for a 3D coordinates system with float values.
		/// </summary>
		///
		/// <param name="px">The position along the X axis.</param>
		/// <param name="py">The position along the Y axis.</param>
		/// <param name="pz">The position along the Z axis.</param>
		public Vec3D(float px, float py, float pz)
		{
			fx = px;
			fy = py;
			fz = pz;
			useFloat = true;
		}

		/// <summary>
		/// Creates a new Vec3D instance for a 2D coordinates system with float values.
		/// </summary>
		///
		/// <param name="px">The position along the X axis.</param>
		/// <param name="py">The position along the Y axis.</param>
		public Vec3D(float px, float py)
			: this(px, py, 0f)
		{
		}

		/// <summary>
		/// Indicates whether the position is expressed using floating point values or not.
		/// </summary>
		///
		/// <returns><c>true</c> if the position is expressed using floating point values.</returns>
		public bool IsFloat()
		{
			return useFloat;
		}

		/// <exclude />
		public int[] ToIntArray()
		{
			return new int[3] { ix, iy, iz };
		}

		/// <exclude />
		public float[] ToFloatArray()
		{
			return new float[3] { fx, fy, fz };
		}

		/// <exclude />
		public override string ToString()
		{
			if (IsFloat())
			{
				return string.Format("({0},{1},{2})", fx, fy, fz);
			}
			return string.Format("({0},{1},{2})", ix, iy, iz);
		}
	}
}
