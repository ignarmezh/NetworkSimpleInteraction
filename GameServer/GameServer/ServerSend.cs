using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer
{
	class ServerSend
	{
		private static void SendTCPData( int _toClient, Packet _packet )
		{
			_packet.WriteLength();
			Server.clients[_toClient].tcp.SendData( _packet );
		}

		private static void SendTCPDataToAll( Packet _packet )
		{
			_packet.WriteLength();
			for ( int i = 1; i <= Server.MaxPlayers; i++ )
			{
				Server.clients[i].tcp.SendData( _packet );
			}
		}

		private static void SendTCPDataToAll( int _exceptClient, Packet _packet )
		{
			_packet.WriteLength();
			for ( int i = 1; i <= Server.MaxPlayers; i++ )
			{
				if ( i != _exceptClient )
				{
					Server.clients[i].tcp.SendData( _packet );
				}
			}
		}
		private static void SendUDPDataToAll( Packet _packet )
		{
			_packet.WriteLength();
			for ( int i = 1; i <= Server.MaxPlayers; i++ )
			{
				Server.clients[i].udp.SendData( _packet );
			}
		}
		private static void SendUDPDataToAll( int _exceptClient, Packet _packet )
		{
			_packet.WriteLength();
			for ( int i = 1; i <= Server.MaxPlayers; i++ )
			{
				if ( i != _exceptClient )
				{
					Server.clients[i].udp.SendData( _packet );
				}
			}
		}

		public static void Welcome( int _toClient, string _msg, int _sizeOfField )
		{
			using ( Packet _packet = new Packet( ( int )ServerPackets.welcome ) )
			{
				_packet.Write( _msg );
				_packet.Write( _toClient );
				_packet.Write( _sizeOfField );

				SendTCPData( _toClient, _packet );
			}
		}

		public static void SpawnPlayer( int _toClient, Player _player )
		{
			using ( Packet _packet = new Packet( ( int )ServerPackets.spawnPlayer ) )
			{
				_packet.Write( _player.id );
				_packet.Write( _player.username );
				_packet.Write( _player.position );
				_packet.Write( _player.rotation );

				SendTCPData( _toClient, _packet );
			}
		}

		public static void Route( Player _player, List<Vector2> _route )
		{
			if ( _route == null )
				return;
			using ( Packet _packet = new Packet( ( int )ServerPackets.route ) )
			{
				_packet.Write( _player.id );
				_packet.Write( _route.Count );
				foreach ( Vector2 point in _route )
				{
					_packet.Write( new Vector3( point.X, point.Y, 0 ) );
				}
				SendTCPDataToAll( _packet );
			}
		}

		public static void Disconnect( Player _player )
		{
			using(Packet _packet = new Packet( ( int )ServerPackets.disconnect ) )
			{
				_packet.Write( _player.id );

				SendTCPDataToAll( _packet );
			}
		}

		public static void QueuePlayer( int _id )
		{
			using ( Packet _packet = new Packet( ( int )ServerPackets.queuePlayer ) )
			{
				_packet.Write( _id );

				SendTCPDataToAll( _packet );
			}
		}
	}
}
