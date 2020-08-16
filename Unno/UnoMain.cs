using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Unno
{
    class UnoMain
    {
        static void Main(string[] args)
        {
            UnoMain objUnoMain = new UnoMain();
            

            /// TO DO: WHEN SESSIONDATA PRESENT & RESUME PLAY, NO GAMESETUP OBJECT WILL BE CREATED. LOAD EVERYTHING FROM FILE AND CONTINUE
            
            GameSetup gmSetup = new GameSetup();
            // Console.WriteLine("Initial Deck Pile: ");
            // ShowDeckPile(gmSetup.DeckPile);
            
            // objUnoMain.ResumeGame();
            objUnoMain.StartGame(gmSetup);
            


            Console.ReadLine();

        }
        public void StartGame(GameSetup gmSetup)
        {
            
            
            int ContinueGame=1, gameCounts=1, ComputerTotalScore=0, PlayerTotalScore = 0;
            List<GameSetup> lstGameObjs = new List<GameSetup>();
            List<Players> lstPlayerList = gmSetup.Players;
            while(ContinueGame == 1 && gameCounts >= 1)
            {
                
                if(gameCounts == 1)
                {
                    Players NextPlayer;
                    string[] InitialDicardDeckCard = new string[2];
                    gmSetup.DistributeCards(gmSetup.Players, gmSetup.DeckPile); // distribute 7 cards among players
                    string ShuffledPlayer = gmSetup.CheckWhoShuffled(gmSetup.Players); // USE THIS FOR MORE THAN 2 PLAYERS TO START PLAY AND LINK WITH BELOW FUNCTION CALL.
                    string Player = gmSetup.PlayersTurnToStartPlay(gmSetup.Players); // CODED FOR 2 PLAYERS
                    
                    InitialDicardDeckCard = gmSetup.InitiateDiscardPile(gmSetup.DeckPile);              // turn topmost card in DECKPILE to start game with same colour or Name
                    if(InitialDicardDeckCard[1] == "WildCard")
                    {
                        // PLAYER TO PLAY FIRST CHOOSES COLOUR OF THE GAME AND NEXT PLAYERS TURN
                        // string playerToStartPlay = Player;
                        Console.WriteLine("Initial Discard Pile's first card seems to be a WildCard, Please set colour to start play.");
                        string setColour=Convert.ToString(Console.ReadLine());
                        while (setColour != "Red" || setColour != "Blue" || setColour != "Yellow" || setColour != "Green")
                        {
                            Console.WriteLine("Invalid Colour Input");
                            setColour = Convert.ToString(Console.ReadLine());
                        }
                        gmSetup.Play(gmSetup.Players, Player, setColour);
                        lstGameObjs.Add(gmSetup);
                        gameCounts++;

                    }
                    else if(InitialDicardDeckCard[1] == "WildTake4")
                    {
                        // PLAYER TO PLAY FIRST CHOOSES COLOUR, TAKE 4 AND SKIP
                        // string playerToStartPlay = Player;
                        Console.WriteLine("Initial Discard Pile's first card seems to be a WildTake4, Please set colour to start play.");
                        string setColour = Convert.ToString(Console.ReadLine());
                        while (setColour != "Red" || setColour != "Blue" || setColour != "Yellow" || setColour != "Green")
                        {
                            Console.WriteLine("Invalid Colour Input");
                            setColour = Convert.ToString(Console.ReadLine());
                        }
                        gmSetup.DrawCard("WildTake4", Player);
                        NextPlayer = lstPlayerList[(lstPlayerList.FindIndex(p => p.PlayerName == Player) - 1 < 0) ? lstPlayerList.Count - 1 : lstPlayerList.FindIndex(p => p.PlayerName == Player) - 1];
                        gmSetup.Play(gmSetup.Players, NextPlayer.PlayerName, setColour);
                        lstGameObjs.Add(gmSetup);
                        gameCounts++;
                    }
                    else if (InitialDicardDeckCard[0] != "Wild" && InitialDicardDeckCard[1] == "MissATurn")
                    {
                        // PLAYER TO PLAY FIRST MISSES TURN
                        // string playerToStartPlay = Player;
                        NextPlayer = lstPlayerList[(lstPlayerList.FindIndex(p => p.PlayerName == Player) - 1 < 0) ? lstPlayerList.Count - 1 : lstPlayerList.FindIndex(p => p.PlayerName == Player) - 1];
                        gmSetup.Play(gmSetup.Players, NextPlayer.PlayerName,InitialDicardDeckCard[0]);
                        lstGameObjs.Add(gmSetup);
                        gameCounts++;
                    }
                    else if (InitialDicardDeckCard[0] != "Wild" && InitialDicardDeckCard[1] == "TakeTwo")
                    {
                        // PLAYER TO PLAY FIRST DRAWS TWO CARDS AND SKIPS TURN
                        // string playerToStartPlay = Player;
                        gmSetup.DrawCard("TakeTwo", Player);
                        NextPlayer = lstPlayerList[(lstPlayerList.FindIndex(p => p.PlayerName == Player) - 1 < 0) ? lstPlayerList.Count - 1 : lstPlayerList.FindIndex(p => p.PlayerName == Player) - 1];
                        gmSetup.Play(gmSetup.Players, NextPlayer.PlayerName, InitialDicardDeckCard[0]);
                        lstGameObjs.Add(gmSetup);
                        gameCounts++;

                    }
                    else if (InitialDicardDeckCard[0] != "Wild" && InitialDicardDeckCard[1] == "ChangeDirection")
                    {
                        // 2 PLAYER SCENARIO: // PLAYER TO PLAY SKIPS TURN
                        // string playerToStartPlay = Player;
                        NextPlayer = lstPlayerList[(lstPlayerList.FindIndex(p => p.PlayerName == Player) - 1 < 0) ? lstPlayerList.Count - 1 : lstPlayerList.FindIndex(p => p.PlayerName == Player) - 1];
                        gmSetup.Play(gmSetup.Players, NextPlayer.PlayerName, InitialDicardDeckCard[0]);
                        lstGameObjs.Add(gmSetup);
                        gameCounts++;
                    }
                    else
                    {
                        
                        gmSetup.Play(gmSetup.Players, Player, InitialDicardDeckCard[0]);
                        lstGameObjs.Add(gmSetup);
                        gameCounts++;
                    }
                }
                else
                {
                    // string[] DicardDeckCard = new string[2];
                    //GameSetup gameSetupNew = new GameSetup();
                    //gameSetupNew.DistributeCards(gameSetupNew.Players, gameSetupNew.DeckPile);
                    //gameSetupNew.InitiateDiscardPile(gameSetupNew.DeckPile);
                    //string PlayertoStartNewGame = lstGameObjs.Last().LastGameWinner;
                    //gameSetupNew.Play(gameSetupNew.Players, PlayertoStartNewGame,"");
                    //lstGameObjs.Add(gameSetupNew);
                    //gameCounts++;


                    Players NextPlayer;
                    string[] InitialDicardDeckCard = new string[2];
                    GameSetup gameSetupNew = new GameSetup();
                    gameSetupNew.DistributeCards(gameSetupNew.Players, gameSetupNew.DeckPile); 
                    string Player = lstGameObjs.Last().LastGameWinner; 
                    InitialDicardDeckCard = gameSetupNew.InitiateDiscardPile(gameSetupNew.DeckPile);  
                    if (InitialDicardDeckCard[1] == "WildCard")
                    {
                        // PLAYER TO PLAY FIRST CHOOSES COLOUR OF THE GAME AND NEXT PLAYERS TURN
                        // string playerToStartPlay = Player;
                        Console.WriteLine("Initial Discard Pile's first card seems to be a WildCard, Please set colour to start play.");
                        string setColour = Convert.ToString(Console.ReadLine());
                        while (setColour != "Red" || setColour != "Blue" || setColour != "Yellow" || setColour != "Green")
                        {
                            Console.WriteLine("Invalid Colour Input");
                            setColour = Convert.ToString(Console.ReadLine());
                        }
                        gameSetupNew.Play(gameSetupNew.Players, Player, setColour);
                        lstGameObjs.Add(gameSetupNew);
                        gameCounts++;

                    }
                    else if (InitialDicardDeckCard[1] == "WildTake4")
                    {
                        // PLAYER TO PLAY FIRST CHOOSES COLOUR, TAKE 4 AND SKIP
                        // string playerToStartPlay = Player;
                        Console.WriteLine("Initial Discard Pile's first card seems to be a WildTake4, Please set colour to start play.");
                        string setColour = Convert.ToString(Console.ReadLine());
                        while (setColour != "Red" || setColour != "Blue" || setColour != "Yellow" || setColour != "Green")
                        {
                            Console.WriteLine("Invalid Colour Input");
                            setColour = Convert.ToString(Console.ReadLine());
                        }
                        gameSetupNew.DrawCard("WildTake4", Player);
                        NextPlayer = lstPlayerList[(lstPlayerList.FindIndex(p => p.PlayerName == Player) - 1 < 0) ? lstPlayerList.Count - 1 : lstPlayerList.FindIndex(p => p.PlayerName == Player) - 1];
                        gameSetupNew.Play(gameSetupNew.Players, NextPlayer.PlayerName, setColour);
                        lstGameObjs.Add(gameSetupNew);
                        gameCounts++;
                    }
                    else if (InitialDicardDeckCard[0] != "Wild" && InitialDicardDeckCard[1] == "MissATurn")
                    {
                        // PLAYER TO PLAY FIRST MISSES TURN
                        // string playerToStartPlay = Player;
                        NextPlayer = lstPlayerList[(lstPlayerList.FindIndex(p => p.PlayerName == Player) - 1 < 0) ? lstPlayerList.Count - 1 : lstPlayerList.FindIndex(p => p.PlayerName == Player) - 1];
                        gameSetupNew.Play(gameSetupNew.Players, NextPlayer.PlayerName, InitialDicardDeckCard[0]);
                        lstGameObjs.Add(gameSetupNew);
                        gameCounts++;
                    }
                    else if (InitialDicardDeckCard[0] != "Wild" && InitialDicardDeckCard[1] == "TakeTwo")
                    {
                        // PLAYER TO PLAY FIRST DRAWS TWO CARDS AND SKIPS TURN
                        // string playerToStartPlay = Player;
                        gameSetupNew.DrawCard("TakeTwo", Player);
                        NextPlayer = lstPlayerList[(lstPlayerList.FindIndex(p => p.PlayerName == Player) - 1 < 0) ? lstPlayerList.Count - 1 : lstPlayerList.FindIndex(p => p.PlayerName == Player) - 1];
                        gameSetupNew.Play(gameSetupNew.Players, NextPlayer.PlayerName, InitialDicardDeckCard[0]);
                        lstGameObjs.Add(gameSetupNew);
                        gameCounts++;

                    }
                    else if (InitialDicardDeckCard[0] != "Wild" && InitialDicardDeckCard[1] == "ChangeDirection")
                    {
                        // 2 PLAYER SCENARIO: // PLAYER TO PLAY SKIPS TURN
                        // string playerToStartPlay = Player;
                        NextPlayer = lstPlayerList[(lstPlayerList.FindIndex(p => p.PlayerName == Player) - 1 < 0) ? lstPlayerList.Count - 1 : lstPlayerList.FindIndex(p => p.PlayerName == Player) - 1];
                        gameSetupNew.Play(gameSetupNew.Players, NextPlayer.PlayerName, InitialDicardDeckCard[0]);
                        lstGameObjs.Add(gameSetupNew);
                        gameCounts++;
                    }
                    else
                    {

                        gameSetupNew.Play(gameSetupNew.Players, Player, InitialDicardDeckCard[0]);
                        lstGameObjs.Add(gameSetupNew);
                        gameCounts++;
                    }
                }

                // MULTIPLE GAMES TILL SCORE REACHES 500

                foreach(GameSetup game in lstGameObjs)
                {
                    string winnerName = game.LastGameWinner;
                    int winnerPointScored = game.LastGameScore;
                    string playerName = String.Empty;
                    // take players name from PlayersList inside GameSetup Object - For playerCount > 2.
                    if(winnerName == "Computer")
                    {
                        ComputerTotalScore += winnerPointScored;                        
                    }
                    else
                    {
                        PlayerTotalScore += winnerPointScored;                        
                    }                  

                }

                // CHECKS WHICH PLAYER REACHES 500 AND EXITS 
                if (ComputerTotalScore >= 500)
                {
                    Console.WriteLine();
                    Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
                    Console.WriteLine("Player : Computer Wins  [Total Game Score reached 500]");
                    Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
                    break;
                }
                if (PlayerTotalScore >= 500)
                {
                    Console.WriteLine();
                    Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
                    Console.WriteLine("Player : Player Wins  [Total Game Score reached 500]");
                    Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
                    break;
                }
                Console.WriteLine();
                Console.WriteLine("Another Game ?? Enter 'Y' for YES. Enter 'N' for NO");
                Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
                ContinueGame = (Console.ReadLine() == "Y") ? 1 : ((Console.ReadLine() == "N" ) ? 0 : 2);
                
                if(ContinueGame == 0 || ContinueGame  == 2)
                {
                    Console.WriteLine();
                    Console.WriteLine("User chose to END GAME");
                    Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
                    break;
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine(">>>>>>>>>>>>>>>>>>>> NEW GAME BEGINS >>>>>>>>>>>>>>>>>>>>>");
                    Console.WriteLine();
                }
                
            }
            // CLOSE CONSOLE APP
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(">>>>>>>>>>>>>>>>>>>> GAME OVER >>>>>>>>>>>>>>>>>>>>>");
        }

        //static void ShowDeckPile(List<Cards> DeckPile)
        //{
        //    int count = 0;
        //    foreach (Cards c in DeckPile)
        //    {
        //        Console.Write("{0}  ",++count);
        //        Console.Write("CardsColour: {0}  CardName: {1}  CardPoint: {2} \n",c.Colour,c.Name,c.Points);               
                
        //    }

        //}

        //void ShowDiscardPile(List<Cards> DiscardPile)
        //{
        //    int count = 0;
        //    foreach (Cards c in DiscardPile)
        //    {
        //        Console.WriteLine(" Discard Pile ");
        //        Console.Write("{0}  ", ++count);
        //        Console.Write("CardsColour: {0}  CardName: {1}  CardPoint: {2} \n", c.Colour, c.Name, c.Points);

        //    }

        //}
        internal void ResumeGame()
        {
            // READ FROM SAVED SESSION FILE AND LOAD GAME WITH SAME VALUES FOR OBJECT.
        }
    }
}
