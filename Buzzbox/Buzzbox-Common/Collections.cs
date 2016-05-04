using System.Collections.Generic;

namespace Buzzbox_Common
{
    public class Collections
    {
        public static Dictionary<string, string> KeywordReplacements = new Dictionary<string, string>
        {
            {"Battlecry and Deathrattle", "$BD$"},
            {"Battlecry", "$B$"},
            {"Charge", "$CH$"},
            {"Choose One", "$CO$"},
            {"Combo", "$C$"},
            {"Counter", "$CR$"},
            {"Deathrattle", "$DR$"},
            {"Discover", "$D$"},           
            {"Divine Shield", "$DV$"},
            {"Enrage", "$E$"},
            {"Freeze", "$F$"},
            {"Immune", "$I$"},
            {"Inspire", "$IN$"},            
            {"Overload", "$O$"},
            {"Secret", "$SA$"},
            {"Secrets", "$S$"},
            {"Silence", "$SI$"},
            {"Stealth", "$ST$"},
            {"Spell Damage", "$SD$"},            
            {"Taunt", "$T$"},
            {"Transform", "$TR$"},
            {"Windfury", "$W$"}
        };

        public static Dictionary<string, string> ReverseKeywordReplacements = new Dictionary<string, string>
        {
            {"$BD$","<b>Battlecry and Deathrattle</b>"},
            {"$B$","<b>Battlecry</b>"},
            {"$CH$","<b>Charge</b>"},
            {"$CO$","<b>Choose One</b>"},
            {"$C$","<b>Combo</b>"},
            {"$CR$","<b>Counter</b>"},
            {"$DR$","<b>Deathrattle</b>"},
            {"$D$","<b>Discover:</b>"},
            {"$DV$","<b>Divine Shield</b>"},
            {"$E$","<b>Enrage</b>"},
            {"$F$","<b>Freeze</b>"},
            {"$I$","<b>Immune</b>"},
            {"$IN$","<b>Inspire</b>"},
            {"$O$","<b>Overload</b>"},
            {"$SS$","<b>Secrets</b>"},
            {"$S$","<b>Secret</b>"},
            {"$SI$","<b>Silence</b>"},
            {"$ST$","<b>Stealth</b>"},
            {"$SD$","<b>Spell Damage</b>"},
            {"$T$","<b>Taunt</b>"},
            {"$TR$","<b>Transform</b>"},
            {"$W$","<b>Windfury</b>"}
        };
    }
}
