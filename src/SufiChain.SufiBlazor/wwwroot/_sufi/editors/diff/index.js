import { P as pe, E as A, k as U, I as ve, a as R, J as Fe, K as Ve, S as $, D as x, V as Ie, F as we, L as Pe, R as G, W as Ce, M as je, N as Ue, o as ze, p as We, q as qe, t as He, x as Ke, y as Z, z as _e, A as $e, B as Je, G as Qe } from "../chunks/index-DrKr9VzA.js";
class v {
  constructor(e, t, s, i) {
    this.fromA = e, this.toA = t, this.fromB = s, this.toB = i;
  }
  /**
  @internal
  */
  offset(e, t = e) {
    return new v(this.fromA + e, this.toA + e, this.fromB + t, this.toB + t);
  }
}
function B(n, e, t, s, i, r) {
  if (n == s)
    return [];
  let l = Q(n, e, t, s, i, r), o = Y(n, e + l, t, s, i + l, r);
  e += l, t -= o, i += l, r -= o;
  let f = t - e, d = r - i;
  if (!f || !d)
    return [new v(e, t, i, r)];
  if (f > d) {
    let h = n.slice(e, t).indexOf(s.slice(i, r));
    if (h > -1)
      return [
        new v(e, e + h, i, i),
        new v(e + h + d, t, r, r)
      ];
  } else if (d > f) {
    let h = s.slice(i, r).indexOf(n.slice(e, t));
    if (h > -1)
      return [
        new v(e, e, i, i + h),
        new v(t, t, i + h + f, r)
      ];
  }
  if (f == 1 || d == 1)
    return [new v(e, t, i, r)];
  let a = ke(n, e, t, s, i, r);
  if (a) {
    let [h, c, u] = a;
    return B(n, e, h, s, i, c).concat(B(n, h + u, t, s, c + u, r));
  }
  return Ye(n, e, t, s, i, r);
}
let y = 1e9, E = 0, J = !1;
function Ye(n, e, t, s, i, r) {
  let l = t - e, o = r - i;
  if (y < 1e9 && Math.min(l, o) > y * 16 || E > 0 && Date.now() > E)
    return Math.min(l, o) > y * 64 ? [new v(e, t, i, r)] : X(n, e, t, s, i, r);
  let f = Math.ceil((l + o) / 2);
  W.reset(f), q.reset(f);
  let d = (u, g) => n.charCodeAt(e + u) == s.charCodeAt(i + g), a = (u, g) => n.charCodeAt(t - u - 1) == s.charCodeAt(r - g - 1), h = (l - o) % 2 != 0 ? q : null, c = h ? null : W;
  for (let u = 0; u < f; u++) {
    if (u > y || E > 0 && !(u & 63) && Date.now() > E)
      return X(n, e, t, s, i, r);
    let g = W.advance(u, l, o, f, h, !1, d) || q.advance(u, l, o, f, c, !0, a);
    if (g)
      return Ze(n, e, t, e + g[0], s, i, r, i + g[1]);
  }
  return [new v(e, t, i, r)];
}
class xe {
  constructor() {
    this.vec = [];
  }
  reset(e) {
    this.len = e << 1;
    for (let t = 0; t < this.len; t++)
      this.vec[t] = -1;
    this.vec[e + 1] = 0, this.start = this.end = 0;
  }
  advance(e, t, s, i, r, l, o) {
    for (let f = -e + this.start; f <= e - this.end; f += 2) {
      let d = i + f, a = f == -e || f != e && this.vec[d - 1] < this.vec[d + 1] ? this.vec[d + 1] : this.vec[d - 1] + 1, h = a - f;
      for (; a < t && h < s && o(a, h); )
        a++, h++;
      if (this.vec[d] = a, a > t)
        this.end += 2;
      else if (h > s)
        this.start += 2;
      else if (r) {
        let c = i + (t - s) - f;
        if (c >= 0 && c < this.len && r.vec[c] != -1)
          if (l) {
            let u = r.vec[c];
            if (u >= t - a)
              return [u, i + u - c];
          } else {
            let u = t - r.vec[c];
            if (a >= u)
              return [a, h];
          }
      }
    }
    return null;
  }
}
const W = /* @__PURE__ */ new xe(), q = /* @__PURE__ */ new xe();
function Ze(n, e, t, s, i, r, l, o) {
  let f = !1;
  return !S(n, s) && ++s == t && (f = !0), !S(i, o) && ++o == l && (f = !0), f ? [new v(e, t, r, l)] : B(n, e, s, i, r, o).concat(B(n, s, t, i, o, l));
}
function Ae(n, e) {
  let t = 1, s = Math.min(n, e);
  for (; t < s; )
    t = t << 1;
  return t;
}
function Q(n, e, t, s, i, r) {
  if (e == t || e == r || n.charCodeAt(e) != s.charCodeAt(i))
    return 0;
  let l = Ae(t - e, r - i);
  for (let o = e, f = i; ; ) {
    let d = o + l, a = f + l;
    if (d > t || a > r || n.slice(o, d) != s.slice(f, a)) {
      if (l == 1)
        return o - e - (S(n, o) ? 0 : 1);
      l = l >> 1;
    } else {
      if (d == t || a == r)
        return d - e;
      o = d, f = a;
    }
  }
}
function Y(n, e, t, s, i, r) {
  if (e == t || i == r || n.charCodeAt(t - 1) != s.charCodeAt(r - 1))
    return 0;
  let l = Ae(t - e, r - i);
  for (let o = t, f = r; ; ) {
    let d = o - l, a = f - l;
    if (d < e || a < i || n.slice(d, o) != s.slice(a, f)) {
      if (l == 1)
        return t - o - (S(n, o) ? 0 : 1);
      l = l >> 1;
    } else {
      if (d == e || a == i)
        return t - d;
      o = d, f = a;
    }
  }
}
function H(n, e, t, s, i, r, l, o) {
  let f = s.slice(i, r), d = null;
  for (; ; ) {
    if (d || l < o)
      return d;
    for (let a = e + l; ; ) {
      S(n, a) || a++;
      let h = a + l;
      if (S(n, h) || (h += h == a + 1 ? 1 : -1), h >= t)
        break;
      let c = n.slice(a, h), u = -1;
      for (; (u = f.indexOf(c, u + 1)) != -1; ) {
        let g = Q(n, h, t, s, i + u + c.length, r), p = Y(n, e, a, s, i, i + u), m = c.length + g + p;
        (!d || d[2] < m) && (d = [a - p, i + u - p, m]);
      }
      a = h;
    }
    if (o < 0)
      return d;
    l = l >> 1;
  }
}
function ke(n, e, t, s, i, r) {
  let l = t - e, o = r - i;
  if (l < o) {
    let f = ke(s, i, r, n, e, t);
    return f && [f[1], f[0], f[2]];
  }
  return l < 4 || o * 2 < l ? null : H(n, e, t, s, i, r, Math.floor(l / 4), -1);
}
function X(n, e, t, s, i, r) {
  J = !0;
  let l = t - e, o = r - i, f;
  if (l < o) {
    let c = H(s, i, r, n, e, t, Math.floor(l / 6), 50);
    f = c && [c[1], c[0], c[2]];
  } else
    f = H(n, e, t, s, i, r, Math.floor(o / 6), 50);
  if (!f)
    return [new v(e, t, i, r)];
  let [d, a, h] = f;
  return B(n, e, d, s, i, a).concat(B(n, d + h, t, s, a + h, r));
}
function be(n, e) {
  for (let t = 1; t < n.length; t++) {
    let s = n[t - 1], i = n[t];
    s.toA > i.fromA - e && s.toB > i.fromB - e && (n[t - 1] = new v(s.fromA, i.toA, s.fromB, i.toB), n.splice(t--, 1));
  }
}
function Xe(n, e, t) {
  for (; ; ) {
    be(t, 1);
    let s = !1;
    for (let i = 0; i < t.length; i++) {
      let r = t[i], l, o;
      (l = Q(n, r.fromA, r.toA, e, r.fromB, r.toB)) && (r = t[i] = new v(r.fromA + l, r.toA, r.fromB + l, r.toB)), (o = Y(n, r.fromA, r.toA, e, r.fromB, r.toB)) && (r = t[i] = new v(r.fromA, r.toA - o, r.fromB, r.toB - o));
      let f = r.toA - r.fromA, d = r.toB - r.fromB;
      if (f && d)
        continue;
      let a = r.fromA - (i ? t[i - 1].toA : 0), h = (i < t.length - 1 ? t[i + 1].fromA : n.length) - r.toA;
      if (!a || !h)
        continue;
      let c = f ? n.slice(r.fromA, r.toA) : e.slice(r.fromB, r.toB);
      a <= c.length && n.slice(r.fromA - a, r.fromA) == c.slice(c.length - a) ? (t[i] = new v(r.fromA - a, r.toA - a, r.fromB - a, r.toB - a), s = !0) : h <= c.length && n.slice(r.toA, r.toA + h) == c.slice(0, h) && (t[i] = new v(r.fromA + h, r.toA + h, r.fromB + h, r.toB + h), s = !0);
    }
    if (!s)
      break;
  }
  return t;
}
function et(n, e, t) {
  for (let s = 0, i = 0; i < n.length; i++) {
    let r = n[i], l = r.toA - r.fromA, o = r.toB - r.fromB;
    if (l && o || l > 3 || o > 3) {
      let f = i == n.length - 1 ? e.length : n[i + 1].fromA, d = r.fromA - s, a = f - r.toA, h = te(e, r.fromA, d), c = ee(e, r.toA, a), u = r.fromA - h, g = c - r.toA;
      if ((!l || !o) && u && g) {
        let p = Math.max(l, o), [m, w, M] = l ? [e, r.fromA, r.toA] : [t, r.fromB, r.toB];
        p > u && e.slice(h, r.fromA) == m.slice(M - u, M) ? (r = n[i] = new v(h, h + l, r.fromB - u, r.toB - u), h = r.fromA, c = ee(e, r.toA, f - r.toA)) : p > g && e.slice(r.toA, c) == m.slice(w, w + g) && (r = n[i] = new v(c - l, c, r.fromB + g, r.toB + g), c = r.toA, h = te(e, r.fromA, r.fromA - s)), u = r.fromA - h, g = c - r.toA;
      }
      if (u || g)
        r = n[i] = new v(r.fromA - u, r.toA + g, r.fromB - u, r.toB + g);
      else if (l) {
        if (!o) {
          let p = ne(e, r.fromA, r.toA), m, w = p < 0 ? -1 : re(e, r.toA, r.fromA);
          p > -1 && (m = p - r.fromA) <= a && e.slice(r.fromA, p) == e.slice(r.toA, r.toA + m) ? r = n[i] = r.offset(m) : w > -1 && (m = r.toA - w) <= d && e.slice(r.fromA - m, r.fromA) == e.slice(w, r.toA) && (r = n[i] = r.offset(-m));
        }
      } else {
        let p = ne(t, r.fromB, r.toB), m, w = p < 0 ? -1 : re(t, r.toB, r.fromB);
        p > -1 && (m = p - r.fromB) <= a && t.slice(r.fromB, p) == t.slice(r.toB, r.toB + m) ? r = n[i] = r.offset(m) : w > -1 && (m = r.toB - w) <= d && t.slice(r.fromB - m, r.fromB) == t.slice(w, r.toB) && (r = n[i] = r.offset(-m));
      }
    }
    s = r.toA;
  }
  return be(n, 3), n;
}
let b;
try {
  b = /* @__PURE__ */ new RegExp("[\\p{Alphabetic}\\p{Number}]", "u");
} catch {
}
function Be(n) {
  return n > 48 && n < 58 || n > 64 && n < 91 || n > 96 && n < 123;
}
function Me(n, e) {
  if (e == n.length)
    return 0;
  let t = n.charCodeAt(e);
  return t < 192 ? Be(t) ? 1 : 0 : b ? !Se(t) || e == n.length - 1 ? b.test(String.fromCharCode(t)) ? 1 : 0 : b.test(n.slice(e, e + 2)) ? 2 : 0 : 0;
}
function De(n, e) {
  if (!e)
    return 0;
  let t = n.charCodeAt(e - 1);
  return t < 192 ? Be(t) ? 1 : 0 : b ? !Oe(t) || e == 1 ? b.test(String.fromCharCode(t)) ? 1 : 0 : b.test(n.slice(e - 2, e)) ? 2 : 0 : 0;
}
const Le = 8;
function ee(n, e, t) {
  if (e == n.length || !De(n, e))
    return e;
  for (let s = e, i = e + t, r = 0; r < Le; r++) {
    let l = Me(n, s);
    if (!l || s + l > i)
      return s;
    s += l;
  }
  return e;
}
function te(n, e, t) {
  if (!e || !Me(n, e))
    return e;
  for (let s = e, i = e - t, r = 0; r < Le; r++) {
    let l = De(n, s);
    if (!l || s - l < i)
      return s;
    s -= l;
  }
  return e;
}
function re(n, e, t) {
  for (; e != t; e--)
    if (n.charCodeAt(e - 1) == 10)
      return e;
  return -1;
}
function ne(n, e, t) {
  for (; e != t; e++)
    if (n.charCodeAt(e) == 10)
      return e;
  return -1;
}
const Se = (n) => n >= 55296 && n <= 56319, Oe = (n) => n >= 56320 && n <= 57343;
function S(n, e) {
  return !e || e == n.length || !Se(n.charCodeAt(e - 1)) || !Oe(n.charCodeAt(e));
}
function tt(n, e, t) {
  var s;
  let i = t?.override;
  return i ? i(n, e) : (y = ((s = t?.scanLimit) !== null && s !== void 0 ? s : 1e9) >> 1, E = t?.timeout ? Date.now() + t.timeout : 0, J = !1, Xe(n, e, B(n, 0, n.length, e, 0, e.length)));
}
function ye() {
  return !J;
}
function Ee(n, e, t) {
  return et(tt(n, e, t), n, e);
}
const C = /* @__PURE__ */ we.define({
  combine: (n) => n[0]
}), K = /* @__PURE__ */ R.define(), rt = /* @__PURE__ */ we.define(), k = /* @__PURE__ */ $.define({
  create(n) {
    return null;
  },
  update(n, e) {
    for (let t of e.effects)
      t.is(K) && (n = t.value);
    for (let t of e.state.facet(rt))
      n = t(n, e);
    return n;
  }
});
class L {
  constructor(e, t, s, i, r, l = !0) {
    this.changes = e, this.fromA = t, this.toA = s, this.fromB = i, this.toB = r, this.precise = l;
  }
  /**
  @internal
  */
  offset(e, t) {
    return e || t ? new L(this.changes, this.fromA + e, this.toA + e, this.fromB + t, this.toB + t, this.precise) : this;
  }
  /**
  Returns `fromA` if the chunk is empty in A, or the end of the
  last line in the chunk otherwise.
  */
  get endA() {
    return Math.max(this.fromA, this.toA - 1);
  }
  /**
  Returns `fromB` if the chunk is empty in B, or the end of the
  last line in the chunk otherwise.
  */
  get endB() {
    return Math.max(this.fromB, this.toB - 1);
  }
  /**
  Build a set of changed chunks for the given documents.
  */
  static build(e, t, s) {
    let i = Ee(e.toString(), t.toString(), s);
    return Te(i, e, t, 0, 0, ye());
  }
  /**
  Update a set of chunks for changes in document A. `a` should
  hold the updated document A.
  */
  static updateA(e, t, s, i, r) {
    return ae(oe(e, i, !0, s.length), e, t, s, r);
  }
  /**
  Update a set of chunks for changes in document B.
  */
  static updateB(e, t, s, i, r) {
    return ae(oe(e, i, !1, t.length), e, t, s, r);
  }
}
function ie(n, e, t, s) {
  let i = t.lineAt(n), r = s.lineAt(e);
  return i.to == n && r.to == e && n < t.length && e < s.length ? [n + 1, e + 1] : [i.from, r.from];
}
function se(n, e, t, s) {
  let i = t.lineAt(n), r = s.lineAt(e);
  return i.from == n && r.from == e ? [n, e] : [i.to + 1, r.to + 1];
}
function Te(n, e, t, s, i, r) {
  let l = [];
  for (let o = 0; o < n.length; o++) {
    let f = n[o], [d, a] = ie(f.fromA + s, f.fromB + i, e, t), [h, c] = se(f.toA + s, f.toB + i, e, t), u = [f.offset(-d + s, -a + i)];
    for (; o < n.length - 1; ) {
      let g = n[o + 1], [p, m] = ie(g.fromA + s, g.fromB + i, e, t);
      if (p > h + 1 && m > c + 1)
        break;
      u.push(g.offset(-d + s, -a + i)), [h, c] = se(g.toA + s, g.toB + i, e, t), o++;
    }
    l.push(new L(u, d, Math.max(d, h), a, Math.max(a, c), r));
  }
  return l;
}
const F = 1e3;
function le(n, e, t, s) {
  let i = 0, r = n.length;
  for (; ; ) {
    if (i == r) {
      let a = 0, h = 0;
      i && ({ toA: a, toB: h } = n[i - 1]);
      let c = e - (t ? a : h);
      return [a + c, h + c];
    }
    let l = i + r >> 1, o = n[l], [f, d] = t ? [o.fromA, o.toA] : [o.fromB, o.toB];
    if (f > e)
      r = l;
    else if (d <= e)
      i = l + 1;
    else
      return s ? [o.fromA, o.fromB] : [o.toA, o.toB];
  }
}
function oe(n, e, t, s) {
  let i = [];
  return e.iterChangedRanges((r, l, o, f) => {
    let d = 0, a = t ? e.length : s, h = 0, c = t ? s : e.length;
    r > F && ([d, h] = le(n, r - F, t, !0)), l < e.length - F && ([a, c] = le(n, l + F, t, !1));
    let u = f - o - (l - r), g, [p, m] = t ? [u, 0] : [0, u];
    i.length && (g = i[i.length - 1]).toA >= d ? i[i.length - 1] = {
      fromA: g.fromA,
      fromB: g.fromB,
      toA: a,
      toB: c,
      diffA: g.diffA + p,
      diffB: g.diffB + m
    } : i.push({ fromA: d, toA: a, fromB: h, toB: c, diffA: p, diffB: m });
  }), i;
}
function ae(n, e, t, s, i) {
  if (!n.length)
    return e;
  let r = [];
  for (let l = 0, o = 0, f = 0, d = 0; ; l++) {
    let a = l == n.length ? null : n[l], h = a ? a.fromA + o : t.length, c = a ? a.fromB + f : s.length;
    for (; d < e.length; ) {
      let m = e[d];
      if (a && (m.toA + o > h || m.toB + f > c))
        break;
      r.push(m.offset(o, f)), d++;
    }
    if (!a)
      break;
    let u = a.toA + o + a.diffA, g = a.toB + f + a.diffB, p = Ee(t.sliceString(h, u), s.sliceString(c, g), i);
    for (let m of Te(p, t, s, h, c, ye()))
      r.push(m);
    for (o += a.diffA, f += a.diffB; d < e.length; ) {
      let m = e[d];
      if (m.fromA + o > u && m.fromB + f > g)
        break;
      d++;
    }
  }
  return r;
}
const nt = { scanLimit: 500 }, Re = /* @__PURE__ */ Ie.fromClass(class {
  constructor(n) {
    ({ deco: this.deco, gutter: this.gutter } = de(n));
  }
  update(n) {
    (n.docChanged || n.viewportChanged || it(n.startState, n.state) || st(n.startState, n.state)) && ({ deco: this.deco, gutter: this.gutter } = de(n.view));
  }
}, {
  decorations: (n) => n.deco
}), V = /* @__PURE__ */ pe.low(/* @__PURE__ */ Pe({
  class: "cm-changeGutter",
  markers: (n) => {
    var e;
    return ((e = n.plugin(Re)) === null || e === void 0 ? void 0 : e.gutter) || Ue.empty;
  }
}));
function it(n, e) {
  return n.field(k, !1) != e.field(k, !1);
}
function st(n, e) {
  return n.facet(C) != e.facet(C);
}
const fe = /* @__PURE__ */ x.line({ class: "cm-changedLine" }), lt = /* @__PURE__ */ x.mark({ class: "cm-changedText" }), ot = /* @__PURE__ */ x.mark({ tagName: "ins", class: "cm-insertedLine" }), at = /* @__PURE__ */ x.mark({ tagName: "del", class: "cm-deletedLine" }), he = /* @__PURE__ */ new class extends je {
  constructor() {
    super(...arguments), this.elementClass = "cm-changedLineGutter";
  }
}();
function ft(n, e, t, s, i, r) {
  let l = t ? n.fromA : n.fromB, o = t ? n.toA : n.toB, f = 0;
  if (l != o) {
    i.add(l, l, fe), i.add(l, o, t ? at : ot), r && r.add(l, l, he);
    for (let d = e.iterRange(l, o - 1), a = l; !d.next().done; ) {
      if (d.lineBreak) {
        a++, i.add(a, a, fe), r && r.add(a, a, he);
        continue;
      }
      let h = a + d.value.length;
      if (s)
        for (; f < n.changes.length; ) {
          let c = n.changes[f], u = l + (t ? c.fromA : c.fromB), g = l + (t ? c.toA : c.toB), p = Math.max(a, u), m = Math.min(h, g);
          if (p < m && i.add(p, m, lt), g < h)
            f++;
          else
            break;
        }
      a = h;
    }
  }
}
function de(n) {
  let e = n.state.field(k), { side: t, highlightChanges: s, markGutter: i, overrideChunk: r } = n.state.facet(C), l = t == "a", o = new G(), f = i ? new G() : null, { from: d, to: a } = n.viewport;
  for (let h of e) {
    if ((l ? h.fromA : h.fromB) >= a)
      break;
    (l ? h.toA : h.toB) > d && (!r || !r(n.state, h, o, f)) && ft(h, n.state.doc, l, s, o, f);
  }
  return { deco: o.finish(), gutter: f && f.finish() };
}
class I extends Ce {
  constructor(e) {
    super(), this.height = e;
  }
  eq(e) {
    return this.height == e.height;
  }
  toDOM() {
    let e = document.createElement("div");
    return e.className = "cm-mergeSpacer", e.style.height = this.height + "px", e;
  }
  updateDOM(e) {
    return e.style.height = this.height + "px", !0;
  }
  get estimatedHeight() {
    return this.height;
  }
  ignoreEvent() {
    return !1;
  }
}
const z = /* @__PURE__ */ R.define({
  map: (n, e) => n.map(e)
}), T = /* @__PURE__ */ $.define({
  create: () => x.none,
  update: (n, e) => {
    for (let t of e.effects)
      if (t.is(z))
        return t.value;
    return n.map(e.changes);
  },
  provide: (n) => A.decorations.from(n)
}), P = 0.01;
function ce(n, e) {
  if (n.size != e.size)
    return !1;
  let t = n.iter(), s = e.iter();
  for (; t.value; ) {
    if (t.from != s.from || Math.abs(t.value.spec.widget.height - s.value.spec.widget.height) > 1)
      return !1;
    t.next(), s.next();
  }
  return !0;
}
function ht(n, e, t) {
  let s = new G(), i = new G(), r = n.state.field(T).iter(), l = e.state.field(T).iter(), o = 0, f = 0, d = 0, a = 0, h = n.viewport, c = e.viewport;
  for (let m = 0; ; m++) {
    let w = m < t.length ? t[m] : null, M = w ? w.fromA : n.state.doc.length, Ge = w ? w.fromB : e.state.doc.length;
    if (o < M) {
      let N = n.lineBlockAt(o).top + d, Ne = e.lineBlockAt(f).top + a, D = N - Ne;
      D < -P ? (d -= D, s.add(o, o, x.widget({
        widget: new I(-D),
        block: !0,
        side: -1
      }))) : D > P && (a += D, i.add(f, f, x.widget({
        widget: new I(D),
        block: !0,
        side: -1
      })));
    }
    if (M > o + 1e3 && o < h.from && M > h.from && f < c.from && Ge > c.from) {
      let N = Math.min(h.from - o, c.from - f);
      o += N, f += N, m--;
    } else if (w)
      o = w.toA, f = w.toB;
    else
      break;
    for (; r.value && r.from < o; )
      d -= r.value.spec.widget.height, r.next();
    for (; l.value && l.from < f; )
      a -= l.value.spec.widget.height, l.next();
  }
  for (; r.value; )
    d -= r.value.spec.widget.height, r.next();
  for (; l.value; )
    a -= l.value.spec.widget.height, l.next();
  let u = n.contentHeight + d - (e.contentHeight + a);
  u < P ? s.add(n.state.doc.length, n.state.doc.length, x.widget({
    widget: new I(-u),
    block: !0,
    side: 1
  })) : u > P && i.add(e.state.doc.length, e.state.doc.length, x.widget({
    widget: new I(u),
    block: !0,
    side: 1
  }));
  let g = s.finish(), p = i.finish();
  ce(g, n.state.field(T)) || n.dispatch({ effects: z.of(g) }), ce(p, e.state.field(T)) || e.dispatch({ effects: z.of(p) });
}
const _ = /* @__PURE__ */ R.define({
  map: (n, e) => e.mapPos(n)
});
class dt extends Ce {
  constructor(e) {
    super(), this.lines = e;
  }
  eq(e) {
    return this.lines == e.lines;
  }
  toDOM(e) {
    let t = document.createElement("div");
    return t.className = "cm-collapsedLines", t.textContent = e.state.phrase("$ unchanged lines", this.lines), t.addEventListener("click", (s) => {
      let i = e.posAtDOM(s.target);
      e.dispatch({ effects: _.of(i) });
      let { side: r, sibling: l } = e.state.facet(C);
      l && l().dispatch({ effects: _.of(ct(i, e.state.field(k), r == "a")) });
    }), t;
  }
  ignoreEvent(e) {
    return e instanceof MouseEvent;
  }
  get estimatedHeight() {
    return 27;
  }
  get type() {
    return "collapsed-unchanged-code";
  }
}
function ct(n, e, t) {
  let s = 0, i = 0;
  for (let r = 0; ; r++) {
    let l = r < e.length ? e[r] : null;
    if (!l || (t ? l.fromA : l.fromB) >= n)
      return i + (n - s);
    [s, i] = t ? [l.toA, l.toB] : [l.toB, l.toA];
  }
}
const ut = /* @__PURE__ */ $.define({
  create(n) {
    return x.none;
  },
  update(n, e) {
    n = n.map(e.changes);
    for (let t of e.effects)
      t.is(_) && (n = n.update({ filter: (s) => s != t.value }));
    if (n.size && e.state.field(k) != e.startState.field(k, !1)) {
      let t = e.state.facet(C).side == "a", s = [];
      for (let i of e.state.field(k))
        n.between(t ? i.fromA : i.fromB, t ? i.toA : i.toB, (r) => {
          s.push(r);
        });
      s.length && (n = n.update({ filter: (i) => s.indexOf(i) < 0 }));
    }
    return n;
  },
  provide: (n) => A.decorations.from(n)
});
function ue({ margin: n = 3, minSize: e = 4 }) {
  return ut.init((t) => mt(t, n, e));
}
function mt(n, e, t) {
  let s = new G(), i = n.facet(C).side == "a", r = n.field(k), l = 1;
  for (let o = 0; ; o++) {
    let f = o < r.length ? r[o] : null, d = o ? l + e : 1, a = f ? n.doc.lineAt(i ? f.fromA : f.fromB).number - 1 - e : n.doc.lines, h = a - d + 1;
    if (h >= t && s.add(n.doc.line(d).from, n.doc.line(a).to, x.replace({
      widget: new dt(h),
      block: !0
    })), !f)
      break;
    l = n.doc.lineAt(Math.min(n.doc.length, i ? f.toA : f.toB)).number;
  }
  return s.finish();
}
const gt = /* @__PURE__ */ A.styleModule.of(/* @__PURE__ */ new Ve({
  ".cm-mergeView": {
    overflowY: "auto"
  },
  ".cm-mergeViewEditors": {
    display: "flex",
    alignItems: "stretch"
  },
  ".cm-mergeViewEditor": {
    flexGrow: 1,
    flexBasis: 0,
    overflow: "hidden"
  },
  ".cm-merge-revert": {
    width: "1.6em",
    flexGrow: 0,
    flexShrink: 0,
    position: "relative"
  },
  ".cm-merge-revert button": {
    position: "absolute",
    display: "block",
    width: "100%",
    boxSizing: "border-box",
    textAlign: "center",
    background: "none",
    border: "none",
    font: "inherit",
    cursor: "pointer"
  }
})), pt = /* @__PURE__ */ A.baseTheme({
  ".cm-mergeView & .cm-scroller, .cm-mergeView &": {
    height: "auto !important",
    overflowY: "visible !important"
  },
  "&.cm-merge-a .cm-changedLine, .cm-deletedChunk": {
    backgroundColor: "rgba(160, 128, 100, .08)"
  },
  "&.cm-merge-b .cm-changedLine, .cm-inlineChangedLine": {
    backgroundColor: "rgba(100, 160, 128, .08)"
  },
  "&light.cm-merge-a .cm-changedText, &light .cm-deletedChunk .cm-deletedText": {
    background: "linear-gradient(#ee443366, #ee443366) bottom/100% 2px no-repeat"
  },
  "&dark.cm-merge-a .cm-changedText, &dark .cm-deletedChunk .cm-deletedText": {
    background: "linear-gradient(#ffaa9966, #ffaa9966) bottom/100% 2px no-repeat"
  },
  "&light.cm-merge-b .cm-changedText": {
    background: "linear-gradient(#22bb22aa, #22bb22aa) bottom/100% 2px no-repeat"
  },
  "&dark.cm-merge-b .cm-changedText": {
    background: "linear-gradient(#88ff88aa, #88ff88aa) bottom/100% 2px no-repeat"
  },
  "&.cm-merge-b .cm-deletedText": {
    background: "#ff000033"
  },
  ".cm-insertedLine, .cm-deletedLine, .cm-deletedLine del": {
    textDecoration: "none"
  },
  ".cm-deletedChunk": {
    paddingLeft: "6px",
    "& .cm-chunkButtons": {
      position: "absolute",
      insetInlineEnd: "5px"
    },
    "& button": {
      border: "none",
      cursor: "pointer",
      color: "white",
      margin: "0 2px",
      borderRadius: "3px",
      "&[name=accept]": { background: "#2a2" },
      "&[name=reject]": { background: "#d43" }
    }
  },
  ".cm-collapsedLines": {
    padding: "5px 5px 5px 10px",
    cursor: "pointer",
    "&:before": {
      content: '"⦚"',
      marginInlineEnd: "7px"
    },
    "&:after": {
      content: '"⦚"',
      marginInlineStart: "7px"
    }
  },
  "&light .cm-collapsedLines": {
    color: "#444",
    background: "linear-gradient(to bottom, transparent 0, #f3f3f3 30%, #f3f3f3 70%, transparent 100%)"
  },
  "&dark .cm-collapsedLines": {
    color: "#ddd",
    background: "linear-gradient(to bottom, transparent 0, #222 30%, #222 70%, transparent 100%)"
  },
  ".cm-changeGutter": { width: "3px", paddingLeft: "1px" },
  "&light.cm-merge-a .cm-changedLineGutter, &light .cm-deletedLineGutter": { background: "#e43" },
  "&dark.cm-merge-a .cm-changedLineGutter, &dark .cm-deletedLineGutter": { background: "#fa9" },
  "&light.cm-merge-b .cm-changedLineGutter": { background: "#2b2" },
  "&dark.cm-merge-b .cm-changedLineGutter": { background: "#8f8" },
  ".cm-inlineChangedLineGutter": { background: "#75d" }
}), me = /* @__PURE__ */ new ve(), j = /* @__PURE__ */ new ve();
class vt {
  /**
  Create a new merge view.
  */
  constructor(e) {
    this.revertDOM = null, this.revertToA = !1, this.revertToLeft = !1, this.measuring = -1, this.diffConf = e.diffConfig || nt;
    let t = [
      pe.low(Re),
      pt,
      gt,
      T,
      A.updateListener.of((h) => {
        this.measuring < 0 && (h.heightChanged || h.viewportChanged) && !h.transactions.some((c) => c.effects.some((u) => u.is(z))) && this.measure();
      })
    ], s = [C.of({
      side: "a",
      sibling: () => this.b,
      highlightChanges: e.highlightChanges !== !1,
      markGutter: e.gutter !== !1
    })];
    e.gutter !== !1 && s.push(V);
    let i = U.create({
      doc: e.a.doc,
      selection: e.a.selection,
      extensions: [
        e.a.extensions || [],
        A.editorAttributes.of({ class: "cm-merge-a" }),
        j.of(s),
        t
      ]
    }), r = [C.of({
      side: "b",
      sibling: () => this.a,
      highlightChanges: e.highlightChanges !== !1,
      markGutter: e.gutter !== !1
    })];
    e.gutter !== !1 && r.push(V);
    let l = U.create({
      doc: e.b.doc,
      selection: e.b.selection,
      extensions: [
        e.b.extensions || [],
        A.editorAttributes.of({ class: "cm-merge-b" }),
        j.of(r),
        t
      ]
    });
    this.chunks = L.build(i.doc, l.doc, this.diffConf);
    let o = [
      k.init(() => this.chunks),
      me.of(e.collapseUnchanged ? ue(e.collapseUnchanged) : [])
    ];
    i = i.update({ effects: R.appendConfig.of(o) }).state, l = l.update({ effects: R.appendConfig.of(o) }).state, this.dom = document.createElement("div"), this.dom.className = "cm-mergeView", this.editorDOM = this.dom.appendChild(document.createElement("div")), this.editorDOM.className = "cm-mergeViewEditors";
    let f = e.orientation || "a-b", d = document.createElement("div");
    d.className = "cm-mergeViewEditor";
    let a = document.createElement("div");
    a.className = "cm-mergeViewEditor", this.editorDOM.appendChild(f == "a-b" ? d : a), this.editorDOM.appendChild(f == "a-b" ? a : d), this.a = new A({
      state: i,
      parent: d,
      root: e.root,
      dispatchTransactions: (h) => this.dispatch(h, this.a)
    }), this.b = new A({
      state: l,
      parent: a,
      root: e.root,
      dispatchTransactions: (h) => this.dispatch(h, this.b)
    }), this.setupRevertControls(!!e.revertControls, e.revertControls == "b-to-a", e.renderRevertControl), e.parent && e.parent.appendChild(this.dom), this.scheduleMeasure();
  }
  dispatch(e, t) {
    if (e.some((s) => s.docChanged)) {
      let s = e[e.length - 1], i = e.reduce((l, o) => l.compose(o.changes), Fe.empty(e[0].startState.doc.length));
      this.chunks = t == this.a ? L.updateA(this.chunks, s.newDoc, this.b.state.doc, i, this.diffConf) : L.updateB(this.chunks, this.a.state.doc, s.newDoc, i, this.diffConf), t.update([...e, s.state.update({ effects: K.of(this.chunks) })]);
      let r = t == this.a ? this.b : this.a;
      r.update([r.state.update({ effects: K.of(this.chunks) })]), this.scheduleMeasure();
    } else
      t.update(e);
  }
  /**
  Reconfigure an existing merge view.
  */
  reconfigure(e) {
    if ("diffConfig" in e && (this.diffConf = e.diffConfig), "orientation" in e) {
      let r = e.orientation != "b-a";
      if (r != (this.editorDOM.firstChild == this.a.dom.parentNode)) {
        let l = this.a.dom.parentNode, o = this.b.dom.parentNode;
        l.remove(), o.remove(), this.editorDOM.insertBefore(r ? l : o, this.editorDOM.firstChild), this.editorDOM.appendChild(r ? o : l), this.revertToLeft = !this.revertToLeft, this.revertDOM && (this.revertDOM.textContent = "");
      }
    }
    if ("revertControls" in e || "renderRevertControl" in e) {
      let r = !!this.revertDOM, l = this.revertToA, o = this.renderRevert;
      "revertControls" in e && (r = !!e.revertControls, l = e.revertControls == "b-to-a"), "renderRevertControl" in e && (o = e.renderRevertControl), this.setupRevertControls(r, l, o);
    }
    let t = "highlightChanges" in e, s = "gutter" in e, i = "collapseUnchanged" in e;
    if (t || s || i) {
      let r = [], l = [];
      if (t || s) {
        let o = this.a.state.facet(C), f = s ? e.gutter !== !1 : o.markGutter, d = t ? e.highlightChanges !== !1 : o.highlightChanges;
        r.push(j.reconfigure([
          C.of({ side: "a", sibling: () => this.b, highlightChanges: d, markGutter: f }),
          f ? V : []
        ])), l.push(j.reconfigure([
          C.of({ side: "b", sibling: () => this.a, highlightChanges: d, markGutter: f }),
          f ? V : []
        ]));
      }
      if (i) {
        let o = me.reconfigure(e.collapseUnchanged ? ue(e.collapseUnchanged) : []);
        r.push(o), l.push(o);
      }
      this.a.dispatch({ effects: r }), this.b.dispatch({ effects: l });
    }
    this.scheduleMeasure();
  }
  setupRevertControls(e, t, s) {
    this.revertToA = t, this.revertToLeft = this.revertToA == (this.editorDOM.firstChild == this.a.dom.parentNode), this.renderRevert = s, !e && this.revertDOM ? (this.revertDOM.remove(), this.revertDOM = null) : e && !this.revertDOM ? (this.revertDOM = this.editorDOM.insertBefore(document.createElement("div"), this.editorDOM.firstChild.nextSibling), this.revertDOM.addEventListener("mousedown", (i) => this.revertClicked(i)), this.revertDOM.className = "cm-merge-revert") : this.revertDOM && (this.revertDOM.textContent = "");
  }
  scheduleMeasure() {
    if (this.measuring < 0) {
      let e = this.dom.ownerDocument.defaultView || window;
      this.measuring = e.requestAnimationFrame(() => {
        this.measuring = -1, this.measure();
      });
    }
  }
  measure() {
    ht(this.a, this.b, this.chunks), this.revertDOM && this.updateRevertButtons();
  }
  updateRevertButtons() {
    let e = this.revertDOM, t = e.firstChild, s = this.a.viewport, i = this.b.viewport;
    for (let r = 0; r < this.chunks.length; r++) {
      let l = this.chunks[r];
      if (l.fromA > s.to || l.fromB > i.to)
        break;
      if (l.fromA < s.from || l.fromB < i.from)
        continue;
      let o = this.a.lineBlockAt(l.fromA).top + "px";
      for (; t && +t.dataset.chunk < r; )
        t = ge(t);
      t && t.dataset.chunk == String(r) ? (t.style.top != o && (t.style.top = o), t = t.nextSibling) : e.insertBefore(this.renderRevertButton(o, r), t);
    }
    for (; t; )
      t = ge(t);
  }
  renderRevertButton(e, t) {
    let s;
    if (this.renderRevert)
      s = this.renderRevert();
    else {
      s = document.createElement("button");
      let i = this.a.state.phrase("Revert this chunk");
      s.setAttribute("aria-label", i), s.setAttribute("title", i), s.textContent = this.revertToLeft ? "⇜" : "⇝";
    }
    return s.style.top = e, s.setAttribute("data-chunk", String(t)), s;
  }
  revertClicked(e) {
    let t = e.target, s;
    for (; t && t.parentNode != this.revertDOM; )
      t = t.parentNode;
    if (t && (s = this.chunks[t.dataset.chunk])) {
      let [i, r, l, o, f, d] = this.revertToA ? [this.b, this.a, s.fromB, s.toB, s.fromA, s.toA] : [this.a, this.b, s.fromA, s.toA, s.fromB, s.toB], a = i.state.sliceDoc(l, Math.max(l, o - 1));
      l != o && d <= r.state.doc.length && (a += i.state.lineBreak), r.dispatch({
        changes: { from: f, to: Math.min(r.state.doc.length, d), insert: a },
        userEvent: "revert"
      }), e.preventDefault();
    }
  }
  /**
  Destroy this merge view.
  */
  destroy() {
    this.a.destroy(), this.b.destroy(), this.measuring > -1 && (this.dom.ownerDocument.defaultView || window).cancelAnimationFrame(this.measuring), this.dom.remove();
  }
}
function ge(n) {
  let e = n.nextSibling;
  return n.remove(), e;
}
const O = /* @__PURE__ */ new Map();
let wt = 1;
function Ct(n) {
  switch ((n ?? "plaintext").toLowerCase()) {
    case "json":
      return Qe();
    case "html":
    case "scriban":
      return Je();
    case "markdown":
      return $e();
    case "css":
      return _e();
    case "javascript":
      return Z();
    case "typescript":
      return Z({ typescript: !0 });
    case "xml":
      return Ke();
    default:
      return [];
  }
}
function At(n, e, t = {}) {
  const s = `sb-diff-${wt++}`, i = Ct(t.language), r = new vt({
    parent: n,
    a: {
      doc: t.original ?? "",
      extensions: [U.readOnly.of(!0), i]
    },
    b: {
      doc: t.suggested ?? "",
      extensions: [
        ze(),
        We.of([...qe, ...He]),
        i,
        A.updateListener.of((l) => {
          l.docChanged && e.invokeMethodAsync("OnSuggestedChanged", l.state.doc.toString());
        }),
        U.readOnly.of(!!t.readOnlySuggested)
      ]
    },
    highlightChanges: !0,
    gutter: !0
  });
  return n.setAttribute("dir", t.direction ?? "ltr"), O.set(s, r), s;
}
function kt(n) {
  O.get(n)?.destroy(), O.delete(n);
}
function bt(n) {
  return O.get(n)?.b.state.doc.toString() ?? "";
}
function Bt(n, e, t) {
  const s = O.get(n);
  s && (s.a.dispatch({ changes: { from: 0, to: s.a.state.doc.length, insert: e ?? "" } }), s.b.dispatch({ changes: { from: 0, to: s.b.state.doc.length, insert: t ?? "" } }));
}
function Mt(n, e) {
  const t = O.get(n);
  if (!t)
    return;
  const s = t.chunks;
  if (!s?.length)
    return;
  const i = t.b.state.selection.main.head, r = s.findIndex((o) => o.fromB > i), l = e === "next" ? s[r >= 0 ? r : 0] : s[Math.max(0, (r < 0 ? s.length : r) - 1)];
  l && (t.b.dispatch({ selection: { anchor: l.fromB }, scrollIntoView: !0 }), t.b.focus());
}
export {
  kt as destroyEditor,
  bt as getSuggested,
  Mt as goToChunk,
  At as initEditor,
  Bt as setValues
};
