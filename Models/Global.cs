using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pastra.Models
{
    public class Global
    {
        public static string Ace = "<div class='cardTooltip'>Ace<span><img src='/Resources/01C.png' class='card'/></span></div>";

        public static string King = "<div class='cardTooltip'>King<span><img src='/Resources/0KC.png' class='card'/></span></div>";

        public static string Queen = "<div class='cardTooltip'>Queen<span><img src='/Resources/0QC.png' class='card'/></span></div>";

        public static string Jack = "<div class='cardTooltip'>Jack<span><img src='/Resources/0JC.png' class='card'/></span></div>";

        public static string TenD = "<div class='cardTooltip'>Ten of Diamonds<span><img src='/Resources/10D.png' class='card'/></span></div>";

        public static string TwoC = "<div class='cardTooltip'>Two of Clubs<span><img src='/Resources/02C.png' class='card'/></span></div>";

        public static int HandSize = 4;

        public static int DeckSize = 52;
    }
}