# tipe

A minimal Terminal User Interface (TUI) app built with [Terminal.Gui](https://github.com/tui-cs/Terminal.Gui) **v2**.

```bash
dotnet run        # launch (Esc or the Quit button exits)
dotnet build      # compile only
```

Run the piece-table performance benchmarks in Release mode:

```bash
dotnet run --project tipe.Benchmarks -c Release
```

The benchmarks generate a deterministic 100-word document, wrap lines at 90
characters, and measure adding 1, 10, 100, or 1,000 words, looking up a line,
and generating the full document. Additions are prepared deterministically and
the benchmark parameter controls the number of items in the add list.

## Project layout
| File | Purpose |
|---|---|
| `Program.cs` | App entry point + the root `MainWindow` view. Add your views here. |
| `tipe.Benchmarks/` | BenchmarkDotNet performance tests for the piece table. |
| `AGENTS.md` | **Canonical Terminal.Gui v2 patterns + gotchas for AI agents and humans.** Start here. |
| `CLAUDE.md` | Thin pointer to `AGENTS.md` for Claude / Claude Code. |
| `.cursorrules` / `.windsurfrules` / `.aider.md` | Auto-loaded rules for Cursor / Windsurf / Aider — short banner that points at `AGENTS.md`. |
| `*.csproj` | Targets `net10.0`, references `Terminal.Gui`. |

## Building with an AI agent?
Terminal.Gui v2 is a complete rewrite — most examples online (and in model training data)
are **v1 and won't compile**. **Read [`AGENTS.md`](./AGENTS.md) first**: it has the canonical
minimal app, the v1→v2 corrections table, `Pos`/`Dim` layout, the common pitfalls, and links
to Terminal.Gui's CI-validated docs.

## Learn more
- Getting started: https://tui-cs.github.io/Terminal.Gui/
- Repository & samples: https://github.com/tui-cs/Terminal.Gui
- v1→v2 primer: https://github.com/tui-cs/Terminal.Gui/blob/develop/ai-v2-primer.md
