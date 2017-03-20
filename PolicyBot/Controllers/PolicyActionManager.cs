using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PolicyBot.Models;
using System.Threading.Tasks;

namespace PolicyBot
{
    public class PolicyActionManager
    {
        enum Leaves { Sick, Casual, Privilege, Maternity, Paternity, Loss_of_pay };

        public async Task<string> GetActionToPerform(PolicyLUIS userData)
        {
            string replyMessage = null;

            var topScoringIntent = userData.intents[0];
            if (topScoringIntent.intent == "None")
            {
                return replyMessage;
            }

            var intent = topScoringIntent.intent;
            switch (intent.ToLower())
            {
                case "getleave": replyMessage = "leave";
                    break;
                case "getleavecasual": replyMessage = "casual";
                    break;
                case "getleavesick":
                    replyMessage = "sick";
                    break;
                default: replyMessage = "none";
                    break;
            }
            return replyMessage;
        }
        
    }
}