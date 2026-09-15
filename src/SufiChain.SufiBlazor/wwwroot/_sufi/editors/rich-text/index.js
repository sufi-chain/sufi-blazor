import { E as ct, P as vt, a as bt, p as re, N as le, C as ce, i as ae, b as fe, c as ue, D as wt, d as Dt, e as Et, f as de, h as he, g as ge, j as me } from "../chunks/schema-Dvjie1O7.js";
const zt = ["top", "right", "bottom", "left"], Lt = ["start", "end"], Mt = /* @__PURE__ */ zt.reduce((t, e) => t.concat(e, e + "-" + Lt[0], e + "-" + Lt[1]), []), U = Math.min, H = Math.max, st = Math.round, z = (t) => ({
  x: t,
  y: t
}), pe = {
  left: "right",
  right: "left",
  bottom: "top",
  top: "bottom"
};
function Wt(t, e, n) {
  return H(t, U(e, n));
}
function W(t, e) {
  return typeof t == "function" ? t(e) : t;
}
function I(t) {
  return t.split("-")[0];
}
function P(t) {
  return t.split("-")[1];
}
function _t(t) {
  return t === "x" ? "y" : "x";
}
function xt(t) {
  return t === "y" ? "height" : "width";
}
function B(t) {
  const e = t[0];
  return e === "t" || e === "b" ? "y" : "x";
}
function Ot(t) {
  return _t(B(t));
}
function jt(t, e, n) {
  n === void 0 && (n = !1);
  const i = P(t), o = Ot(t), s = xt(o);
  let r = o === "x" ? i === (n ? "end" : "start") ? "right" : "left" : i === "start" ? "bottom" : "top";
  return e.reference[s] > e.floating[s] && (r = lt(r)), [r, lt(r)];
}
function we(t) {
  const e = lt(t);
  return [rt(t), e, rt(e)];
}
function rt(t) {
  return t.includes("start") ? t.replace("start", "end") : t.replace("end", "start");
}
const kt = ["left", "right"], Ht = ["right", "left"], ye = ["top", "bottom"], ve = ["bottom", "top"];
function be(t, e, n) {
  switch (t) {
    case "top":
    case "bottom":
      return n ? e ? Ht : kt : e ? kt : Ht;
    case "left":
    case "right":
      return e ? ye : ve;
    default:
      return [];
  }
}
function xe(t, e, n, i) {
  const o = P(t);
  let s = be(I(t), n === "start", i);
  return o && (s = s.map((r) => r + "-" + o), e && (s = s.concat(s.map(rt)))), s;
}
function lt(t) {
  const e = I(t);
  return pe[e] + t.slice(e.length);
}
function Oe(t) {
  var e, n, i, o;
  return {
    top: (e = t.top) != null ? e : 0,
    right: (n = t.right) != null ? n : 0,
    bottom: (i = t.bottom) != null ? i : 0,
    left: (o = t.left) != null ? o : 0
  };
}
function Ct(t) {
  return typeof t != "number" ? Oe(t) : {
    top: t,
    right: t,
    bottom: t,
    left: t
  };
}
function j(t) {
  const {
    x: e,
    y: n,
    width: i,
    height: o
  } = t;
  return {
    width: i,
    height: o,
    top: n,
    left: e,
    right: e + i,
    bottom: n + o,
    x: e,
    y: n
  };
}
function Pt(t, e, n) {
  let {
    reference: i,
    floating: o
  } = t;
  const s = B(e), r = Ot(e), l = xt(r), f = I(e), a = s === "y", h = i.x + i.width / 2 - o.width / 2, c = i.y + i.height / 2 - o.height / 2, u = i[l] / 2 - o[l] / 2;
  let d;
  switch (f) {
    case "top":
      d = {
        x: h,
        y: i.y - o.height
      };
      break;
    case "bottom":
      d = {
        x: h,
        y: i.y + i.height
      };
      break;
    case "right":
      d = {
        x: i.x + i.width,
        y: c
      };
      break;
    case "left":
      d = {
        x: i.x - o.width,
        y: c
      };
      break;
    default:
      d = {
        x: i.x,
        y: i.y
      };
  }
  const g = P(e);
  return g && (d[r] += u * (g === "end" ? 1 : -1) * (n && a ? -1 : 1)), d;
}
async function Ce(t, e) {
  var n;
  e === void 0 && (e = {});
  const {
    x: i,
    y: o,
    platform: s,
    rects: r,
    elements: l,
    strategy: f
  } = t, {
    boundary: a = "clippingAncestors",
    rootBoundary: h = "viewport",
    elementContext: c = "floating",
    altBoundary: u = !1,
    padding: d = 0
  } = W(e, t), g = Ct(d), p = l[u ? c === "floating" ? "reference" : "floating" : c], w = j(await s.getClippingRect({
    element: (n = await (s.isElement == null ? void 0 : s.isElement(p))) == null || n ? p : p.contextElement || await (s.getDocumentElement == null ? void 0 : s.getDocumentElement(l.floating)),
    boundary: a,
    rootBoundary: h,
    strategy: f
  })), v = c === "floating" ? {
    x: i,
    y: o,
    width: r.floating.width,
    height: r.floating.height
  } : r.reference, b = await (s.getOffsetParent == null ? void 0 : s.getOffsetParent(l.floating)), x = await (s.isElement == null ? void 0 : s.isElement(b)) && await (s.getScale == null ? void 0 : s.getScale(b)) || {
    x: 1,
    y: 1
  }, T = j(s.convertOffsetParentRelativeRectToViewportRelativeRect ? await s.convertOffsetParentRelativeRectToViewportRelativeRect({
    elements: l,
    rect: v,
    offsetParent: b,
    strategy: f
  }) : v);
  return {
    top: (w.top - T.top + g.top) / x.y,
    bottom: (T.bottom - w.bottom + g.bottom) / x.y,
    left: (w.left - T.left + g.left) / x.x,
    right: (T.right - w.right + g.right) / x.x
  };
}
const Te = 50, Se = async (t, e, n) => {
  const {
    placement: i = "bottom",
    strategy: o = "absolute",
    middleware: s = [],
    platform: r
  } = n, l = r.detectOverflow ? r : {
    ...r,
    detectOverflow: Ce
  }, f = await (r.isRTL == null ? void 0 : r.isRTL(e));
  let a = await r.getElementRects({
    reference: t,
    floating: e,
    strategy: o
  }), {
    x: h,
    y: c
  } = Pt(a, i, f), u = i, d = 0;
  const g = {};
  for (let m = 0; m < s.length; m++) {
    const p = s[m];
    if (!p)
      continue;
    const {
      name: w,
      fn: v
    } = p, {
      x: b,
      y: x,
      data: T,
      reset: y
    } = await v({
      x: h,
      y: c,
      initialPlacement: i,
      placement: u,
      strategy: o,
      middlewareData: g,
      rects: a,
      platform: l,
      elements: {
        reference: t,
        floating: e
      }
    });
    h = b ?? h, c = x ?? c, g[w] = {
      ...g[w],
      ...T
    }, y && d < Te && (d++, typeof y == "object" && (y.placement && (u = y.placement), y.rects && (a = y.rects === !0 ? await r.getElementRects({
      reference: t,
      floating: e,
      strategy: o
    }) : y.rects), {
      x: h,
      y: c
    } = Pt(a, u, f)), m = -1);
  }
  return {
    x: h,
    y: c,
    placement: u,
    strategy: o,
    middlewareData: g
  };
}, Re = (t) => ({
  name: "arrow",
  options: t,
  async fn(e) {
    const {
      x: n,
      y: i,
      placement: o,
      rects: s,
      platform: r,
      elements: l,
      middlewareData: f
    } = e, {
      element: a,
      padding: h = 0
    } = W(t, e) || {};
    if (a == null)
      return {};
    const c = Ct(h), u = {
      x: n,
      y: i
    }, d = Ot(o), g = xt(d), m = await r.getDimensions(a), p = d === "y", w = p ? "top" : "left", v = p ? "bottom" : "right", b = p ? "clientHeight" : "clientWidth", x = s.reference[g] + s.reference[d] - u[d] - s.floating[g], T = u[d] - s.reference[d], y = await (r.getOffsetParent == null ? void 0 : r.getOffsetParent(a));
    let S = y ? y[b] : 0;
    (!S || !await (r.isElement == null ? void 0 : r.isElement(y))) && (S = l.floating[b] || s.floating[g]);
    const E = x / 2 - T / 2, A = S / 2 - m[g] / 2 - 1, O = U(c[w], A), R = U(c[v], A), N = S - m[g] - R, L = S / 2 - m[g] / 2 + E, M = Wt(O, L, N), Y = !f.arrow && P(o) != null && L !== M && s.reference[g] / 2 - (L < O ? O : R) - m[g] / 2 < 0, F = Y ? L < O ? L - O : L - N : 0;
    return {
      [d]: u[d] + F,
      data: {
        [d]: M,
        centerOffset: L - M - F,
        ...Y && {
          alignmentOffset: F
        }
      },
      reset: Y
    };
  }
});
function Ae(t, e, n) {
  return (t ? [...n.filter((o) => P(o) === t), ...n.filter((o) => P(o) !== t)] : n.filter((o) => I(o) === o)).filter((o) => t ? P(o) === t || (e ? rt(o) !== o : !1) : !0);
}
const De = function(t) {
  return t === void 0 && (t = {}), {
    name: "autoPlacement",
    options: t,
    async fn(e) {
      var n, i, o;
      const {
        rects: s,
        middlewareData: r,
        placement: l,
        platform: f,
        elements: a
      } = e, {
        crossAxis: h = !1,
        alignment: c,
        allowedPlacements: u = Mt,
        autoAlignment: d = !0,
        ...g
      } = W(t, e), m = c !== void 0 || u === Mt ? Ae(c || null, d, u) : u, p = ((n = r.autoPlacement) == null ? void 0 : n.index) || 0, w = m[p];
      if (w == null)
        return {};
      if (l !== w)
        return {
          reset: {
            placement: m[0]
          }
        };
      const v = await f.detectOverflow(e, g), b = jt(w, s, await (f.isRTL == null ? void 0 : f.isRTL(a.floating))), x = [v[I(w)], v[b[0]], v[b[1]]], T = [...((i = r.autoPlacement) == null ? void 0 : i.overflows) || [], {
        placement: w,
        overflows: x
      }], y = m[p + 1];
      if (y)
        return {
          data: {
            index: p + 1,
            overflows: T
          },
          reset: {
            placement: y
          }
        };
      const S = T.map((O) => {
        const R = P(O.placement);
        return [O.placement, R && h ? (
          // Check along the mainAxis and main crossAxis side.
          O.overflows.slice(0, 2).reduce((N, L) => N + L, 0)
        ) : (
          // Check only the mainAxis.
          O.overflows[0]
        ), O.overflows];
      }).sort((O, R) => O[1] - R[1]), A = ((o = S.filter((O) => O[2].slice(
        0,
        // Aligned placements should not check their opposite crossAxis
        // side.
        P(O[0]) ? 2 : 3
      ).every((R) => R <= 0))[0]) == null ? void 0 : o[0]) || S[0][0];
      return A !== l ? {
        data: {
          index: p + 1,
          overflows: T
        },
        reset: {
          placement: A
        }
      } : {};
    }
  };
}, Ee = function(t) {
  return t === void 0 && (t = {}), {
    name: "flip",
    options: t,
    async fn(e) {
      var n, i;
      const {
        placement: o,
        middlewareData: s,
        rects: r,
        initialPlacement: l,
        platform: f,
        elements: a
      } = e, {
        mainAxis: h = !0,
        crossAxis: c = !0,
        fallbackPlacements: u,
        fallbackStrategy: d = "bestFit",
        fallbackAxisSideDirection: g = "none",
        flipAlignment: m = !0,
        ...p
      } = W(t, e);
      if ((n = s.arrow) != null && n.alignmentOffset)
        return {};
      const w = I(o), v = B(l), b = I(l) === l, x = await (f.isRTL == null ? void 0 : f.isRTL(a.floating)), T = u || (b || !m ? [lt(l)] : we(l)), y = g !== "none";
      !u && y && T.push(...xe(l, m, g, x));
      const S = [l, ...T], E = await f.detectOverflow(e, p), A = [];
      let O = ((i = s.flip) == null ? void 0 : i.overflows) || [];
      if (h && A.push(E[w]), c) {
        const M = jt(o, r, x);
        A.push(E[M[0]], E[M[1]]);
      }
      if (O = [...O, {
        placement: o,
        overflows: A
      }], !A.every((M) => M <= 0)) {
        var R, N;
        const M = (((R = s.flip) == null ? void 0 : R.index) || 0) + 1, Y = S[M];
        if (Y && (!(c === "alignment" ? v !== B(Y) : !1) || // We leave the current main axis only if every placement on that axis
        // overflows the main axis.
        O.every((k) => B(k.placement) === v ? k.overflows[0] > 0 : !0)))
          return {
            data: {
              index: M,
              overflows: O
            },
            reset: {
              placement: Y
            }
          };
        let F = (N = O.filter((J) => J.overflows[0] <= 0).sort((J, k) => J.overflows[1] - k.overflows[1])[0]) == null ? void 0 : N.placement;
        if (!F)
          switch (d) {
            case "bestFit": {
              var L;
              const J = (L = O.filter((k) => {
                if (y) {
                  const _ = B(k.placement);
                  return _ === v || // Create a bias to the `y` side axis due to horizontal
                  // reading directions favoring greater width.
                  _ === "y";
                }
                return !0;
              }).map((k) => [k.placement, k.overflows.filter((_) => _ > 0).reduce((_, se) => _ + se, 0)]).sort((k, _) => k[1] - _[1])[0]) == null ? void 0 : L[0];
              J && (F = J);
              break;
            }
            case "initialPlacement":
              F = l;
              break;
          }
        if (o !== F)
          return {
            reset: {
              placement: F
            }
          };
      }
      return {};
    }
  };
};
function It(t, e) {
  return {
    top: t.top - e.height,
    right: t.right - e.width,
    bottom: t.bottom - e.height,
    left: t.left - e.width
  };
}
function Bt(t) {
  return zt.some((e) => t[e] >= 0);
}
const Le = function(t) {
  return t === void 0 && (t = {}), {
    name: "hide",
    options: t,
    async fn(e) {
      const {
        rects: n,
        platform: i
      } = e, {
        strategy: o = "referenceHidden",
        ...s
      } = W(t, e);
      switch (o) {
        case "referenceHidden": {
          const r = await i.detectOverflow(e, {
            ...s,
            elementContext: "reference"
          }), l = It(r, n.reference);
          return {
            data: {
              referenceHiddenOffsets: l,
              referenceHidden: Bt(l)
            }
          };
        }
        case "escaped": {
          const r = await i.detectOverflow(e, {
            ...s,
            altBoundary: !0
          }), l = It(r, n.floating);
          return {
            data: {
              escapedOffsets: l,
              escaped: Bt(l)
            }
          };
        }
        default:
          return {};
      }
    }
  };
};
function Kt(t) {
  const e = U(...t.map((s) => s.left)), n = U(...t.map((s) => s.top)), i = H(...t.map((s) => s.right)), o = H(...t.map((s) => s.bottom));
  return {
    x: e,
    y: n,
    width: i - e,
    height: o - n
  };
}
function Me(t) {
  const e = t.slice().sort((o, s) => o.y - s.y), n = [];
  let i = null;
  for (let o = 0; o < e.length; o++) {
    const s = e[o];
    !i || s.y - i.y > i.height / 2 ? n.push([s]) : n[n.length - 1].push(s), i = s;
  }
  return n.map((o) => j(Kt(o)));
}
const ke = function(t) {
  return t === void 0 && (t = {}), {
    name: "inline",
    options: t,
    async fn(e) {
      const {
        placement: n,
        elements: i,
        rects: o,
        platform: s,
        strategy: r
      } = e, {
        padding: l = 2,
        x: f,
        y: a
      } = W(t, e), h = Array.from(await (s.getClientRects == null ? void 0 : s.getClientRects(i.reference)) || []);
      if (!h.length)
        return {};
      const c = Me(h), u = j(Kt(h)), d = Ct(l);
      function g() {
        if (c.length === 2 && (c[0].left > c[1].right || c[1].left > c[0].right) && f != null && a != null)
          return c.find((p) => f > p.left - d.left && f < p.right + d.right && a > p.top - d.top && a < p.bottom + d.bottom) || u;
        if (c.length >= 2) {
          if (B(n) === "y") {
            const y = c[0], S = c[c.length - 1], E = I(n) === "top", A = y.top, O = S.bottom, R = E ? y.left : S.left, N = E ? y.right : S.right;
            return j({
              x: R,
              y: A,
              width: N - R,
              height: O - A
            });
          }
          const p = I(n) === "left", w = H(...c.map((y) => y.right)), v = U(...c.map((y) => y.left)), b = c.filter((y) => p ? y.left === v : y.right === w), x = b[0].top, T = b[b.length - 1].bottom;
          return j({
            x: v,
            y: x,
            width: w - v,
            height: T - x
          });
        }
        return u;
      }
      const m = await s.getElementRects({
        reference: {
          getBoundingClientRect: g
        },
        floating: i.floating,
        strategy: r
      });
      return o.reference.x !== m.reference.x || o.reference.y !== m.reference.y || o.reference.width !== m.reference.width || o.reference.height !== m.reference.height ? {
        reset: {
          rects: m
        }
      } : {};
    }
  };
}, He = /* @__PURE__ */ new Set(["left", "top"]);
async function Pe(t, e) {
  const {
    placement: n,
    platform: i,
    elements: o
  } = t, s = await (i.isRTL == null ? void 0 : i.isRTL(o.floating)), r = I(n), l = P(n), f = B(n) === "y", a = He.has(r) ? -1 : 1, h = s && f ? -1 : 1, c = W(e, t);
  let {
    mainAxis: u,
    crossAxis: d,
    alignmentAxis: g
  } = typeof c == "number" ? {
    mainAxis: c,
    crossAxis: 0,
    alignmentAxis: null
  } : {
    mainAxis: c.mainAxis || 0,
    crossAxis: c.crossAxis || 0,
    alignmentAxis: c.alignmentAxis
  };
  return l && typeof g == "number" && (d = l === "end" ? g * -1 : g), f ? {
    x: d * h,
    y: u * a
  } : {
    x: u * a,
    y: d * h
  };
}
const Ie = function(t) {
  return t === void 0 && (t = 0), {
    name: "offset",
    options: t,
    async fn(e) {
      var n, i;
      const {
        x: o,
        y: s,
        placement: r,
        middlewareData: l
      } = e, f = await Pe(e, t);
      return r === ((n = l.offset) == null ? void 0 : n.placement) && (i = l.arrow) != null && i.alignmentOffset ? {} : {
        x: o + f.x,
        y: s + f.y,
        data: {
          ...f,
          placement: r
        }
      };
    }
  };
}, Be = function(t) {
  return t === void 0 && (t = {}), {
    name: "shift",
    options: t,
    async fn(e) {
      const {
        x: n,
        y: i,
        placement: o,
        platform: s
      } = e, {
        mainAxis: r = !0,
        crossAxis: l = !1,
        limiter: f = {
          fn: (v) => {
            let {
              x: b,
              y: x
            } = v;
            return {
              x: b,
              y: x
            };
          }
        },
        ...a
      } = W(t, e), h = {
        x: n,
        y: i
      }, c = await s.detectOverflow(e, a), u = B(o), d = _t(u);
      let g = h[d], m = h[u];
      const p = (v, b) => Wt(b + c[v === "y" ? "top" : "left"], b, b - c[v === "y" ? "bottom" : "right"]);
      r && (g = p(d, g)), l && (m = p(u, m));
      const w = f.fn({
        ...e,
        [d]: g,
        [u]: m
      });
      return {
        ...w,
        data: {
          x: w.x - n,
          y: w.y - i,
          enabled: {
            [d]: r,
            [u]: l
          }
        }
      };
    }
  };
}, Ue = function(t) {
  return t === void 0 && (t = {}), {
    name: "size",
    options: t,
    async fn(e) {
      const {
        placement: n,
        rects: i,
        platform: o,
        elements: s
      } = e, {
        apply: r = () => {
        },
        ...l
      } = W(t, e), f = await o.detectOverflow(e, l), a = I(n), h = P(n), c = B(n) === "y", {
        width: u,
        height: d
      } = i.floating;
      let g, m;
      a === "top" || a === "bottom" ? (g = a, m = h === (await (o.isRTL == null ? void 0 : o.isRTL(s.floating)) ? "start" : "end") ? "left" : "right") : (m = a, g = h === "end" ? "top" : "bottom");
      const p = d - f.top - f.bottom, w = u - f.left - f.right, v = U(d - f[g], p), b = U(u - f[m], w), x = e.middlewareData.shift, T = !x;
      let y = v, S = b;
      x != null && x.enabled.x && (S = w), x != null && x.enabled.y && (y = p), T && !h && (c ? S = u - 2 * H(f.left, f.right) : y = d - 2 * H(f.top, f.bottom)), await r({
        ...e,
        availableWidth: S,
        availableHeight: y
      });
      const E = await o.getDimensions(s.floating);
      return u !== E.width || d !== E.height ? {
        reset: {
          rects: !0
        }
      } : {};
    }
  };
};
function at() {
  return typeof window < "u";
}
function et(t) {
  return Xt(t) ? (t.nodeName || "").toLowerCase() : "#document";
}
function D(t) {
  var e;
  return (t == null || (e = t.ownerDocument) == null ? void 0 : e.defaultView) || window;
}
function X(t) {
  var e;
  return (e = (Xt(t) ? t.ownerDocument : t.document) || window.document) == null ? void 0 : e.documentElement;
}
function Xt(t) {
  return at() ? t instanceof Node || t instanceof D(t).Node : !1;
}
function $(t) {
  return at() ? t instanceof Element || t instanceof D(t).Element : !1;
}
function q(t) {
  return at() ? t instanceof HTMLElement || t instanceof D(t).HTMLElement : !1;
}
function Ut(t) {
  return !at() || typeof ShadowRoot > "u" ? !1 : t instanceof ShadowRoot || t instanceof D(t).ShadowRoot;
}
function ft(t) {
  const {
    overflow: e,
    overflowX: n,
    overflowY: i,
    display: o
  } = V(t);
  return /auto|scroll|overlay|hidden|clip/.test(e + i + n) && o !== "inline" && o !== "contents";
}
function $e(t) {
  return /^(table|td|th)$/.test(et(t));
}
function ut(t) {
  try {
    if (t.matches(":popover-open"))
      return !0;
  } catch {
  }
  try {
    return t.matches(":modal");
  } catch {
    return !1;
  }
}
const Ve = /transform|translate|scale|rotate|perspective|filter/, Ne = /paint|layout|strict|content/, G = (t) => !!t && t !== "none";
let gt;
function Tt(t) {
  const e = $(t) ? V(t) : t;
  return G(e.transform) || G(e.translate) || G(e.scale) || G(e.rotate) || G(e.perspective) || !St() && (G(e.backdropFilter) || G(e.filter)) || Ve.test(e.willChange || "") || Ne.test(e.contain || "");
}
function Fe(t) {
  let e = Z(t);
  for (; q(e) && !it(e); ) {
    if (Tt(e))
      return e;
    if (ut(e))
      return null;
    e = Z(e);
  }
  return null;
}
function St() {
  return gt == null && (gt = typeof CSS < "u" && CSS.supports && CSS.supports("-webkit-backdrop-filter", "none")), gt;
}
function it(t) {
  return /^(html|body|#document)$/.test(et(t));
}
function V(t) {
  return D(t).getComputedStyle(t);
}
function dt(t) {
  return $(t) ? {
    scrollLeft: t.scrollLeft,
    scrollTop: t.scrollTop
  } : {
    scrollLeft: t.scrollX,
    scrollTop: t.scrollY
  };
}
function Z(t) {
  if (et(t) === "html")
    return t;
  const e = (
    // Step into the shadow DOM of the parent of a slotted node.
    t.assignedSlot || // DOM Element detected.
    t.parentNode || // ShadowRoot detected.
    Ut(t) && t.host || // Fallback.
    X(t)
  );
  return Ut(e) ? e.host : e;
}
function qt(t) {
  const e = Z(t);
  return it(e) ? (t.ownerDocument || t).body : q(e) && ft(e) ? e : qt(e);
}
function Yt(t, e, n) {
  var i;
  e === void 0 && (e = []);
  const o = qt(t), s = o === ((i = t.ownerDocument) == null ? void 0 : i.body), r = D(o);
  return s ? (yt(r), e.concat(r, r.visualViewport || [], ft(o) ? o : [], [])) : e.concat(o, Yt(o, []));
}
function yt(t) {
  return t.parent && Object.getPrototypeOf(t.parent) ? t.frameElement : null;
}
function Jt(t) {
  const e = V(t);
  let n = parseFloat(e.width) || 0, i = parseFloat(e.height) || 0;
  const o = q(t), s = o ? t.offsetWidth : n, r = o ? t.offsetHeight : i, l = st(n) !== s || st(i) !== r;
  return l && (n = s, i = r), {
    width: n,
    height: i,
    $: l
  };
}
function Gt(t) {
  return $(t) ? t : t.contextElement;
}
function tt(t) {
  const e = Gt(t);
  if (!q(e))
    return z(1);
  const n = e.getBoundingClientRect(), {
    width: i,
    height: o,
    $: s
  } = Jt(e);
  let r = (s ? st(n.width) : n.width) / i, l = (s ? st(n.height) : n.height) / o;
  return (!r || !Number.isFinite(r)) && (r = 1), (!l || !Number.isFinite(l)) && (l = 1), {
    x: r,
    y: l
  };
}
const ze = /* @__PURE__ */ z(0);
function Qt(t) {
  const e = D(t);
  return !St() || !e.visualViewport ? ze : {
    x: e.visualViewport.offsetLeft,
    y: e.visualViewport.offsetTop
  };
}
function We(t, e, n) {
  return e === void 0 && (e = !1), !!n && e && n === D(t);
}
function ot(t, e, n, i) {
  e === void 0 && (e = !1), n === void 0 && (n = !1);
  const o = t.getBoundingClientRect(), s = Gt(t);
  let r = z(1);
  e && (i ? $(i) && (r = tt(i)) : r = tt(t));
  const l = We(s, n, i) ? Qt(s) : z(0);
  let f = (o.left + l.x) / r.x, a = (o.top + l.y) / r.y, h = o.width / r.x, c = o.height / r.y;
  if (s && i) {
    const u = D(s), d = $(i) ? D(i) : i;
    let g = u, m = yt(g);
    for (; m && d !== g; ) {
      const p = tt(m), w = m.getBoundingClientRect(), v = V(m), b = w.left + (m.clientLeft + parseFloat(v.paddingLeft)) * p.x, x = w.top + (m.clientTop + parseFloat(v.paddingTop)) * p.y;
      f *= p.x, a *= p.y, h *= p.x, c *= p.y, f += b, a += x, g = D(m), m = yt(g);
    }
  }
  return j({
    width: h,
    height: c,
    x: f,
    y: a
  });
}
function ht(t, e) {
  const n = dt(t).scrollLeft;
  return e ? e.left + n : ot(X(t)).left + n;
}
function Zt(t, e) {
  const n = t.getBoundingClientRect(), i = n.left + e.scrollLeft - ht(t, n), o = n.top + e.scrollTop;
  return {
    x: i,
    y: o
  };
}
function _e(t) {
  let {
    elements: e,
    rect: n,
    offsetParent: i,
    strategy: o
  } = t;
  const s = o === "fixed", r = X(i), l = e ? ut(e.floating) : !1;
  if (i === r || l && s)
    return n;
  let f = {
    scrollLeft: 0,
    scrollTop: 0
  }, a = z(1);
  const h = z(0), c = q(i);
  if ((c || !s) && ((et(i) !== "body" || ft(r)) && (f = dt(i)), c)) {
    const d = ot(i);
    a = tt(i), h.x = d.x + i.clientLeft, h.y = d.y + i.clientTop;
  }
  const u = r && !c && !s ? Zt(r, f) : z(0);
  return {
    width: n.width * a.x,
    height: n.height * a.y,
    x: n.x * a.x - f.scrollLeft * a.x + h.x + u.x,
    y: n.y * a.y - f.scrollTop * a.y + h.y + u.y
  };
}
function je(t) {
  return t.getClientRects ? Array.from(t.getClientRects()) : [];
}
function Ke(t) {
  const e = dt(t), n = t.ownerDocument.body, i = H(t.scrollWidth, t.clientWidth, n.scrollWidth, n.clientWidth), o = H(t.scrollHeight, t.clientHeight, n.scrollHeight, n.clientHeight);
  let s = -e.scrollLeft + ht(t);
  const r = -e.scrollTop;
  return V(n).direction === "rtl" && (s += H(t.clientWidth, n.clientWidth) - i), {
    width: i,
    height: o,
    x: s,
    y: r
  };
}
const Xe = 25;
function qe(t, e, n) {
  n === void 0 && (n = "viewport");
  const i = n === "layoutViewport", o = D(t), s = X(t), r = o.visualViewport;
  let l = s.clientWidth, f = s.clientHeight, a = 0, h = 0;
  if (r) {
    const u = !St() || e === "fixed";
    i ? u || (a = -r.offsetLeft, h = -r.offsetTop) : (l = r.width, f = r.height, u && (a = r.offsetLeft, h = r.offsetTop));
  }
  if (ht(s) <= 0) {
    const u = s.ownerDocument, d = u.body, g = getComputedStyle(d), m = u.compatMode === "CSS1Compat" && parseFloat(g.marginLeft) + parseFloat(g.marginRight) || 0, p = Math.abs(s.clientWidth - d.clientWidth - m), w = getComputedStyle(s).scrollbarGutter === "stable both-edges" ? p / 2 : p;
    w <= Xe && (l -= w);
  }
  return {
    width: l,
    height: f,
    x: a,
    y: h
  };
}
function Ye(t, e) {
  const n = ot(t, !0, e === "fixed"), i = n.top + t.clientTop, o = n.left + t.clientLeft, s = tt(t), r = t.clientWidth * s.x, l = t.clientHeight * s.y, f = o * s.x, a = i * s.y;
  return {
    width: r,
    height: l,
    x: f,
    y: a
  };
}
function $t(t, e, n) {
  let i;
  if (e === "viewport" || e === "layoutViewport")
    i = qe(t, n, e);
  else if (e === "document")
    i = Ke(X(t));
  else if ($(e))
    i = Ye(e, n);
  else {
    const o = Qt(t);
    i = {
      x: e.x - o.x,
      y: e.y - o.y,
      width: e.width,
      height: e.height
    };
  }
  return j(i);
}
function Je(t, e) {
  const n = e.get(t);
  if (n)
    return n;
  let i = Yt(t, []).filter((l) => $(l) && et(l) !== "body"), o = null;
  const s = V(t).position === "fixed";
  let r = s ? Z(t) : t;
  for (; $(r) && !it(r); ) {
    const l = V(r), f = Tt(r), a = o ? o.position : s ? "fixed" : "";
    !f && (a === "fixed" || a === "absolute" && l.position === "static") ? i = i.filter((c) => c !== r) : o = l, r = Z(r);
  }
  return e.set(t, i), i;
}
function Ge(t) {
  let {
    element: e,
    boundary: n,
    rootBoundary: i,
    strategy: o
  } = t;
  const r = [...n === "clippingAncestors" ? ut(e) ? [] : Je(e, this._c) : [].concat(n), i], l = $t(e, r[0], o);
  let f = l.top, a = l.right, h = l.bottom, c = l.left;
  for (let u = 1; u < r.length; u++) {
    const d = $t(e, r[u], o);
    f = H(d.top, f), a = U(d.right, a), h = U(d.bottom, h), c = H(d.left, c);
  }
  return {
    width: a - c,
    height: h - f,
    x: c,
    y: f
  };
}
function Qe(t) {
  const {
    width: e,
    height: n
  } = Jt(t);
  return {
    width: e,
    height: n
  };
}
function Ze(t, e, n) {
  const i = q(e), o = X(e), s = n === "fixed", r = ot(t, !0, s, e);
  let l = {
    scrollLeft: 0,
    scrollTop: 0
  };
  const f = z(0);
  if ((i || !s) && ((et(e) !== "body" || ft(o)) && (l = dt(e)), i)) {
    const u = ot(e, !0, s, e);
    f.x = u.x + e.clientLeft, f.y = u.y + e.clientTop;
  }
  !i && o && (f.x = ht(o));
  const a = o && !i && !s ? Zt(o, l) : z(0), h = r.left + l.scrollLeft - f.x - a.x, c = r.top + l.scrollTop - f.y - a.y;
  return {
    x: h,
    y: c,
    width: r.width,
    height: r.height
  };
}
function mt(t) {
  return V(t).position === "static";
}
function Vt(t, e) {
  if (!q(t) || V(t).position === "fixed")
    return null;
  if (e)
    return e(t);
  let n = t.offsetParent;
  return X(t) === n && (n = n.ownerDocument.body), n;
}
function te(t, e) {
  const n = D(t);
  if (ut(t))
    return n;
  if (!q(t)) {
    let o = Z(t);
    for (; o && !it(o); ) {
      if ($(o) && !mt(o))
        return o;
      o = Z(o);
    }
    return n;
  }
  let i = Vt(t, e);
  for (; i && $e(i) && mt(i); )
    i = Vt(i, e);
  return i && it(i) && mt(i) && !Tt(i) ? n : i || Fe(t) || n;
}
const tn = async function(t) {
  const e = this.getOffsetParent || te, n = this.getDimensions, i = await n(t.floating);
  return {
    reference: Ze(t.reference, await e(t.floating), t.strategy),
    floating: {
      x: 0,
      y: 0,
      width: i.width,
      height: i.height
    }
  };
};
function en(t) {
  return V(t).direction === "rtl";
}
const nn = {
  convertOffsetParentRelativeRectToViewportRelativeRect: _e,
  getDocumentElement: X,
  getClippingRect: Ge,
  getOffsetParent: te,
  getElementRects: tn,
  getClientRects: je,
  getDimensions: Qe,
  getScale: tt,
  isElement: $,
  isRTL: en
}, on = Ie, sn = De, rn = Be, ln = Ee, cn = Ue, an = Le, fn = Re, un = ke, dn = (t, e, n) => {
  const i = /* @__PURE__ */ new Map(), o = n ?? {}, s = {
    ...nn,
    ...o.platform,
    _c: i
  };
  return Se(t, e, {
    ...o,
    platform: s
  });
};
function hn(t, e) {
  const n = Math.min(t.top, e.top), i = Math.max(t.bottom, e.bottom), o = Math.min(t.left, e.left), s = Math.max(t.right, e.right) - o, r = i - n;
  return new DOMRect(o, n, s, r);
}
var gn = class {
  get middlewares() {
    const t = [];
    return this.floatingUIOptions.flip && t.push(ln(typeof this.floatingUIOptions.flip != "boolean" ? this.floatingUIOptions.flip : void 0)), this.floatingUIOptions.shift && t.push(rn(typeof this.floatingUIOptions.shift != "boolean" ? this.floatingUIOptions.shift : void 0)), this.floatingUIOptions.offset && t.push(on(typeof this.floatingUIOptions.offset != "boolean" ? this.floatingUIOptions.offset : void 0)), this.floatingUIOptions.arrow && t.push(fn(this.floatingUIOptions.arrow)), this.floatingUIOptions.size && t.push(cn(typeof this.floatingUIOptions.size != "boolean" ? this.floatingUIOptions.size : void 0)), this.floatingUIOptions.autoPlacement && t.push(sn(typeof this.floatingUIOptions.autoPlacement != "boolean" ? this.floatingUIOptions.autoPlacement : void 0)), this.floatingUIOptions.hide && t.push(an(typeof this.floatingUIOptions.hide != "boolean" ? this.floatingUIOptions.hide : void 0)), this.floatingUIOptions.inline && t.push(un(typeof this.floatingUIOptions.inline != "boolean" ? this.floatingUIOptions.inline : void 0)), t;
  }
  get virtualElement() {
    var t, e;
    const { selection: n } = this.editor.state, i = (t = this.getReferencedVirtualElement) === null || t === void 0 ? void 0 : t.call(this);
    if (i) return i;
    if (!(!((e = this.view) === null || e === void 0 || (e = e.dom) === null || e === void 0) && e.parentNode)) return;
    const o = re(this.view, n.from, n.to);
    let s = {
      getBoundingClientRect: () => o,
      getClientRects: () => [o]
    };
    if (n instanceof le) {
      let r = this.view.nodeDOM(n.from);
      const l = r.dataset.nodeViewWrapper ? r : r.querySelector("[data-node-view-wrapper]");
      l && (r = l), r && (s = {
        getBoundingClientRect: () => r.getBoundingClientRect(),
        getClientRects: () => [r.getBoundingClientRect()]
      });
    }
    if (n instanceof ce) {
      const { $anchorCell: r, $headCell: l } = n, f = r ? r.pos : l.pos, a = l ? l.pos : r.pos, h = this.view.nodeDOM(f), c = this.view.nodeDOM(a);
      if (!h || !c) return;
      const u = h === c ? h.getBoundingClientRect() : hn(h.getBoundingClientRect(), c.getBoundingClientRect());
      s = {
        getBoundingClientRect: () => u,
        getClientRects: () => [u]
      };
    }
    return s;
  }
  constructor({ editor: t, element: e, view: n, pluginKey: i = "bubbleMenu", updateDelay: o = 250, resizeDelay: s = 60, shouldShow: r, appendTo: l, getReferencedVirtualElement: f, options: a }) {
    var h;
    this.preventHide = !1, this.isVisible = !1, this.scrollTarget = window, this.floatingUIOptions = {
      strategy: "absolute",
      placement: "top",
      offset: 8,
      flip: {},
      shift: {},
      arrow: !1,
      size: !1,
      autoPlacement: !1,
      hide: !1,
      inline: !1,
      onShow: void 0,
      onHide: void 0,
      onUpdate: void 0,
      onDestroy: void 0
    }, this.shouldShow = ({ view: c, state: u, from: d, to: g }) => {
      const { doc: m, selection: p } = u, { empty: w } = p, v = !m.textBetween(d, g).length && ae(u.selection), b = this.element.contains(document.activeElement);
      return !(!(c.hasFocus() || b) || w || v || !this.editor.isEditable);
    }, this.mousedownHandler = () => {
      this.preventHide = !0;
    }, this.dragstartHandler = () => {
      this.hide();
    }, this.resizeHandler = () => {
      this.resizeDebounceTimer && clearTimeout(this.resizeDebounceTimer), this.resizeDebounceTimer = window.setTimeout(() => {
        this.updatePosition();
      }, this.resizeDelay);
    }, this.focusHandler = () => {
      setTimeout(() => this.update(this.editor.view));
    }, this.blurHandler = ({ event: c }) => {
      var u;
      if (this.editor.isDestroyed) {
        this.destroy();
        return;
      }
      if (this.preventHide) {
        this.preventHide = !1;
        return;
      }
      c?.relatedTarget && (!((u = this.element.parentNode) === null || u === void 0) && u.contains(c.relatedTarget)) || c?.relatedTarget !== this.editor.view.dom && this.hide();
    }, this.handleDebouncedUpdate = (c, u) => {
      const d = !u?.selection.eq(c.state.selection), g = !u?.doc.eq(c.state.doc);
      !d && !g || (this.updateDebounceTimer && clearTimeout(this.updateDebounceTimer), this.updateDebounceTimer = window.setTimeout(() => {
        this.updateHandler(c, d, g, u);
      }, this.updateDelay));
    }, this.updateHandler = (c, u, d, g) => {
      const { composing: m } = c;
      if (!(m || !u && !d)) {
        if (!this.getShouldShow(g)) {
          this.hide();
          return;
        }
        this.show(), this.updatePosition();
      }
    }, this.transactionHandler = ({ transaction: c }) => {
      const u = c.getMeta(this.pluginKey);
      u === "updatePosition" ? this.updatePosition() : u && typeof u == "object" && u.type === "updateOptions" ? this.updateOptions(u.options) : u === "hide" ? this.hide() : u === "show" && (this.updatePosition(), this.show());
    }, this.editor = t, this.element = e, this.view = n, this.pluginKey = i, this.updateDelay = o, this.resizeDelay = s, this.appendTo = l, this.scrollTarget = (h = a?.scrollTarget) !== null && h !== void 0 ? h : window, this.getReferencedVirtualElement = f, this.floatingUIOptions = {
      ...this.floatingUIOptions,
      ...a
    }, this.element.tabIndex = 0, r && (this.shouldShow = r), this.element.addEventListener("mousedown", this.mousedownHandler, { capture: !0 }), this.view.dom.addEventListener("dragstart", this.dragstartHandler), this.editor.on("focus", this.focusHandler), this.editor.on("blur", this.blurHandler), this.editor.on("transaction", this.transactionHandler), window.addEventListener("resize", this.resizeHandler), this.scrollTarget.addEventListener("scroll", this.resizeHandler), this.update(n, n.state), this.getShouldShow() && (this.show(), this.updatePosition());
  }
  updatePosition() {
    if (!this.isVisible) return;
    const t = this.virtualElement;
    t && dn(t, this.element, {
      placement: this.floatingUIOptions.placement,
      strategy: this.floatingUIOptions.strategy,
      middleware: this.middlewares
    }).then(({ x: e, y: n, strategy: i, middlewareData: o }) => {
      var s, r;
      if (!(!this.isVisible || this.editor.isDestroyed || !this.element.isConnected)) {
        if (!((s = o.hide) === null || s === void 0) && s.referenceHidden || !((r = o.hide) === null || r === void 0) && r.escaped) {
          this.element.style.visibility = "hidden";
          return;
        }
        this.element.style.visibility = "visible", this.element.style.width = "max-content", this.element.style.position = i, this.element.style.left = `${e}px`, this.element.style.top = `${n}px`, this.isVisible && this.floatingUIOptions.onUpdate && this.floatingUIOptions.onUpdate();
      }
    });
  }
  update(t, e) {
    const { state: n } = t, i = n.selection.from !== n.selection.to;
    if (this.updateDelay > 0 && i) {
      this.handleDebouncedUpdate(t, e);
      return;
    }
    const o = !e?.selection.eq(t.state.selection), s = !e?.doc.eq(t.state.doc);
    this.updateHandler(t, o, s, e);
  }
  getShouldShow(t) {
    var e;
    const { state: n } = this.view, { selection: i } = n, { ranges: o } = i, s = Math.min(...o.map((l) => l.$from.pos)), r = Math.max(...o.map((l) => l.$to.pos));
    return ((e = this.shouldShow) === null || e === void 0 ? void 0 : e.call(this, {
      editor: this.editor,
      element: this.element,
      view: this.view,
      state: n,
      oldState: t,
      from: s,
      to: r
    })) || !1;
  }
  show() {
    var t;
    if (this.isVisible) return;
    this.element.style.visibility = "visible", this.element.style.opacity = "1";
    const e = typeof this.appendTo == "function" ? this.appendTo() : this.appendTo;
    (t = e ?? this.view.dom.parentElement) === null || t === void 0 || t.appendChild(this.element), this.floatingUIOptions.onShow && this.floatingUIOptions.onShow(), this.isVisible = !0;
  }
  hide() {
    this.isVisible && (this.element.style.visibility = "hidden", this.element.style.opacity = "0", this.element.remove(), this.floatingUIOptions.onHide && this.floatingUIOptions.onHide(), this.isVisible = !1);
  }
  updateOptions(t) {
    if (t.updateDelay !== void 0 && (this.updateDelay = t.updateDelay), t.resizeDelay !== void 0 && (this.resizeDelay = t.resizeDelay), t.appendTo !== void 0 && (this.appendTo = t.appendTo), t.getReferencedVirtualElement !== void 0 && (this.getReferencedVirtualElement = t.getReferencedVirtualElement), t.shouldShow !== void 0 && t.shouldShow && (this.shouldShow = t.shouldShow), t.options !== void 0) {
      var e;
      const n = (e = t.options.scrollTarget) !== null && e !== void 0 ? e : window;
      n !== this.scrollTarget && (this.scrollTarget.removeEventListener("scroll", this.resizeHandler), this.scrollTarget = n, this.scrollTarget.addEventListener("scroll", this.resizeHandler)), this.floatingUIOptions = {
        ...this.floatingUIOptions,
        ...t.options
      };
    }
  }
  destroy() {
    this.hide(), this.element.removeEventListener("mousedown", this.mousedownHandler, { capture: !0 }), this.view.dom.removeEventListener("dragstart", this.dragstartHandler), window.removeEventListener("resize", this.resizeHandler), this.scrollTarget.removeEventListener("scroll", this.resizeHandler), this.editor.off("focus", this.focusHandler), this.editor.off("blur", this.blurHandler), this.editor.off("transaction", this.transactionHandler), this.floatingUIOptions.onDestroy && this.floatingUIOptions.onDestroy();
  }
};
const mn = (t) => new vt({
  key: typeof t.pluginKey == "string" ? new bt(t.pluginKey) : t.pluginKey,
  view: (e) => new gn({
    view: e,
    ...t
  })
}), pn = ct.create({
  name: "bubbleMenu",
  addOptions() {
    return {
      element: null,
      pluginKey: "bubbleMenu",
      updateDelay: void 0,
      appendTo: void 0,
      shouldShow: null
    };
  },
  addProseMirrorPlugins() {
    return this.options.element ? [mn({
      pluginKey: this.options.pluginKey,
      editor: this.editor,
      element: this.options.element,
      updateDelay: this.options.updateDelay,
      options: this.options.options,
      appendTo: this.options.appendTo,
      getReferencedVirtualElement: this.options.getReferencedVirtualElement,
      shouldShow: this.options.shouldShow
    })] : [];
  }
});
var wn = pn, yn = fe, vn = ue;
function K(t) {
  const e = (t ?? "html").toLowerCase();
  return e === "markdown" ? "markdown" : e === "json" ? "json" : "html";
}
function ee(t, e) {
  const n = K(e);
  return n === "markdown" ? t.getMarkdown() : n === "json" ? JSON.stringify(t.getJSON()) : t.getHTML();
}
const bn = ["bold", "italic", "underline", "strike", "code", "highlight", "link"], xn = [
  "heading",
  "paragraph",
  "blockquote",
  "codeBlock",
  "bulletList",
  "orderedList",
  "taskList",
  "table",
  "sufiCallout"
];
function nt(t) {
  if (!(t == null || t === ""))
    return String(t);
}
function ne(t, e, n) {
  const { from: i, to: o } = t.state.selection, s = bn.filter((d) => t.isActive(d)), r = xn.find((d) => t.isActive(d)) ?? t.state.selection.$from.parent.type.name, l = t.getAttributes("heading"), f = t.getAttributes("link"), a = t.getAttributes(r), h = nt(t.getAttributes("paragraph").textAlign) ?? nt(l.textAlign) ?? nt(a.textAlign), c = t.storage.characterCount?.words?.() ?? t.getText().trim().split(/\s+/).filter(Boolean).length, u = t.storage.characterCount?.characters?.() ?? t.getText().length;
  return {
    editorId: e,
    canUndo: t.can().undo(),
    canRedo: t.can().redo(),
    isEmpty: t.isEmpty,
    selectionText: t.state.doc.textBetween(i, o, " "),
    selectionFrom: i,
    selectionTo: o,
    activeNode: r,
    headingLevel: t.isActive("heading") ? Number(l.level ?? 0) : 0,
    contentFormat: n,
    characterCount: u,
    wordCount: c,
    activeMarks: s,
    activeMarkAttrs: {
      href: t.isActive("link") ? nt(f.href) : void 0
    },
    activeNodeAttrs: {
      ...Object.fromEntries(
        Object.entries(a ?? {}).map(([d, g]) => [d, nt(g)])
      ),
      textAlign: h
    }
  };
}
function On(t, e) {
  if (e.stripAllFormatting)
    return t.replace(/<[^>]+>/g, " ");
  let n = t;
  return e.cleanWordHtml && (n = n.replace(/<!--\[if[\s\S]*?<!\[endif\]-->/gi, "").replace(/\s(class|style)="Mso[^"]*"/gi, "")), e.removeInlineStyles && (n = n.replace(/\sstyle="[^"]*"/gi, "")), e.removeCssClasses && (n = n.replace(/\sclass="[^"]*"/gi, "")), n = n.replace(/<script[\s\S]*?<\/script>/gi, ""), n = n.replace(/\son\w+="[^"]*"/gi, ""), n;
}
const Cn = ct.create({
  name: "sufiPasteCleanup",
  addOptions() {
    return {
      stripAllFormatting: !1,
      cleanWordHtml: !0,
      removeInlineStyles: !1,
      removeCssClasses: !1
    };
  },
  addProseMirrorPlugins() {
    const t = this.options;
    return [
      new vt({
        key: new bt("sufiPasteCleanup"),
        props: {
          transformPastedHTML(e) {
            return On(e, t);
          }
        }
      })
    ];
  }
}), Q = new bt("sufiAiSuggestion");
function Nt(t, e, n) {
  return Math.max(e, Math.min(n, t));
}
function Rt(t, e) {
  const n = Nt(e.from || 1, 1, t);
  if (e.kind === "insert" && (!e.to || e.to <= 0))
    return { from: n, to: n };
  const i = e.to && e.to > 0 ? e.to : t, o = Nt(i, n, t);
  return { from: n, to: o };
}
function Tn(t) {
  return Array.isArray(t) ? t.map((e) => {
    const n = e ?? {}, i = String(n.kind ?? n.Kind ?? "replace").toLowerCase(), o = i === "insert" || i === "delete" ? i : "replace";
    return {
      id: String(n.id ?? n.Id ?? ""),
      kind: o,
      from: Number(n.from ?? n.From ?? 1),
      to: Number(n.to ?? n.To ?? 0),
      insertText: n.insertText != null || n.InsertText != null ? String(n.insertText ?? n.InsertText) : void 0,
      deleteText: n.deleteText != null || n.DeleteText != null ? String(n.deleteText ?? n.DeleteText) : void 0
    };
  }).filter((e) => e.id.length > 0) : [];
}
function Sn(t) {
  const e = document.createElement("span");
  return e.className = "sb-suggestion sb-suggestion--insert", e.setAttribute("data-sb-suggestion", t.id), e.textContent = t.insertText ?? "", e;
}
function Ft(t, e) {
  const n = [], i = t.content.size;
  for (const o of e) {
    const { from: s, to: r } = Rt(i, o);
    (o.kind === "delete" || o.kind === "replace") && r > s && n.push(
      Dt.inline(s, r, {
        class: "sb-suggestion sb-suggestion--delete",
        "data-sb-suggestion": o.id
      })
    ), (o.kind === "insert" || o.kind === "replace") && (o.insertText ?? "").length > 0 && n.push(
      Dt.widget(o.kind === "insert" ? s : r, () => Sn(o), {
        side: 1,
        key: `sb-suggestion-insert-${o.id}`
      })
    );
  }
  return wt.create(t, n);
}
const Rn = ct.create({
  name: "sufiAiSuggestion",
  addProseMirrorPlugins() {
    return [
      new vt({
        key: Q,
        state: {
          init: () => ({ decorations: wt.empty, items: [] }),
          apply(t, e) {
            const n = {
              items: e.items,
              decorations: e.decorations.map(t.mapping, t.doc)
            }, i = t.getMeta(Q);
            if (!i)
              return n;
            if (Array.isArray(i))
              return { items: i, decorations: Ft(t.doc, i) };
            if (i.clear)
              return { items: [], decorations: wt.empty };
            const o = n.items.filter((s) => s.id !== i.accept && s.id !== i.reject);
            return { items: o, decorations: Ft(t.doc, o) };
          }
        },
        props: {
          decorations(t) {
            return Q.getState(t)?.decorations;
          }
        }
      })
    ];
  }
});
function ie(t) {
  return Q.getState(t.state)?.items ?? [];
}
function An(t, e) {
  t.dispatch(t.state.tr.setMeta(Q, e));
}
function Dn(t) {
  t.dispatch(t.state.tr.setMeta(Q, { clear: !0 }));
}
function oe(t, e, n) {
  t.dispatch(t.state.tr.setMeta(Q, n === "accept" ? { accept: e } : { reject: e }));
}
function En(t, e, n) {
  const i = ie(t), o = (e ? i.find((h) => h.id === e) : i[0]) ?? null;
  if (!o)
    return null;
  const { from: s, to: r } = Rt(t.state.doc.content.size, o);
  let l, f;
  try {
    l = t.coordsAtPos(s), f = t.coordsAtPos(Math.max(s, r));
  } catch {
    return null;
  }
  const a = n ?? t.dom.getBoundingClientRect();
  return {
    top: Math.min(l.top, f.top) - a.top,
    left: Math.min(l.left, f.left) - a.left,
    width: Math.max(8, Math.abs(f.right - l.left)),
    height: Math.max(8, Math.abs(f.bottom - l.top))
  };
}
const Ln = ct.create({
  name: "sufiKeymap",
  addKeyboardShortcuts() {
    return {
      "Mod-s": () => (this.editor.emit("sufiShortcut", { name: "save" }), !0),
      "Mod-p": () => (this.editor.emit("sufiShortcut", { name: "preview" }), !0)
    };
  }
}), C = /* @__PURE__ */ new Map(), At = /* @__PURE__ */ new Map();
let Mn = 1;
function pt(t, e, n, i) {
  t.invokeMethodAsync("OnEditorStateChanged", JSON.stringify(ne(e, n, i)));
}
function Bn(t, e, n = {}, i) {
  const o = `sb-rte-${Mn++}`, s = (n.contentFormat ?? "html").toLowerCase(), r = n.features ?? Et.Default, l = [
    ...de({ features: r }),
    yn.configure({ placeholder: n.placeholder ?? "" }),
    vn,
    Cn.configure(n.pasteCleanup ?? {}),
    Rn,
    Ln
  ];
  i && he(r, Et.BubbleMenu) && l.push(
    wn.configure({
      element: i,
      shouldShow: ({ editor: a, from: h, to: c }) => a.isEditable && h !== c
    })
  );
  const f = new ge({
    element: t,
    editable: !(n.readOnly || n.disabled),
    content: n.content ?? "",
    contentType: K(s),
    extensions: l,
    editorProps: {
      attributes: {
        class: "sb-editor__prose",
        dir: n.direction ?? "ltr",
        role: "textbox",
        "aria-multiline": "true"
      }
    },
    onUpdate: ({ editor: a }) => {
      e.invokeMethodAsync(
        "OnEditorContentChanged",
        ee(a, s),
        a.getHTML(),
        a.getText()
      ), pt(e, a, o, s);
    },
    onSelectionUpdate: ({ editor: a }) => {
      pt(e, a, o, s);
    },
    onCreate: ({ editor: a }) => {
      pt(e, a, o, s);
    }
  });
  return f.on("sufiShortcut", (a) => {
    e.invokeMethodAsync("OnEditorShortcut", a.name);
  }), C.set(o, f), At.set(o, s), o;
}
function Un(t) {
  const e = C.get(t);
  e && (e.destroy(), C.delete(t), At.delete(t));
}
function $n(t, e) {
  const n = C.get(t);
  return n ? ee(n, e) : "";
}
function Vn(t, e, n) {
  const i = C.get(t);
  i && i.commands.setContent(e ?? "", { contentType: K(n) });
}
function Nn(t) {
  C.get(t)?.commands.focus();
}
function Fn(t, e) {
  const n = C.get(t);
  n && n.setEditable(e);
}
function zn(t, e) {
  C.get(t)?.view.dom.setAttribute("dir", e);
}
function Wn(t, e, n) {
  const i = C.get(t);
  if (!i)
    return !1;
  const o = i.chain().focus();
  switch (e) {
    case "Undo":
      return o.undo().run();
    case "Redo":
      return o.redo().run();
    case "Bold":
      return o.toggleBold().run();
    case "Italic":
      return o.toggleItalic().run();
    case "Underline":
      return o.toggleUnderline().run();
    case "Strike":
      return o.toggleStrike().run();
    case "Code":
      return o.toggleCode().run();
    case "Highlight":
      return o.toggleHighlight().run();
    case "Blockquote":
      return o.toggleBlockquote().run();
    case "CodeBlock":
      return o.toggleCodeBlock().run();
    case "BulletList":
      return o.toggleBulletList().run();
    case "OrderedList":
      return o.toggleOrderedList().run();
    case "TaskList":
      return o.toggleTaskList().run();
    case "Heading1":
    case "Heading2":
    case "Heading3":
    case "Heading4":
    case "Heading5":
    case "Heading6":
      return o.toggleHeading({ level: Number(e.replace("Heading", "")) }).run();
    case "Paragraph":
      return o.setParagraph().run();
    case "AlignLeft":
      return o.setTextAlign("left").run();
    case "AlignCenter":
      return o.setTextAlign("center").run();
    case "AlignRight":
      return o.setTextAlign("right").run();
    case "AlignJustify":
      return o.setTextAlign("justify").run();
    case "HorizontalRule":
      return o.setHorizontalRule().run();
    case "ClearFormatting":
      return o.unsetAllMarks().clearNodes().run();
    case "InsertTable":
      return o.insertTable({ rows: 3, cols: 3, withHeaderRow: !0 }).run();
    case "InsertCallout":
      return o.insertContent({
        type: "sufiCallout",
        attrs: { kind: n || "note" },
        content: [{ type: "paragraph" }]
      }).run();
    default:
      return !1;
  }
}
function kn(t, e, n) {
  const i = C.get(t);
  i && i.chain().focus().insertContent(e ?? "", { contentType: K(n) }).run();
}
function Hn(t, e, n, i, o) {
  if (!me(e))
    return;
  const s = C.get(t);
  s && (s.state.selection.empty && n && s.chain().focus().insertContent(n).run(), s.chain().focus().extendMarkRange("link").setLink({ href: e, target: i || null, rel: o || "noopener noreferrer" }).run());
}
function _n(t, e, n, i, o) {
  C.get(t)?.chain().focus().setImage({ src: e, alt: n ?? "", width: i, height: o }).run();
}
function jn(t, e, n) {
  Hn(t, e, n, "_blank", "noopener noreferrer");
}
function Kn(t, e, n) {
  const i = C.get(t);
  if (i) {
    if (e === "textStyle" || e === "fontFamily") {
      i.chain().focus().setFontFamily(String(n?.fontFamily ?? n?.font ?? "")).run();
      return;
    }
    i.chain().focus().setMark(e, n ?? {}).run();
  }
}
function Xn(t, e, n) {
  const i = C.get(t);
  if (i) {
    if (e === "heading") {
      i.chain().focus().setHeading({ level: Number(n?.level ?? 1) }).run();
      return;
    }
    e === "paragraph" && i.chain().focus().setParagraph().run();
  }
}
function qn(t) {
  const e = C.get(t);
  if (!e)
    return { text: "", from: 0, to: 0, nodeType: "" };
  const { from: n, to: i } = e.state.selection;
  return {
    text: e.state.doc.textBetween(n, i, " "),
    from: n,
    to: i,
    nodeType: Pn(e)
  };
}
function Pn(t) {
  return ["heading", "paragraph", "blockquote", "codeBlock", "bulletList", "orderedList", "taskList", "table", "sufiCallout"].find(
    (e) => t.isActive(e)
  ) ?? t.state.selection.$from.parent.type.name;
}
function Yn(t, e, n) {
  C.get(t)?.chain().focus().insertContent(e ?? "", { contentType: K(n) }).run();
}
function Jn(t, e) {
  const n = C.get(t);
  if (!n)
    return;
  const i = Tn(JSON.parse(e || "[]"));
  An(n.view, i);
}
function Gn(t, e) {
  const n = C.get(t);
  if (!n)
    return;
  const i = ie(n.view).find((a) => a.id === e);
  if (!i)
    return;
  const o = At.get(t) ?? "html", { from: s, to: r } = Rt(n.state.doc.content.size, i), l = i.insertText ?? "", f = s <= 1 && r >= n.state.doc.content.size;
  i.kind === "delete" ? n.chain().focus().deleteRange({ from: s, to: r }).run() : i.kind === "insert" ? n.chain().focus().insertContentAt(s, l, { contentType: K(o) }).run() : f ? n.commands.setContent(l, { contentType: K(o) }) : n.chain().focus().insertContentAt({ from: s, to: r }, l, { contentType: K(o) }).run(), oe(n.view, e, "accept");
}
function Qn(t, e) {
  const n = C.get(t);
  n && oe(n.view, e, "reject");
}
function Zn(t) {
  const e = C.get(t);
  e && Dn(e.view);
}
function ti(t, e) {
  const n = C.get(t);
  if (!n)
    return null;
  const i = n.view.dom.closest(".sb-editor-host")?.getBoundingClientRect() ?? n.view.dom.closest(".sb-editor")?.getBoundingClientRect();
  return En(n.view, e, i);
}
function ei(t, e) {
  kn(t, e, "html");
}
function ni(t, e) {
  const n = C.get(t);
  return n ? JSON.stringify(ne(n, t, e)) : "{}";
}
export {
  Gn as acceptSuggestion,
  Xn as applyBlock,
  Kn as applyMark,
  Zn as clearEditorSuggestions,
  Un as destroyEditor,
  Wn as execCommand,
  Nn as focusEditor,
  $n as getContent,
  qn as getSelection,
  ni as getState,
  ti as getSuggestionRect,
  Bn as initEditor,
  kn as insertContent,
  jn as insertFile,
  _n as insertImage,
  Hn as insertLink,
  Qn as rejectSuggestion,
  Yn as replaceSelection,
  Vn as setContent,
  zn as setDirection,
  Fn as setEditable,
  Jn as showSuggestions,
  ei as streamInsert
};
