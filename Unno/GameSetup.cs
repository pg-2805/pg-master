using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Unno;
using Newtonsoft.Json;

namespace Unno
{
    class GameSetup
    {
        public static IEnumerable<string> lstCardColours = new List<string> { "Red", "Green", "Yellow", "Blue", "Wild" };
        public static IEnumerable<string> lstCArdNames = new List<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "MissATurn", "TakeTwo", "ChangeDirection", "WildCard", "WildTake4" /*,"WildSwapHands"*/ };

        public List<Players> Players { get; set; } = new List<Players>() {
            new Players() { PlayerId =1,PlayerName="Computer",hasShuffled=true,PlayersDeck=new List<Cards>()},
            new Players() { PlayerId =2,PlayerName="Player",hasShuffled=false,PlayersDeck=new List<Cards>()} // Player name is HARD CODED here. Change to take input from user.
        };

        public List<Cards> DiscardPile { get; set; }
        //public DeckCard DeckPile { get; set; }

        public List<Cards> DeckPile { get; set; }

        public int PlayerCount { get; set; } = 2; // 2 by default,  TO DO: 4 players

        public Players PlayersPlay { get; set; }

        // public int clockWise;

        public string UserSelectedColour { get; set; } = String.Empty;
        public int GameNumber { get; set; } = 1;
        public int LastGameScore { get; set; }
        public string LastGameWinner { get; set; }
        public string SessionPath { get; set; } = String.Empty;

