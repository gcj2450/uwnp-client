namespace Sfs2X.Entities.Data
{
	/// <summary>
	/// A wrapper object used by SFSObject and SFSArray to encapsulate data and relative types
	/// </summary>
	///
	/// <exclude />
	public class SFSDataWrapper
	{
		private int type;

		private object data;

		/// <exclude />
		public int Type
		{
			get
			{
				return type;
			}
		}

		/// <exclude />
		public object Data
		{
			get
			{
				return data;
			}
		}

		/// <exclude />
		public SFSDataWrapper(int type, object data)
		{
			this.type = type;
			this.data = data;
		}

		/// <exclude />
		public SFSDataWrapper(SFSDataType tp, object data)
		{
			type = (int)tp;
			this.data = data;
		}
	}
}
