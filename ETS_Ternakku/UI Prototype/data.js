// Sample data for the TernakKu prototype. Each database (akun / tamu) is seeded
// separately and stored under its own localStorage key, mimicking two SQLite files.

const DAY = 86400000;
const TODAY = (() => { const d = new Date(); d.setHours(0, 0, 0, 0); return d; })();
const BULAN = ['Jan', 'Feb', 'Mar', 'Apr', 'Mei', 'Jun', 'Jul', 'Agu', 'Sep', 'Okt', 'Nov', 'Des'];
const BULAN_PANJANG = ['Januari', 'Februari', 'Maret', 'April', 'Mei', 'Juni', 'Juli', 'Agustus', 'September', 'Oktober', 'November', 'Desember'];
const HARI = ['Minggu', 'Senin', 'Selasa', 'Rabu', 'Kamis', 'Jumat', 'Sabtu'];

function addDays(d, n) { return new Date(d.getTime() + n * DAY); }
function iso(d) { const z = new Date(d.getTime() - d.getTimezoneOffset() * 60000); return z.toISOString().slice(0, 10); }
function parseIso(s) { const [y, m, d] = s.split('-').map(Number); return new Date(y, m - 1, d); }
function daysBetween(a, b) { return Math.round((b - a) / DAY); }

function mulberry32(seed) {
  return function () {
    seed |= 0; seed = (seed + 0x6D2B79F5) | 0;
    let t = Math.imul(seed ^ (seed >>> 15), 1 | seed);
    t = (t + Math.imul(t ^ (t >>> 7), 61 | t)) ^ t;
    return ((t ^ (t >>> 14)) >>> 0) / 4294967296;
  };
}

const SPESIES = [
  { id: 1, nama: 'Sapi', bunting: 283, prefix: 'SP', pad: 4 },
  { id: 2, nama: 'Kambing', bunting: 150, prefix: 'KB', pad: 3 },
  { id: 3, nama: 'Domba', bunting: 147, prefix: 'DM', pad: 3 }
];

const RAS = [
  { id: 1, spesiesId: 1, nama: 'Bali' }, { id: 2, spesiesId: 1, nama: 'Limosin' },
  { id: 3, spesiesId: 1, nama: 'Simental' }, { id: 4, spesiesId: 1, nama: 'PO' },
  { id: 5, spesiesId: 2, nama: 'Etawa' }, { id: 6, spesiesId: 2, nama: 'Kacang' },
  { id: 7, spesiesId: 2, nama: 'Boer' }, { id: 8, spesiesId: 3, nama: 'Garut' },
  { id: 9, spesiesId: 3, nama: 'Merino' }
];

const NAMA_JANTAN = ['Arjuna', 'Gatot', 'Jaka', 'Bagus', 'Tegar', 'Raja', 'Brewok', 'Kumbang', 'Guntur', 'Samson', 'Badai', 'Joko', 'Ableh', 'Dodo', 'Petruk', 'Bejo'];
const NAMA_BETINA = ['Sri', 'Melati', 'Mawar', 'Ayu', 'Dewi', 'Putri', 'Lestari', 'Wangi', 'Kenanga', 'Sekar', 'Ratna', 'Tari', 'Intan', 'Ningsih', 'Asih', 'Laras'];

