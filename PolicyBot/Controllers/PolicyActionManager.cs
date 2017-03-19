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

            var actions = topScoringIntent.actions;
            if (actions.Count() > 0)
            {
                var action = actions[0];
                replyMessage = action.name;
            }
            return replyMessage;
        }
        
    }
}