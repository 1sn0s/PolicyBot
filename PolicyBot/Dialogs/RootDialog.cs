namespace PolicyBot
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using System.Web;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    [Serializable]
    internal class RootDialog : IDialog<object>
    {
        private const string ShowDocument = "(1) I want the policy document ?";
        private const string ShowSpecific = "(2) I want to know about a specific policy ?";

        private readonly IDictionary<string, string> options = new Dictionary<string, string>
        {
            { "1", ShowDocument },
            { "2", ShowSpecific }
        };

        public async Task StartAsync(IDialogContext context)
        {            
            var welcomeMessage = context.MakeMessage();
            welcomeMessage.Text = "Welcome, I am an automated assistant for Company policies";
            await context.PostAsync(welcomeMessage);
            context.Wait(this.MessageReceivedAsync);
        }

        public async virtual Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            await this.DisplayOptionsAsync(context);
        }

        public async Task DisplayOptionsAsync(IDialogContext context)
        {
            PromptDialog.Choice<string>(
                context,
                this.ProcessSelectedOptionAsync,
                this.options.Keys,
                "Please select an option by its number ?",
                "Oops!, what you wrote is not a valid option, please try again",
                3,
                PromptStyle.PerLine,
                this.options.Values);
        }

        public async Task ProcessSelectedOptionAsync(IDialogContext context, IAwaitable<string> argument)
        {
            var message = await argument;

            var replyMessage = context.MakeMessage();

            Attachment attachment = null;

            switch (message)
            {
                case "1":
                    attachment = await GetPolicyAttachmentAsync(replyMessage.ServiceUrl, replyMessage.Conversation.Id);
                    replyMessage.Attachments = new List<Attachment> { attachment };
                    await context.PostAsync(replyMessage);
                    break;
                case "2":
                    //Pass the context to PolicyDialog
                    context.Call(new PolicyDialog(), this.ResumeAfterOptionDialog);
                    break;
            }
        }

        private async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var message = await result;
            }
            catch (Exception ex)
            {
                await context.PostAsync($"Failed with message: {ex.Message}");
            }
            finally
            {
                context.Wait(this.MessageReceivedAsync);
            }
        }

        private static async Task<Attachment> GetPolicyAttachmentAsync(string serviceUrl, string conversationId)
        {
            var imagePath = HttpContext.Current.Server.MapPath("~/images/abc.png");

            using (var connector = new ConnectorClient(new Uri(serviceUrl)))
            {
                var attachments = new Attachments(connector);
                var response = await attachments.Client.Conversations.UploadAttachmentAsync(
                    conversationId,
                    new AttachmentData
                    {
                        Name = "abc.png",
                        OriginalBase64 = File.ReadAllBytes(imagePath),
                        Type = "image/png"
                    });

                var attachmentUri = attachments.GetAttachmentUri(response.Id);

                // Typo bug in current assembly version '.Replace("{vieWId}", Uri.EscapeDataString(viewId))'.
                // TODO: remove this line when replacement Bug is fixed on future releases. PR: https://github.com/Microsoft/BotBuilder/pull/2079
                attachmentUri = attachmentUri.Replace("{viewId}", "original");

                return new Attachment
                {
                    Name = "big-image.png",
                    ContentType = "image/png",
                    ContentUrl = attachmentUri
                };
            }
        }
    }
}