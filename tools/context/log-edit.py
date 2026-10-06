#!/usr/bin/env python3
"""Record one Edit Log entry in Context/Edit-Log.md (see Context/Rules.md#edit-log).

Usage:
  python tools/context/log-edit.py --quest Q-ID --files "path/a, path/b" \
      --summary "แก้อะไรและเพราะอะไร" --validation "ผลตรวจและข้อจำกัด"

- Keeps the newest MAX_ENTRIES entries (3 lines each); older ones move to
  Context/archive/edit-log/YYYY-MM.md (by the entry's own date).
- Same Quest ID -> the old entry is replaced and the new one goes on top.
- Exclusive lock file + atomic replace so several agents can call it at once.
  A busy lock is waited on, never deleted.
"""
from __future__ import annotations

import argparse
import datetime as dt
import os
import re
import socket
import sys
import tempfile
import time
from pathlib import Path

MAX_ENTRIES = 10
MAX_FIELD = 400
MARKER = "<!-- entries -->"
QUEST_RE = re.compile(r"^Q-\d{8}-[a-z0-9]+(?:-[a-z0-9]+)*$")
ENTRY_RE = re.compile(r"^- \*\*(\d{4}-\d{2}-\d{2}) \d{2}:\d{2} · (Q-[^*]+)\*\* — ")
SECRET_RES = [
    re.compile(r"(?i)\b(pass(word|wd)?|secret|token|api[_-]?key|auth)\b\s*[:=]\s*\S+"),
    re.compile(r"-----BEGIN [A-Z ]*PRIVATE KEY-----"),
    re.compile(r"\b(sk|pk|rk)-[A-Za-z0-9_-]{16,}"),
    re.compile(r"\bgh[pousr]_[A-Za-z0-9]{20,}"),
    re.compile(r"\bAKIA[0-9A-Z]{16}\b"),
    re.compile(r"\beyJ[A-Za-z0-9_-]{10,}\.[A-Za-z0-9_-]{10,}\."),
    re.compile(r"\bxox[abpr]-[A-Za-z0-9-]{10,}"),
]


class InputError(Exception):
    pass


def clean(name: str, value: str) -> str:
    text = " ".join(value.split())  # one line: no newline/markdown-entry injection
    if not text:
        raise InputError(f"--{name} must not be empty")
    if len(text) > MAX_FIELD:
        raise InputError(f"--{name} is {len(text)} chars; keep it under {MAX_FIELD} and link long evidence")
    for rx in SECRET_RES:
        if rx.search(text):
            raise InputError(f"--{name} looks like it contains a secret/token; remove it")
    return text


def clean_files(raw: str, root: Path) -> str:
    items = [p.strip().replace("\\", "/") for p in raw.split(",")]
    items = [p for p in items if p]
    if not items:
        raise InputError("--files must list at least one path")
    for p in items:
        if p.lower() in {"all", "*", "**", "."}:
            raise InputError(f"--files: '{p}' is not an explicit path")
        if Path(p).is_absolute() or re.match(r"^[A-Za-z]:", p):
            raise InputError(f"--files: use repo-relative paths, got '{p}'")
        resolved = (root / p.replace("**", "x").replace("*", "x")).resolve()
        if root.resolve() not in (resolved, *resolved.parents):
            raise InputError(f"--files: '{p}' points outside the repository")
    return clean("files", ", ".join(items))


def atomic_write(path: Path, text: str) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    fd, tmp = tempfile.mkstemp(dir=path.parent, prefix=f".{path.name}.", suffix=".tmp")
    try:
        with os.fdopen(fd, "w", encoding="utf-8", newline="\n") as f:
            f.write(text)
            f.flush()
            os.fsync(f.fileno())
        for attempt in range(20):  # Windows: replace fails while a reader holds the file
            try:
                os.replace(tmp, path)
                return
            except PermissionError:
                if attempt == 19:
                    raise
                time.sleep(0.1)
    finally:
        if os.path.exists(tmp):
            os.remove(tmp)


