---
name: ship-branch
description: Ship the current feature/* branch per Context/Gitflow.md — verify the work is complete (quest, Edit Log, pre-merge gates), commit, push, open and merge a PR into Develop, then delete the merged branch (local + remote) only after confirming it merged. Use when the user says to commit/push/merge a finished feature, "ship it", or to close out a branch.
---

# Ship branch (commit → push → PR → merge → delete branch)

Rules live in `Context/Gitflow.md` (branches, Conventional Commits, gates) and `Context/Rules.md` (quest closing). This skill is the checklist; follow it in order and stop at the first failed step. Invoking this skill is the user's go-ahead to commit/push/merge **the current `feature/*` branch into `Develop`** only. Anything else (release/hotfix, merging into `main`, force-push, deleting other branches) → ask first.

## 0. Preflight
- `git branch --show-current` must be `feature/<topic>`. On `main`/`Develop` → stop and ask (no direct commits there).
- `git status -s` and `git diff --stat`: every changed file must belong to this task (own quest's Files). Uncommitted files with no owner quest are someone else's work → don't stage them; tell the user.
- Never use `git add -A`/`.`; stage explicit paths.

## 1. Completeness check (before anything is committed or deleted)
Report each item as done / not done; any "not done" → fix it or ask the user, don't ship.
1. **Goal met:** re-read the quest's Goal (`Context/quests/Q-*.md`) and the user's request in this conversation; confirm each point is implemented. No leftover TODOs or half-finished edits in the diff.
2. **Gates** (from repo root, `Context/Gitflow.md` §6):
   - `dotnet build BEPAL/Bepal_Game/Bepal_Game.slnx` → 0 errors, 0 warnings
   - `dotnet run --project BEPAL/Bepal_Game/Bepal -- --autoplay` → `RESULT: reached To be continued` (log path is printed; `%TEMP%/bepal_autoplay.log`)
   - add `-- --autoplay --refuse` if Day 3 merchant/shop was touched
   - `-- --shots "$env:TEMP/bepal_shots"` and look at the affected PNGs if UI changed
   Re-run gates after the *last* code edit; a gate result from before later edits doesn't count.
3. **Docs in sync:** if scenes/folders were added or renamed → `BEPAL/Docs/GDD/04-class-diagram.md` updated; if DevTools-coupled members changed → `DevTools/` updated; new scene → `ShotRunner` entry.
4. **Quest + Edit Log:** quest file has real Validation and Next action; Status `ready-to-close`; one Edit Log entry via `python tools/context/log-edit.py --quest Q-ID --files "..." --summary "..." --validation "..."`. If the quest isn't confirmed closed by the user, leave it in `Context/quests/` (ready-to-close) and skip the archive step below.

## 2. Commit
- Conventional Commits `<type>(<scope>): <summary>` (feat/fix/docs/style/refactor/test/chore); body = why, short.
- Code commit first, then a separate `docs(context): close Q-ID` commit that moves the quest to `Context/archive/quests/` (`git mv`, after checking source is in `Context/quests/` and target in `Context/archive/quests/`) and carries the Edit Log change. Only archive when the user has said to close it.
- End each commit message with the attribution line from the session's system reminder (Co-Authored-By …).
- Pass messages via heredoc (`git commit -F - <<'EOF'`).

## 3. Push
`git push -u origin <branch>` (no `--force`). If rejected → `git pull --rebase origin Develop`/inspect, don't overwrite.

## 4. PR and merge into Develop
- `gh pr create --base Develop --head <branch> --title "<commit summary>" --body ...` — body: Summary, Test plan (gates actually run, with results; say which were not run), then `🤖 Generated with [Claude Code](https://claude.com/claude-code)`.
- `gh pr merge --merge` (merge commit, like earlier PRs; Gitflow says `--no-ff`). Don't use squash/rebase.
- If merge is blocked (required review/checks/conflicts) → report why and stop; don't bypass with `--admin`.

## 5. Verify merge, then delete the branch
Delete **only after** all of these pass:
1. `gh pr view <n> --json state,mergeCommit` → `state: MERGED`.
2. `git fetch origin` then `git branch -r --merged origin/Develop` lists `origin/<branch>` (or the PR's tip commit is an ancestor of `origin/Develop`).
3. `git status -s` clean and no unpushed commits on the branch (`git log origin/<branch>..<branch>` empty).
4. Step 1 checklist has no open item.

Then:
```sh
git checkout Develop
git pull origin Develop
git branch -d <branch>                 # -d, never -D; if it refuses, stop and investigate
git fetch --prune                      # drops stale origin/<branch> refs
git ls-remote --heads origin <branch>  # empty = remote already deleted; else delete it:
git push origin --delete <branch>
```
Step 4 deliberately omits `gh pr merge --delete-branch` (it deletes the remote branch at merge time, before these checks). If it was used anyway, the remote is already gone — `ls-remote` shows that; `git push origin --delete` on a missing branch errors with "remote ref does not exist" (harmless, not a failed step).
Finish by showing `git log --oneline -4` and `git branch -a`.

## 6. Report (Thai, concise)
Commits (hash + subject), PR link, gates run vs not run, branch deleted (local/remote), and what's left: **don't** merge `Develop` → `main` here — that's a release (`Context/Gitflow.md` §4.2); mention it only as a next step.
