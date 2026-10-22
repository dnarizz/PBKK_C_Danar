// TernakKu clickable prototype. Plain JS, no dependencies, works from file://.

const DEMO_PW = 'ternakku';
const KEY_SESSION = 'ternakku.session';
const KEY_SETTINGS = 'ternakku.settings';
const keyDb = (kind) => `ternakku.db.${kind}`;

const store = {
  get(k) { try { const v = localStorage.getItem(k); return v ? JSON.parse(v) : null; } catch { return null; } },
  set(k, v) { try { localStorage.setItem(k, JSON.stringify(v)); } catch { /* storage unavailable */ } },
  del(k) { try { localStorage.removeItem(k); } catch { /* storage unavailable */ } }
};

/* ---------------- Icons ---------------- */
const IC = {
  leaf: '<path d="M11 20A7 7 0 0 1 9.8 6.1C15.5 5 17 4.5 19 2c1 2 2 4.2 2 8 0 5.5-4.8 10-10 10Z"/><path d="M2 21c0-3 1.9-5.4 5.1-6"/>',
  dashboard: '<rect x="3" y="3" width="7" height="9" rx="1.5"/><rect x="14" y="3" width="7" height="5" rx="1.5"/><rect x="14" y="12" width="7" height="9" rx="1.5"/><rect x="3" y="16" width="7" height="5" rx="1.5"/>',
  tag: '<path d="M12 2H2v10l9.3 9.3a1 1 0 0 0 1.4 0l8.6-8.6a1 1 0 0 0 0-1.4Z"/><circle cx="7" cy="7" r="1.5"/>',
  home: '<path d="M3 10 12 3l9 7v10a1 1 0 0 1-1 1H4a1 1 0 0 1-1-1Z"/><path d="M9 21v-6h6v6"/>',
  box: '<path d="M21 8 12 3 3 8v8l9 5 9-5Z"/><path d="m3 8 9 5 9-5"/><path d="M12 13v8"/>',
  heart: '<path d="M19 14c1.5-1.5 3-3.2 3-5.5A5.5 5.5 0 0 0 16.5 3c-1.8 0-3 .5-4.5 2-1.5-1.5-2.7-2-4.5-2A5.5 5.5 0 0 0 2 8.5c0 2.3 1.5 4 3 5.5l7 7Z"/><path d="M3.2 12h4.3l1.5-3 3 6 1.5-3h4.3"/>',
  repro: '<circle cx="6" cy="6" r="3"/><circle cx="18" cy="6" r="3"/><circle cx="12" cy="18" r="3"/><path d="M6 9v1a2 2 0 0 0 2 2h8a2 2 0 0 0 2-2V9"/><path d="M12 12v3"/>',
  swap: '<path d="M8 3 4 7l4 4"/><path d="M4 7h16"/><path d="m16 21 4-4-4-4"/><path d="M20 17H4"/>',
  wallet: '<rect x="2" y="6" width="20" height="14" rx="2"/><path d="M2 10h20"/><path d="M16 15h2"/>',
  file: '<path d="M14 3H6a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V9Z"/><path d="M14 3v6h6"/><path d="M8 13h8"/><path d="M8 17h5"/>',
  db: '<ellipse cx="12" cy="5" rx="8" ry="3"/><path d="M4 5v14c0 1.7 3.6 3 8 3s8-1.3 8-3V5"/><path d="M4 12c0 1.7 3.6 3 8 3s8-1.3 8-3"/>',
  users: '<circle cx="9" cy="8" r="4"/><path d="M2 21a7 7 0 0 1 14 0"/><path d="M16 4a4 4 0 0 1 0 8"/><path d="M22 21a7 7 0 0 0-4-6.3"/>',
  settings: '<circle cx="12" cy="12" r="3"/><path d="M19.4 15a1.7 1.7 0 0 0 .3 1.8l.1.1a2 2 0 1 1-2.8 2.8l-.1-.1a1.7 1.7 0 0 0-1.8-.3 1.7 1.7 0 0 0-1 1.5V21a2 2 0 1 1-4 0v-.1a1.7 1.7 0 0 0-1.1-1.5 1.7 1.7 0 0 0-1.8.3l-.1.1a2 2 0 1 1-2.8-2.8l.1-.1a1.7 1.7 0 0 0 .3-1.8 1.7 1.7 0 0 0-1.5-1H3a2 2 0 1 1 0-4h.1a1.7 1.7 0 0 0 1.5-1.1 1.7 1.7 0 0 0-.3-1.8l-.1-.1a2 2 0 1 1 2.8-2.8l.1.1a1.7 1.7 0 0 0 1.8.3H9a1.7 1.7 0 0 0 1-1.5V3a2 2 0 1 1 4 0v.1a1.7 1.7 0 0 0 1 1.5 1.7 1.7 0 0 0 1.8-.3l.1-.1a2 2 0 1 1 2.8 2.8l-.1.1a1.7 1.7 0 0 0-.3 1.8V9a1.7 1.7 0 0 0 1.5 1H21a2 2 0 1 1 0 4h-.1a1.7 1.7 0 0 0-1.5 1Z"/>',
  logout: '<path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4"/><path d="m16 17 5-5-5-5"/><path d="M21 12H9"/>',
  search: '<circle cx="11" cy="11" r="7"/><path d="m20 20-3.5-3.5"/>',
  bell: '<path d="M6 8a6 6 0 0 1 12 0c0 7 3 9 3 9H3s3-2 3-9"/><path d="M10.3 21a1.94 1.94 0 0 0 3.4 0"/>',
  plus: '<path d="M12 5v14M5 12h14"/>',
  x: '<path d="M18 6 6 18M6 6l12 12"/>',
  check: '<path d="M20 6 9 17l-5-5"/>',
  eye: '<path d="M2 12s3.5-7 10-7 10 7 10 7-3.5 7-10 7S2 12 2 12Z"/><circle cx="12" cy="12" r="3"/>',
  eyeOff: '<path d="M9.9 4.2A10 10 0 0 1 12 4c7 0 10 8 10 8a17 17 0 0 1-3.2 4.2"/><path d="M6.6 6.6A17 17 0 0 0 2 12s3 8 10 8a9.7 9.7 0 0 0 5.4-1.6"/><path d="M2 2l20 20"/><path d="M9.9 9.9a3 3 0 0 0 4.2 4.2"/>',
  lock: '<rect x="3" y="11" width="18" height="11" rx="2"/><path d="M7 11V7a5 5 0 0 1 10 0v4"/>',
  user: '<circle cx="12" cy="8" r="4"/><path d="M4 21a8 8 0 0 1 16 0"/>',
  guest: '<circle cx="12" cy="12" r="10"/><circle cx="12" cy="10" r="3"/><path d="M6.2 18.8a7 7 0 0 1 11.6 0"/>',
  alert: '<path d="M10.3 3.9 1.8 18a2 2 0 0 0 1.7 3h17a2 2 0 0 0 1.7-3L13.7 3.9a2 2 0 0 0-3.4 0Z"/><path d="M12 9v4"/><path d="M12 17h.01"/>',
  calendar: '<rect x="3" y="4" width="18" height="18" rx="2"/><path d="M16 2v4M8 2v4M3 10h18"/>',
  scale: '<path d="M12 3v18"/><path d="M7 21h10"/><path d="M5 7h14"/><path d="m5 7-3 7a3 3 0 0 0 6 0Z"/><path d="m19 7-3 7a3 3 0 0 0 6 0Z"/>',
  syringe: '<path d="m18 2 4 4"/><path d="m17 7 3-3"/><path d="M19 9 8.7 19.3a2.4 2.4 0 0 1-3.4 0l-.6-.6a2.4 2.4 0 0 1 0-3.4L15 5"/><path d="m9 11 4 4"/><path d="m5 19-3 3"/><path d="m14 4 6 6"/>',
  download: '<path d="M12 3v12"/><path d="m7 10 5 5 5-5"/><path d="M5 21h14"/>',
  upload: '<path d="M12 15V3"/><path d="m7 8 5-5 5 5"/><path d="M5 21h14"/>',
  chevL: '<path d="m15 18-6-6 6-6"/>',
  chevR: '<path d="m9 18 6-6-6-6"/>',
  sun: '<circle cx="12" cy="12" r="4"/><path d="M12 2v2M12 20v2M4.9 4.9l1.4 1.4M17.7 17.7l1.4 1.4M2 12h2M20 12h2M4.9 19.1l1.4-1.4M17.7 6.3l1.4-1.4"/>',
  moon: '<path d="M21 12.8A9 9 0 1 1 11.2 3a7 7 0 0 0 9.8 9.8Z"/>',
  arrowR: '<path d="M5 12h14"/><path d="m13 6 6 6-6 6"/>',
  edit: '<path d="M12 20h9"/><path d="M16.5 3.5a2.1 2.1 0 0 1 3 3L7 19l-4 1 1-4Z"/>',
  info: '<circle cx="12" cy="12" r="10"/><path d="M12 16v-4"/><path d="M12 8h.01"/>',
  trend: '<path d="m22 7-8.5 8.5-5-5L2 17"/><path d="M16 7h6v6"/>',
  excel: '<rect x="3" y="3" width="18" height="18" rx="2"/><path d="M3 9h18M3 15h18M9 3v18"/>',
  restore: '<path d="M3 12a9 9 0 1 0 3-6.7L3 8"/><path d="M3 3v5h5"/>',
  key: '<circle cx="7.5" cy="15.5" r="4.5"/><path d="m10.7 12.3 9.3-9.3"/><path d="m16 7 3 3"/><path d="m19 4 2 2"/>'
};

function icon(name, size = 18, sw = 1.75) {
  return `<svg width="${size}" height="${size}" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="${sw}" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">${IC[name] || ''}</svg>`;
}

const COW = `<svg viewBox="0 0 64 64" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><path d="M14 18c-5 0-8-3-8-6 4 0 7 1 9 3"/><path d="M50 18c5 0 8-3 8-6-4 0-7 1-9 3"/><path d="M18 14c2-3 6-5 14-5s12 2 14 5c2 4 2 10 0 16-1 4-1 9-2 13-1 6-6 11-12 11s-11-5-12-11c-1-4-1-9-2-13-2-6-2-12 0-16Z"/><circle cx="25" cy="27" r="1.6" fill="currentColor"/><circle cx="39" cy="27" r="1.6" fill="currentColor"/><path d="M24 44c0-4 4-6 8-6s8 2 8 6-4 7-8 7-8-3-8-7Z"/><circle cx="29" cy="45" r="1" fill="currentColor"/><circle cx="35" cy="45" r="1" fill="currentColor"/></svg>`;

const SCENE = `<svg class="scene" viewBox="0 0 240 140" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
  <circle cx="198" cy="32" r="13" fill="var(--leaf)" stroke="none" opacity="0.55"/>
  <path d="M0 120c40-14 80-14 120-6s80 10 120-4" />
  <path d="M74 64 120 30l46 34" />
  <rect x="82" y="60" width="76" height="58" rx="2" fill="var(--primary-soft)" />
  <rect x="112" y="46" width="16" height="12" rx="1.5" />
  <rect x="104" y="86" width="32" height="32" />
  <path d="m104 86 32 32M136 86l-32 32" />
  <path d="M180 98v20M196 98v20M212 98v20M228 98v18M174 104h60M174 112h60" />
  <circle cx="40" cy="78" r="15" fill="var(--leaf)" stroke="none" opacity="0.55"/>
  <path d="M40 84v34" />
</svg>`;

// Three line-art scenes (viewBox 0 0 260 110): barn and tree, windmill and silo, hay bales and cart.
const SCENES = [
  `<path d="M150 52 182 28l32 24"/><rect x="156" y="50" width="52" height="40" rx="1.5"/><rect x="174" y="66" width="16" height="24"/><path d="m174 66 16 24M190 66l-16 24"/>
   <path d="M10 82v16M28 82v16M46 82v16M64 82v16M82 82v16M4 87h84M4 94h84"/>
   <circle cx="236" cy="60" r="12"/><path d="M236 72v24"/>
   <path d="M110 98c2-8 4-12 8-14M116 98c0-6 2-9 5-11M122 98c-1-5-3-8-6-9"/>`,
  `<path d="M120 98V44a14 14 0 0 1 28 0v54"/><path d="M120 58h28M120 72h28M120 86h28"/><path d="M134 30v-4"/>
   <path d="M196 98l14-58 14 58M201 78h18M205 60h10"/><circle cx="210" cy="36" r="3"/>
   <path d="M210 36l-16-9M210 36l16-9M210 36v-18M210 36l-16 9M210 36l16 9"/><path d="M213 36h20l4-4v8l-4-4"/>
   <path d="M20 88h46l-4 10H24Z"/><path d="M14 88h58"/><path d="M80 98c1-7 3-10 6-12M86 98c0-5 2-8 4-9"/>
   <path d="M160 98v-14M176 98v-14M156 88h24M156 94h24"/>`,
  `<circle cx="36" cy="84" r="14"/><path d="M36 84m-7 0a7 7 0 1 0 7-7"/><circle cx="68" cy="89" r="9"/><path d="M68 89m-4 0a4 4 0 1 0 4-4"/>
   <path d="M118 80h58v-14h-58Z"/><path d="M120 66c6-12 18-14 26-6 8-10 22-8 28 6"/><circle cx="134" cy="88" r="10"/><path d="M134 78v20M124 88h20M127 81l14 14M141 81l-14 14"/>
   <path d="M176 72l28-8"/><path d="M200 28q5-6 10 0q5-6 10 0M224 16q4-5 8 0q4-5 8 0"/>
   <path d="M222 98c2-9 5-13 9-15M229 98c0-6 2-10 6-12M236 98c-1-6-3-9-7-10"/>`
];
const sceneSvg = (i, cls) => `<svg class="${cls}" viewBox="0 0 260 110" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">${SCENES[i]}</svg>`;
// Utama pages get the barn, Operasional the windmill, Bisnis and Sistem the hay cart.
function sceneIndex() {
  const p = S.route.page === 'detail' ? 'ternak' : S.route.page;
  const g = MENU.findIndex(([, items]) => items.some(([id]) => id === p));
  return Math.min(Math.max(g, 0), 2);
}

// Static background behind every page: soft sun, field contours, rolling hills with a small farm scene.
const deco = (i) => `<div class="deco" aria-hidden="true">
  <div class="sun"></div>
  <svg class="contours" viewBox="0 0 420 260" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round">
    <path d="M-10 40C80 10 170 70 260 40s120-30 170-10"/><path d="M-10 80c90-30 180 30 270 0s120-30 170-10"/>
    <path d="M-10 120c90-30 180 30 270 0s120-30 170-10"/><path d="M-10 160c90-30 180 30 270 0s120-30 170-10"/>
  </svg>
  <svg class="hills" viewBox="0 0 1440 200" preserveAspectRatio="none">
    <path class="h2" d="M0 120C180 70 360 60 560 95s380 55 560 15 250-40 320-30V200H0Z"/>
    <path class="h1" d="M0 160c200-40 420-50 640-20s420 40 600 5 160-25 200-20V200H0Z"/>
  </svg>
  ${sceneSvg(i, 'farm')}
  <svg class="sprig" viewBox="0 0 60 60" fill="none" stroke="currentColor" stroke-width="1.75" stroke-linecap="round" stroke-linejoin="round">
    <path d="M30 56V20"/><path d="M30 34c-8-2-13-8-13-16 8 1 13 7 13 16Z"/><path d="M30 26c7-2 11-7 11-14-7 1-11 6-11 14Z"/>
  </svg>
</div>`;