function makeDb(kind) {
  const guest = kind === 'tamu';
  const rnd = mulberry32(guest ? 77 : 2026);
  const pick = (arr) => arr[Math.floor(rnd() * arr.length)];
  const between = (a, b) => a + rnd() * (b - a);
  const int = (a, b) => Math.floor(between(a, b + 1));

  const kandangDef = guest ? [
    ['A', 'Penggemukan', 'Penggemukan', 1, 10, 6],
    ['B', 'Indukan', 'Indukan', 1, 10, 8],
    ['C', 'Kambing', 'Indukan', 2, 12, 10],
    ['D', 'Domba', 'Penggemukan', 3, 8, 4]
  ] : [
    ['A', 'Penggemukan', 'Penggemukan', 1, 40, 38],
    ['B', 'Indukan', 'Indukan', 1, 60, 52],
    ['C', 'Kambing', 'Penggemukan', 2, 80, 61],
    ['D', 'Domba', 'Penggemukan', 3, 40, 30],
    ['E', 'Sapi muda', 'Penggemukan', 1, 30, 22],
    ['G', 'Kambing indukan', 'Indukan', 2, 40, 33],
    ['K', 'Karantina', 'Karantina', 0, 30, 12]
  ];
  const kandang = kandangDef.map((k, i) => ({ id: i + 1, kode: k[0], nama: k[1], jenis: k[2], spesiesId: k[3], kapasitas: k[4], target: k[5] }));

  const counters = { 1: 0, 2: 0, 3: 0 };
  const ternak = [];
  const timbang = [];
  let tid = 0, wid = 0;

  function newCode(spId) {
    counters[spId] += 1;
    const sp = SPESIES.find((s) => s.id === spId);
    return `${sp.prefix}-${String(counters[spId]).padStart(sp.pad, '0')}`;
  }

  function addAnimal(k, opts = {}) {
    const spId = opts.spesiesId || (k && k.spesiesId) || pick([1, 2, 3]);
    const rasList = RAS.filter((r) => r.spesiesId === spId);
    let jk = opts.jk;
    if (!jk) {
      if (k && k.jenis === 'Indukan') jk = rnd() < 0.9 ? 'Betina' : 'Jantan';
      else if (k && k.jenis === 'Penggemukan') jk = rnd() < 0.85 ? 'Jantan' : 'Betina';
      else jk = rnd() < 0.5 ? 'Jantan' : 'Betina';
    }
    const umurHari = opts.umurHari || (spId === 1 ? int(320, 1500) : int(200, 1100));
    const lahir = addDays(TODAY, -umurHari);
    const beli = !opts.lahirDiSini && rnd() < 0.62;
    const masuk = beli ? addDays(TODAY, -int(40, Math.min(umurHari - 60, 420))) : lahir;
    let berat, adg;
    if (spId === 1) { berat = Math.min(480, 120 + umurHari * 0.28 + between(-20, 30)); adg = jk === 'Jantan' ? between(0.55, 1.0) : between(0.28, 0.55); }
    else if (spId === 2) { berat = Math.min(62, 14 + umurHari * 0.04 + between(-4, 6)); adg = between(0.05, 0.12); }
    else { berat = Math.min(48, 14 + umurHari * 0.032 + between(-3, 5)); adg = between(0.06, 0.14); }
    tid += 1;
    const t = {
      id: tid,
      kode: opts.kode || newCode(spId),
      nama: opts.nama !== undefined ? opts.nama : (rnd() < 0.55 ? pick(jk === 'Jantan' ? NAMA_JANTAN : NAMA_BETINA) : ''),
      spesiesId: spId,
      rasId: opts.rasId || pick(rasList).id,
      kandangId: k ? k.id : null,
      jk,
      lahir: iso(lahir),
      asal: beli ? 'Beli' : 'Lahir',
      tanggalMasuk: iso(masuk),
      hargaBeli: beli ? Math.round((spId === 1 ? between(9, 18) * 1e6 : between(1.6, 3.4) * 1e6) / 50000) * 50000 : null,
      indukId: null,
      pejantanId: null,
      status: 'Aktif',
      kondisi: 'Sehat',
      berat: Math.round(berat * 10) / 10,
      dicatatOleh: 'admin'
    };
    if (opts.kode) counters[spId] += 1;
    ternak.push(t);
    const span = daysBetween(masuk, TODAY);
    const n = Math.min(10, Math.max(2, Math.floor(span / 14)));
    for (let i = n - 1; i >= 0; i--) {
      const ago = i * 14 + (i === 0 ? int(0, 3) : 0);
      const b = t.berat - adg * ago + (i === 0 ? 0 : between(-0.6, 0.6) * (spId === 1 ? 2 : 0.3));
      wid += 1;
      timbang.push({ id: wid, ternakId: t.id, tanggal: iso(addDays(TODAY, -ago)), berat: Math.round(Math.max(b, 4) * 10) / 10, lingkarDada: spId === 1 ? int(140, 200) : int(55, 85), oleh: pick(['budi', 'admin']) });
    }
    return t;
  }

  for (const k of kandang) {
    for (let i = 0; i < k.target; i++) {
      if (k.jenis === 'Karantina') addAnimal(k, { spesiesId: pick([1, 1, 2, 3]), umurHari: int(250, 900) });
      else if (k.kode === 'E') addAnimal(k, { umurHari: int(240, 480) });
      else addAnimal(k);
    }
  }

  // Signature animal used in the design mock-ups.
  if (!guest) {
    const bima = ternak.find((t) => t.kandangId === 1 && t.spesiesId === 1);
    Object.assign(bima, { nama: 'Bima', rasId: 2, jk: 'Jantan', lahir: iso(addDays(TODAY, -830)), asal: 'Beli', tanggalMasuk: iso(addDays(TODAY, -203)), hargaBeli: 14500000, berat: 412.5 });
    const mine = timbang.filter((w) => w.ternakId === bima.id);
    mine.forEach((w, i) => { w.berat = Math.round((412.5 - (mine.length - 1 - i) * 14 * 0.92 + (i % 2 ? 0.4 : -0.3)) * 10) / 10; });
    mine[mine.length - 1].berat = 412.5;
  }

  // A few purchases this month so the dashboard shows recent arrivals.
  ternak.filter((t) => t.asal === 'Beli' && t.nama !== 'Bima').slice(0, guest ? 2 : 6)
    .forEach((t, i) => { t.tanggalMasuk = iso(addDays(TODAY, -Math.min(i * 3, TODAY.getDate() - 1))); });

  // Parents for some young animals.
  const indukan = ternak.filter((t) => t.jk === 'Betina');
  const pejantan = ternak.filter((t) => t.jk === 'Jantan');
  for (const t of ternak) {
    if (t.asal === 'Lahir' && rnd() < 0.7) {
      const ibu = indukan.find((x) => x.spesiesId === t.spesiesId && x.id !== t.id && x.lahir < t.lahir);
      const bapak = pejantan.find((x) => x.spesiesId === t.spesiesId && x.id !== t.id && x.lahir < t.lahir);
      if (ibu) t.indukId = ibu.id;
      if (bapak) t.pejantanId = bapak.id;
    }
  }

  // Sold and dead animals (inactive).
  const mutasi = [];
  let mid = 0;
  const nonaktif = guest ? 3 : 15;
  for (let i = 0; i < nonaktif; i++) {
    const t = addAnimal(null, { spesiesId: pick([1, 2, 2, 3]) });
    const jual = i % 5 !== 4;
    t.status = jual ? 'Terjual' : 'Mati';
    t.kandangId = null;
    mid += 1;
    const tgl = iso(addDays(TODAY, -int(3, 170)));
    mutasi.push(jual
      ? { id: mid, ternakId: t.id, tanggal: tgl, jenis: 'Jual', hargaJual: Math.round((t.spesiesId === 1 ? between(16, 26) : between(2.5, 4.5)) * 1e6 / 50000) * 50000, pembeli: pick(['Pak Darto', 'CV Sumber Rejeki', 'Bu Ningsih', 'Pasar Hewan Muntilan', 'H. Saleh']), oleh: 'admin' }
      : { id: mid, ternakId: t.id, tanggal: tgl, jenis: 'Mati', penyebab: pick(['Kembung (bloat)', 'Diare berat', 'Cacingan parah']), oleh: 'sari' });
  }
  const aktif = ternak.filter((t) => t.status === 'Aktif');
  for (let i = 0; i < (guest ? 2 : 6); i++) {
    const t = aktif[int(0, aktif.length - 1)];
    const asal = kandang.find((k) => k.id === t.kandangId);
    mid += 1;
    mutasi.push({ id: mid, ternakId: t.id, tanggal: iso(addDays(TODAY, -int(2, 60))), jenis: 'Pindah', kandangAsalId: kandang[(kandang.indexOf(asal) + 1) % kandang.length].id, kandangTujuanId: asal.id, oleh: 'budi' });
  }

  // Health.
  const obat = [
    { id: 1, nama: 'Vaksin PMK', jenis: 'Vaksin', satuan: 'dosis', stok: 46, min: 20 },
    { id: 2, nama: 'Vaksin Anthrax', jenis: 'Vaksin', satuan: 'dosis', stok: 30, min: 20 },
    { id: 3, nama: 'Albendazole (obat cacing)', jenis: 'Obat', satuan: 'bolus', stok: 18, min: 25 },
    { id: 4, nama: 'Oxytetracycline', jenis: 'Obat', satuan: 'ml', stok: 240, min: 100 },
    { id: 5, nama: 'Vitamin B kompleks', jenis: 'Vitamin', satuan: 'ml', stok: 320, min: 100 },
    { id: 6, nama: 'Antiseptik povidone', jenis: 'Obat', satuan: 'ml', stok: 60, min: 80 }
  ];
  const keluhan = [
    ['Nafsu makan turun', 'Indigesti ringan', 'Vitamin B kompleks 10 ml, pakan hijauan', 5, 10],
    ['Diare', 'Enteritis', 'Oxytetracycline 15 ml, cairan elektrolit', 4, 15],
    ['Luka di kaki', 'Luka terbuka', 'Dibersihkan, antiseptik', 6, 5],
    ['Kurus, bulu kusam', 'Cacingan', 'Albendazole 1 bolus', 3, 1],
    ['Perut kembung', 'Bloat ringan', 'Minyak nabati 100 ml, diajak jalan', null, null],
    ['Batuk dan pilek', 'ISPA', 'Oxytetracycline 10 ml', 4, 10]
  ];
  const kesehatan = [];
  let kid = 0;
  const sakit = [];
  for (let i = 0; i < (guest ? 5 : 22); i++) {
    const t = aktif[int(0, aktif.length - 1)];
    const k = pick(keluhan);
    const ago = i < (guest ? 1 : 3) ? int(0, 3) : int(4, 120);
    kid += 1;
    kesehatan.push({ id: kid, ternakId: t.id, tanggal: iso(addDays(TODAY, -ago)), keluhan: k[0], diagnosa: k[1], tindakan: k[2], obatId: k[3], dosis: k[4], oleh: 'sari' });
    if (ago <= 3) sakit.push(t);
  }
  sakit.forEach((t, i) => { t.kondisi = i < 2 ? 'Diobati' : 'Periksa'; });
  for (let i = 0; i < (guest ? 1 : 2); i++) {
    const t = aktif[int(0, aktif.length - 1)];
    if (t.kondisi === 'Sehat') t.kondisi = 'Periksa';
  }

  const jadwal = [];
  let jid = 0;
  const pushJadwal = (t, obatId, offset, status) => {
    jid += 1;
    jadwal.push({ id: jid, ternakId: t.id, obatId, rencana: iso(addDays(TODAY, offset)), realisasi: status === 'Selesai' ? iso(addDays(TODAY, offset)) : null, status });
  };
  const byKandang = (kode) => aktif.filter((t) => kandang.find((k) => k.id === t.kandangId)?.kode === kode);
  byKandang('B').slice(0, guest ? 4 : 12).forEach((t) => pushJadwal(t, 1, 0, 'Terjadwal'));
  byKandang('C').slice(0, guest ? 3 : 8).forEach((t) => pushJadwal(t, 2, 6, 'Terjadwal'));
  byKandang('A').slice(0, guest ? 3 : 10).forEach((t) => pushJadwal(t, 1, -12, 'Selesai'));
  byKandang('D').slice(0, guest ? 2 : 4).forEach((t) => pushJadwal(t, 1, -3, 'Terlewat'));
  byKandang('A').slice(0, guest ? 2 : 6).forEach((t) => pushJadwal(t, 2, 13, 'Terjadwal'));
  if (!guest) byKandang('G').slice(0, 6).forEach((t) => pushJadwal(t, 1, 21, 'Terjadwal'));

  // Reproduction.
  const kawin = [];
  let pid = 0;
  const betina = aktif.filter((t) => t.jk === 'Betina' && t.spesiesId !== 0);
  const nBunting = guest ? 4 : 18;
  for (let i = 0; i < nBunting + (guest ? 3 : 8); i++) {
    const ibu = betina[i * 2 % betina.length];
    const sp = SPESIES.find((s) => s.id === ibu.spesiesId);
    const bapak = pejantan.find((x) => x.spesiesId === ibu.spesiesId && x.status === 'Aktif');
    let ago, status;
    if (i < nBunting) { ago = i < 3 ? sp.bunting - int(1, 6) : int(70, sp.bunting - 10); status = 'Bunting'; }
    else if (i < nBunting + (guest ? 2 : 5)) { ago = int(10, 50); status = 'Menunggu'; }
    else { ago = int(80, 140); status = 'Gagal'; }
    const ib = ibu.spesiesId === 1 && rnd() < 0.6;
    pid += 1;
    const tgl = addDays(TODAY, -ago);
    kawin.push({ id: pid, indukId: ibu.id, pejantanId: ib ? null : bapak?.id || null, metode: ib ? 'IB' : 'Alami', kodeStraw: ib ? `${pick(['LIM', 'SIM', 'BAL'])}-${int(10000, 99999)}` : '', tanggal: iso(tgl), perkiraanLahir: iso(addDays(tgl, sp.bunting)), status, oleh: 'sari' });
  }

  // Feed.
  const pakan = guest ? [
    { id: 1, nama: 'Konsentrat', satuan: 'kg', stok: 90, min: 60, harga: 4500, pakai: 25 },
    { id: 2, nama: 'Rumput gajah', satuan: 'kg', stok: 420, min: 150, harga: 600, pakai: 140 },
    { id: 3, nama: 'Dedak', satuan: 'kg', stok: 35, min: 40, harga: 3200, pakai: 12 },
    { id: 4, nama: 'Mineral blok', satuan: 'buah', stok: 6, min: 4, harga: 18000, pakai: 0.5 }
  ] : [
    { id: 1, nama: 'Konsentrat', satuan: 'kg', stok: 120, min: 200, harga: 4500, pakai: 85 },
    { id: 2, nama: 'Dedak', satuan: 'kg', stok: 140, min: 150, harga: 3200, pakai: 40 },
    { id: 3, nama: 'Rumput gajah', satuan: 'kg', stok: 2400, min: 800, harga: 600, pakai: 950 },
    { id: 4, nama: 'Hijauan campur', satuan: 'kg', stok: 900, min: 400, harga: 700, pakai: 300 },
    { id: 5, nama: 'Ampas tahu', satuan: 'kg', stok: 380, min: 150, harga: 1500, pakai: 90 },
    { id: 6, nama: 'Mineral blok', satuan: 'buah', stok: 24, min: 10, harga: 18000, pakai: 1 }
  ];
  const pakanLog = [];
  let lid = 0;
  const jamPagi = ['06.10', '06.25', '06.40', '07.05', '07.20', '07.35'];
  kandang.slice(0, guest ? 3 : 5).forEach((k, i) => {
    const p = pakan[i % 2 === 0 ? 0 : (guest ? 1 : 2)];
    lid += 1;
    const isi = k.target;
    const jml = p.satuan === 'kg' ? Math.round((k.spesiesId === 1 ? isi * (p.nama === 'Konsentrat' ? 2 : 12) : isi * (p.nama === 'Konsentrat' ? 0.4 : 3))) : 1;
    pakanLog.push({ id: lid, tanggal: iso(TODAY), jam: jamPagi[i], kandangId: k.id, pakanId: p.id, jumlah: jml, oleh: 'budi' });
  });
  const pakanMasuk = [];
  let pmid = 0;
  for (let m = 0; m < 6; m++) {
    for (const p of pakan.slice(0, guest ? 2 : 5)) {
      pmid += 1;
      const jml = Math.round(p.pakai * int(12, 20) / 10) * 10;
      pakanMasuk.push({ id: pmid, pakanId: p.id, tanggal: iso(addDays(TODAY, -(m * 30 + int(2, 25)))), jumlah: jml, total: jml * p.harga, pemasok: pick(['UD Tani Makmur', 'KUD Sido Rukun', 'Pak Wiryo (rumput)', 'Toko Pakan Sentosa']), oleh: 'admin' });
    }
  }

  // Finance: derived rows plus manual ones.
  const keuangan = [];
  let fid = 0;
  for (const pm of pakanMasuk) {
    fid += 1;
    const p = pakan.find((x) => x.id === pm.pakanId);
    keuangan.push({ id: fid, tanggal: pm.tanggal, jenis: 'Keluar', kategori: 'Pakan', jumlah: pm.total, ket: `${p.nama} ${pm.jumlah} ${p.satuan} · ${pm.pemasok}`, sumber: 'PakanMasuk' });
  }
  for (const m of mutasi.filter((x) => x.jenis === 'Jual')) {
    fid += 1;
    const t = ternak.find((x) => x.id === m.ternakId);
    keuangan.push({ id: fid, tanggal: m.tanggal, jenis: 'Masuk', kategori: 'Penjualan ternak', jumlah: m.hargaJual, ket: `${t.kode} · ${m.pembeli}`, sumber: 'Mutasi' });
  }
  for (let m = 0; m < 6; m++) {
    const base = addDays(TODAY, -m * 30);
    const rows = [
      ['Keluar', 'Gaji pekerja', guest ? 2500000 : 7500000, 'Gaji 3 anak kandang'],
      ['Keluar', 'Obat dan vaksin', int(4, 12) * 150000, 'Belanja apotek hewan'],
      ['Keluar', 'Listrik dan air', int(5, 8) * 100000, 'Tagihan bulanan'],
      ['Masuk', 'Penjualan pupuk', int(6, 14) * 100000, 'Pupuk kandang karungan']
    ];
    if (!guest && m % 2 === 0) rows.push(['Masuk', 'Penjualan susu', int(18, 30) * 100000, 'Susu kambing etawa']);
    for (const r of rows) {
      fid += 1;
      keuangan.push({ id: fid, tanggal: iso(addDays(base, -int(0, 26))), jenis: r[0], kategori: r[1], jumlah: r[2], ket: r[3], sumber: 'Manual' });
    }
  }

  // Population snapshot for the last 12 months (per species).
  const populasi = [];
  const now = { 1: aktif.filter((t) => t.spesiesId === 1).length, 2: aktif.filter((t) => t.spesiesId === 2).length, 3: aktif.filter((t) => t.spesiesId === 3).length };
  for (let i = 11; i >= 0; i--) {
    const f = 1 - i * (guest ? 0.018 : 0.0185);
    const d = new Date(TODAY.getFullYear(), TODAY.getMonth() - i, 1);
    populasi.push({ bulan: BULAN[d.getMonth()], tahun: d.getFullYear(), 1: Math.round(now[1] * f), 2: Math.round(now[2] * (f - 0.01 * Math.sin(i))), 3: Math.round(now[3] * f) });
  }

  const pengguna = guest ? [
    { id: 1, username: 'tamu', nama: 'Tamu', peran: 'Tamu', aktif: true, terakhir: iso(TODAY) }
  ] : [
    { id: 1, username: 'admin', nama: 'Admin', peran: 'Admin', aktif: true, terakhir: iso(TODAY) },
    { id: 2, username: 'petugas', nama: 'Budi Santoso', peran: 'Petugas', aktif: true, terakhir: iso(addDays(TODAY, -1)) },
    { id: 3, username: 'mantri', nama: 'drh. Sari Wulandari', peran: 'Mantri', aktif: true, terakhir: iso(addDays(TODAY, -2)) },
    { id: 4, username: 'yanto', nama: 'Yanto', peran: 'Petugas', aktif: false, terakhir: iso(addDays(TODAY, -64)) }
  ];

  return {
    kind,
    file: guest ? 'tamu.db' : 'ternakku.db',
    peternakan: guest ? 'Peternakan Contoh' : 'Peternakan Sumber Makmur',
    dibuat: iso(TODAY),
    spesies: SPESIES.map((s) => ({ ...s })),
    ras: RAS.map((r) => ({ ...r })),
    kandang: kandang.map(({ target, ...k }) => k),
    ternak, timbang, mutasi, obat, kesehatan, jadwal, kawin,
    pakan, pakanLog, pakanMasuk, keuangan, populasi, pengguna,
    pengingatSelesai: []
  };
}
