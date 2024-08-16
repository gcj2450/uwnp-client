using System;

namespace Sfs2X.Requests
{
	/// <summary>
	/// The MessageRecipientMode class is used to specify the recipient/s of moderator and administrator messages.
	/// </summary>
	///
	/// <remarks>
	/// Read the constants descriptions in the <see cref="T:Sfs2X.Requests.MessageRecipientType" /> enumeration for more informations.
	/// </remarks>
	///
	/// <seealso cref="T:Sfs2X.Requests.ModeratorMessageRequest" />
	/// <seealso cref="T:Sfs2X.Requests.AdminMessageRequest" />
	public class MessageRecipientMode
	{
		private object target;

		private int mode;

		/// <summary>
		/// Returns the moderator/administrator message target, according to the selected recipient mode.
		/// </summary>
		public object Target
		{
			get
			{
				return target;
			}
		}

		/// <summary>
		/// Returns the selected recipient mode.
		/// </summary>
		public int Mode
		{
			get
			{
				return mode;
			}
		}

		/// <summary>
		/// Creates a new MessageRecipientMode instance.
		/// </summary>
		///
		/// <remarks>
		/// The instance must be passed as <em>recipientMode</em> parameter to the <see cref="T:Sfs2X.Requests.ModeratorMessageRequest" /> and <see cref="T:Sfs2X.Requests.AdminMessageRequest" /> classes constructors.
		/// </remarks>
		///
		/// <param name="mode">One of the costants contained in the <see cref="T:Sfs2X.Requests.MessageRecipientType" /> enumerator, describing the recipient mode.</param>
		/// <param name="target">The moderator/administrator message recipient/s, according to the selected recipient mode.</param>
		public MessageRecipientMode(int mode, object target)
		{
			if (mode < 0 || mode > 3)
			{
				throw new ArgumentException("Illegal recipient mode: " + mode);
			}
			this.mode = mode;
			this.target = target;
		}
	}
}
