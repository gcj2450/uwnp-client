using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.CompilerServices;
using Sfs2X.Core;

namespace Sfs2X.Util
{
	/// This version of the CryptoInitializerV2 class is used when building the project for Unity only.
	/// This is due to a compatibility issue with Unity's Nintendo Switch build, not supporting System.Net.Http.
	/// In this class method Init() was modified and static class ExtensionMethods added.
	public class CryptoInitializerV2 : ICryptoInitializer
	{
		[CompilerGenerated]
		private sealed class _003CInit_003Ed__6 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncVoidMethodBuilder _003C_003Et__builder;

			public CryptoInitializerV2 _003C_003E4__this;

			private string _003CtargetUrl_003E5__1;

			private HttpClient _003ChttpClient_003E5__2;

			private FormUrlEncodedContent _003CformContent_003E5__3;

			private HttpResponseMessage _003Creq_003E5__4;

			private string _003Cres_003E5__5;

			private HttpResponseMessage _003C_003Es__6;

			private Exception _003Cex_003E5__7;

			private TaskAwaiter<HttpResponseMessage> _003C_003Eu__1;

			private void MoveNext()
			{
				int num = _003C_003E1__state;
				try
				{
					if (num != 0)
					{
						_003CtargetUrl_003E5__1 = (_003C_003E4__this.useHttps ? "https://" : "http://") + _003C_003E4__this.sfs.Config.Host + ":" + (_003C_003E4__this.useHttps ? _003C_003E4__this.sfs.Config.HttpsPort : _003C_003E4__this.sfs.Config.HttpPort) + "/BlueBox/CryptoManager";
						_003ChttpClient_003E5__2 = new HttpClient();
					}
					try
					{
						if (num != 0)
						{
							_003ChttpClient_003E5__2.BaseAddress = new Uri(_003CtargetUrl_003E5__1);
							_003CformContent_003E5__3 = new FormUrlEncodedContent(new KeyValuePair<string, string>[1]
							{
								new KeyValuePair<string, string>("SessToken", _003C_003E4__this.sfs.SessionToken)
							});
						}
						try
						{
							TaskAwaiter<HttpResponseMessage> awaiter;
							if (num != 0)
							{
								awaiter = _003ChttpClient_003E5__2.PostAsync("", _003CformContent_003E5__3).GetAwaiter();
								if (!awaiter.IsCompleted)
								{
									num = (_003C_003E1__state = 0);
									_003C_003Eu__1 = awaiter;
									_003CInit_003Ed__6 stateMachine = this;
									_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
									return;
								}
							}
							else
							{
								awaiter = _003C_003Eu__1;
								_003C_003Eu__1 = default(TaskAwaiter<HttpResponseMessage>);
								num = (_003C_003E1__state = -1);
							}
							_003C_003Es__6 = awaiter.GetResult();
							_003Creq_003E5__4 = _003C_003Es__6;
							_003C_003Es__6 = null;
							_003Creq_003E5__4.EnsureSuccessStatusCode();
							_003Cres_003E5__5 = _003Creq_003E5__4.Content.ReadAsStringAsync().Result;
							_003C_003E4__this.OnHttpResponse(_003Cres_003E5__5);
							_003Creq_003E5__4 = null;
							_003Cres_003E5__5 = null;
						}
						catch (Exception ex)
						{
							_003Cex_003E5__7 = ex;
							_003C_003E4__this.OnHttpError(_003Cex_003E5__7.Message);
						}
						_003CformContent_003E5__3 = null;
					}
					finally
					{
						if (num < 0 && _003ChttpClient_003E5__2 != null)
						{
							((IDisposable)_003ChttpClient_003E5__2).Dispose();
						}
					}
					_003ChttpClient_003E5__2 = null;
				}
				catch (Exception ex)
				{
					_003C_003E1__state = -2;
					_003CtargetUrl_003E5__1 = null;
					_003C_003Et__builder.SetException(ex);
					return;
				}
				_003C_003E1__state = -2;
				_003CtargetUrl_003E5__1 = null;
				_003C_003Et__builder.SetResult();
			}

			void IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		private const string KEY_SESSION_TOKEN = "SessToken";

		private const string TARGET_SERVLET = "/BlueBox/CryptoManager";

		private SmartFox sfs;

		private bool useHttps = true;

		public CryptoInitializerV2(SmartFox sfs)
		{
			if (!sfs.IsConnected)
			{
				throw new InvalidOperationException("Cryptography cannot be initialized before connecting to SmartFoxServer!");
			}
			if (sfs.GetSocketEngine().CryptoKey != null)
			{
				throw new InvalidOperationException("Cryptography is already initialized!");
			}
			this.sfs = sfs;
		}

		public void Run()
		{
			Init();
		}

		[AsyncStateMachine(typeof(_003CInit_003Ed__6))]
		[DebuggerStepThrough]
		private void Init()
		{
			_003CInit_003Ed__6 stateMachine = new _003CInit_003Ed__6();
			stateMachine._003C_003Et__builder = AsyncVoidMethodBuilder.Create();
			stateMachine._003C_003E4__this = this;
			stateMachine._003C_003E1__state = -1;
			stateMachine._003C_003Et__builder.Start(ref stateMachine);
		}

		private void OnHttpResponse(string rawData)
		{
			byte[] data = Convert.FromBase64String(rawData);
			ByteArray byteArray = new ByteArray();
			ByteArray byteArray2 = new ByteArray();
			byteArray.WriteBytes(data, 0, 16);
			byteArray2.WriteBytes(data, 16, 16);
			sfs.GetSocketEngine().CryptoKey = new CryptoKey(byteArray2, byteArray);
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["success"] = true;
			sfs.DispatchEvent(new SFSEvent(SFSEvent.CRYPTO_INIT, dictionary));
		}

		private void OnHttpError(string errorMsg)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["success"] = false;
			dictionary["errorMessage"] = errorMsg;
			sfs.DispatchEvent(new SFSEvent(SFSEvent.CRYPTO_INIT, dictionary));
		}
	}
}