        public GameSetup()
        {
            try
            {

                DeckPile = new List<Cards>();

                foreach (string colour in lstCardColours)
                {

                    foreach (string Cardname in lstCArdNames)
                    {
                        if (colour != "Wild")
                        {
                            if (Cardname == "0")
                            {
                                Cards card = new Cards();
                                card.Colour = colour;
                                card.Name = Cardname;
                                card.Points = 1;
                                DeckPile.Add(card);
                            }
                            else if (Cardname != "WildCard" && Cardname != "WildTake4")
                            {
                                for (int i = 1; i <= 2; i++)
                                {
                                    Cards card = new Cards();
                                    card.Colour = colour;
                                    card.Name = Cardname;
                                    if (Cardname == "Reverse" || Cardname == "TakeTwo"  /*|| Cardname == "ChangeDirection"*/)
                                    {
                                        card.Points = 20;
                                    }
                                    else
                                    {
                                        card.Points = 1;
                                    }
                                    DeckPile.Add(card);
                                }

                            }

                        }
                        else
                        {

                            // loop for no. of cards to be formed. 4 each with colour wild.
                            if (Cardname == "WildCard" || Cardname == "WildTake4")
                            {
                                for (int i = 1; i <= 4; i++)
                                {
                                    Cards card = new Cards();
                                    card.Colour = colour;
                                    card.Name = Cardname;
                                    card.Points = 50;
                                    DeckPile.Add(card);
                                }


                            }


                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            Shuffle(DeckPile);
        }

        // Shuffling the Initial Deck and Discard Pile
        static public void Shuffle(List<Cards> DeckPile)
        {
            int randIndex;
            for (int i = 0; i < DeckPile.Count; i++)
            {
                Random r = new Random();
                randIndex = r.Next(DeckPile.Count - i);
                var temp = DeckPile[i];
                DeckPile[i] = DeckPile[randIndex];
                DeckPile[randIndex] = temp;
            }

        }



        public void DistributeCards(List<Players> lstPlayers, List<Cards> Deck)
        {
            // count number of players and
            PlayerCount = lstPlayers.Count;
            int count = 0;
            int DeckCount = 0;
            while (count < 7)  
            {
                foreach (Players p in lstPlayers)
                {
                    p.PlayersDeck.Add(Deck.First());
                    Deck.Remove(Deck.First());
                }
                DeckCount = DeckPile.Count;
                count++;
            }


        }

        public string[] InitiateDiscardPile(List<Cards> DeckPile)
        {
            string[] InitialDiscardCard = new string[2];
            Cards DiscardPileStart = DeckPile.First();
            DiscardPile = new List<Cards>() { new Cards() { Colour = DiscardPileStart.Colour, Name = DiscardPileStart.Name, Points = DiscardPileStart.Points } };
            // Console.WriteLine("Discard Pile: CardColur {0} CardName {1}", DiscardPile.Last().Colour, DiscardPile.Last().Name);
            Console.WriteLine();
            InitialDiscardCard[0] = DiscardPile.Last().Colour;
            InitialDiscardCard[1] = DiscardPile.Last().Name;
            return InitialDiscardCard;
        }
        public void InitiateDiscardPile()
        {

            if (DeckPile.Count > 1)
            {
                Console.WriteLine();
                Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
                Console.Write("Discard Pile: CardColur {0} | CardName {1} ", DiscardPile.Last().Colour, DiscardPile.Last().Name);
                if (DiscardPile.Last().Colour == "Wild" && (DiscardPile.Last().Name == "WildCard" || DiscardPile.Last().Name == "WildTake4"))
                {
                    Console.Write("| NewColour {0}", (UserSelectedColour != null || UserSelectedColour != "") ? UserSelectedColour : "");
                    Console.WriteLine();
                }
                Console.WriteLine();
                Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
               
            }

        }

        public string CheckWhoShuffled(List<Players> lstPlayers)
        {
            Players ShuffledPlayer = new Players();
            ShuffledPlayer = lstPlayers.Find(p => p.hasShuffled == true);

            return ShuffledPlayer.PlayerName;
        }

        public string PlayersTurnToStartPlay(List<Players> lstPlayers)
        {
            Players Player = new Players();
            Player = lstPlayers.Last();

            return Player.PlayerName;
        }

        // MAKE PLAY CONTINUOUS AND ADD PLAYED CARDS IN DISCARD PILE AFTER EACH PLAY.
        public void Play(List<Players> lstPlayers, string playerWhoPlayed, string UserSetColourInitial/*,List<GameSetup> lstGameObjects*/)
        {
            try
            {
                Players plyr = new Players();
                Players NextPlayer, NextToNextPlayer;
                plyr = lstPlayers[lstPlayers.FindIndex(p => p.PlayerName == playerWhoPlayed)];
                UserSelectedColour = UserSetColourInitial;
                InitiateDiscardPile();
                Cards playedCard = ChooseCardtoPlay(plyr);
                int Player1DeckCount = lstPlayers[0].PlayersDeck.Count;
                int Player2DeckCount = lstPlayers[1].PlayersDeck.Count;
                string[] playedCardStatus;

                while (Player1DeckCount != 0 && Player2DeckCount != 0)
                {
                    //if (!PauseGame() && DiscardPile.Count > 1 )
                    //{
                    playedCardStatus = CheckForSpecialCards(playedCard);
                    if (ValidatePlay(playedCard, DiscardPile, lstPlayers, playerWhoPlayed))
                    {
                        // DETERMINE CARD TYPE AND PLAY ACTION
                        if (playedCardStatus[0] == "false") // Normal flow action
                        {
                            if(playedCard.Name != "Draw")
                            {
                                DiscardPile.Add(playedCard);
                                plyr.PlayersDeck.Remove(playedCard);
                                Player1DeckCount = plyr.PlayersDeck.Count;
                            }
                            //DiscardPile.Add(playedCard);   // last card of discard pile should be the first card of the discard pile.
                            
                            NextPlayer = lstPlayers[(lstPlayers.FindIndex(p => p.PlayerName == playerWhoPlayed) - 1 < 0) ? lstPlayers.Count - 1 : lstPlayers.FindIndex(p => p.PlayerName == playerWhoPlayed) - 1];
                            playerWhoPlayed = NextPlayer.PlayerName;
                            InitiateDiscardPile();
                            Cards newPlayedCard = ChooseCardtoPlay(NextPlayer);
                            playedCard = newPlayedCard;
                            NextPlayer.PlayersDeck.Remove(playedCard);
                            if (NextPlayer.PlayerName == "Computer")
                            {
                                Player1DeckCount = NextPlayer.PlayersDeck.Count;
                            }
                            else
                            {
                                Player2DeckCount = NextPlayer.PlayersDeck.Count;
                            }
                            //Player2DeckCount = NextPlayer.PlayersDeck.Count;
                            //InitiateDiscardPile();
                        }
                        if (playedCardStatus[0] == "true")   // Handle special card actions.
                        {

                            if (playedCardStatus[1] == "MissATurn")
                            {
                                // Next player misses a turn
                                DiscardPile.Add(playedCard);
                                plyr.PlayersDeck.Remove(playedCard);
                                if (plyr.PlayerName == "Computer")
                                {
                                    Player1DeckCount = plyr.PlayersDeck.Count;
                                }
                                else
                                {
                                    Player2DeckCount = plyr.PlayersDeck.Count;
                                }

                                NextPlayer = lstPlayers[(lstPlayers.FindIndex(p => p.PlayerName == playerWhoPlayed) - 1 < 0) ? lstPlayers.Count - 1 : lstPlayers.FindIndex(p => p.PlayerName == playerWhoPlayed) - 1];
                                NextToNextPlayer = lstPlayers[(lstPlayers.FindIndex(p => p.PlayerName == NextPlayer.PlayerName) - 1 < 0) ? lstPlayers.Count - 1 : lstPlayers.FindIndex(p => p.PlayerName == NextPlayer.PlayerName) - 1];
                                playerWhoPlayed = NextToNextPlayer.PlayerName;
                                InitiateDiscardPile();
                                Cards newPlayedCard = ChooseCardtoPlay(NextToNextPlayer);
                                playedCard = newPlayedCard;
                                NextToNextPlayer.PlayersDeck.Remove(playedCard);
                                if (NextToNextPlayer.PlayerName == "Computer")
                                {
                                    Player1DeckCount = NextToNextPlayer.PlayersDeck.Count;
                                }
                                else
                                {
                                    Player2DeckCount = NextToNextPlayer.PlayersDeck.Count;
                                }
                                //InitiateDiscardPile();
                            }
                            else if (playedCardStatus[1] == "TakeTwo")
                            {
                                // Next Player draws 2 cards from deck pile and skips turn

                                DiscardPile.Add(playedCard);
                                plyr.PlayersDeck.Remove(playedCard);
                                if (plyr.PlayerName == "Computer")
                                {
                                    Player1DeckCount = plyr.PlayersDeck.Count;
                                }
                                else
                                {
                                    Player2DeckCount = plyr.PlayersDeck.Count;
                                }

                                NextPlayer = lstPlayers[(lstPlayers.FindIndex(p => p.PlayerName == playerWhoPlayed) - 1 < 0) ? lstPlayers.Count - 1 : lstPlayers.FindIndex(p => p.PlayerName == playerWhoPlayed) - 1];
                                playerWhoPlayed = NextPlayer.PlayerName;
                                DrawCard("TakeTwo", NextPlayer);
                                if (NextPlayer.PlayerName == "Computer")
                                {
                                    Player1DeckCount = NextPlayer.PlayersDeck.Count;
                                }
                                else
                                {
                                    Player2DeckCount = NextPlayer.PlayersDeck.Count;
                                }
                                // SKIP TURN
                                NextToNextPlayer = lstPlayers[(lstPlayers.FindIndex(p => p.PlayerName == NextPlayer.PlayerName) - 1 < 0) ? lstPlayers.Count - 1 : lstPlayers.FindIndex(p => p.PlayerName == NextPlayer.PlayerName) - 1];
                                playerWhoPlayed = NextToNextPlayer.PlayerName;
                                InitiateDiscardPile();
                                Cards newPlayedCard = ChooseCardtoPlay(NextToNextPlayer);
                                playedCard = newPlayedCard;
                                NextToNextPlayer.PlayersDeck.Remove(playedCard);
                                if (NextToNextPlayer.PlayerName == "Computer")
                                {
                                    Player1DeckCount = NextToNextPlayer.PlayersDeck.Count;
                                }
                                else
                                {
                                    Player2DeckCount = NextToNextPlayer.PlayersDeck.Count;
                                }
                            }
                            else if (playedCardStatus[1] == "ChangeDirection")
                            {
                                //  The starting direction of play is
                                //    clockwise.When this card is played, the direction of
                                //    play is reversed.If it is currently clockwise it
                                //    switches to anti-clockwise.If it is anti - clockwise
                                //    then it switches to clockwise.

                                // DONE ONLY FOR 2 PLAYERS AS OF NOW : SAME AS MISSATURN CARD FOR 2 PLAYERS
                                DiscardPile.Add(playedCard);
                                plyr.PlayersDeck.Remove(playedCard);
                                if (plyr.PlayerName == "Computer")
                                {
                                    Player1DeckCount = plyr.PlayersDeck.Count;
                                }
                                else
                                {
                                    Player2DeckCount = plyr.PlayersDeck.Count;
                                }

                                NextPlayer = lstPlayers[(lstPlayers.FindIndex(p => p.PlayerName == playerWhoPlayed) - 1 < 0) ? lstPlayers.Count - 1 : lstPlayers.FindIndex(p => p.PlayerName == playerWhoPlayed) - 1];
                                NextToNextPlayer = lstPlayers[(lstPlayers.FindIndex(p => p.PlayerName == NextPlayer.PlayerName) - 1 < 0) ? lstPlayers.Count - 1 : lstPlayers.FindIndex(p => p.PlayerName == NextPlayer.PlayerName) - 1];
                                playerWhoPlayed = NextToNextPlayer.PlayerName;
                                InitiateDiscardPile();
                                Cards newPlayedCard = ChooseCardtoPlay(NextToNextPlayer);
                                playedCard = newPlayedCard;
                                NextToNextPlayer.PlayersDeck.Remove(playedCard);
                                if (NextToNextPlayer.PlayerName == "Computer")
                                {
                                    Player1DeckCount = NextToNextPlayer.PlayersDeck.Count;
                                }
                                else
                                {
                                    Player2DeckCount = NextToNextPlayer.PlayersDeck.Count;
                                }
                                // InitiateDiscardPile();



                            }
                            else if (playedCardStatus[1] == "WildCard")
                            {
                                // If a player has a wild card, then they can play that
                                // instead.The wild card lets the person that played it
                                // choose a new colour to continue with, replacing the previous colour
                                DiscardPile.Add(playedCard);
                                plyr.PlayersDeck.Remove(playedCard);
                                if (plyr.PlayerName == "Computer")
                                {
                                    Player1DeckCount = plyr.PlayersDeck.Count;
                                }
                                else
                                {
                                    Player2DeckCount = plyr.PlayersDeck.Count;
                                }
                                Console.WriteLine();
                                Console.WriteLine("You played an Wild Card, Please select the new Colour [Red,Blue,Yellow,Green]");
                                UserSelectedColour = Convert.ToString(Console.ReadLine());

                                NextPlayer = lstPlayers[(lstPlayers.FindIndex(p => p.PlayerName == playerWhoPlayed) - 1 < 0) ? lstPlayers.Count - 1 : lstPlayers.FindIndex(p => p.PlayerName == playerWhoPlayed) - 1];
                                playerWhoPlayed = NextPlayer.PlayerName;
                                InitiateDiscardPile();
                                Cards newPlayedCard = ChooseCardtoPlay(NextPlayer);
                                playedCard = newPlayedCard;
                                NextPlayer.PlayersDeck.Remove(playedCard);
                                if (NextPlayer.PlayerName == "Computer")
                                {
                                    Player1DeckCount = NextPlayer.PlayersDeck.Count;
                                }
                                else
                                {
                                    Player2DeckCount = NextPlayer.PlayersDeck.Count;
                                }
                                //InitiateDiscardPile();



                            }
                            else if (playedCardStatus[1] == "WildTake4")
                            {
                                /*
                                The take 4 card may only be played when the player
                                does not have a card of current colour in their hand. The
                                player who played the card may now choose a new
                                colour to continue with (just like a wild card).
                                Additionally the next player must take 4 cards from the
                                draw pile and skip their turn. 
                                */

                                // HANDLE WILD COLOUR : SET condition.
                                DiscardPile.Add(playedCard);
                                plyr.PlayersDeck.Remove(playedCard);
                                if (plyr.PlayerName == "Computer")
                                {
                                    Player1DeckCount = plyr.PlayersDeck.Count;
                                }
                                else
                                {
                                    Player2DeckCount = plyr.PlayersDeck.Count;
                                }
                                Console.WriteLine();
                                Console.WriteLine("You played an Wild Card, Please select the new Colour [Red,Blue,Yellow,Green]");
                                UserSelectedColour = Convert.ToString(Console.ReadLine());

                                NextPlayer = lstPlayers[(lstPlayers.FindIndex(p => p.PlayerName == playerWhoPlayed) - 1 < 0) ? lstPlayers.Count - 1 : lstPlayers.FindIndex(p => p.PlayerName == playerWhoPlayed) - 1];
                                playerWhoPlayed = NextPlayer.PlayerName;
                                DrawCard("WildTake4", NextPlayer);
                                if (NextPlayer.PlayerName == "Computer")
                                {
                                    Player1DeckCount = NextPlayer.PlayersDeck.Count;
                                }
                                else
                                {
                                    Player2DeckCount = NextPlayer.PlayersDeck.Count;
                                }
                                // SKIP TURN
                                NextToNextPlayer = lstPlayers[(lstPlayers.FindIndex(p => p.PlayerName == NextPlayer.PlayerName) - 1 < 0) ? lstPlayers.Count - 1 : lstPlayers.FindIndex(p => p.PlayerName == NextPlayer.PlayerName) - 1];
                                playerWhoPlayed = NextToNextPlayer.PlayerName;
                                InitiateDiscardPile();
                                Cards newPlayedCard = ChooseCardtoPlay(NextToNextPlayer);
                                playedCard = newPlayedCard;
                                NextToNextPlayer.PlayersDeck.Remove(playedCard);
                                if (NextToNextPlayer.PlayerName == "Computer")
                                {
                                    Player1DeckCount = NextToNextPlayer.PlayersDeck.Count;
                                }
                                else
                                {
                                    Player2DeckCount = NextToNextPlayer.PlayersDeck.Count;
                                }

                            }
                        }
                    }
                    else
                    {

                        // Cant play -- Display
                        // penalties
                        
                        Console.WriteLine("Invalid Card Played. Try again");
                        InitiateDiscardPile();
                        Cards newPlayedCard = ChooseCardtoPlay(Players.Find(p => p.PlayerName == playerWhoPlayed));
                        playedCard = newPlayedCard;
                    }
                    // }
                    //else
                    //{
                    //    // WRITE ALL VARIABLE VALUES TO FILE    -- SAVING SESSION STATE




                    //    string sDeckPile = JsonConvert.SerializeObject(DeckPile);
                    //    string sDiscardPile = JsonConvert.SerializeObject(DiscardPile);
                    //    string sListofPlayers = JsonConvert.SerializeObject(lstPlayers);
                    //    string sPlayerwhoPlayed = JsonConvert.SerializeObject(playerWhoPlayed);

                    //    string CurrentAppPath=Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory); // APPLICATION DEBUG FOLDER
                    //    string FileName = "SessionData_"+DateTime.Now.ToString("ddmmyyyy");
                    //    string path = CurrentAppPath + "\\" + FileName;
                    //    string sFileName = path + "\\SessionState.txt";
                    //    if (!Directory.Exists(path))
                    //    {
                    //        Directory.CreateDirectory(path);
                    //        if (!File.Exists(sFileName))
                    //        {
                    //            File.Create(sFileName);
                    //        }
                    //    }
                    //    else
                    //    {

                    //        if (!File.Exists(sFileName))
                    //        {
                    //            File.Create(sFileName);
                    //        }
                    //    }
                    //    FileStream fileStream = new FileStream(sFileName, FileMode.Open);

                    //        using (StreamWriter writer = new StreamWriter(fileStream))
                    //        {
                    //            writer.WriteLine("DECK PILE: {0}", sDiscardPile);
                    //            writer.WriteLine("DISCARD PILE: {0}", sDiscardPile);
                    //            writer.WriteLine("PLAYER LIST: {0}", lstPlayers);
                    //            writer.WriteLine("CURRENT PLAYER NAME: {0}", playerWhoPlayed);
                    //            writer.Flush();
                    //            writer.Close();
                    //        }
                    //}

                } // WHILE END

                // DECLARE WINNER AND CALCULATE POINTS
                if (lstPlayers[0].PlayersDeck.Count == 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("Winner : {0}", lstPlayers[0].PlayerName);
                    CalculatePoints(lstPlayers[0], lstPlayers);
                    LastGameWinner = lstPlayers[0].PlayerName;
                    LastGameScore = lstPlayers[0].PointsScored;
                    Console.WriteLine();
                    Console.WriteLine("Total Points Scored in this game: {0}", lstPlayers[0].PointsScored);
                }
                else if (lstPlayers[1].PlayersDeck.Count == 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("Winner : {0}", lstPlayers[1].PlayerName);
                    CalculatePoints(lstPlayers[1], lstPlayers);
                    LastGameWinner = lstPlayers[1].PlayerName;
                    LastGameScore = lstPlayers[1].PointsScored;
                    Console.WriteLine();
                    Console.WriteLine("Total Points Scored in this game: {0}", lstPlayers[1].PointsScored);
                } // Continue if-else-if for the number of players
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Cards ChooseCardtoPlay(Players Player)
        {
            try
            {
                int PlayerChoosedCardIndex;
                ShowPlayersDeck(Player.PlayersDeck, Player.PlayerName);
                Console.Write("Choose your Card (as 1/2/3..) : " );                
                PlayerChoosedCardIndex = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
                if (Player.PlayersDeck.Count == 1)
                {
                    if (ValidatePlay(Player.PlayersDeck[0], DiscardPile, Players, Player.PlayerName))
                    {
                        return Player.PlayersDeck[PlayerChoosedCardIndex - 1];
                    }
                    else
                    {
                        Console.WriteLine("Invalid Card Played. Try again");
                        InitiateDiscardPile();
                        ShowPlayersDeck(Player.PlayersDeck, Player.PlayerName);
                        Console.Write("Choose your Card (as 1/2/3..) : ");
                        PlayerChoosedCardIndex = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
                        return Player.PlayersDeck[PlayerChoosedCardIndex - 1];                       
                        
                    }

                }
                if (PlayerChoosedCardIndex == 99)
                {
                    // DRAW CARD FROM DECK PILE
                    // int validateDraw = Player.PlayersDeck.FindIndex(c => c.Colour == ((DiscardPile.Last().Colour == "Wild")? UserSelectedColour : DiscardPile.Last().Colour ) || c.Name == DiscardPile.Last().Name );
                    
                    // if(validateDraw == -1)
                    //{
                        DrawCard("99", Player.PlayerName);
                        if (Player.PlayersDeck.Last().Colour == DiscardPile.Last().Colour || Player.PlayersDeck.Last().Name == DiscardPile.Last().Name)
                        {
                            return Player.PlayersDeck.Last();
                        }
                        else
                        {
                        // NEXT PLAYER SHOULD PLAY : TODO
                        return new Cards() { Colour="Draw" , Name="Draw", Points=0};
                        }
                    //}return
                    //else
                    //{
                    //    Console.WriteLine("Invalid Play: Can not draw as you have card");
                    //    InitiateDiscardPile();
                    //    ShowPlayersDeck(Player.PlayersDeck, Player.PlayerName);
                    //    Console.Write("Choose your Card (as 1/2/3..) : ");
                    //    PlayerChoosedCardIndex = Convert.ToInt32(Console.ReadLine());
                    //    Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
                    //    return Player.PlayersDeck[PlayerChoosedCardIndex - 1];
                    //}
                    

                }
               
                    return Player.PlayersDeck[PlayerChoosedCardIndex - 1];
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        void ShowPlayersDeck(List<Cards> PlayersDeck, string PlayerName)
        {
            int count = 0;
            Console.WriteLine();
            Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
            Console.WriteLine("PLAYER: {0}", PlayerName);
            Console.WriteLine();
            if (PlayersDeck.Count == 2)
            {
                Console.WriteLine(PlayerName + "  Calls:  UNO");
                Console.WriteLine();
            }
            foreach (Cards c in PlayersDeck)
            {
                Console.Write("{0}  ", ++count);
                Console.Write("CardsColour: {0}  CardName: {1}  CardPoint: {2} \n", c.Colour, c.Name, c.Points);
                Console.WriteLine();

            }
            Console.WriteLine("99 Draw Card");
            Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
            Console.WriteLine();

        }

        bool ValidatePlay(Cards PlayedCard, List<Cards> DiscardPile, List<Players> lstPlayers, string playerWhoPlayed)
        {

            Cards topCardofDiscardPile = DiscardPile[DiscardPile.Count - 1];
            if (UserSelectedColour != null && PlayedCard.Colour == UserSelectedColour)
            {
                return true;
            }
            else if (DiscardPile.Last().Colour == "Wild" && PlayedCard.Name == "WildTake4")
            {
                Players Playerobj = lstPlayers.Find(p => p.PlayerName == playerWhoPlayed);
                int index = Playerobj.PlayersDeck.FindIndex(c => c.Colour == UserSelectedColour);                 
                if (index == 0 || index== -1)
                {
                    return true;
                }
            }
            else if (DiscardPile.Last().Colour != "Wild" && PlayedCard.Name == "WildTake4")
            {
                Players Playerobj = lstPlayers.Find(p => p.PlayerName == playerWhoPlayed);
                int index = Playerobj.PlayersDeck.FindIndex(c => c.Colour == DiscardPile.Last().Colour);
                if (index == -1 || index== 0)
                {
                    return true;
                }
            }
            else if (PlayedCard.Colour == "Wild" && PlayedCard.Name == "WildCard")
            {
                return true;
            }
            else if (PlayedCard.Colour == topCardofDiscardPile.Colour)
            {
                return true;
            }
            else if (PlayedCard.Colour ==  "Draw" && PlayedCard.Name == "Draw")
            {
                return true;
            }
            else
            {
                if (PlayedCard.Name == topCardofDiscardPile.Name)
                {
                    return true;
                }
            }
            return false;
        }



        string[] CheckForSpecialCards(Cards card)
        {
            string[] cardStatus = new string[2];
            try
            {
                List<string> normalCards = new List<string>() { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9","Draw" };
                if (normalCards.Contains(card.Name))
                {
                    cardStatus[0] = "false";
                    cardStatus[1] = "Normal Card";
                }
                else
                {
                    if (card.Name == "MissATurn")
                    {
                        cardStatus[0] = "true";
                        cardStatus[1] = "MissATurn";
                    }
                    else if (card.Name == "TakeTwo")
                    {
                        cardStatus[0] = "true";
                        cardStatus[1] = "TakeTwo";
                    }
                    else if (card.Name == "ChangeDirection")
                    {
                        cardStatus[0] = "true";
                        cardStatus[1] = "ChangeDirection";
                    }
                    else if (card.Name == "WildCard")
                    {
                        cardStatus[0] = "true";
                        cardStatus[1] = "WildCard";
                    }
                    else if (card.Name == "WildTake4")
                    {
                        cardStatus[0] = "true";
                        cardStatus[1] = "WildTake4";
                    }

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return cardStatus;
        }

        void DrawCard(string DrawType, Players player)
        {
            if (DrawType == "TakeTwo")
            {
                player.PlayersDeck.Add(DeckPile.Last());
                DeckPile.Remove(DeckPile.Last());
                player.PlayersDeck.Add(DeckPile.Last());
                DeckPile.Remove(DeckPile.Last());
            }
            else if (DrawType == "99")
            {

                player.PlayersDeck.Add(DeckPile.Last());
                DeckPile.Remove(DeckPile.Last());

            }
            else if (DrawType == "WildTake4")
            {
                for (int i = 0; i < 4; i++)
                {
                    player.PlayersDeck.Add(DeckPile.Last());
                    DeckPile.Remove(DeckPile.Last());
                }

            }
        }

        public void DrawCard(string DrawType, string startingPlayerName)
        {
            Players startingPlayer = Players.Find(p => p.PlayerName == startingPlayerName);
            if (DrawType == "TakeTwo")
            {
                startingPlayer.PlayersDeck.Add(DeckPile.Last());
                DeckPile.Remove(DeckPile.Last());
                startingPlayer.PlayersDeck.Add(DeckPile.Last());
                DeckPile.Remove(DeckPile.Last());
            }
            else if (DrawType == "99")
            {
                
                startingPlayer.PlayersDeck.Add(DeckPile.Last());
                DeckPile.Remove(DeckPile.Last());

            }
            else if (DrawType == "WildTake4")
            {
                for (int i = 0; i < 4; i++)
                {
                    startingPlayer.PlayersDeck.Add(DeckPile.Last());
                    DeckPile.Remove(DeckPile.Last());
                }
            }
        }

        void CalculatePoints(Players players, List<Players> lstofPlayers)
        {
            int PointsScored = 0;
            foreach (Players p in lstofPlayers)
            {
                if (p.PlayerName != players.PlayerName)
                {
                    p.PlayersDeck.ForEach(c => PointsScored += c.Points);
                }
            }

            players.PointsScored = PointsScored;

        }


        bool PauseGame()
        {
            Console.WriteLine("DO YOU WANT TO PAUSE GAME ? ENTER 'Y' FOR YES 'N' for NO");
            string input = Console.ReadLine().ToString();
            if (input == "Y")
            {
                return true;
            }
            return false;
        }

    }
}
