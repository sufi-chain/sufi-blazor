import { E as at, P as Ot, a as Ct, p as fe, N as ue, C as de, i as he, b as ge, c as me, D as yt, d as kt, e as Lt, f as pe, h as we, g as ye, j as ve } from "../chunks/schema-BC-lZYXa.js";
const _t = ["top", "right", "bottom", "left"], Mt = ["start", "end"], Ht = /* @__PURE__ */ _t.reduce((t, e) => t.concat(e, e + "-" + Mt[0], e + "-" + Mt[1]), []), U = Math.min, H = Math.max, rt = Math.round, z = (t) => ({
  x: t,
  y: t
}), be = {
  left: "right",
  right: "left",
  bottom: "top",
  top: "bottom"
};
function jt(t, e, n) {
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
function Kt(t) {
  return t === "x" ? "y" : "x";
}
function Tt(t) {
  return t === "y" ? "height" : "width";
}
function B(t) {
  const e = t[0];
  return e === "t" || e === "b" ? "y" : "x";
}
function Rt(t) {
  return Kt(B(t));
}
function Xt(t, e, n) {
  n === void 0 && (n = !1);
  const i = P(t), o = Rt(t), s = Tt(o);
  let r = o === "x" ? i === (n ? "end" : "start") ? "right" : "left" : i === "start" ? "bottom" : "top";
  return e.reference[s] > e.floating[s] && (r = ct(r)), [r, ct(r)];
}
function xe(t) {
  const e = ct(t);
  return [lt(t), e, lt(e)];
}
function lt(t) {
  return t.includes("start") ? t.replace("start", "end") : t.replace("end", "start");
}
const Pt = ["left", "right"], It = ["right", "left"], Oe = ["top", "bottom"], Ce = ["bottom", "top"];
function Te(t, e, n) {
  switch (t) {
    case "top":
    case "bottom":
      return n ? e ? It : Pt : e ? Pt : It;
    case "left":
    case "right":
      return e ? Oe : Ce;
    default:
      return [];
  }
}
function Re(t, e, n, i) {
  const o = P(t);
  let s = Te(I(t), n === "start", i);
  return o && (s = s.map((r) => r + "-" + o), e && (s = s.concat(s.map(lt)))), s;
}
function ct(t) {
  const e = I(t);
  return be[e] + t.slice(e.length);
}
function Se(t) {
  var e, n, i, o;
  return {
    top: (e = t.top) != null ? e : 0,
    right: (n = t.right) != null ? n : 0,
    bottom: (i = t.bottom) != null ? i : 0,
    left: (o = t.left) != null ? o : 0
  };
}
function St(t) {
  return typeof t != "number" ? Se(t) : {
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
function Bt(t, e, n) {
  let {
    reference: i,
    floating: o
  } = t;
  const s = B(e), r = Rt(e), l = Tt(r), a = I(e), u = s === "y", h = i.x + i.width / 2 - o.width / 2, c = i.y + i.height / 2 - o.height / 2, f = i[l] / 2 - o[l] / 2;
  let d;
  switch (a) {
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
  return g && (d[r] += f * (g === "end" ? 1 : -1) * (n && u ? -1 : 1)), d;
}
async function Ae(t, e) {
  var n;
  e === void 0 && (e = {});
  const {
    x: i,
    y: o,
    platform: s,
    rects: r,
    elements: l,
    strategy: a
  } = t, {
    boundary: u = "clippingAncestors",
    rootBoundary: h = "viewport",
    elementContext: c = "floating",
    altBoundary: f = !1,
    padding: d = 0
  } = W(e, t), g = St(d), p = l[f ? c === "floating" ? "reference" : "floating" : c], w = j(await s.getClippingRect({
    element: (n = await (s.isElement == null ? void 0 : s.isElement(p))) == null || n ? p : p.contextElement || await (s.getDocumentElement == null ? void 0 : s.getDocumentElement(l.floating)),
    boundary: u,
    rootBoundary: h,
    strategy: a
  })), v = c === "floating" ? {
    x: i,
    y: o,
    width: r.floating.width,
    height: r.floating.height
  } : r.reference, b = await (s.getOffsetParent == null ? void 0 : s.getOffsetParent(l.floating)), x = await (s.isElement == null ? void 0 : s.isElement(b)) && await (s.getScale == null ? void 0 : s.getScale(b)) || {
    x: 1,
    y: 1
  }, C = j(s.convertOffsetParentRelativeRectToViewportRelativeRect ? await s.convertOffsetParentRelativeRectToViewportRelativeRect({
    elements: l,
    rect: v,
    offsetParent: b,
    strategy: a
  }) : v);
  return {
    top: (w.top - C.top + g.top) / x.y,
    bottom: (C.bottom - w.bottom + g.bottom) / x.y,
    left: (w.left - C.left + g.left) / x.x,
    right: (C.right - w.right + g.right) / x.x
  };
}
const Ee = 50, De = async (t, e, n) => {
  const {
    placement: i = "bottom",
    strategy: o = "absolute",
    middleware: s = [],
    platform: r
  } = n, l = r.detectOverflow ? r : {
    ...r,
    detectOverflow: Ae
  }, a = await (r.isRTL == null ? void 0 : r.isRTL(e));
  let u = await r.getElementRects({
    reference: t,
    floating: e,
    strategy: o
  }), {
    x: h,
    y: c
  } = Bt(u, i, a), f = i, d = 0;
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
      data: C,
      reset: y
    } = await v({
      x: h,
      y: c,
      initialPlacement: i,
      placement: f,
      strategy: o,
      middlewareData: g,
      rects: u,
      platform: l,
      elements: {
        reference: t,
        floating: e
      }
    });
    h = b ?? h, c = x ?? c, g[w] = {
      ...g[w],
      ...C
    }, y && d < Ee && (d++, typeof y == "object" && (y.placement && (f = y.placement), y.rects && (u = y.rects === !0 ? await r.getElementRects({
      reference: t,
      floating: e,
      strategy: o
    }) : y.rects), {
      x: h,
      y: c
    } = Bt(u, f, a)), m = -1);
  }
  return {
    x: h,
    y: c,
    placement: f,
    strategy: o,
    middlewareData: g
  };
}, ke = (t) => ({
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
      middlewareData: a
    } = e, {
      element: u,
      padding: h = 0
    } = W(t, e) || {};
    if (u == null)
      return {};
    const c = St(h), f = {
      x: n,
      y: i
    }, d = Rt(o), g = Tt(d), m = await r.getDimensions(u), p = d === "y", w = p ? "top" : "left", v = p ? "bottom" : "right", b = p ? "clientHeight" : "clientWidth", x = s.reference[g] + s.reference[d] - f[d] - s.floating[g], C = f[d] - s.reference[d], y = await (r.getOffsetParent == null ? void 0 : r.getOffsetParent(u));
    let R = y ? y[b] : 0;
    (!R || !await (r.isElement == null ? void 0 : r.isElement(y))) && (R = l.floating[b] || s.floating[g]);
    const D = x / 2 - C / 2, A = R / 2 - m[g] / 2 - 1, O = U(c[w], A), S = U(c[v], A), V = R - m[g] - S, k = R / 2 - m[g] / 2 + D, L = jt(O, k, V), Y = !a.arrow && P(o) != null && k !== L && s.reference[g] / 2 - (k < O ? O : S) - m[g] / 2 < 0, F = Y ? k < O ? k - O : k - V : 0;
    return {
      [d]: f[d] + F,
      data: {
        [d]: L,
        centerOffset: k - L - F,
        ...Y && {
          alignmentOffset: F
        }
      },
      reset: Y
    };
  }
});
function Le(t, e, n) {
  return (t ? [...n.filter((o) => P(o) === t), ...n.filter((o) => P(o) !== t)] : n.filter((o) => I(o) === o)).filter((o) => t ? P(o) === t || (e ? lt(o) !== o : !1) : !0);
}
const Me = function(t) {
  return t === void 0 && (t = {}), {
    name: "autoPlacement",
    options: t,
    async fn(e) {
      var n, i, o;
      const {
        rects: s,
        middlewareData: r,
        placement: l,
        platform: a,
        elements: u
      } = e, {
        crossAxis: h = !1,
        alignment: c,
        allowedPlacements: f = Ht,
        autoAlignment: d = !0,
        ...g
      } = W(t, e), m = c !== void 0 || f === Ht ? Le(c || null, d, f) : f, p = ((n = r.autoPlacement) == null ? void 0 : n.index) || 0, w = m[p];
      if (w == null)
        return {};
      if (l !== w)
        return {
          reset: {
            placement: m[0]
          }
        };
      const v = await a.detectOverflow(e, g), b = Xt(w, s, await (a.isRTL == null ? void 0 : a.isRTL(u.floating))), x = [v[I(w)], v[b[0]], v[b[1]]], C = [...((i = r.autoPlacement) == null ? void 0 : i.overflows) || [], {
        placement: w,
        overflows: x
      }], y = m[p + 1];
      if (y)
        return {
          data: {
            index: p + 1,
            overflows: C
          },
          reset: {
            placement: y
          }
        };
      const R = C.map((O) => {
        const S = P(O.placement);
        return [O.placement, S && h ? (
          // Check along the mainAxis and main crossAxis side.
          O.overflows.slice(0, 2).reduce((V, k) => V + k, 0)
        ) : (
          // Check only the mainAxis.
          O.overflows[0]
        ), O.overflows];
      }).sort((O, S) => O[1] - S[1]), A = ((o = R.filter((O) => O[2].slice(
        0,
        // Aligned placements should not check their opposite crossAxis
        // side.
        P(O[0]) ? 2 : 3
      ).every((S) => S <= 0))[0]) == null ? void 0 : o[0]) || R[0][0];
      return A !== l ? {
        data: {
          index: p + 1,
          overflows: C
        },
        reset: {
          placement: A
        }
      } : {};
    }
  };
}, He = function(t) {
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
        platform: a,
        elements: u
      } = e, {
        mainAxis: h = !0,
        crossAxis: c = !0,
        fallbackPlacements: f,
        fallbackStrategy: d = "bestFit",
        fallbackAxisSideDirection: g = "none",
        flipAlignment: m = !0,
        ...p
      } = W(t, e);
      if ((n = s.arrow) != null && n.alignmentOffset)
        return {};
      const w = I(o), v = B(l), b = I(l) === l, x = await (a.isRTL == null ? void 0 : a.isRTL(u.floating)), C = f || (b || !m ? [ct(l)] : xe(l)), y = g !== "none";
      !f && y && C.push(...Re(l, m, g, x));
      const R = [l, ...C], D = await a.detectOverflow(e, p), A = [];
      let O = ((i = s.flip) == null ? void 0 : i.overflows) || [];
      if (h && A.push(D[w]), c) {
        const L = Xt(o, r, x);
        A.push(D[L[0]], D[L[1]]);
      }
      if (O = [...O, {
        placement: o,
        overflows: A
      }], !A.every((L) => L <= 0)) {
        var S, V;
        const L = (((S = s.flip) == null ? void 0 : S.index) || 0) + 1, Y = R[L];
        if (Y && (!(c === "alignment" ? v !== B(Y) : !1) || // We leave the current main axis only if every placement on that axis
        // overflows the main axis.
        O.every((M) => B(M.placement) === v ? M.overflows[0] > 0 : !0)))
          return {
            data: {
              index: L,
              overflows: O
            },
            reset: {
              placement: Y
            }
          };
        let F = (V = O.filter((J) => J.overflows[0] <= 0).sort((J, M) => J.overflows[1] - M.overflows[1])[0]) == null ? void 0 : V.placement;
        if (!F)
          switch (d) {
            case "bestFit": {
              var k;
              const J = (k = O.filter((M) => {
                if (y) {
                  const _ = B(M.placement);
                  return _ === v || // Create a bias to the `y` side axis due to horizontal
                  // reading directions favoring greater width.
                  _ === "y";
                }
                return !0;
              }).map((M) => [M.placement, M.overflows.filter((_) => _ > 0).reduce((_, ae) => _ + ae, 0)]).sort((M, _) => M[1] - _[1])[0]) == null ? void 0 : k[0];
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
function Ut(t, e) {
  return {
    top: t.top - e.height,
    right: t.right - e.width,
    bottom: t.bottom - e.height,
    left: t.left - e.width
  };
}
function Nt(t) {
  return _t.some((e) => t[e] >= 0);
}
const Pe = function(t) {
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
          }), l = Ut(r, n.reference);
          return {
            data: {
              referenceHiddenOffsets: l,
              referenceHidden: Nt(l)
            }
          };
        }
        case "escaped": {
          const r = await i.detectOverflow(e, {
            ...s,
            altBoundary: !0
          }), l = Ut(r, n.floating);
          return {
            data: {
              escapedOffsets: l,
              escaped: Nt(l)
            }
          };
        }
        default:
          return {};
      }
    }
  };
};
function qt(t) {
  const e = U(...t.map((s) => s.left)), n = U(...t.map((s) => s.top)), i = H(...t.map((s) => s.right)), o = H(...t.map((s) => s.bottom));
  return {
    x: e,
    y: n,
    width: i - e,
    height: o - n
  };
}
function Ie(t) {
  const e = t.slice().sort((o, s) => o.y - s.y), n = [];
  let i = null;
  for (let o = 0; o < e.length; o++) {
    const s = e[o];
    !i || s.y - i.y > i.height / 2 ? n.push([s]) : n[n.length - 1].push(s), i = s;
  }
  return n.map((o) => j(qt(o)));
}
const Be = function(t) {
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
        x: a,
        y: u
      } = W(t, e), h = Array.from(await (s.getClientRects == null ? void 0 : s.getClientRects(i.reference)) || []);
      if (!h.length)
        return {};
      const c = Ie(h), f = j(qt(h)), d = St(l);
      function g() {
        if (c.length === 2 && (c[0].left > c[1].right || c[1].left > c[0].right) && a != null && u != null)
          return c.find((p) => a > p.left - d.left && a < p.right + d.right && u > p.top - d.top && u < p.bottom + d.bottom) || f;
        if (c.length >= 2) {
          if (B(n) === "y") {
            const y = c[0], R = c[c.length - 1], D = I(n) === "top", A = y.top, O = R.bottom, S = D ? y.left : R.left, V = D ? y.right : R.right;
            return j({
              x: S,
              y: A,
              width: V - S,
              height: O - A
            });
          }
          const p = I(n) === "left", w = H(...c.map((y) => y.right)), v = U(...c.map((y) => y.left)), b = c.filter((y) => p ? y.left === v : y.right === w), x = b[0].top, C = b[b.length - 1].bottom;
          return j({
            x: v,
            y: x,
            width: w - v,
            height: C - x
          });
        }
        return f;
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
}, Ue = /* @__PURE__ */ new Set(["left", "top"]);
async function Ne(t, e) {
  const {
    placement: n,
    platform: i,
    elements: o
  } = t, s = await (i.isRTL == null ? void 0 : i.isRTL(o.floating)), r = I(n), l = P(n), a = B(n) === "y", u = Ue.has(r) ? -1 : 1, h = s && a ? -1 : 1, c = W(e, t);
  let {
    mainAxis: f,
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
  return l && typeof g == "number" && (d = l === "end" ? g * -1 : g), a ? {
    x: d * h,
    y: f * u
  } : {
    x: f * u,
    y: d * h
  };
}
const $e = function(t) {
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
      } = e, a = await Ne(e, t);
      return r === ((n = l.offset) == null ? void 0 : n.placement) && (i = l.arrow) != null && i.alignmentOffset ? {} : {
        x: o + a.x,
        y: s + a.y,
        data: {
          ...a,
          placement: r
        }
      };
    }
  };
}, Ve = function(t) {
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
        limiter: a = {
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
        ...u
      } = W(t, e), h = {
        x: n,
        y: i
      }, c = await s.detectOverflow(e, u), f = B(o), d = Kt(f);
      let g = h[d], m = h[f];
      const p = (v, b) => jt(b + c[v === "y" ? "top" : "left"], b, b - c[v === "y" ? "bottom" : "right"]);
      r && (g = p(d, g)), l && (m = p(f, m));
      const w = a.fn({
        ...e,
        [d]: g,
        [f]: m
      });
      return {
        ...w,
        data: {
          x: w.x - n,
          y: w.y - i,
          enabled: {
            [d]: r,
            [f]: l
          }
        }
      };
    }
  };
}, Fe = function(t) {
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
      } = W(t, e), a = await o.detectOverflow(e, l), u = I(n), h = P(n), c = B(n) === "y", {
        width: f,
        height: d
      } = i.floating;
      let g, m;
      u === "top" || u === "bottom" ? (g = u, m = h === (await (o.isRTL == null ? void 0 : o.isRTL(s.floating)) ? "start" : "end") ? "left" : "right") : (m = u, g = h === "end" ? "top" : "bottom");
      const p = d - a.top - a.bottom, w = f - a.left - a.right, v = U(d - a[g], p), b = U(f - a[m], w), x = e.middlewareData.shift, C = !x;
      let y = v, R = b;
      x != null && x.enabled.x && (R = w), x != null && x.enabled.y && (y = p), C && !h && (c ? R = f - 2 * H(a.left, a.right) : y = d - 2 * H(a.top, a.bottom)), await r({
        ...e,
        availableWidth: R,
        availableHeight: y
      });
      const D = await o.getDimensions(s.floating);
      return f !== D.width || d !== D.height ? {
        reset: {
          rects: !0
        }
      } : {};
    }
  };
};
function ft() {
  return typeof window < "u";
}
function nt(t) {
  return Yt(t) ? (t.nodeName || "").toLowerCase() : "#document";
}
function E(t) {
  var e;
  return (t == null || (e = t.ownerDocument) == null ? void 0 : e.defaultView) || window;
}
function X(t) {
  var e;
  return (e = (Yt(t) ? t.ownerDocument : t.document) || window.document) == null ? void 0 : e.documentElement;
}
function Yt(t) {
  return ft() ? t instanceof Node || t instanceof E(t).Node : !1;
}
function N(t) {
  return ft() ? t instanceof Element || t instanceof E(t).Element : !1;
}
function q(t) {
  return ft() ? t instanceof HTMLElement || t instanceof E(t).HTMLElement : !1;
}
function $t(t) {
  return !ft() || typeof ShadowRoot > "u" ? !1 : t instanceof ShadowRoot || t instanceof E(t).ShadowRoot;
}
function ut(t) {
  const {
    overflow: e,
    overflowX: n,
    overflowY: i,
    display: o
  } = $(t);
  return /auto|scroll|overlay|hidden|clip/.test(e + i + n) && o !== "inline" && o !== "contents";
}
function ze(t) {
  return /^(table|td|th)$/.test(nt(t));
}
function dt(t) {
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
const We = /transform|translate|scale|rotate|perspective|filter/, _e = /paint|layout|strict|content/, G = (t) => !!t && t !== "none";
let mt;
function At(t) {
  const e = N(t) ? $(t) : t;
  return G(e.transform) || G(e.translate) || G(e.scale) || G(e.rotate) || G(e.perspective) || !Et() && (G(e.backdropFilter) || G(e.filter)) || We.test(e.willChange || "") || _e.test(e.contain || "");
}
function je(t) {
  let e = Z(t);
  for (; q(e) && !ot(e); ) {
    if (At(e))
      return e;
    if (dt(e))
      return null;
    e = Z(e);
  }
  return null;
}
function Et() {
  return mt == null && (mt = typeof CSS < "u" && CSS.supports && CSS.supports("-webkit-backdrop-filter", "none")), mt;
}
function ot(t) {
  return /^(html|body|#document)$/.test(nt(t));
}
function $(t) {
  return E(t).getComputedStyle(t);
}
function ht(t) {
  return N(t) ? {
    scrollLeft: t.scrollLeft,
    scrollTop: t.scrollTop
  } : {
    scrollLeft: t.scrollX,
    scrollTop: t.scrollY
  };
}
function Z(t) {
  if (nt(t) === "html")
    return t;
  const e = (
    // Step into the shadow DOM of the parent of a slotted node.
    t.assignedSlot || // DOM Element detected.
    t.parentNode || // ShadowRoot detected.
    $t(t) && t.host || // Fallback.
    X(t)
  );
  return $t(e) ? e.host : e;
}
function Jt(t) {
  const e = Z(t);
  return ot(e) ? (t.ownerDocument || t).body : q(e) && ut(e) ? e : Jt(e);
}
function Gt(t, e, n) {
  var i;
  e === void 0 && (e = []);
  const o = Jt(t), s = o === ((i = t.ownerDocument) == null ? void 0 : i.body), r = E(o);
  return s ? (vt(r), e.concat(r, r.visualViewport || [], ut(o) ? o : [], [])) : e.concat(o, Gt(o, []));
}
function vt(t) {
  return t.parent && Object.getPrototypeOf(t.parent) ? t.frameElement : null;
}
function Qt(t) {
  const e = $(t);
  let n = parseFloat(e.width) || 0, i = parseFloat(e.height) || 0;
  const o = q(t), s = o ? t.offsetWidth : n, r = o ? t.offsetHeight : i, l = rt(n) !== s || rt(i) !== r;
  return l && (n = s, i = r), {
    width: n,
    height: i,
    $: l
  };
}
function Zt(t) {
  return N(t) ? t : t.contextElement;
}
function et(t) {
  const e = Zt(t);
  if (!q(e))
    return z(1);
  const n = e.getBoundingClientRect(), {
    width: i,
    height: o,
    $: s
  } = Qt(e);
  let r = (s ? rt(n.width) : n.width) / i, l = (s ? rt(n.height) : n.height) / o;
  return (!r || !Number.isFinite(r)) && (r = 1), (!l || !Number.isFinite(l)) && (l = 1), {
    x: r,
    y: l
  };
}
const Ke = /* @__PURE__ */ z(0);
function te(t) {
  const e = E(t);
  return !Et() || !e.visualViewport ? Ke : {
    x: e.visualViewport.offsetLeft,
    y: e.visualViewport.offsetTop
  };
}
function Xe(t, e, n) {
  return e === void 0 && (e = !1), !!n && e && n === E(t);
}
function st(t, e, n, i) {
  e === void 0 && (e = !1), n === void 0 && (n = !1);
  const o = t.getBoundingClientRect(), s = Zt(t);
  let r = z(1);
  e && (i ? N(i) && (r = et(i)) : r = et(t));
  const l = Xe(s, n, i) ? te(s) : z(0);
  let a = (o.left + l.x) / r.x, u = (o.top + l.y) / r.y, h = o.width / r.x, c = o.height / r.y;
  if (s && i) {
    const f = E(s), d = N(i) ? E(i) : i;
    let g = f, m = vt(g);
    for (; m && d !== g; ) {
      const p = et(m), w = m.getBoundingClientRect(), v = $(m), b = w.left + (m.clientLeft + parseFloat(v.paddingLeft)) * p.x, x = w.top + (m.clientTop + parseFloat(v.paddingTop)) * p.y;
      a *= p.x, u *= p.y, h *= p.x, c *= p.y, a += b, u += x, g = E(m), m = vt(g);
    }
  }
  return j({
    width: h,
    height: c,
    x: a,
    y: u
  });
}
function gt(t, e) {
  const n = ht(t).scrollLeft;
  return e ? e.left + n : st(X(t)).left + n;
}
function ee(t, e) {
  const n = t.getBoundingClientRect(), i = n.left + e.scrollLeft - gt(t, n), o = n.top + e.scrollTop;
  return {
    x: i,
    y: o
  };
}
function qe(t) {
  let {
    elements: e,
    rect: n,
    offsetParent: i,
    strategy: o
  } = t;
  const s = o === "fixed", r = X(i), l = e ? dt(e.floating) : !1;
  if (i === r || l && s)
    return n;
  let a = {
    scrollLeft: 0,
    scrollTop: 0
  }, u = z(1);
  const h = z(0), c = q(i);
  if ((c || !s) && ((nt(i) !== "body" || ut(r)) && (a = ht(i)), c)) {
    const d = st(i);
    u = et(i), h.x = d.x + i.clientLeft, h.y = d.y + i.clientTop;
  }
  const f = r && !c && !s ? ee(r, a) : z(0);
  return {
    width: n.width * u.x,
    height: n.height * u.y,
    x: n.x * u.x - a.scrollLeft * u.x + h.x + f.x,
    y: n.y * u.y - a.scrollTop * u.y + h.y + f.y
  };
}
function Ye(t) {
  return t.getClientRects ? Array.from(t.getClientRects()) : [];
}
function Je(t) {
  const e = ht(t), n = t.ownerDocument.body, i = H(t.scrollWidth, t.clientWidth, n.scrollWidth, n.clientWidth), o = H(t.scrollHeight, t.clientHeight, n.scrollHeight, n.clientHeight);
  let s = -e.scrollLeft + gt(t);
  const r = -e.scrollTop;
  return $(n).direction === "rtl" && (s += H(t.clientWidth, n.clientWidth) - i), {
    width: i,
    height: o,
    x: s,
    y: r
  };
}
const Ge = 25;
function Qe(t, e, n) {
  n === void 0 && (n = "viewport");
  const i = n === "layoutViewport", o = E(t), s = X(t), r = o.visualViewport;
  let l = s.clientWidth, a = s.clientHeight, u = 0, h = 0;
  if (r) {
    const f = !Et() || e === "fixed";
    i ? f || (u = -r.offsetLeft, h = -r.offsetTop) : (l = r.width, a = r.height, f && (u = r.offsetLeft, h = r.offsetTop));
  }
  if (gt(s) <= 0) {
    const f = s.ownerDocument, d = f.body, g = getComputedStyle(d), m = f.compatMode === "CSS1Compat" && parseFloat(g.marginLeft) + parseFloat(g.marginRight) || 0, p = Math.abs(s.clientWidth - d.clientWidth - m), w = getComputedStyle(s).scrollbarGutter === "stable both-edges" ? p / 2 : p;
    w <= Ge && (l -= w);
  }
  return {
    width: l,
    height: a,
    x: u,
    y: h
  };
}
function Ze(t, e) {
  const n = st(t, !0, e === "fixed"), i = n.top + t.clientTop, o = n.left + t.clientLeft, s = et(t), r = t.clientWidth * s.x, l = t.clientHeight * s.y, a = o * s.x, u = i * s.y;
  return {
    width: r,
    height: l,
    x: a,
    y: u
  };
}
function Vt(t, e, n) {
  let i;
  if (e === "viewport" || e === "layoutViewport")
    i = Qe(t, n, e);
  else if (e === "document")
    i = Je(X(t));
  else if (N(e))
    i = Ze(e, n);
  else {
    const o = te(t);
    i = {
      x: e.x - o.x,
      y: e.y - o.y,
      width: e.width,
      height: e.height
    };
  }
  return j(i);
}
function tn(t, e) {
  const n = e.get(t);
  if (n)
    return n;
  let i = Gt(t, []).filter((l) => N(l) && nt(l) !== "body"), o = null;
  const s = $(t).position === "fixed";
  let r = s ? Z(t) : t;
  for (; N(r) && !ot(r); ) {
    const l = $(r), a = At(r), u = o ? o.position : s ? "fixed" : "";
    !a && (u === "fixed" || u === "absolute" && l.position === "static") ? i = i.filter((c) => c !== r) : o = l, r = Z(r);
  }
  return e.set(t, i), i;
}
function en(t) {
  let {
    element: e,
    boundary: n,
    rootBoundary: i,
    strategy: o
  } = t;
  const r = [...n === "clippingAncestors" ? dt(e) ? [] : tn(e, this._c) : [].concat(n), i], l = Vt(e, r[0], o);
  let a = l.top, u = l.right, h = l.bottom, c = l.left;
  for (let f = 1; f < r.length; f++) {
    const d = Vt(e, r[f], o);
    a = H(d.top, a), u = U(d.right, u), h = U(d.bottom, h), c = H(d.left, c);
  }
  return {
    width: u - c,
    height: h - a,
    x: c,
    y: a
  };
}
function nn(t) {
  const {
    width: e,
    height: n
  } = Qt(t);
  return {
    width: e,
    height: n
  };
}
function on(t, e, n) {
  const i = q(e), o = X(e), s = n === "fixed", r = st(t, !0, s, e);
  let l = {
    scrollLeft: 0,
    scrollTop: 0
  };
  const a = z(0);
  if ((i || !s) && ((nt(e) !== "body" || ut(o)) && (l = ht(e)), i)) {
    const f = st(e, !0, s, e);
    a.x = f.x + e.clientLeft, a.y = f.y + e.clientTop;
  }
  !i && o && (a.x = gt(o));
  const u = o && !i && !s ? ee(o, l) : z(0), h = r.left + l.scrollLeft - a.x - u.x, c = r.top + l.scrollTop - a.y - u.y;
  return {
    x: h,
    y: c,
    width: r.width,
    height: r.height
  };
}
function pt(t) {
  return $(t).position === "static";
}
function Ft(t, e) {
  if (!q(t) || $(t).position === "fixed")
    return null;
  if (e)
    return e(t);
  let n = t.offsetParent;
  return X(t) === n && (n = n.ownerDocument.body), n;
}
function ne(t, e) {
  const n = E(t);
  if (dt(t))
    return n;
  if (!q(t)) {
    let o = Z(t);
    for (; o && !ot(o); ) {
      if (N(o) && !pt(o))
        return o;
      o = Z(o);
    }
    return n;
  }
  let i = Ft(t, e);
  for (; i && ze(i) && pt(i); )
    i = Ft(i, e);
  return i && ot(i) && pt(i) && !At(i) ? n : i || je(t) || n;
}
const sn = async function(t) {
  const e = this.getOffsetParent || ne, n = this.getDimensions, i = await n(t.floating);
  return {
    reference: on(t.reference, await e(t.floating), t.strategy),
    floating: {
      x: 0,
      y: 0,
      width: i.width,
      height: i.height
    }
  };
};
function rn(t) {
  return $(t).direction === "rtl";
}
const ln = {
  convertOffsetParentRelativeRectToViewportRelativeRect: qe,
  getDocumentElement: X,
  getClippingRect: en,
  getOffsetParent: ne,
  getElementRects: sn,
  getClientRects: Ye,
  getDimensions: nn,
  getScale: et,
  isElement: N,
  isRTL: rn
}, cn = $e, an = Me, fn = Ve, un = He, dn = Fe, hn = Pe, gn = ke, mn = Be, pn = (t, e, n) => {
  const i = /* @__PURE__ */ new Map(), o = n ?? {}, s = {
    ...ln,
    ...o.platform,
    _c: i
  };
  return De(t, e, {
    ...o,
    platform: s
  });
};
function wn(t, e) {
  const n = Math.min(t.top, e.top), i = Math.max(t.bottom, e.bottom), o = Math.min(t.left, e.left), s = Math.max(t.right, e.right) - o, r = i - n;
  return new DOMRect(o, n, s, r);
}
var yn = class {
  get middlewares() {
    const t = [];
    return this.floatingUIOptions.flip && t.push(un(typeof this.floatingUIOptions.flip != "boolean" ? this.floatingUIOptions.flip : void 0)), this.floatingUIOptions.shift && t.push(fn(typeof this.floatingUIOptions.shift != "boolean" ? this.floatingUIOptions.shift : void 0)), this.floatingUIOptions.offset && t.push(cn(typeof this.floatingUIOptions.offset != "boolean" ? this.floatingUIOptions.offset : void 0)), this.floatingUIOptions.arrow && t.push(gn(this.floatingUIOptions.arrow)), this.floatingUIOptions.size && t.push(dn(typeof this.floatingUIOptions.size != "boolean" ? this.floatingUIOptions.size : void 0)), this.floatingUIOptions.autoPlacement && t.push(an(typeof this.floatingUIOptions.autoPlacement != "boolean" ? this.floatingUIOptions.autoPlacement : void 0)), this.floatingUIOptions.hide && t.push(hn(typeof this.floatingUIOptions.hide != "boolean" ? this.floatingUIOptions.hide : void 0)), this.floatingUIOptions.inline && t.push(mn(typeof this.floatingUIOptions.inline != "boolean" ? this.floatingUIOptions.inline : void 0)), t;
  }
  get virtualElement() {
    var t, e;
    const { selection: n } = this.editor.state, i = (t = this.getReferencedVirtualElement) === null || t === void 0 ? void 0 : t.call(this);
    if (i) return i;
    if (!(!((e = this.view) === null || e === void 0 || (e = e.dom) === null || e === void 0) && e.parentNode)) return;
    const o = fe(this.view, n.from, n.to);
    let s = {
      getBoundingClientRect: () => o,
      getClientRects: () => [o]
    };
    if (n instanceof ue) {
      let r = this.view.nodeDOM(n.from);
      const l = r.dataset.nodeViewWrapper ? r : r.querySelector("[data-node-view-wrapper]");
      l && (r = l), r && (s = {
        getBoundingClientRect: () => r.getBoundingClientRect(),
        getClientRects: () => [r.getBoundingClientRect()]
      });
    }
    if (n instanceof de) {
      const { $anchorCell: r, $headCell: l } = n, a = r ? r.pos : l.pos, u = l ? l.pos : r.pos, h = this.view.nodeDOM(a), c = this.view.nodeDOM(u);
      if (!h || !c) return;
      const f = h === c ? h.getBoundingClientRect() : wn(h.getBoundingClientRect(), c.getBoundingClientRect());
      s = {
        getBoundingClientRect: () => f,
        getClientRects: () => [f]
      };
    }
    return s;
  }
  constructor({ editor: t, element: e, view: n, pluginKey: i = "bubbleMenu", updateDelay: o = 250, resizeDelay: s = 60, shouldShow: r, appendTo: l, getReferencedVirtualElement: a, options: u }) {
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
    }, this.shouldShow = ({ view: c, state: f, from: d, to: g }) => {
      const { doc: m, selection: p } = f, { empty: w } = p, v = !m.textBetween(d, g).length && he(f.selection), b = this.element.contains(document.activeElement);
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
      var f;
      if (this.editor.isDestroyed) {
        this.destroy();
        return;
      }
      if (this.preventHide) {
        this.preventHide = !1;
        return;
      }
      c?.relatedTarget && (!((f = this.element.parentNode) === null || f === void 0) && f.contains(c.relatedTarget)) || c?.relatedTarget !== this.editor.view.dom && this.hide();
    }, this.handleDebouncedUpdate = (c, f) => {
      const d = !f?.selection.eq(c.state.selection), g = !f?.doc.eq(c.state.doc);
      !d && !g || (this.updateDebounceTimer && clearTimeout(this.updateDebounceTimer), this.updateDebounceTimer = window.setTimeout(() => {
        this.updateHandler(c, d, g, f);
      }, this.updateDelay));
    }, this.updateHandler = (c, f, d, g) => {
      const { composing: m } = c;
      if (!(m || !f && !d)) {
        if (!this.getShouldShow(g)) {
          this.hide();
          return;
        }
        this.show(), this.updatePosition();
      }
    }, this.transactionHandler = ({ transaction: c }) => {
      const f = c.getMeta(this.pluginKey);
      f === "updatePosition" ? this.updatePosition() : f && typeof f == "object" && f.type === "updateOptions" ? this.updateOptions(f.options) : f === "hide" ? this.hide() : f === "show" && (this.updatePosition(), this.show());
    }, this.editor = t, this.element = e, this.view = n, this.pluginKey = i, this.updateDelay = o, this.resizeDelay = s, this.appendTo = l, this.scrollTarget = (h = u?.scrollTarget) !== null && h !== void 0 ? h : window, this.getReferencedVirtualElement = a, this.floatingUIOptions = {
      ...this.floatingUIOptions,
      ...u
    }, this.element.tabIndex = 0, r && (this.shouldShow = r), this.element.addEventListener("mousedown", this.mousedownHandler, { capture: !0 }), this.view.dom.addEventListener("dragstart", this.dragstartHandler), this.editor.on("focus", this.focusHandler), this.editor.on("blur", this.blurHandler), this.editor.on("transaction", this.transactionHandler), window.addEventListener("resize", this.resizeHandler), this.scrollTarget.addEventListener("scroll", this.resizeHandler), this.update(n, n.state), this.getShouldShow() && (this.show(), this.updatePosition());
  }
  updatePosition() {
    if (!this.isVisible) return;
    const t = this.virtualElement;
    t && pn(t, this.element, {
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
const vn = (t) => new Ot({
  key: typeof t.pluginKey == "string" ? new Ct(t.pluginKey) : t.pluginKey,
  view: (e) => new yn({
    view: e,
    ...t
  })
}), bn = at.create({
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
    return this.options.element ? [vn({
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
var xn = bn, On = ge, Cn = me;
function K(t) {
  const e = (t ?? "html").toLowerCase();
  return e === "markdown" ? "markdown" : e === "json" ? "json" : "html";
}
function ie(t, e) {
  const n = K(e);
  return n === "markdown" ? t.getMarkdown() : n === "json" ? JSON.stringify(t.getJSON()) : t.getHTML();
}
const Tn = ["bold", "italic", "underline", "strike", "code", "highlight", "link"], Rn = [
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
function it(t) {
  if (!(t == null || t === ""))
    return String(t);
}
function oe(t, e, n) {
  const { from: i, to: o } = t.state.selection, s = Tn.filter((d) => t.isActive(d)), r = Rn.find((d) => t.isActive(d)) ?? t.state.selection.$from.parent.type.name, l = t.getAttributes("heading"), a = t.getAttributes("link"), u = t.getAttributes(r), h = it(t.getAttributes("paragraph").textAlign) ?? it(l.textAlign) ?? it(u.textAlign), c = t.storage.characterCount?.words?.() ?? t.getText().trim().split(/\s+/).filter(Boolean).length, f = t.storage.characterCount?.characters?.() ?? t.getText().length;
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
    characterCount: f,
    wordCount: c,
    activeMarks: s,
    activeMarkAttrs: {
      href: t.isActive("link") ? it(a.href) : void 0
    },
    activeNodeAttrs: {
      ...Object.fromEntries(
        Object.entries(u ?? {}).map(([d, g]) => [d, it(g)])
      ),
      textAlign: h
    }
  };
}
function Sn(t, e) {
  if (e.stripAllFormatting)
    return t.replace(/<[^>]+>/g, " ");
  let n = t;
  return e.cleanWordHtml && (n = n.replace(/<!--\[if[\s\S]*?<!\[endif\]-->/gi, "").replace(/\s(class|style)="Mso[^"]*"/gi, "")), e.removeInlineStyles && (n = n.replace(/\sstyle="[^"]*"/gi, "")), e.removeCssClasses && (n = n.replace(/\sclass="[^"]*"/gi, "")), n = n.replace(/<script[\s\S]*?<\/script>/gi, ""), n = n.replace(/\son\w+="[^"]*"/gi, ""), n;
}
const An = at.create({
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
      new Ot({
        key: new Ct("sufiPasteCleanup"),
        props: {
          transformPastedHTML(e) {
            return Sn(e, t);
          }
        }
      })
    ];
  }
}), Q = new Ct("sufiAiSuggestion");
function zt(t, e, n) {
  return Math.max(e, Math.min(n, t));
}
function Dt(t, e) {
  const n = zt(e.from || 1, 1, t);
  if (e.kind === "insert" && (!e.to || e.to <= 0))
    return { from: n, to: n };
  const i = e.to && e.to > 0 ? e.to : t, o = zt(i, n, t);
  return { from: n, to: o };
}
function En(t) {
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
function Dn(t) {
  const e = document.createElement("span");
  return e.className = "sb-suggestion sb-suggestion--insert", e.setAttribute("data-sb-suggestion", t.id), e.textContent = t.insertText ?? "", e;
}
function Wt(t, e) {
  const n = [], i = t.content.size;
  for (const o of e) {
    const { from: s, to: r } = Dt(i, o);
    (o.kind === "delete" || o.kind === "replace") && r > s && n.push(
      kt.inline(s, r, {
        class: "sb-suggestion sb-suggestion--delete",
        "data-sb-suggestion": o.id
      })
    ), (o.kind === "insert" || o.kind === "replace") && (o.insertText ?? "").length > 0 && n.push(
      kt.widget(o.kind === "insert" ? s : r, () => Dn(o), {
        side: 1,
        key: `sb-suggestion-insert-${o.id}`
      })
    );
  }
  return yt.create(t, n);
}
const kn = at.create({
  name: "sufiAiSuggestion",
  addProseMirrorPlugins() {
    return [
      new Ot({
        key: Q,
        state: {
          init: () => ({ decorations: yt.empty, items: [] }),
          apply(t, e) {
            const n = {
              items: e.items,
              decorations: e.decorations.map(t.mapping, t.doc)
            }, i = t.getMeta(Q);
            if (!i)
              return n;
            if (Array.isArray(i))
              return { items: i, decorations: Wt(t.doc, i) };
            if (i.clear)
              return { items: [], decorations: yt.empty };
            const o = n.items.filter((s) => s.id !== i.accept && s.id !== i.reject);
            return { items: o, decorations: Wt(t.doc, o) };
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
function se(t) {
  return Q.getState(t.state)?.items ?? [];
}
function Ln(t, e) {
  t.dispatch(t.state.tr.setMeta(Q, e));
}
function Mn(t) {
  t.dispatch(t.state.tr.setMeta(Q, { clear: !0 }));
}
function re(t, e, n) {
  t.dispatch(t.state.tr.setMeta(Q, n === "accept" ? { accept: e } : { reject: e }));
}
function Hn(t, e, n) {
  const i = se(t), o = (e ? i.find((h) => h.id === e) : i[0]) ?? null;
  if (!o)
    return null;
  const { from: s, to: r } = Dt(t.state.doc.content.size, o);
  let l, a;
  try {
    l = t.coordsAtPos(s), a = t.coordsAtPos(Math.max(s, r));
  } catch {
    return null;
  }
  const u = n ?? t.dom.getBoundingClientRect();
  return {
    top: Math.min(l.top, a.top) - u.top,
    left: Math.min(l.left, a.left) - u.left,
    width: Math.max(8, Math.abs(a.right - l.left)),
    height: Math.max(8, Math.abs(a.bottom - l.top))
  };
}
const Pn = at.create({
  name: "sufiKeymap",
  addKeyboardShortcuts() {
    return {
      "Mod-s": () => (this.editor.emit("sufiShortcut", { name: "save" }), !0),
      "Mod-p": () => (this.editor.emit("sufiShortcut", { name: "preview" }), !0)
    };
  }
}), le = /* @__PURE__ */ new Set();
function ce(t) {
  t && le.add(t);
}
function bt(t) {
  return !!t && !le.has(t);
}
const tt = /* @__PURE__ */ new Map();
let In = 1;
function T(t) {
  return tt.get(t)?.editor;
}
function xt(t, e, ...n) {
  const i = t.dotNetRef;
  !i || !bt(t.token) || i.invokeMethodAsync(e, ...n).catch(() => {
  });
}
function wt(t, e) {
  t.editor && xt(
    t,
    "OnEditorStateChanged",
    JSON.stringify(oe(t.editor, e, t.format))
  );
}
function Vn(t) {
  if (ce(t), !!t)
    for (const [e, n] of tt)
      n.token === t && (n.dotNetRef = null, n.editor.destroy(), tt.delete(e));
}
function Fn(t, e, n = {}, i) {
  const o = n.callbackToken ?? "";
  if (!bt(o))
    return "";
  const s = `sb-rte-${In++}`, r = (n.contentFormat ?? "html").toLowerCase(), l = n.features ?? Lt.Default, a = {
    editor: void 0,
    format: r,
    token: o,
    dotNetRef: e
  }, u = [
    ...pe({ features: l }),
    On.configure({ placeholder: n.placeholder ?? "" }),
    Cn,
    An.configure(n.pasteCleanup ?? {}),
    kn,
    Pn
  ];
  i && we(l, Lt.BubbleMenu) && u.push(
    xn.configure({
      element: i,
      shouldShow: ({ editor: c, from: f, to: d }) => c.isEditable && f !== d
    })
  );
  const h = new ye({
    element: t,
    editable: !(n.readOnly || n.disabled),
    content: n.content ?? "",
    contentType: K(r),
    extensions: u,
    editorProps: {
      attributes: {
        class: "sb-editor__prose",
        dir: n.direction ?? "ltr",
        role: "textbox",
        "aria-multiline": "true"
      }
    },
    onUpdate: ({ editor: c }) => {
      a.editor = c, xt(
        a,
        "OnEditorContentChanged",
        ie(c, r),
        c.getHTML(),
        c.getText()
      ), wt(a, s);
    },
    onSelectionUpdate: ({ editor: c }) => {
      a.editor = c, wt(a, s);
    },
    onCreate: ({ editor: c }) => {
      a.editor = c, wt(a, s);
    }
  });
  return a.editor = h, bt(o) ? (h.on("sufiShortcut", (c) => {
    xt(a, "OnEditorShortcut", c.name);
  }), tt.set(s, a), s) : (a.dotNetRef = null, h.destroy(), "");
}
function zn(t) {
  const e = tt.get(t);
  e && (ce(e.token), e.dotNetRef = null, e.editor.destroy(), tt.delete(t));
}
function Wn(t, e) {
  const n = T(t);
  return n ? ie(n, e) : "";
}
function _n(t, e, n) {
  const i = T(t);
  i && i.commands.setContent(e ?? "", { contentType: K(n) });
}
function jn(t) {
  T(t)?.commands.focus();
}
function Kn(t, e) {
  const n = T(t);
  n && n.setEditable(e);
}
function Xn(t, e) {
  T(t)?.view.dom.setAttribute("dir", e);
}
function qn(t, e, n) {
  const i = T(t);
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
function Bn(t, e, n) {
  const i = T(t);
  i && i.chain().focus().insertContent(e ?? "", { contentType: K(n) }).run();
}
function Un(t, e, n, i, o) {
  if (!ve(e))
    return;
  const s = T(t);
  s && (s.state.selection.empty && n && s.chain().focus().insertContent(n).run(), s.chain().focus().extendMarkRange("link").setLink({ href: e, target: i || null, rel: o || "noopener noreferrer" }).run());
}
function Yn(t, e, n, i, o) {
  T(t)?.chain().focus().setImage({ src: e, alt: n ?? "", width: i, height: o }).run();
}
function Jn(t, e, n) {
  Un(t, e, n, "_blank", "noopener noreferrer");
}
function Gn(t, e, n) {
  const i = T(t);
  if (i) {
    if (e === "textStyle" || e === "fontFamily") {
      i.chain().focus().setFontFamily(String(n?.fontFamily ?? n?.font ?? "")).run();
      return;
    }
    i.chain().focus().setMark(e, n ?? {}).run();
  }
}
function Qn(t, e, n) {
  const i = T(t);
  if (i) {
    if (e === "heading") {
      i.chain().focus().setHeading({ level: Number(n?.level ?? 1) }).run();
      return;
    }
    e === "paragraph" && i.chain().focus().setParagraph().run();
  }
}
function Zn(t) {
  const e = T(t);
  if (!e)
    return { text: "", from: 0, to: 0, nodeType: "" };
  const { from: n, to: i } = e.state.selection;
  return {
    text: e.state.doc.textBetween(n, i, " "),
    from: n,
    to: i,
    nodeType: Nn(e)
  };
}
function Nn(t) {
  return ["heading", "paragraph", "blockquote", "codeBlock", "bulletList", "orderedList", "taskList", "table", "sufiCallout"].find(
    (e) => t.isActive(e)
  ) ?? t.state.selection.$from.parent.type.name;
}
function ti(t, e, n) {
  T(t)?.chain().focus().insertContent(e ?? "", { contentType: K(n) }).run();
}
function ei(t, e) {
  const n = T(t);
  if (!n)
    return;
  const i = En(JSON.parse(e || "[]"));
  Ln(n.view, i);
}
function ni(t, e) {
  const n = T(t);
  if (!n)
    return;
  const i = se(n.view).find((u) => u.id === e);
  if (!i)
    return;
  const o = tt.get(t)?.format ?? "html", { from: s, to: r } = Dt(n.state.doc.content.size, i), l = i.insertText ?? "", a = s <= 1 && r >= n.state.doc.content.size;
  i.kind === "delete" ? n.chain().focus().deleteRange({ from: s, to: r }).run() : i.kind === "insert" ? n.chain().focus().insertContentAt(s, l, { contentType: K(o) }).run() : a ? n.commands.setContent(l, { contentType: K(o) }) : n.chain().focus().insertContentAt({ from: s, to: r }, l, { contentType: K(o) }).run(), re(n.view, e, "accept");
}
function ii(t, e) {
  const n = T(t);
  n && re(n.view, e, "reject");
}
function oi(t) {
  const e = T(t);
  e && Mn(e.view);
}
function si(t, e) {
  const n = T(t);
  if (!n)
    return null;
  const i = n.view.dom.closest(".sb-editor-host")?.getBoundingClientRect() ?? n.view.dom.closest(".sb-editor")?.getBoundingClientRect();
  return Hn(n.view, e, i);
}
function ri(t, e) {
  Bn(t, e, "html");
}
function li(t, e) {
  const n = T(t);
  return n ? JSON.stringify(oe(n, t, e)) : "{}";
}
export {
  ni as acceptSuggestion,
  Qn as applyBlock,
  Gn as applyMark,
  oi as clearEditorSuggestions,
  zn as destroyEditor,
  Vn as detachEditorCallback,
  qn as execCommand,
  jn as focusEditor,
  Wn as getContent,
  Zn as getSelection,
  li as getState,
  si as getSuggestionRect,
  Fn as initEditor,
  Bn as insertContent,
  Jn as insertFile,
  Yn as insertImage,
  Un as insertLink,
  ii as rejectSuggestion,
  ti as replaceSelection,
  _n as setContent,
  Xn as setDirection,
  Kn as setEditable,
  ei as showSuggestions,
  ri as streamInsert
};
