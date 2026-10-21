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
- Historical data
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

- **FR-1.** The system MUST …
- **FR-2.** The system MUST …
- **FR-3.** The system SHOULD …

## 8. Non-Functional Requirements

- **Performance** — p95 latency targets, throughput, payload size limits.
- **Security** — authn/authz model, threat-model notes, encryption.
- **Privacy & Compliance** — FERPA / COPPA / GDPR / WCAG / SOC 2 obligations.
- **Accessibility** — WCAG 2.1 AA conformance for all UI added.
- **Scalability** — expected load, partitioning strategy.
- **Reliability** — availability target, failure modes, idempotency.
- **Observability** — required metrics, log fields, traces, alerts.
- **Maintainability** — coding conventions, owned modules.
- **Internationalization** — strings externalised, tz/locale handled.
- **Backward compatibility** — migration & deprecation policy.
