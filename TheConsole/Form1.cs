using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Windows.Forms;


namespace TheConsole
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// The proxy that will communicate with the WCF service.
        /// </summary>
        private BroadcastorService.BroadcastorServiceClient GameServer;

        /// <summary>
        /// The game board that the user sees on the console
        /// </summary>
        public static char[,] GameBoard = new char[35,110];

        /// <summary>
        /// The position of the player currently behind the console (not other players in the game)
        /// </summary>
        public static int[] PlayerPosition = new int[2] {0, 0}; // 0,0 is vertrical position, 0,1 is horizontal position

        /// <summary>
        /// The game board piece that the player is standing on
        /// </summary>
        public char PlayerSpotStandingOn = ' ';
            
        /// <summary>
        /// Some randome game piece I decided to pick and experiment with
        /// </summary>
        public static char PlayerGamePiece = 'D';

        /// <summary>
        /// The interface of WCF callback calls that the client (game console) implements
        /// </summary>
        public class BroadcastorCallback : BroadcastorService.IBroadcastorServiceCallback
        {
            private System.Threading.SynchronizationContext syncContext =
                AsyncOperationManager.SynchronizationContext;
            
            /// <summary>
            /// The event handlers to send a message, notice when someone registers and to update the game board
            /// </summary>
            public static EventHandler _broadcastorMessage;
            public static EventHandler _broadcastRegistration;
            public static EventHandler _updateGameBoard;

            /// <summary>
            /// Need to set the handlers so that the console functions properly
            /// </summary>
            /// <param name="Message"> Indicates a new message being sent to chat </param>
            /// <param name="Registration"> Someone just joined the game </param>
            /// <param name="GameBoard"> The game board needs to be updated</param>
            public void SetHandlers(EventHandler Message, EventHandler Registration, EventHandler GameBoard)
            {
                _broadcastorMessage = Message;
                _broadcastRegistration = Registration;
                _updateGameBoard = GameBoard;
            }

            /// <summary>
            /// The callback from the server that keeps everything updated. All updates go through here
            /// </summary>
            /// <param name="eventData"></param>
            public void BroadcastToClient(BroadcastorService.EventDataType eventData)
            {
                // spin off a new thread with the eventData parameter
                syncContext.Post(new System.Threading.SendOrPostCallback(OnBroadcast),
                       eventData);
            }

            /// <summary>
            /// Synchronizes data out to the other clients and web service
            /// </summary>
            /// <param name="eventData"> The type of event thats happened </param>
            private void OnBroadcast(object eventData)
            {
              //  Since each eventData type HashSet different Properties, We need To make sure its generic
                BroadcastorService.EventDataType TempData = (BroadcastorService.EventDataType)eventData;
                switch (TempData.CurrentEvent)
                {
                    case BroadcastorService.BroadcastorServiceEvent.Registration:
                        {
                            // invoke the registration method
                            _broadcastRegistration.Invoke(eventData, null);
                            break;
                        }
                    case BroadcastorService.BroadcastorServiceEvent.SendMessage:
                        {
                            // invoke the chat message method
                            _broadcastorMessage.Invoke(eventData, null);
                            break;
                        }
                    case BroadcastorService.BroadcastorServiceEvent.UpdateGameBoard:
                        {
                            // need to go through each row of the game board and update it. this is an update from the server
                            for (int I = 0; I < TempData.GameBoard.Length; I++)
                            {                                
                                int StringLength = TempData.GameBoard[I].Length;
                                int BoardPosition = 0;
                             
                                // iterate through the received gameboard and update our local game board
                                foreach (char C in TempData.GameBoard[I])
                                {
                                    GameBoard[I, BoardPosition] = C;
                                    BoardPosition++;
                                }
                            }
                                                       
                            // invoke the method that updates the game board to the screen
                            _updateGameBoard.Invoke(eventData, null);
                            
                            break;
                        }
                    default:
                        MessageBox.Show("Event unrecognized");
                        break;
                }
            }
        }


        /// <summary>
        /// Initializes the form
        /// </summary>
        public Form1()
        {
            InitializeComponent();

            // set the send button to be the accept button so that you can just hit enter to send a message
            this.AcceptButton = SendButton;
            this.UpdateDefaultButton();
        }

        /// <summary>
        /// registers the user with the server. The callback will update the chat window
        /// </summary>
        private void RegisterClient()
        {
            // if the client is null, then its clear of old data
            if ((this.GameServer != null))
            {
                // if it hops in here, it still has old data. Return the resources back to where they need to be
                this.GameServer.Abort();

                // make the client fresh and null so we have a clean slate to work with. Clean slates are better than dirty ones.
                this.GameServer = null;
            }

            // setup the callback channel for the server to communicate with this client
            BroadcastorCallback cb = new BroadcastorCallback();
            cb.SetHandlers(this.HandleBroadcast, this.HandleRegistration, this.UpdateGame);

            System.ServiceModel.InstanceContext context =
                new System.ServiceModel.InstanceContext(cb);
            this.GameServer =
                new BroadcastorService.BroadcastorServiceClient(context);

            try
            {
                // make the calls to register the player and add his game piece to the board
                this.GameServer.RegisterClient(this.txtClientName.Text);
                this.GameServer.AddPlayer(this.txtClientName.Text);
            }
                // with any exceptions, handle them. Needs to be developed a bit more
            catch (FaultException E)
            {
                Form1.HandleException(E.Message);
            }
            catch (Exception E)
            {
                HandleException(E.Message);
            }
        }


        /// <summary>
        /// Handles registration from any other clients thats not this one with the server. Nothing too fancy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void HandleRegistration(object sender, EventArgs e)
        {
            try
            {
                // get the event data from the server and then notify the user who joined
                var eventData = (BroadcastorService.EventDataType)sender;
                this.ChatWindow.Text += string.Format("{0} has joined the converstaion. \r\n", eventData.ClientName);
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Handles a chat message from the server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void HandleBroadcast(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {

            }
            else
            {
                try
                {
                    // get the event data from the broadcast and update the chat window
                    var eventData = (BroadcastorService.EventDataType)sender;
                    
                    // simple house cleaning of the window here.
                    if (this.ChatWindow.Text == "")
                        this.ChatWindow.Text += "\r\n";

                    // add the text to the chat window
                    this.ChatWindow.Text += string.Format("{1}: {0} \r\n",
                        eventData.EventMessage, eventData.ClientName);
                }
                catch (Exception ex)
                {
                }
            }
        }

        /// <summary>
        /// Registers a client with the server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRegisterClient_Click_1(object sender, EventArgs e)
        {
            if (this.txtClientName.Text == "")
            {
                MessageBox.Show(this, "Client Name cannot be empty");
                return;
            }
            this.RegisterClient();
        }

        /// <summary>
        /// Sends a message to the server to broadcast to everyone that's currently connected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendButton_Click(object sender, EventArgs e)
        {
            // if the user never registered with the server, they cant send messages anywhere
            if (this.GameServer == null)
            {
                MessageBox.Show(this, "Client is not registered");
                return;
            }

            // make sure they arent spamming it with empty crap
            if (this.Chat.Text == "")
            {
                MessageBox.Show(this, "Cannot broadcast an empty message");
                return;
            }

            // setup the callback channel for the server to make a callback
            BroadcastorCallback cb = new BroadcastorCallback();
            cb.SetHandlers(this.HandleBroadcast, this.HandleRegistration, this.UpdateGame);
            
            System.ServiceModel.InstanceContext context =
                new System.ServiceModel.InstanceContext(cb);
            this.GameServer =
                new BroadcastorService.BroadcastorServiceClient(context);

            // make the call to the server and send the message
            this.GameServer.NotifyServer(
                new BroadcastorService.EventDataType()
                {
                    ClientName = this.txtClientName.Text,
                    EventMessage = this.Chat.Text,
                    CurrentEvent = BroadcastorService.BroadcastorServiceEvent.SendMessage
                });

            // clear out the chat box
            Chat.Text = string.Empty;
        }

        /// <summary>
        /// Generates a game board on the server to push out to all of the clients
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenerateGameBoard_ButtonClick(object sender, EventArgs e)
        {
            // again, setup the callback channel for the proxy
            BroadcastorCallback cb = new BroadcastorCallback();
            cb.SetHandlers(this.HandleBroadcast, this.HandleRegistration, this.UpdateGame);

            System.ServiceModel.InstanceContext context =
                new System.ServiceModel.InstanceContext(cb);
            this.GameServer =
                new BroadcastorService.BroadcastorServiceClient(context);

            // call the generategameboard service
            this.GameServer.GenerateGameBoard();
        }

        /// <summary>
        /// Updates the game board locally
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateGame(object sender, EventArgs e)
        {
            Console.Clear();
            for (int I = 0; I < 35; I++)
            {
                string TempString = "";
                for (int J = 0; J < 110; J++)
                {
                    TempString += GameBoard[I, J];
                }
                    Console.AppendText(TempString + Environment.NewLine);
            }
        }

        /// <summary>
        /// Handles the key presses for the user to move around
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            /*
             * Note the reason why CallUpdate is in every IF block is because if theyre up against the wall and attempting
             * to move through the wall, no update is needed. Cuts down on network traffic and the game appears faster
             */ 
            switch (e.KeyCode)
            {
                case Keys.Left:
                    if (PlayerPosition[0] > 0)
                    {
                        // put what the player was standing on back where it was
                        GameBoard[PlayerPosition[1], PlayerPosition[0]] = PlayerSpotStandingOn;

                        // adjust the players position
                        PlayerPosition[0]--;

                        // get the piece that the player's position is currently on. The player's position
                        // will be updated int he CallUpdate
                        PlayerSpotStandingOn = GameBoard[PlayerPosition[1], PlayerPosition[0]];

                        CallUpdate();
                    }
                    break;
                case Keys.Up:
                    if (PlayerPosition[1] > 0)
                    {
                        // put what the player was standing on back where it was
                        GameBoard[PlayerPosition[1], PlayerPosition[0]] = PlayerSpotStandingOn;

                        // adjust the player's position
                        PlayerPosition[1]--;

                        // get the piece that the player's position is currently on. The player's position
                        // will be updated int he CallUpdate
                        PlayerSpotStandingOn = GameBoard[PlayerPosition[1], PlayerPosition[0]];

                        CallUpdate();
                    }
                    break;
                case Keys.Right:
                    if (PlayerPosition[0] < 109)
                    {
                        // put what the player was standing on back where it was
                        GameBoard[PlayerPosition[1], PlayerPosition[0]] = PlayerSpotStandingOn;
                        
                        // adjust the player's position
                        PlayerPosition[0]++;

                        // get the piece that the player's position is currently on. The player's position
                        // will be updated int he CallUpdate
                        PlayerSpotStandingOn = GameBoard[PlayerPosition[1], PlayerPosition[0]];

                        CallUpdate();
                    }
                    break;
                case Keys.Down:
                    if (PlayerPosition[1] < 34)
                    {
                        // put what the player was standing on back where it was
                        GameBoard[PlayerPosition[1], PlayerPosition[0]] = PlayerSpotStandingOn;

                        // adjust the player's position
                        PlayerPosition[1]++;

                        // get the piece that the player's position is currently on. The player's position
                        // will be updated int he CallUpdate
                        PlayerSpotStandingOn = GameBoard[PlayerPosition[1], PlayerPosition[0]];

                        CallUpdate();
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Updates the game board on the server to distribute out to all of the connected clients
        /// </summary>
        public void CallUpdate()
        {
            // setup the callback so that there is a callback channel and we can receive the update
            BroadcastorCallback cb = new BroadcastorCallback();
            cb.SetHandlers(this.HandleBroadcast, this.HandleRegistration, this.UpdateGame);

            // get the current context for the callback
            System.ServiceModel.InstanceContext context =
                new System.ServiceModel.InstanceContext(cb);
            this.GameServer =
                new BroadcastorService.BroadcastorServiceClient(context);

            // update a gameboard to send off to the server
            string[] TempGameBoard = new string[35];
            GameBoard[PlayerPosition[1], PlayerPosition[0]] = PlayerGamePiece;
            for (int I = 0; I < 35; I++)
            {
                string TempString = "";
                for (int J = 0; J < 110; J++)
                {
                    TempString += GameBoard[I, J];
                }
                TempGameBoard[I] = TempString;
            }

            // always put the connection attempts with in a try block. Ensures stability
            try
            {
                // send off the gameboard to the server to update
                this.GameServer.UpdateGameBoard(TempGameBoard);
            }
            catch (FaultException E)
            {
                // show the message to the user
                HandleException(E.Message);
            }
        }

        // Handle any exception from a connection and display the message to the user
        public static void HandleException(string p)
        {
            // show the message to the user. Since this is an incredibly rough project, this is mainly for debug purposes.
            Trace.TraceError(p);
            MessageBox.Show(p, ClientResources.ServerConnectionErrorHeader, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

    }
}
