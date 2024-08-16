using Sfs2X.Bitswarm;
using Sfs2X.Entities.Data;

namespace Sfs2X.Requests
{
	/// <exclude />
	public class BaseRequest : IRequest
	{
		/// <exclude />
		public static readonly string KEY_ERROR_CODE = "ec";

		/// <exclude />
		public static readonly string KEY_ERROR_PARAMS = "ep";

		/// <exclude />
		protected ISFSObject sfso;

		private int id;

		/// <exclude />
		protected int targetController;

		private bool isEncrypted;

		/// <exclude />
		public int TargetController
		{
			get
			{
				return targetController;
			}
			set
			{
				targetController = value;
			}
		}

		/// <exclude />
		public bool IsEncrypted
		{
			get
			{
				return isEncrypted;
			}
			set
			{
				isEncrypted = value;
			}
		}

		/// <exclude />
		public IMessage Message
		{
			get
			{
				IMessage message = new Message();
				message.Id = id;
				message.IsEncrypted = isEncrypted;
				message.TargetController = targetController;
				message.Content = sfso;
				if (this is ExtensionRequest)
				{
					message.IsUDP = (this as ExtensionRequest).UseUDP;
				}
				return message;
			}
		}

		/// <exclude />
		public int Id
		{
			get
			{
				return id;
			}
			set
			{
				id = value;
			}
		}

		/// <exclude />
		public RequestType Type
		{
			get
			{
				return (RequestType)id;
			}
			set
			{
				id = (int)value;
			}
		}

		/// <exclude />
		public BaseRequest(RequestType tp)
		{
			sfso = SFSObject.NewInstance();
			targetController = 0;
			isEncrypted = false;
			id = (int)tp;
		}

		/// <exclude />
		public BaseRequest(int id)
		{
			sfso = SFSObject.NewInstance();
			targetController = 0;
			isEncrypted = false;
			this.id = id;
		}

		/// <exclude />
		public virtual void Validate(SmartFox sfs)
		{
		}

		/// <exclude />
		public virtual void Execute(SmartFox sfs)
		{
		}
	}
}
