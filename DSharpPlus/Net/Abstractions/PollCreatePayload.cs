using System.Collections.Generic;
using System.Linq;
using DSharpPlus.Entities;
using Newtonsoft.Json;

namespace DSharpPlus.Net.Abstractions;

public sealed class PollCreatePayload
{
    /// <summary>
    /// Gets the question for this poll. Only text is supported.
    /// </summary>
    [JsonProperty("question")]
    public DiscordPollMedia Question { get; internal set; }

    /// <summary>
    /// Gets the answers available in the poll.
    /// </summary>
    [JsonProperty("answers")]
    public IReadOnlyList<DiscordPollAnswer> Answers { get; internal set; }

    /// <summary>
    /// Gets the expiry date for this poll.
    /// </summary>
    [JsonProperty("duration")]
    public int Duration { get; internal set; }

    /// <summary>
    /// Whether the poll allows for multiple answers.
    /// </summary>
    [JsonProperty("allow_multiselect")]
    public bool AllowMultisect { get; internal set; }

    /// <summary>
    /// Gets the layout type for this poll. Defaults to <see cref="DiscordPollLayoutType.Default"/>.
    /// </summary>
    [JsonProperty("layout_type", NullValueHandling = NullValueHandling.Ignore)]
    public DiscordPollLayoutType? Layout { get; internal set; }

    internal PollCreatePayload() { }

    internal PollCreatePayload(DiscordPoll poll)
    {
        this.Question = poll.Question;
        this.Answers = poll.Answers;
        this.AllowMultisect = poll.AllowMultisect;
        this.Layout = poll.Layout;
    }

    internal PollCreatePayload(DiscordPollBuilder builder)
    {
        this.Question = new DiscordPollMedia { Text = builder.Question };
        this.Answers = builder.Options.Select(x => new DiscordPollAnswer { AnswerData = x }).ToList();
        this.AllowMultisect = builder.IsMultipleChoice;
        this.Duration = builder.Duration;
    }
}
