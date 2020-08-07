using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;

namespace GameServer
{
	public class PathNode
	{
		// Координаты точки на карте
		public Vector2 Position { get; set; }
		// Длина пути от старта
		public int PathLengthFromStart { get; set; }
		// Точка, из которой пришли в эту точку
		public PathNode CameFrom { get; set; }
		// Примерное расстояние до цели
		public int HeuristicEstimatePathLength { get; set; }
		// Ожидаемое полное расстояние до цели
		public int EstimateFullPathLength
		{
			get
			{
				return this.PathLengthFromStart + this.HeuristicEstimatePathLength;
			}
		}
	}
	class Route
	{
		public static List<Vector2> FindPath( int[,] field, Vector2 start, Vector2 goal )
		{
			// Шаг 1.
			var closedSet = new Collection<PathNode>();
			var openSet = new Collection<PathNode>();
			// Шаг 2.
			PathNode startNode = new PathNode()
			{
				Position = start,
				CameFrom = null,
				PathLengthFromStart = 0,
				HeuristicEstimatePathLength = ( int )GetHeuristicPathLength( start, goal )
			};
			openSet.Add( startNode );
			while ( openSet.Count > 0 )
			{
				// Шаг 3.
				var currentNode = openSet.OrderBy( node =>
				   node.EstimateFullPathLength ).First();
				// Шаг 4.
				if ( currentNode.Position == goal )
					return GetPathForNode( currentNode );
				// Шаг 5.
				openSet.Remove( currentNode );
				closedSet.Add( currentNode );
				// Шаг 6.
				foreach ( var neighbourNode in GetNeighbours( currentNode, goal, field ) )
				{
					// Шаг 7.
					if ( closedSet.Count( node => node.Position == neighbourNode.Position ) > 0 )
						continue;
					var openNode = openSet.FirstOrDefault( node =>
					   node.Position == neighbourNode.Position );
					// Шаг 8.
					if ( openNode == null )
						openSet.Add( neighbourNode );
					else
					  if ( openNode.PathLengthFromStart > neighbourNode.PathLengthFromStart )
					{
						// Шаг 9.
						openNode.CameFrom = currentNode;
						openNode.PathLengthFromStart = neighbourNode.PathLengthFromStart;
					}
				}
			}
			// Шаг 10.
			return null;
		}
		private static int GetDistanceBetweenNeighbours()
		{
			return 1;
		}
		private static float GetHeuristicPathLength( Vector2 from, Vector2 to )
		{
			return MathF.Abs( from.X - to.X ) + Math.Abs( from.Y - to.Y );
		}

		private static Collection<PathNode> GetNeighbours( PathNode pathNode, Vector2 goal, int[,] field )
		{
			var result = new Collection<PathNode>();

			// Соседними точками являются соседние по стороне клетки.
			Vector2[] neighbourPoints = new Vector2[4];
			neighbourPoints[0] = new Vector2( pathNode.Position.X + 1, pathNode.Position.Y );
			neighbourPoints[1] = new Vector2( pathNode.Position.X - 1, pathNode.Position.Y );
			neighbourPoints[2] = new Vector2( pathNode.Position.X, pathNode.Position.Y + 1 );
			neighbourPoints[3] = new Vector2( pathNode.Position.X, pathNode.Position.Y - 1 );

			foreach ( var point in neighbourPoints )
			{
				// Проверяем, что не вышли за границы карты.
				if ( point.X < 0 || point.X >= field.GetLength( 0 ) )
					continue;
				if ( point.Y < 0 || point.Y >= field.GetLength( 1 ) )
					continue;
				// Проверяем, что по клетке можно ходить.
				if ( ( field[( int )point.X, ( int )point.Y] != 0 ) && ( field[( int )point.X, ( int )point.Y] != 1 ) )
					continue;
				// Заполняем данные для точки маршрута.
				var neighbourNode = new PathNode()
				{
					Position = point,
					CameFrom = pathNode,
					PathLengthFromStart = pathNode.PathLengthFromStart +
					GetDistanceBetweenNeighbours(),
					HeuristicEstimatePathLength = ( int )GetHeuristicPathLength( point, goal )
				};
				result.Add( neighbourNode );
			}
			return result;
		}
		private static List<Vector2> GetPathForNode( PathNode pathNode )
		{
			var result = new List<Vector2>();
			var currentNode = pathNode;
			while ( currentNode != null )
			{
				result.Add( currentNode.Position );
				currentNode = currentNode.CameFrom;
			}
			result.Reverse();
			return result;
		}
	}
}
