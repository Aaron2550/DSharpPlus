using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity.Enums;
using Microsoft.Extensions.Logging;
namespace DSharpPlus.Interactivity.EventHandling;

internal class ComponentPaginator : IPaginator
{
    private readonly DiscordClient _client;
    private readonly InteractivityConfiguration _config;
    private readonly DiscordMessageBuilder _builder = new();
    private readonly Dictionary<ulong, IPaginationRequest> _requests = new();

    public ComponentPaginator(DiscordClient client, InteractivityConfiguration config)
    {
        this._client = client;
        this._client.ComponentInteractionCreated += this.Handle;
        this._config = config;
    }

    public async Task DoPaginationAsync(IPaginationRequest request)
    {
        ulong id = (await request.GetMessageAsync()).Id;
        this._requests.Add(id, request);

        try
        {
            TaskCompletionSource<bool> tcs = await request.GetTaskCompletionSourceAsync();
            await tcs.Task;
        }
        catch (Exception ex)
        {
            this._client.Logger.LogError(InteractivityEvents.InteractivityPaginationError, ex, "There was an exception while paginating.");
        }
        finally
        {
            this._requests.Remove(id);
            try
            {
                await request.DoCleanupAsync();
            }
            catch (Exception ex)
            {
                this._client.Logger.LogError(InteractivityEvents.InteractivityPaginationError, ex, "There was an exception while cleaning up pagination.");
            }
        }
    }

    public void Dispose() => this._client.ComponentInteractionCreated -= this.Handle;


    private async Task Handle(DiscordClient _, ComponentInteractionCreateEventArgs e)
    {
        if (!this._requests.TryGetValue(e.Message.Id, out IPaginationRequest? req))
        {
            return;
        }

        await e.Interaction.CreateResponseAsync(DiscordInteractionResponseType.DeferredMessageUpdate);

        if (await req.GetUserAsync() != e.User)
        {
            if (this._config.ResponseBehavior is InteractionResponseBehavior.Respond)
            {
                await e.Interaction.CreateFollowupMessageAsync(new() { Content = this._config.ResponseMessage, IsEphemeral = true });
            }

            return;
        }

        if (req is InteractionPaginationRequest ipr)
        {
            ipr.RegenerateCTS(e.Interaction); // Necessary to ensure we don't prematurely yeet the CTS //
        }

        await this.HandlePaginationAsync(req, e);
    }

    private async Task HandlePaginationAsync(IPaginationRequest request, ComponentInteractionCreateEventArgs args)
    {
        PaginationButtons buttons = this._config.PaginationButtons;
        DiscordMessage msg = await request.GetMessageAsync();
        string id = args.Id;
        TaskCompletionSource<bool> tcs = await request.GetTaskCompletionSourceAsync();

        Task paginationTask = id switch
        {
            _ when id == buttons.SkipLeft.CustomId => request.SkipLeftAsync(),
            _ when id == buttons.SkipRight.CustomId => request.SkipRightAsync(),
            _ when id == buttons.Stop.CustomId => Task.FromResult(tcs.TrySetResult(true)),
            _ when id == buttons.Left.CustomId => request.PreviousPageAsync(),
            _ when id == buttons.Right.CustomId => request.NextPageAsync(),
            _ => Task.CompletedTask
        };

        await paginationTask;

        if (id == buttons.Stop.CustomId)
        {
            return;
        }

        Page page = await request.GetPageAsync();
        IEnumerable<DiscordButtonComponent> bts = await request.GetButtonsAsync();

        if (request is InteractionPaginationRequest ipr)
        {
            DiscordWebhookBuilder builder = new DiscordWebhookBuilder()
                .WithContent(page.Content)
                .AddEmbed(page.Embed)
                .AddComponents(bts);

            await args.Interaction.EditOriginalResponseAsync(builder);
            return;
        }


        this._builder.Clear();

        this._builder
            .WithContent(page.Content)
            .AddEmbed(page.Embed)
            .AddComponents(bts);

        await this._builder.ModifyAsync(msg);

    }
}
