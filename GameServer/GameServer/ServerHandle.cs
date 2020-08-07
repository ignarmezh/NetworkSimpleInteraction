using System;
using System.Linq;
using System.Numerics;

namespace GameServer
{
	class ServerHandle
	{
		public static void WelcomeReceived( int _fromClient, Packet _packet )
		{
			int _clientidCkeck = _packet.ReadInt();
			string _username = _packet.ReadString();

			Console.WriteLine( $"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected and is now player {_fromClient}." );
			if ( _fromClient != _clientidCkeck )
			{
				Console.WriteLine( $"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientidCkeck})!" );
			}
			Server.clients[_fromClient].SendIntoTheGame( _username );

			if ( Server.queuePlayers.Count == 1 )
			{
				ServerSend.QueuePlayer( Server.queuePlayers[Server.queuePlayer - 1] );
			}
		}

		public static void TargetPosition( int _fromClient, Packet _packet )
		{
			Vector3 _targetPosition = _packet.ReadVector3();

			if ( Server.queuePlayers.Count == Server.queuePlayer )
				Server.queuePlayer = 1;
			else
				Server.queuePlayer++;

			ServerSend.QueuePlayer( Server.queuePlayers[Server.queuePlayer - 1] );

			Server.clients[_fromClient].player.SetTargetPosition( _targetPosition );
		}
	}
}
