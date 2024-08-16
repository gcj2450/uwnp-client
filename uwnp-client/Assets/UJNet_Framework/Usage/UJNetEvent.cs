using System;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Collections;
using UJNet.Data;

namespace UJNet
{
	class UJNetEvent {
		
		internal const string onConnectionEvent = "OnConnection";
		internal const string onConnectionLostEvent = "OnConnectionLost";
        internal const string onDebugMessageEvent = "OnDebugMessage";
        internal const string onResponseEvent = "OnResponse";	// shared by socket and http
        internal const string onHttpCloseEvent = "OnHttpClose";
		internal const string onHttpErrorEvent = "OnHttpError";
		internal const string onHttpResponseEvent = "OnHttpResponse";
		
		private String type;
        private Hashtable parameters;
		
		public UJNetEvent(String type, Hashtable param){
			this.type = type;
			this.parameters = param;
		}
		
		public object GetParameter(string key)
        {
            return parameters[key];
		}
		
		public new string GetType()
        {
            return type;
        }
		
	}	
}