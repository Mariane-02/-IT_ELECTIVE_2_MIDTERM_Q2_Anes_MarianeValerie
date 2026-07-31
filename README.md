# Protected YouTube Playlist Builder

ASP.NET Core MVC app with session-based auth, DTO-protected forms, and indexed
model binding for a dynamic (JS-free) playlist track list.

**Login:** `admin` / `password123`

## Module 3: Bug Hunt Findings

### Bug 1 — The "Vanishing Inputs" State Bug
**Reproduce:** Start creating a playlist, type a URL into Track #1, click
"+ Add Another Track". The new row appears, but Track #1's URL disappears.

**Root cause:** `AddVideoRow` calls `ModelState.Clear()` before returning
`View("Create", dto)`. Razor's tag/input helpers render a field's value from
`ModelState` first if an entry exists, falling back to the model only when
`ModelState` has nothing for that key. Without clearing, ASP.NET Core keeps
stale `ModelState` entries from the previous request that no longer line up
with the newly-indexed `Videos` list, causing rows to render blank or
misaligned.

**Lesson:** Any time a server-side postback reshapes the model (adding or
removing list items), call `ModelState.Clear()` before re-rendering the view
so inputs are bound purely from the fresh model, not leftover POST data.

### Bug 2 — The "Bypassed Security" Bug
**Reproduce:** Log out, then type `/Playlist/Create` directly into the
browser address bar.

**Root cause:** `AuthorizeSessionAttribute.OnActionExecuting` checks the
session and, when missing, sets `context.Result = new
RedirectToActionResult("Login", "Auth", null)`. Setting `context.Result` is
what actually short-circuits the pipeline — if that line were removed (or if
a custom filter merely returned without setting `Result`), the action would
still execute even though the user isn't authenticated.

**Lesson:** An `ActionFilter` doesn't block execution just because it
contains conditional logic — it must explicitly set `context.Result` to halt
the pipeline early.

### Bug 3 — The "Broken Validation Trap"
**Reproduce:** Leave "Playlist Title" blank, add 3 tracks, click "Save
Complete Playlist" — validation correctly blocks it. Now leave it blank and
click "+ Add Another Track" instead.

**Root cause:** The "Add Track" and "Remove" buttons use `formnovalidate`,
which disables the browser's built-in HTML5 validation for that specific
submit action, letting the incomplete form post to `AddVideoRow`/
`RemoveVideoRow` regardless of client-side errors. This is intentional here,
since those actions only reshape the row list and shouldn't be blocked by
validation — only the final `Save` button (no `formnovalidate`) needs the
title to be valid.

**Lesson:** `formnovalidate` lets you have multiple submit buttons on one
form with different validation requirements — but it's a client-side-only
bypass, so server-side `ModelState.IsValid` checks in `Save` are still the
real safety net.
