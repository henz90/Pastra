using Pastra.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Pastra.Controllers
{
    public class HomeController : Controller
    {
        public static Deck deck = new Deck();
        public static List<Card> opponentHand;
        public static List<Card> playerHand;
        public static List<Card> board;
        public static List<Card> toRemove = new List<Card>();
        public static List<Card> playerCollected = new List<Card>();
        public static List<Card> opponentCollected = new List<Card>();

        public ActionResult Index(int? id)
        {
            //  Game in progress
            if (id.HasValue)
            {
                Card playedCard = GetCardFromID((int)id);

                PlayCard(playedCard, true);

                Card opponentPlay = OpponentPlay();

                PlayCard(opponentPlay, false);

                if (!opponentHand.Any())
                {
                    opponentHand = deck.DrawHand();
                    playerHand = deck.DrawHand();
                }

                ViewBag.OpponentPlay = opponentPlay;

                ViewBag.OpponentHand = opponentHand;

                ViewBag.Board = board;

                //  Game is over
                if (deck.getDeck.Count() == 0)
                {
                    //playerCollected.Distinct(); //  FIXME:  Find where duplicates are happening

                    //opponentCollected.Distinct(); //  FIXME:  Find where duplicates are happening

                    int playerScore = 0;

                    playerScore += playerCollected.Sum(x => x.Points);

                    int opponentScore = 0;

                    opponentScore += opponentCollected.Sum(x => x.Points);

                    foreach (Card card in playerCollected)
                    {
                        if (card.Pastra > 0)
                        {
                            playerScore += (int)card.Value * card.Pastra;
                        }
                    }

                    foreach (Card card in opponentCollected)
                    {
                        if (card.Pastra > 0)
                        {
                            opponentScore += (int)card.Value * card.Pastra;
                        }
                    }

                    if (playerCollected.Count() > opponentCollected.Count())
                    {
                        playerScore += 3;
                    }
                    else
                    {
                        opponentScore += 3;
                    }

                    ViewBag.Complete = true;

                    ViewBag.PlayerScore = playerScore;

                    ViewBag.OpponentScore = opponentScore;

                    ViewBag.playerCollected = playerCollected;

                    ViewBag.OpponentCollected = opponentCollected;

                    ViewBag.Complete = true;
                }
                //  Game isn't over
                else
                {
                    ViewBag.Complete = false;
                }

                return View(playerHand);
            }
            //  New Game
            else
            {
            Start:
                Start();

                //  OPPONENT
                opponentHand = deck.DrawHand();

                if (!CheckForTriplets(opponentHand))
                {
                    //  Hand cannot start with 3 of the same card
                    goto Start;
                };

                ViewBag.OpponentHand = opponentHand;

                //  PLAYER
                playerHand = deck.DrawHand();

                if (!CheckForTriplets(playerHand))
                {
                    //  Hand cannot start with 3 of the same card
                    goto Start;
                };

                //  BOARD
                board = deck.DrawHand();

                if (!CheckForTriplets(board))
                {
                    //  Board cannot start with 3 of the same card
                    goto Start;
                };

                if (!CheckBoard(board))
                {
                    //  Board cannot start with a Jack
                    goto Start;
                }

                ViewBag.Board = board;

                ViewBag.Complete = false;

                return View(playerHand);
            }
        }

        private Card OpponentPlay()
        {
            //List<Card> memory = board.Concat(playerCollected).Concat(opponentCollected).ToList();

            //List<Card> cardsLeft = deck.getDeck.ToList().Except(memory).ToList();

            //  Get all combos from the played card
            List<List<Card>> combos = GetCombos(new List<Card>(), board, new List<List<Card>>());

            List<Card> bestCombo = new List<Card>();

            Card bestCard = null;

            bool isTherePastra = false;

            foreach (Card card in opponentHand)
            {
                //  There is a board
                if (combos.Any())
                {
                    //  Get all matches based on card
                    List<List<Card>> matchedCombos = GetMatches(card, combos);

                    //  There are matches
                    if (matchedCombos.Any())
                    {
                        //  Best combo based on matches
                        List<Card> contender = matchedCombos.OrderBy(x => x.Count()).OrderBy(y => y.Sum(z => z.Points)).FirstOrDefault();

                        //  Board has a pastra
                        if (board.Count() == contender.Count())
                        {
                            bestCombo = contender;
                            bestCard = card;
                            isTherePastra = true;
                        }
                        //  Compare bestCombo to contenter
                        else if (contender.Sum(x => x.Points) >= bestCombo.Sum(x => x.Points) || !bestCombo.Any() && !isTherePastra)
                        {
                            //  Combo with the highest points
                            bestCombo = contender;
                            bestCard = card;
                        }
                    }
                    //  Jack in hand
                    if ((int)card.Value == 11 && !isTherePastra)
                    {
                        //  FIXME:  Needs additional logic
                        bestCard = card;
                    }
                }
            }
            //  Best card to be played exists
            if (bestCard == null)
            {
                //  Play the card with best confidence not to fuck over the computer the most
                bestCard = opponentHand.OrderBy(x => x.Value).FirstOrDefault();
            }

            return bestCard;
        }

        private void PlayCard(Card playedCard, bool isPlayer)
        {
            if (isPlayer)
            {
                playerHand.Remove(playedCard);

                //  Jack is played, if there are cards on the board collect them all
                if ((int)playedCard.Value == 11 && board.Count() > 0)
                {
                    playerCollected.Add(playedCard);

                    foreach (Card card in board)
                    {
                        toRemove.Add(card);
                        playerCollected.Add(card);
                    }
                }
                else
                {
                    CollectCardLogic(playedCard, true);
                }
            }
            else
            {
                opponentHand.Remove(playedCard);

                //  Jack is played, if there are cards on the board collect them all
                if ((int)playedCard.Value == 11 && board.Count() > 0)
                {
                    opponentCollected.Add(playedCard);

                    foreach (Card card in board)
                    {
                        toRemove.Add(card);
                        opponentCollected.Add(card);
                    }
                }
                else
                {
                    CollectCardLogic(playedCard, false);
                }
            }

            if (toRemove.Count() == board.Count() && (int)playedCard.Value != 11)
            {
                Pastra(playedCard);
            }
            
            //  Remove the Collected cards from the Board
            foreach (Card card in toRemove)
            {
                board.Remove(card);
            }

            //  Reset toRemove
            toRemove = new List<Card>();
        }

        private void CollectCardLogic(Card playedCard, bool isPlayer)
        {
            if (isPlayer)
            {
                foreach (Card card in board)
                {
                    //  Collect cards with same value as playedCard (For Face Cards)
                    if ((int)playedCard.Value >= 12 && playedCard.Value == card.Value)
                    {
                        toRemove.Add(card);
                        playerCollected.Add(playedCard);
                        playerCollected.Add(card);
                    }
                }

                //  Collect cards with summed value as playedCard
                if ((int)playedCard.Value <= 10)
                {
                    ComboLogic(playedCard);

                    //  If cards were Collected, collect the played card too
                    if (toRemove.Count != 0)
                    {
                        playerCollected.Add(playedCard);
                    }
                    playerCollected.AddRange(toRemove.Distinct());
                }
            }
            else
            {
                foreach (Card card in board)
                {
                    //  Collect cards with same value as playedCard (For Face Cards)
                    if ((int)playedCard.Value >= 12 && playedCard.Value == card.Value)
                    {
                        toRemove.Add(card);
                        opponentCollected.Add(playedCard);
                        opponentCollected.Add(card);
                    }
                }

                //  Collect cards with summed value as playedCard
                if ((int)playedCard.Value <= 10)
                {
                    ComboLogic(playedCard);

                    //  If cards were Collected, collect the played card too
                    if (toRemove.Count != 0)
                    {
                        opponentCollected.Add(playedCard);
                    }
                    opponentCollected.AddRange(toRemove.Distinct());
                }
            }

            //  No cards to Collect, only to add to board
            if (toRemove.Count == 0)
            {
                board.Add(playedCard);
            }
        }

        private void ComboLogic(Card playedCard)
        {
            //  Get all combos from the board
            List<List<Card>> combos = GetCombos(new List<Card>(), board, new List<List<Card>>());

            //  Get combos that match the played card
            List<List<Card>> matchedCombos = GetMatches(playedCard, combos);

            //  Get all cards in the matched combos
            List<Card> duplicates = matchedCombos.SelectMany(x => x)
                .GroupBy(x => x)
                .Where(y => y.Count() > 1)
                .Select(z => z.Key)
                .ToList();

            //  There are duplicates
            if (duplicates.Count() > 0)
            {
                List<List<Card>> combosWithDuplicate = new List<List<Card>>();

                foreach (List<Card> combo in matchedCombos)
                {
                    if (combo.Any(x => duplicates.Contains(x)))
                    {
                        combosWithDuplicate.Add(combo);
                    }
                    else
                    {
                        toRemove.AddRange(combo);
                    }
                }

                List<Card> highestPointCombo = combosWithDuplicate.OrderByDescending(x => x.Sum(y => y.Points)).FirstOrDefault();

                List<Card> highestSizeCombo = combosWithDuplicate.OrderByDescending(x => x.Count()).FirstOrDefault();

                if (highestSizeCombo.Sum(x => x.Points) >= highestPointCombo.Sum(x => x.Points))
                {
                    toRemove.AddRange(highestSizeCombo);
                }
                else
                {
                    toRemove.AddRange(highestPointCombo);
                }

            }
            else
            {
                foreach (List<Card> combo in matchedCombos)
                {
                    toRemove.AddRange(combo);
                }
            }

        }

        private List<List<Card>> GetMatches(Card playedCard, List<List<Card>> combos)
        {
            List<List<Card>> matchedCombos = new List<List<Card>>();

            foreach (List<Card> combo in combos)
            {
                int sum = 0;

                foreach (Card card in combo)
                {
                    sum += (int)card.Value;
                }

                if (sum == (int)playedCard.Value)
                {
                    matchedCombos.Add(combo);
                }
            }

            return matchedCombos;
        }

        //  https://codereview.stackexchange.com/questions/7001/generating-all-combinations-of-an-array
        private List<List<Card>> GetCombos(List<Card> previous, List<Card> board, List<List<Card>> combos)
        {
            for (int i = 0; i < board.Count; i++)
            {
                //  cardCombos
                Card card = board[i];
                List<Card> newPrevious = previous.Take(i).Concat(previous.Skip(i)).ToList();
                newPrevious.Add(card);

                combos.Add(newPrevious);
                List<Card> newBoard = board.Skip(i + 1).ToList();
                GetCombos(newPrevious, newBoard, combos);
            }

            return combos;
        }

        private void Pastra(Card playedCard)
        {
            List<List<Card>> combos = GetCombos(new List<Card>(), board, new List<List<Card>>());

            List<List<Card>> matchedCombos = GetMatches(playedCard, combos);

            int pastraMultiplier = matchedCombos.Count();

            playedCard.Pastra = pastraMultiplier;

            if (playedCard.Pastra > 0)
            {
                _ = pastraMultiplier;
            }
        }

        private void Start()
        {
            deck = new Deck();
            deck.InstantiateDeck();
            deck.ShuffleDeck();
            playerCollected = new List<Card>();
            opponentCollected = new List<Card>();
            toRemove = new List<Card>();
        }

        //  Check hands for triplicates
        private bool CheckForTriplets(List<Card> hand)
        {
            var check = hand.GroupBy(x => x.Value);
            foreach (var item in check)
            {
                if (item.Count() >= 3)
                {
                    return false;
                }
            }
            return true;
        }

        //  Check board for Jacks
        private bool CheckBoard(List<Card> hand)
        {
            foreach (Card card in hand)
            {
                if (hand.Any(x => x.Value == Card.VALUE.JACK))
                {
                    return false;
                }
            }
            return true;
        }

        private Card GetCardFromID(int id)
        {
            Card playedCard = new Card();

            foreach (Card card in deck.fullDeck)
            {
                if (card.ID == id)
                {
                    playedCard = card;
                }
            }
            return playedCard;
        }

        public ActionResult Rules()
        {
            return View("Rules");
        }
    }
}