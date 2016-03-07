using System;
using System.Diagnostics;
using System.Net;
using System.Security.Principal;
using System.Threading;
using WebSocketSharp;
using WebSocketSharp.Net;

namespace Example
{
	public class Program
	{
		public static void Main(string[] args)
		{
			// Create a new instance of the WebSocket class.
			//
			// The WebSocket class inherits the System.IDisposable interface, so you can
			// use the using statement. And the WebSocket connection will be closed with
			// close status 1001 (going away) when the control leaves the using block.
			//
			// If you would like to connect to the server with the secure connection,
			// you should create the instance with the wss scheme WebSocket URL.


			//using (var ws = new WebSocket ("ws://echo.websocket.org"))
			//using (var ws = new WebSocket ("wss://echo.websocket.org"))
			//using (var ws = new WebSocket ("ws://localhost:4649/Echo"))
			//using (var ws = new WebSocket ("ws://localhost:4649/Echo?name=nobita"))
			//using (var ws = new WebSocket ("wss://localhost:4649/Echo"))
			//using (var ws = new WebSocket ("ws://localhost:4649/Chat"))
			//using (var ws = new WebSocket ("ws://localhost:4649/Chat?name=nobita"))
			//using (var ws = new WebSocket ("wss://localhost:4649/Chat"))
			using (var ws = new WebSocket("ws://bocorp-test.ioffice100.net/an/Users/bbd70056c684475e932b0dd6f90ffa50/CreateConnection?d=3/1/2016 8:11:18 AM"))
			{
				// Set the WebSocket events.

				ws.OnOpen += (sender, e) => ws.Send("Hi, there!");

				ws.OnMessage += (sender, e) =>
				{
					Console.WriteLine(e.Data);
				};

				ws.OnError += (sender, e) =>
				 {
					 //Console.WriteLine(e.Data);
				 };

				ws.OnClose += (sender, e) =>
			{
				//	Console.WriteLine(e.Data);
			};

#if DEBUG
				// To change the logging level.
				ws.Log.Level = LogLevel.Trace;

				// To change the wait time for the response to the Ping or Close.
				ws.WaitTime = TimeSpan.FromSeconds(10);

				// To emit a WebSocket.OnMessage event when receives a ping.
				ws.EmitOnPing = true;
#endif
				// To enable the Per-message Compression extension.
				//ws.Compression = CompressionMethod.Deflate;

				/* To validate the server certificate.
				ws.SslConfiguration.ServerCertificateValidationCallback =
				  (sender, certificate, chain, sslPolicyErrors) => {
					ws.Log.Debug (
					  String.Format (
						"Certificate:\n- Issuer: {0}\n- Subject: {1}",
						certificate.Issuer,
						certificate.Subject));

					return true; // If the server certificate is valid.
				  };
				 */

				// To send the credentials for the HTTP Authentication (Basic/Digest).
				//ws.SetCredentials ("nobita", "password", false);

				// To send the Origin header.
				//ws.Origin = "http://localhost:4649";

				// To send the Cookies.
				//ws.SetCookie (new Cookie ("name", "nobita"));
				//ws.SetCookie (new Cookie ("roles", "\"idiot, gunfighter\""));

				//	To connect through the HTTP Proxy server.

				var proxy = HttpWebRequest.GetSystemWebProxy();

				if (proxy != null)
				{
					var url = new Uri("http://bocorp-test.ioffice100.net/");
					Uri proxyUri = proxy.GetProxy(url);
					if (proxyUri != null)
					{
						if (proxyUri.Host != url.Host)
						{
							ICredentials credentials = CredentialCache.DefaultNetworkCredentials;
							if (proxy.Credentials != null)
							{
								credentials = proxy.Credentials;
							}

							System.Net.NetworkCredential netCredential = null;
							if (credentials != null)
							{
								//就是windows系统本地保存的用户凭证！这样基本没有什么技术问题了
								netCredential = credentials.GetCredential(proxyUri, "Basic");
								if (netCredential == null)
								{
									netCredential = credentials.GetCredential(proxyUri, "Digest");
								}
							}

							var username = string.Empty;
							var password = string.Empty;

							if (netCredential != null)
							{
								username = netCredential.UserName;
								password = netCredential.Password;
							}
							ws.SetProxy(proxyUri.AbsoluteUri, username, password);
						}
					}
				}

				// To enable the redirection.
				//ws.EnableRedirection = true;

				// Connect to the server.
				ws.Connect();

				// Connect to the server asynchronously.
				//ws.ConnectAsync ();

				Console.WriteLine("\nType 'exit' to exit.\n");
				while (true)
				{
					Thread.Sleep(1000);
					Console.Write("> ");
					var msg = Console.ReadLine();
					if (msg == "exit")
						break;

					// Send a text message.
					ws.Send(msg);
				}
			}
		}
	}
}