/* ---------------- Formatting ---------------- */
const esc = (s) => String(s ?? '').replace(/[&<>"']/g, (c) => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c]));
const num = (n, d = 0) => Number(n).toLocaleString('id-ID', { minimumFractionDigits: d, maximumFractionDigits: d });
const rp = (n) => `Rp ${num(Math.round(n))}`;
const rpShort = (n) => {
  const a = Math.abs(n);
  const s = a >= 1e9 ? `${num(a / 1e9, 1)} M` : a >= 1e6 ? `${num(a / 1e6, 1)} jt` : a >= 1e3 ? `${num(a / 1e3, 0)} rb` : num(a);
  return `${n < 0 ? '−' : ''}Rp ${s}`;
};
const fmtDate = (s) => { const d = parseIso(s); return `${d.getDate()} ${BULAN[d.getMonth()]} ${d.getFullYear()}`; };
const fmtShort = (s) => { const d = parseIso(s); return `${d.getDate()} ${BULAN[d.getMonth()]}`; };
const fmtLong = (d) => `${HARI[d.getDay()]}, ${d.getDate()} ${BULAN_PANJANG[d.getMonth()]} ${d.getFullYear()}`;
function relDay(s) {
  const n = daysBetween(TODAY, parseIso(s));
  if (n === 0) return 'hari ini';
  if (n === 1) return 'besok';
  if (n === -1) return 'kemarin';
  return n > 0 ? `${n} hari lagi` : `${-n} hari lalu`;
}
function umur(lahir) {
  const m = Math.floor(daysBetween(parseIso(lahir), TODAY) / 30.44);
  const y = Math.floor(m / 12);
  return y > 0 ? `${y} th ${m % 12} bl` : `${m} bl`;
}
const plural = (n, w) => `${num(n)} ${w}`;

/* ---------------- State ---------------- */
const S = {
  screen: 'auth',
  mode: null,
  user: null,
  db: null,
  ix: null,
  route: { page: 'dashboard', id: null },
  settings: { theme: 'light', density: 'comfortable' },
  auth: { showPw: false, error: '', remember: true, username: '' },
  ui: null,
  dialog: null,
  palette: null,
  notif: false,
  snack: null,
  fresh: null,
  maxed: true,
  anim: new Set()
};

function freshUi() {
  return {
    popSp: 'all',
    ternak: { q: '', sp: 'all', kandang: 'all', status: 'Aktif', page: 1 },
    detailTab: 'pertumbuhan',
    kandangSel: null,
    pakanForm: { kandangId: '', pakanId: '', jumlah: '' },
    pakanErr: '',
    calMonth: iso(TODAY).slice(0, 7),
    calDay: iso(TODAY),
    repro: 'aktif',
    mutasi: 'Semua',
    keuRange: '6',
    keuJenis: 'Semua',
    laporan: { jenis: 'ternak', dari: iso(addDays(TODAY, -90)), sampai: iso(TODAY) },
    masterTab: 'spesies'
  };
}

function loadDb(kind) {
  let db = store.get(keyDb(kind));
  if (!db || !db.ternak) { db = makeDb(kind); store.set(keyDb(kind), db); }
  return db;
}
function saveDb() { store.set(keyDb(S.db.kind), S.db); }

function reindex() {
  const db = S.db;
  const map = (arr) => new Map(arr.map((x) => [x.id, x]));
  const weights = new Map();
  for (const w of db.timbang) {
    if (!weights.has(w.ternakId)) weights.set(w.ternakId, []);
    weights.get(w.ternakId).push(w);
  }
  for (const arr of weights.values()) arr.sort((a, b) => (a.tanggal < b.tanggal ? -1 : a.tanggal > b.tanggal ? 1 : a.id - b.id));
  S.ix = { ternak: map(db.ternak), kandang: map(db.kandang), ras: map(db.ras), spesies: map(db.spesies), obat: map(db.obat), pakan: map(db.pakan), weights };
}

const T = (id) => S.ix.ternak.get(Number(id));
const K = (id) => S.ix.kandang.get(Number(id));
const spOf = (t) => S.ix.spesies.get(t.spesiesId);
const rasOf = (t) => S.ix.ras.get(t.rasId);
const aktif = () => S.db.ternak.filter((t) => t.status === 'Aktif');
const isiKandang = (kId) => S.db.ternak.filter((t) => t.status === 'Aktif' && t.kandangId === Number(kId)).length;
const kLabel = (kId) => { const k = K(kId); return k ? `${k.kode} · ${k.nama}` : '—'; };
const tLabel = (t) => `${t.kode}${t.nama ? ` · ${t.nama}` : ''}`;
const nextId = (arr) => arr.reduce((m, x) => Math.max(m, x.id), 0) + 1;

function weightsOf(id) { return S.ix.weights.get(Number(id)) || []; }
function lastWeight(id) { const w = weightsOf(id); return w.length ? w[w.length - 1] : null; }
function adg(id, span = 30) {
  const w = weightsOf(id);
  if (w.length < 2) return null;
  const last = w[w.length - 1];
  const limit = addDays(parseIso(last.tanggal), -span);
  let prev = w[0];
  for (const x of w) if (parseIso(x.tanggal) <= limit) prev = x;
  if (prev === last) prev = w[w.length - 2];
  const d = daysBetween(parseIso(prev.tanggal), parseIso(last.tanggal));
  return d > 0 ? (last.berat - prev.berat) / d : null;
}

function nextKode(spId) {
  const sp = S.ix.spesies.get(Number(spId));
  const max = S.db.ternak.filter((t) => t.spesiesId === sp.id).reduce((m, t) => Math.max(m, Number(t.kode.split('-')[1]) || 0), 0);
  return `${sp.prefix}-${String(max + 1).padStart(sp.pad, '0')}`;
}

/* ---------------- Roles ---------------- */
const MENU = [
  ['Utama', [['dashboard', 'Dashboard', 'dashboard'], ['ternak', 'Ternak', 'tag'], ['kandang', 'Kandang', 'home']]],
  ['Operasional', [['pakan', 'Pakan', 'box'], ['kesehatan', 'Kesehatan', 'heart'], ['reproduksi', 'Reproduksi', 'repro'], ['mutasi', 'Mutasi', 'swap']]],
  ['Bisnis', [['keuangan', 'Keuangan', 'wallet'], ['laporan', 'Laporan', 'file']]],
  ['Sistem', [['master', 'Data master', 'db'], ['pengguna', 'Pengguna', 'users'], ['pengaturan', 'Pengaturan', 'settings']]]
];
const ACCESS = {
  Admin: null,
  Tamu: ['dashboard', 'ternak', 'kandang', 'pakan', 'kesehatan', 'reproduksi', 'mutasi', 'keuangan', 'laporan', 'master', 'pengaturan'],
  Petugas: ['dashboard', 'ternak', 'kandang', 'pakan', 'pengaturan'],
  Mantri: ['dashboard', 'ternak', 'kandang', 'kesehatan', 'reproduksi', 'pengaturan']
};
const PERAN_LABEL = { Admin: 'Pemilik peternakan', Petugas: 'Petugas kandang', Mantri: 'Mantri hewan', Tamu: 'Database tamu' };
const canPage = (p) => { const a = ACCESS[S.user.peran]; return !a || a.includes(p); };
const CAN = {
  tambahTernak: ['Admin', 'Tamu'],
  timbang: ['Admin', 'Tamu', 'Petugas'],
  mutasi: ['Admin', 'Tamu'],
  pakan: ['Admin', 'Tamu', 'Petugas'],
  kesehatan: ['Admin', 'Tamu', 'Mantri'],
  master: ['Admin', 'Tamu']
};
const can = (a) => CAN[a].includes(S.user.peran);

/* ---------------- Reminders ---------------- */
function reminders() {
  const db = S.db;
  const out = [];
  const today = iso(TODAY);
  const groups = new Map();
  for (const j of db.jadwal) {
    const d = daysBetween(TODAY, parseIso(j.rencana));
    const open = j.status === 'Terjadwal' && d >= 0 && d <= 3;
    const doneToday = j.status === 'Selesai' && j.realisasi === today && d >= 0 && d <= 3 && j.doneVia === 'pengingat';
    const missed = j.status === 'Terlewat';
    if (!open && !doneToday && !missed) continue;
    const t = T(j.ternakId);
    if (!t || t.status !== 'Aktif') continue;
    const key = `${missed ? 'lewat' : 'vaksin'}-${j.obatId}-${t.kandangId}-${j.rencana}`;
    if (!groups.has(key)) groups.set(key, { j, kandangId: t.kandangId, ids: [], done: true, missed });
    const g = groups.get(key);
    g.ids.push(j.id);
    if (!doneToday) g.done = false;
  }
  for (const g of groups.values()) {
    const ob = S.ix.obat.get(g.j.obatId);
    const nama = ob.nama.replace('Vaksin ', '');
    const k = K(g.kandangId);
    if (g.missed) {
      out.push({ key: `l${g.ids[0]}`, tone: 'danger', icon: 'alert', title: `Vaksin ${nama} terlewat`, sub: `Kandang ${k.kode} · ${g.ids.length} ekor · ${relDay(g.j.rencana)}`, btn: ['Jadwalkan', 'go', 'kesehatan'], order: 0 });
    } else {
      out.push({ key: `v${g.ids[0]}`, tone: 'danger', icon: 'syringe', title: `Vaksinasi ${nama}`, sub: `Kandang ${k.kode} · ${g.ids.length} ekor · ${relDay(g.j.rencana)}`, btn: g.done ? null : ['Selesai', 'vaksinSelesai', g.ids.join(',')], done: g.done, order: 1 });
    }
  }
  for (const kw of db.kawin.filter((x) => x.status === 'Bunting')) {
    const d = daysBetween(TODAY, parseIso(kw.perkiraanLahir));
    if (d < -3 || d > 7) continue;
    const t = T(kw.indukId);
    out.push({ key: `k${kw.id}`, tone: 'ok', icon: 'calendar', title: `Perkiraan lahir ${t.kode}`, sub: `${spOf(t).nama} ${rasOf(t).nama} · ${fmtDate(kw.perkiraanLahir)}`, btn: ['Lihat', 'openTernak', t.id], order: 2 });
  }
  for (const p of db.pakan.filter((x) => x.stok < x.min)) {
    out.push({ key: `p${p.id}`, tone: 'warn', icon: 'alert', title: `${p.nama} tinggal ${num(p.stok)} ${p.satuan}`, sub: `Stok minimum ${num(p.min)} ${p.satuan}`, btn: canPage('pakan') && can('pakan') ? ['Pesan', 'openPakanMasuk', p.id] : null, order: 3 });
  }
  return out.sort((a, b) => (a.done ? 1 : 0) - (b.done ? 1 : 0) || a.order - b.order);
}

/* ---------------- Charts ---------------- */
function niceMax(v) {
  if (v <= 0) return 1;
  const m = Math.pow(10, Math.floor(Math.log10(v)));
  for (const f of [1, 1.5, 2, 2.5, 3, 4, 5, 6, 8, 10]) if (f * m >= v) return f * m;
  return 10 * m;
}

function barChart(vals, labels, aria, { h = 210, w = 720, fmt = (v) => num(v) } = {}) {
  const max = niceMax(Math.max(...vals));
  const padL = 40, padT = 24, padB = 24, gap = 10;
  const pw = w - padL, ph = h - padT - padB;
  const bw = (pw - gap * (vals.length - 1)) / vals.length;
  let g = '';
  [0, 0.5, 1].forEach((f) => {
    const y = padT + ph - ph * f;
    g += `<line class="${f === 0 ? 'axis' : 'grid'}" x1="${padL}" x2="${w}" y1="${y}" y2="${y}"/><text x="${padL - 8}" y="${y + 4}" text-anchor="end">${fmt(max * f)}</text>`;
  });
  vals.forEach((v, i) => {
    const bh = Math.max(2, ph * v / max);
    const x = padL + i * (bw + gap);
    const last = i === vals.length - 1;
    g += `<rect class="bar${last ? ' hi' : ''}" x="${x}" y="${padT + ph - bh}" width="${bw}" height="${bh}" rx="4"><title>${esc(labels[i])}: ${fmt(v)}</title></rect>`;
    if (last) g += `<text class="lbl-strong" x="${x + bw / 2}" y="${padT + ph - bh - 8}" text-anchor="middle">${fmt(v)}</text>`;
    g += `<text x="${x + bw / 2}" y="${h - 6}" text-anchor="middle" ${last ? 'class="lbl-strong"' : ''}>${esc(labels[i])}</text>`;
  });
  return `<svg class="chart" viewBox="0 0 ${w} ${h}" role="img" aria-label="${esc(aria)}">${g}</svg>`;
}

function groupedBars(rows, aria, { h = 230, w = 720 } = {}) {
  const max = niceMax(Math.max(...rows.flatMap((r) => [r.masuk, r.keluar])));
  const padL = 56, padT = 16, padB = 24, gap = 22;
  const pw = w - padL, ph = h - padT - padB;
  const gw = (pw - gap * (rows.length - 1)) / rows.length;
  const bw = (gw - 6) / 2;
  let g = '';
  [0, 0.5, 1].forEach((f) => {
    const y = padT + ph - ph * f;
    g += `<line class="${f === 0 ? 'axis' : 'grid'}" x1="${padL}" x2="${w}" y1="${y}" y2="${y}"/><text x="${padL - 8}" y="${y + 4}" text-anchor="end">${rpShort(max * f).replace('Rp ', '')}</text>`;
  });
  rows.forEach((r, i) => {
    const x = padL + i * (gw + gap);
    const hi = ph * r.masuk / max, ho = ph * r.keluar / max;
    g += `<rect class="bar-in" x="${x}" y="${padT + ph - hi}" width="${bw}" height="${Math.max(hi, 1)}" rx="3"><title>${r.label} pemasukan ${rp(r.masuk)}</title></rect>`;
    g += `<rect class="bar-out" x="${x + bw + 6}" y="${padT + ph - ho}" width="${bw}" height="${Math.max(ho, 1)}" rx="3"><title>${r.label} pengeluaran ${rp(r.keluar)}</title></rect>`;
    g += `<text x="${x + gw / 2}" y="${h - 6}" text-anchor="middle">${r.label}</text>`;
  });
  return `<svg class="chart" viewBox="0 0 ${w} ${h}" role="img" aria-label="${esc(aria)}">${g}</svg>`;
}

function lineChart(pts, aria, { h = 240, w = 720, target = null, unit = 'kg' } = {}) {
  const ys = pts.map((p) => p.y).concat(target ? [target] : []);
  let lo = Math.min(...ys), hi = Math.max(...ys);
  const padV = (hi - lo) * 0.15 || 5;
  lo = Math.max(0, Math.floor((lo - padV) / 5) * 5); hi = Math.ceil((hi + padV) / 5) * 5;
  const padL = 44, padT = 16, padB = 26, padR = 12;
  const pw = w - padL - padR, ph = h - padT - padB;
  const X = (i) => padL + (pts.length === 1 ? pw / 2 : pw * i / (pts.length - 1));
  const Y = (v) => padT + ph - ph * (v - lo) / (hi - lo || 1);
  let g = '';
  [0, 0.5, 1].forEach((f) => {
    const v = lo + (hi - lo) * f;
    g += `<line class="${f === 0 ? 'axis' : 'grid'}" x1="${padL}" x2="${w - padR}" y1="${Y(v)}" y2="${Y(v)}"/><text x="${padL - 8}" y="${Y(v) + 4}" text-anchor="end">${num(v)}</text>`;
  });
  if (target) g += `<line class="target" x1="${padL}" x2="${w - padR}" y1="${Y(target)}" y2="${Y(target)}"/><text x="${w - padR}" y="${Y(target) - 6}" text-anchor="end">Target ${num(target)} ${unit}</text>`;
  const d = pts.map((p, i) => `${i ? 'L' : 'M'}${X(i).toFixed(1)} ${Y(p.y).toFixed(1)}`).join(' ');
  g += `<path class="area" d="${d} L${X(pts.length - 1)} ${padT + ph} L${X(0)} ${padT + ph} Z"/><path class="line" d="${d}"/>`;
  const every = Math.ceil(pts.length / 8);
  pts.forEach((p, i) => {
    g += `<circle class="pt" cx="${X(i)}" cy="${Y(p.y)}" r="4"><title>${esc(p.label)}: ${num(p.y, 1)} ${unit}</title></circle>`;
    if (i % every === 0 || i === pts.length - 1) g += `<text x="${X(i)}" y="${h - 6}" text-anchor="middle">${esc(p.label)}</text>`;
  });
  const last = pts[pts.length - 1];
  g += `<text class="lbl-strong" x="${X(pts.length - 1) - 6}" y="${Y(last.y) - 10}" text-anchor="end">${num(last.y, 1)} ${unit}</text>`;
  return `<svg class="chart" viewBox="0 0 ${w} ${h}" role="img" aria-label="${esc(aria)}">${g}</svg>`;
}

/* ---------------- Small view helpers ---------------- */
const enter = (k) => (S.anim.has(k) ? ' enter' : '');
const kondisiBadge = (t) => {
  if (t.status !== 'Aktif') return `<span class="badge neutral">${t.status}</span>`;
  return { Sehat: '<span class="badge ok">Sehat</span>', Diobati: '<span class="badge danger">Diobati</span>', Periksa: '<span class="badge warn">Perlu diperiksa</span>' }[t.kondisi];
};
const pctClass = (p) => (p >= 100 ? 'danger' : p >= 80 ? 'warn' : 'ok');
const meter = (pct, cls = '') => `<div class="meter"><span class="${cls}" style="width:${Math.min(100, Math.max(0, pct))}%"></span></div>`;
const options = (list, sel) => list.map(([v, l, dis]) => `<option value="${esc(v)}"${String(v) === String(sel) ? ' selected' : ''}${dis ? ' disabled' : ''}>${esc(l)}</option>`).join('');
const segmented = (label, items, val, act) => `<div class="segmented" role="group" aria-label="${esc(label)}">${items.map(([v, l]) => `<button type="button" aria-pressed="${String(v) === String(val)}" data-act="${act}" data-arg="${esc(v)}">${esc(l)}</button>`).join('')}</div>`;
function pageHead(title, sub, actions = '') {
  return `<header class="page-head"><div><h1>${title}</h1>${sub ? `<p>${sub}</p>` : ''}</div><div class="actions">${actions}</div></header>`;
}
function stat(label, value, unit, sub, tone, ic) {
  return `<div class="card stat lift"><div class="top"><span>${label}</span><span class="ico ${tone}">${icon(ic, 16, 2)}</span></div><div class="val"><b>${value}</b>${unit ? `<span>${unit}</span>` : ''}</div><div class="sub">${sub}</div></div>`;
}
const searchButton = () => `<button type="button" class="search-box" data-act="palette" aria-label="Cari (Ctrl+K)">${icon('search', 16, 2)}<span style="flex:1;text-align:left">Cari ternak, kandang, menu…</span><kbd>Ctrl K</kbd></button>`;

/* ---------------- Screens ---------------- */
function viewTitlebar() {
  let chip = '';
  if (S.screen === 'app') {
    chip = S.mode === 'tamu'
      ? `<span class="db-chip guest" title="Data tamu disimpan terpisah dari data akun">${icon('guest', 12, 2)}Mode tamu · ${S.db.file}</span>`
      : `<span class="db-chip" title="Database akun">${icon('db', 12, 2)}${S.db.file}</span>`;
  }
  return `<div class="titlebar">
    <div class="app-id"><span class="logo-sm">${icon('leaf', 12, 2.25)}</span>TernakKu</div>${chip}
    <div class="spacer"></div>
    <button type="button" class="win-btn" aria-label="Minimalkan" data-act="noop"><svg width="10" height="10" viewBox="0 0 10 10" aria-hidden="true"><path d="M0 5h10" stroke="currentColor"/></svg></button>
    <button type="button" class="win-btn" aria-label="${S.maxed ? 'Pulihkan ukuran' : 'Maksimalkan'}" data-act="maxToggle">${S.maxed
      ? '<svg width="10" height="10" viewBox="0 0 10 10" fill="none" aria-hidden="true"><rect x="0.5" y="2.5" width="7" height="7" stroke="currentColor"/><path d="M2.5 2.5v-2h7v7h-2" stroke="currentColor"/></svg>'
      : '<svg width="10" height="10" viewBox="0 0 10 10" fill="none" aria-hidden="true"><rect x="0.5" y="0.5" width="9" height="9" stroke="currentColor"/></svg>'}</button>
    <button type="button" class="win-btn close" aria-label="Tutup aplikasi" data-act="winClose"><svg width="10" height="10" viewBox="0 0 10 10" aria-hidden="true"><path d="M0 0l10 10M10 0 0 10" stroke="currentColor"/></svg></button>
  </div>`;
}

function viewAuth() {
  const a = S.auth;
  return `<main class="auth"><div class="auth-card${enter('auth')}">
    <div class="auth-hero">${SCENE}<h1>TernakKu</h1></div>
    ${a.error ? `<div class="auth-error" role="alert">${icon('alert', 16, 2)}${esc(a.error)}</div>` : ''}
    <form class="auth-form" data-form="login" novalidate>
      <div class="input-icon">${icon('user', 18)}<label class="sr" for="l-user">Username</label>
        <input class="input" id="l-user" name="username" autocomplete="username" placeholder="Username" value="${esc(a.username)}" style="height:44px"></div>
      <div class="input-icon">${icon('lock', 18)}<label class="sr" for="l-pass">Password</label>
        <input class="input" id="l-pass" name="password" type="${a.showPw ? 'text' : 'password'}" autocomplete="current-password" placeholder="Password" style="height:44px;padding-right:44px">
        <button type="button" class="trail" style="top:6px" data-act="togglePw" aria-label="${a.showPw ? 'Sembunyikan password' : 'Tampilkan password'}">${icon(a.showPw ? 'eyeOff' : 'eye', 18)}</button></div>
      <div class="auth-row"><label class="check"><input type="checkbox" id="l-remember" data-act="toggleRemember" ${a.remember ? 'checked' : ''}>Ingat saya</label></div>
      <button type="submit" class="btn btn-primary btn-block" style="height:44px">Masuk ${icon('arrowR', 16, 2.25)}</button>
    </form>
    <div class="divider">atau</div>
    <button type="button" class="btn btn-secondary btn-block" style="height:44px" data-act="guest" title="Memakai database tamu yang terpisah dari data akun">${icon('guest', 18)}Masuk sebagai tamu</button>
    <div class="auth-foot">${icon('lock', 14, 2)}Offline · v1.0</div>
  </div></main>`;
}

function viewClosed() {
  const ses = store.get(KEY_SESSION);
  return `<main class="closed"><div>
    <span class="brand"><span class="logo" style="width:56px;height:56px">${icon('leaf', 28, 2)}</span></span>
    <h1 style="font-size:20px;color:var(--text)">TernakKu ditutup</h1>
    <p>${ses ? `Pilihan masuk diingat: langsung terbuka sebagai <b>${esc(ses.mode === 'tamu' ? 'Tamu' : ses.nama)}</b>.` : 'Saat dibuka, aplikasi akan meminta login atau mode tamu.'}</p>
    <button type="button" class="btn btn-primary" data-act="reopen">Buka TernakKu</button>
  </div></main>`;
}

const NARROW = 860;
const navCollapsed = () => S.settings.navCollapsed || window.innerWidth < NARROW;

const SIDEBAR_GROUND = '<svg viewBox="0 0 260 14" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" aria-hidden="true"><path d="M0 4c60-6 140-6 260 0"/><path d="M0 12c50-4 120-4 180-1s60 2 80-1"/></svg>';

function viewSidebar() {
  const sick = S.db.ternak.filter((t) => t.status === 'Aktif' && t.kondisi !== 'Sehat').length;
  const soon = S.db.kawin.filter((k) => k.status === 'Bunting' && daysBetween(TODAY, parseIso(k.perkiraanLahir)) <= 7).length;
  const status = {
    pakan: [S.db.pakan.filter((p) => p.stok < p.min).length, 'warn', 'stok menipis'],
    kesehatan: [sick, 'danger', 'perlu perhatian'],
    reproduksi: [soon, 'warn', 'segera lahir']
  };
  const collapsed = navCollapsed();
  let nav = '';
  for (const [grp, items] of MENU) {
    const vis = items.filter(([p]) => canPage(p));
    if (!vis.length) continue;
    nav += `<span class="nav-group"><span>${grp}</span></span>`;
    for (const [p, label, ic] of vis) {
      const cur = S.route.page === p || (p === 'ternak' && S.route.page === 'detail');
      const [n, tone, what] = status[p] || [0];
      const tip = n ? `${label} · ${n} ${what}` : label;
      nav += `<button type="button" class="nav-item" data-act="go" data-arg="${p}" data-tip="${esc(tip)}" ${cur ? 'aria-current="page"' : ''} aria-label="${esc(tip)}">
        <span class="ic">${icon(ic)}${n ? `<span class="ndot ${tone}"></span>` : ''}</span><span class="lbl">${label}</span>
        ${n ? `<span class="count ${tone}">${n}</span>` : cur ? `<span class="leafmark">${icon('leaf', 14, 2)}</span>` : ''}</button>`;
    }
  }
  const ini = S.user.nama.replace('drh. ', '').split(' ').map((w) => w[0]).join('').slice(0, 2).toUpperCase();
  const guest = S.mode === 'tamu';
  return `<nav class="sidebar${collapsed ? ' collapsed' : ''}" aria-label="Menu utama">
    ${window.innerWidth < NARROW ? '' : `<button type="button" class="collapse-btn" data-act="navToggle" aria-label="${collapsed ? 'Lebarkan menu' : 'Ciutkan menu'}" aria-expanded="${!collapsed}">${icon(collapsed ? 'chevR' : 'chevL', 14, 2.25)}</button>`}
    <div class="farm-card" data-tip="${esc(S.db.peternakan)} · ${S.db.file}">
      <span class="logo">${icon('leaf', 22, 2)}</span>
      <div class="meta"><b>${esc(S.db.peternakan)}</b><span class="db${guest ? ' guest' : ''}"><i></i>${guest ? 'Mode tamu · ' : ''}${S.db.file}</span></div>
    </div>
    <div class="nav-scroll" id="nav-scroll"><span class="nav-ind" aria-hidden="true"></span>${nav}</div>
    <div class="sidebar-art">${sceneSvg((sceneIndex() + 1) % 3, 'art')}${SIDEBAR_GROUND}</div>
    <div class="sidebar-foot"><span class="avatar" data-tip="${esc(S.user.nama)} · ${PERAN_LABEL[S.user.peran]}">${guest ? icon('guest', 18) : esc(ini)}</span>
      <div class="who"><b>${esc(S.user.nama)}</b><small>${PERAN_LABEL[S.user.peran]}</small></div>
      <button type="button" class="icon-btn" data-act="logout" aria-label="Keluar" data-tip="Keluar">${icon('logout')}</button></div>
  </nav>`;
}

// Slide the active pill from where it was to the current item.
function placeNavIndicator() {
  const ind = document.querySelector('.nav-ind');
  if (!ind) return;
  const cur = document.querySelector('.nav-item[aria-current="page"]');
  if (!cur) { ind.style.opacity = '0'; S.navY = null; return; }
  const y = cur.offsetTop;
  ind.style.opacity = '1';
  ind.style.height = `${cur.offsetHeight}px`;
  if (S.navY != null && S.navY !== y) {
    ind.style.transition = 'none';
    ind.style.transform = `translateY(${S.navY}px)`;
    void ind.offsetHeight;
    ind.style.transition = '';
  }
  ind.style.transform = `translateY(${y}px)`;
  S.navY = y;
}

const navTip = document.createElement('div');
navTip.id = 'navtip';
navTip.setAttribute('role', 'tooltip');
document.body.appendChild(navTip);
document.addEventListener('mouseover', (e) => {
  const el = e.target.closest('.sidebar.collapsed [data-tip]');
  if (!el) { navTip.classList.remove('show'); return; }
  const r = el.getBoundingClientRect();
  navTip.textContent = el.dataset.tip;
  navTip.style.left = `${r.right + 12}px`;
  navTip.style.top = `${r.top + r.height / 2}px`;
  navTip.classList.add('show');
});
let wasNarrow = window.innerWidth < NARROW;
window.addEventListener('resize', () => {
  const narrow = window.innerWidth < NARROW;
  if (narrow !== wasNarrow && S.screen === 'app') { wasNarrow = narrow; render(); }
});

function viewApp() {
  const pages = { dashboard: pageDashboard, ternak: pageTernak, detail: pageDetail, kandang: pageKandang, pakan: pagePakan, kesehatan: pageKesehatan, reproduksi: pageReproduksi, mutasi: pageMutasi, keuangan: pageKeuangan, laporan: pageLaporan, master: pageMaster, pengguna: pagePengguna, pengaturan: pagePengaturan };
  return `${viewSidebar()}<div class="main-area">${deco(sceneIndex())}<div class="content" id="content"><div class="page${enter('page')}">${pages[S.route.page]()}</div></div></div>`;
}

/* ---------------- Pages ---------------- */
function greeting() {
  const h = new Date().getHours();
  return h < 4 || h >= 18 ? 'Selamat malam' : h < 11 ? 'Selamat pagi' : h < 15 ? 'Selamat siang' : 'Selamat sore';
}
const nowJam = () => { const n = new Date(); return `${String(n.getHours()).padStart(2, '0')}.${String(n.getMinutes()).padStart(2, '0')}`; };

// Today's work, in time order. Feed and vaccine items read real data; the rest are routine tasks ticked by hand.
function todayTasks() {
  const db = S.db;
  const today = iso(TODAY);
  if (!db.tugas || db.tugas.tanggal !== today) db.tugas = { tanggal: today, done: [] };
  const manual = (key) => db.tugas.done.includes(key);
  const tasks = [];
  if (canPage('pakan')) {
    const logs = db.pakanLog.filter((l) => l.tanggal === today).sort((a, b) => (a.jam < b.jam ? -1 : 1));
    const pagi = logs.filter((l) => l.jam < '12.00');
    tasks.push(pagi.length
      ? { key: 'pakan-pagi', jam: pagi[0].jam, icon: 'box', title: 'Pakan pagi', sub: `${new Set(pagi.map((l) => l.kandangId)).size} kandang · ${num(pagi.reduce((a, l) => a + l.jumlah, 0))} kg`, done: true, btn: ['Lihat', 'go', 'pakan'] }
      : { key: 'pakan-pagi', jam: '06.30', icon: 'box', title: 'Pakan pagi', sub: 'Semua kandang', done: false, btn: ['Catat', 'go', 'pakan'] });
    tasks.push({ key: 'pakan-sore', jam: '16.00', icon: 'box', title: 'Pakan sore', sub: `${S.db.kandang.length} kandang`, done: manual('pakan-sore'), toggle: true, btn: ['Catat', 'go', 'pakan'] });
  }
  if (canPage('kesehatan')) {
    const groups = new Map();
    for (const j of db.jadwal.filter((x) => x.rencana === today && x.status !== 'Terlewat')) {
      const t = T(j.ternakId);
      if (!t || t.status !== 'Aktif') continue;
      const key = `${j.obatId}-${t.kandangId}`;
      if (!groups.has(key)) groups.set(key, { j, k: t.kandangId, ids: [], done: true });
      const g = groups.get(key);
      g.ids.push(j.id);
      if (j.status !== 'Selesai') g.done = false;
    }
    [...groups.values()].forEach((g, i) => {
      tasks.push({ key: `vak-${g.ids[0]}`, jam: `0${9 + i}.00`.slice(-5), icon: 'syringe', title: `Vaksin ${S.ix.obat.get(g.j.obatId).nama.replace('Vaksin ', '')}`, sub: `Kandang ${K(g.k).kode} · ${g.ids.length} ekor`, done: g.done, doneAct: g.done ? null : ['vaksinSelesai', g.ids.join(',')], btn: ['Jadwal', 'go', 'kesehatan'] });
    });
    const periksa = aktif().filter((t) => t.kondisi === 'Periksa');
    if (periksa.length) tasks.push({ key: 'periksa', jam: '10.30', icon: 'heart', title: `Periksa ${periksa.length} ternak`, sub: `Kandang ${[...new Set(periksa.map((t) => K(t.kandangId).kode))].join(', ')}`, done: manual('periksa'), toggle: true, btn: ['Buka', 'go', 'kesehatan'] });
  }
  if (can('timbang')) {
    const k = S.db.kandang[0];
    tasks.push({ key: 'timbang', jam: '14.00', icon: 'scale', title: `Timbang kandang ${k.kode}`, sub: `${isiKandang(k.id)} ekor · rutin 14 hari`, done: manual('timbang'), toggle: true, btn: ['Daftar', 'lihatKandang', k.id] });
  }
  tasks.push({ key: 'cek-sore', jam: '17.30', icon: 'home', title: 'Cek kandang & gudang', sub: 'Air minum, pintu, lampu', done: manual('cek-sore'), toggle: true });
  return tasks.sort((a, b) => (a.jam < b.jam ? -1 : 1));
}

function donut(parts, size = 132, w = 16) {
  const r = (size - w) / 2, c = 2 * Math.PI * r;
  const total = parts.reduce((a, p) => a + p.v, 0) || 1;
  let off = 0;
  const segs = parts.map((p) => {
    const len = c * p.v / total;
    const s = `<circle cx="${size / 2}" cy="${size / 2}" r="${r}" fill="none" stroke="${p.color}" stroke-width="${w}" stroke-dasharray="${Math.max(len - 3, 0)} ${c}" stroke-dashoffset="${-off}" transform="rotate(-90 ${size / 2} ${size / 2})"><title>${esc(p.label)}: ${p.v}</title></circle>`;
    off += len;
    return s;
  }).join('');
  return `<svg class="donut" width="${size}" height="${size}" viewBox="0 0 ${size} ${size}" role="img" aria-label="${parts.map((p) => `${p.label} ${p.v}`).join(', ')}"><circle cx="${size / 2}" cy="${size / 2}" r="${r}" fill="none" stroke="var(--divider)" stroke-width="${w}"/>${segs}</svg>`;
}

function sparkline(vals, w = 220, h = 56) {
  const lo = Math.min(...vals), hi = Math.max(...vals);
  const X = (i) => (w - 8) * i / (vals.length - 1) + 4;
  const Y = (v) => h - 6 - (h - 12) * (v - lo) / (hi - lo || 1);
  const d = vals.map((v, i) => `${i ? 'L' : 'M'}${X(i).toFixed(1)} ${Y(v).toFixed(1)}`).join(' ');
  return `<svg class="spark" viewBox="0 0 ${w} ${h}" aria-hidden="true"><path class="area" d="${d} L${X(vals.length - 1)} ${h} L${X(0)} ${h} Z"/><path class="line" d="${d}"/><circle class="pt" cx="${X(vals.length - 1)}" cy="${Y(vals[vals.length - 1])}" r="3.5"/></svg>`;
}

function tile(cls, ic, title, page, body, extra = '') {
  const link = page && canPage(page) ? `<button type="button" class="tile-link" data-act="go" data-arg="${page}">Buka ${icon('arrowR', 14, 2)}</button>` : '';
  return `<section class="tile ${cls}" aria-label="${esc(title)}"><header class="tile-head"><span class="tile-ico">${icon(ic, 16, 2)}</span><h2>${title}</h2>${extra}${link}</header>${body}</section>`;
}

function pageDashboard() {
  const db = S.db;
  const act = aktif();
  const per = [1, 2, 3].map((s) => act.filter((t) => t.spesiesId === s).length);
  const baru = act.filter((t) => t.tanggalMasuk.slice(0, 7) === iso(TODAY).slice(0, 7)).length;
  const rem = reminders();
  const open = rem.filter((r) => !r.done).length;
  const tipis = db.pakan.filter((p) => p.stok < p.min);

  // Greeting band + timeline.
  const tasks = todayTasks();
  const now = nowJam();
  const doneN = tasks.filter((t) => t.done).length;
  let nowPlaced = false;
  let tl = '';
  for (const t of tasks) {
    if (!nowPlaced && t.jam > now) { tl += `<li class="tl-now" aria-label="Sekarang ${now}"><span>${now}</span></li>`; nowPlaced = true; }
    const dot = t.toggle
      ? `<button type="button" class="tl-dot" data-act="tugasToggle" data-arg="${t.key}" aria-pressed="${t.done}" aria-label="${t.done ? 'Batalkan tanda selesai' : 'Tandai selesai'}: ${esc(t.title)}">${t.done ? icon('check', 14, 3) : ''}</button>`
      : t.doneAct
        ? `<button type="button" class="tl-dot" data-act="${t.doneAct[0]}" data-arg="${t.doneAct[1]}" aria-pressed="false" aria-label="Tandai selesai: ${esc(t.title)}"></button>`
        : `<span class="tl-dot" aria-hidden="true">${t.done ? icon('check', 14, 3) : ''}</span>`;
    tl += `<li class="tl-item${t.done ? ' done' : ''}${!t.done && t.jam < now ? ' late' : ''}"><span class="tl-time num">${t.jam}</span>${dot}
      <div class="tl-card"><div class="tl-title">${icon(t.icon, 14, 2)}<b>${esc(t.title)}</b></div><small>${esc(t.sub)}</small>${t.btn && canPage(t.btn[1] === 'go' ? t.btn[2] : 'ternak') ? `<button type="button" class="tl-btn" data-act="${t.btn[1]}" data-arg="${t.btn[2]}">${t.btn[0]} ${icon('chevR', 12, 2.25)}</button>` : ''}</div></li>`;
  }
  if (!nowPlaced) tl += `<li class="tl-now" aria-label="Sekarang ${now}"><span>${now}</span></li>`;

  const hero = `<section class="hero">
    <div class="hero-top">
      <div class="hero-text"><span class="hero-date">${fmtLong(TODAY)}</span><h1>${greeting()}, ${esc(S.user.nama.split(' ')[0])}</h1>
        <p>${open ? `<b>${open} hal</b> perlu ditindak hari ini` : 'Tidak ada yang mendesak hari ini'} · ${doneN} dari ${tasks.length} tugas selesai</p></div>
      <div class="hero-actions">${searchButton()}<button type="button" class="icon-btn" data-act="notif" aria-label="Notifikasi">${icon('bell')}${open ? '<span class="dot"></span>' : ''}</button>${can('tambahTernak') ? `<button type="button" class="btn btn-primary" data-act="dlg" data-arg="tambahTernak">${icon('plus', 16, 2.25)}Tambah ternak</button>` : ''}</div>
    </div>
    <div class="tl-wrap"><div class="tl-progress" aria-hidden="true"><span style="width:${tasks.length ? doneN / tasks.length * 100 : 0}%"></span></div><ol class="tl" aria-label="Tugas hari ini">${tl}</ol></div>
  </section>`;

  // Population tile.
  const sp = S.ui.popSp;
  const vals = db.populasi.map((p) => (sp === 'all' ? p[1] + p[2] + p[3] : p[sp]));
  vals[vals.length - 1] = sp === 'all' ? act.length : per[Number(sp) - 1];
  const growth = vals[0] ? Math.round((vals[vals.length - 1] - vals[0]) / vals[0] * 100) : 0;
  const colors = ['var(--primary)', 'var(--warn-bar)', 'var(--info)'];
  const pop = tile('t-pop', 'tag', 'Populasi', 'ternak', `<div class="pop-body">
      <div class="pop-donut">${donut(db.spesies.slice(0, 3).map((s, i) => ({ label: s.nama, v: per[i], color: colors[i] })))}<div class="donut-center"><b class="num">${num(act.length)}</b><span>ekor aktif</span></div></div>
      <ul class="pop-legend">${db.spesies.slice(0, 3).map((s, i) => `<li><button type="button" data-act="popSp" data-arg="${s.id}" aria-pressed="${sp === String(s.id)}"><i style="background:${colors[i]}"></i><span>${s.nama}</span><b class="num">${per[i]}</b></button></li>`).join('')}
        <li><button type="button" data-act="popSp" data-arg="all" aria-pressed="${sp === 'all'}"><i style="background:var(--text-2)"></i><span>Semua</span><b class="num">+${baru}</b><small>bulan ini</small></button></li></ul>
      <div class="pop-chart"><div class="pop-chart-head"><span>${sp === 'all' ? 'Semua spesies' : S.ix.spesies.get(Number(sp)).nama} · 12 bulan</span><span class="badge ok plain">${growth >= 0 ? '+' : ''}${growth}%</span></div>${barChart(vals, db.populasi.map((p) => p.bulan), `Populasi ${vals[0]} menjadi ${vals[vals.length - 1]} ekor`, { h: 170, w: 520 })}</div>
    </div>`);

  // Reminders tile.
  const remTile = `<section class="tile t-rem" aria-label="Perlu perhatian"><header class="tile-head"><span class="tile-ico">${icon('bell', 16, 2)}</span><h2>Perlu perhatian</h2><span class="badge plain rem-count">${open}</span></header>
    <ul class="reminder-list">${rem.length ? rem.slice(0, 5).map((r) => `<li class="reminder${r.done ? ' done' : ''}"><span class="ico ${r.tone}">${icon(r.icon, 16, 2)}</span><div class="txt"><b>${esc(r.title)}</b><span>${esc(r.sub)}${r.done ? ' · selesai' : ''}</span></div>${r.btn ? `<button type="button" class="btn btn-secondary btn-sm" style="color:var(--primary)" data-act="${r.btn[1]}" data-arg="${r.btn[2]}">${r.btn[0]}</button>` : ''}</li>`).join('') : '<li class="empty"><strong>Semua beres</strong>Tidak ada pengingat.</li>'}</ul></section>`;

  // Pen site plan.
  const plots = db.kandang.map((k) => {
    const isi = isiKandang(k.id), pct = Math.round(isi / k.kapasitas * 100), c = pctClass(pct);
    const sick = act.filter((t) => t.kandangId === k.id && t.kondisi !== 'Sehat').length;
    return `<button type="button" class="plot ${c}" style="flex-grow:${k.kapasitas}" data-act="selPenGo" data-arg="${k.id}" title="Kandang ${k.kode} · ${esc(k.nama)}: ${isi} dari ${k.kapasitas} ekor"><span class="plot-fill" style="height:${Math.min(pct, 100)}%"></span><b>${k.kode}</b><small class="num">${isi}/${k.kapasitas}</small>${sick ? `<span class="plot-alert" aria-label="${sick} perlu perhatian">${sick}</span>` : ''}</button>`;
  }).join('');
  const pen = tile('t-pen', 'home', 'Denah kandang', 'kandang', `<div class="plan">${plots}<button type="button" class="plot store" data-act="go" data-arg="${canPage('pakan') ? 'pakan' : 'kandang'}" title="Gudang pakan"><b>${icon('box', 16, 2)}</b><small>Gudang</small></button></div>
    <div class="legend"><span><i style="background:var(--primary)"></i>&lt; 80%</span><span><i style="background:var(--warn-bar)"></i>80–99%</span><span><i style="background:var(--danger)"></i>penuh</span><span><i class="dot-alert"></i>perlu perhatian</span></div>`);

  // Feed silos.
  const silos = db.pakan.slice(0, 6).map((p) => {
    const pct = Math.min(100, p.stok / (p.min * 3) * 100);
    const low = p.stok < p.min;
    return `<button type="button" class="silo${low ? ' low' : ''}" data-act="go" data-arg="pakan" title="${esc(p.nama)}: ${num(p.stok)} ${p.satuan}, minimum ${num(p.min)}"><span class="tank"><span class="fill" style="height:${pct}%"></span><i class="minline" style="bottom:33.3%"></i></span><b>${esc(p.nama.split(' ')[0])}</b><small class="num">${num(p.stok)} ${p.satuan}</small></button>`;
  }).join('');
  const feed = canPage('pakan') ? tile('t-feed', 'box', 'Stok pakan', 'pakan', `<div class="silos">${silos}</div><p class="tile-foot">${tipis.length ? `<span class="badge warn">${tipis.length} di bawah minimum</span>` : '<span class="badge ok">Semua aman</span>'}<span class="muted">garis putus = stok minimum</span></p>`) : '';

  // Upcoming births.
  const births = db.kawin.filter((k) => k.status === 'Bunting').sort((a, b) => (a.perkiraanLahir < b.perkiraanLahir ? -1 : 1));
  const repro = tile('t-repro', 'repro', 'Segera lahir', canPage('reproduksi') ? 'reproduksi' : null, `<ul class="births">${births.slice(0, 4).map((k) => { const t = T(k.indukId); const d = daysBetween(TODAY, parseIso(k.perkiraanLahir)); return `<li><button type="button" data-act="openTernak" data-arg="${t.id}"><span class="count${d <= 7 ? ' soon' : ''}">${d >= 0 ? `H-${d}` : `+${-d}`}</span><span class="who"><b>${t.kode}</b><small>${spOf(t).nama} ${rasOf(t).nama} · ${fmtShort(k.perkiraanLahir)}</small></span></button></li>`; }).join('')}</ul><p class="tile-foot"><span class="muted">${births.length} induk bunting</span></p>`);

  // Health.
  const diobati = act.filter((t) => t.kondisi === 'Diobati').length;
  const periksa = act.filter((t) => t.kondisi === 'Periksa').length;
  const vak7 = db.jadwal.filter((j) => j.status === 'Terjadwal' && daysBetween(TODAY, parseIso(j.rencana)) >= 0 && daysBetween(TODAY, parseIso(j.rencana)) <= 7).length;
  const health = tile('t-health', 'heart', 'Kesehatan', canPage('kesehatan') ? 'kesehatan' : null, `<svg class="pulse" viewBox="0 0 240 40" preserveAspectRatio="none" aria-hidden="true"><path d="M0 22h70l8-12 10 24 10-30 10 26 7-8h125"/></svg>
    <div class="trio"><div><b class="num" style="color:var(--danger)">${diobati}</b><span>diobati</span></div><div><b class="num" style="color:var(--warn)">${periksa}</b><span>perlu periksa</span></div><div><b class="num">${vak7}</b><span>vaksin 7 hari</span></div></div>`);

  // Money.
  let money = '';
  if (canPage('keuangan')) {
    const months = [];
    for (let i = 5; i >= 0; i--) {
      const key = iso(new Date(TODAY.getFullYear(), TODAY.getMonth() - i, 1)).slice(0, 7);
      const rows = db.keuangan.filter((x) => x.tanggal.slice(0, 7) === key);
      months.push({ masuk: rows.filter((x) => x.jenis === 'Masuk').reduce((a, x) => a + x.jumlah, 0), keluar: rows.filter((x) => x.jenis === 'Keluar').reduce((a, x) => a + x.jumlah, 0) });
    }
    const cur = months[months.length - 1];
    const laba = cur.masuk - cur.keluar;
    money = tile('t-money', 'wallet', 'Laba bulan ini', 'keuangan', `<div class="money"><b class="num" style="color:${laba >= 0 ? 'var(--primary)' : 'var(--danger)'}">${rpShort(laba)}</b>${sparkline(months.map((m) => m.masuk - m.keluar))}</div>
      <div class="trio two"><div><b class="num">${rpShort(cur.masuk)}</b><span>masuk</span></div><div><b class="num">${rpShort(cur.keluar)}</b><span>keluar</span></div></div>`);
  }

  // Latest weighings.
  const latest = [...db.timbang].sort((a, b) => (a.tanggal < b.tanggal ? 1 : a.tanggal > b.tanggal ? -1 : b.id - a.id)).slice(0, 5);
  const wRows = latest.map((w) => {
    const t = T(w.ternakId);
    const arr = weightsOf(t.id);
    const i = arr.indexOf(w);
    const prev = i > 0 ? arr[i - 1] : null;
    const g = prev ? (w.berat - prev.berat) / Math.max(1, daysBetween(parseIso(prev.tanggal), parseIso(w.tanggal))) : null;
    return `<tr class="clickable" data-act="openTernak" data-arg="${t.id}"><td><a href="#" data-act="openTernak" data-arg="${t.id}" class="strong">${t.kode}</a> <span class="muted">${rasOf(t).nama}</span></td><td class="r strong">${num(w.berat, 1)} kg</td><td class="r strong" style="color:${g === null ? 'var(--text-2)' : g >= 0 ? 'var(--primary)' : 'var(--danger)'}">${g === null ? '—' : `${g >= 0 ? '+' : '−'}${num(Math.abs(g), 2)}`}</td><td class="r muted">${fmtShort(w.tanggal)}</td></tr>`;
  }).join('');
  const weigh = tile('t-weigh', 'scale', 'Penimbangan terbaru', 'ternak', `<div class="table-wrap"><table class="data compact"><thead><tr><th>Ternak</th><th class="r">Berat</th><th class="r">ADG kg/hari</th><th class="r">Tanggal</th></tr></thead><tbody>${wRows}</tbody></table></div>`);

  const guest = S.mode === 'tamu' ? `<div class="guest-strip">${icon('info', 18)}<span>Anda memakai <b>database tamu</b> (${S.db.file}). Data di sini tidak tercampur dengan data akun.</span><button type="button" class="btn btn-secondary btn-sm" data-act="logout">Masuk dengan akun</button></div>` : '';
  return `${hero}${guest}<div class="bento${feed ? '' : ' no-feed'}${money ? '' : ' no-money'}">${pop}${remTile}${pen}${feed}${repro}${health}${money}${weigh}</div>`;
}

const PER_PAGE = 12;
function ternakFiltered() {
  const f = S.ui.ternak;
  const q = f.q.trim().toLowerCase();
  return S.db.ternak.filter((t) => (f.status === 'Semua' || t.status === f.status)
    && (f.kandang === 'all' || t.kandangId === Number(f.kandang))
    && (!q || `${t.kode} ${t.nama} ${rasOf(t).nama} ${spOf(t).nama}`.toLowerCase().includes(q)));
}
function pageTernak() {
  const f = S.ui.ternak;
  const base = ternakFiltered();
  const list = base.filter((t) => f.sp === 'all' || t.spesiesId === Number(f.sp)).sort((a, b) => a.spesiesId - b.spesiesId || a.kode.localeCompare(b.kode));
  const pages = Math.max(1, Math.ceil(list.length / PER_PAGE));
  if (f.page > pages) f.page = pages;
  const slice = list.slice((f.page - 1) * PER_PAGE, f.page * PER_PAGE);
  const chips = [['all', 'Semua', base.length], ...S.db.spesies.map((s) => [String(s.id), s.nama, base.filter((t) => t.spesiesId === s.id).length])]
    .map(([v, l, c]) => `<button type="button" class="chip" aria-pressed="${f.sp === v}" data-act="ternakSp" data-arg="${v}">${l}<small>${c}</small></button>`).join('');
  const rows = slice.map((t) => {
    const w = lastWeight(t.id);
    return `<tr class="clickable${S.fresh === t.id ? ' fresh' : ''}" data-act="openTernak" data-arg="${t.id}">
      <td><a href="#" class="strong" data-act="openTernak" data-arg="${t.id}">${t.kode}</a></td>
      <td><span class="strong" style="font-weight:600">${esc(t.nama || '—')}</span> <span class="muted">· ${rasOf(t).nama}</span></td>
      <td>${spOf(t).nama}</td><td>${t.jk}</td><td class="num">${umur(t.lahir)}</td>
      <td class="muted">${t.kandangId ? kLabel(t.kandangId) : '—'}</td>
      <td class="r strong">${w ? `${num(w.berat, 1)} kg` : '—'}</td>
      <td>${kondisiBadge(t)}</td>
      <td class="r"><button type="button" class="btn btn-ghost btn-sm" data-act="openTernak" data-arg="${t.id}">Detail ${icon('chevR', 14, 2)}</button></td></tr>`;
  }).join('');
  const pg = [];
  for (let i = 1; i <= pages; i++) if (i === 1 || i === pages || Math.abs(i - f.page) <= 1) pg.push(i); else if (pg[pg.length - 1] !== '…') pg.push('…');
  const nAktif = aktif().length;
  return `${pageHead('Ternak', `${num(nAktif)} ekor aktif di ${S.db.kandang.length} kandang`,
    `<button type="button" class="btn btn-secondary" data-act="snackExport" data-arg="ternak">${icon('excel', 16)}Ekspor Excel</button>${can('tambahTernak') ? `<button type="button" class="btn btn-primary" data-act="dlg" data-arg="tambahTernak">${icon('plus', 16, 2.25)}Tambah ternak</button>` : ''}`)}
  <section class="card">
    <div class="toolbar">
      <label class="search-box" style="width:280px">${icon('search', 16, 2)}<span class="sr">Cari ternak</span><input id="f-q" type="search" placeholder="Cari kode, nama, atau ras" value="${esc(f.q)}" data-bind="ui.ternak.q" data-reset-page></label>
      <div class="chips" role="group" aria-label="Spesies">${chips}</div>
      <span style="flex:1"></span>
      <label class="sr" for="f-kandang">Kandang</label><select class="select" id="f-kandang" data-bind="ui.ternak.kandang" data-reset-page>${options([['all', 'Semua kandang'], ...S.db.kandang.map((k) => [k.id, `${k.kode} · ${k.nama}`])], f.kandang)}</select>
      <label class="sr" for="f-status">Status</label><select class="select" id="f-status" data-bind="ui.ternak.status" data-reset-page>${options([['Aktif', 'Aktif'], ['Terjual', 'Terjual'], ['Mati', 'Mati'], ['Semua', 'Semua status']], f.status)}</select>
    </div>
    ${slice.length ? `<div class="table-wrap"><table class="data"><thead><tr><th>Kode</th><th>Nama · ras</th><th>Spesies</th><th>Kelamin</th><th>Umur</th><th>Kandang</th><th class="r">Berat terakhir</th><th>Kondisi</th><th class="r"><span class="sr">Aksi</span></th></tr></thead><tbody>${rows}</tbody></table></div>`
      : '<div class="empty"><strong>Tidak ada ternak yang cocok</strong>Coba kata kunci lain atau pilih spesies Semua.</div>'}
    <div class="pager"><span>Menampilkan ${slice.length} dari ${num(list.length)} ternak · halaman ${f.page} dari ${pages}</span>
      <div class="pages"><button type="button" data-act="ternakPage" data-arg="${f.page - 1}" aria-label="Sebelumnya" ${f.page <= 1 ? 'disabled' : ''}>${icon('chevL', 14, 2)}</button>${pg.map((p) => (p === '…' ? '<button type="button" disabled>…</button>' : `<button type="button" data-act="ternakPage" data-arg="${p}" ${p === f.page ? 'aria-current="page"' : ''}>${p}</button>`)).join('')}<button type="button" data-act="ternakPage" data-arg="${f.page + 1}" aria-label="Berikutnya" ${f.page >= pages ? 'disabled' : ''}>${icon('chevR', 14, 2)}</button></div></div>
  </section>`;
}

function pageDetail() {
  const t = T(S.route.id);
  if (!t) return '<div class="empty"><strong>Ternak tidak ditemukan</strong></div>';
  const sp = spOf(t), ras = rasOf(t);
  const w = weightsOf(t.id);
  const last = w[w.length - 1], first = w[0];
  const g = adg(t.id);
  const k = K(t.kandangId);
  const tab = S.ui.detailTab;
  const kw = S.db.kawin.filter((x) => x.indukId === t.id).sort((a, b) => (a.tanggal < b.tanggal ? 1 : -1));
  const bunting = kw.find((x) => x.status === 'Bunting');

  let fourth;
  if (t.jk === 'Jantan' && k && k.jenis === 'Penggemukan') {
    const target = t.spesiesId === 1 ? 450 : 40;
    const eta = g && g > 0 && last ? Math.max(0, Math.ceil((target - last.berat) / g)) : null;
    fourth = stat(`Target jual ${target} kg`, eta === null ? '—' : last.berat >= target ? 'Siap' : `± ${eta}`, eta && last.berat < target ? 'hari' : '', g ? `jika ADG tetap ${num(g, 2)} kg/hari` : 'Butuh dua penimbangan', 'info', 'trend');
  } else if (t.jk === 'Betina') {
    fourth = bunting
      ? stat('Status reproduksi', 'Bunting', '', `Perkiraan lahir ${fmtDate(bunting.perkiraanLahir)} · ${relDay(bunting.perkiraanLahir)}`, 'warn', 'repro')
      : stat('Status reproduksi', 'Kosong', '', kw.length ? `Terakhir kawin ${fmtDate(kw[0].tanggal)}` : 'Belum ada catatan perkawinan', 'warn', 'repro');
  } else {
    fourth = stat('Anak tercatat', S.db.ternak.filter((x) => x.pejantanId === t.id).length, 'ekor', 'Sebagai pejantan', 'info', 'repro');
  }

  const tabs = [['pertumbuhan', 'Pertumbuhan'], ['profil', 'Profil'], ['kesehatan', 'Kesehatan'], ['reproduksi', 'Reproduksi'], ['silsilah', 'Silsilah']];
  let body = '';
  if (tab === 'pertumbuhan') {
    const pts = w.map((x) => ({ label: fmtShort(x.tanggal), y: x.berat }));
    const target = t.jk === 'Jantan' && k && k.jenis === 'Penggemukan' ? (t.spesiesId === 1 ? 450 : 40) : null;
    const riw = [...w].reverse().map((x, i, arr) => { const p = arr[i + 1]; const d = p ? x.berat - p.berat : null; return `<tr><td>${fmtDate(x.tanggal)}</td><td class="r strong">${num(x.berat, 1)} kg</td><td class="r" style="color:${d === null ? 'var(--text-2)' : d >= 0 ? 'var(--primary)' : 'var(--danger)'};font-weight:600">${d === null ? '—' : `${d >= 0 ? '+' : '−'}${num(Math.abs(d), 1)}`}</td><td class="r muted">${x.lingkarDada} cm</td></tr>`; }).join('');
    body = `<div class="row-2"><section class="card card-pad wide" style="display:flex;flex-direction:column;gap:12px"><div class="card-head"><div><h2>Berat badan sejak masuk</h2><p>Penimbangan tiap 14 hari · kg</p></div></div>${pts.length ? lineChart(pts, `Berat ${t.kode} dari ${first.berat} menjadi ${last.berat} kg`, { target }) : '<div class="empty">Belum ada penimbangan</div>'}</section>
      <section class="card narrow"><div class="card-head" style="padding:20px 20px 8px"><h2>Riwayat penimbangan</h2></div><div class="table-wrap" style="max-height:300px"><table class="data"><thead><tr><th>Tanggal</th><th class="r">Berat</th><th class="r">Selisih</th><th class="r">L. dada</th></tr></thead><tbody>${riw}</tbody></table></div></section></div>`;
  } else if (tab === 'profil') {
    const induk = t.indukId ? T(t.indukId) : null, pej = t.pejantanId ? T(t.pejantanId) : null;
    const kv = [['Kode ear tag', t.kode], ['Nama', t.nama || '—'], ['Spesies · ras', `${sp.nama} · ${ras.nama}`], ['Jenis kelamin', t.jk], ['Tanggal lahir', `${fmtDate(t.lahir)} · ${umur(t.lahir)}`], ['Asal usul', t.asal === 'Beli' ? `Dibeli · ${fmtDate(t.tanggalMasuk)}` : 'Lahir di peternakan'], ['Harga beli', t.hargaBeli ? rp(t.hargaBeli) : '—'], ['Kandang', k ? kLabel(k.id) : '—'], ['Induk', induk ? tLabel(induk) : '—'], ['Pejantan', pej ? tLabel(pej) : '—'], ['Status', t.status], ['Dicatat oleh', t.dicatatOleh]];
    body = `<section class="card card-pad"><div class="kv">${kv.map(([a, b]) => `<div>${a}</div><div>${esc(b)}</div>`).join('')}</div></section>`;
  } else if (tab === 'kesehatan') {
    const recs = S.db.kesehatan.filter((x) => x.ternakId === t.id).sort((a, b) => (a.tanggal < b.tanggal ? 1 : -1));
    const jd = S.db.jadwal.filter((x) => x.ternakId === t.id).sort((a, b) => (a.rencana < b.rencana ? 1 : -1));
    const tl = [...recs.map((r) => ({ d: r.tanggal, title: r.diagnosa, sub: `${r.keluhan} · ${r.tindakan}`, who: 'Mantri', warn: true })),
      ...jd.map((j) => ({ d: j.realisasi || j.rencana, title: `${S.ix.obat.get(j.obatId).nama} · ${j.status.toLowerCase()}`, sub: j.status === 'Terjadwal' ? `Dijadwalkan ${relDay(j.rencana)}` : j.status === 'Terlewat' ? 'Belum dilakukan' : 'Tanpa keluhan setelah vaksin.', who: 'Mantri', warn: j.status !== 'Selesai' }))]
      .sort((a, b) => (a.d < b.d ? 1 : -1));
    body = `<section class="card card-pad" style="display:flex;flex-direction:column;gap:16px"><div class="card-head"><h2>Riwayat kesehatan</h2>${can('kesehatan') && t.status === 'Aktif' ? `<button type="button" class="btn btn-secondary btn-sm" data-act="dlg" data-arg="periksa:${t.id}">${icon('plus', 14, 2)}Catat pemeriksaan</button>` : ''}</div>
      ${tl.length ? `<ul class="timeline">${tl.map((x) => `<li><span class="dot${x.warn ? ' warn' : ''}"></span><div><small>${fmtDate(x.d)} · ${x.who}</small><b>${esc(x.title)}</b><p>${esc(x.sub)}</p></div></li>`).join('')}</ul>` : '<div class="empty"><strong>Belum ada catatan kesehatan</strong></div>'}</section>`;
  } else if (tab === 'reproduksi') {
    const anak = S.db.ternak.filter((x) => x.indukId === t.id || x.pejantanId === t.id);
    body = `<div class="row-2"><section class="card wide"><div class="card-head" style="padding:20px 20px 8px"><h2>${t.jk === 'Betina' ? 'Riwayat perkawinan' : 'Sebagai pejantan'}</h2></div>
      ${t.jk === 'Betina' ? (kw.length ? `<div class="table-wrap"><table class="data"><thead><tr><th>Tanggal kawin</th><th>Metode</th><th>Status</th><th class="r">Perkiraan lahir</th></tr></thead><tbody>${kw.map((x) => `<tr><td>${fmtDate(x.tanggal)}</td><td>${x.metode === 'IB' ? `IB · ${x.kodeStraw}` : `Alami · ${x.pejantanId ? T(x.pejantanId).kode : '—'}`}</td><td>${kawinBadge(x)}</td><td class="r">${fmtDate(x.perkiraanLahir)}</td></tr>`).join('')}</tbody></table></div>` : '<div class="empty"><strong>Belum ada perkawinan</strong></div>')
        : `<div class="empty"><strong>${S.db.kawin.filter((x) => x.pejantanId === t.id).length} perkawinan</strong>tercatat dengan ${t.kode} sebagai pejantan.</div>`}</section>
      <section class="card narrow"><div class="card-head" style="padding:20px 20px 8px"><h2>Keturunan</h2><span class="badge neutral plain">${anak.length}</span></div><div style="padding:0 20px 12px"><ul class="mini-list">${anak.length ? anak.map((a) => `<li><a href="#" data-act="openTernak" data-arg="${a.id}" style="font-weight:600">${tLabel(a)}</a><span class="muted">${a.jk} · ${umur(a.lahir)}</span></li>`).join('') : '<li class="muted">Belum ada keturunan tercatat</li>'}</ul></div></section></div>`;
  } else {
    const node = (x, role) => (x ? `<a href="#" class="node" data-act="openTernak" data-arg="${x.id}" style="color:inherit"><small>${role}</small><b>${tLabel(x)}</b><small>${rasOf(x).nama} · ${umur(x.lahir)}</small></a>` : `<div class="node unknown"><small>${role}</small><b>Tidak diketahui</b></div>`);
    const induk = t.indukId ? T(t.indukId) : null, pej = t.pejantanId ? T(t.pejantanId) : null;
    const anak = S.db.ternak.filter((x) => x.indukId === t.id || x.pejantanId === t.id).slice(0, 6);
    body = (!induk && !pej && t.asal === 'Beli')
      ? `<section class="card"><div class="empty"><strong>Silsilah belum tercatat</strong>${esc(t.nama || t.kode)} dibeli dari luar, jadi induk dan pejantannya tidak ada di data.</div></section>`
      : `<section class="card card-pad"><div class="tree">
        <div class="lvl">${node(induk && induk.indukId ? T(induk.indukId) : null, 'Nenek (dari induk)')}${node(pej && pej.pejantanId ? T(pej.pejantanId) : null, 'Kakek (dari pejantan)')}</div>
        <div class="lvl">${node(induk, 'Induk')}${node(pej, 'Pejantan')}</div>
        <div class="lvl"><div class="node me"><small>Ternak ini</small><b>${tLabel(t)}</b><small>${ras.nama} · ${umur(t.lahir)}</small></div></div>
        ${anak.length ? `<div class="lvl">${anak.map((a) => node(a, 'Anak')).join('')}</div>` : ''}</div></section>`;
  }

  return `<nav class="crumb" aria-label="Lokasi"><button type="button" data-act="go" data-arg="ternak">Ternak</button>${icon('chevR', 14, 2)}<span>${t.kode}</span></nav>
  <section class="card card-pad"><div class="profile">
    <div class="photo" aria-label="Foto ternak belum ada">${COW}</div>
    <div class="meta"><h1>${esc(t.nama || t.kode)} <small>${t.kode}</small> ${kondisiBadge(t)}</h1>
      <div class="facts"><span>${sp.nama} ${ras.nama} · ${t.jk}</span><span>Lahir ${fmtDate(t.lahir)} · ${umur(t.lahir)}</span><span>${k ? `Kandang ${kLabel(k.id)}` : t.status}</span><span>${t.asal === 'Beli' ? `Dibeli ${fmtDate(t.tanggalMasuk)}` : 'Lahir di peternakan'}</span></div></div>
    <div class="actions" style="display:flex;gap:10px;flex-wrap:wrap">${t.status === 'Aktif' && can('mutasi') ? `<button type="button" class="btn btn-secondary" data-act="dlg" data-arg="mutasi:${t.id}">${icon('swap', 16)}Mutasi</button>` : ''}${t.status === 'Aktif' && can('timbang') ? `<button type="button" class="btn btn-primary" data-act="dlg" data-arg="timbang:${t.id}">${icon('scale', 16)}Catat penimbangan</button>` : ''}</div>
  </div></section>
  <section class="grid-stats">
    ${stat('Berat terakhir', last ? num(last.berat, 1) : '—', 'kg', last ? `Ditimbang ${fmtDate(last.tanggal)}` : 'Belum ditimbang', 'ok', 'scale')}
    ${stat('ADG 30 hari', g === null ? '—' : `${g >= 0 ? '+' : '−'}${num(Math.abs(g), 2)}`, 'kg/hari', g === null ? 'Butuh dua penimbangan' : g >= 0 ? 'Pertambahan bobot harian' : 'Berat turun, periksa kondisi', g !== null && g < 0 ? 'danger' : 'ok', 'trend')}
    ${stat('Naik sejak masuk', last && first ? `${last.berat - first.berat >= 0 ? '+' : '−'}${num(Math.abs(last.berat - first.berat), 1)}` : '—', 'kg', last && first ? `dari ${num(first.berat, 1)} kg dalam ${daysBetween(parseIso(first.tanggal), parseIso(last.tanggal))} hari` : '', 'ok', 'tag')}
    ${fourth}
  </section>
  <div class="tabs" role="tablist">${tabs.map(([v, l]) => `<button type="button" role="tab" aria-selected="${tab === v}" data-act="detailTab" data-arg="${v}">${l}</button>`).join('')}</div>
  ${body}`;
}

function kawinBadge(k) {
  return { Bunting: '<span class="badge warn">Bunting</span>', Menunggu: '<span class="badge info">Menunggu cek</span>', Gagal: '<span class="badge neutral">Gagal</span>', Lahir: '<span class="badge ok">Sudah lahir</span>' }[k.status];
}

function jatah(k, p) {
  const isi = isiKandang(k.id);
  if (p.satuan !== 'kg') return Math.max(1, Math.round(isi / 20));
  const sp = k.spesiesId || 1;
  const rate = { Konsentrat: [2, 0.4, 0.3], Dedak: [1, 0.25, 0.2], 'Rumput gajah': [25, 4, 3], 'Hijauan campur': [8, 2, 1.5], 'Ampas tahu': [3, 0.5, 0.4] }[p.nama] || [5, 1, 1];
  return Math.round(isi * rate[sp - 1]);
}

function pageKandang() {
  const db = S.db;
  if (!S.ui.kandangSel || !K(S.ui.kandangSel)) S.ui.kandangSel = db.kandang[0].id;
  const tot = db.kandang.reduce((a, k) => a + k.kapasitas, 0);
  const isiTot = db.kandang.reduce((a, k) => a + isiKandang(k.id), 0);
  const pens = db.kandang.map((k) => {
    const isi = isiKandang(k.id), pct = Math.round(isi / k.kapasitas * 100), c = pctClass(pct);
    return `<button type="button" class="pen" aria-pressed="${S.ui.kandangSel === k.id}" data-act="selPen" data-arg="${k.id}">
      <div class="ttl"><span class="code ${c}">${k.kode}</span><div><b>${esc(k.nama)}</b><small>${k.spesiesId ? S.ix.spesies.get(k.spesiesId).nama : 'Campuran'} · ${k.jenis}</small></div></div>
      ${meter(pct, c === 'ok' ? '' : c)}
      <div class="nums"><span><b class="num">${isi}</b> ekor</span><span class="num">${pct}% · kapasitas ${k.kapasitas}</span></div></button>`;
  }).join('');
  const k = K(S.ui.kandangSel);
  const isi = isiKandang(k.id), pct = Math.round(isi / k.kapasitas * 100);
  const list = aktif().filter((t) => t.kandangId === k.id);
  const logs = db.pakanLog.filter((l) => l.kandangId === k.id && l.tanggal === iso(TODAY));
  const nextVak = db.jadwal.filter((j) => j.status !== 'Selesai' && T(j.ternakId)?.kandangId === k.id).sort((a, b) => (a.rencana < b.rencana ? -1 : 1))[0];
  const sakit = list.filter((t) => t.kondisi !== 'Sehat').length;
  return `${pageHead('Denah kandang', `${db.kandang.length} kandang · ${num(isiTot)} dari ${num(tot)} tempat terisi (${Math.round(isiTot / tot * 100)}%)`,
    `<div class="legend"><span><i style="background:var(--primary)"></i>di bawah 80%</span><span><i style="background:var(--warn-bar)"></i>80–99%</span><span><i style="background:var(--danger)"></i>penuh</span></div>${can('master') ? `<button type="button" class="btn btn-primary" data-act="dlg" data-arg="master:kandang">${icon('plus', 16, 2.25)}Tambah kandang</button>` : ''}`)}
  <div class="split">
    <div class="pens">${pens}</div>
    <aside class="card card-pad side-panel${enter('panel')}" style="display:flex;flex-direction:column;gap:16px" aria-label="Detail kandang">
      <div><h2 style="font-size:18px">Kandang ${k.kode} · ${esc(k.nama)}</h2><p class="muted" style="font-size:13px">${k.jenis} · ${isi} dari ${k.kapasitas} ekor</p></div>
      ${meter(pct, pctClass(pct) === 'ok' ? '' : pctClass(pct))}
      <div style="display:flex;gap:8px;flex-wrap:wrap">${canPage('pakan') && can('pakan') ? `<button type="button" class="btn btn-primary btn-sm" data-act="beriPakan" data-arg="${k.id}">${icon('box', 14)}Beri pakan</button>` : ''}<button type="button" class="btn btn-secondary btn-sm" data-act="lihatKandang" data-arg="${k.id}">${icon('tag', 14)}Lihat ternak</button></div>
      <div><div class="label" style="font-size:12px;font-weight:600;color:var(--text-2);text-transform:uppercase;letter-spacing:.06em;margin-bottom:4px">Pakan hari ini</div>
        <ul class="mini-list">${logs.length ? logs.map((l) => `<li><span>${S.ix.pakan.get(l.pakanId).nama}</span><span class="num strong">${num(l.jumlah)} ${S.ix.pakan.get(l.pakanId).satuan} · ${l.jam}</span></li>`).join('') : '<li class="muted">Belum diberi pakan hari ini</li>'}</ul></div>
      <div><div class="label" style="font-size:12px;font-weight:600;color:var(--text-2);text-transform:uppercase;letter-spacing:.06em;margin-bottom:4px">Kesehatan</div>
        <ul class="mini-list"><li><span>Perlu perhatian</span><span>${sakit ? `<span class="badge warn">${sakit} ekor</span>` : '<span class="badge ok">Semua sehat</span>'}</span></li>
        <li><span>Vaksin berikutnya</span><span class="muted">${nextVak ? `${S.ix.obat.get(nextVak.obatId).nama.replace('Vaksin ', '')} · ${relDay(nextVak.rencana)}` : '—'}</span></li></ul></div>
      <div><div style="display:flex;justify-content:space-between;align-items:center"><span class="label" style="font-size:12px;font-weight:600;color:var(--text-2);text-transform:uppercase;letter-spacing:.06em">Ternak di kandang ini</span><a href="#" data-act="lihatKandang" data-arg="${k.id}" style="font-size:13px;font-weight:600">Lihat ${isi}</a></div>
        <ul class="mini-list">${list.slice(0, 6).map((t) => { const w = lastWeight(t.id); return `<li><a href="#" data-act="openTernak" data-arg="${t.id}" style="font-weight:600">${tLabel(t)}</a><span style="display:flex;gap:8px;align-items:center">${t.kondisi !== 'Sehat' ? kondisiBadge(t) : ''}<span class="num muted">${w ? `${num(w.berat, 1)} kg` : ''}</span></span></li>`; }).join('') || '<li class="muted">Kandang kosong</li>'}</ul></div>
    </aside>
  </div>`;
}

function pagePakan() {
  const db = S.db;
  const f = S.ui.pakanForm;
  if (!f.kandangId) f.kandangId = String(db.kandang[0].id);
  if (!f.pakanId) f.pakanId = String(db.pakan[0].id);
  const p = S.ix.pakan.get(Number(f.pakanId));
  const k = K(f.kandangId);
  const j = Number(String(f.jumlah).replace(',', '.'));
  const over = f.jumlah !== '' && j > p.stok;
  const invalid = f.jumlah !== '' && !(j > 0);
  const stocks = db.pakan.map((x) => {
    const st = x.stok <= 0 ? ['danger', 'Habis'] : x.stok < x.min ? ['warn', 'Menipis'] : ['ok', 'Aman'];
    const hari = x.pakai ? Math.floor(x.stok / x.pakai) : null;
    return `<div class="card stock lift"><div class="head"><span>${esc(x.nama)}</span><span class="badge ${st[0]}">${st[1]}</span></div>
      <div class="amt">${num(x.stok)}<small>${x.satuan}</small></div>${meter(x.stok / (x.min * 3) * 100, st[0] === 'ok' ? '' : st[0])}
      <div class="sub">Minimum ${num(x.min)} · pakai ${num(x.pakai, x.pakai < 1 ? 1 : 0)}/hari${hari !== null ? ` · cukup ± ${hari} hari` : ''}</div></div>`;
  }).join('');
  const todayLogs = db.pakanLog.filter((l) => l.tanggal === iso(TODAY)).sort((a, b) => b.id - a.id);
  const jt = jatah(k, p);
  const masuk = [...db.pakanMasuk].sort((a, b) => (a.tanggal < b.tanggal ? 1 : a.tanggal > b.tanggal ? -1 : b.id - a.id)).slice(0, 6);
  return `${pageHead('Pakan', `Stok gudang dan pemberian pakan hari ini · ${fmtLong(TODAY)}`, `<button type="button" class="btn btn-primary" data-act="openPakanMasuk" data-arg="">${icon('download', 16, 2)}Catat pakan masuk</button>`)}
  <section class="stocks" aria-label="Stok gudang">${stocks}</section>
  <div class="row-2">
    <section class="card card-pad narrow" style="display:flex;flex-direction:column;gap:14px" aria-labelledby="h-beri">
      <h2 id="h-beri" style="font-size:16px">Catat pemberian pakan</h2>
      <form data-form="pakan" novalidate style="display:flex;flex-direction:column;gap:14px">
        <div class="field"><label for="pk-kandang">Kandang</label><select class="select" id="pk-kandang" name="kandangId" data-bind="ui.pakanForm.kandangId">${options(db.kandang.map((x) => [x.id, `${x.kode} · ${x.nama} (${isiKandang(x.id)} ekor)`]), f.kandangId)}</select></div>
        <div class="field"><label for="pk-pakan">Jenis pakan</label><select class="select" id="pk-pakan" name="pakanId" data-bind="ui.pakanForm.pakanId">${options(db.pakan.map((x) => [x.id, `${x.nama} · stok ${num(x.stok)} ${x.satuan}`]), f.pakanId)}</select></div>
        <div class="field"><label for="pk-jml">Jumlah</label>
          <div class="input-icon"><input class="input${over || invalid ? ' invalid' : ''}" id="pk-jml" name="jumlah" inputmode="decimal" placeholder="0" value="${esc(f.jumlah)}" data-bind="ui.pakanForm.jumlah" style="padding-right:56px" aria-describedby="pk-hint"><span style="position:absolute;right:12px;top:50%;transform:translateY(-50%);color:var(--text-2);font-size:13px">${p.satuan}</span></div>
          ${over ? `<span class="error" id="pk-hint">Melebihi stok gudang (${num(p.stok)} ${p.satuan})</span>` : invalid ? '<span class="error" id="pk-hint">Isi jumlah lebih dari 0</span>' : `<span class="hint" id="pk-hint">Stok gudang: ${num(p.stok)} ${p.satuan}</span>`}</div>
        <div class="field"><span class="label">Isi cepat</span><div class="quick">
          <button type="button" class="chip" data-act="pakanQuick" data-arg="${jt}">Jatah harian · ${num(jt)} ${p.satuan}</button>
          <button type="button" class="chip" data-act="pakanQuick" data-arg="${Math.round(jt / 2)}">Setengah · ${num(Math.round(jt / 2))}</button></div></div>
        <button type="submit" class="btn btn-primary" ${over || invalid || f.jumlah === '' ? 'disabled style="opacity:.5;cursor:not-allowed"' : ''}>${icon('check', 16, 2.25)}Simpan <kbd style="background:transparent;color:inherit;border-color:rgba(255,255,255,.4)">Ctrl S</kbd></button>
      </form>
    </section>
    <section class="card wide" aria-labelledby="h-log"><div class="card-head" style="padding:20px 20px 8px"><h2 id="h-log">Pemberian hari ini</h2><span class="badge neutral plain">${todayLogs.length} catatan</span></div>
      ${todayLogs.length ? `<div class="table-wrap"><table class="data"><thead><tr><th>Waktu</th><th>Kandang</th><th>Pakan</th><th class="r">Jumlah</th><th>Dicatat</th></tr></thead><tbody>${todayLogs.map((l) => `<tr class="${S.fresh === `log${l.id}` ? 'fresh' : ''}"><td class="num">${l.jam}</td><td>${kLabel(l.kandangId)}</td><td>${S.ix.pakan.get(l.pakanId).nama}</td><td class="r strong">${num(l.jumlah)} ${S.ix.pakan.get(l.pakanId).satuan}</td><td class="muted">${esc(l.oleh)}</td></tr>`).join('')}</tbody></table></div>` : '<div class="empty"><strong>Belum ada pemberian hari ini</strong></div>'}
      <div class="card-head" style="padding:20px 20px 8px;border-top:1px solid var(--border)"><h2>Pakan masuk terakhir</h2></div>
      <div class="table-wrap"><table class="data"><thead><tr><th>Tanggal</th><th>Pakan</th><th>Pemasok</th><th class="r">Jumlah</th><th class="r">Total</th></tr></thead><tbody>${masuk.map((m) => { const x = S.ix.pakan.get(m.pakanId); return `<tr class="${S.fresh === `pm${m.id}` ? 'fresh' : ''}"><td>${fmtDate(m.tanggal)}</td><td>${x.nama}</td><td class="muted">${esc(m.pemasok)}</td><td class="r">${num(m.jumlah)} ${x.satuan}</td><td class="r strong">${rp(m.total)}</td></tr>`; }).join('')}</tbody></table></div>
    </section>
  </div>`;
}

function pageKesehatan() {
  const db = S.db;
  const act = aktif();
  const diobati = act.filter((t) => t.kondisi === 'Diobati');
  const periksa = act.filter((t) => t.kondisi === 'Periksa');
  const vak7 = db.jadwal.filter((j) => j.status === 'Terjadwal' && daysBetween(TODAY, parseIso(j.rencana)) >= 0 && daysBetween(TODAY, parseIso(j.rencana)) <= 7).length;
  const lewat = db.jadwal.filter((j) => j.status === 'Terlewat').length;
  const obatTipis = db.obat.filter((o) => o.stok < o.min);
  const recs = [...db.kesehatan].sort((a, b) => (a.tanggal < b.tanggal ? 1 : a.tanggal > b.tanggal ? -1 : b.id - a.id)).slice(0, 12);

  const [yy, mm] = S.ui.calMonth.split('-').map(Number);
  const first = new Date(yy, mm - 1, 1);
  const start = addDays(first, -((first.getDay() + 6) % 7));
  const byDay = new Map();
  for (const j of db.jadwal) { const d = j.rencana; if (!byDay.has(d)) byDay.set(d, []); byDay.get(d).push(j); }
  let cal = ['Sen', 'Sel', 'Rab', 'Kam', 'Jum', 'Sab', 'Min'].map((d) => `<span class="dow">${d}</span>`).join('');
  for (let i = 0; i < 42; i++) {
    const d = addDays(start, i), s = iso(d);
    const js = byDay.get(s) || [];
    const kinds = [...new Set(js.map((j) => (j.status === 'Selesai' ? 'ok' : j.status === 'Terlewat' ? 'danger' : '')))].slice(0, 3);
    cal += `<button type="button" class="day${d.getMonth() !== mm - 1 ? ' out' : ''}${s === iso(TODAY) ? ' today' : ''}${s === S.ui.calDay ? ' sel' : ''}" data-act="calDay" data-arg="${s}" aria-label="${fmtDate(s)}${js.length ? `, ${js.length} jadwal` : ''}">${d.getDate()}<span class="pips">${kinds.map((c) => `<i class="${c}"></i>`).join('')}</span></button>`;
  }
  const dayJ = (byDay.get(S.ui.calDay) || []);
  const grouped = new Map();
  for (const j of dayJ) { const t = T(j.ternakId); const key = `${j.obatId}-${t.kandangId}-${j.status}`; if (!grouped.has(key)) grouped.set(key, { j, k: t.kandangId, ids: [] }); grouped.get(key).ids.push(j.id); }
  const dayList = [...grouped.values()].map((g) => `<li><span><b style="font-weight:600">${S.ix.obat.get(g.j.obatId).nama}</b><br><small class="muted">Kandang ${K(g.k)?.kode || '—'} · ${g.ids.length} ekor</small></span>${g.j.status === 'Terjadwal' && can('kesehatan') ? `<button type="button" class="btn btn-secondary btn-sm" data-act="vaksinSelesai" data-arg="${g.ids.join(',')}">Tandai selesai</button>` : `<span class="badge ${g.j.status === 'Selesai' ? 'ok' : g.j.status === 'Terlewat' ? 'danger' : 'info'}">${g.j.status}</span>`}</li>`).join('');

  const perhatian = [...diobati, ...periksa];
  return `${pageHead('Kesehatan', 'Pemeriksaan, pengobatan, dan jadwal vaksin', can('kesehatan') ? `<button type="button" class="btn btn-secondary" data-act="dlg" data-arg="jadwal">${icon('calendar', 16)}Jadwalkan vaksin</button><button type="button" class="btn btn-primary" data-act="dlg" data-arg="periksa">${icon('plus', 16, 2.25)}Catat pemeriksaan</button>` : '')}
  <section class="grid-stats">
    ${stat('Sedang diobati', diobati.length, 'ekor', 'Dalam masa pengobatan', 'danger', 'heart')}
    ${stat('Perlu diperiksa', periksa.length, 'ekor', 'Dilaporkan petugas kandang', 'warn', 'alert')}
    ${stat('Vaksin 7 hari ke depan', vak7, 'ekor', lewat ? `<span style="color:var(--danger);font-weight:600">${lewat} terlewat</span>` : 'Tidak ada yang terlewat', 'info', 'syringe')}
    ${stat('Stok obat menipis', obatTipis.length, 'jenis', obatTipis.map((o) => o.nama.split(' (')[0]).join(', ') || 'Semua aman', 'warn', 'box')}
  </section>
  <div class="split">
    <div style="display:flex;flex-direction:column;gap:16px;min-width:0">
      <section class="card"><div class="card-head" style="padding:20px 20px 8px"><h2>Perlu perhatian</h2><span class="badge neutral plain">${perhatian.length}</span></div>
        ${perhatian.length ? `<div class="table-wrap"><table class="data"><thead><tr><th>Ternak</th><th>Kandang</th><th>Kondisi</th><th>Catatan terakhir</th><th class="r"><span class="sr">Aksi</span></th></tr></thead><tbody>${perhatian.map((t) => { const r = db.kesehatan.filter((x) => x.ternakId === t.id).sort((a, b) => (a.tanggal < b.tanggal ? 1 : -1))[0]; return `<tr><td><a href="#" data-act="openTernak" data-arg="${t.id}" class="strong">${tLabel(t)}</a></td><td class="muted">${kLabel(t.kandangId)}</td><td>${kondisiBadge(t)}</td><td class="muted">${r ? `${esc(r.diagnosa)} · ${fmtShort(r.tanggal)}` : 'Laporan petugas'}</td><td class="r">${can('kesehatan') ? `<button type="button" class="btn btn-ghost btn-sm" data-act="dlg" data-arg="periksa:${t.id}">Periksa</button><button type="button" class="btn btn-secondary btn-sm" data-act="tandaiSehat" data-arg="${t.id}">Tandai sehat</button>` : ''}</td></tr>`; }).join('')}</tbody></table></div>` : '<div class="empty"><strong>Semua ternak sehat</strong></div>'}</section>
      <section class="card"><div class="card-head" style="padding:20px 20px 8px"><h2>Riwayat pemeriksaan</h2></div>
        <div class="table-wrap"><table class="data"><thead><tr><th>Tanggal</th><th>Ternak</th><th>Keluhan</th><th>Diagnosa</th><th>Tindakan</th></tr></thead><tbody>${recs.map((r) => { const t = T(r.ternakId); return `<tr class="${S.fresh === `ks${r.id}` ? 'fresh' : ''}"><td>${fmtShort(r.tanggal)}</td><td><a href="#" data-act="openTernak" data-arg="${t.id}" class="strong">${t.kode}</a></td><td>${esc(r.keluhan)}</td><td>${esc(r.diagnosa)}</td><td class="muted" style="white-space:normal;min-width:220px">${esc(r.tindakan)}</td></tr>`; }).join('')}</tbody></table></div></section>
    </div>
    <div style="display:flex;flex-direction:column;gap:16px">
      <section class="card card-pad" style="display:flex;flex-direction:column;gap:12px" aria-label="Kalender vaksin">
        <div class="card-head"><h2>${BULAN_PANJANG[mm - 1]} ${yy}</h2><div style="display:flex;gap:4px"><button type="button" class="icon-btn" style="width:32px;height:32px" data-act="calNav" data-arg="-1" aria-label="Bulan sebelumnya">${icon('chevL', 16, 2)}</button><button type="button" class="icon-btn" style="width:32px;height:32px" data-act="calNav" data-arg="1" aria-label="Bulan berikutnya">${icon('chevR', 16, 2)}</button></div></div>
        <div class="cal">${cal}</div>
        <div class="legend"><span><i style="background:var(--warn-bar)"></i>terjadwal</span><span><i style="background:var(--primary)"></i>selesai</span><span><i style="background:var(--danger)"></i>terlewat</span></div>
        <div style="border-top:1px solid var(--divider);padding-top:10px"><b style="font-size:13px">${fmtLong(parseIso(S.ui.calDay))}</b><ul class="mini-list">${dayList || '<li class="muted">Tidak ada jadwal</li>'}</ul></div>
      </section>
      <section class="card card-pad" style="display:flex;flex-direction:column;gap:10px"><h2 style="font-size:16px">Stok obat dan vaksin</h2>
        ${db.obat.map((o) => `<div style="display:flex;flex-direction:column;gap:6px"><div style="display:flex;justify-content:space-between;font-size:13px"><span>${esc(o.nama)}</span><span class="num" style="${o.stok < o.min ? 'color:var(--warn);font-weight:600' : 'color:var(--text-2)'}">${num(o.stok)} ${o.satuan}</span></div>${meter(o.stok / (o.min * 3) * 100, o.stok < o.min ? 'warn' : '')}</div>`).join('')}</section>
    </div>
  </div>`;
}

function pageReproduksi() {
  const db = S.db;
  const f = S.ui.repro;
  const bunting = db.kawin.filter((k) => k.status === 'Bunting');
  const menunggu = db.kawin.filter((k) => k.status === 'Menunggu');
  const lahir7 = bunting.filter((k) => daysBetween(TODAY, parseIso(k.perkiraanLahir)) <= 7).length;
  const gagal = db.kawin.filter((k) => k.status === 'Gagal').length;
  let list = db.kawin.filter((k) => (f === 'aktif' ? ['Bunting', 'Menunggu'].includes(k.status) : f === 'riwayat' ? ['Gagal', 'Lahir'].includes(k.status) : k.status === f));
  list = list.sort((a, b) => (a.perkiraanLahir < b.perkiraanLahir ? -1 : 1));
  const rows = list.map((k) => {
    const t = T(k.indukId);
    const sp = spOf(t);
    const el = daysBetween(parseIso(k.tanggal), TODAY);
    const sisa = daysBetween(TODAY, parseIso(k.perkiraanLahir));
    let aksi = '';
    if (can('kesehatan')) {
      if (k.status === 'Menunggu') aksi = `<button type="button" class="btn btn-secondary btn-sm" data-act="kawinStatus" data-arg="${k.id}:Bunting">Bunting</button> <button type="button" class="btn btn-ghost btn-sm" data-act="kawinStatus" data-arg="${k.id}:Gagal">Gagal</button>`;
      else if (k.status === 'Bunting') aksi = `<button type="button" class="btn ${sisa <= 7 ? 'btn-primary' : 'btn-secondary'} btn-sm" data-act="dlg" data-arg="lahir:${k.id}">Catat kelahiran</button>`;
    }
    if (k.status === 'Lahir') aksi = `<span class="muted">${k.lahir.jumlah} anak · ${fmtShort(k.lahir.tanggal)}</span>`;
    return `<tr class="${S.fresh === `kw${k.id}` ? 'fresh' : ''}"><td><a href="#" data-act="openTernak" data-arg="${t.id}" class="strong">${tLabel(t)}</a></td><td class="muted">${sp.nama} ${rasOf(t).nama}</td><td>${fmtDate(k.tanggal)}</td><td>${k.metode === 'IB' ? `IB · <span class="muted">${esc(k.kodeStraw)}</span>` : `Alami · <span class="muted">${k.pejantanId ? T(k.pejantanId).kode : '—'}</span>`}</td><td>${kawinBadge(k)}</td><td class="r">${fmtDate(k.perkiraanLahir)}</td>
      <td style="min-width:170px">${k.status === 'Bunting' ? `<div style="display:flex;flex-direction:column;gap:4px">${meter(el / sp.bunting * 100, sisa <= 7 ? 'warn' : '')}<small class="muted num">${sisa >= 0 ? `H-${sisa}` : `lewat ${-sisa} hari`} · ${el} dari ${sp.bunting} hari</small></div>` : k.status === 'Menunggu' ? `<small class="muted">Cek bunting ${relDay(iso(addDays(parseIso(k.tanggal), 60)))}</small>` : ''}</td><td class="r">${aksi}</td></tr>`;
  }).join('');
  return `${pageHead('Reproduksi', 'Perkawinan, kebuntingan, dan kelahiran', can('kesehatan') ? `<button type="button" class="btn btn-primary" data-act="dlg" data-arg="kawin">${icon('plus', 16, 2.25)}Catat perkawinan</button>` : '')}
  <section class="grid-stats">
    ${stat('Induk bunting', bunting.length, 'ekor', `<span style="color:var(--warn);font-weight:600">${lahir7}</span> diperkirakan lahir ≤ 7 hari`, 'warn', 'repro')}
    ${stat('Menunggu cek bunting', menunggu.length, 'ekor', 'Dicek ± 60 hari setelah kawin', 'info', 'calendar')}
    ${stat('Lahir tercatat', db.kawin.filter((k) => k.status === 'Lahir').length, 'kali', 'Anak langsung masuk data ternak', 'ok', 'tag')}
    ${stat('Gagal bunting', gagal, 'kali', 'Bisa dikawinkan ulang', 'danger', 'alert')}
  </section>
  <section class="card">
    <div class="toolbar">${segmented('Filter status', [['aktif', 'Sedang berjalan'], ['Bunting', 'Bunting'], ['Menunggu', 'Menunggu cek'], ['riwayat', 'Riwayat']], f, 'reproF')}</div>
    ${list.length ? `<div class="table-wrap"><table class="data"><thead><tr><th>Induk</th><th>Spesies</th><th>Tanggal kawin</th><th>Metode</th><th>Status</th><th class="r">Perkiraan lahir</th><th>Kebuntingan</th><th class="r"><span class="sr">Aksi</span></th></tr></thead><tbody>${rows}</tbody></table></div>` : '<div class="empty"><strong>Tidak ada data</strong></div>'}
  </section>`;
}

function pageMutasi() {
  const db = S.db;
  const f = S.ui.mutasi;
  const list = db.mutasi.filter((m) => f === 'Semua' || m.jenis === f).sort((a, b) => (a.tanggal < b.tanggal ? 1 : a.tanggal > b.tanggal ? -1 : b.id - a.id));
  const ym = iso(TODAY).slice(0, 7);
  const jualBulan = db.mutasi.filter((m) => m.jenis === 'Jual' && m.tanggal.slice(0, 7) === ym);
  const pindah30 = db.mutasi.filter((m) => m.jenis === 'Pindah' && daysBetween(parseIso(m.tanggal), TODAY) <= 30).length;
  const mati90 = db.mutasi.filter((m) => m.jenis === 'Mati' && daysBetween(parseIso(m.tanggal), TODAY) <= 90).length;
  const badge = { Pindah: 'info', Jual: 'ok', Mati: 'neutral' };
  const rows = list.map((m) => {
    const t = T(m.ternakId);
    const det = m.jenis === 'Pindah' ? `${K(m.kandangAsalId)?.kode || '—'} ${icon('arrowR', 12, 2)} ${K(m.kandangTujuanId)?.kode || '—'}` : m.jenis === 'Jual' ? `<b>${rp(m.hargaJual)}</b> · ${esc(m.pembeli)}` : esc(m.penyebab);
    return `<tr class="${S.fresh === `mt${m.id}` ? 'fresh' : ''}"><td>${fmtDate(m.tanggal)}</td><td><a href="#" data-act="openTernak" data-arg="${t.id}" class="strong">${tLabel(t)}</a> <span class="muted">· ${spOf(t).nama}</span></td><td><span class="badge ${badge[m.jenis]}">${m.jenis}</span></td><td><span style="display:inline-flex;align-items:center;gap:6px">${det}</span></td><td class="muted">${esc(m.oleh)}</td></tr>`;
  }).join('');
  return `${pageHead('Mutasi', 'Pindah kandang, penjualan, dan kematian', can('mutasi') ? `<button type="button" class="btn btn-primary" data-act="dlg" data-arg="mutasi">${icon('plus', 16, 2.25)}Mutasi baru</button>` : '')}
  <section class="grid-stats">
    ${stat('Terjual bulan ini', jualBulan.length, 'ekor', rp(jualBulan.reduce((a, m) => a + m.hargaJual, 0)), 'ok', 'wallet')}
    ${stat('Pindah kandang', pindah30, 'kali', '30 hari terakhir', 'info', 'swap')}
    ${stat('Kematian', mati90, 'ekor', '90 hari terakhir', 'danger', 'alert')}
  </section>
  <section class="card"><div class="toolbar">${segmented('Jenis mutasi', [['Semua', 'Semua'], ['Pindah', 'Pindah'], ['Jual', 'Jual'], ['Mati', 'Mati']], f, 'mutasiF')}</div>
    ${list.length ? `<div class="table-wrap"><table class="data"><thead><tr><th>Tanggal</th><th>Ternak</th><th>Jenis</th><th>Detail</th><th>Dicatat</th></tr></thead><tbody>${rows}</tbody></table></div>` : '<div class="empty"><strong>Belum ada mutasi</strong></div>'}</section>`;
}

function pageKeuangan() {
  const db = S.db;
  const range = Number(S.ui.keuRange);
  const start = iso(new Date(TODAY.getFullYear(), TODAY.getMonth() - (range - 1), 1));
  const inRange = db.keuangan.filter((x) => x.tanggal >= start && x.tanggal <= iso(TODAY));
  const masuk = inRange.filter((x) => x.jenis === 'Masuk').reduce((a, x) => a + x.jumlah, 0);
  const keluar = inRange.filter((x) => x.jenis === 'Keluar').reduce((a, x) => a + x.jumlah, 0);
  const laba = masuk - keluar;
  const months = [];
  for (let i = 5; i >= 0; i--) {
    const d = new Date(TODAY.getFullYear(), TODAY.getMonth() - i, 1);
    const key = iso(d).slice(0, 7);
    const rows = db.keuangan.filter((x) => x.tanggal.slice(0, 7) === key);
    months.push({ label: BULAN[d.getMonth()], masuk: rows.filter((x) => x.jenis === 'Masuk').reduce((a, x) => a + x.jumlah, 0), keluar: rows.filter((x) => x.jenis === 'Keluar').reduce((a, x) => a + x.jumlah, 0) });
  }
  const kat = new Map();
  inRange.filter((x) => x.jenis === 'Keluar').forEach((x) => kat.set(x.kategori, (kat.get(x.kategori) || 0) + x.jumlah));
  const katList = [...kat.entries()].sort((a, b) => b[1] - a[1]);
  const jf = S.ui.keuJenis;
  const list = inRange.filter((x) => jf === 'Semua' || x.jenis === jf).sort((a, b) => (a.tanggal < b.tanggal ? 1 : a.tanggal > b.tanggal ? -1 : b.id - a.id));
  const sumberLabel = { PakanMasuk: 'Otomatis · pakan', Mutasi: 'Otomatis · penjualan', Manual: 'Manual' };
  return `${pageHead('Keuangan', 'Pemasukan, pengeluaran, dan laba-rugi', `${segmented('Periode', [['1', 'Bulan ini'], ['3', '3 bulan'], ['6', '6 bulan']], S.ui.keuRange, 'keuRange')}<button type="button" class="btn btn-primary" data-act="dlg" data-arg="transaksi">${icon('plus', 16, 2.25)}Catat transaksi</button>`)}
  <section class="grid-stats">
    ${stat('Pemasukan', rpShort(masuk), '', `${inRange.filter((x) => x.jenis === 'Masuk').length} transaksi`, 'ok', 'download')}
    ${stat('Pengeluaran', rpShort(keluar), '', `${inRange.filter((x) => x.jenis === 'Keluar').length} transaksi`, 'warn', 'upload')}
    ${stat('Laba bersih', `<span style="color:${laba >= 0 ? 'var(--primary)' : 'var(--danger)'}">${rpShort(laba)}</span>`, '', masuk ? `Margin ${Math.round(laba / masuk * 100)}%` : '—', laba >= 0 ? 'ok' : 'danger', 'trend')}
  </section>
  <div class="row-2">
    <section class="card card-pad wide" style="display:flex;flex-direction:column;gap:12px"><div class="card-head"><div><h2>Arus kas 6 bulan</h2><p>Pemasukan dan pengeluaran per bulan</p></div><div class="legend"><span><i style="background:var(--primary)"></i>Pemasukan</span><span><i style="background:var(--warn-bar)"></i>Pengeluaran</span></div></div>
      ${groupedBars(months, 'Pemasukan dan pengeluaran 6 bulan terakhir')}</section>
    <section class="card card-pad narrow" style="display:flex;flex-direction:column;gap:12px"><h2 style="font-size:16px">Pengeluaran per kategori</h2>
      ${katList.map(([n, v]) => `<div style="display:flex;flex-direction:column;gap:6px"><div style="display:flex;justify-content:space-between;font-size:13px"><span>${esc(n)}</span><span class="num strong" style="font-weight:600">${rpShort(v)}</span></div>${meter(v / keluar * 100, 'warn')}</div>`).join('') || '<div class="empty">Tidak ada pengeluaran</div>'}</section>
  </div>
  <section class="card"><div class="toolbar">${segmented('Jenis', [['Semua', 'Semua'], ['Masuk', 'Pemasukan'], ['Keluar', 'Pengeluaran']], jf, 'keuJenis')}<span style="flex:1"></span><button type="button" class="btn btn-secondary btn-sm" data-act="snackExport" data-arg="keuangan">${icon('excel', 14)}Ekspor Excel</button></div>
    <div class="table-wrap" style="max-height:420px"><table class="data"><thead><tr><th>Tanggal</th><th>Keterangan</th><th>Kategori</th><th>Sumber</th><th class="r">Jumlah</th></tr></thead><tbody>${list.map((x) => `<tr class="${S.fresh === `fn${x.id}` ? 'fresh' : ''}"><td>${fmtDate(x.tanggal)}</td><td>${esc(x.ket)}</td><td class="muted">${esc(x.kategori)}</td><td><span class="badge ${x.sumber === 'Manual' ? 'neutral' : 'info'} plain">${sumberLabel[x.sumber]}</span></td><td class="r strong" style="color:${x.jenis === 'Masuk' ? 'var(--primary)' : 'var(--text)'}">${x.jenis === 'Masuk' ? '+' : '−'}${rp(x.jumlah)}</td></tr>`).join('')}</tbody></table></div></section>`;
}

function laporanData() {
  const db = S.db;
  const { jenis, dari, sampai } = S.ui.laporan;
  const inP = (d) => d >= dari && d <= sampai;
  if (jenis === 'ternak') {
    return { title: 'Laporan data ternak', head: ['Kandang', 'Sapi', 'Kambing', 'Domba', 'Total'], rows: db.kandang.map((k) => { const l = aktif().filter((t) => t.kandangId === k.id); const c = [1, 2, 3].map((s) => l.filter((t) => t.spesiesId === s).length); return [`${k.kode} · ${k.nama}`, ...c, l.length]; }), foot: ['Total', ...[1, 2, 3].map((s) => aktif().filter((t) => t.spesiesId === s).length), aktif().length] };
  }
  if (jenis === 'kesehatan') {
    const r = db.kesehatan.filter((x) => inP(x.tanggal)).sort((a, b) => (a.tanggal < b.tanggal ? -1 : 1));
    return { title: 'Laporan kesehatan', head: ['Tanggal', 'Ternak', 'Diagnosa', 'Tindakan'], rows: r.slice(0, 14).map((x) => [fmtShort(x.tanggal), T(x.ternakId).kode, x.diagnosa, x.tindakan]), foot: [`${r.length} pemeriksaan`, '', '', ''] };
  }
  if (jenis === 'keuangan') {
    const m = new Map();
    db.keuangan.filter((x) => inP(x.tanggal)).forEach((x) => { const k = x.tanggal.slice(0, 7); if (!m.has(k)) m.set(k, [0, 0]); m.get(k)[x.jenis === 'Masuk' ? 0 : 1] += x.jumlah; });
    const rows = [...m.entries()].sort().map(([k, [a, b]]) => { const [y, mo] = k.split('-'); return [`${BULAN_PANJANG[Number(mo) - 1]} ${y}`, rp(a), rp(b), rp(a - b)]; });
    const tot = [...m.values()].reduce((s, [a, b]) => [s[0] + a, s[1] + b], [0, 0]);
    return { title: 'Laporan laba-rugi', head: ['Bulan', 'Pemasukan', 'Pengeluaran', 'Laba'], rows, foot: ['Total', rp(tot[0]), rp(tot[1]), rp(tot[0] - tot[1])] };
  }
  const rows = db.pakan.map((p) => { const masuk = db.pakanMasuk.filter((x) => x.pakanId === p.id && inP(x.tanggal)).reduce((a, x) => a + x.jumlah, 0); return [p.nama, `${num(masuk)} ${p.satuan}`, `${num(p.stok)} ${p.satuan}`, p.stok < p.min ? 'Menipis' : 'Aman']; });
  return { title: 'Laporan stok pakan', head: ['Pakan', 'Masuk', 'Stok akhir', 'Status'], rows, foot: null };
}

function pageLaporan() {
  const L = S.ui.laporan;
  const types = [['ternak', 'Data ternak', 'Jumlah ternak per kandang dan spesies', 'tag'], ['kesehatan', 'Kesehatan', 'Pemeriksaan dan pengobatan per periode', 'heart'], ['keuangan', 'Keuangan', 'Laba-rugi per bulan', 'wallet'], ['pakan', 'Pakan', 'Pakan masuk dan stok akhir', 'box']];
  const d = laporanData();
  return `${pageHead('Laporan', 'Pratinjau lalu simpan sebagai PDF atau Excel')}
  <div class="split split-l">
    <section class="card card-pad" style="display:flex;flex-direction:column;gap:16px">
      <div class="field"><span class="label">Jenis laporan</span><div style="display:flex;flex-direction:column;gap:8px">${types.map(([v, l, s, ic]) => `<button type="button" class="pen" style="flex-direction:row;align-items:center;padding:12px" aria-pressed="${L.jenis === v}" data-act="lapJenis" data-arg="${v}"><span class="ico ${L.jenis === v ? 'ok' : 'info'}" style="width:34px;height:34px;border-radius:6px;display:grid;place-items:center">${icon(ic, 16, 2)}</span><span><b style="display:block;font-size:14px">${l}</b><small class="muted" style="font-size:12px">${s}</small></span></button>`).join('')}</div></div>
      <div class="form-grid" style="grid-template-columns:1fr 1fr"><div class="field"><label for="lp-dari">Dari</label><input class="input" type="date" id="lp-dari" value="${L.dari}" data-bind="ui.laporan.dari"></div><div class="field"><label for="lp-sampai">Sampai</label><input class="input" type="date" id="lp-sampai" value="${L.sampai}" data-bind="ui.laporan.sampai"></div></div>
      <div style="display:flex;gap:10px"><button type="button" class="btn btn-primary" style="flex:1" data-act="snackExport" data-arg="pdf">${icon('file', 16)}Simpan PDF</button><button type="button" class="btn btn-secondary" style="flex:1" data-act="snackExport" data-arg="xlsx">${icon('excel', 16)}Ekspor Excel</button></div>
    </section>
    <section class="report-preview" aria-label="Pratinjau laporan"><div class="paper">
      <div style="display:flex;justify-content:space-between;align-items:flex-start;gap:12px"><div><h3>${d.title}</h3><div style="color:#5C6650">${esc(S.db.peternakan)} · ${fmtDate(L.dari)} – ${fmtDate(L.sampai)}</div></div><div style="display:flex;align-items:center;gap:6px;font-weight:700;color:#2E7A15">${icon('leaf', 14, 2)}TernakKu</div></div>
      <table><thead><tr>${d.head.map((h, i) => `<th class="${i ? 'r' : ''}">${h}</th>`).join('')}</tr></thead><tbody>${d.rows.map((r) => `<tr>${r.map((c, i) => `<td class="${i ? 'r' : ''}">${esc(c)}</td>`).join('')}</tr>`).join('') || `<tr><td colspan="${d.head.length}">Tidak ada data pada periode ini</td></tr>`}</tbody>${d.foot ? `<tfoot><tr>${d.foot.map((c, i) => `<th class="${i ? 'r' : ''}" style="color:#1F2A17">${esc(c)}</th>`).join('')}</tr></tfoot>` : ''}</table>
      <div style="color:#5C6650;display:flex;justify-content:space-between"><span>Dicetak ${fmtDate(iso(TODAY))} oleh ${esc(S.user.nama)}</span><span>Halaman 1</span></div>
    </div></section>
  </div>`;
}

function pageMaster() {
  const db = S.db;
  const tab = S.ui.masterTab;
  const edit = (ent, id) => (can('master') ? `<button type="button" class="btn btn-ghost btn-sm" data-act="dlg" data-arg="master:${ent}:${id}">${icon('edit', 14)}Ubah</button>` : '');
  let body;
  if (tab === 'spesies') {
    body = `<div class="row-2"><section class="card narrow"><div class="card-head" style="padding:16px 20px 8px"><h2>Spesies</h2>${can('master') ? `<button type="button" class="btn btn-secondary btn-sm" data-act="dlg" data-arg="master:spesies">${icon('plus', 14, 2)}Tambah</button>` : ''}</div><div class="table-wrap"><table class="data"><thead><tr><th>Nama</th><th>Kode</th><th class="r">Masa bunting</th><th class="r"></th></tr></thead><tbody>${db.spesies.map((s) => `<tr><td class="strong">${esc(s.nama)}</td><td class="muted">${s.prefix}</td><td class="r">${s.bunting} hari</td><td class="r">${edit('spesies', s.id)}</td></tr>`).join('')}</tbody></table></div></section>
      <section class="card wide"><div class="card-head" style="padding:16px 20px 8px"><h2>Ras</h2>${can('master') ? `<button type="button" class="btn btn-secondary btn-sm" data-act="dlg" data-arg="master:ras">${icon('plus', 14, 2)}Tambah</button>` : ''}</div><div class="table-wrap"><table class="data"><thead><tr><th>Ras</th><th>Spesies</th><th class="r">Jumlah aktif</th><th class="r"></th></tr></thead><tbody>${db.ras.map((r) => `<tr><td class="strong">${esc(r.nama)}</td><td class="muted">${S.ix.spesies.get(r.spesiesId).nama}</td><td class="r">${aktif().filter((t) => t.rasId === r.id).length}</td><td class="r">${edit('ras', r.id)}</td></tr>`).join('')}</tbody></table></div></section></div>`;
  } else if (tab === 'kandang') {
    body = `<section class="card"><div class="card-head" style="padding:16px 20px 8px"><h2>Kandang</h2>${can('master') ? `<button type="button" class="btn btn-secondary btn-sm" data-act="dlg" data-arg="master:kandang">${icon('plus', 14, 2)}Tambah</button>` : ''}</div><div class="table-wrap"><table class="data"><thead><tr><th>Kode</th><th>Nama</th><th>Jenis</th><th>Spesies</th><th class="r">Kapasitas</th><th class="r">Terisi</th><th class="r"></th></tr></thead><tbody>${db.kandang.map((k) => `<tr><td class="strong">${k.kode}</td><td>${esc(k.nama)}</td><td>${k.jenis}</td><td class="muted">${k.spesiesId ? S.ix.spesies.get(k.spesiesId).nama : 'Campuran'}</td><td class="r">${k.kapasitas}</td><td class="r">${isiKandang(k.id)}</td><td class="r">${edit('kandang', k.id)}</td></tr>`).join('')}</tbody></table></div></section>`;
  } else if (tab === 'pakan') {
    body = `<section class="card"><div class="card-head" style="padding:16px 20px 8px"><h2>Daftar pakan</h2>${can('master') ? `<button type="button" class="btn btn-secondary btn-sm" data-act="dlg" data-arg="master:pakan">${icon('plus', 14, 2)}Tambah</button>` : ''}</div><div class="table-wrap"><table class="data"><thead><tr><th>Nama</th><th>Satuan</th><th class="r">Stok minimum</th><th class="r">Harga per satuan</th><th class="r">Pemakaian / hari</th><th class="r"></th></tr></thead><tbody>${db.pakan.map((p) => `<tr><td class="strong">${esc(p.nama)}</td><td>${p.satuan}</td><td class="r">${num(p.min)}</td><td class="r">${rp(p.harga)}</td><td class="r">${num(p.pakai, p.pakai < 1 ? 1 : 0)}</td><td class="r">${edit('pakan', p.id)}</td></tr>`).join('')}</tbody></table></div></section>`;
  } else {
    body = `<section class="card"><div class="card-head" style="padding:16px 20px 8px"><h2>Obat dan vaksin</h2>${can('master') ? `<button type="button" class="btn btn-secondary btn-sm" data-act="dlg" data-arg="master:obat">${icon('plus', 14, 2)}Tambah</button>` : ''}</div><div class="table-wrap"><table class="data"><thead><tr><th>Nama</th><th>Jenis</th><th>Satuan</th><th class="r">Stok</th><th class="r">Minimum</th><th class="r"></th></tr></thead><tbody>${db.obat.map((o) => `<tr><td class="strong">${esc(o.nama)}</td><td><span class="badge ${o.jenis === 'Vaksin' ? 'info' : o.jenis === 'Obat' ? 'warn' : 'ok'} plain">${o.jenis}</span></td><td>${o.satuan}</td><td class="r" style="${o.stok < o.min ? 'color:var(--warn);font-weight:600' : ''}">${num(o.stok)}</td><td class="r">${num(o.min)}</td><td class="r">${edit('obat', o.id)}</td></tr>`).join('')}</tbody></table></div></section>`;
  }
  return `${pageHead('Data master', 'Referensi yang dipakai di seluruh aplikasi')}
  <div class="tabs" role="tablist">${[['spesies', 'Spesies dan ras'], ['kandang', 'Kandang'], ['pakan', 'Pakan'], ['obat', 'Obat dan vaksin']].map(([v, l]) => `<button type="button" role="tab" aria-selected="${tab === v}" data-act="masterTab" data-arg="${v}">${l}</button>`).join('')}</div>${body}`;
}

function pagePengguna() {
  const db = S.db;
  const peranBadge = { Admin: 'ok', Petugas: 'info', Mantri: 'warn' };
  const matrix = [['Data ternak', 'Ubah', 'Lihat', 'Lihat'], ['Penimbangan dan pakan', 'Ya', 'Ya', '—'], ['Kesehatan dan reproduksi', 'Ya', '—', 'Ya'], ['Mutasi dan keuangan', 'Ya', '—', '—'], ['Laporan dan backup', 'Ya', '—', '—'], ['Kelola pengguna', 'Ya', '—', '—']];
  return `${pageHead('Pengguna', 'Akun dan hak akses di database akun', `<button type="button" class="btn btn-primary" data-act="dlg" data-arg="pengguna">${icon('plus', 16, 2.25)}Tambah pengguna</button>`)}
  <section class="card"><div class="table-wrap"><table class="data"><thead><tr><th>Nama</th><th>Username</th><th>Peran</th><th>Status</th><th>Login terakhir</th><th class="r"><span class="sr">Aksi</span></th></tr></thead><tbody>${db.pengguna.map((u) => `<tr class="${S.fresh === `us${u.id}` ? 'fresh' : ''}"><td class="strong">${esc(u.nama)}</td><td class="muted">${esc(u.username)}</td><td><span class="badge ${peranBadge[u.peran]} plain">${u.peran}</span></td><td>${u.aktif ? '<span class="badge ok">Aktif</span>' : '<span class="badge neutral">Nonaktif</span>'}</td><td class="muted">${relDay(u.terakhir)}</td><td class="r">${u.username === S.user.username ? '<span class="muted">Anda</span>' : `<button type="button" class="btn btn-ghost btn-sm" data-act="resetPw" data-arg="${u.id}">${icon('key', 14)}Reset password</button><button type="button" class="btn btn-secondary btn-sm" data-act="toggleUser" data-arg="${u.id}">${u.aktif ? 'Nonaktifkan' : 'Aktifkan'}</button>`}</td></tr>`).join('')}</tbody></table></div></section>
  <section class="card"><div class="card-head" style="padding:16px 20px 8px"><h2>Hak akses per peran</h2></div><div class="table-wrap"><table class="data"><thead><tr><th>Fitur</th><th>Admin</th><th>Petugas kandang</th><th>Mantri</th></tr></thead><tbody>${matrix.map((r) => `<tr>${r.map((c, i) => `<td class="${i ? '' : 'strong'}" style="${c === '—' ? 'color:var(--text-2)' : ''}">${c}</td>`).join('')}</tr>`).join('')}</tbody></table></div></section>`;
}

function pagePengaturan() {
  const ses = store.get(KEY_SESSION);
  const path = `%LOCALAPPDATA%\\TernakKu\\${S.db.file}`;
  return `${pageHead('Pengaturan', 'Tampilan, cara masuk, dan database')}
  <section class="card card-pad"><h2 style="font-size:16px;margin-bottom:4px">Tampilan</h2>
    <div class="setting"><div><b>Tema</b><small>Terang atau gelap</small></div>${segmented('Tema', [['light', 'Terang'], ['dark', 'Gelap']], S.settings.theme, 'theme')}</div>
    <div class="setting"><div><b>Kepadatan tabel</b><small>Tinggi baris ${S.settings.density === 'compact' ? '36' : '44'} px</small></div>${segmented('Kepadatan', [['comfortable', 'Nyaman'], ['compact', 'Padat']], S.settings.density, 'density')}</div>
  </section>
  <section class="card card-pad"><h2 style="font-size:16px;margin-bottom:4px">Masuk</h2>
    <div class="setting"><div><b>Ingat saya</b><small>${ses ? `Aplikasi langsung terbuka sebagai ${esc(ses.mode === 'tamu' ? 'Tamu' : ses.nama)} (${ses.mode === 'tamu' ? 'tamu.db' : 'ternakku.db'}) tanpa login` : 'Login atau pilihan tamu diminta setiap kali aplikasi dibuka'}</small></div>
      ${ses ? '<button type="button" class="btn btn-secondary" data-act="forget">Lupakan</button>' : '<button type="button" class="btn btn-secondary" data-act="rememberNow">Ingat pilihan ini</button>'}</div>
    ${S.mode === 'akun' ? `<div class="setting"><div><b>Password</b><small>Akun ${esc(S.user.username)}</small></div><button type="button" class="btn btn-secondary" data-act="dlg" data-arg="gantiPw">${icon('key', 16)}Ganti password</button></div>` : ''}
  </section>
  <section class="card card-pad"><h2 style="font-size:16px;margin-bottom:4px">Database</h2>
    <div class="setting"><div><b>Database aktif</b><small style="font-family:Consolas,monospace">${esc(path)}</small></div><span class="badge ${S.mode === 'tamu' ? 'warn' : 'ok'}">${S.mode === 'tamu' ? 'Database tamu' : 'Database akun'}</span></div>
    <div class="setting"><div><b>Pemisahan data</b><small>Akun memakai ternakku.db, tamu memakai tamu.db. Keduanya tidak saling membaca.</small></div></div>
    ${S.user.peran === 'Admin' || S.mode === 'tamu' ? `<div class="setting"><div><b>Backup dan pulihkan</b><small>Salin file database ke folder pilihan</small></div><div style="display:flex;gap:8px"><button type="button" class="btn btn-secondary" data-act="backup">${icon('download', 16)}Backup</button><button type="button" class="btn btn-secondary" data-act="confirm" data-arg="restore">${icon('restore', 16)}Pulihkan</button></div></div>` : ''}
    ${S.mode === 'tamu' ? `<div class="setting"><div><b>Kosongkan database tamu</b><small>Kembalikan tamu.db ke data contoh awal</small></div><button type="button" class="btn btn-secondary" style="color:var(--danger)" data-act="confirm" data-arg="resetTamu">Kosongkan</button></div>` : ''}
  </section>
  <p class="muted" style="font-size:12px">TernakKu v1.0 · berjalan offline</p>`;
}

/* ---------------- Dialogs ---------------- */
function fld(name, label, control, { err, hint, span } = {}) {
  return `<div class="field"${span ? ' style="grid-column:1/-1"' : ''}><label for="d-${name}">${label}</label>${control}${err ? `<span class="error">${esc(err)}</span>` : hint ? `<span class="hint">${hint}</span>` : ''}</div>`;
}
function inp(name, v, { type = 'text', ph = '', attrs = '' } = {}) {
  const e = S.dialog.errors?.[name];
  return `<input class="input${e ? ' invalid' : ''}" id="d-${name}" name="${name}" type="${type}" value="${esc(v[name] ?? '')}" placeholder="${esc(ph)}" ${attrs}>`;
}
function sel(name, list, v, live = false) {
  return `<select class="select" id="d-${name}" name="${name}" ${live ? 'data-live' : ''}>${options(list, v[name])}</select>`;
}
function dialogShell(title, body, okLabel, { wide = false, danger = false } = {}) {
  return `<div class="scrim${enter('dialog')}" data-act="closeDialog"><form class="dialog${wide ? ' wide' : ''}" data-form="dialog" data-act="noop" role="dialog" aria-modal="true" aria-labelledby="dlg-title" novalidate>
    <header><h2 id="dlg-title">${title}</h2><button type="button" class="icon-btn" style="border:0;width:36px;height:36px" data-act="closeDialog" aria-label="Tutup">${icon('x', 18)}</button></header>
    <div class="body">${body}</div>
    <footer><button type="button" class="btn btn-secondary" data-act="closeDialog">Batal</button><button type="submit" class="btn ${danger ? 'btn-danger' : 'btn-primary'}">${okLabel}</button></footer></form></div>`;
}

const kandangOptions = (exclude) => S.db.kandang.map((k) => { const isi = isiKandang(k.id); return [k.id, `${k.kode} · ${k.nama} (${isi}/${k.kapasitas}${isi >= k.kapasitas ? ', penuh' : ''})`, isi >= k.kapasitas || k.id === exclude]; });
const ternakOptions = (filter = () => true) => aktif().filter(filter).sort((a, b) => a.kode.localeCompare(b.kode)).map((t) => [t.id, `${tLabel(t)} — ${t.kandangId ? K(t.kandangId).kode : ''}`]);

const MASTER = {
  spesies: { title: 'spesies', arr: 'spesies', fields: [['nama', 'Nama', 'text'], ['prefix', 'Kode ear tag (2 huruf)', 'text'], ['bunting', 'Masa bunting (hari)', 'number']] },
  ras: { title: 'ras', arr: 'ras', fields: [['spesiesId', 'Spesies', 'spesies'], ['nama', 'Nama ras', 'text']] },
  kandang: { title: 'kandang', arr: 'kandang', fields: [['kode', 'Kode', 'text'], ['nama', 'Nama', 'text'], ['jenis', 'Jenis', ['Penggemukan', 'Indukan', 'Karantina']], ['spesiesId', 'Spesies', 'spesies0'], ['kapasitas', 'Kapasitas (ekor)', 'number']] },
  pakan: { title: 'pakan', arr: 'pakan', fields: [['nama', 'Nama', 'text'], ['satuan', 'Satuan', ['kg', 'karung', 'buah']], ['min', 'Stok minimum', 'number'], ['harga', 'Harga per satuan (Rp)', 'number'], ['pakai', 'Pemakaian per hari', 'number']] },
  obat: { title: 'obat atau vaksin', arr: 'obat', fields: [['nama', 'Nama', 'text'], ['jenis', 'Jenis', ['Obat', 'Vaksin', 'Vitamin']], ['satuan', 'Satuan', 'text'], ['min', 'Stok minimum', 'number']] }
};

function viewDialog() {
  const d = S.dialog;
  if (!d) return '';
  const v = d.values;
  const E = d.errors || {};
  switch (d.type) {
    case 'tambahTernak': {
      const spId = Number(v.spesiesId);
      const body = `<div class="form-grid">
        ${fld('spesiesId', 'Spesies', sel('spesiesId', S.db.spesies.map((s) => [s.id, s.nama]), v, true))}
        ${fld('rasId', 'Ras', sel('rasId', S.db.ras.filter((r) => r.spesiesId === spId).map((r) => [r.id, r.nama]), v))}
        ${fld('kode', 'Kode ear tag', inp('kode', v), { err: E.kode, hint: 'Terisi otomatis, boleh diubah' })}
        ${fld('nama', 'Nama (opsional)', inp('nama', v, { ph: 'mis. Bima' }))}
        ${fld('jk', 'Jenis kelamin', sel('jk', [['Jantan', 'Jantan'], ['Betina', 'Betina']], v))}
        ${fld('lahir', 'Tanggal lahir', inp('lahir', v, { type: 'date', attrs: `max="${iso(TODAY)}"` }), { err: E.lahir })}
        ${fld('kandangId', 'Kandang', sel('kandangId', kandangOptions(), v), { err: E.kandangId })}
        ${fld('berat', 'Berat awal (kg)', inp('berat', v, { type: 'number', attrs: 'step="0.1" min="0"' }), { err: E.berat })}
        ${fld('asal', 'Asal usul', sel('asal', [['Beli', 'Dibeli'], ['Lahir', 'Lahir di peternakan']], v, true))}
        ${v.asal === 'Beli' ? fld('hargaBeli', 'Harga beli (Rp)', inp('hargaBeli', v, { type: 'number', ph: '0' })) : fld('indukId', 'Induk', sel('indukId', [['', '— tidak diketahui —'], ...ternakOptions((t) => t.jk === 'Betina' && t.spesiesId === spId)], v))}
      </div>`;
      return dialogShell('Tambah ternak', body, 'Simpan', { wide: true });
    }
    case 'timbang': {
      const t = T(v.ternakId), w = lastWeight(t.id);
      return dialogShell(`Catat penimbangan · ${t.kode}`, `<div class="form-grid">${fld('tanggal', 'Tanggal', inp('tanggal', v, { type: 'date', attrs: `max="${iso(TODAY)}"` }))}${fld('berat', 'Berat (kg)', inp('berat', v, { type: 'number', attrs: 'step="0.1" min="0"' }), { err: E.berat, hint: w ? `Terakhir ${num(w.berat, 1)} kg · ${fmtShort(w.tanggal)}` : '' })}${fld('lingkarDada', 'Lingkar dada (cm)', inp('lingkarDada', v, { type: 'number' }))}</div>`, 'Simpan');
    }
    case 'mutasi': {
      const t = v.ternakId ? T(v.ternakId) : null;
      let extra = '';
      if (v.jenis === 'Pindah') extra = fld('kandangTujuanId', 'Kandang tujuan', sel('kandangTujuanId', [['', 'Pilih kandang'], ...kandangOptions(t?.kandangId)], v), { err: E.kandangTujuanId });
      else if (v.jenis === 'Jual') extra = `${fld('hargaJual', 'Harga jual (Rp)', inp('hargaJual', v, { type: 'number' }), { err: E.hargaJual })}${fld('pembeli', 'Pembeli', inp('pembeli', v), { err: E.pembeli })}`;
      else extra = fld('penyebab', 'Penyebab kematian', inp('penyebab', v), { err: E.penyebab, span: true });
      return dialogShell('Mutasi ternak', `<div class="field"><span class="label">Jenis mutasi</span>${segmented('Jenis mutasi', [['Pindah', 'Pindah kandang'], ['Jual', 'Jual'], ['Mati', 'Mati']], v.jenis, 'mutasiJenis')}</div>
        <div class="form-grid">${fld('ternakId', 'Ternak', sel('ternakId', [['', 'Pilih ternak aktif'], ...ternakOptions()], v, true), { err: E.ternakId, span: true, hint: t ? `${spOf(t).nama} ${rasOf(t).nama} · kandang ${kLabel(t.kandangId)}` : '' })}${fld('tanggal', 'Tanggal', inp('tanggal', v, { type: 'date', attrs: `max="${iso(TODAY)}"` }))}${extra}</div>
        ${v.jenis !== 'Pindah' ? `<p class="hint" style="font-size:13px;color:var(--text-2)">${v.jenis === 'Jual' ? 'Status ternak berubah menjadi Terjual dan pemasukan otomatis tercatat di Keuangan.' : 'Status ternak berubah menjadi Mati dan tidak bisa dipilih lagi.'}</p>` : ''}`, 'Simpan', { wide: true });
    }
    case 'pakanMasuk': {
      const p = S.ix.pakan.get(Number(v.pakanId));
      return dialogShell('Catat pakan masuk', `<div class="form-grid">${fld('pakanId', 'Pakan', sel('pakanId', S.db.pakan.map((x) => [x.id, `${x.nama} (stok ${num(x.stok)} ${x.satuan})`]), v, true), { span: true })}${fld('jumlah', `Jumlah (${p.satuan})`, inp('jumlah', v, { type: 'number', attrs: 'min="0"' }), { err: E.jumlah })}${fld('total', 'Total harga (Rp)', inp('total', v, { type: 'number', ph: v.jumlah ? String(Number(v.jumlah) * p.harga) : '0' }), { hint: `Kosongkan untuk ${rp(p.harga)} per ${p.satuan}` })}${fld('pemasok', 'Pemasok', inp('pemasok', v, { ph: 'mis. UD Tani Makmur' }))}${fld('tanggal', 'Tanggal', inp('tanggal', v, { type: 'date', attrs: `max="${iso(TODAY)}"` }))}</div><p class="hint" style="font-size:13px;color:var(--text-2)">Stok bertambah dan pengeluaran otomatis tercatat di Keuangan.</p>`, 'Simpan');
    }
    case 'periksa':
      return dialogShell('Catat pemeriksaan', `<div class="form-grid">${fld('ternakId', 'Ternak', sel('ternakId', [['', 'Pilih ternak'], ...ternakOptions()], v), { err: E.ternakId, span: true })}${fld('tanggal', 'Tanggal', inp('tanggal', v, { type: 'date', attrs: `max="${iso(TODAY)}"` }))}${fld('kondisi', 'Kondisi setelah diperiksa', sel('kondisi', [['Diobati', 'Diobati'], ['Sehat', 'Sehat'], ['Periksa', 'Perlu diperiksa lagi']], v))}${fld('keluhan', 'Keluhan', inp('keluhan', v), { err: E.keluhan })}${fld('diagnosa', 'Diagnosa', inp('diagnosa', v), { err: E.diagnosa })}${fld('tindakan', 'Tindakan', inp('tindakan', v), { span: true })}${fld('obatId', 'Obat (opsional)', sel('obatId', [['', '— tanpa obat —'], ...S.db.obat.map((o) => [o.id, `${o.nama} · stok ${num(o.stok)} ${o.satuan}`])], v, true))}${fld('dosis', `Dosis${v.obatId ? ` (${S.ix.obat.get(Number(v.obatId)).satuan})` : ''}`, inp('dosis', v, { type: 'number', attrs: v.obatId ? '' : 'disabled' }), { err: E.dosis })}</div>`, 'Simpan', { wide: true });
    case 'jadwal':
      return dialogShell('Jadwalkan vaksin', `<div class="form-grid">${fld('kandangId', 'Kandang', sel('kandangId', S.db.kandang.map((k) => [k.id, `${k.kode} · ${k.nama} (${isiKandang(k.id)} ekor)`]), v))}${fld('obatId', 'Vaksin', sel('obatId', S.db.obat.filter((o) => o.jenis === 'Vaksin').map((o) => [o.id, o.nama]), v))}${fld('rencana', 'Tanggal rencana', inp('rencana', v, { type: 'date', attrs: `min="${iso(TODAY)}"` }), { err: E.rencana, hint: 'Pengingat muncul 3 hari sebelumnya' })}</div>`, 'Jadwalkan');
    case 'kawin': {
      const ibu = v.indukId ? T(v.indukId) : null;
      const sp = ibu ? spOf(ibu) : null;
      const perk = ibu && v.tanggal ? fmtDate(iso(addDays(parseIso(v.tanggal), sp.bunting))) : '—';
      const sibuk = new Set(S.db.kawin.filter((k) => ['Bunting', 'Menunggu'].includes(k.status)).map((k) => k.indukId));
      return dialogShell('Catat perkawinan', `<div class="form-grid">${fld('indukId', 'Induk betina', sel('indukId', [['', 'Pilih induk'], ...ternakOptions((t) => t.jk === 'Betina' && !sibuk.has(t.id))], v, true), { err: E.indukId, span: true })}
        <div class="field"><span class="label">Metode</span>${segmented('Metode', [['Alami', 'Alami'], ['IB', 'Inseminasi buatan']], v.metode, 'kawinMetode')}</div>${fld('tanggal', 'Tanggal kawin', inp('tanggal', v, { type: 'date', attrs: `max="${iso(TODAY)}"` }), { live: true })}
        ${v.metode === 'IB' ? fld('kodeStraw', 'Kode straw', inp('kodeStraw', v, { ph: 'mis. LIM-20431' }), { err: E.kodeStraw }) : fld('pejantanId', 'Pejantan', sel('pejantanId', [['', 'Pilih pejantan'], ...ternakOptions((t) => t.jk === 'Jantan' && (!ibu || t.spesiesId === ibu.spesiesId))], v), { err: E.pejantanId })}
        <div class="field"><span class="label">Perkiraan lahir</span><div class="input" style="display:flex;align-items:center;background:var(--surface-sunken)">${perk}</div><span class="hint">${sp ? `Tanggal kawin + ${sp.bunting} hari (${sp.nama})` : 'Pilih induk dulu'}</span></div></div>`, 'Simpan', { wide: true });
    }
    case 'lahir': {
      const kw = S.db.kawin.find((k) => k.id === Number(v.kawinId));
      const ibu = T(kw.indukId);
      const n = Math.max(0, Number(v.jumlah || 0) - Number(v.mati || 0));
      let anak = '';
      for (let i = 0; i < Math.min(n, 4); i++) anak += `${fld(`jk${i}`, `Anak ${i + 1} · kelamin`, sel(`jk${i}`, [['Jantan', 'Jantan'], ['Betina', 'Betina']], v))}${fld(`berat${i}`, `Anak ${i + 1} · berat lahir (kg)`, inp(`berat${i}`, v, { type: 'number', attrs: 'step="0.1"' }))}`;
      return dialogShell(`Catat kelahiran · ${ibu.kode}`, `<div class="form-grid">${fld('tanggal', 'Tanggal lahir', inp('tanggal', v, { type: 'date', attrs: `max="${iso(TODAY)}"` }))}${fld('jumlah', 'Jumlah anak', sel('jumlah', [1, 2, 3, 4].map((x) => [x, x]), v, true))}${fld('mati', 'Mati saat lahir', sel('mati', [0, 1, 2, 3, 4].map((x) => [x, x]), v, true), { err: E.mati })}${fld('kandangId', 'Kandang anak', sel('kandangId', kandangOptions(), v), { err: E.kandangId })}${anak}</div><p class="hint" style="font-size:13px;color:var(--text-2)">${n} anak akan didaftarkan ke data ternak dengan induk ${ibu.kode}.</p>`, 'Simpan', { wide: true });
    }
    case 'transaksi':
      return dialogShell('Catat transaksi', `<div class="field"><span class="label">Jenis</span>${segmented('Jenis transaksi', [['Masuk', 'Pemasukan'], ['Keluar', 'Pengeluaran']], v.jenis, 'trxJenis')}</div><div class="form-grid">${fld('kategori', 'Kategori', sel('kategori', (v.jenis === 'Masuk' ? ['Penjualan susu', 'Penjualan pupuk', 'Lain-lain'] : ['Gaji pekerja', 'Obat dan vaksin', 'Listrik dan air', 'Perbaikan kandang', 'Lain-lain']).map((x) => [x, x]), v))}${fld('jumlah', 'Jumlah (Rp)', inp('jumlah', v, { type: 'number' }), { err: E.jumlah })}${fld('tanggal', 'Tanggal', inp('tanggal', v, { type: 'date', attrs: `max="${iso(TODAY)}"` }))}${fld('ket', 'Keterangan', inp('ket', v))}</div>`, 'Simpan');
    case 'master': {
      const m = MASTER[v._ent];
      const body = `<div class="form-grid">${m.fields.map(([n, l, ty]) => {
        if (Array.isArray(ty)) return fld(n, l, sel(n, ty.map((x) => [x, x]), v));
        if (ty === 'spesies') return fld(n, l, sel(n, S.db.spesies.map((s) => [s.id, s.nama]), v));
        if (ty === 'spesies0') return fld(n, l, sel(n, [[0, 'Campuran'], ...S.db.spesies.map((s) => [s.id, s.nama])], v));
        return fld(n, l, inp(n, v, { type: ty }), { err: E[n] });
      }).join('')}</div>`;
      return dialogShell(`${v._id ? 'Ubah' : 'Tambah'} ${m.title}`, body, 'Simpan');
    }
    case 'pengguna':
      return dialogShell('Tambah pengguna', `<div class="form-grid">${fld('nama', 'Nama lengkap', inp('nama', v), { err: E.nama })}${fld('username', 'Username', inp('username', v), { err: E.username })}${fld('peran', 'Peran', sel('peran', [['Petugas', 'Petugas kandang'], ['Mantri', 'Mantri'], ['Admin', 'Admin']], v))}<span></span>${fld('pw', 'Password', inp('pw', v, { type: 'password' }), { err: E.pw, hint: 'Minimal 6 karakter' })}${fld('pw2', 'Ulangi password', inp('pw2', v, { type: 'password' }), { err: E.pw2 })}</div>`, 'Simpan', { wide: true });
    case 'gantiPw':
      return dialogShell('Ganti password', `${fld('lama', 'Password lama', inp('lama', v, { type: 'password' }), { err: E.lama })}${fld('baru', 'Password baru', inp('baru', v, { type: 'password' }), { err: E.baru, hint: 'Minimal 6 karakter' })}${fld('baru2', 'Ulangi password baru', inp('baru2', v, { type: 'password' }), { err: E.baru2 })}`, 'Simpan');
    case 'confirm': {
      const c = { restore: ['Pulihkan database?', `Data di ${S.db.file} akan diganti dengan isi file backup yang dipilih.`, 'Pilih file backup', false], resetTamu: ['Kosongkan database tamu?', 'Semua perubahan di tamu.db hilang dan diganti data contoh awal. Database akun tidak tersentuh.', 'Kosongkan', true] }[v.what];
      return dialogShell(c[0], `<p style="font-size:14px">${c[1]}</p>`, c[2], { danger: c[3] });
    }
    default: return '';
  }
}

function openDialog(type, arg) {
  const today = iso(TODAY);
  let values = {};
  const [a, b] = String(arg ?? '').split(':');
  switch (type) {
    case 'tambahTernak': {
      const k = S.db.kandang.find((x) => isiKandang(x.id) < x.kapasitas);
      const spId = k?.spesiesId || 1;
      values = { spesiesId: spId, rasId: S.db.ras.find((r) => r.spesiesId === spId).id, kode: nextKode(spId), _kodeAuto: nextKode(spId), nama: '', jk: 'Jantan', lahir: iso(addDays(TODAY, -365)), kandangId: k?.id, berat: '', asal: 'Beli', hargaBeli: '', indukId: '' };
      break;
    }
    case 'timbang': values = { ternakId: a, tanggal: today, berat: '', lingkarDada: '' }; break;
    case 'mutasi': values = { ternakId: a || '', jenis: 'Pindah', tanggal: today, kandangTujuanId: '', hargaJual: '', pembeli: '', penyebab: '' }; break;
    case 'pakanMasuk': values = { pakanId: a || S.db.pakan[0].id, jumlah: '', total: '', pemasok: '', tanggal: today }; break;
    case 'periksa': values = { ternakId: a || '', tanggal: today, keluhan: '', diagnosa: '', tindakan: '', obatId: '', dosis: '', kondisi: 'Diobati' }; break;
    case 'jadwal': values = { kandangId: S.db.kandang[0].id, obatId: S.db.obat.find((o) => o.jenis === 'Vaksin').id, rencana: iso(addDays(TODAY, 7)) }; break;
    case 'kawin': values = { indukId: '', metode: 'Alami', tanggal: today, pejantanId: '', kodeStraw: '' }; break;
    case 'lahir': { const kw = S.db.kawin.find((k) => k.id === Number(a)); const ibu = T(kw.indukId); values = { kawinId: a, tanggal: today, jumlah: 1, mati: 0, kandangId: ibu.kandangId, jk0: 'Betina', berat0: '' }; break; }
    case 'transaksi': values = { jenis: 'Keluar', kategori: 'Gaji pekerja', jumlah: '', tanggal: today, ket: '' }; break;
    case 'master': {
      const m = MASTER[a];
      const ex = b ? S.db[m.arr].find((x) => x.id === Number(b)) : null;
      values = { _ent: a, _id: b || '' };
      for (const [n, , ty] of m.fields) values[n] = ex ? ex[n] : Array.isArray(ty) ? ty[0] : ty === 'spesies' ? 1 : ty === 'spesies0' ? 0 : '';
      break;
    }
    case 'pengguna': values = { nama: '', username: '', peran: 'Petugas', pw: '', pw2: '' }; break;
    case 'gantiPw': values = { lama: '', baru: '', baru2: '' }; break;
    case 'confirm': values = { what: a }; break;
    default: break;
  }
  S.dialog = { type, values, errors: {} };
  S.notif = false;
  S.anim.add('dialog');
  renderOverlay();
}

function closeDialog() {
  const scrim = document.querySelector('#overlay .scrim');
  if (!scrim) { S.dialog = null; renderOverlay(); return; }
  scrim.classList.add('closing');
  setTimeout(() => { S.dialog = null; renderOverlay(); }, 120);
}

function captureDialog() {
  const form = document.querySelector('#overlay form[data-form="dialog"]');
  if (!form) return;
  const fd = new FormData(form);
  for (const [k, v] of fd.entries()) S.dialog.values[k] = v;
}

function finish(msg, undo, fresh) {
  saveDb();
  reindex();
  S.dialog = null;
  S.fresh = fresh ?? null;
  showSnack(msg, undo);
  render();
}

function submitDialog() {
  captureDialog();
  const d = S.dialog;
  const v = d.values;
  const E = {};
  const db = S.db;
  const user = S.user.username;
  const nowJam = () => { const n = new Date(); return `${String(n.getHours()).padStart(2, '0')}.${String(n.getMinutes()).padStart(2, '0')}`; };
  switch (d.type) {
    case 'tambahTernak': {
      const kode = v.kode.trim().toUpperCase();
      if (!kode) E.kode = 'Kode wajib diisi';
      else if (db.ternak.some((t) => t.kode === kode)) E.kode = `Kode ${kode} sudah dipakai`;
      if (!(Number(v.berat) > 0)) E.berat = 'Isi berat lebih dari 0';
      if (!v.kandangId) E.kandangId = 'Pilih kandang';
      else if (isiKandang(v.kandangId) >= K(v.kandangId).kapasitas) E.kandangId = 'Kandang sudah penuh';
      if (!v.lahir || v.lahir > iso(TODAY)) E.lahir = 'Tanggal lahir tidak valid';
      if (Object.keys(E).length) break;
      const id = nextId(db.ternak);
      const t = { id, kode, nama: v.nama.trim(), spesiesId: Number(v.spesiesId), rasId: Number(v.rasId), kandangId: Number(v.kandangId), jk: v.jk, lahir: v.lahir, asal: v.asal, tanggalMasuk: iso(TODAY), hargaBeli: v.asal === 'Beli' && v.hargaBeli ? Number(v.hargaBeli) : null, indukId: v.indukId ? Number(v.indukId) : null, pejantanId: null, status: 'Aktif', kondisi: 'Sehat', berat: Number(v.berat), dicatatOleh: user };
      db.ternak.push(t);
      db.timbang.push({ id: nextId(db.timbang), ternakId: id, tanggal: iso(TODAY), berat: Number(v.berat), lingkarDada: 0, oleh: user });
      S.route = { page: 'ternak' };
      S.ui.ternak = { q: '', sp: String(t.spesiesId), kandang: 'all', status: 'Aktif', page: 1 };
      reindex();
      const list = ternakFiltered().filter((x) => x.spesiesId === t.spesiesId).sort((a, b) => a.kode.localeCompare(b.kode));
      S.ui.ternak.page = Math.floor(list.findIndex((x) => x.id === id) / PER_PAGE) + 1;
      S.anim.add('page');
      return finish(`${kode} ditambahkan`, () => { db.ternak = db.ternak.filter((x) => x.id !== id); db.timbang = db.timbang.filter((x) => x.ternakId !== id); }, id);
    }
    case 'timbang': {
      if (!(Number(v.berat) > 0)) { E.berat = 'Isi berat lebih dari 0'; break; }
      const id = nextId(db.timbang);
      db.timbang.push({ id, ternakId: Number(v.ternakId), tanggal: v.tanggal, berat: Number(v.berat), lingkarDada: Number(v.lingkarDada) || 0, oleh: user });
      const t = T(v.ternakId);
      const prev = t.berat;
      t.berat = Number(v.berat);
      S.ui.detailTab = 'pertumbuhan';
      return finish(`Penimbangan ${t.kode} disimpan`, () => { db.timbang = db.timbang.filter((x) => x.id !== id); t.berat = prev; });
    }
    case 'mutasi': {
      const t = v.ternakId ? T(v.ternakId) : null;
      if (!t) E.ternakId = 'Pilih ternak';
      if (v.jenis === 'Pindah' && !v.kandangTujuanId) E.kandangTujuanId = 'Pilih kandang tujuan';
      if (v.jenis === 'Jual') { if (!(Number(v.hargaJual) > 0)) E.hargaJual = 'Isi harga jual'; if (!v.pembeli.trim()) E.pembeli = 'Isi nama pembeli'; }
      if (v.jenis === 'Mati' && !v.penyebab.trim()) E.penyebab = 'Isi penyebab';
      if (Object.keys(E).length) break;
      const before = { ...t };
      const id = nextId(db.mutasi);
      const m = { id, ternakId: t.id, tanggal: v.tanggal, jenis: v.jenis, oleh: user };
      let fid = null;
      if (v.jenis === 'Pindah') { Object.assign(m, { kandangAsalId: t.kandangId, kandangTujuanId: Number(v.kandangTujuanId) }); t.kandangId = Number(v.kandangTujuanId); }
      if (v.jenis === 'Jual') {
        Object.assign(m, { hargaJual: Number(v.hargaJual), pembeli: v.pembeli.trim() });
        t.status = 'Terjual'; t.kandangId = null;
        fid = nextId(db.keuangan);
        db.keuangan.push({ id: fid, tanggal: v.tanggal, jenis: 'Masuk', kategori: 'Penjualan ternak', jumlah: Number(v.hargaJual), ket: `${t.kode} · ${v.pembeli.trim()}`, sumber: 'Mutasi' });
      }
      if (v.jenis === 'Mati') { m.penyebab = v.penyebab.trim(); t.status = 'Mati'; t.kandangId = null; }
      db.mutasi.push(m);
      if (S.route.page !== 'detail') { S.route = { page: 'mutasi' }; S.ui.mutasi = 'Semua'; }
      return finish(`Mutasi ${t.kode} disimpan`, () => { Object.assign(t, before); db.mutasi = db.mutasi.filter((x) => x.id !== id); if (fid) db.keuangan = db.keuangan.filter((x) => x.id !== fid); }, `mt${id}`);
    }
    case 'pakanMasuk': {
      const p = S.ix.pakan.get(Number(v.pakanId));
      const j = Number(v.jumlah);
      if (!(j > 0)) { E.jumlah = 'Isi jumlah lebih dari 0'; break; }
      const total = Number(v.total) || j * p.harga;
      const id = nextId(db.pakanMasuk), fid = nextId(db.keuangan);
      db.pakanMasuk.push({ id, pakanId: p.id, tanggal: v.tanggal, jumlah: j, total, pemasok: v.pemasok.trim() || '—', oleh: user });
      db.keuangan.push({ id: fid, tanggal: v.tanggal, jenis: 'Keluar', kategori: 'Pakan', jumlah: total, ket: `${p.nama} ${num(j)} ${p.satuan} · ${v.pemasok.trim() || '—'}`, sumber: 'PakanMasuk' });
      p.stok += j;
      return finish(`${p.nama} bertambah ${num(j)} ${p.satuan}`, () => { p.stok -= j; db.pakanMasuk = db.pakanMasuk.filter((x) => x.id !== id); db.keuangan = db.keuangan.filter((x) => x.id !== fid); }, `pm${id}`);
    }
    case 'periksa': {
      const t = v.ternakId ? T(v.ternakId) : null;
      if (!t) E.ternakId = 'Pilih ternak';
      if (!v.keluhan.trim()) E.keluhan = 'Isi keluhan';
      if (!v.diagnosa.trim()) E.diagnosa = 'Isi diagnosa';
      const ob = v.obatId ? S.ix.obat.get(Number(v.obatId)) : null;
      if (ob && !(Number(v.dosis) > 0)) E.dosis = 'Isi dosis';
      else if (ob && Number(v.dosis) > ob.stok) E.dosis = `Stok hanya ${num(ob.stok)} ${ob.satuan}`;
      if (Object.keys(E).length) break;
      const id = nextId(db.kesehatan);
      db.kesehatan.push({ id, ternakId: t.id, tanggal: v.tanggal, keluhan: v.keluhan.trim(), diagnosa: v.diagnosa.trim(), tindakan: v.tindakan.trim() || '—', obatId: ob ? ob.id : null, dosis: ob ? Number(v.dosis) : null, oleh: user });
      if (ob) ob.stok -= Number(v.dosis);
      const prevK = t.kondisi;
      t.kondisi = v.kondisi;
      return finish(`Pemeriksaan ${t.kode} disimpan`, () => { db.kesehatan = db.kesehatan.filter((x) => x.id !== id); if (ob) ob.stok += Number(v.dosis); t.kondisi = prevK; }, `ks${id}`);
    }
    case 'jadwal': {
      if (!v.rencana || v.rencana < iso(TODAY)) { E.rencana = 'Pilih tanggal hari ini atau setelahnya'; break; }
      const list = aktif().filter((t) => t.kandangId === Number(v.kandangId));
      let id = nextId(db.jadwal);
      const ids = [];
      for (const t of list) { db.jadwal.push({ id, ternakId: t.id, obatId: Number(v.obatId), rencana: v.rencana, realisasi: null, status: 'Terjadwal' }); ids.push(id); id += 1; }
      S.ui.calMonth = v.rencana.slice(0, 7); S.ui.calDay = v.rencana;
      return finish(`${list.length} ekor dijadwalkan ${fmtDate(v.rencana)}`, () => { db.jadwal = db.jadwal.filter((x) => !ids.includes(x.id)); });
    }
    case 'kawin': {
      const ibu = v.indukId ? T(v.indukId) : null;
      if (!ibu) E.indukId = 'Pilih induk';
      if (v.metode === 'IB' && !v.kodeStraw.trim()) E.kodeStraw = 'Isi kode straw';
      if (v.metode === 'Alami' && !v.pejantanId) E.pejantanId = 'Pilih pejantan';
      if (Object.keys(E).length) break;
      const id = nextId(db.kawin);
      db.kawin.push({ id, indukId: ibu.id, pejantanId: v.metode === 'Alami' ? Number(v.pejantanId) : null, metode: v.metode, kodeStraw: v.metode === 'IB' ? v.kodeStraw.trim() : '', tanggal: v.tanggal, perkiraanLahir: iso(addDays(parseIso(v.tanggal), spOf(ibu).bunting)), status: 'Menunggu', oleh: user });
      S.ui.repro = 'aktif';
      return finish(`Perkawinan ${ibu.kode} dicatat`, () => { db.kawin = db.kawin.filter((x) => x.id !== id); }, `kw${id}`);
    }
    case 'lahir': {
      const kw = db.kawin.find((k) => k.id === Number(v.kawinId));
      const ibu = T(kw.indukId);
      const hidup = Number(v.jumlah) - Number(v.mati);
      if (hidup < 0) { E.mati = 'Lebih banyak dari jumlah anak'; break; }
      if (hidup && isiKandang(v.kandangId) + hidup > K(v.kandangId).kapasitas) { E.kandangId = 'Kapasitas kandang tidak cukup'; break; }
      const ids = [];
      for (let i = 0; i < hidup; i++) {
        const id = nextId(db.ternak);
        const kode = nextKode(ibu.spesiesId);
        const b = Number(v[`berat${i}`]) || (ibu.spesiesId === 1 ? 25 : 3);
        db.ternak.push({ id, kode, nama: '', spesiesId: ibu.spesiesId, rasId: ibu.rasId, kandangId: Number(v.kandangId), jk: v[`jk${i}`] || 'Betina', lahir: v.tanggal, asal: 'Lahir', tanggalMasuk: v.tanggal, hargaBeli: null, indukId: ibu.id, pejantanId: kw.pejantanId, status: 'Aktif', kondisi: 'Sehat', berat: b, dicatatOleh: user });
        db.timbang.push({ id: nextId(db.timbang), ternakId: id, tanggal: v.tanggal, berat: b, lingkarDada: 0, oleh: user });
        ids.push(id);
        reindex();
      }
      kw.status = 'Lahir';
      kw.lahir = { tanggal: v.tanggal, jumlah: Number(v.jumlah), mati: Number(v.mati) };
      return finish(`${hidup} anak ${ibu.kode} didaftarkan`, () => { kw.status = 'Bunting'; delete kw.lahir; db.ternak = db.ternak.filter((x) => !ids.includes(x.id)); db.timbang = db.timbang.filter((x) => !ids.includes(x.ternakId)); });
    }
    case 'transaksi': {
      if (!(Number(v.jumlah) > 0)) { E.jumlah = 'Isi jumlah lebih dari 0'; break; }
      const id = nextId(db.keuangan);
      db.keuangan.push({ id, tanggal: v.tanggal, jenis: v.jenis, kategori: v.kategori, jumlah: Number(v.jumlah), ket: v.ket.trim() || v.kategori, sumber: 'Manual' });
      return finish('Transaksi dicatat', () => { db.keuangan = db.keuangan.filter((x) => x.id !== id); }, `fn${id}`);
    }
    case 'master': {
      const m = MASTER[v._ent];
      for (const [n, , ty] of m.fields) if (ty === 'text' && !String(v[n]).trim()) E[n] = 'Wajib diisi';
      if (v._ent === 'kandang' && !(Number(v.kapasitas) > 0)) E.kapasitas = 'Isi kapasitas';
      if (Object.keys(E).length) break;
      const rec = {};
      for (const [n, , ty] of m.fields) rec[n] = ['number', 'spesies', 'spesies0'].includes(ty) ? Number(v[n]) : String(v[n]).trim();
      if (v._id) Object.assign(db[m.arr].find((x) => x.id === Number(v._id)), rec);
      else {
        const extra = v._ent === 'pakan' ? { stok: 0 } : v._ent === 'obat' ? { stok: 0 } : v._ent === 'spesies' ? { pad: 3 } : {};
        db[m.arr].push({ id: nextId(db[m.arr]), ...extra, ...rec });
      }
      return finish(`Data ${m.title} disimpan`);
    }
    case 'pengguna': {
      if (!v.nama.trim()) E.nama = 'Isi nama';
      if (!v.username.trim()) E.username = 'Isi username';
      else if (db.pengguna.some((u) => u.username === v.username.trim().toLowerCase())) E.username = 'Username sudah dipakai';
      if (v.pw.length < 6) E.pw = 'Minimal 6 karakter';
      else if (v.pw !== v.pw2) E.pw2 = 'Password tidak sama';
      if (Object.keys(E).length) break;
      const id = nextId(db.pengguna);
      db.pengguna.push({ id, username: v.username.trim().toLowerCase(), nama: v.nama.trim(), peran: v.peran, aktif: true, terakhir: iso(TODAY), pw: v.pw });
      return finish(`Akun ${v.username.trim()} dibuat`, () => { db.pengguna = db.pengguna.filter((x) => x.id !== id); }, `us${id}`);
    }
    case 'gantiPw': {
      const u = db.pengguna.find((x) => x.username === S.user.username);
      if (v.lama !== (u.pw || DEMO_PW)) E.lama = 'Password lama salah';
      if (v.baru.length < 6) E.baru = 'Minimal 6 karakter';
      else if (v.baru !== v.baru2) E.baru2 = 'Password tidak sama';
      if (Object.keys(E).length) break;
      u.pw = v.baru;
      return finish('Password diganti');
    }
    case 'confirm': {
      if (v.what === 'resetTamu') { S.db = makeDb('tamu'); saveDb(); reindex(); S.ui = freshUi(); S.route = { page: 'pengaturan' }; return finish('Database tamu dikosongkan'); }
      S.dialog = null; showSnack('Simulasi: jendela pilih file backup terbuka'); renderOverlay(); return;
    }
    default: break;
  }
  d.errors = E;
  renderOverlay();
}

/* ---------------- Palette & notifications ---------------- */
function paletteItems() {
  const q = S.palette.q.trim().toLowerCase();
  const items = [];
  for (const [, list] of MENU) for (const [p, l, ic] of list) if (canPage(p) && (!q || l.toLowerCase().includes(q))) items.push({ group: 'Menu', icon: ic, title: l, sub: 'Buka halaman', act: 'go', arg: p });
  if (q) {
    for (const t of S.db.ternak.filter((x) => `${x.kode} ${x.nama} ${rasOf(x).nama}`.toLowerCase().includes(q)).slice(0, 8)) items.push({ group: 'Ternak', icon: 'tag', title: tLabel(t), sub: `${spOf(t).nama} ${rasOf(t).nama} · ${t.status === 'Aktif' ? kLabel(t.kandangId) : t.status}`, act: 'openTernak', arg: t.id });
    for (const k of S.db.kandang.filter((x) => `kandang ${x.kode} ${x.nama}`.toLowerCase().includes(q))) items.push({ group: 'Kandang', icon: 'home', title: `Kandang ${k.kode} · ${k.nama}`, sub: `${isiKandang(k.id)} dari ${k.kapasitas} ekor`, act: 'selPenGo', arg: k.id });
  }
  return items.slice(0, 14);
}

function viewPalette() {
  if (!S.palette) return '';
  const items = paletteItems();
  if (S.palette.idx >= items.length) S.palette.idx = Math.max(0, items.length - 1);
  let g = '', html = '';
  items.forEach((it, i) => {
    if (it.group !== g) { g = it.group; html += `<li class="group">${g}</li>`; }
    html += `<li><button type="button" class="${i === S.palette.idx ? 'active' : ''}" data-act="palPick" data-arg="${i}"><span class="ico ok" style="width:32px;height:32px;border-radius:6px;display:grid;place-items:center">${icon(it.icon, 16, 2)}</span><span class="t"><b>${esc(it.title)}</b><small>${esc(it.sub)}</small></span>${i === S.palette.idx ? '<kbd>Enter</kbd>' : ''}</button></li>`;
  });
  return `<div class="scrim${enter('palette')}" data-act="closePalette"><div class="dialog palette" data-act="noop" role="dialog" aria-label="Pencarian">
    <div class="search-row">${icon('search', 18, 2)}<label class="sr" for="pal-q">Cari</label><input id="pal-q" placeholder="Cari ternak, kandang, atau menu…" value="${esc(S.palette.q)}" data-palette autocomplete="off"><kbd>Esc</kbd></div>
    <ul>${html || '<li class="empty"><strong>Tidak ditemukan</strong>Coba kode ear tag seperti SP-0112.</li>'}</ul></div></div>`;
}

function viewNotif() {
  if (!S.notif) return '';
  const rem = reminders().filter((r) => !r.done);
  return `<div class="notif-pop${enter('notif')}" role="dialog" aria-label="Notifikasi" data-act="noop"><header><span>Notifikasi</span><span class="badge warn plain">${rem.length}</span></header>
    <ul>${rem.map((r) => `<li><span class="ico ${r.tone}" style="width:30px;height:30px;border-radius:999px;display:grid;place-items:center;flex:none">${icon(r.icon, 14, 2)}</span><div><b style="display:block">${esc(r.title)}</b><span class="muted">${esc(r.sub)}</span></div></li>`).join('') || '<li class="muted">Tidak ada notifikasi</li>'}</ul></div>`;
}

let snackTimer = null;
function showSnack(text, undo) {
  clearTimeout(snackTimer);
  S.snack = { text, undo };
  S.anim.add('snack');
  snackTimer = setTimeout(() => { S.snack = null; renderOverlay(); }, 3000);
}
const viewSnack = () => (S.snack ? `<div class="snackbar${enter('snack')}" role="status">${icon('check', 16, 2.25)}<span>${esc(S.snack.text)}</span>${S.snack.undo ? '<button type="button" data-act="undo">Batalkan</button>' : ''}</div>` : '');

/* ---------------- Render ---------------- */
const root = document.getElementById('app');
const overlay = document.getElementById('overlay');

function captureFocus() {
  const a = document.activeElement;
  if (!a || !a.id) return null;
  return { id: a.id, s: a.selectionStart, e: a.selectionEnd };
}
function restoreFocus(f) {
  if (!f) return;
  const el = document.getElementById(f.id);
  if (!el) return;
  el.focus({ preventScroll: true });
  if (f.s != null && el.setSelectionRange) { try { el.setSelectionRange(f.s, f.e); } catch { /* not a text input */ } }
}

function render() {
  const f = captureFocus();
  const c = document.getElementById('content');
  const keep = c && !S.anim.has('page') ? c.scrollTop : 0;
  const navScroll = document.getElementById('nav-scroll')?.scrollTop || 0;
  if (S.screen !== 'app') S.navY = null;
  const body = S.screen === 'auth' ? viewAuth() : S.screen === 'closed' ? viewClosed() : viewApp();
  root.innerHTML = `<div class="window">${viewTitlebar()}<div class="window-body">${body}</div></div>`;
  const c2 = document.getElementById('content');
  if (c2) c2.scrollTop = keep;
  const ns = document.getElementById('nav-scroll');
  if (ns) { ns.scrollTop = navScroll; placeNavIndicator(); }
  navTip.classList.remove('show');
  renderOverlay(true);
  restoreFocus(f);
  S.anim.clear();
}

function renderOverlay(fromRender) {
  const f = fromRender ? null : captureFocus();
  const opening = S.anim.has('dialog') || S.anim.has('palette');
  overlay.innerHTML = S.screen === 'app' ? `${viewNotif()}${viewDialog()}${viewPalette()}${viewSnack()}` : viewSnack();
  if (opening) {
    const first = overlay.querySelector('#pal-q') || overlay.querySelector('.dialog .body input:not([disabled]), .dialog .body select');
    if (first) first.focus();
  } else restoreFocus(f);
  if (!fromRender) S.anim.clear();
}

function applySettings() {
  document.documentElement.dataset.theme = S.settings.theme;
  document.documentElement.dataset.density = S.settings.density;
}

/* ---------------- Session ---------------- */
function enterApp(mode, username, remember) {
  S.mode = mode;
  S.db = loadDb(mode);
  reindex();
  S.user = mode === 'tamu' ? { username: 'tamu', nama: 'Tamu', peran: 'Tamu' } : { ...S.db.pengguna.find((u) => u.username === username) };
  if (remember) store.set(KEY_SESSION, { mode, username, nama: S.user.nama });
  S.ui = freshUi();
  S.route = { page: 'dashboard' };
  S.screen = 'app';
  S.dialog = null; S.palette = null; S.notif = false;
  S.anim.add('page');
}

function startup() {
  const ses = store.get(KEY_SESSION);
  if (ses) {
    if (ses.mode === 'tamu') { enterApp('tamu', 'tamu', false); return; }
    const akun = loadDb('akun');
    const u = akun.pengguna.find((x) => x.username === ses.username && x.aktif);
    if (u) { enterApp('akun', u.username, false); return; }
    store.del(KEY_SESSION);
  }
  S.screen = 'auth';
  S.auth = { showPw: false, error: '', remember: false, username: '' };
  S.anim.add('auth');
}

function go(page) {
  if (!canPage(page)) return;
  S.route = { page };
  S.notif = false;
  S.palette = null;
  S.fresh = null;
  S.anim.add('page');
  const c = document.getElementById('content');
  if (c) c.scrollTop = 0;
  render();
}

/* ---------------- Actions ---------------- */
const ACT = {
  noop() {},
  togglePw(_, el) {
    S.auth.showPw = !S.auth.showPw;
    const pw = document.getElementById('l-pass');
    pw.type = S.auth.showPw ? 'text' : 'password';
    el.innerHTML = icon(S.auth.showPw ? 'eyeOff' : 'eye', 18);
    el.setAttribute('aria-label', S.auth.showPw ? 'Sembunyikan password' : 'Tampilkan password');
    pw.focus();
  },
  toggleRemember(_, el) { S.auth.remember = el.checked; },
  guest() { enterApp('tamu', 'tamu', S.auth.remember); render(); },
  logout() { store.del(KEY_SESSION); S.screen = 'auth'; S.auth = { showPw: false, error: '', remember: false, username: '' }; S.anim.add('auth'); S.dialog = null; S.snack = null; render(); },
  winClose() { S.screen = 'closed'; S.dialog = null; S.palette = null; S.notif = false; S.snack = null; render(); },
  reopen() { startup(); render(); },
  maxToggle() { S.maxed = !S.maxed; render(); },
  go(p) { go(p); },
  palette() { S.palette = { q: '', idx: 0 }; S.notif = false; S.anim.add('palette'); renderOverlay(); },
  closePalette() { S.palette = null; renderOverlay(); },
  palPick(i) { const it = paletteItems()[Number(i)]; S.palette = null; if (it) ACT[it.act](String(it.arg)); else renderOverlay(); },
  notif() { S.notif = !S.notif; S.anim.add('notif'); renderOverlay(); },
  popSp(v) { S.ui.popSp = v; render(); },
  ternakSp(v) { S.ui.ternak.sp = v; S.ui.ternak.page = 1; render(); },
  ternakPage(p) { S.ui.ternak.page = Number(p); render(); document.getElementById('content').scrollTop = 0; },
  openTernak(id) { S.route = { page: 'detail', id: Number(id) }; S.ui.detailTab = 'pertumbuhan'; S.palette = null; S.notif = false; S.anim.add('page'); render(); document.getElementById('content').scrollTop = 0; },
  detailTab(t) { S.ui.detailTab = t; render(); },
  selPen(id) { S.ui.kandangSel = Number(id); S.anim.add('panel'); render(); },
  selPenGo(id) { S.ui.kandangSel = Number(id); S.palette = null; go('kandang'); },
  lihatKandang(id) { S.ui.ternak = { q: '', sp: 'all', kandang: String(id), status: 'Aktif', page: 1 }; go('ternak'); },
  beriPakan(id) { S.ui.pakanForm = { kandangId: String(id), pakanId: S.ui.pakanForm.pakanId, jumlah: '' }; go('pakan'); },
  pakanQuick(n) { S.ui.pakanForm.jumlah = String(n); render(); document.getElementById('pk-jml')?.focus(); },
  openPakanMasuk(id) { if (S.route.page !== 'pakan' && canPage('pakan')) go('pakan'); openDialog('pakanMasuk', id); },
  calDay(d) { S.ui.calDay = d; S.ui.calMonth = d.slice(0, 7); render(); },
  calNav(n) { const [y, m] = S.ui.calMonth.split('-').map(Number); const d = new Date(y, m - 1 + Number(n), 1); S.ui.calMonth = iso(d).slice(0, 7); render(); },
  reproF(v) { S.ui.repro = v; render(); },
  mutasiF(v) { S.ui.mutasi = v; render(); },
  keuRange(v) { S.ui.keuRange = v; render(); },
  keuJenis(v) { S.ui.keuJenis = v; render(); },
  lapJenis(v) { S.ui.laporan.jenis = v; render(); },
  masterTab(v) { S.ui.masterTab = v; render(); },
  theme(v) { S.settings.theme = v; store.set(KEY_SETTINGS, S.settings); applySettings(); render(); },
  density(v) { S.settings.density = v; store.set(KEY_SETTINGS, S.settings); applySettings(); render(); },
  forget() { store.del(KEY_SESSION); showSnack('Pilihan masuk dilupakan'); render(); },
  rememberNow() { store.set(KEY_SESSION, { mode: S.mode, username: S.user.username, nama: S.user.nama }); showSnack('Pilihan masuk diingat'); render(); },
  backup() { showSnack(`Backup disimpan: ${S.db.file.replace('.db', '')}-backup-${iso(TODAY)}.db`); renderOverlay(); },
  confirm(what) { openDialog('confirm', what); },
  dlg(arg) { const [type, ...rest] = String(arg).split(':'); openDialog(type, rest.join(':')); },
  closeDialog() { closeDialog(); },
  mutasiJenis(v) { captureDialog(); S.dialog.values.jenis = v; S.dialog.errors = {}; renderOverlay(); },
  kawinMetode(v) { captureDialog(); S.dialog.values.metode = v; S.dialog.errors = {}; renderOverlay(); },
  trxJenis(v) { captureDialog(); S.dialog.values.jenis = v; S.dialog.values.kategori = v === 'Masuk' ? 'Penjualan susu' : 'Gaji pekerja'; renderOverlay(); },
  snackExport(kind) {
    const f = { ternak: `data-ternak-${iso(TODAY)}.xlsx`, keuangan: `keuangan-${iso(TODAY)}.xlsx`, pdf: `laporan-${S.ui.laporan.jenis}-${iso(TODAY)}.pdf`, xlsx: `laporan-${S.ui.laporan.jenis}-${iso(TODAY)}.xlsx` }[kind];
    showSnack(`Disimpan ke Dokumen\\TernakKu\\${f}`); renderOverlay();
  },
  vaksinSelesai(ids) {
    const list = ids.split(',').map(Number).map((id) => S.db.jadwal.find((j) => j.id === id));
    const before = list.map((j) => ({ ...j }));
    list.forEach((j) => { j.status = 'Selesai'; j.realisasi = iso(TODAY); j.doneVia = 'pengingat'; });
    saveDb();
    showSnack(`Vaksinasi ${list.length} ekor ditandai selesai`, () => list.forEach((j, i) => Object.assign(j, before[i], { doneVia: undefined })));
    render();
  },
  navToggle() { S.settings.navCollapsed = !S.settings.navCollapsed; store.set(KEY_SETTINGS, S.settings); render(); },
  tugasToggle(key) {
    const done = S.db.tugas.done;
    const i = done.indexOf(key);
    if (i >= 0) done.splice(i, 1); else done.push(key);
    saveDb();
    render();
  },
  tandaiSehat(id) { const t = T(id); const prev = t.kondisi; t.kondisi = 'Sehat'; saveDb(); showSnack(`${t.kode} ditandai sehat`, () => { t.kondisi = prev; }); render(); },
  kawinStatus(arg) { const [id, st] = arg.split(':'); const k = S.db.kawin.find((x) => x.id === Number(id)); const prev = k.status; k.status = st; saveDb(); showSnack(`Status ${T(k.indukId).kode}: ${st}`, () => { k.status = prev; }); render(); },
  resetPw(id) { const u = S.db.pengguna.find((x) => x.id === Number(id)); delete u.pw; saveDb(); showSnack(`Password ${u.username} direset ke password awal`); renderOverlay(); },
  toggleUser(id) { const u = S.db.pengguna.find((x) => x.id === Number(id)); u.aktif = !u.aktif; saveDb(); showSnack(`${u.nama} ${u.aktif ? 'diaktifkan' : 'dinonaktifkan'}`, () => { u.aktif = !u.aktif; }); render(); },
  undo() { const u = S.snack?.undo; S.snack = null; clearTimeout(snackTimer); if (u) { u(); saveDb(); reindex(); S.fresh = null; } render(); }
};

function submitPakan() {
  const f = S.ui.pakanForm;
  const p = S.ix.pakan.get(Number(f.pakanId));
  const j = Number(String(f.jumlah).replace(',', '.'));
  if (!(j > 0) || j > p.stok) return;
  const id = nextId(S.db.pakanLog);
  const n = new Date();
  S.db.pakanLog.push({ id, tanggal: iso(TODAY), jam: `${String(n.getHours()).padStart(2, '0')}.${String(n.getMinutes()).padStart(2, '0')}`, kandangId: Number(f.kandangId), pakanId: p.id, jumlah: j, oleh: S.user.username });
  p.stok -= j;
  f.jumlah = '';
  S.fresh = `log${id}`;
  saveDb();
  showSnack(`${num(j)} ${p.satuan} ${p.nama} untuk kandang ${K(f.kandangId).kode} disimpan`, () => { p.stok += j; S.db.pakanLog = S.db.pakanLog.filter((x) => x.id !== id); });
  render();
}

function login(fd) {
  const username = String(fd.get('username') || '').trim().toLowerCase();
  const pw = String(fd.get('password') || '');
  S.auth.username = username;
  if (!username || !pw) { S.auth.error = 'Isi username dan password'; render(); return; }
  const akun = loadDb('akun');
  const u = akun.pengguna.find((x) => x.username === username);
  if (!u || pw !== (u.pw || DEMO_PW)) { S.auth.error = 'Username atau password salah'; render(); document.getElementById('l-pass')?.focus(); return; }
  if (!u.aktif) { S.auth.error = 'Akun dinonaktifkan. Hubungi admin.'; render(); return; }
  u.terakhir = iso(TODAY);
  store.set(keyDb('akun'), akun);
  enterApp('akun', u.username, S.auth.remember);
  render();
}

/* ---------------- Events ---------------- */
document.addEventListener('click', (e) => {
  const el = e.target.closest('[data-act]');
  if (S.notif && !e.target.closest('.notif-pop') && !(el && el.dataset.act === 'notif')) { S.notif = false; renderOverlay(); }
  if (!el) return;
  if (el.tagName === 'A' || el.tagName === 'BUTTON') e.preventDefault();
  if (el.tagName === 'INPUT') { /* checkbox keeps default */ }
  const fn = ACT[el.dataset.act];
  if (fn) fn(el.dataset.arg, el, e);
});

function setPath(path, value) {
  const keys = path.split('.');
  let o = S;
  for (let i = 0; i < keys.length - 1; i++) o = o[keys[i]];
  o[keys[keys.length - 1]] = value;
}

document.addEventListener('input', (e) => {
  const el = e.target;
  if (el.dataset.palette !== undefined) { S.palette.q = el.value; S.palette.idx = 0; renderOverlay(); return; }
  if (el.dataset.bind && el.tagName === 'INPUT' && el.type !== 'date') {
    setPath(el.dataset.bind, el.value);
    if (el.dataset.resetPage !== undefined) S.ui.ternak.page = 1;
    render();
  }
});

document.addEventListener('change', (e) => {
  const el = e.target;
  if (el.dataset.bind && (el.tagName === 'SELECT' || el.type === 'date')) {
    setPath(el.dataset.bind, el.value);
    if (el.dataset.resetPage !== undefined) S.ui.ternak.page = 1;
    render();
    return;
  }
  if (el.closest('form[data-form="dialog"]') && (el.dataset.live !== undefined || el.type === 'date')) {
    captureDialog();
    const v = S.dialog.values;
    if (S.dialog.type === 'tambahTernak' && el.name === 'spesiesId') {
      v.rasId = S.db.ras.find((r) => r.spesiesId === Number(v.spesiesId)).id;
      if (v.kode === v._kodeAuto) { v.kode = nextKode(v.spesiesId); v._kodeAuto = v.kode; }
      v.indukId = '';
    }
    if (S.dialog.type === 'kawin' && el.name === 'indukId') v.pejantanId = '';
    if (S.dialog.type === 'periksa' && el.name === 'obatId' && !v.obatId) v.dosis = '';
    renderOverlay();
  }
});

document.addEventListener('submit', (e) => {
  e.preventDefault();
  const form = e.target;
  if (form.dataset.form === 'login') login(new FormData(form));
  else if (form.dataset.form === 'dialog') submitDialog();
  else if (form.dataset.form === 'pakan') submitPakan();
});

document.addEventListener('keydown', (e) => {
  const ctrl = e.ctrlKey || e.metaKey;
  if (S.screen !== 'app') return;
  if (S.palette) {
    const n = paletteItems().length;
    if (e.key === 'ArrowDown') { e.preventDefault(); S.palette.idx = Math.min(n - 1, S.palette.idx + 1); renderOverlay(); return; }
    if (e.key === 'ArrowUp') { e.preventDefault(); S.palette.idx = Math.max(0, S.palette.idx - 1); renderOverlay(); return; }
    if (e.key === 'Enter') { e.preventDefault(); ACT.palPick(String(S.palette.idx)); return; }
  }
  if (e.key === 'Escape') {
    if (S.palette) { S.palette = null; renderOverlay(); } else if (S.dialog) closeDialog(); else if (S.notif) { S.notif = false; renderOverlay(); }
    return;
  }
  if (ctrl && e.key.toLowerCase() === 'k') { e.preventDefault(); ACT.palette(); return; }
  if (ctrl && e.key.toLowerCase() === 's' && S.route.page === 'pakan' && !S.dialog) { e.preventDefault(); submitPakan(); return; }
  if (ctrl && e.key.toLowerCase() === 'n' && !S.dialog) {
    const map = { dashboard: 'tambahTernak', ternak: 'tambahTernak', pakan: 'pakanMasuk', kesehatan: 'periksa', reproduksi: 'kawin', mutasi: 'mutasi', keuangan: 'transaksi', pengguna: 'pengguna' };
    const t = map[S.route.page];
    const ok = { tambahTernak: can('tambahTernak'), pakanMasuk: can('pakan'), periksa: can('kesehatan'), kawin: can('kesehatan'), mutasi: can('mutasi'), transaksi: true, pengguna: true };
    if (t && ok[t]) { e.preventDefault(); openDialog(t); }
  }
});

/* ---------------- Boot ---------------- */
S.settings = { ...S.settings, ...(store.get(KEY_SETTINGS) || {}) };
applySettings();
startup();
render();
