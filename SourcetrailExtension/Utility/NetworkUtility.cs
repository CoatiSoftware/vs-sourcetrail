﻿/*
 * Copyright 2017 Coati Software OG
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

namespace CoatiSoftware.SourcetrailExtension.Utility
{
	public class StateObject
	{
		public Socket _workSocket = null;
		public const int _bufferSize = 1024;
		public byte[] _buffer = new byte[_bufferSize];
		public StringBuilder _stringBuilder = new StringBuilder();
	}

	public class AsynchronousSocketListener
	{
		public static ManualResetEvent _allDone = new ManualResetEvent(false);

		public delegate void OnReadCallback(string message);

		public static OnReadCallback _onReadCallback = null;
		public static OnReadCallback _onErrorCallback = null;

		private static string _endOfMessageToken = "<EOM>";

		public static uint _port = 6666;
		
		public AsynchronousSocketListener()
		{
		}

		public void DoWork()
		{
			StartListening();
		}

		public static void StartListening()
		{
			const string ipAddressString = "127.0.0.1";

			IPAddress ipAddress = IPAddress.Parse(ipAddressString);
			IPEndPoint localEndPoint = new IPEndPoint(ipAddress, (int)_port);

			Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

			try
			{
				listener.Bind(localEndPoint);
				listener.Listen(100);

				while (true)
				{
					_allDone.Reset();

					listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
					_allDone.WaitOne();
				}
			}
			catch (Exception e)
			{
				Logging.Logging.LogError("Exception: " + e.Message);
			}
		}

		public static void AcceptCallback(IAsyncResult ar)
		{
			try
			{
				_allDone.Set();

				Socket listener = (Socket)ar.AsyncState;
				Socket handler = listener.EndAccept(ar);

				StateObject state = new StateObject();
				state._workSocket = handler;
				handler.BeginReceive(state._buffer, 0, StateObject._bufferSize, 0, new AsyncCallback(ReadCallback), state);
			}
			catch(Exception e)
			{
				Logging.Logging.LogError("Exception: " + e.Message);
			}
		}

		public static void ReadCallback(IAsyncResult ar)
		{
			try
			{
				string content = String.Empty;

				StateObject state = (StateObject)ar.AsyncState;
				Socket handler = state._workSocket;

				int bytesRead = handler.EndReceive(ar);

				if (bytesRead > 0)
				{
					state._stringBuilder.Append(Encoding.ASCII.GetString(state._buffer, 0, bytesRead));

					content = state._stringBuilder.ToString();
					if (content.IndexOf(_endOfMessageToken) > -1)
					{
						if (_onReadCallback != null)
						{
							_onReadCallback(content);
						}
					}
					else
					{
						handler.BeginReceive(state._buffer, 0, StateObject._bufferSize, 0, new AsyncCallback(ReadCallback), state);
					}
				}
			}
			catch(Exception e)
			{
				Logging.Logging.LogError("Excpetion: " + e.Message);
			}
		}
	}

	public class AsynchronousClient
	{
		public static uint _port = 6667;

		private static ManualResetEvent connectDone = new ManualResetEvent(false);
		private static ManualResetEvent sendDone = new ManualResetEvent(false);

		private static String response = String.Empty;

		public static AsynchronousSocketListener.OnReadCallback _onErrorCallback = null;

		public static void Send(string message)
		{
			IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
			IPEndPoint remoteEP = new IPEndPoint(ipAddress, (int)_port);

			Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

			try
			{
				IAsyncResult ar = client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
				if (!connectDone.WaitOne(2000))
				{
					client.EndConnect(ar);

					Logging.Logging.LogWarning("Connection timed out, message was not sent");

					return;
				}

				Send(client, message);
				sendDone.WaitOne();
			}
			catch(Exception e)
			{
				Logging.Logging.LogError("Exception: " + e.Message);
			}
			finally
			{
				if (client.Connected)
				{
					client.Shutdown(SocketShutdown.Both);
					client.Close();
				}
			}
		}

		private static void ConnectCallback(IAsyncResult ar)
		{
			try
			{
				Socket client = (Socket)ar.AsyncState;

				if (client.Connected == false)
				{
					return;
				}

				client.EndConnect(ar);

				connectDone.Set();
			}
			catch (Exception e)
			{
				Logging.Logging.LogError("Excpetion: " + e.Message);

				if (_onErrorCallback != null)
				{
					_onErrorCallback(e.ToString());
				}
			}
		}

		private static void Send(Socket client, String message)
		{
			try
			{
				byte[] bytes = Encoding.UTF8.GetBytes(message);
				client.BeginSend(bytes, 0, bytes.Length, 0, new AsyncCallback(SendCallback), client);
			}
			catch(Exception e)
			{
				Logging.Logging.LogError("Exception: " + e.Message);
			}
		}

		private static void SendCallback(IAsyncResult ar)
		{
			try
			{
				Socket client = (Socket)ar.AsyncState;

				int bytesSent = client.EndSend(ar);
				
				sendDone.Set();
			}
			catch (Exception e)
			{
				Logging.Logging.LogError("Excpetion: " + e.Message);

				if (_onErrorCallback != null)
				{
					if (e is ObjectDisposedException)
					{
						// get this exception every once in a while
						// doesn't seem to do much, no idea yet why it's there to begin with
					}
					else
					{

						_onErrorCallback(e.ToString());
					}
				}
			}
		}
	}
}
