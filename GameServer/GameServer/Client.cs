using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Numerics;

namespace GameServer
{
	class Client
	{
		public static int dataBufferSize = 4096;

		public Player player;
		public int id;
		public TCP tcp;
		public UDP udp;

		public Client( int _clientId )
		{
			id = _clientId;
			tcp = new TCP( id );
			udp = new UDP( id );
		}

		public class TCP
		{
			public TcpClient socket;

			private readonly int id;
			private NetworkStream stream;
			private Packet receivedData;
			private byte[] receiveBuffer;

			public TCP( int _id )
			{
				id = _id;
			}

			public void Connect( TcpClient _socket )
			{
				Server.queuePlayers.Add( id );
				if ( Server.queuePlayers.Count == 1 )
					Server.queuePlayer++;

				socket = _socket;
				socket.ReceiveBufferSize = dataBufferSize;
				socket.SendBufferSize = dataBufferSize;

				stream = socket.GetStream();

				receivedData = new Packet();

				receiveBuffer = new byte[dataBufferSize];

				stream.BeginRead( receiveBuffer, 0, dataBufferSize, RecieveCallback, null );

				ServerSend.Welcome( id, "Welcome to the server", Server.SizeOfField );
			}

			public void SendData( Packet _packet )
			{
				try
				{
					if ( socket != null )
					{
						stream.BeginWrite( _packet.ToArray(), 0, _packet.Length(), null, null );
					}
				}
				catch ( Exception _ex )
				{
					Console.WriteLine( $"Error sending data to player {id} via TCP: {_ex}" );
				}
			}

			private void RecieveCallback( IAsyncResult _result )
			{
				try
				{
					int _byteLenght = stream.EndRead( _result );
					if ( _byteLenght <= 0 )
					{
						Server.clients[id].Disconnect();
						return;
					}

					byte[] _data = new byte[_byteLenght];
					Array.Copy( receiveBuffer, _data, _byteLenght );

					receivedData.Reset( HandleData( _data ) );
					stream.BeginRead( receiveBuffer, 0, dataBufferSize, RecieveCallback, null );
				}
				catch ( Exception _ex )
				{
					Console.WriteLine( $"Error recieving TCP data: {_ex}" );
					Server.clients[id].Disconnect();
				}

			}

			public void Disconnect()
			{
				socket.Close();
				stream = null;
				receivedData = null;
				receiveBuffer = null;
				socket = null;
			}

			private bool HandleData( byte[] _data )
			{
				int _packetLength = 0;

				receivedData.SetBytes( _data );

				if ( receivedData.UnreadLength() >= 4 )
				{
					_packetLength = receivedData.ReadInt();
					if ( _packetLength <= 0 )
					{
						return true;
					}
				}

				while ( _packetLength > 0 && _packetLength <= receivedData.UnreadLength() )
				{
					byte[] _packetBytes = receivedData.ReadBytes( _packetLength );
					ThreadManager.ExecuteOnMainThread( () =>
					{
						using ( Packet _packet = new Packet( _packetBytes ) )
						{
							int _packetId = _packet.ReadInt();
							Server.packetHandlers[_packetId]( id, _packet );
						}
					} );

					_packetLength = 0;
					if ( receivedData.UnreadLength() >= 4 )
					{
						_packetLength = receivedData.ReadInt();
						if ( _packetLength <= 0 )
						{
							return true;
						}
					}
				}

				if ( _packetLength <= 1 )
				{
					return true;
				}

				return false;
			}
		}
		public class UDP
		{
			public IPEndPoint endPoint;

			private int id;

			public UDP( int _id )
			{
				id = _id;
			}

			public void Connect( IPEndPoint _endPoint )
			{
				endPoint = _endPoint;
			}

			public void SendData( Packet _packet )
			{
				Server.SendUDPData( endPoint, _packet );
			}

			public void HandleData( Packet _packetData )
			{
				int _packetLength = _packetData.ReadInt();
				byte[] _packetBytes = _packetData.ReadBytes( _packetLength );

				ThreadManager.ExecuteOnMainThread( () =>
				{
					using ( Packet _packet = new Packet( _packetBytes ) )
					{
						int _packetId = _packet.ReadInt();
						Server.packetHandlers[_packetId]( id, _packet );
					}
				} );
			}

			public void Disconnect()
			{
				endPoint = null;
			}
		}
		public void SendIntoTheGame( string _playerName )
		{
			Random rndValue = new Random();
			Vector3 _position = new Vector3( rndValue.Next( 0, Server.SizeOfField ), rndValue.Next( 0, Server.SizeOfField ), 0 );
			foreach ( var _client in Server.clients )
			{
				if ( _client.Value.player != null )
				{
					if ( _client.Value.player.position == _position )
					{
						_position = new Vector3( rndValue.Next( 0, Server.SizeOfField ), rndValue.Next( 0, Server.SizeOfField ), 0 );
					}
				}
			}

			player = new Player( id, _playerName, _position );

			foreach ( Client _client in Server.clients.Values )
			{
				if ( _client.player != null )
				{
					if ( _client.id != id )
					{
						ServerSend.SpawnPlayer( id, _client.player );
					}
				}
			}

			foreach ( Client _client in Server.clients.Values )
			{
				if ( _client.player != null )
				{
					ServerSend.SpawnPlayer( _client.id, player );
				}
			}
		}

		private void Disconnect()
		{
			Console.WriteLine( $"{tcp.socket.Client.RemoteEndPoint} has disconnected." );

			//смена очереди на следующего, с условием того, что вышел тот, кто ходит в данный момент
			Server.queuePlayers.Remove( id );
			if ( Server.queuePlayer >= Server.queuePlayers.Count )
			{
				if ( Server.queuePlayers.Count > 0 )
				{
					Server.queuePlayer = 1;
					ServerSend.QueuePlayer( Server.queuePlayers[Server.queuePlayer - 1] );
				}
				else
					Server.queuePlayer = 0;
			}

			ServerSend.Disconnect( player );

			player = null;
			tcp.Disconnect();
			udp.Disconnect();
		}
	}
}
