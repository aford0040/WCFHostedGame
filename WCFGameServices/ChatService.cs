using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace BroadcastorService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class BroadcastorService : IBroadcastorService
    {
        /// <summary>
        /// The list of connected clients
        /// </summary>
        private static Dictionary<string, IBroadcastorCallBack> clients =
            new Dictionary<string, IBroadcastorCallBack>();

        /// <summary>
        /// Used to make stuff thread safe
        /// </summary>
        private static object locker = new object();
        private static readonly object BoardUpdateLock = new object();

        /// <summary>
        /// A 2-d game board represented as 35 rows of 110 characters
        /// </summary>
        public static string[] PlayArea = new string[35];

        /// <summary>
        /// Registers a client with the client name
        /// </summary>
        /// <param name="clientName"> Name of the client to register </param>
        public void RegisterClient(string clientName)
        {
            if (clientName != null && clientName != "")
            {
                try
                {
                    // get the client's callback channel so that the server can communicate with it
                    IBroadcastorCallBack callback =
                        OperationContext.Current.GetCallbackChannel<IBroadcastorCallBack>();
                    // make sure this is thread safe
                    lock (locker)
                    {
                        //remove the old client
                        if (clients.Keys.Contains(clientName))
                            clients.Remove(clientName);

                        /*
                         * TODO: currently does not support people with the same name
                         */

                        // add the client to the list
                        clients.Add(clientName, callback);

                        // create a data type to send back to the server
                        EventDataType TempData = new EventDataType();
                        TempData.ClientName = clientName;
                        TempData.CurrentEvent = Event.Registration;
                        
                        // send the data to the clients and have them handle it
                        NotifyServer(TempData);
                    }
                }
                catch (Exception ex)
                {
                    throw new FaultException(ex.Message);
                }
            }
        }

        /// <summary>
        /// Notifies all of the connected clients of an event
        /// </summary>
        /// <param name="eventData"></param>
        public void NotifyServer(EventDataType eventData)
        {
            lock (locker)
            {
                var inactiveClients = new List<string>();
                foreach (var client in clients)
                {
                        try
                        {
                            // send the event data to the client
                            client.Value.BroadcastToClient(eventData);
                        }
                            // if there's an error, just remove them from the list. Dont have time to sort it out....for now
                        catch (Exception ex)
                        {
                            inactiveClients.Add(client.Key);
                            throw new FaultException(ex.Message);
                        }
                    
                }

                if (inactiveClients.Count > 0)
                {
                    foreach (var client in inactiveClients)
                    {
                        clients.Remove(client);
                    }
                }
            }
        }

        /// <summary>
        /// Add player to the game
        /// </summary>
        /// <param name="clientName"> Name of the player to add</param>
        public void AddPlayer(string clientName)
        {
            IBroadcastorCallBack callback =
                OperationContext.Current.GetCallbackChannel<IBroadcastorCallBack>();
            lock (locker)
            {
                if (clients.Keys.Contains(clientName))
                    clients.Remove(clientName);
                clients.Add(clientName, callback);
            }
        }

        /// <summary>
        /// Update the game board and send the board back to the clients
        /// </summary>
        /// <param name="RecievedGameBoard"> Game board received from the</param>
        public void UpdateGameBoard(string[] RecievedGameBoard)
        {
            lock (BoardUpdateLock)
            {
                // update the play area
                PlayArea = RecievedGameBoard;
            
                EventDataType CurrentData = new EventDataType();
                CurrentData.GameBoard = PlayArea;
                CurrentData.CurrentEvent = Event.UpdateGameBoard;

                // broadcast the new game to all of the clients
                foreach (var client in clients)
                {
                    try
                    {
                        client.Value.BroadcastToClient(CurrentData);
                    }
                    catch (Exception e)
                    {
                        string Text = e.Message;
                    }
                }
            }
        }

        public void GenerateGameBoard()
        {
            GameBoard CurrentGame = new GameBoard();
            EventDataType CurrentData = new EventDataType();
            CurrentData.GameBoard = PlayArea;
            CurrentData.CurrentEvent = Event.UpdateGameBoard;

            foreach (var client in clients)
            {
                try
                {
                    client.Value.BroadcastToClient(CurrentData);
                }
                catch (Exception e)
                {
                    string Text = e.Message;
                }
            }
        }

        /// <summary>
        /// A simple enumerable that specifies what type of event happened.
        /// </summary>
        public enum Event
        {
            Registration = 1,
            SendMessage = 2,
            UpdateGameBoard = 3
        };

    }
}
