# TernakKu — UI prototype

Clickable offline prototype of the TernakKu WPF app. Open `index.html` in Edge or Chrome; no internet or install needed.

## Demo accounts (database akun, `ternakku.db`)

| Username | Role | Password |
|---|---|---|
| admin | Admin | ternakku |
| petugas | Petugas kandang | ternakku |
| mantri | Mantri | ternakku |

"Masuk sebagai tamu" opens a separate guest database (`tamu.db`) with its own sample data.
Both databases are kept in the browser's localStorage under different keys (`ternakku.db.akun`, `ternakku.db.tamu`).

## Reset

Clear the site data for the file in the browser (or run `localStorage.clear()` in DevTools) to start from fresh sample data.
