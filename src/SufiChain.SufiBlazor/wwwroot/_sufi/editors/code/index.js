import { V as de, E as y, D as M, S as me, s as ge, h as De, F as pe, c as xe, R as ve, a as F, b as f, W as Oe, l as qe, g as ye, d as S, e as $e, f as Be, P as Ne, i as _e, j as ze, k as Y, m as Ke, r as Ve, C as A, n as be, o as je, p as He, q as Qe, t as Je, u as Ge, v as Ue, w as Ye, x as Ze, y as le, z as Xe, A as et, B as tt, G as it, H as st } from "../chunks/index-DrKr9VzA.js";
class oe {
  constructor(e, t, s) {
    this.from = e, this.to = t, this.diagnostic = s;
  }
}
class P {
  constructor(e, t, s) {
    this.diagnostics = e, this.panel = t, this.selected = s;
  }
  static init(e, t, s) {
    let n = s.facet(C).markerFilter;
    n && (e = n(e, s));
    let r = e.slice().sort((x, m) => x.from - m.from || x.to - m.to), l = new ve(), o = [], a = 0, c = s.doc.iter(), h = 0, d = s.doc.length;
    for (let x = 0; ; ) {
      let m = x == r.length ? null : r[x];
      if (!m && !o.length)
        break;
      let v, p;
      if (o.length)
        v = a, p = o.reduce((u, w) => Math.min(u, w.to), m && m.from > v ? m.from : 1e8);
      else {
        if (v = m.from, v > d)
          break;
        p = m.to, o.push(m), x++;
      }
      for (; x < r.length; ) {
        let u = r[x];
        if (u.from == v && (u.to > u.from || u.to == v))
          o.push(u), x++, p = Math.min(u.to, p);
        else {
          p = Math.min(u.from, p);
          break;
        }
      }
      p = Math.min(p, d);
      let R = !1;
      if (o.some((u) => u.from == v && (u.to == p || p == d)) && (R = v == p, !R && p - v < 10)) {
        let u = v - (h + c.value.length);
        u > 0 && (c.next(u), h = v);
        for (let w = v; ; ) {
          if (w >= p) {
            R = !0;
            break;
          }
          if (!c.lineBreak && h + c.value.length > w)
            break;
          w = h + c.value.length, h += c.value.length, c.next();
        }
      }
      let ne = xt(o);
      if (R)
        l.add(v, v, M.widget({
          widget: new dt(ne),
          diagnostics: o.slice()
        }));
      else {
        let u = o.reduce((w, re) => re.markClass ? w + " " + re.markClass : w, "");
        l.add(v, p, M.mark({
          class: "cm-lintRange cm-lintRange-" + ne + u,
          diagnostics: o.slice(),
          inclusiveEnd: o.some((w) => w.to > p)
        }));
      }
      if (a = p, a == d)
        break;
      for (let u = 0; u < o.length; u++)
        o[u].to <= a && o.splice(u--, 1);
    }
    let g = l.finish();
    return new P(g, t, D(g));
  }
}
function D(i, e = null, t = 0) {
  let s = null;
  return i.between(t, 1e9, (n, r, { spec: l }) => {
    if (!(e && l.diagnostics.indexOf(e) < 0))
      if (!s)
        s = new oe(n, r, e || l.diagnostics[0]);
      else {
        if (l.diagnostics.indexOf(s.diagnostic) < 0)
          return !1;
        s = new oe(s.from, r, s.diagnostic);
      }
  }), s;
}
function nt(i, e) {
  let t = e.pos, s = e.end || t, n = i.state.facet(C).hideOn(i, t, s);
  if (n != null)
    return n;
  let r = i.startState.doc.lineAt(e.pos);
  return !!(i.effects.some((l) => l.is(X)) || i.changes.touchesRange(r.from, Math.max(r.to, s)));
}
function rt(i, e) {
  return i.field(k, !1) ? e : e.concat(F.appendConfig.of(Me));
}
function lt(i, e) {
  return {
    effects: rt(i, [X.of(e)])
  };
}
const X = /* @__PURE__ */ F.define(), ke = /* @__PURE__ */ F.define(), Se = /* @__PURE__ */ F.define(), k = /* @__PURE__ */ me.define({
  create() {
    return new P(M.none, null, null);
  },
  update(i, e) {
    if (e.docChanged && i.diagnostics.size) {
      let t = i.diagnostics.map(e.changes), s = null, n = i.panel;
      if (i.selected) {
        let r = e.changes.mapPos(i.selected.from, 1);
        s = D(t, i.selected.diagnostic, r) || D(t, null, r);
      }
      !t.size && n && e.state.facet(C).autoPanel && (n = null), i = new P(t, n, s);
    }
    for (let t of e.effects)
      if (t.is(X)) {
        let s = e.state.facet(C).autoPanel ? t.value.length ? K.open : null : i.panel;
        i = P.init(t.value, s, e.state);
      } else t.is(ke) ? i = new P(i.diagnostics, t.value ? K.open : null, i.selected) : t.is(Se) && (i = new P(i.diagnostics, i.panel, t.value));
    return i;
  },
  provide: (i) => [
    ge.from(i, (e) => e.panel),
    y.decorations.from(i, (e) => e.diagnostics)
  ]
}), ot = /* @__PURE__ */ M.mark({ class: "cm-lintRange cm-lintRange-active" });
function at(i, e, t) {
  let { diagnostics: s } = i.state.field(k), n, r = -1, l = -1;
  s.between(e - (t < 0 ? 1 : 0), e + (t > 0 ? 1 : 0), (a, c, { spec: h }) => {
    if (e >= a && e <= c && (a == c || (e > a || t > 0) && (e < c || t < 0)))
      return n = h.diagnostics, r = a, l = c, !1;
  });
  let o = i.state.facet(C).tooltipFilter;
  return n && o && (n = o(n, i.state)), n ? {
    pos: r,
    end: l,
    above: !0,
    create() {
      return { dom: ct(i, n) };
    }
  } : null;
}
function ct(i, e) {
  return f("ul", { class: "cm-tooltip-lint" }, e.map((t) => Ce(i, t, !1)));
}
const ae = (i) => {
  let e = i.state.field(k, !1);
  return !e || !e.panel ? !1 : (i.dispatch({ effects: ke.of(!1) }), !0);
}, ht = /* @__PURE__ */ de.fromClass(class {
  constructor(i) {
    this.view = i, this.timeout = -1, this.set = !0;
    let { delay: e } = i.state.facet(C);
    this.lintTime = Date.now() + e, this.run = this.run.bind(this), this.timeout = setTimeout(this.run, e);
  }
  run() {
    clearTimeout(this.timeout);
    let i = Date.now();
    if (i < this.lintTime - 10)
      this.timeout = setTimeout(this.run, this.lintTime - i);
    else {
      this.set = !1;
      let { state: e } = this.view, { sources: t } = e.facet(C);
      t.length && ut(t.map((s) => Promise.resolve(s(this.view))), (s) => {
        this.view.state.doc == e.doc && this.view.dispatch(lt(this.view.state, s.reduce((n, r) => n.concat(r))));
      }, (s) => {
        qe(this.view.state, s);
      });
    }
  }
  update(i) {
    let e = i.state.facet(C);
    (i.docChanged || e != i.startState.facet(C) || e.needsRefresh && e.needsRefresh(i)) && (this.lintTime = Date.now() + e.delay, this.set || (this.set = !0, this.timeout = setTimeout(this.run, e.delay)));
  }
  force() {
    this.set && (this.lintTime = Date.now(), this.run());
  }
  destroy() {
    clearTimeout(this.timeout);
  }
});
function ut(i, e, t) {
  let s = [], n = -1;
  for (let r of i)
    r.then((l) => {
      s.push(l), clearTimeout(n), s.length == i.length ? e(s) : n = setTimeout(() => e(s), 200);
    }, t);
}
const C = /* @__PURE__ */ pe.define({
  combine(i) {
    return {
      sources: i.map((e) => e.source).filter((e) => e != null),
      ...xe(i.map((e) => e.config), {
        delay: 750,
        markerFilter: null,
        tooltipFilter: null,
        needsRefresh: null,
        hideOn: () => null
      }, {
        delay: Math.max,
        markerFilter: ce,
        tooltipFilter: ce,
        needsRefresh: (e, t) => e ? t ? (s) => e(s) || t(s) : e : t,
        hideOn: (e, t) => e ? t ? (s, n, r) => e(s, n, r) || t(s, n, r) : e : t,
        autoPanel: (e, t) => e || t
      })
    };
  }
});
function ce(i, e) {
  return i ? e ? (t, s) => e(i(t, s), s) : i : e;
}
function ft(i, e = {}) {
  return [
    C.of({ source: i, config: e }),
    ht,
    Me
  ];
}
function we(i) {
  let e = [];
  if (i)
    e: for (let { name: t } of i) {
      for (let s = 0; s < t.length; s++) {
        let n = t[s];
        if (/[a-zA-Z]/.test(n) && !e.some((r) => r.toLowerCase() == n.toLowerCase())) {
          e.push(n);
          continue e;
        }
      }
      e.push("");
    }
  return e;
}
function Ce(i, e, t) {
  var s;
  let n = t ? we(e.actions) : [];
  return f("li", { class: "cm-diagnostic cm-diagnostic-" + e.severity }, f("span", { class: "cm-diagnosticText" }, e.renderMessage ? e.renderMessage(i) : e.message), (s = e.actions) === null || s === void 0 ? void 0 : s.map((r, l) => {
    let o = !1, a = (x) => {
      if (x.preventDefault(), o)
        return;
      o = !0;
      let m = D(i.state.field(k).diagnostics, e);
      m && r.apply(i, m.from, m.to);
    }, { name: c } = r, h = n[l] ? c.indexOf(n[l]) : -1, d = h < 0 ? c : [
      c.slice(0, h),
      f("u", c.slice(h, h + 1)),
      c.slice(h + 1)
    ], g = r.markClass ? " " + r.markClass : "";
    return f("button", {
      type: "button",
      class: "cm-diagnosticAction" + g,
      onclick: a,
      onmousedown: a,
      "aria-label": ` Action: ${c}${h < 0 ? "" : ` (access key "${n[l]})"`}.`
    }, d);
  }), e.source && f("div", { class: "cm-diagnosticSource" }, e.source));
}
class dt extends Oe {
  constructor(e) {
    super(), this.sev = e;
  }
  eq(e) {
    return e.sev == this.sev;
  }
  toDOM() {
    return f("span", { class: "cm-lintPoint cm-lintPoint-" + this.sev });
  }
}
class he {
  constructor(e, t) {
    this.diagnostic = t, this.id = "item_" + Math.floor(Math.random() * 4294967295).toString(16), this.dom = Ce(e, t, !0), this.dom.id = this.id, this.dom.setAttribute("role", "option");
  }
}
class K {
  constructor(e) {
    this.view = e, this.items = [];
    let t = (n) => {
      if (!(n.ctrlKey || n.altKey || n.metaKey)) {
        if (n.keyCode == 27)
          ae(this.view), this.view.focus();
        else if (n.keyCode == 38 || n.keyCode == 33)
          this.moveSelection((this.selectedIndex - 1 + this.items.length) % this.items.length);
        else if (n.keyCode == 40 || n.keyCode == 34)
          this.moveSelection((this.selectedIndex + 1) % this.items.length);
        else if (n.keyCode == 36)
          this.moveSelection(0);
        else if (n.keyCode == 35)
          this.moveSelection(this.items.length - 1);
        else if (n.keyCode == 13)
          this.view.focus();
        else if (n.keyCode >= 65 && n.keyCode <= 90 && this.selectedIndex >= 0) {
          let { diagnostic: r } = this.items[this.selectedIndex], l = we(r.actions);
          for (let o = 0; o < l.length; o++)
            if (l[o].toUpperCase().charCodeAt(0) == n.keyCode) {
              let a = D(this.view.state.field(k).diagnostics, r);
              a && r.actions[o].apply(e, a.from, a.to);
            }
        } else
          return;
        n.preventDefault();
      }
    }, s = (n) => {
      for (let r = 0; r < this.items.length; r++)
        this.items[r].dom.contains(n.target) && this.moveSelection(r);
    };
    this.list = f("ul", {
      tabIndex: 0,
      role: "listbox",
      "aria-label": this.view.state.phrase("Diagnostics"),
      onkeydown: t,
      onclick: s
    }), this.dom = f("div", { class: "cm-panel-lint" }, this.list, f("button", {
      type: "button",
      name: "close",
      "aria-label": this.view.state.phrase("close"),
      onclick: () => ae(this.view)
    }, "×")), this.update();
  }
  get selectedIndex() {
    let e = this.view.state.field(k).selected;
    if (!e)
      return -1;
    for (let t = 0; t < this.items.length; t++)
      if (this.items[t].diagnostic == e.diagnostic)
        return t;
    return -1;
  }
  update() {
    let { diagnostics: e, selected: t } = this.view.state.field(k), s = 0, n = !1, r = null, l = /* @__PURE__ */ new Set();
    for (e.between(0, this.view.state.doc.length, (o, a, { spec: c }) => {
      for (let h of c.diagnostics) {
        if (l.has(h))
          continue;
        l.add(h);
        let d = -1, g;
        for (let x = s; x < this.items.length; x++)
          if (this.items[x].diagnostic == h) {
            d = x;
            break;
          }
        d < 0 ? (g = new he(this.view, h), this.items.splice(s, 0, g), n = !0) : (g = this.items[d], d > s && (this.items.splice(s, d - s), n = !0)), t && g.diagnostic == t.diagnostic ? g.dom.hasAttribute("aria-selected") || (g.dom.setAttribute("aria-selected", "true"), r = g) : g.dom.hasAttribute("aria-selected") && g.dom.removeAttribute("aria-selected"), s++;
      }
    }); s < this.items.length && !(this.items.length == 1 && this.items[0].diagnostic.from < 0); )
      n = !0, this.items.pop();
    this.items.length == 0 && (this.items.push(new he(this.view, {
      from: -1,
      to: -1,
      severity: "info",
      message: this.view.state.phrase("No diagnostics")
    })), n = !0), r ? (this.list.setAttribute("aria-activedescendant", r.id), this.view.requestMeasure({
      key: this,
      read: () => ({ sel: r.dom.getBoundingClientRect(), panel: this.list.getBoundingClientRect() }),
      write: ({ sel: o, panel: a }) => {
        let c = a.height / this.list.offsetHeight;
        o.top < a.top ? this.list.scrollTop -= (a.top - o.top) / c : o.bottom > a.bottom && (this.list.scrollTop += (o.bottom - a.bottom) / c);
      }
    })) : this.selectedIndex < 0 && this.list.removeAttribute("aria-activedescendant"), n && this.sync();
  }
  sync() {
    let e = this.list.firstChild;
    function t() {
      let s = e;
      e = s.nextSibling, s.remove();
    }
    for (let s of this.items)
      if (s.dom.parentNode == this.list) {
        for (; e != s.dom; )
          t();
        e = s.dom.nextSibling;
      } else
        this.list.insertBefore(s.dom, e);
    for (; e; )
      t();
  }
  moveSelection(e) {
    if (this.selectedIndex < 0)
      return;
    let t = this.view.state.field(k), s = D(t.diagnostics, this.items[e].diagnostic);
    s && this.view.dispatch({
      selection: { anchor: s.from, head: s.to },
      scrollIntoView: !0,
      effects: Se.of(s)
    });
  }
  static open(e) {
    return new K(e);
  }
}
function mt(i, e = 'viewBox="0 0 40 40"') {
  return `url('data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" ${e}>${encodeURIComponent(i)}</svg>')`;
}
function N(i) {
  return mt(`<path d="m0 2.5 l2 -1.5 l1 0 l2 1.5 l1 0" stroke="${i}" fill="none" stroke-width=".7"/>`, 'width="6" height="3"');
}
const gt = /* @__PURE__ */ y.baseTheme({
  ".cm-diagnostic": {
    padding: "3px 6px 3px 8px",
    marginLeft: "-1px",
    display: "block",
    whiteSpace: "pre-wrap"
  },
  ".cm-diagnostic-error": { borderLeft: "5px solid #d11" },
  ".cm-diagnostic-warning": { borderLeft: "5px solid orange" },
  ".cm-diagnostic-info": { borderLeft: "5px solid #999" },
  ".cm-diagnostic-hint": { borderLeft: "5px solid #66d" },
  ".cm-diagnosticAction": {
    font: "inherit",
    border: "none",
    padding: "2px 4px",
    backgroundColor: "#444",
    color: "white",
    borderRadius: "3px",
    marginLeft: "8px",
    cursor: "pointer"
  },
  ".cm-diagnosticSource": {
    fontSize: "70%",
    opacity: 0.7
  },
  ".cm-lintRange": {
    backgroundPosition: "left bottom",
    backgroundRepeat: "repeat-x",
    paddingBottom: "0.7px"
  },
  ".cm-lintRange-error": { backgroundImage: /* @__PURE__ */ N("#f11") },
  ".cm-lintRange-warning": { backgroundImage: /* @__PURE__ */ N("orange") },
  ".cm-lintRange-info": { backgroundImage: /* @__PURE__ */ N("#999") },
  ".cm-lintRange-hint": { backgroundImage: /* @__PURE__ */ N("#66d") },
  ".cm-lintRange-active": { backgroundColor: "#ffdd9980" },
  ".cm-tooltip-lint": {
    padding: 0,
    margin: 0
  },
  ".cm-lintPoint": {
    position: "relative",
    "&:after": {
      content: '""',
      position: "absolute",
      bottom: 0,
      left: "-2px",
      borderLeft: "3px solid transparent",
      borderRight: "3px solid transparent",
      borderBottom: "4px solid #d11"
    }
  },
  ".cm-lintPoint-warning": {
    "&:after": { borderBottomColor: "orange" }
  },
  ".cm-lintPoint-info": {
    "&:after": { borderBottomColor: "#999" }
  },
  ".cm-lintPoint-hint": {
    "&:after": { borderBottomColor: "#66d" }
  },
  ".cm-panel.cm-panel-lint": {
    position: "relative",
    "& ul": {
      maxHeight: "100px",
      overflowY: "auto",
      "& [aria-selected]": {
        backgroundColor: "#ddd",
        "& u": { textDecoration: "underline" }
      },
      "&:focus [aria-selected]": {
        background_fallback: "#bdf",
        backgroundColor: "Highlight",
        color_fallback: "white",
        color: "HighlightText"
      },
      "& u": { textDecoration: "none" },
      padding: 0,
      margin: 0
    },
    "& [name=close]": {
      position: "absolute",
      top: "0",
      right: "2px",
      background: "inherit",
      border: "none",
      font: "inherit",
      padding: 0,
      margin: 0
    }
  },
  "&dark .cm-lintRange-active": { backgroundColor: "#86714a80" },
  "&dark .cm-panel.cm-panel-lint ul": {
    "& [aria-selected]": {
      backgroundColor: "#2e343e"
    }
  }
});
function pt(i) {
  return i == "error" ? 4 : i == "warning" ? 3 : i == "info" ? 2 : 1;
}
function xt(i) {
  let e = "hint", t = 1;
  for (let s of i) {
    let n = pt(s.severity);
    n > t && (t = n, e = s.severity);
  }
  return e;
}
const vt = /* @__PURE__ */ De(at, { hideOn: nt }), Me = [
  k,
  /* @__PURE__ */ y.decorations.compute([k], (i) => {
    let { selected: e, panel: t } = i.field(k);
    return !e || !t || e.from == e.to ? M.none : M.set([
      ot.range(e.from, e.to)
    ]);
  }),
  vt,
  gt
], ue = typeof String.prototype.normalize == "function" ? (i) => i.normalize("NFKD") : (i) => i;
class q {
  /**
  Create a text cursor. The query is the search string, `from` to
  `to` provides the region to search.
  
  When `normalize` is given, it will be called, on both the query
  string and the content it is matched against, before comparing.
  You can, for example, create a case-insensitive search by
  passing `s => s.toLowerCase()`.
  
  Text is always normalized with
  [`.normalize("NFKD")`](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/String/normalize)
  (when supported).
  */
  constructor(e, t, s = 0, n = e.length, r, l) {
    this.test = l, this.value = { from: 0, to: 0, precise: !1 }, this.done = !1, this.matches = [], this.buffer = "", this.bufferPos = 0, this.iter = e.iterRange(s, n), this.bufferStart = s, this.normalize = r ? (o) => r(ue(o)) : ue, this.query = this.normalize(t);
  }
  peek() {
    if (this.bufferPos == this.buffer.length) {
      if (this.bufferStart += this.buffer.length, this.iter.next(), this.iter.done)
        return -1;
      this.bufferPos = 0, this.buffer = this.iter.value;
    }
    return _e(this.buffer, this.bufferPos);
  }
  /**
  Look for the next match. Updates the iterator's
  [`value`](https://codemirror.net/6/docs/ref/#search.SearchCursor.value) and
  [`done`](https://codemirror.net/6/docs/ref/#search.SearchCursor.done) properties. Should be called
  at least once before using the cursor.
  */
  next() {
    for (; this.matches.length; )
      this.matches.pop();
    return this.nextOverlapping();
  }
  /**
  The `next` method will ignore matches that partially overlap a
  previous match. This method behaves like `next`, but includes
  such matches.
  */
  nextOverlapping() {
    for (; ; ) {
      let e = this.peek();
      if (e < 0)
        return this.done = !0, this;
      let t = Ke(e), s = this.bufferStart + this.bufferPos;
      this.bufferPos += ze(e);
      let n = this.normalize(t);
      if (n.length)
        for (let r = 0, l = s, o = !0; ; r++) {
          let a = n.charCodeAt(r), c = this.match(a, l, o, this.bufferPos + this.bufferStart, r == n.length - 1);
          if (c)
            return this.value = c, this;
          if (r == n.length - 1)
            break;
          o && r < t.length && t.charCodeAt(r) == a ? l++ : o = !1;
        }
    }
  }
  match(e, t, s, n, r) {
    let l = null;
    for (let o = 0; o < this.matches.length; ) {
      let a = this.matches[o], c = !1;
      this.query.charCodeAt(a.index) == e && (a.index == this.query.length - 1 ? l = { from: a.from, to: n, precise: r && a.precise } : (a.index++, c = !0)), c ? o++ : this.matches.splice(o, 1);
    }
    return this.query.charCodeAt(0) == e && (this.query.length == 1 ? l = { from: t, to: n, precise: s && r } : this.matches.push({ from: t, index: 1, precise: s })), l && this.test && !this.test(l.from, l.to, this.buffer, this.bufferStart) && (l = null), l;
  }
}
typeof Symbol < "u" && (q.prototype[Symbol.iterator] = function() {
  return this;
});
const Le = { from: -1, to: -1, match: /* @__PURE__ */ /.*/.exec(""), precise: !0 }, ee = "gm" + (/x/.unicode == null ? "" : "u");
class Re {
  /**
  Create a cursor that will search the given range in the given
  document. `query` should be the raw pattern (as you'd pass it to
  `new RegExp`).
  */
  constructor(e, t, s, n = 0, r = e.length) {
    if (this.text = e, this.to = r, this.curLine = "", this.done = !1, this.value = Le, /\\[sWDnr]|\n|\r|\[\^/.test(t))
      return new Ae(e, t, s, n, r);
    this.re = new RegExp(t, ee + (s?.ignoreCase ? "i" : "")), this.test = s?.test, this.iter = e.iter();
    let l = e.lineAt(n);
    this.curLineStart = l.from, this.matchPos = V(e, n), this.getLine(this.curLineStart);
  }
  getLine(e) {
    this.iter.next(e), this.iter.lineBreak ? this.curLine = "" : (this.curLine = this.iter.value, this.curLineStart + this.curLine.length > this.to && (this.curLine = this.curLine.slice(0, this.to - this.curLineStart)), this.iter.next());
  }
  nextLine() {
    this.curLineStart = this.curLineStart + this.curLine.length + 1, this.curLineStart > this.to ? this.curLine = "" : this.getLine(0);
  }
  /**
  Move to the next match, if there is one.
  */
  next() {
    for (let e = this.matchPos - this.curLineStart; ; ) {
      this.re.lastIndex = e;
      let t = this.matchPos <= this.to && this.re.exec(this.curLine);
      if (t) {
        let s = this.curLineStart + t.index, n = s + t[0].length;
        if (this.matchPos = V(this.text, n + (s == n ? 1 : 0)), s == this.curLineStart + this.curLine.length && this.nextLine(), (s < n || s > this.value.to) && (!this.test || this.test(s, n, t)))
          return this.value = { from: s, to: n, precise: !0, match: t }, this;
        e = this.matchPos - this.curLineStart;
      } else if (this.curLineStart + this.curLine.length < this.to)
        this.nextLine(), e = 0;
      else
        return this.done = !0, this;
    }
  }
}
const G = /* @__PURE__ */ new WeakMap();
class W {
  constructor(e, t) {
    this.from = e, this.text = t;
  }
  get to() {
    return this.from + this.text.length;
  }
  static get(e, t, s) {
    let n = G.get(e);
    if (!n || n.from >= s || n.to <= t) {
      let o = new W(t, e.sliceString(t, s));
      return G.set(e, o), o;
    }
    if (n.from == t && n.to == s)
      return n;
    let { text: r, from: l } = n;
    return l > t && (r = e.sliceString(t, l) + r, l = t), n.to < s && (r += e.sliceString(n.to, s)), G.set(e, new W(l, r)), new W(t, r.slice(t - l, s - l));
  }
}
class Ae {
  constructor(e, t, s, n, r) {
    this.text = e, this.to = r, this.done = !1, this.value = Le, this.matchPos = V(e, n), this.re = new RegExp(t, ee + (s?.ignoreCase ? "i" : "")), this.test = s?.test, this.flat = W.get(e, n, this.chunkEnd(
      n + 5e3
      /* Chunk.Base */
    ));
  }
  chunkEnd(e) {
    return e >= this.to ? this.to : this.text.lineAt(e).to;
  }
  next() {
    for (; ; ) {
      let e = this.re.lastIndex = this.matchPos - this.flat.from, t = this.re.exec(this.flat.text);
      if (t && !t[0] && t.index == e && (this.re.lastIndex = e + 1, t = this.re.exec(this.flat.text)), t) {
        let s = this.flat.from + t.index, n = s + t[0].length;
        if ((this.flat.to >= this.to || t.index + t[0].length <= this.flat.text.length - 10) && (!this.test || this.test(s, n, t)))
          return this.value = { from: s, to: n, precise: !0, match: t }, this.matchPos = V(this.text, n + (s == n ? 1 : 0)), this;
      }
      if (this.flat.to == this.to)
        return this.done = !0, this;
      this.flat = W.get(this.text, this.flat.from, this.chunkEnd(this.flat.from + this.flat.text.length * 2));
    }
  }
}
typeof Symbol < "u" && (Re.prototype[Symbol.iterator] = Ae.prototype[Symbol.iterator] = function() {
  return this;
});
function yt(i) {
  try {
    return new RegExp(i, ee), !0;
  } catch {
    return !1;
  }
}
function V(i, e) {
  if (e >= i.length)
    return e;
  let t = i.lineAt(e), s;
  for (; e < t.to && (s = t.text.charCodeAt(e - t.from)) >= 56320 && s < 57344; )
    e++;
  return e;
}
const bt = (i) => {
  let e = $e(i, "cm-goto-line");
  if (e) {
    let l = e.dom.querySelector("input[type=text]");
    return l && l.select(), !0;
  }
  let { state: t } = i, s = String(t.doc.lineAt(i.state.selection.main.head).number), { close: n, result: r } = Be(i, {
    class: "cm-goto-line",
    label: t.phrase("Go to line"),
    input: { type: "text", name: "line", value: s },
    focus: !0,
    submitLabel: t.phrase("go")
  });
  return r.then((l) => {
    let o = l && /^([+-])?(\d+)?(:\d+)?(%)?$/.exec(l.elements.line.value);
    if (!o) {
      i.dispatch({ effects: n });
      return;
    }
    let a = t.doc.lineAt(t.selection.main.head), [, c, h, d, g] = o, x = d ? +d.slice(1) : 0, m = h ? +h : a.number;
    if (h && g) {
      let R = m / 100;
      c && (R = R * (c == "-" ? -1 : 1) + a.number / t.doc.lines), m = Math.round(t.doc.lines * R);
    } else h && c && (m = m * (c == "-" ? -1 : 1) + a.number);
    let v = t.doc.line(Math.max(1, Math.min(t.doc.lines, m))), p = S.cursor(v.from + Math.max(0, Math.min(x, v.length)));
    i.dispatch({
      effects: [n, y.scrollIntoView(p.from, { y: "center" })],
      selection: p
    });
  }), !0;
}, kt = ({ state: i, dispatch: e }) => {
  let { selection: t } = i, s = S.create(t.ranges.map((n) => i.wordAt(n.head) || S.cursor(n.head)), t.mainIndex);
  return s.eq(t) ? !1 : (e(i.update({ selection: s })), !0);
};
function St(i, e) {
  let { main: t, ranges: s } = i.selection, n = i.wordAt(t.head), r = n && n.from == t.from && n.to == t.to;
  for (let l = !1, o = new q(i.doc, e, s[s.length - 1].to); ; )
    if (o.next(), o.done) {
      if (l)
        return null;
      o = new q(i.doc, e, 0, Math.max(0, s[s.length - 1].from - 1)), l = !0;
    } else {
      if (l && s.some((a) => a.from == o.value.from))
        continue;
      if (r) {
        let a = i.wordAt(o.value.from);
        if (!a || a.from != o.value.from || a.to != o.value.to)
          continue;
      }
      return o.value;
    }
}
const wt = ({ state: i, dispatch: e }) => {
  let { ranges: t } = i.selection;
  if (t.some((r) => r.from === r.to))
    return kt({ state: i, dispatch: e });
  let s = i.sliceDoc(t[0].from, t[0].to);
  if (i.selection.ranges.some((r) => i.sliceDoc(r.from, r.to) != s))
    return !1;
  let n = St(i, s);
  return n ? (e(i.update({
    selection: i.selection.addRange(S.range(n.from, n.to), !1),
    effects: y.scrollIntoView(n.to)
  })), !0) : !1;
}, O = /* @__PURE__ */ pe.define({
  combine(i) {
    return xe(i, {
      top: !1,
      caseSensitive: !1,
      literal: !1,
      regexp: !1,
      wholeWord: !1,
      createPanel: (e) => new qt(e),
      scrollToMatch: (e) => y.scrollIntoView(e)
    });
  }
});
class Ee {
  /**
  Create a query object.
  */
  constructor(e) {
    this.search = e.search, this.caseSensitive = !!e.caseSensitive, this.literal = !!e.literal, this.regexp = !!e.regexp, this.replace = e.replace || "", this.valid = !!this.search && (!this.regexp || yt(this.search)), this.unquoted = this.unquote(this.search), this.wholeWord = !!e.wholeWord, this.test = e.test;
  }
  /**
  @internal
  */
  unquote(e) {
    return this.literal ? e : e.replace(/\\([nrt\\])/g, (t, s) => s == "n" ? `
` : s == "r" ? "\r" : s == "t" ? "	" : "\\");
  }
  /**
  Compare this query to another query.
  */
  eq(e) {
    return this.search == e.search && this.replace == e.replace && this.caseSensitive == e.caseSensitive && this.regexp == e.regexp && this.wholeWord == e.wholeWord && this.test == e.test;
  }
  /**
  @internal
  */
  create() {
    return this.regexp ? new Et(this) : new Lt(this);
  }
  /**
  Get a search cursor for this query, searching through the given
  range in the given state.
  */
  getCursor(e, t = 0, s) {
    let n = e.doc ? e : Y.create({ doc: e });
    return s == null && (s = n.doc.length), this.regexp ? I(this, n, t, s) : T(this, n, t, s);
  }
}
class Pe {
  constructor(e) {
    this.spec = e;
  }
}
function Ct(i, e, t) {
  return (s, n, r, l) => {
    if (t && !t(s, n, r, l))
      return !1;
    let o = s >= l && n <= l + r.length ? r.slice(s - l, n - l) : e.doc.sliceString(s, n);
    return i(o, e, s, n);
  };
}
function T(i, e, t, s) {
  let n;
  return i.wholeWord && (n = Mt(e.doc, e.charCategorizer(e.selection.main.head))), i.test && (n = Ct(i.test, e, n)), new q(e.doc, i.unquoted, t, s, i.caseSensitive ? void 0 : (r) => r.toLowerCase(), n);
}
function Mt(i, e) {
  return (t, s, n, r) => ((r > t || r + n.length < s) && (r = Math.max(0, t - 2), n = i.sliceString(r, Math.min(i.length, s + 2))), (e(j(n, t - r)) != A.Word || e(H(n, t - r)) != A.Word) && (e(H(n, s - r)) != A.Word || e(j(n, s - r)) != A.Word));
}
class Lt extends Pe {
  constructor(e) {
    super(e);
  }
  nextMatch(e, t, s) {
    let n = T(this.spec, e, s, e.doc.length).nextOverlapping();
    if (n.done) {
      let r = Math.min(e.doc.length, t + this.spec.unquoted.length);
      n = T(this.spec, e, 0, r).nextOverlapping();
    }
    return n.done || n.value.from == t && n.value.to == s ? null : n.value;
  }
  // Searching in reverse is, rather than implementing an inverted search
  // cursor, done by scanning chunk after chunk forward.
  prevMatchInRange(e, t, s) {
    for (let n = s; ; ) {
      let r = Math.max(t, n - 1e4 - this.spec.unquoted.length), l = T(this.spec, e, r, n), o = null;
      for (; !l.nextOverlapping().done; )
        o = l.value;
      if (o)
        return o;
      if (r == t)
        return null;
      n -= 1e4;
    }
  }
  prevMatch(e, t, s) {
    let n = this.prevMatchInRange(e, 0, t);
    return n || (n = this.prevMatchInRange(e, Math.max(0, s - this.spec.unquoted.length), e.doc.length)), n && (n.from != t || n.to != s) ? n : null;
  }
  getReplacement(e) {
    return this.spec.unquote(this.spec.replace);
  }
  matchAll(e, t) {
    let s = T(this.spec, e, 0, e.doc.length), n = [];
    for (; !s.next().done; ) {
      if (n.length >= t)
        return null;
      n.push(s.value);
    }
    return n;
  }
  highlight(e, t, s, n) {
    let r = T(this.spec, e, Math.max(0, t - this.spec.unquoted.length), Math.min(s + this.spec.unquoted.length, e.doc.length));
    for (; !r.next().done; )
      n(r.value.from, r.value.to);
  }
}
function Rt(i, e, t) {
  return (s, n, r) => (!t || t(s, n, r)) && i(r[0], e, s, n);
}
function I(i, e, t, s) {
  let n;
  return i.wholeWord && (n = At(e.charCategorizer(e.selection.main.head))), i.test && (n = Rt(i.test, e, n)), new Re(e.doc, i.search, { ignoreCase: !i.caseSensitive, test: n }, t, s);
}
function j(i, e) {
  return i.slice(be(i, e, !1), e);
}
function H(i, e) {
  return i.slice(e, be(i, e));
}
function At(i) {
  return (e, t, s) => !s[0].length || (i(j(s.input, s.index)) != A.Word || i(H(s.input, s.index)) != A.Word) && (i(H(s.input, s.index + s[0].length)) != A.Word || i(j(s.input, s.index + s[0].length)) != A.Word);
}
class Et extends Pe {
  nextMatch(e, t, s) {
    let n = I(this.spec, e, s, e.doc.length).next();
    return n.done && (n = I(this.spec, e, 0, t).next()), n.done ? null : n.value;
  }
  prevMatchInRange(e, t, s) {
    for (let n = 1; ; n++) {
      let r = Math.max(
        t,
        s - n * 1e4
        /* FindPrev.ChunkSize */
      ), l = I(this.spec, e, r, s), o = null;
      for (; !l.next().done; )
        o = l.value;
      if (o && (r == t || o.from > r + 10))
        return o;
      if (r == t)
        return null;
    }
  }
  prevMatch(e, t, s) {
    return this.prevMatchInRange(e, 0, t) || this.prevMatchInRange(e, s, e.doc.length);
  }
  getReplacement(e) {
    return this.spec.unquote(this.spec.replace).replace(/\$([$&]|\d+)/g, (t, s) => {
      if (s == "&")
        return e.match[0];
      if (s == "$")
        return "$";
      for (let n = s.length; n > 0; n--) {
        let r = +s.slice(0, n);
        if (r > 0 && r < e.match.length)
          return e.match[r] + s.slice(n);
      }
      return t;
    });
  }
  matchAll(e, t) {
    let s = I(this.spec, e, 0, e.doc.length), n = [];
    for (; !s.next().done; ) {
      if (n.length >= t)
        return null;
      n.push(s.value);
    }
    return n;
  }
  highlight(e, t, s, n) {
    let r = I(this.spec, e, Math.max(
      0,
      t - 250
      /* RegExp.HighlightMargin */
    ), Math.min(s + 250, e.doc.length));
    for (; !r.next().done; )
      n(r.value.from, r.value.to);
  }
}
const $ = /* @__PURE__ */ F.define(), te = /* @__PURE__ */ F.define(), E = /* @__PURE__ */ me.define({
  create(i) {
    return new U(Z(i).create(), null);
  },
  update(i, e) {
    for (let t of e.effects)
      t.is($) ? i = new U(t.value.create(), i.panel) : t.is(te) && (i = new U(i.query, t.value ? ie : null));
    return i;
  },
  provide: (i) => ge.from(i, (e) => e.panel)
});
class U {
  constructor(e, t) {
    this.query = e, this.panel = t;
  }
}
const Pt = /* @__PURE__ */ M.mark({ class: "cm-searchMatch" }), Ft = /* @__PURE__ */ M.mark({ class: "cm-searchMatch cm-searchMatch-selected" }), Tt = /* @__PURE__ */ de.fromClass(class {
  constructor(i) {
    this.view = i, this.decorations = this.highlight(i.state.field(E));
  }
  update(i) {
    let e = i.state.field(E);
    (e != i.startState.field(E) || i.docChanged || i.selectionSet || i.viewportChanged) && (this.decorations = this.highlight(e));
  }
  highlight({ query: i, panel: e }) {
    if (!e || !i.spec.valid)
      return M.none;
    let { view: t } = this, s = new ve();
    for (let n = 0, r = t.visibleRanges, l = r.length; n < l; n++) {
      let { from: o, to: a } = r[n];
      for (; n < l - 1 && a > r[n + 1].from - 500; )
        a = r[++n].to;
      i.highlight(t.state, o, a, (c, h) => {
        let d = t.state.selection.ranges.some((g) => g.from == c && g.to == h);
        s.add(c, h, d ? Ft : Pt);
      });
    }
    return s.finish();
  }
}, {
  decorations: (i) => i.decorations
});
function B(i) {
  return (e) => {
    let t = e.state.field(E, !1);
    return t && t.query.spec.valid ? i(e, t) : Ie(e);
  };
}
const Q = /* @__PURE__ */ B((i, { query: e }) => {
  let { to: t } = i.state.selection.main, s = e.nextMatch(i.state, t, t);
  if (!s)
    return !1;
  let n = S.single(s.from, s.to), r = i.state.facet(O);
  return i.dispatch({
    selection: n,
    effects: [se(i, s), r.scrollToMatch(n.main, i)],
    userEvent: "select.search"
  }), Te(i), !0;
}), J = /* @__PURE__ */ B((i, { query: e }) => {
  let { state: t } = i, { from: s } = t.selection.main, n = e.prevMatch(t, s, s);
  if (!n)
    return !1;
  let r = S.single(n.from, n.to), l = i.state.facet(O);
  return i.dispatch({
    selection: r,
    effects: [se(i, n), l.scrollToMatch(r.main, i)],
    userEvent: "select.search"
  }), Te(i), !0;
}), It = /* @__PURE__ */ B((i, { query: e }) => {
  let t = e.matchAll(i.state, 1e3);
  return !t || !t.length ? !1 : (i.dispatch({
    selection: S.create(t.map((s) => S.range(s.from, s.to))),
    userEvent: "select.search.matches"
  }), !0);
}), Wt = ({ state: i, dispatch: e }) => {
  let t = i.selection;
  if (t.ranges.length > 1 || t.main.empty)
    return !1;
  let { from: s, to: n } = t.main, r = [], l = 0;
  for (let o = new q(i.doc, i.sliceDoc(s, n)); !o.next().done; ) {
    if (r.length > 1e3)
      return !1;
    o.value.from == s && (l = r.length), r.push(S.range(o.value.from, o.value.to));
  }
  return e(i.update({
    selection: S.create(r, l),
    userEvent: "select.search.matches"
  })), !0;
}, fe = /* @__PURE__ */ B((i, { query: e }) => {
  let { state: t } = i, { from: s, to: n } = t.selection.main;
  if (t.readOnly)
    return !1;
  let r = e.nextMatch(t, s, s);
  if (!r)
    return !1;
  let l = r, o = [], a, c, h = [];
  l.precise ? l.from == s && l.to == n && (c = t.toText(e.getReplacement(l)), o.push({ from: l.from, to: l.to, insert: c }), l = e.nextMatch(t, l.from, l.to), h.push(y.announce.of(t.phrase("replaced match on line $", t.doc.lineAt(s).number) + "."))) : l = e.nextMatch(t, l.from, l.to);
  let d = i.state.changes(o);
  return l && (a = S.single(l.from, l.to).map(d), h.push(se(i, l)), h.push(t.facet(O).scrollToMatch(a.main, i))), i.dispatch({
    changes: d,
    selection: a,
    effects: h,
    userEvent: "input.replace"
  }), !0;
}), Dt = /* @__PURE__ */ B((i, { query: e }) => {
  if (i.state.readOnly)
    return !1;
  let t = [];
  for (let n of e.matchAll(i.state, 1e9)) {
    let { from: r, to: l, precise: o } = n;
    o && t.push({ from: r, to: l, insert: e.getReplacement(n) });
  }
  if (!t.length)
    return !1;
  let s = i.state.phrase("replaced $ matches", t.length) + ".";
  return i.dispatch({
    changes: t,
    effects: y.announce.of(s),
    userEvent: "input.replace.all"
  }), !0;
});
function ie(i) {
  return i.state.facet(O).createPanel(i);
}
function Z(i, e) {
  var t, s, n, r, l;
  let o = i.selection.main, a = o.empty || o.to > o.from + 100 ? "" : i.sliceDoc(o.from, o.to);
  if (e && !a)
    return e;
  let c = i.facet(O);
  return new Ee({
    search: ((t = e?.literal) !== null && t !== void 0 ? t : c.literal) ? a : a.replace(/\n/g, "\\n"),
    caseSensitive: (s = e?.caseSensitive) !== null && s !== void 0 ? s : c.caseSensitive,
    literal: (n = e?.literal) !== null && n !== void 0 ? n : c.literal,
    regexp: (r = e?.regexp) !== null && r !== void 0 ? r : c.regexp,
    wholeWord: (l = e?.wholeWord) !== null && l !== void 0 ? l : c.wholeWord
  });
}
function Fe(i) {
  let e = ye(i, ie);
  return e && e.dom.querySelector("[main-field]");
}
function Te(i) {
  let e = Fe(i);
  e && e == i.root.activeElement && e.select();
}
const Ie = (i) => {
  let e = i.state.field(E, !1);
  if (e && e.panel) {
    let t = Fe(i);
    if (t && t != i.root.activeElement) {
      let s = Z(i.state, e.query.spec);
      s.valid && i.dispatch({ effects: $.of(s) }), t.focus(), t.select();
    }
  } else
    i.dispatch({ effects: [
      te.of(!0),
      e ? $.of(Z(i.state, e.query.spec)) : F.appendConfig.of(Bt)
    ] });
  return !0;
}, We = (i) => {
  let e = i.state.field(E, !1);
  if (!e || !e.panel)
    return !1;
  let t = ye(i, ie);
  return t && t.dom.contains(i.root.activeElement) && i.focus(), i.dispatch({ effects: te.of(!1) }), !0;
}, Ot = [
  { key: "Mod-f", run: Ie, scope: "editor search-panel" },
  { key: "F3", run: Q, shift: J, scope: "editor search-panel", preventDefault: !0 },
  { key: "Mod-g", run: Q, shift: J, scope: "editor search-panel", preventDefault: !0 },
  { key: "Escape", run: We, scope: "editor search-panel" },
  { key: "Mod-Shift-l", run: Wt },
  { key: "Mod-Alt-g", run: bt },
  { key: "Mod-d", run: wt, preventDefault: !0 }
];
class qt {
  constructor(e) {
    this.view = e;
    let t = this.query = e.state.field(E).query.spec;
    this.commit = this.commit.bind(this), this.searchField = f("input", {
      value: t.search,
      placeholder: b(e, "Find"),
      "aria-label": b(e, "Find"),
      class: "cm-textfield",
      name: "search",
      form: "",
      "main-field": "true",
      onchange: this.commit,
      onkeyup: this.commit
    }), this.replaceField = f("input", {
      value: t.replace,
      placeholder: b(e, "Replace"),
      "aria-label": b(e, "Replace"),
      class: "cm-textfield",
      name: "replace",
      form: "",
      onchange: this.commit,
      onkeyup: this.commit
    }), this.caseField = f("input", {
      type: "checkbox",
      name: "case",
      form: "",
      checked: t.caseSensitive,
      onchange: this.commit
    }), this.reField = f("input", {
      type: "checkbox",
      name: "re",
      form: "",
      checked: t.regexp,
      onchange: this.commit
    }), this.wordField = f("input", {
      type: "checkbox",
      name: "word",
      form: "",
      checked: t.wholeWord,
      onchange: this.commit
    });
    function s(n, r, l) {
      return f("button", { class: "cm-button", name: n, onclick: r, type: "button" }, l);
    }
    this.dom = f("div", { onkeydown: (n) => this.keydown(n), class: "cm-search" }, [
      this.searchField,
      s("next", () => Q(e), [b(e, "next")]),
      s("prev", () => J(e), [b(e, "previous")]),
      s("select", () => It(e), [b(e, "all")]),
      f("label", null, [this.caseField, b(e, "match case")]),
      f("label", null, [this.reField, b(e, "regexp")]),
      f("label", null, [this.wordField, b(e, "by word")]),
      ...e.state.readOnly ? [] : [
        f("br"),
        this.replaceField,
        s("replace", () => fe(e), [b(e, "replace")]),
        s("replaceAll", () => Dt(e), [b(e, "replace all")])
      ],
      f("button", {
        name: "close",
        onclick: () => We(e),
        "aria-label": b(e, "close"),
        type: "button"
      }, ["×"])
    ]);
  }
  commit() {
    let e = new Ee({
      search: this.searchField.value,
      caseSensitive: this.caseField.checked,
      regexp: this.reField.checked,
      wholeWord: this.wordField.checked,
      replace: this.replaceField.value
    });
    e.eq(this.query) || (this.query = e, this.view.dispatch({ effects: $.of(e) }));
  }
  keydown(e) {
    Ve(this.view, e, "search-panel") ? e.preventDefault() : e.keyCode == 13 && e.target == this.searchField ? (e.preventDefault(), (e.shiftKey ? J : Q)(this.view)) : e.keyCode == 13 && e.target == this.replaceField && (e.preventDefault(), fe(this.view));
  }
  update(e) {
    for (let t of e.transactions)
      for (let s of t.effects)
        s.is($) && !s.value.eq(this.query) && this.setQuery(s.value);
  }
  setQuery(e) {
    this.query = e, this.searchField.value = e.search, this.replaceField.value = e.replace, this.caseField.checked = e.caseSensitive, this.reField.checked = e.regexp, this.wordField.checked = e.wholeWord;
  }
  mount() {
    this.searchField.select();
  }
  get pos() {
    return 80;
  }
  get top() {
    return this.view.state.facet(O).top;
  }
}
function b(i, e) {
  return i.state.phrase(e);
}
const _ = 30, z = /[\s\.,:;?!]/;
function se(i, { from: e, to: t }) {
  let s = i.state.doc.lineAt(e), n = i.state.doc.lineAt(t).to, r = Math.max(s.from, e - _), l = Math.min(n, t + _), o = i.state.sliceDoc(r, l);
  if (r != s.from) {
    for (let a = 0; a < _; a++)
      if (!z.test(o[a + 1]) && z.test(o[a])) {
        o = o.slice(a);
        break;
      }
  }
  if (l != n) {
    for (let a = o.length - 1; a > o.length - _; a--)
      if (!z.test(o[a - 1]) && z.test(o[a])) {
        o = o.slice(0, a);
        break;
      }
  }
  return y.announce.of(`${i.state.phrase("current match")}. ${o} ${i.state.phrase("on line")} ${s.number}.`);
}
const $t = /* @__PURE__ */ y.baseTheme({
  ".cm-panel.cm-search": {
    padding: "2px 6px 4px",
    position: "relative",
    "& [name=close]": {
      position: "absolute",
      top: "0",
      right: "4px",
      backgroundColor: "inherit",
      border: "none",
      font: "inherit",
      padding: 0,
      margin: 0
    },
    "& input, & button, & label": {
      margin: ".2em .6em .2em 0"
    },
    "& input[type=checkbox]": {
      marginRight: ".2em"
    },
    "& label": {
      fontSize: "80%",
      whiteSpace: "pre"
    }
  },
  "&light .cm-searchMatch": { backgroundColor: "#ffff0054" },
  "&dark .cm-searchMatch": { backgroundColor: "#00ffff8a" },
  "&light .cm-searchMatch-selected": { backgroundColor: "#ff6a0054" },
  "&dark .cm-searchMatch-selected": { backgroundColor: "#ff00ff8a" }
}), Bt = [
  E,
  /* @__PURE__ */ Ne.low(Tt),
  $t
], L = /* @__PURE__ */ new Map();
let Nt = 1;
function _t(i) {
  switch ((i ?? "plaintext").toLowerCase()) {
    case "json":
      return it();
    case "html":
    case "scriban":
      return tt();
    case "markdown":
      return et();
    case "css":
      return Xe();
    case "javascript":
      return le();
    case "typescript":
      return le({ typescript: !0 });
    case "xml":
      return Ze();
    default:
      return [];
  }
}
function Vt(i, e, t = {}) {
  const s = `sb-code-${Nt++}`, n = [
    je(),
    He.of([
      ...Qe,
      ...Je,
      Ge,
      ...Ot,
      {
        key: "Mod-s",
        run: () => (e.invokeMethodAsync("OnEditorShortcut", "save"), !0)
      },
      {
        key: "Mod-p",
        run: () => (e.invokeMethodAsync("OnEditorShortcut", "preview"), !0)
      }
    ]),
    _t(t.language),
    y.updateListener.of((l) => {
      l.docChanged && e.invokeMethodAsync("OnEditorContentChanged", l.state.doc.toString());
    }),
    y.editable.of(!(t.readOnly || t.disabled)),
    Y.readOnly.of(!!t.readOnly)
  ];
  t.lineNumbers !== !1 && n.push(Ue()), t.wordWrap && n.push(y.lineWrapping), t.placeholder && n.push(Ye(t.placeholder)), t.validateJson && (t.language ?? "").toLowerCase() === "json" && n.push(ft(st()));
  const r = new y({
    parent: i,
    state: Y.create({
      doc: t.value ?? "",
      extensions: n
    })
  });
  return i.setAttribute("dir", t.direction ?? "ltr"), L.set(s, r), s;
}
function jt(i) {
  L.get(i)?.destroy(), L.delete(i);
}
function zt(i) {
  return L.get(i)?.state.doc.toString() ?? "";
}
function Ht(i, e) {
  const t = L.get(i);
  t && t.dispatch({
    changes: { from: 0, to: t.state.doc.length, insert: e ?? "" }
  });
}
function Qt(i, e) {
  const t = L.get(i);
  if (!t)
    return;
  const { from: s, to: n } = t.state.selection.main;
  t.dispatch({ changes: { from: s, to: n, insert: e ?? "" } });
}
function Jt(i) {
  const e = L.get(i);
  if (!e)
    return "";
  const { from: t, to: s } = e.state.selection.main;
  return e.state.doc.sliceString(t, s);
}
function Gt(i) {
  L.get(i)?.focus();
}
function Ut(i) {
  const e = L.get(i);
  if (!e)
    return !1;
  try {
    const t = JSON.parse(e.state.doc.toString()), s = JSON.stringify(t, null, 2);
    return e.dispatch({ changes: { from: 0, to: e.state.doc.length, insert: s } }), !0;
  } catch {
    return !1;
  }
}
function Yt(i) {
  const e = zt(i);
  if (!e.trim())
    return !0;
  try {
    return JSON.parse(e), !0;
  } catch {
    return !1;
  }
}
export {
  jt as destroyEditor,
  Gt as focusEditor,
  Ut as formatJson,
  Jt as getSelection,
  zt as getValue,
  Vt as initEditor,
  Qt as insertText,
  Ht as setValue,
  Yt as validateJson
};
