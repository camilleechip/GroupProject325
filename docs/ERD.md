# Entity Relationship Diagram & Decisions

This file tracks the data model and the design decisions behind it.
Update the diagram and add a row to the decisions log whenever the model changes.

## Diagram (planned, RBAC branch)

Only the entities involved in roles and access are shown. Item, MinorItem,
CustomProperty, PropertyOption and ItemPropertyValue still need to be added.

```mermaid
erDiagram
    User ||--o{ CampaignMember : "belongs to campaigns via"
    Campaign ||--o{ CampaignMember : "has members via"
    User ||--|| UserInventory : owns

    User {
        Guid Id PK
        string UserName
        string Email UK
        string OAuthProvider
        string OAuthSubject
    }

    Campaign {
        int Id PK
        string Title
        string Description
        string System
        CampaignPermission MemberPermissions "flags: what Members may do"
        DateTime CreatedAt
        DateTime UpdatedAt
    }

    CampaignMember {
        Guid UserId PK, FK
        int CampaignId PK, FK
        CampaignRole Role "GM or Member"
    }

    UserInventory {
        Guid Id PK
        Guid UserId FK
    }
```

## Decisions log

| # | Date | Decision | Why | Alternatives considered |
|---|------|----------|-----|-------------------------|
| 1 | 2026-11-12 | No site-wide Admin role for now. | Basic per-campaign roles come first; site admin will be discussed later as a team. | Global Admin role on User. |
| 2 | 2026-11-12 | Roles are per campaign, stored in a `CampaignMember` join table (User ↔ Campaign + Role). | A user can be GM of one campaign and a Member of another. | Single `Role` column on User. |
| 3 | 2026-11-12 | Roles are `GM` and `Member`. A GM can do everything in their campaign. | Matches the proposal's GM/Player user stories. | — |
| 4 | 2026-11-12 | The GM sets one permission set for all Members of a campaign (not per member). | Simpler to build and explain; per-member overrides can be added later. | Per-member permissions; role default + per-member overrides. |
| 5 | 2026-11-12 | Member permissions are stored as a `[Flags]` enum column on Campaign. | Compact and quick to check (`HasFlag`). | A permission-rows table. |
| 6 | 2026-11-12 | Replace `User.Role` (`PartyRole`) and `User.PartyId` with `CampaignMember`. Update `UsersController` to match. | One source of truth for roles. | Keep both side by side for a while. |
| 7 | 2026-11-12 | Login and OAuth testing are out of scope for this task. | Separate task. | Dev-only test login. |
| 8 | 2026-11-12 | Starting permissions: `ViewPartyInventory`, `AddItems`, `EditItems`, `DeleteItems`. *(placeholder, review)* | Covers the inventory features that exist today. Encounters can be added when that model exists. | — |
| 9 | 2026-11-12 | `CampaignMember` uses a composite key (`UserId`, `CampaignId`). | Stops a user from being added to the same campaign twice. | Separate `Id` column + unique index. |
| 10 | 2026-11-12 | `PartyRole` renamed to `CampaignRole`; `User.PartyId` removed. | Roles now belong to a campaign, not a party. | — |
| 11 | 2026-11-12 | Register a cookie authentication scheme with no OAuth yet. | `[Authorize]` crashed without any scheme; now it redirects to a login URL instead. | Leave it broken until the OAuth task. |
| 12 | 2026-11-12 | Permission checks use resource-based authorization (`CampaignPermissionHandler`). GM always passes; Member passes if the campaign's `MemberPermissions` has the flag. | Permissions depend on *which* campaign, so a plain `[Authorize(Roles = ...)]` can't express them. | Role claims in the cookie. |

## Open questions

- What happens to `Campaign.GameMaster` (a text name) now that the GM comes from `CampaignMember`? (Left in place for now.)
- Is "Party" the same thing as "Campaign"?
- Who creates the first EF Core migration, so teammates don't create conflicting ones?
- When a campaign is created, the creator should become its GM. That needs a signed-in user, so it waits for the OAuth task.
- Hook `CampaignPermissionHandler` into the Item/Campaign controllers (not done in this branch).
