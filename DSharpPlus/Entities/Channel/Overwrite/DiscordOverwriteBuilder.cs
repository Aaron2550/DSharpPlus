using System.Threading.Tasks;

using Newtonsoft.Json;

namespace DSharpPlus.Entities;

/// <summary>
/// Represents a Discord permission overwrite builder.
/// </summary>
public sealed class DiscordOverwriteBuilder
{
    /// <summary>
    /// Gets or sets the allowed permissions for this overwrite.
    /// </summary>
    public DiscordPermissions Allowed { get; set; }

    /// <summary>
    /// Gets or sets the denied permissions for this overwrite.
    /// </summary>
    public DiscordPermissions Denied { get; set; }

    /// <summary>
    /// Gets the type of this overwrite's target.
    /// </summary>
    public DiscordOverwriteType Type { get; private set; }

    /// <summary>
    /// Gets the target for this overwrite.
    /// </summary>
    public SnowflakeObject Target { get; private set; }

    /// <summary>
    /// Creates a new Discord permission overwrite builder for a member. This class can be used to construct permission overwrites for guild channels, used when creating channels.
    /// </summary>
    public DiscordOverwriteBuilder(DiscordMember member)
    {
        this.Target = member;
        this.Type = DiscordOverwriteType.Member;
    }
    /// <summary>
    /// Creates a new Discord permission overwrite builder for a role. This class can be used to construct permission overwrites for guild channels, used when creating channels.
    /// </summary>
    public DiscordOverwriteBuilder(DiscordRole role)
    {
        this.Target = role;
        this.Type = DiscordOverwriteType.Role;
    }

    /// <summary>
    /// Allows a permission for this overwrite.
    /// </summary>
    /// <param name="permission">Permission or permission set to allow for this overwrite.</param>
    /// <returns>This builder.</returns>
    public DiscordOverwriteBuilder Allow(DiscordPermissions permission)
    {
        this.Allowed |= permission;
        return this;
    }

    /// <summary>
    /// Denies a permission for this overwrite.
    /// </summary>
    /// <param name="permission">Permission or permission set to deny for this overwrite.</param>
    /// <returns>This builder.</returns>
    public DiscordOverwriteBuilder Deny(DiscordPermissions permission)
    {
        this.Denied |= permission;
        return this;
    }

    /// <summary>
    /// Sets the member to which this overwrite applies.
    /// </summary>
    /// <param name="member">Member to which apply this overwrite's permissions.</param>
    /// <returns>This builder.</returns>
    public DiscordOverwriteBuilder For(DiscordMember member)
    {
        this.Target = member;
        this.Type = DiscordOverwriteType.Member;
        return this;
    }

    /// <summary>
    /// Sets the role to which this overwrite applies.
    /// </summary>
    /// <param name="role">Role to which apply this overwrite's permissions.</param>
    /// <returns>This builder.</returns>
    public DiscordOverwriteBuilder For(DiscordRole role)
    {
        this.Target = role;
        this.Type = DiscordOverwriteType.Role;
        return this;
    }

    /// <summary>
    /// Populates this builder with data from another overwrite object.
    /// </summary>
    /// <param name="other">Overwrite from which data will be used.</param>
    /// <returns>This builder.</returns>
    public async Task<DiscordOverwriteBuilder> FromAsync(DiscordOverwrite other)
    {
        this.Allowed = other.Allowed;
        this.Denied = other.Denied;
        this.Type = other.Type;
        this.Target = this.Type == DiscordOverwriteType.Member ? await other.GetMemberAsync() : await other.GetRoleAsync();

        return this;
    }

    /// <summary>
    /// Builds this DiscordOverwrite.
    /// </summary>
    /// <returns>Use this object for creation of new overwrites.</returns>
    internal DiscordRestOverwrite Build()
    {
        return new DiscordRestOverwrite()
        {
            Allow = this.Allowed,
            Deny = this.Denied,
            Id = this.Target.Id,
            Type = this.Type,
        };
    }
}

internal struct DiscordRestOverwrite
{
    [JsonProperty("allow", NullValueHandling = NullValueHandling.Ignore)]
    internal DiscordPermissions Allow { get; set; }

    [JsonProperty("deny", NullValueHandling = NullValueHandling.Ignore)]
    internal DiscordPermissions Deny { get; set; }

    [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
    internal ulong Id { get; set; }

    [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
    internal DiscordOverwriteType Type { get; set; }
}