class Lock:
    def __init__(self, path: Path, timeout: float):
        self.path, self.timeout, self.held = path, timeout, False

    def __enter__(self):
        deadline = time.monotonic() + self.timeout
        while True:
            try:
                fd = os.open(self.path, os.O_CREAT | os.O_EXCL | os.O_WRONLY)
            except FileExistsError:
                if time.monotonic() >= deadline:
                    raise TimeoutError(
                        f"lock busy for {self.timeout:.0f}s: {self.path}\n"
                        "Another agent may be writing. Not removing it; if it is stale, ask the user."
                    )
                time.sleep(0.2)
                continue
            with os.fdopen(fd, "w", encoding="utf-8") as f:
                f.write(f"pid={os.getpid()} host={socket.gethostname()} at={dt.datetime.now().isoformat()}\n")
            self.held = True
            return self

    def __exit__(self, *exc):
        if self.held:
            os.remove(self.path)  # only ever removes the lock this process created


def split_entries(body: str) -> list[list[str]]:
    entries: list[list[str]] = []
    for line in body.splitlines():
        if ENTRY_RE.match(line):
            entries.append([line])
        elif entries and line.startswith("  "):
            entries[-1].append(line)
    return entries


def archive(root: Path, old: list[list[str]]) -> None:
    by_month: dict[str, list[list[str]]] = {}
    for e in old:
        by_month.setdefault(ENTRY_RE.match(e[0]).group(1)[:7], []).append(e)
    for month, entries in by_month.items():
        path = root / "Context" / "archive" / "edit-log" / f"{month}.md"
        current = path.read_text(encoding="utf-8") if path.exists() else (
            f"# Edit Log archive — {month}\n\nSnapshot ประวัติ ไม่ใช้ยืนยันสถานะปัจจุบัน ใหม่สุดอยู่บน\n\n"
        )
        head, _, tail = current.partition("\n\n- **")
        tail = ("- **" + tail) if _ else ""
        block = "\n".join("\n".join(e) for e in entries)
        atomic_write(path, head.rstrip("\n") + "\n\n" + block + ("\n" + tail if tail else "\n"))


def main(argv: list[str] | None = None) -> int:
    ap = argparse.ArgumentParser(description="Add/update one entry in Context/Edit-Log.md")
    ap.add_argument("--quest", required=True)
    ap.add_argument("--files", required=True)
    ap.add_argument("--summary", required=True)
    ap.add_argument("--validation", required=True)
    ap.add_argument("--lock-timeout", type=float, default=30.0)
    ap.add_argument("--root", help=argparse.SUPPRESS)  # tests only
    args = ap.parse_args(argv)

    root = Path(args.root) if args.root else Path(__file__).resolve().parents[2]
    log = root / "Context" / "Edit-Log.md"
    try:
        quest = args.quest.strip()
        if not QUEST_RE.match(quest):
            raise InputError(f"--quest '{quest}' must look like Q-YYYYMMDD-short-name")
        files = clean_files(args.files, root)
        summary = clean("summary", args.summary)
        validation = clean("validation", args.validation)
    except InputError as e:
        print(f"error: {e}", file=sys.stderr)
        return 2

    stamp = dt.datetime.now().strftime("%Y-%m-%d %H:%M")
    entry = [f"- **{stamp} · {quest}** — {summary}", f"  - Files: {files}", f"  - Validation: {validation}"]

    try:
        with Lock(root / "Context" / ".edit-log.lock", args.lock_timeout):
            if not log.exists():
                print(f"error: {log} not found", file=sys.stderr)
                return 1
            text = log.read_text(encoding="utf-8")
            if MARKER not in text:
                print(f"error: '{MARKER}' missing in {log}; not rewriting it", file=sys.stderr)
                return 1
            header, _, body = text.partition(MARKER)
            entries = [e for e in split_entries(body) if ENTRY_RE.match(e[0]).group(2) != quest]
            entries.insert(0, entry)
            keep, old = entries[:MAX_ENTRIES], entries[MAX_ENTRIES:]
            if old:
                archive(root, old)
            atomic_write(log, header + MARKER + "\n" + "\n".join("\n".join(e) for e in keep) + "\n")
    except TimeoutError as e:
        print(f"error: {e}", file=sys.stderr)
        return 3

    print(f"logged {quest} -> {log.relative_to(root)}" + (f" (archived {len(old)})" if old else ""))
    return 0


if __name__ == "__main__":
    if hasattr(sys.stdout, "reconfigure"):
        sys.stdout.reconfigure(encoding="utf-8")
        sys.stderr.reconfigure(encoding="utf-8")
    sys.exit(main())
