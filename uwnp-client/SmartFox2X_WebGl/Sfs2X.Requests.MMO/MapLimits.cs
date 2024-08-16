using System;
using Sfs2X.Entities.Data;

namespace Sfs2X.Requests.MMO
{
	/// <summary>
	/// The MapLimits class is used to set the limits of the virtual environment represented by an MMORoom when creating it.
	/// </summary>
	///
	/// <remarks>
	/// The limits represent the minimum and maximum coordinate values (2D or 3D) that the MMORoom should expect.
	/// </remarks>
	///
	/// <seealso cref="T:Sfs2X.Requests.MMO.MMORoomSettings" />
	/// <seealso cref="T:Sfs2X.Entities.MMORoom" />
	/// <seealso cref="T:Sfs2X.Entities.Data.Vec3D" />
	public class MapLimits
	{
		private Vec3D lowerLimit;

		private Vec3D higherLimit;

		/// <summary>
		/// Returns the lower coordinates limit of the virtual environment along the X,Y,Z axes.
		/// </summary>
		public Vec3D LowerLimit
		{
			get
			{
				return lowerLimit;
			}
		}

		/// <summary>
		/// Returns the higher coordinates limit of the virtual environment along the X,Y,Z axes.
		/// </summary>
		public Vec3D HigherLimit
		{
			get
			{
				return higherLimit;
			}
		}

		/// <summary>
		/// Creates a new MapLimits instance.
		/// </summary>
		///
		/// <remarks>
		/// The <see cref="P:Sfs2X.Requests.MMO.MMORoomSettings.MapLimits" /> property must be set to this instance during the MMORoom creation.
		/// </remarks>
		///
		/// <param name="lowerLimit">The lower coordinates limit of the virtual environment along the X,Y,Z axes.</param>
		/// <param name="higherLimit">The higher coordinates limit of the virtual environment along the X,Y,Z axes.</param>
		public MapLimits(Vec3D lowerLimit, Vec3D higherLimit)
		{
			if (lowerLimit == null || higherLimit == null)
			{
				throw new ArgumentException("Map limits arguments must be both non null!");
			}
			this.lowerLimit = lowerLimit;
			this.higherLimit = higherLimit;
		}
	}
}
