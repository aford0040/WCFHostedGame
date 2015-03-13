using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace BroadcastorService
{
    [ServiceContract(CallbackContract = typeof(IBroadcastorCallBack))]
    public interface IBroadcastorService
    {
        /// <summary>
        /// Registers an incoming client to the server
        /// </summary>
        /// <param name="clientName"> The name of the client to register </param>
        [OperationContract(IsOneWay = true)]
        void RegisterClient(string clientName);

        /// <summary>
        /// An event that the server receives. Pushes out event info to each of the clients
        /// </summary>
        /// <param name="eventData"> </param>
        [OperationContract(IsOneWay = true)]
        void NotifyServer(EventDataType eventData);

        /// <summary>
        /// Adds a player to the game board and list of clients
        /// </summary>
        /// <param name="clientName"> The name of the client to add</param>
        [OperationContract(IsOneWay = true)]
        void AddPlayer(string clientName);

        /// <summary>
        /// Generates a game board and sends it to all of the connected clients
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void GenerateGameBoard();

        /// <summary>
        /// Updates the game board and sends it to all of the clients
        /// </summary>
        /// <param name="RecievedGameBoard"> Game board received from a client </param>
        [OperationContract(IsOneWay = true)]
        void UpdateGameBoard(string[] RecievedGameBoard);

    }

    /// <summary>
    /// A data contract to send to the client
    /// </summary>
    [DataContract()]
    public class EventDataType
    {
        /// <summary>
        ///  name of the client
        /// </summary>
        [DataMember]
        public string ClientName { get; set; }

        /// <summary>
        /// event message to send to the rest of the clients
        /// </summary>
        [DataMember]
        public string EventMessage { get; set; }

        /// <summary>
        /// The event to take account for
        /// </summary>
        [DataMember]
        public BroadcastorService.Event CurrentEvent;

        /// <summary>
        /// The game board currently in play
        /// </summary>
        [DataMember]
        public string[] GameBoard = new string[35];
    }

    /// <summary>
    /// The call back that the client's host so that the server can communicate
    /// </summary>
    public interface IBroadcastorCallBack
    {
        [OperationContract(IsOneWay = true)]
        void BroadcastToClient(EventDataType eventData);
    }

    /// <summary>
    /// The game board that the clients are playing on
    /// </summary>
    [DataContract()]
    public class GameBoard
    {
        /// <summary>
        /// Generates a game board with random sections (rooms)
        /// </summary>
        public GameBoard()
        {
            string[,] GameBoard = new string[35,110];
            Random Num = new Random();

            GameBoard = GenerateQuadrants(Num.Next(4, 8));

            // generate a loop to connect all the quadrants
            for (int I = 0; I < 35; I++)
            {
                string GameRow = "";
                for (int J = 0; J < 110; J++)
                    GameRow += GameBoard[I, J];
                BroadcastorService.PlayArea[I] = GameRow;
            }
        }

        /// <summary>
        /// Generate the rooms
        /// </summary>
        /// <param name="NumberOfQuadrants"> Number of quadrants to generate </param>
        /// <returns> the </returns>
        public string[,] GenerateQuadrants(int NumberOfQuadrants)
        {
            string[,] Board = new string[35, 110];
            for (int I = 0; I < 35; I++)
                for (int J = 0; J < 110; J++)
                    Board[I, J] = " ";

            // generate random widths and heights
            Random WidthRandom = new Random();
            Random HeightRandom = new Random();
            string[] Quadrants = new string[NumberOfQuadrants];

            for (int I = 0; I < NumberOfQuadrants; I++)
            {
                int Width = WidthRandom.Next(3, 20);
                int Height = HeightRandom.Next(3, 10);

                // assign them randomly around the board. theyre semi-random
                switch (I)
                {
                    case 0:
                        {
                            for(int J = 0; J < Height; J++)
                            {
                                for (int K = 0; K < Width; K++)
                                {
                                    Board[4 + J, 5 + K] = "#";
                                }
                            }
                            break;
                        }
                    case 1:
                        {
                            for (int J = 0; J < Height; J++)
                            {
                                for (int K = 0; K < Width; K++)
                                {
                                    Board[4 + J, 88 + K] = "#";
                                }
                            }

                            break;
                        }
                    case 2:
                        {
                            for (int J = 0; J < Height; J++)
                            {
                                for (int K = 0; K < Width; K++)
                                {
                                    Board[22 + J, 5 + K] = "#";
                                }
                            }

                            break;
                        }
                    case 3:
                        {
                            for (int J = 0; J < Height; J++)
                            {
                                for (int K = 0; K < Width; K++)
                                {
                                    Board[22 + J, 85 + K] = "#";
                                }
                            }

                            break;
                        }
                    case 4:
                        {
                            for (int J = 0; J < Height; J++)
                            {
                                for (int K = 0; K < Width; K++)
                                {
                                    Board[4 + J, 27 + K] = "#";
                                }
                            }

                            break;
                        }
                    case 5:
                        {
                            for (int J = 0; J < Height; J++)
                            {
                                for (int K = 0; K < Width; K++)
                                {
                                    Board[22 + J, 27 + K] = "#";
                                }
                            }

                            break;
                        }
                    case 6:
                        {
                            for (int J = 0; J < Height; J++)
                            {
                                for (int K = 0; K < Width; K++)
                                {
                                    Board[4 + J, 59 + K] = "#";
                                }
                            }

                            break;
                        }
                    case 7:
                        {
                            for (int J = 0; J < Height; J++)
                            {
                                for (int K = 0; K < Width; K++)
                                {
                                    Board[22 + J, 59 + K] = "#";
                                }
                            }

                            break;
                        }
                }
            }
            
            return Board;
        }
    }
}
