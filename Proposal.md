# {TTRPG Ledger}

## Metadata

| Field                | Value                                         |
| -------------------- | --------------------------------------------- |
| **Feature ID**       | {1.0}                                         |
| **Section**          | {e.g. Adaptive Learning Core}                 |
| **Severity**         | MAJOR                                         |
| **Markets**          | TTRPG Players - System Agnostic,              |
| **Status (today)**   | THIN                                          |
| **Estimated effort** | M (2–4w)                                      |
| **Owner (proposed)** | Bolievers (Believers) Wednesday Evening Group |

---

## 1. Problem Statement

A System-agnostic ledger application for tracking resources in a tabletop game.  
In a tabletop game, often times there are issues tracking items as different people write different things down. Inevitably, some items become duplicated, lost, forgotten, or incorrectly transcribed. By creating a ledger system, the group of TTRPG players can more easily track important items, treasure, and experience.

## 2. Project Requirements

- Plan, design, develop, and deploy a .NET Blazor web application.
- The web application must meet the goals and specifications for a specified target audience.
- The web application is a complete, functional, and usable application meeting design and development standards for performance, accessability, and usability.
- The application employs user authentication.
- The application employs CRUD functionality.
- The application is tested for quality assurance and operation.
- The group uses a Jira Project Board to manage application development tasks.
- The group uses a Github Repo to manage source code.
- Code comments and user documentation (a basic start and use guide are sufficient) are included.
- The application is deployed to a cloud service.

## 3. Scope Goals

- Implement OAuth
- Associate User Accounts to campaigns/games/groups
- View party inventory
- View Player inventory
- Permissions systems
- Dashboard to select campaigns
- Encounter creation to add items
- CRUD Routes
- Approval of transactions
- Object attributes
- Timestamping for the creation and update of objects

## 4. Scope Extensions/ Stretch Goals

- Enable user creation of templates
- Live updates via things like websocket
- Notification system for players
- GM can set universal permissions
- Invoicing - GM sends options to choose from, players select what they receive
- Pricing of items, sale value, purchase options, etc.
- Drag and drop UI
- Enable the usage of owned containers
- Historical log for encounters
- Allow more than one character per player

## 5. Out of Scope Non-Goals

- Not a full game management system
- No built in dice-roller
- No character stats tracking
- No live interface for game pieces
- No built chat/dialogue options
- Not a catalogue of all possible objects

## 6. Personas & User Stories

- **As a {GM (Admin)}**, I want to be able to create encounters in order to add experience, treasure, and items to the party.
- **As a {Quartermaster (Moderator)}**, I want to be able to assist the GM by editing encounters and party inventory, in addition to my own.
- **As a {Player (User)}**, I want to be able to move items from the party into my personal inventory, and manage my personal inventory.

## 7. Functional Requirements

Numbered, testable, written in MUST / SHOULD / MAY (RFC 2119) form.

- **FR-1.** The system MUST use OAuth for user verification.
- **FR-2.** The system MUST track party inventory.
- **FR-3.** The system MUST support a dashboard to allow users to switch views between multiple campaigns.
- **FR-4.** The system MUST view player inventory.
- **FR-5.** The system MUST allow admin to manage new campaigns.
- **FR-6.** The system MUST assign the GM position to first person who creates the campaign.
- **FR-7.** The system MUST have be able to handle permissions based on user role.
- **FR-8.** The system MUST enable the management of objects.
- **FR-9.** The system SHOULD allow GM to set permissions for the campaign.

## 8. Non-Functional Requirements

- **Performance** — LCP under 1.25 seconds in development environment.
- **Security** — Authentication handled through OAuth.
- **Accessibility** — WCAG 2.1 AA conformance for all UI added.
- **Responsive** — Mobile first web design with CLS within industry standards.
- **Scalability** — Easily handles groups between 1-12 users.
- **Reliability** — Maintains constant uptime while not under server maintanance. Users can access data within two minutes of server cold-start.
- **Maintainability** — Follows .NET Blazor standards and practices.
- **Internationalization** — Supports english, spanish, japanese, thai, korean, tagalog, and cantonese.
