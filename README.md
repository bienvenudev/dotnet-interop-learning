# .NET Interoperability Fundamentals

Projects from Pluralsight's ".NET Interoperability Fundamentals" course — P/Invoke,
native interop with Win32 APIs, and (eventually) COM interop / C++/CLI.

Note: the course was recorded in 2014, so some examples (e.g. processor architecture
values, tooling) are dated. Modern real-world equivalent noted where relevant (e.g. CsWin32
as the current standard approach vs. hand-translating native structs).

## Structure

- `01-is32bit-process/` — P/Invoke basics: calling `kernel32`, translating a native struct
  (`SYSTEM_INFO`), determining process architecture (32-bit vs. WOW64).
- `02-enumerate-windows/` — callback-based P/Invoke: `EnumWindows` with a delegate,
  string marshaling for window titles/visibility.
- `03-calculator-sendinput/` — the hardest case: native union struct marshaling
  (`INPUT`/`KEYBDINPUT`) for `SendInput`. Hit real marshaling bugs from hand-translating the
  union layout, traced to a struct size mismatch, resolved by switching to CsWin32
  (Microsoft's official Win32 binding generator) instead of hand-rolled `DllImport`.

## Progress

- [x] P/Invoke fundamentals
- [x] Struct marshaling (simple + union)
- [x] Callback/delegate-based P/Invoke
- [ ] COM interop
- [ ] C++/CLI
