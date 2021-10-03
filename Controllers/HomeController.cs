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
            if (id.HasValue)
            {
                Card playedCard = GetCard((int)id);

                PlayCard(playedCard);

                //OpponentPlay();

                ViewBag.OpponentHand = opponentHand;

                ViewBag.Board = board;

                return View(playerHand);
            }
            else
            {
            Start:
                Start();

                //  OPPONENT
                opponentHand = deck.DrawHand();

                if (!CheckHand(opponentHand))
                {
                    goto Start;
                };

                ViewBag.OpponentHand = opponentHand;

                //  PLAYER
                playerHand = deck.DrawHand();

                if (!CheckHand(playerHand))
                {
                    goto Start;
                };

                //  BOARD
                board = deck.DrawHand();

                if (!CheckHand(board))
                {
                    goto Start;
                };

                if (!CheckBoard(board))
                {
                    goto Start;
                }

                ViewBag.Board = board;

                ViewBag.Deck = deck;

                return View(playerHand);
            }
        }

        private void OpponentPlay()
        {
            throw new NotImplementedException();
        }

        private void PlayCard(Card playedCard)
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
                foreach (Card card in board)
                {
                    //  Collect cards with same value as playedCard
                    if (playedCard.Value == card.Value)
                    {
                        Pastra(playedCard);
                        toRemove.Add(card);
                        playerCollected.Add(playedCard);
                        playerCollected.Add(card);
                    }
                }
                //  Collect cards with summed value as playedCard
                if ((int)playedCard.Value <= 10)
                {
                    GetCombinations(board, playedCard, new List<Card>());
                    if (toRemove.Count != 0)
                    {
                        playerCollected.Add(playedCard);
                    }
                    playerCollected.AddRange(toRemove);
                }

                //  No cards to Collect, only to add to board
                if (toRemove.Count == 0)
                {
                    board.Add(playedCard);
                }
            }

            foreach (Card card in toRemove)
            {
                board.Remove(card);
            }
            
            //  Reset toRemove
            toRemove = new List<Card>();
        }

        private void Pastra(Card playedCard)
        {
            int pastras = 0;

            for (int i = 0; i < board.Count; i++)
            {
                if (playedCard.Value == board[i].Value)
                {
                    pastras++;
                }
            }

            deck.fullDeck.First(x => x.ID == playedCard.ID).Pastra = pastras;
        }

        private void GetCombinations(List<Card> board, Card playedCard, List<Card> partial)
        {
            int sum = 0;

            foreach (Card card in partial)
            {
                sum += (int)card.Value;
            }

            if (sum == (int)playedCard.Value)
            {
                toRemove.AddRange(partial);
                Pastra(playedCard);
            }

            if (sum >= (int)playedCard.Value)
            {
                return;
            }

            for (int i = 0; i < board.Count; i++)
            {
                List<Card> remaining = new List<Card>();

                Card card = board[i];

                for (int j = i + 1; j < board.Count; j++)
                {
                    remaining.Add(board[j]);
                }

                List<Card> partial_rec = new List<Card>(partial);

                partial_rec.Add(card);

                GetCombinations(remaining, playedCard, partial_rec);
            }

            //  FIXME:      EDGECASE
            //  playedCard = 6
            //  board { ..., 5, 1, 1, ... }
            //  Collects 5, 1, 1 (WRONG)
            //  SOLUTION:
            //  if toRemove contains multiple collections that equal (int)playedCard.Value then set toRemove to the one with the greatest point value

            return;
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
        private bool CheckHand(List<Card> hand)
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

        private Card GetCard(int id)
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


        public ActionResult Modal()
        {
            return View();
        }

        public ActionResult Rules()
        {
            return View("Rules");
        }
    }
}