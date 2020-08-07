using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace GameServer
{
	class Player
	{
		public int id;
		public string username;

		public Vector3 position;
		public Quaternion rotation;

		public Player( int _id, string _username, Vector3 _spawnPosition )
		{
			id = _id;
			username = _username;
			position = _spawnPosition;
			rotation = Quaternion.Identity;
		}

		public void Update()
		{

		}

		public void SetTargetPosition( Vector3 _targetPos )
		{
			WayToTargetPoint( _targetPos );
		}

		private void WayToTargetPoint( Vector3 _targetPos )
		{
			int[,] field = new int[Server.SizeOfField, Server.SizeOfField];
			for ( int i = 0; i < Server.SizeOfField; i++ )
			{
				for ( int j = 0; j < Server.SizeOfField; j++ )
				{
					field[i, j] = 0;
				}
			}
			foreach ( var _client in Server.clients.Values )
			{
				if ( _client.player != null )
				{
					field[( int )_client.player.position.X, ( int )_client.player.position.Y] = 2;
					if ( ( int )_client.player.position.X == _targetPos.X && ( int )_client.player.position.Y == _targetPos.Y )
						field[( int )_client.player.position.X, ( int )_client.player.position.Y] = 1;
				}
			}
			List<Vector2> route = Route.FindPath( field, new Vector2( position.X, position.Y ), new Vector2( _targetPos.X, _targetPos.Y ) );

			//удаление последних вершин если они совпадают с положением другого клиента
			for ( int i = 1; i <= Server.clients.Values.Count; i++ )
			{
				if ( Server.clients[i].player != null )
				{
					if ( Server.clients[i].player.position.X == route[route.Count - 1].X && Server.clients[i].player.position.Y == route[route.Count - 1].Y )
					{
						route.RemoveAt( route.Count - 1 );
						i = 1;
					}
				}

			}

			//сохранение позиции пользователя
			position = new Vector3( route[route.Count - 1].X, route[route.Count - 1].Y, 0 );

			ServerSend.Route( this, route );
		}
	}
}
