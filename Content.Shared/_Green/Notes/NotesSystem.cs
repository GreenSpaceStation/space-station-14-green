using System.Text;
using Content.Shared.Examine;
using Content.Shared.Humanoid;
using Content.Shared.Verbs;
using Robust.Shared.Utility;

namespace Content.Shared._Green.Notes;

public sealed class NotesSystem : EntitySystem
{
    [Dependency] private readonly ExamineSystemShared _examine = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<HumanoidAppearanceComponent, GetVerbsEvent<ExamineVerb>>(OnGetVerbs);
    }

    private void OnGetVerbs(Entity<HumanoidAppearanceComponent> entity, ref GetVerbsEvent<ExamineVerb> e)
    {
        var user = e.User;

        var disabled = !_examine.IsInDetailsRange(user, entity);

        e.Verbs.Add(new()
        {
            Text = Loc.GetString("notes-verb-text"),
            Category = VerbCategory.Examine,
            Disabled = disabled,
            Message = disabled ? Loc.GetString("notes-verb-disabled") : null,
            Icon = new SpriteSpecifier.Texture(new("/Textures/Interface/VerbIcons/vv.svg.192dpi.png")),
            Act = () =>
            {
                _examine.SendExamineTooltip(user, entity, FormattedMessage.FromMarkupOrThrow(GetNotes(entity)), false, false);
            }
        });
    }

    private string GetNotes(Entity<HumanoidAppearanceComponent> entity)
    {
        StringBuilder builder = new();

        builder.AppendLine(Loc.GetString("humanoid-profile-editor-ooc-label"));

        builder.AppendLine(Loc.GetString($"notes-verb-erp-{entity.Comp.Erp.ToString().ToLower()}"));

        return builder.ToString();
    }
}
