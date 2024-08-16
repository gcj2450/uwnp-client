using System.Security.Cryptography;
using System.Text;

namespace Sfs2X.Util
{
	/// <summary>
	/// Helper class for logging in with a pre-hashed password.
	/// </summary>
	///
	/// <remarks>
	/// This is needed if your server-side database stores User passwords hashed with MD5.
	/// <para />
	/// For more information see the <see href="http://docs2x.smartfoxserver.com/DevelopmentBasics/signup-assistant-basics" target="_blank">Sign Up Assistant component tutorial</see> (<b>Password Mode</b> section).
	/// </remarks>
	public class PasswordUtil
	{
		/// <summary>
		/// Generates the MD5 hash of the user password.
		/// </summary>
		///
		/// <param name="pass">The plain text password.</param>
		///
		/// <returns>The hashed password.</returns>
		///
		/// <example>
		/// <code>
		/// string userName = "testName";
		/// string userPass = "testPass";
		///
		/// string md5Pass = PasswordUtil.MD5Password(userPass);
		/// sfs.Send(new LoginRequest(userName, md5Pass, sfs.Config.Zone));
		/// </code>
		/// </example>
		public static string MD5Password(string pass)
		{
			StringBuilder stringBuilder = new StringBuilder(string.Empty);
			byte[] array = new MD5CryptoServiceProvider().ComputeHash(Encoding.Default.GetBytes(pass));
			foreach (byte b in array)
			{
				stringBuilder.Append(b.ToString("x2"));
			}
			return stringBuilder.ToString();
		}
	}
}
