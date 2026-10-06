---
name: read-readme
description: Read README.md at the repo root and follow its "AI agent session start" order (AGENTS.md → Context/Rules.md + Status.md → Quest Board → relevant quests → task-specific docs). Use at the start of a session, or when asked to read README.md / get oriented in the BePal repo.
---

# Read README.md (session start)

1. Read `README.md` at the repo root (`d:\CoProject\CoProjectBepal\README.md`).
2. Summarize in Thai, briefly: what the project is, where code/GDD/Agile live, and the reading order.
3. Follow the reading order from README, in this order, only as far as the task needs:
   1. `AGENTS.md`
   2. `Context/Rules.md` + `Context/Status.md`
   3. Active Quest Board — per `Context/Agent-Handoff.md`, read only the **name, status, owner** of every file in `Context/quests/`
   4. Details of quests relevant to the task, or whose scope (Files) overlaps the work
   5. Task-specific docs/source, chosen via `Context/README.md`
4. Do NOT auto-read: `Context/Edit-Log.md`, `Context/archive/`, the whole GDD/Agile sets — open them only when the task needs them.
5. The Quest Board is context, not an order to resume old work. Don't start a quest unless the user asks.

If the user only said "read README", stop after step 2 and ask whether to continue with step 3.
