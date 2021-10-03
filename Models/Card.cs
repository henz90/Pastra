﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pastra.Models
{
    public class Card
    {
        public enum SUIT
        {
            HEARTS,
            DIAMONDS,
            SPADES,
            CLUBS
        }

        public enum VALUE
        {
            ACE = 1,
            TWO,
            THREE,
            FOUR,
            FIVE,
            SIX,
            SEVEN,
            EIGHT,
            NINE,
            TEN,
            JACK,
            QUEEN,
            KING,
        }

        public int ID { get; set; }
        public SUIT Suit { get; set; }
        public VALUE Value { get; set; }
        public int Pastra { get; set; }

        public int Points
        {
            get
            {
                int points = 0;

                switch (Value)
                {
                    case VALUE.ACE:
                        points = 1;
                        break;
                    case VALUE.TWO:
                        if (Suit.Equals("CLUBS"))
                        {
                            points = 2;
                        }
                        break;
                    case VALUE.TEN:
                        if (Suit.Equals("DIAMONDS"))
                        {
                            points = 3;
                        }
                        break;
                    case VALUE.JACK:
                        points = 1;
                        break;
                    default:
                        break;
                }

                return points;
            }
        }
        public string Image
        {
            get
            {
                string image = "";
                
                int numValue = (int)Value;
                if (numValue < 10)
                {
                    image += "0" + numValue;
                }
                else if (numValue == 10)
                {
                    image += numValue;
                }
                else
                {
                    image += "0" + Value.ToString()[0];
                }

                image += Suit.ToString()[0];

                image += ".png";

                return image;
            }
        }
    }
}