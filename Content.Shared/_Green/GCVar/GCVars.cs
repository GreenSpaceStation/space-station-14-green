using Robust.Shared.Configuration;

namespace Content.Shared._Green.GCVar;

public sealed class GCVars
{
    public static readonly CVarDef<string> DiscordBanWebhook =
        CVarDef.Create("discord.ban_webhook", string.Empty, CVar.SERVERONLY | CVar.CONFIDENTIAL);
}
