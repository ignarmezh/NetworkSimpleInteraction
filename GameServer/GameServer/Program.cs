using System;
using System.Threading;

namespace GameServer
{
	class Program
	{
		private static bool isRunning = false;
		static void Main( string[] args )
		{
			Console.Title = "GameServer";
			isRunning = true;

			Thread mainThread = new Thread( new ThreadStart( MainThread ) );
			mainThread.Start();

			Random _rnd = new Random();

			//по условию до 5ти :)
			Server.Start( _rnd.Next( 1, 6 ), 26950, _rnd.Next( 7, 13 ) );
			//Server.Start( 5, 26950, _rnd.Next( 7, 13 ) );
		}

		private static void MainThread()
		{
			Console.WriteLine( $"Main Thread started. Running at {Constants.TICKS_PER_SEC} ticks per second." );
			DateTime _nextLoop = DateTime.Now;

			while ( isRunning )
			{
				while ( _nextLoop < DateTime.Now )
				{
					GameLogic.Update();

					_nextLoop = _nextLoop.AddMilliseconds( Constants.MS_PER_TICK );

					//fix background thread work
					if ( _nextLoop > DateTime.Now )
					{
						Thread.Sleep( _nextLoop - DateTime.Now );
					}
				}
			}
		}
	}
}
