namespace DSharpPlus.Commands.Converters;

using System.Globalization;
using System.Threading.Tasks;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

public class UInt32Converter : ISlashArgumentConverter<uint>, ITextArgumentConverter<uint>
{
    public string ReadableName { get; init; } = "Unsigned Integer (0 through 4,294,967,295)";
    public ApplicationCommandOptionType ParameterType { get; init; } = ApplicationCommandOptionType.Integer;
    public bool RequiresText { get; init; } = true;

    public Task<Optional<uint>> ConvertAsync(TextConverterContext context, MessageCreateEventArgs eventArgs) => uint.TryParse(context.Argument, CultureInfo.InvariantCulture, out uint result)
        ? Task.FromResult(Optional.FromValue(result))
        : Task.FromResult(Optional.FromNoValue<uint>());

    public Task<Optional<uint>> ConvertAsync(InteractionConverterContext context, InteractionCreateEventArgs eventArgs) => uint.TryParse(context.Argument.RawValue, CultureInfo.InvariantCulture, out uint result)
        ? Task.FromResult(Optional.FromValue(result))
        : Task.FromResult(Optional.FromNoValue<uint>());
}
