# Gitflow Workflow Specification

> **ที่มา:** ปรับจาก `Your-Story/docs/gitflow-workflow.md` เมื่อ 2026-10-06 คำสั่ง build และ verify แก้ให้ตรงกับ repo นี้แล้ว (ต้นฉบับใช้ `CoPoject/CoPoject.slnx` และ `--screenshot`)
> **สถานะ:** ทีมเริ่มใช้ตั้งแต่ 2026-10-06 (ผู้ใช้สั่ง) — สร้าง `Develop` จาก `main` @ `0cb2715` แล้ว commit ก่อนหน้านั้นไม่ได้ใช้ Conventional Commits
> คำสั่งเกมเต็มอยู่ใน [CLAUDE.md](../CLAUDE.md) เอกสารนี้เป็นเจ้าของเฉพาะกติกา branch, commit และ merge

This document establishes the **Gitflow Workflow** for the **BePal** repository, modeled after the [Atlassian Gitflow Workflow](https://www.atlassian.com/git/tutorials/comparing-workflows/gitflow-workflow) (originally by Vincent Driessen) and adapted for our .NET / MonoGame project.

---

## 1. Overview & Branching Architecture

Gitflow assigns explicit responsibilities to branches and governs when and how they interact.

```
(main)       ──────────────────────────────● [v0.1.0] ───────────────────────● [v0.1.1] ───
                                           ▲                                 ▲
                                          ╱                                 ╱
(release)                       ╭────────●────────╮                        ╱
                               ╱                  ▼                       ╱
(Develop)    ──●─────●────────●───────────────────●──────●─────●─────────●───────────────
                ╲   ▲            ╲                      ▲       ▲       ▲
                 ▼ ╱              ▼                    ╱       ╱         ╲
(feature)         ● (feature/A)    ● (feature/B) ─────╯       ╱           ▼
                                                             ╱      (hotfix/v0.1.1)
(hotfix)                                    ────────────────╯
```

---

## 2. Core Branches

### `main` (Production Branch)
- **Role**: Official release history.
- **Rules**:
  - No direct commits to `main`.
  - Every commit on `main` is an official release or hotfix, tagged with a semantic version (e.g. `v0.1.0`, `v0.1.1`).
  - `main` must always pass the [pre-merge gates](#6-pre-merge-verification-gates).

### `Develop` (Integration Branch)
- **Role**: Integration hub for completed features.
- **Rules**:
  - Source branch for all new features and releases.
  - Receives merged features, release bugfix back-merges, and hotfix back-merges.
  - Must stay buildable (`dotnet build BEPAL/Bepal_Game/Bepal_Game.slnx`).

---

## 3. Supporting Branches

Deleted after being merged back into the core branches.

| Branch Type | Branch From | Merge Into | Naming Convention | Lifecycle |
|---|---|---|---|---|
| **Feature** | `Develop` | `Develop` | `feature/<topic>` or `feature/<issue-id>-<name>` | Deleted after merge |
| **Release** | `Develop` | `main` **AND** `Develop` | `release/v<version>` (e.g. `release/v0.2.0`) | Deleted after release |
| **Hotfix** | `main` | `main` **AND** `Develop` | `hotfix/v<version>` (e.g. `hotfix/v0.1.1`) | Deleted after hotfix |

---

## 4. Operational Workflows

### 4.1. Feature Branches (`feature/*`)

New gameplay mechanics, subsystems, screens, or UI components.

1. **Create from `Develop`**:
   ```bash
   git checkout Develop
   git pull origin Develop
   git checkout -b feature/care-wheel-decay
   ```
2. **Commit** with [Conventional Commits](#5-commit-message-conventions):
   ```bash
   git commit -m "feat(gameplay): implement satisfaction decay per care cycle"
   ```
3. **Verify locally** — run the [pre-merge gates](#6-pre-merge-verification-gates).
4. **Merge back into `Develop`** via Pull Request; after review and green checks, merge `--no-ff`:
   ```bash
   git push -u origin feature/care-wheel-decay
   # open PR -> Develop, then after approval:
   git checkout Develop
   git merge --no-ff feature/care-wheel-decay
   git push origin Develop
   git branch -d feature/care-wheel-decay
   git push origin --delete feature/care-wheel-decay
   ```

### 4.2. Release Branches (`release/*`)

When `Develop` has everything for a milestone (sprint end, demo).

1. **Create from `Develop`**:
   ```bash
   git checkout Develop
   git pull origin Develop
   git checkout -b release/v0.1.0
   ```
2. **Polish only** — allowed: bug fixes, docs, asset polish, version metadata. Forbidden: new features, significant refactoring.
3. **Merge into `main` and tag**:
   ```bash
   git checkout main
   git pull origin main
   git merge --no-ff release/v0.1.0
   git tag -a v0.1.0 -m "Release v0.1.0: Day 1-5 vertical slice prototype"
   git push origin main --tags
   ```
4. **Back-merge into `Develop`** so release fixes reach ongoing work:
   ```bash
   git checkout Develop
   git pull origin Develop
   git merge --no-ff release/v0.1.0
   git push origin Develop
   ```
5. **Clean up**:
   ```bash
   git branch -d release/v0.1.0
   git push origin --delete release/v0.1.0
   ```

### 4.3. Hotfix Branches (`hotfix/*`)

Critical defects in a release, without disturbing `Develop`.

1. **Create from `main`**:
   ```bash
   git checkout main
   git pull origin main
   git checkout -b hotfix/v0.1.1
   ```
2. **Minimal fix**, then run the [pre-merge gates](#6-pre-merge-verification-gates).
3. **Merge into `main` and tag**:
   ```bash
   git checkout main
   git merge --no-ff hotfix/v0.1.1
   git tag -a v0.1.1 -m "Hotfix v0.1.1: <what was fixed>"
   git push origin main --tags
   ```
4. **Back-merge** into the active `release/*` branch if one exists, otherwise into `Develop`:
   ```bash
   git checkout Develop
   git merge --no-ff hotfix/v0.1.1
   git push origin Develop
   ```
5. **Clean up**:
   ```bash
   git branch -d hotfix/v0.1.1
   git push origin --delete hotfix/v0.1.1
   ```

---

## 5. Commit Message Conventions

All commits follow Conventional Commits:

```
<type>(<scope>): <short summary>

[optional body: motivation, domain context, design rationale]

[optional footer: Closes #123, Refs #456]
```

| Type | Use for |
|---|---|
| `feat` | New gameplay mechanic, screen, or domain state model |
| `fix` | Bug in simulation math, QTE logic, graphics, or asset pipeline |
| `docs` | GDD, Agile, Context, workflow docs |
| `style` | Formatting, whitespace, naming (no logic change) |
| `refactor` | Restructuring without changing gameplay behavior |
| `test` | Unit tests, playtest/autoplay harness, regression checks |
| `chore` | Build, NuGet, MGCB config, repo tooling |

---

## 6. Pre-Merge Verification Gates

Before any branch is merged into `Develop` or `main` (run from the repo root):

1. **Compilation** — 0 errors and 0 warnings:
   ```powershell
   dotnet build BEPAL/Bepal_Game/Bepal_Game.slnx
   ```
2. **Autoplay (soft-lock check)** — output must contain `RESULT: reached To be continued` (`TIMEOUT` = soft-lock); repeat with `--refuse` when Day 3 merchant/shop is touched:
   ```powershell
   dotnet run --project BEPAL/Bepal_Game/Bepal -- --autoplay
   ```
3. **Visual regression (screenshots)** — every screen renders to PNG without error; inspect the images after UI changes. Write to a folder outside the repo:
   ```powershell
   dotnet run --project BEPAL/Bepal_Game/Bepal -- --shots "$env:TEMP/bepal_shots"
   ```
4. **Unit tests** (only once a test project exists):
   ```powershell
   dotnet test BEPAL/Bepal_Game/Bepal_Game.slnx
   ```
5. **PR review** — every merge into `Develop` or `main` goes through a Pull Request reviewed by a team member or an approved agent.
