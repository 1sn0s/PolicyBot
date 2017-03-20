using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using PolicyBot.Models;

namespace PolicyBot
{
    #region LUIS Integration
    //[LuisModel("ad338c30-cf9b-4588-8fc9-ceb8e447842e", "7f730da314ac448d9061ec6280fb1757")]
    //[Serializable]
    //public class PolicyDialog : LuisDialog<object>
    //{
    //    public const string Entity_leave = "Leave";
    //    enum Leaves { Sick, Casual, Privilege, Maternity, Paternity, Loss_of_pay};

    //    [LuisIntent("")]
    //    public async Task None(IDialogContext context, LuisResult result)
    //    {
    //        string message = $"Sorry I did not understand: "
    //            + string.Join(", ", result.Intents.Select(i => i.Intent));
    //        await context.PostAsync(message);
    //        context.Wait(MessageReceived);
    //    }

    //    [LuisIntent("GetPolicyDetail")]
    //    public async Task GetPolicyDetail(IDialogContext context, LuisResult result)
    //    {
    //        var leaveTypes = (IEnumerable<Leaves>) Enum.GetValues(typeof(Leaves));
    //        EntityRecommendation typeOfLeave;

    //        if (!result.TryFindEntity(Entity_leave, out typeOfLeave))
    //        {
    //            PromptDialog.Choice(context,
    //                                SelectLeaveType,
    //                                leaveTypes,
    //                                "Which leave type do you want to know about ?");
    //        }
    //        else
    //        {
    //            //TODO: - Get the policy specifics from the DB/File system
    //            await context.PostAsync($"Policy details about {typeOfLeave} is : ...");
    //            context.Wait(MessageReceived);
    //        }
    //    }

    //    private async Task SelectLeaveType(IDialogContext context, IAwaitable<Leaves> leave)
    //    {
    //        var message = string.Empty;
    //        switch (await leave)
    //        {
    //            case Leaves.Casual:
    //            case Leaves.Sick:
    //            case Leaves.Privilege:
    //            case Leaves.Maternity:
    //            case Leaves.Paternity:
    //            case Leaves.Loss_of_pay:
    //                message = $"Policy details about {leave} is : ...";
    //                break;
    //            default:
    //                message = $"Sorry!! I am not aware of {leave}";
    //                break;
    //        }
    //        await context.PostAsync(message);
    //        context.Wait(MessageReceived);
    //    }
    //}
    #endregion LUIS Integration
    [Serializable]
    public class PolicyDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("what policy do you want to know about?");
            context.Wait(this.MessageReceivedAsync);
        }

        public async virtual Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            await context.PostAsync($"checking ...");
            string policyKey = string.Empty;
            //Send and recive from LUIS here 
            PolicyLUIS userRequest = await LUIS.ProcessuserInput(message.Text);
            policyKey = await new PolicyActionManager().GetActionToPerform(userRequest);
            //policyKey = "leave";
            PrepareResponse(context, policyKey);
        }

        //Prepare the respnse
        private async Task PrepareResponse(IDialogContext context, string policyKey)
        {
            //Manage the query and prepare the response
            string replyMessage = string.Empty;
            //If policy has sub policy
            PolicyDataController policyData = new PolicyDataController();
            Policy policy = policyData.GetPolicy(policyKey);
            if (!string.IsNullOrEmpty(policy.policyText))
            {
                replyMessage = policy.policyText;
            }

            if (policy.subpolicies != null)
            {                
                string prompt = string.Concat(replyMessage, "\n  Enter the sub policy you want to know about?");
                this.ShowOptions(context, policy.subpolicies, prompt);
            }
            else
            {
                //If there is no subpolicy, then reply and wait
                await context.PostAsync(replyMessage);
            }
        }

        //Show sub options
        private void ShowOptions(IDialogContext context, List<string> options, string prompt)
        {
            PromptDialog.Choice(context, 
                this.OnOptionSelected,
                options,
                prompt,
                "Oops!, what you wrote is not a valid option, please try again",
                2
                );
        }

        //Choosing sub option
        private async Task OnOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string optionSelected = (await result).ToLower();
                string replyMessage = string.Empty;
                switch (optionSelected)
                {
                    case "exit" : context.Wait(this.MessageReceivedAsync);
                        break;
                    default     : PrepareResponse(context, optionSelected);
                        break;
                }         
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Too many attempts!!");
                context.Wait(this.MessageReceivedAsync);
            }
        }
    }
}