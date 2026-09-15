import { r as Nt, k as Tt, s as yt, l as St, m as Ke, n as At, o as bn, q as Ot, S as Mt, f as wt, M as vt } from "./schema-Dvjie1O7.js";
function Rt(n, t, a) {
  const i = n.attrs;
  return i ? t.filter((o) => o.type !== (typeof n.type == "string" ? n.type : n.type.name) ? !1 : o.attribute.rendered).map((o) => o.attribute.renderHTML ? o.attribute.renderHTML(i) || { [o.name]: o.name in i ? i[o.name] : o.attribute.default } : { [o.name]: o.name in i ? i[o.name] : o.attribute.default }).reduce((o, c) => Ot(o, c), {}) : {};
}
function En(n, t) {
  return Rt(n, t);
}
function kt(n, t) {
  return t?.textDirection ? [St.TextDirection.configure({ direction: t.textDirection }), ...n] : n;
}
function xt(n, t, a, i) {
  const o = {
    name: t.name,
    options: t.options,
    storage: t.storage,
    parent: t.parent
  }, c = bn(t, "renderHTML", o);
  return c ? [t.name, ({ node: u, children: g }) => {
    try {
      return n(c({
        node: u,
        HTMLAttributes: En(u, a)
      }))(g);
    } catch (_) {
      throw new Error(`[tiptap error]: Node ${t.name} cannot be rendered, it's "renderToHTML" method threw an error: ${_.message}`, { cause: _ });
    }
  }] : i?.unhandledNode ? [t.name, i.unhandledNode] : [t.name, () => {
    throw new Error(`[tiptap error]: Node ${t.name} cannot be rendered, it is missing a "renderToHTML" method, please implement it or override the corresponding "nodeMapping" method to have a custom rendering`);
  }];
}
function It(n, t, a, i) {
  const o = {
    name: t.name,
    options: t.options,
    storage: t.storage,
    parent: t.parent
  }, c = bn(t, "renderHTML", o);
  return c ? [t.name, ({ mark: u, children: g }) => {
    try {
      return n(c({
        mark: u,
        HTMLAttributes: En(u, a)
      }))(g);
    } catch (_) {
      throw new Error(`[tiptap error]: Mark ${t.name} cannot be rendered, it's "renderToHTML" method threw an error: ${_.message}`, { cause: _ });
    }
  }] : i?.unhandledMark ? [t.name, i.unhandledMark] : [t.name, () => {
    throw new Error(`Node ${t.name} cannot be rendered, it is missing a "renderToHTML" method`);
  }];
}
const De = "__tiptapUnhandledNode__", Be = "__tiptapUnhandledMark__", Ae = "__originalType", Oe = "__originalAttrs", un = {
  [Ae]: { default: "" },
  [Oe]: { default: {} }
};
function Lt(n, t) {
  let a = !1;
  const i = (o) => {
    var c, u;
    a || (o.type && !(o.type in t.nodes) && (a = !0), (c = o.marks) === null || c === void 0 || c.forEach((g) => {
      g.type && !(g.type in t.marks) && (a = !0);
    }), (u = o.content) === null || u === void 0 || u.forEach(i));
  };
  return i(n), a;
}
function Ct(n, t, a) {
  const i = !!a?.unhandledNode, o = !!a?.unhandledMark, c = (h) => {
    var A;
    return o && !(h.type in t.marks) ? {
      type: Be,
      attrs: {
        [Ae]: h.type,
        [Oe]: (A = h.attrs) !== null && A !== void 0 ? A : {}
      }
    } : h;
  }, u = (h) => i && h.type != null && !(h.type in t.nodes), g = (h, A) => {
    var y;
    return {
      ...A,
      type: De,
      attrs: {
        [Ae]: h.type,
        [Oe]: (y = h.attrs) !== null && y !== void 0 ? y : {}
      }
    };
  }, _ = (h) => {
    var A, y;
    const O = {
      ...h,
      marks: (A = h.marks) === null || A === void 0 ? void 0 : A.map(c),
      content: (y = h.content) === null || y === void 0 ? void 0 : y.map(_)
    };
    return u(h) ? g(h, O) : O;
  };
  return _(n);
}
function Dt(n) {
  return new Mt({
    topNode: n.spec.topNode,
    nodes: n.spec.nodes.addToEnd(De, { attrs: un }),
    marks: n.spec.marks.addToEnd(Be, { attrs: un })
  });
}
function Bt(n, t, a) {
  if (n instanceof Ke) return n;
  const i = At(t);
  return Lt(n, i) ? Ke.fromJSON(Dt(i), Ct(n, i, a)) : Ke.fromJSON(i, n);
}
function gn(n) {
  return n.type === De || n.type === Be ? {
    ...n,
    type: n.attrs[Ae],
    attrs: n.attrs[Oe]
  } : n;
}
function hn(n) {
  var t, a;
  const i = gn(n);
  return {
    ...i,
    marks: (t = i.marks) === null || t === void 0 ? void 0 : t.map((o) => gn(o)),
    content: (a = i.content) === null || a === void 0 ? void 0 : a.map(hn)
  };
}
function mn(n) {
  const t = Object.assign(/* @__PURE__ */ Object.create(null), {
    type: { name: n.attrs[Ae] },
    attrs: n.attrs[Oe],
    toJSON: () => hn(n.toJSON())
  });
  return new Proxy(n, { get(a, i) {
    const o = t[i];
    if (o !== void 0) return o;
    const c = a[i];
    return typeof c == "function" ? c.bind(a) : c;
  } });
}
function $t(n) {
  return (t) => n({
    ...t,
    node: mn(t.node)
  });
}
function Ut(n) {
  return (t) => n({
    ...t,
    mark: mn(t.mark)
  });
}
function Pt({ renderer: n, domOutputSpecToElement: t, mapDefinedTypes: a, content: i, extensions: o, options: c }) {
  o = Nt(o);
  const u = Tt(o), { nodeExtensions: g, markExtensions: _ } = yt(o), { unhandledNode: h, unhandledMark: A, nodeMapping: y, markMapping: O } = c ?? {};
  return i = Bt(i, o, c), n({
    ...c,
    nodeMapping: {
      ...Object.fromEntries(g.filter((M) => M.name in a ? !1 : y ? !(M.name in y) : !0).map((M) => xt(t, M, u, c))),
      ...a,
      ...y,
      ...h ? { [De]: $t(h) } : {}
    },
    markMapping: {
      ...Object.fromEntries(_.filter((M) => O ? !(M.name in O) : !0).map((M) => It(t, M, u, c))),
      ...O,
      ...A ? { [Be]: Ut(A) } : {}
    }
  })({ content: i });
}
function Ht(n, { nodeMapping: t, markMapping: a, unhandledNode: i, unhandledMark: o }) {
  return function c({ content: u, parent: g }) {
    var _, h, A;
    const y = typeof u.type == "string" ? u.type : (_ = (h = u.type) === null || h === void 0 ? void 0 : h.name) !== null && _ !== void 0 ? _ : "", O = (A = t[y]) !== null && A !== void 0 ? A : i;
    if (!O) throw new Error(`missing handler for node type ${y}`);
    const M = n({
      component: O,
      props: {
        node: u,
        parent: g,
        renderElement: c,
        get children() {
          const P = [];
          return u.content && u.content.forEach((v) => {
            P.push(c({
              content: v,
              parent: u
            }));
          }), P;
        }
      }
    });
    return u.marks ? u.marks.reduce((P, v) => {
      var B;
      const $ = typeof v.type == "string" ? v.type : v.type.name, L = (B = a[$]) !== null && B !== void 0 ? B : o;
      if (!L) throw new Error(`missing handler for mark type ${$}`);
      return n({
        component: L,
        props: {
          mark: v,
          parent: g,
          node: u,
          children: P
        }
      });
    }, M) : M;
  };
}
function zt(n) {
  return Ht((t) => t.component(t.props), n);
}
function Xe(n) {
  return n.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
}
function Gt(n) {
  return Xe(n).replace(/"/g, "&quot;");
}
function Le(n) {
  const t = Object.entries(n || {}).filter(([, a]) => a != null).map(([a, i]) => `${a.split(" ").at(-1)}="${Gt(String(i))}"`).join(" ");
  return t ? ` ${t}` : "";
}
function We(n) {
  return [].concat(n || "").filter(Boolean).join("");
}
const Ft = /* @__PURE__ */ new Set([
  "iframe",
  "script",
  "style",
  "title",
  "textarea",
  "div",
  "span",
  "a",
  "button"
]);
function me(n) {
  if (typeof n == "string") return () => Xe(n);
  if (typeof n == "object" && "length" in n) {
    const [t, a, i, ...o] = n;
    let c = t;
    const u = c.split(" ");
    if (u.length > 1 && (c = `${u[1]} xmlns="${u[0]}"`), a === void 0) return () => `<${c}/>`;
    if (a === 0) return (g) => `<${c}>${We(g)}</${c}>`;
    if (typeof a == "object")
      return Array.isArray(a) ? i === void 0 ? (g) => `<${c}>${me(a)(g)}</${c}>` : i === 0 ? (g) => `<${c}>${me(a)(g)}</${c}>` : (g) => `<${c}>${me(a)(g)}${[i].concat(o).map((_) => me(_)(g))}</${c}>` : i === void 0 ? Ft.has(c) ? () => `<${c}${Le(a)}></${c}>` : () => `<${c}${Le(a)}/>` : i === 0 ? (g) => `<${c}${Le(a)}>${We(g)}</${c}>` : (g) => `<${c}${Le(a)}>${[i].concat(o).map((_) => me(_)(g)).join("")}</${c}>`;
  }
  throw new Error("[tiptap error]: Unsupported DomOutputSpec type, check the `renderHTML` method output or implement a node mapping", { cause: n });
}
function Kt({ content: n, extensions: t, staticEditorOptions: a, options: i }) {
  return Pt({
    renderer: zt,
    domOutputSpecToElement: me,
    mapDefinedTypes: {
      doc: ({ children: o }) => We(o),
      text: ({ node: o }) => {
        var c;
        return Xe((c = o.text) !== null && c !== void 0 ? c : "");
      }
    },
    content: n,
    extensions: kt(t, a),
    options: i
  });
}
var Nr = typeof globalThis < "u" ? globalThis : typeof window < "u" ? window : typeof global < "u" ? global : typeof self < "u" ? self : {};
function Zt(n) {
  return n && n.__esModule && Object.prototype.hasOwnProperty.call(n, "default") ? n.default : n;
}
var Ze, fn;
function Wt() {
  if (fn) return Ze;
  fn = 1;
  function n(e) {
    return e instanceof Map ? e.clear = e.delete = e.set = function() {
      throw new Error("map is read-only");
    } : e instanceof Set && (e.add = e.clear = e.delete = function() {
      throw new Error("set is read-only");
    }), Object.freeze(e), Object.getOwnPropertyNames(e).forEach((r) => {
      const l = e[r], N = typeof l;
      (N === "object" || N === "function") && !Object.isFrozen(l) && n(l);
    }), e;
  }
  class t {
    /**
     * @param {CompiledMode} mode
     */
    constructor(r) {
      r.data === void 0 && (r.data = {}), this.data = r.data, this.isMatchIgnored = !1;
    }
    ignoreMatch() {
      this.isMatchIgnored = !0;
    }
  }
  function a(e) {
    return e.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;").replace(/'/g, "&#x27;");
  }
  function i(e, ...r) {
    const l = /* @__PURE__ */ Object.create(null);
    for (const N in e)
      l[N] = e[N];
    return r.forEach(function(N) {
      for (const C in N)
        l[C] = N[C];
    }), /** @type {T} */
    l;
  }
  const o = "</span>", c = (e) => !!e.scope, u = (e, { prefix: r }) => {
    if (e.startsWith("language:"))
      return e.replace("language:", "language-");
    if (e.includes(".")) {
      const l = e.split(".");
      return [
        `${r}${l.shift()}`,
        ...l.map((N, C) => `${N}${"_".repeat(C + 1)}`)
      ].join(" ");
    }
    return `${r}${e}`;
  };
  class g {
    /**
     * Creates a new HTMLRenderer
     *
     * @param {Tree} parseTree - the parse tree (must support `walk` API)
     * @param {{classPrefix: string}} options
     */
    constructor(r, l) {
      this.buffer = "", this.classPrefix = l.classPrefix, r.walk(this);
    }
    /**
     * Adds texts to the output stream
     *
     * @param {string} text */
    addText(r) {
      this.buffer += a(r);
    }
    /**
     * Adds a node open to the output stream (if needed)
     *
     * @param {Node} node */
    openNode(r) {
      if (!c(r)) return;
      const l = u(
        r.scope,
        { prefix: this.classPrefix }
      );
      this.span(l);
    }
    /**
     * Adds a node close to the output stream (if needed)
     *
     * @param {Node} node */
    closeNode(r) {
      c(r) && (this.buffer += o);
    }
    /**
     * returns the accumulated buffer
    */
    value() {
      return this.buffer;
    }
    // helpers
    /**
     * Builds a span element
     *
     * @param {string} className */
    span(r) {
      this.buffer += `<span class="${r}">`;
    }
  }
  const _ = (e = {}) => {
    const r = { children: [] };
    return Object.assign(r, e), r;
  };
  class h {
    constructor() {
      this.rootNode = _(), this.stack = [this.rootNode];
    }
    get top() {
      return this.stack[this.stack.length - 1];
    }
    get root() {
      return this.rootNode;
    }
    /** @param {Node} node */
    add(r) {
      this.top.children.push(r);
    }
    /** @param {string} scope */
    openNode(r) {
      const l = _({ scope: r });
      this.add(l), this.stack.push(l);
    }
    closeNode() {
      if (this.stack.length > 1)
        return this.stack.pop();
    }
    closeAllNodes() {
      for (; this.closeNode(); ) ;
    }
    toJSON() {
      return JSON.stringify(this.rootNode, null, 4);
    }
    /**
     * @typedef { import("./html_renderer").Renderer } Renderer
     * @param {Renderer} builder
     */
    walk(r) {
      return this.constructor._walk(r, this.rootNode);
    }
    /**
     * @param {Renderer} builder
     * @param {Node} node
     */
    static _walk(r, l) {
      return typeof l == "string" ? r.addText(l) : l.children && (r.openNode(l), l.children.forEach((N) => this._walk(r, N)), r.closeNode(l)), r;
    }
    /**
     * @param {Node} node
     */
    static _collapse(r) {
      typeof r != "string" && r.children && (r.children.every((l) => typeof l == "string") ? r.children = [r.children.join("")] : r.children.forEach((l) => {
        h._collapse(l);
      }));
    }
  }
  class A extends h {
    /**
     * @param {*} options
     */
    constructor(r) {
      super(), this.options = r;
    }
    /**
     * @param {string} text
     */
    addText(r) {
      r !== "" && this.add(r);
    }
    /** @param {string} scope */
    startScope(r) {
      this.openNode(r);
    }
    endScope() {
      this.closeNode();
    }
    /**
     * @param {Emitter & {root: DataNode}} emitter
     * @param {string} name
     */
    __addSublanguage(r, l) {
      const N = r.root;
      l && (N.scope = `language:${l}`), this.add(N);
    }
    toHTML() {
      return new g(this, this.options).value();
    }
    finalize() {
      return this.closeAllNodes(), !0;
    }
  }
  function y(e) {
    return e ? typeof e == "string" ? e : e.source : null;
  }
  function O(e) {
    return v("(?=", e, ")");
  }
  function M(e) {
    return v("(?:", e, ")*");
  }
  function P(e) {
    return v("(?:", e, ")?");
  }
  function v(...e) {
    return e.map((l) => y(l)).join("");
  }
  function B(e) {
    const r = e[e.length - 1];
    return typeof r == "object" && r.constructor === Object ? (e.splice(e.length - 1, 1), r) : {};
  }
  function $(...e) {
    return "(" + (B(e).capture ? "" : "?:") + e.map((N) => y(N)).join("|") + ")";
  }
  function L(e) {
    return new RegExp(e.toString() + "|").exec("").length - 1;
  }
  function X(e, r) {
    const l = e && e.exec(r);
    return l && l.index === 0;
  }
  const J = new RegExp($(
    /\[(?:[^\\\]]|\\.)*\]/,
    // a character class, inside which ( and \ lose their meaning
    /\(\?<(?![=!])[^>]+>/,
    // a named capture group `(?<name>` (not a lookbehind `(?<=` / `(?<!`)
    /\(\?'[^']+'/,
    // a named capture group `(?'name'`
    /\(\??/,
    // an opening parenthesis, capturing or non-capturing / lookahead
    /\\([1-9][0-9]*)/,
    // a backreference like `\1`
    /\\./
    // any other escape sequence
  ));
  function G(e, { joinWith: r }) {
    let l = 0;
    return e.map((N) => {
      l += 1;
      const C = l;
      let D = y(N), p = "";
      for (; D.length > 0; ) {
        const f = J.exec(D);
        if (!f) {
          p += D;
          break;
        }
        p += D.substring(0, f.index), D = D.substring(f.index + f[0].length), f[0][0] === "\\" && f[1] ? p += "\\" + String(Number(f[1]) + C) : (p += f[0], (f[0] === "(" || /^\(\?[<']/.test(f[0])) && l++);
      }
      return p;
    }).map((N) => `(${N})`).join(r);
  }
  const Z = /\b\B/, se = "[a-zA-Z]\\w*", te = "[a-zA-Z_]\\w*", fe = "\\b\\d+(\\.\\d+)?", pe = "(-?)(\\b0[xX][a-fA-F0-9]+|(\\b\\d+(\\.\\d*)?|\\.\\d+)([eE][-+]?\\d+)?)", be = "\\b(0b[01]+)", _e = "!|!=|!==|%|%=|&|&&|&=|\\*|\\*=|\\+|\\+=|,|-|-=|/=|/|:|;|<<|<<=|<=|<|===|==|=|>>>=|>>=|>=|>>>|>>|>|\\?|\\[|\\{|\\(|\\^|\\^=|\\||\\|=|\\|\\||~", Ne = (e = {}) => {
    const r = /^#![ ]*\//;
    return e.binary && (e.begin = v(
      r,
      /.*\b/,
      e.binary,
      /\b.*/
    )), i({
      scope: "meta",
      begin: r,
      end: /$/,
      relevance: 0,
      /** @type {ModeCallback} */
      "on:begin": (l, N) => {
        l.index !== 0 && N.ignoreMatch();
      }
    }, e);
  }, re = {
    begin: "\\\\[\\s\\S]",
    relevance: 0
  }, Te = {
    scope: "string",
    begin: "'",
    end: "'",
    illegal: "\\n",
    contains: [re]
  }, le = {
    scope: "string",
    begin: '"',
    end: '"',
    illegal: "\\n",
    contains: [re]
  }, ye = {
    begin: /\b(a|an|the|are|I'm|isn't|don't|doesn't|won't|but|just|should|pretty|simply|enough|gonna|going|wtf|so|such|will|you|your|they|like|more)\b/
  }, R = function(e, r, l = {}) {
    const N = i(
      {
        scope: "comment",
        begin: e,
        end: r,
        contains: []
      },
      l
    );
    N.contains.push({
      scope: "doctag",
      // hack to avoid the space from being included. the space is necessary to
      // match here to prevent the plain text rule below from gobbling up doctags
      begin: "[ ]*(?=(TODO|FIXME|NOTE|BUG|OPTIMIZE|HACK|XXX):)",
      end: /(TODO|FIXME|NOTE|BUG|OPTIMIZE|HACK|XXX):/,
      excludeBegin: !0,
      relevance: 0
    });
    const C = $(
      // list of common 1 and 2 letter words in English
      "I",
      "a",
      "is",
      "so",
      "us",
      "to",
      "at",
      "if",
      "in",
      "it",
      "on",
      // note: this is not an exhaustive list of contractions, just popular ones
      /[A-Za-z]+['](d|ve|re|ll|t|s|n)/,
      // contractions - can't we'd they're let's, etc
      /[A-Za-z]+[-][a-z]+/,
      // `no-way`, etc.
      /[A-Za-z][a-z]{2,}/
      // allow capitalized words at beginning of sentences
    );
    return N.contains.push(
      {
        // TODO: how to include ", (, ) without breaking grammars that use these for
        // comment delimiters?
        // begin: /[ ]+([()"]?([A-Za-z'-]{3,}|is|a|I|so|us|[tT][oO]|at|if|in|it|on)[.]?[()":]?([.][ ]|[ ]|\))){3}/
        // ---
        // this tries to find sequences of 3 english words in a row (without any
        // "programming" type syntax) this gives us a strong signal that we've
        // TRULY found a comment - vs perhaps scanning with the wrong language.
        // It's possible to find something that LOOKS like the start of the
        // comment - but then if there is no readable text - good chance it is a
        // false match and not a comment.
        //
        // for a visual example please see:
        // https://github.com/highlightjs/highlight.js/issues/2827
        begin: v(
          /[ ]+/,
          // necessary to prevent us gobbling up doctags like /* @author Bob Mcgill */
          "(",
          C,
          /[.]?[:]?([.][ ]|[ ])/,
          "){3}"
        )
        // look for 3 words in a row
      }
    ), N;
  }, W = R("//", "$"), Y = R("/\\*", "\\*/"), Q = R("#", "$"), ie = {
    scope: "number",
    begin: fe,
    relevance: 0
  }, de = {
    scope: "number",
    begin: pe,
    relevance: 0
  }, xn = {
    scope: "number",
    begin: be,
    relevance: 0
  }, In = {
    scope: "regexp",
    begin: /\/(?=[^/\n]*\/)/,
    end: /\/[gimuy]*/,
    contains: [
      re,
      {
        begin: /\[/,
        end: /\]/,
        relevance: 0,
        contains: [re]
      }
    ]
  }, Ln = {
    scope: "title",
    begin: se,
    relevance: 0
  }, Cn = {
    scope: "title",
    begin: te,
    relevance: 0
  }, Dn = {
    // excludes method names from keyword processing
    begin: "\\.\\s*" + te,
    relevance: 0
  };
  var Me = /* @__PURE__ */ Object.freeze({
    __proto__: null,
    APOS_STRING_MODE: Te,
    BACKSLASH_ESCAPE: re,
    BINARY_NUMBER_MODE: xn,
    BINARY_NUMBER_RE: be,
    COMMENT: R,
    C_BLOCK_COMMENT_MODE: Y,
    C_LINE_COMMENT_MODE: W,
    C_NUMBER_MODE: de,
    C_NUMBER_RE: pe,
    END_SAME_AS_BEGIN: function(e) {
      return Object.assign(
        e,
        {
          /** @type {ModeCallback} */
          "on:begin": (r, l) => {
            l.data._beginMatch = r[1];
          },
          /** @type {ModeCallback} */
          "on:end": (r, l) => {
            l.data._beginMatch !== r[1] && l.ignoreMatch();
          }
        }
      );
    },
    HASH_COMMENT_MODE: Q,
    IDENT_RE: se,
    MATCH_NOTHING_RE: Z,
    METHOD_GUARD: Dn,
    NUMBER_MODE: ie,
    NUMBER_RE: fe,
    PHRASAL_WORDS_MODE: ye,
    QUOTE_STRING_MODE: le,
    REGEXP_MODE: In,
    RE_STARTERS_RE: _e,
    SHEBANG: Ne,
    TITLE_MODE: Ln,
    UNDERSCORE_IDENT_RE: te,
    UNDERSCORE_TITLE_MODE: Cn
  });
  function Bn(e, r) {
    e.input[e.index - 1] === "." && r.ignoreMatch();
  }
  function $n(e, r) {
    e.className !== void 0 && (e.scope = e.className, delete e.className);
  }
  function Un(e, r) {
    r && e.beginKeywords && (e.begin = "\\b(" + e.beginKeywords.split(" ").join("|") + ")(?!\\.)(?=\\b|\\s)", e.__beforeBegin = Bn, e.keywords = e.keywords || e.beginKeywords, delete e.beginKeywords, e.relevance === void 0 && (e.relevance = 0));
  }
  function Pn(e, r) {
    Array.isArray(e.illegal) && (e.illegal = $(...e.illegal));
  }
  function Hn(e, r) {
    if (e.match) {
      if (e.begin || e.end) throw new Error("begin & end are not supported with match");
      e.begin = e.match, delete e.match;
    }
  }
  function zn(e, r) {
    e.relevance === void 0 && (e.relevance = 1);
  }
  const Gn = (e, r) => {
    if (!e.beforeMatch) return;
    if (e.starts) throw new Error("beforeMatch cannot be used with starts");
    const l = Object.assign({}, e);
    Object.keys(e).forEach((N) => {
      delete e[N];
    }), e.keywords = l.keywords, e.begin = v(l.beforeMatch, O(l.begin)), e.starts = {
      relevance: 0,
      contains: [
        Object.assign(l, { endsParent: !0 })
      ]
    }, e.relevance = 0, delete l.beforeMatch;
  }, Fn = [
    "of",
    "and",
    "for",
    "in",
    "not",
    "or",
    "if",
    "then",
    "parent",
    // common variable name
    "list",
    // common variable name
    "value"
    // common variable name
  ], Kn = "keyword";
  function Ye(e, r, l = Kn) {
    const N = /* @__PURE__ */ Object.create(null);
    return typeof e == "string" ? C(l, e.split(" ")) : Array.isArray(e) ? C(l, e) : Object.keys(e).forEach(function(D) {
      Object.assign(
        N,
        Ye(e[D], r, D)
      );
    }), N;
    function C(D, p) {
      r && (p = p.map((f) => f.toLowerCase())), p.forEach(function(f) {
        const m = f.split("|");
        N[m[0]] = [D, Zn(m[0], m[1])];
      });
    }
  }
  function Zn(e, r) {
    return r ? Number(r) : Wn(e) ? 0 : 1;
  }
  function Wn(e) {
    return Fn.includes(e.toLowerCase());
  }
  const qe = {}, ue = (e) => {
    console.error(e);
  }, Je = (e, ...r) => {
    console.log(`WARN: ${e}`, ...r);
  }, Ee = (e, r) => {
    qe[`${e}/${r}`] || (console.log(`Deprecated as of ${e}. ${r}`), qe[`${e}/${r}`] = !0);
  }, we = new Error();
  function Qe(e, r, { key: l }) {
    let N = 0;
    const C = e[l], D = {}, p = {};
    for (let f = 1; f <= r.length; f++)
      p[f + N] = C[f], D[f + N] = !0, N += L(r[f - 1]);
    e[l] = p, e[l]._emit = D, e[l]._multi = !0;
  }
  function Xn(e) {
    if (Array.isArray(e.begin)) {
      if (e.skip || e.excludeBegin || e.returnBegin)
        throw ue("skip, excludeBegin, returnBegin not compatible with beginScope: {}"), we;
      if (typeof e.beginScope != "object" || e.beginScope === null)
        throw ue("beginScope must be object"), we;
      Qe(e, e.begin, { key: "beginScope" }), e.begin = G(e.begin, { joinWith: "" });
    }
  }
  function Yn(e) {
    if (Array.isArray(e.end)) {
      if (e.skip || e.excludeEnd || e.returnEnd)
        throw ue("skip, excludeEnd, returnEnd not compatible with endScope: {}"), we;
      if (typeof e.endScope != "object" || e.endScope === null)
        throw ue("endScope must be object"), we;
      Qe(e, e.end, { key: "endScope" }), e.end = G(e.end, { joinWith: "" });
    }
  }
  function qn(e) {
    e.scope && typeof e.scope == "object" && e.scope !== null && (e.beginScope = e.scope, delete e.scope);
  }
  function Jn(e) {
    qn(e), typeof e.beginScope == "string" && (e.beginScope = { _wrap: e.beginScope }), typeof e.endScope == "string" && (e.endScope = { _wrap: e.endScope }), Xn(e), Yn(e);
  }
  function Qn(e) {
    function r(p, f) {
      return new RegExp(
        y(p),
        "m" + (e.case_insensitive ? "i" : "") + (e.unicodeRegex ? "u" : "") + (f ? "g" : "")
      );
    }
    class l {
      constructor() {
        this.matchIndexes = {}, this.regexes = [], this.matchAt = 1, this.position = 0;
      }
      // @ts-ignore
      addRule(f, m) {
        m.position = this.position++, this.matchIndexes[this.matchAt] = m, this.regexes.push([m, f]), this.matchAt += L(f) + 1;
      }
      compile() {
        this.regexes.length === 0 && (this.exec = () => null);
        const f = this.regexes.map((m) => m[1]);
        this.matcherRe = r(G(f, { joinWith: "|" }), !0), this.lastIndex = 0;
      }
      /** @param {string} s */
      exec(f) {
        this.matcherRe.lastIndex = this.lastIndex;
        const m = this.matcherRe.exec(f);
        if (!m)
          return null;
        const z = m.findIndex((Se, Ue) => Ue > 0 && Se !== void 0), U = this.matchIndexes[z];
        return m.splice(0, z), Object.assign(m, U);
      }
    }
    class N {
      constructor() {
        this.rules = [], this.multiRegexes = [], this.count = 0, this.lastIndex = 0, this.regexIndex = 0;
      }
      // @ts-ignore
      getMatcher(f) {
        if (this.multiRegexes[f]) return this.multiRegexes[f];
        const m = new l();
        return this.rules.slice(f).forEach(([z, U]) => m.addRule(z, U)), m.compile(), this.multiRegexes[f] = m, m;
      }
      resumingScanAtSamePosition() {
        return this.regexIndex !== 0;
      }
      considerAll() {
        this.regexIndex = 0;
      }
      // @ts-ignore
      addRule(f, m) {
        this.rules.push([f, m]), m.type === "begin" && this.count++;
      }
      /** @param {string} s */
      exec(f) {
        const m = this.getMatcher(this.regexIndex);
        m.lastIndex = this.lastIndex;
        let z = m.exec(f);
        if (this.resumingScanAtSamePosition() && !(z && z.index === this.lastIndex)) {
          const U = this.getMatcher(0);
          U.lastIndex = this.lastIndex + 1, z = U.exec(f);
        }
        return z && (this.regexIndex += z.position + 1, this.regexIndex === this.count && this.considerAll()), z;
      }
    }
    function C(p) {
      const f = new N();
      return p.contains.forEach((m) => f.addRule(m.begin, { rule: m, type: "begin" })), p.terminatorEnd && f.addRule(p.terminatorEnd, { type: "end" }), p.illegal && f.addRule(p.illegal, { type: "illegal" }), f;
    }
    function D(p, f) {
      const m = (
        /** @type CompiledMode */
        p
      );
      if (p.isCompiled) return m;
      [
        $n,
        // do this early so compiler extensions generally don't have to worry about
        // the distinction between match/begin
        Hn,
        Jn,
        Gn
      ].forEach((U) => U(p, f)), e.compilerExtensions.forEach((U) => U(p, f)), p.__beforeBegin = null, [
        Un,
        // do this later so compiler extensions that come earlier have access to the
        // raw array if they wanted to perhaps manipulate it, etc.
        Pn,
        // default to 1 relevance if not specified
        zn
      ].forEach((U) => U(p, f)), p.isCompiled = !0;
      let z = null;
      return typeof p.keywords == "object" && p.keywords.$pattern && (p.keywords = Object.assign({}, p.keywords), z = p.keywords.$pattern, delete p.keywords.$pattern), z = z || /\w+/, p.keywords && (p.keywords = Ye(p.keywords, e.case_insensitive)), m.keywordPatternRe = r(z, !0), f && (p.begin || (p.begin = /\B|\b/), m.beginRe = r(m.begin), !p.end && !p.endsWithParent && (p.end = /\B|\b/), p.end && (m.endRe = r(m.end)), m.terminatorEnd = y(m.end) || "", p.endsWithParent && f.terminatorEnd && (m.terminatorEnd += (p.end ? "|" : "") + f.terminatorEnd)), p.illegal && (m.illegalRe = r(
        /** @type {RegExp | string} */
        p.illegal
      )), p.contains || (p.contains = []), p.contains = [].concat(...p.contains.map(function(U) {
        return Vn(U === "self" ? p : U);
      })), p.contains.forEach(function(U) {
        D(
          /** @type Mode */
          U,
          m
        );
      }), p.starts && D(p.starts, f), m.matcher = C(m), m;
    }
    if (e.compilerExtensions || (e.compilerExtensions = []), e.contains && e.contains.includes("self"))
      throw new Error("ERR: contains `self` is not supported at the top-level of a language.  See documentation.");
    return e.classNameAliases = i(e.classNameAliases || {}), D(
      /** @type Mode */
      e
    );
  }
  function Ve(e) {
    return e ? e.endsWithParent || Ve(e.starts) : !1;
  }
  function Vn(e) {
    return e.variants && !e.cachedVariants && (e.cachedVariants = e.variants.map(function(r) {
      return i(e, { variants: null }, r);
    })), e.cachedVariants ? e.cachedVariants : Ve(e) ? i(e, { starts: e.starts ? i(e.starts) : null }) : Object.isFrozen(e) ? i(e) : e;
  }
  var jn = "11.12.0";
  class et extends Error {
    constructor(r, l) {
      super(r), this.name = "HTMLInjectionError", this.html = l;
    }
  }
  const $e = a, je = i, en = /* @__PURE__ */ Symbol("nomatch"), nt = 7, nn = function(e) {
    const r = /* @__PURE__ */ Object.create(null), l = /* @__PURE__ */ Object.create(null), N = [];
    let C = !0;
    const D = "Could not find the language '{}', did you forget to load/include a language module?", p = { disableAutodetect: !0, name: "Plain text", contains: [] };
    let f = {
      ignoreUnescapedHTML: !1,
      throwUnescapedHTML: !1,
      noHighlightRe: /^(no-?highlight)$/i,
      languageDetectRe: /\blang(?:uage)?-([\w-]+)\b/i,
      classPrefix: "hljs-",
      cssSelector: "pre code",
      languages: null,
      // beta configuration options, subject to change, welcome to discuss
      // https://github.com/highlightjs/highlight.js/issues/1086
      __emitter: A
    };
    function m(s) {
      return f.noHighlightRe.test(s);
    }
    function z(s) {
      let E = s.className + " ";
      E += s.parentNode ? s.parentNode.className : "";
      const w = f.languageDetectRe.exec(E);
      if (w) {
        const x = oe(w[1]);
        return x || (Je(D.replace("{}", w[1])), Je("Falling back to no-highlight mode for this block.", s)), x ? w[1] : "no-highlight";
      }
      return E.split(/\s+/).find((x) => m(x) || oe(x));
    }
    function U(s, E, w) {
      let x = "", H = "";
      typeof E == "object" ? (x = s, w = E.ignoreIllegals, H = E.language) : (Ee("10.7.0", "highlight(lang, code, ...args) has been deprecated."), Ee("10.7.0", `Please use highlight(code, options) instead.
https://github.com/highlightjs/highlight.js/issues/2277`), H = s, x = E), w === void 0 && (w = !0);
      const V = {
        code: x,
        language: H
      };
      Re("before:highlight", V);
      const ce = V.result ? V.result : Se(V.language, V.code, w);
      return ce.code = V.code, Re("after:highlight", ce), ce;
    }
    function Se(s, E, w, x) {
      const H = /* @__PURE__ */ Object.create(null);
      function V(d, b) {
        return d.keywords[b];
      }
      function ce() {
        if (!T.keywords) {
          F.addText(I);
          return;
        }
        let d = 0;
        T.keywordPatternRe.lastIndex = 0;
        let b = T.keywordPatternRe.exec(I), S = "";
        for (; b; ) {
          S += I.substring(d, b.index);
          const k = ee.case_insensitive ? b[0].toLowerCase() : b[0], K = V(T, k);
          if (K) {
            const [ae, mt] = K;
            if (F.addText(S), S = "", H[k] = (H[k] || 0) + 1, H[k] <= nt && (Ie += mt), ae.startsWith("_"))
              S += b[0];
            else {
              const _t = ee.classNameAliases[ae] || ae;
              j(b[0], _t);
            }
          } else
            S += b[0];
          d = T.keywordPatternRe.lastIndex, b = T.keywordPatternRe.exec(I);
        }
        S += I.substring(d), F.addText(S);
      }
      function ke() {
        if (I === "") return;
        let d = null;
        if (typeof T.subLanguage == "string") {
          if (!r[T.subLanguage]) {
            F.addText(I);
            return;
          }
          d = Se(T.subLanguage, I, !0, dn[T.subLanguage]), dn[T.subLanguage] = /** @type {CompiledMode} */
          d._top;
        } else
          d = Pe(I, T.subLanguage.length ? T.subLanguage : null);
        T.relevance > 0 && (Ie += d.relevance), F.__addSublanguage(d._emitter, d.language);
      }
      function q() {
        T.subLanguage != null ? ke() : ce(), I = "";
      }
      function j(d, b) {
        d !== "" && (F.startScope(b), F.addText(d), F.endScope());
      }
      function sn(d, b) {
        let S = 1;
        const k = b.length - 1;
        for (; S <= k; ) {
          if (!d._emit[S]) {
            S++;
            continue;
          }
          const K = ee.classNameAliases[d[S]] || d[S], ae = b[S];
          K ? j(ae, K) : (I = ae, ce(), I = ""), S++;
        }
      }
      function on(d, b) {
        return d.scope && typeof d.scope == "string" && F.openNode(ee.classNameAliases[d.scope] || d.scope), d.beginScope && (d.beginScope._wrap ? (j(I, ee.classNameAliases[d.beginScope._wrap] || d.beginScope._wrap), I = "") : d.beginScope._multi && (sn(d.beginScope, b), I = "")), T = Object.create(d, { parent: { value: T } }), T;
      }
      function cn(d, b, S) {
        let k = X(d.endRe, S);
        if (k) {
          if (d["on:end"]) {
            const K = new t(d);
            d["on:end"](b, K), K.isMatchIgnored && (k = !1);
          }
          if (k) {
            for (; d.endsParent && d.parent; )
              d = d.parent;
            return d;
          }
        }
        if (d.endsWithParent)
          return cn(d.parent, b, S);
      }
      function ft(d) {
        return T.matcher.regexIndex === 0 ? (I += d[0], 1) : (Fe = !0, 0);
      }
      function pt(d) {
        const b = d[0], S = d.rule, k = new t(S), K = [S.__beforeBegin, S["on:begin"]];
        for (const ae of K)
          if (ae && (ae(d, k), k.isMatchIgnored))
            return ft(b);
        return S.skip ? I += b : (S.excludeBegin && (I += b), q(), !S.returnBegin && !S.excludeBegin && (I = b)), on(S, d), S.returnBegin ? 0 : b.length;
      }
      function bt(d) {
        const b = d[0], S = E.substring(d.index), k = cn(T, d, S);
        if (!k)
          return en;
        const K = T;
        T.endScope && T.endScope._wrap ? (q(), j(b, T.endScope._wrap)) : T.endScope && T.endScope._multi ? (q(), sn(T.endScope, d)) : K.skip ? I += b : (K.returnEnd || K.excludeEnd || (I += b), q(), K.excludeEnd && (I = b));
        do
          T.scope && F.closeNode(), !T.skip && !T.subLanguage && (Ie += T.relevance), T = T.parent;
        while (T !== k.parent);
        return k.starts && on(k.starts, d), K.returnEnd ? 0 : b.length;
      }
      function Et() {
        const d = [];
        for (let b = T; b !== ee; b = b.parent)
          b.scope && d.unshift(b.scope);
        d.forEach((b) => F.openNode(b));
      }
      let xe = {};
      function ln(d, b) {
        const S = b && b[0];
        if (I += d, S == null)
          return q(), 0;
        if (xe.type === "begin" && b.type === "end" && xe.index === b.index && S === "") {
          if (I += E.slice(b.index, b.index + 1), !C) {
            const k = new Error(`0 width match regex (${s})`);
            throw k.languageName = s, k.badRule = xe.rule, k;
          }
          return 1;
        }
        if (xe = b, b.type === "begin")
          return pt(b);
        if (b.type === "illegal" && !w) {
          const k = new Error('Illegal lexeme "' + S + '" for mode "' + (T.scope || "<unnamed>") + '"');
          throw k.mode = T, k;
        } else if (b.type === "end") {
          const k = bt(b);
          if (k !== en)
            return k;
        }
        if (b.type === "illegal" && S === "")
          return b.index === E.length || (I += `
`), 1;
        if (Ge > 1e5 && Ge > b.index * 3)
          throw new Error("potential infinite loop, way more iterations than matches");
        return I += S, S.length;
      }
      const ee = oe(s);
      if (!ee)
        throw ue(D.replace("{}", s)), new Error('Unknown language: "' + s + '"');
      const ht = Qn(ee);
      let ze = "", T = x || ht;
      const dn = {}, F = new f.__emitter(f);
      Et();
      let I = "", Ie = 0, ge = 0, Ge = 0, Fe = !1;
      try {
        if (ee.__emitTokens)
          ee.__emitTokens(E, F);
        else {
          for (T.matcher.considerAll(); ; ) {
            Ge++, Fe ? Fe = !1 : T.matcher.considerAll(), T.matcher.lastIndex = ge;
            const d = T.matcher.exec(E);
            if (!d) break;
            const b = E.substring(ge, d.index), S = ln(b, d);
            ge = d.index + S;
          }
          ln(E.substring(ge));
        }
        return F.finalize(), ze = F.toHTML(), {
          language: s,
          value: ze,
          relevance: Ie,
          illegal: !1,
          _emitter: F,
          _top: T
        };
      } catch (d) {
        if (d.message && d.message.includes("Illegal"))
          return {
            language: s,
            value: $e(E),
            illegal: !0,
            relevance: 0,
            _illegalBy: {
              message: d.message,
              index: ge,
              context: E.slice(ge - 100, ge + 100),
              mode: d.mode,
              resultSoFar: ze
            },
            _emitter: F
          };
        if (C)
          return {
            language: s,
            value: $e(E),
            illegal: !1,
            relevance: 0,
            errorRaised: d,
            _emitter: F,
            _top: T
          };
        throw d;
      }
    }
    function Ue(s) {
      const E = {
        value: $e(s),
        illegal: !1,
        relevance: 0,
        _top: p,
        _emitter: new f.__emitter(f)
      };
      return E._emitter.addText(s), E;
    }
    function Pe(s, E) {
      E = E || f.languages || Object.keys(r);
      const w = Ue(s), x = E.filter(oe).filter(an).map(
        (q) => Se(q, s, !1)
      );
      x.unshift(w);
      const H = x.sort((q, j) => {
        if (q.relevance !== j.relevance) return j.relevance - q.relevance;
        if (q.language && j.language) {
          if (oe(q.language).supersetOf === j.language)
            return 1;
          if (oe(j.language).supersetOf === q.language)
            return -1;
        }
        return 0;
      }), [V, ce] = H, ke = V;
      return ke.secondBest = ce, ke;
    }
    function tt(s, E, w) {
      const x = E && l[E] || w;
      s.classList.add("hljs"), s.classList.add(`language-${x}`);
    }
    function He(s) {
      let E = null;
      const w = z(s);
      if (m(w)) return;
      if (Re(
        "before:highlightElement",
        { el: s, language: w }
      ), s.dataset.highlighted) {
        console.log("Element previously highlighted. To highlight again, first unset `dataset.highlighted`.", s);
        return;
      }
      if (s.children.length > 0 && (f.ignoreUnescapedHTML || (console.warn("One of your code blocks includes unescaped HTML. This is a potentially serious security risk."), console.warn("https://github.com/highlightjs/highlight.js/wiki/security"), console.warn("The element with unescaped HTML:"), console.warn(s)), f.throwUnescapedHTML))
        throw new et(
          "One of your code blocks includes unescaped HTML.",
          s.innerHTML
        );
      E = s;
      const x = E.textContent, H = w ? U(x, { language: w, ignoreIllegals: !0 }) : Pe(x);
      s.innerHTML = H.value, s.dataset.highlighted = "yes", tt(s, w, H.language), s.result = {
        language: H.language,
        // TODO: remove with version 11.0
        re: H.relevance,
        relevance: H.relevance
      }, H.secondBest && (s.secondBest = {
        language: H.secondBest.language,
        relevance: H.secondBest.relevance
      }), Re("after:highlightElement", { el: s, result: H, text: x });
    }
    function rt(s) {
      f = je(f, s);
    }
    const it = () => {
      ve(), Ee("10.6.0", "initHighlighting() deprecated.  Use highlightAll() now.");
    };
    function at() {
      ve(), Ee("10.6.0", "initHighlightingOnLoad() deprecated.  Use highlightAll() now.");
    }
    let tn = !1;
    function ve() {
      function s() {
        ve();
      }
      if (document.readyState === "loading") {
        tn || window.addEventListener("DOMContentLoaded", s, !1), tn = !0;
        return;
      }
      document.querySelectorAll(f.cssSelector).forEach(He);
    }
    function st(s, E) {
      let w = null;
      try {
        w = E(e);
      } catch (x) {
        if (ue("Language definition for '{}' could not be registered.".replace("{}", s)), C)
          ue(x);
        else
          throw x;
        w = p;
      }
      w.name || (w.name = s), r[s] = w, w.rawDefinition = E.bind(null, e), w.aliases && rn(w.aliases, { languageName: s });
    }
    function ot(s) {
      delete r[s];
      for (const E of Object.keys(l))
        l[E] === s && delete l[E];
    }
    function ct() {
      return Object.keys(r);
    }
    function oe(s) {
      return s = (s || "").toLowerCase(), r[s] || r[l[s]];
    }
    function rn(s, { languageName: E }) {
      typeof s == "string" && (s = [s]), s.forEach((w) => {
        l[w.toLowerCase()] = E;
      });
    }
    function an(s) {
      const E = oe(s);
      return E && !E.disableAutodetect;
    }
    function lt(s) {
      s["before:highlightBlock"] && !s["before:highlightElement"] && (s["before:highlightElement"] = (E) => {
        s["before:highlightBlock"](
          Object.assign({ block: E.el }, E)
        );
      }), s["after:highlightBlock"] && !s["after:highlightElement"] && (s["after:highlightElement"] = (E) => {
        s["after:highlightBlock"](
          Object.assign({ block: E.el }, E)
        );
      });
    }
    function dt(s) {
      lt(s), N.push(s);
    }
    function ut(s) {
      const E = N.indexOf(s);
      E !== -1 && N.splice(E, 1);
    }
    function Re(s, E) {
      const w = s;
      N.forEach(function(x) {
        x[w] && x[w](E);
      });
    }
    function gt(s) {
      return Ee("10.7.0", "highlightBlock will be removed entirely in v12.0"), Ee("10.7.0", "Please use highlightElement now."), He(s);
    }
    Object.assign(e, {
      highlight: U,
      highlightAuto: Pe,
      highlightAll: ve,
      highlightElement: He,
      // TODO: Remove with v12 API
      highlightBlock: gt,
      configure: rt,
      initHighlighting: it,
      initHighlightingOnLoad: at,
      registerLanguage: st,
      unregisterLanguage: ot,
      listLanguages: ct,
      getLanguage: oe,
      registerAliases: rn,
      autoDetection: an,
      inherit: je,
      addPlugin: dt,
      removePlugin: ut
    }), e.debugMode = function() {
      C = !1;
    }, e.safeMode = function() {
      C = !0;
    }, e.versionString = jn, e.regex = {
      concat: v,
      lookahead: O,
      either: $,
      optional: P,
      anyNumberOfTimes: M
    };
    for (const s in Me)
      typeof Me[s] == "object" && n(Me[s]);
    return Object.assign(e, Me), e;
  }, he = nn({});
  return he.newInstance = () => nn({}), Ze = he, he.HighlightJS = he, he.default = he, Ze;
}
var Xt = /* @__PURE__ */ Wt();
const ne = /* @__PURE__ */ Zt(Xt), pn = "[A-Za-z$_][0-9A-Za-z$_]*", Yt = [
  "as",
  // for exports
  "in",
  "of",
  "if",
  "for",
  "while",
  "finally",
  "var",
  "new",
  "function",
  "do",
  "return",
  "void",
  "else",
  "break",
  "catch",
  "instanceof",
  "with",
  "throw",
  "case",
  "default",
  "try",
  "switch",
  "continue",
  "typeof",
  "delete",
  "let",
  "yield",
  "const",
  "class",
  // JS handles these with a special rule
  // "get",
  // "set",
  "debugger",
  "async",
  "await",
  "static",
  "import",
  "from",
  "export",
  "extends",
  // It's reached stage 3, which is "recommended for implementation":
  "using"
], qt = [
  "true",
  "false",
  "null",
  "undefined",
  "NaN",
  "Infinity"
], _n = [
  // Fundamental objects
  "Object",
  "Function",
  "Boolean",
  "Symbol",
  // numbers and dates
  "Math",
  "Date",
  "Number",
  "BigInt",
  // text
  "String",
  "RegExp",
  // Indexed collections
  "Array",
  "Float32Array",
  "Float64Array",
  "Int8Array",
  "Uint8Array",
  "Uint8ClampedArray",
  "Int16Array",
  "Int32Array",
  "Uint16Array",
  "Uint32Array",
  "BigInt64Array",
  "BigUint64Array",
  // Keyed collections
  "Set",
  "Map",
  "WeakSet",
  "WeakMap",
  // Structured data
  "ArrayBuffer",
  "SharedArrayBuffer",
  "Atomics",
  "DataView",
  "JSON",
  // Control abstraction objects
  "Promise",
  "Generator",
  "GeneratorFunction",
  "AsyncFunction",
  // Reflection
  "Reflect",
  "Proxy",
  // Internationalization
  "Intl",
  // WebAssembly
  "WebAssembly"
], Nn = [
  "Error",
  "EvalError",
  "InternalError",
  "RangeError",
  "ReferenceError",
  "SyntaxError",
  "TypeError",
  "URIError"
], Tn = [
  "setInterval",
  "setTimeout",
  "clearInterval",
  "clearTimeout",
  "require",
  "exports",
  "eval",
  "isFinite",
  "isNaN",
  "parseFloat",
  "parseInt",
  "decodeURI",
  "decodeURIComponent",
  "encodeURI",
  "encodeURIComponent",
  "escape",
  "unescape"
], Jt = [
  "arguments",
  "this",
  "super",
  "console",
  "window",
  "document",
  "localStorage",
  "sessionStorage",
  "module",
  "self",
  "global"
  // Node.js
], Qt = [].concat(
  Tn,
  _n,
  Nn
);
function Vt(n) {
  const t = n.regex, a = (R, { after: W }) => {
    const Y = "</" + R[0].slice(1);
    return R.input.indexOf(Y, W) !== -1;
  }, i = pn, o = {
    begin: "<>",
    end: "</>"
  }, c = /<[A-Za-z0-9\\._:-]+\s*\/>/, u = {
    begin: /<[A-Za-z0-9\\._:-]+/,
    end: /\/[A-Za-z0-9\\._:-]+>|\/>/,
    /**
     * @param {RegExpMatchArray} match
     * @param {CallbackResponse} response
     */
    isTrulyOpeningTag: (R, W) => {
      const Y = R[0].length + R.index, Q = R.input[Y];
      if (
        // HTML should not include another raw `<` inside a tag
        // nested type?
        // `<Array<Array<number>>`, etc.
        Q === "<" || // the , gives away that this is not HTML
        // `<T, A extends keyof T, V>`
        Q === ","
      ) {
        W.ignoreMatch();
        return;
      }
      Q === ">" && (a(R, { after: Y }) || W.ignoreMatch());
      let ie;
      const de = R.input.substring(Y);
      if (ie = de.match(/^\s*=/)) {
        W.ignoreMatch();
        return;
      }
      if ((ie = de.match(/^\s+extends\s+/)) && ie.index === 0) {
        W.ignoreMatch();
        return;
      }
    }
  }, g = {
    $pattern: pn,
    keyword: Yt,
    literal: qt,
    built_in: Qt,
    "variable.language": Jt
  }, _ = "[0-9](_?[0-9])*", h = `\\.(${_})`, A = "0|[1-9](_?[0-9])*|0[0-7]*[89][0-9]*", y = {
    className: "number",
    variants: [
      // DecimalLiteral
      { begin: `(\\b(${A})((${h})|\\.)?|(${h}))[eE][+-]?(${_})\\b` },
      { begin: `\\b(${A})\\b((${h})\\b|\\.)?|(${h})\\b` },
      // DecimalBigIntegerLiteral
      { begin: "\\b(0|[1-9](_?[0-9])*)n\\b" },
      // NonDecimalIntegerLiteral
      { begin: "\\b0[xX][0-9a-fA-F](_?[0-9a-fA-F])*n?\\b" },
      { begin: "\\b0[bB][0-1](_?[0-1])*n?\\b" },
      { begin: "\\b0[oO][0-7](_?[0-7])*n?\\b" },
      // LegacyOctalIntegerLiteral (does not include underscore separators)
      // https://tc39.es/ecma262/#sec-additional-syntax-numeric-literals
      { begin: "\\b0[0-7]+n?\\b" }
    ],
    relevance: 0
  }, O = {
    className: "subst",
    begin: "\\$\\{",
    end: "\\}",
    keywords: g,
    contains: []
    // defined later
  }, M = {
    begin: ".?html`",
    end: "",
    starts: {
      end: "`",
      returnEnd: !1,
      contains: [
        n.BACKSLASH_ESCAPE,
        O
      ],
      subLanguage: "xml"
    }
  }, P = {
    begin: ".?css`",
    end: "",
    starts: {
      end: "`",
      returnEnd: !1,
      contains: [
        n.BACKSLASH_ESCAPE,
        O
      ],
      subLanguage: "css"
    }
  }, v = {
    begin: ".?gql`",
    end: "",
    starts: {
      end: "`",
      returnEnd: !1,
      contains: [
        n.BACKSLASH_ESCAPE,
        O
      ],
      subLanguage: "graphql"
    }
  }, B = {
    className: "string",
    begin: "`",
    end: "`",
    contains: [
      n.BACKSLASH_ESCAPE,
      O
    ]
  }, L = {
    className: "comment",
    variants: [
      n.COMMENT(
        /\/\*\*(?!\/)/,
        "\\*/",
        {
          relevance: 0,
          contains: [
            {
              begin: "(?=@[A-Za-z]+)",
              relevance: 0,
              contains: [
                {
                  className: "doctag",
                  begin: "@[A-Za-z]+"
                },
                {
                  className: "type",
                  begin: "\\{",
                  end: "\\}",
                  excludeEnd: !0,
                  excludeBegin: !0,
                  relevance: 0
                },
                {
                  className: "variable",
                  begin: i + "(?=\\s*(-)|$)",
                  endsParent: !0,
                  relevance: 0
                },
                // eat spaces (not newlines) so we can find
                // types or variables
                {
                  begin: /(?=[^\n])\s/,
                  relevance: 0
                }
              ]
            }
          ]
        }
      ),
      n.C_BLOCK_COMMENT_MODE,
      n.C_LINE_COMMENT_MODE
    ]
  }, X = [
    n.APOS_STRING_MODE,
    n.QUOTE_STRING_MODE,
    M,
    P,
    v,
    B,
    // Skip numbers when they are part of a variable name
    { match: /\$\d+/ },
    y
    // This is intentional:
    // See https://github.com/highlightjs/highlight.js/issues/3288
    // hljs.REGEXP_MODE
  ];
  O.contains = X.concat({
    // we need to pair up {} inside our subst to prevent
    // it from ending too early by matching another }
    begin: /\{/,
    end: /\}/,
    keywords: g,
    contains: [
      "self"
    ].concat(X)
  });
  const J = [].concat(L, O.contains), G = J.concat([
    // eat recursive parens in sub expressions
    {
      begin: /(\s*)\(/,
      end: /\)/,
      keywords: g,
      contains: ["self"].concat(J)
    }
  ]), Z = {
    className: "params",
    // convert this to negative lookbehind in v12
    begin: /(\s*)\(/,
    // to match the parms with
    end: /\)/,
    excludeBegin: !0,
    excludeEnd: !0,
    keywords: g,
    contains: G
  }, se = {
    variants: [
      // class Car extends vehicle
      {
        match: [
          /class/,
          /\s+/,
          i,
          /\s+/,
          /extends/,
          /\s+/,
          t.concat(i, "(", t.concat(/\./, i), ")*")
        ],
        scope: {
          1: "keyword",
          3: "title.class",
          5: "keyword",
          7: "title.class.inherited"
        }
      },
      // class Car
      {
        match: [
          /class/,
          /\s+/,
          i
        ],
        scope: {
          1: "keyword",
          3: "title.class"
        }
      }
    ]
  }, te = {
    relevance: 0,
    match: t.either(
      // Hard coded exceptions
      /\bJSON/,
      // Float32Array, OutT
      /\b[A-Z][a-z]+([A-Z][a-z]*|\d)*/,
      // CSSFactory, CSSFactoryT
      /\b[A-Z]{2,}([A-Z][a-z]+|\d)+([A-Z][a-z]*)*/,
      // FPs, FPsT
      /\b[A-Z]{2,}[a-z]+([A-Z][a-z]+|\d)*([A-Z][a-z]*)*/
      // P
      // single letters are not highlighted
      // BLAH
      // this will be flagged as a UPPER_CASE_CONSTANT instead
    ),
    className: "title.class",
    keywords: {
      _: [
        // se we still get relevance credit for JS library classes
        ..._n,
        ...Nn
      ]
    }
  }, fe = {
    label: "use_strict",
    className: "meta",
    relevance: 10,
    begin: /^\s*['"]use (strict|asm)['"]/
  }, pe = {
    variants: [
      {
        match: [
          /function/,
          /\s+/,
          i,
          /(?=\s*\()/
        ]
      },
      // anonymous function
      {
        match: [
          /function/,
          /\s*(?=\()/
        ]
      }
    ],
    className: {
      1: "keyword",
      3: "title.function"
    },
    label: "func.def",
    contains: [Z],
    illegal: /%/
  }, be = {
    relevance: 0,
    match: /\b[A-Z][A-Z_0-9]+\b/,
    className: "variable.constant"
  };
  function _e(R) {
    return t.concat("(?!", R.join("|"), ")");
  }
  const Ne = {
    match: t.concat(
      /\b/,
      _e([
        ...Tn,
        "super",
        "import",
        "await"
      ].map((R) => `${R}\\s*\\(`)),
      i,
      t.lookahead(/\s*\(/)
    ),
    className: "title.function",
    relevance: 0
  }, re = {
    begin: t.concat(/\./, t.lookahead(
      t.concat(i, /(?![0-9A-Za-z$_(])/)
    )),
    end: i,
    excludeBegin: !0,
    keywords: "prototype",
    className: "property",
    relevance: 0
  }, Te = {
    match: [
      /get|set/,
      /\s+/,
      i,
      /(?=\()/
    ],
    className: {
      1: "keyword",
      3: "title.function"
    },
    contains: [
      {
        // eat to avoid empty params
        begin: /\(\)/
      },
      Z
    ]
  }, le = "(\\([^()]*(\\([^()]*(\\([^()]*\\)[^()]*)*\\)[^()]*)*\\)|" + n.UNDERSCORE_IDENT_RE + ")\\s*=>", ye = {
    match: [
      /const|var|let/,
      /\s+/,
      i,
      /\s*/,
      /=\s*/,
      /(async\s*)?/,
      // async is optional
      t.lookahead(le)
    ],
    keywords: "async",
    className: {
      1: "keyword",
      3: "title.function"
    },
    contains: [
      Z
    ]
  };
  return {
    name: "JavaScript",
    aliases: ["js", "jsx", "mjs", "cjs"],
    keywords: g,
    // this will be extended by TypeScript
    exports: { PARAMS_CONTAINS: G, CLASS_REFERENCE: te },
    illegal: /#(?![$_A-Za-z])/,
    contains: [
      n.SHEBANG({
        label: "shebang",
        binary: "node",
        relevance: 5
      }),
      fe,
      n.APOS_STRING_MODE,
      n.QUOTE_STRING_MODE,
      M,
      P,
      v,
      B,
      L,
      // Skip numbers when they are part of a variable name
      { match: /\$\d+/ },
      y,
      te,
      {
        scope: "attr",
        match: i + t.lookahead(":"),
        relevance: 0
      },
      ye,
      {
        // "value" container
        begin: "(" + n.RE_STARTERS_RE + "|\\b(case|return|throw)\\b)\\s*",
        keywords: "return throw case",
        relevance: 0,
        contains: [
          L,
          n.REGEXP_MODE,
          {
            className: "function",
            // we have to count the parens to make sure we actually have the
            // correct bounding ( ) before the =>.  There could be any number of
            // sub-expressions inside also surrounded by parens.
            begin: le,
            returnBegin: !0,
            end: "\\s*=>",
            contains: [
              {
                className: "params",
                variants: [
                  {
                    begin: n.UNDERSCORE_IDENT_RE,
                    relevance: 0
                  },
                  {
                    className: null,
                    begin: /\(\s*\)/,
                    skip: !0
                  },
                  {
                    begin: /(\s*)\(/,
                    end: /\)/,
                    excludeBegin: !0,
                    excludeEnd: !0,
                    keywords: g,
                    contains: G
                  }
                ]
              }
            ]
          },
          {
            // could be a comma delimited list of params to a function call
            begin: /,/,
            relevance: 0
          },
          {
            match: /\s+/,
            relevance: 0
          },
          {
            // JSX
            variants: [
              { begin: o.begin, end: o.end },
              { match: c },
              {
                begin: u.begin,
                // we carefully check the opening tag to see if it truly
                // is a tag and not a false positive
                "on:begin": u.isTrulyOpeningTag,
                end: u.end
              }
            ],
            subLanguage: "xml",
            contains: [
              {
                begin: u.begin,
                end: u.end,
                skip: !0,
                contains: ["self"]
              }
            ]
          }
        ]
      },
      pe,
      {
        // prevent this from getting swallowed up by function
        // since they appear "function like"
        beginKeywords: "while if switch catch for"
      },
      {
        // we have to count the parens to make sure we actually have the correct
        // bounding ( ).  There could be any number of sub-expressions inside
        // also surrounded by parens.
        begin: "\\b(?!function)" + n.UNDERSCORE_IDENT_RE + "\\([^()]*(\\([^()]*(\\([^()]*\\)[^()]*)*\\)[^()]*)*\\)\\s*\\{",
        // end parens
        returnBegin: !0,
        label: "func.def",
        contains: [
          Z,
          n.inherit(n.TITLE_MODE, { begin: i, className: "title.function" })
        ]
      },
      // catch ... so it won't trigger the property rule below
      {
        match: /\.\.\./,
        relevance: 0
      },
      re,
      // hack: prevents detection of keywords in some circumstances
      // .keyword()
      // $keyword = x
      {
        match: "\\$" + i,
        relevance: 0
      },
      {
        match: [/\bconstructor(?=\s*\()/],
        className: { 1: "title.function" },
        contains: [Z]
      },
      Ne,
      be,
      se,
      Te,
      {
        match: /\$[(.]/
        // relevance booster for a pattern common to JS libs: `$(something)` and `$.something`
      }
    ]
  };
}
const Ce = "[A-Za-z$_][0-9A-Za-z$_]*", yn = [
  "as",
  // for exports
  "in",
  "of",
  "if",
  "for",
  "while",
  "finally",
  "var",
  "new",
  "function",
  "do",
  "return",
  "void",
  "else",
  "break",
  "catch",
  "instanceof",
  "with",
  "throw",
  "case",
  "default",
  "try",
  "switch",
  "continue",
  "typeof",
  "delete",
  "let",
  "yield",
  "const",
  "class",
  // JS handles these with a special rule
  // "get",
  // "set",
  "debugger",
  "async",
  "await",
  "static",
  "import",
  "from",
  "export",
  "extends",
  // It's reached stage 3, which is "recommended for implementation":
  "using"
], Sn = [
  "true",
  "false",
  "null",
  "undefined",
  "NaN",
  "Infinity"
], An = [
  // Fundamental objects
  "Object",
  "Function",
  "Boolean",
  "Symbol",
  // numbers and dates
  "Math",
  "Date",
  "Number",
  "BigInt",
  // text
  "String",
  "RegExp",
  // Indexed collections
  "Array",
  "Float32Array",
  "Float64Array",
  "Int8Array",
  "Uint8Array",
  "Uint8ClampedArray",
  "Int16Array",
  "Int32Array",
  "Uint16Array",
  "Uint32Array",
  "BigInt64Array",
  "BigUint64Array",
  // Keyed collections
  "Set",
  "Map",
  "WeakSet",
  "WeakMap",
  // Structured data
  "ArrayBuffer",
  "SharedArrayBuffer",
  "Atomics",
  "DataView",
  "JSON",
  // Control abstraction objects
  "Promise",
  "Generator",
  "GeneratorFunction",
  "AsyncFunction",
  // Reflection
  "Reflect",
  "Proxy",
  // Internationalization
  "Intl",
  // WebAssembly
  "WebAssembly"
], On = [
  "Error",
  "EvalError",
  "InternalError",
  "RangeError",
  "ReferenceError",
  "SyntaxError",
  "TypeError",
  "URIError"
], Mn = [
  "setInterval",
  "setTimeout",
  "clearInterval",
  "clearTimeout",
  "require",
  "exports",
  "eval",
  "isFinite",
  "isNaN",
  "parseFloat",
  "parseInt",
  "decodeURI",
  "decodeURIComponent",
  "encodeURI",
  "encodeURIComponent",
  "escape",
  "unescape"
], wn = [
  "arguments",
  "this",
  "super",
  "console",
  "window",
  "document",
  "localStorage",
  "sessionStorage",
  "module",
  "self",
  "global"
  // Node.js
], vn = [].concat(
  Mn,
  An,
  On
);
function jt(n) {
  const t = n.regex, a = (R, { after: W }) => {
    const Y = "</" + R[0].slice(1);
    return R.input.indexOf(Y, W) !== -1;
  }, i = Ce, o = {
    begin: "<>",
    end: "</>"
  }, c = /<[A-Za-z0-9\\._:-]+\s*\/>/, u = {
    begin: /<[A-Za-z0-9\\._:-]+/,
    end: /\/[A-Za-z0-9\\._:-]+>|\/>/,
    /**
     * @param {RegExpMatchArray} match
     * @param {CallbackResponse} response
     */
    isTrulyOpeningTag: (R, W) => {
      const Y = R[0].length + R.index, Q = R.input[Y];
      if (
        // HTML should not include another raw `<` inside a tag
        // nested type?
        // `<Array<Array<number>>`, etc.
        Q === "<" || // the , gives away that this is not HTML
        // `<T, A extends keyof T, V>`
        Q === ","
      ) {
        W.ignoreMatch();
        return;
      }
      Q === ">" && (a(R, { after: Y }) || W.ignoreMatch());
      let ie;
      const de = R.input.substring(Y);
      if (ie = de.match(/^\s*=/)) {
        W.ignoreMatch();
        return;
      }
      if ((ie = de.match(/^\s+extends\s+/)) && ie.index === 0) {
        W.ignoreMatch();
        return;
      }
    }
  }, g = {
    $pattern: Ce,
    keyword: yn,
    literal: Sn,
    built_in: vn,
    "variable.language": wn
  }, _ = "[0-9](_?[0-9])*", h = `\\.(${_})`, A = "0|[1-9](_?[0-9])*|0[0-7]*[89][0-9]*", y = {
    className: "number",
    variants: [
      // DecimalLiteral
      { begin: `(\\b(${A})((${h})|\\.)?|(${h}))[eE][+-]?(${_})\\b` },
      { begin: `\\b(${A})\\b((${h})\\b|\\.)?|(${h})\\b` },
      // DecimalBigIntegerLiteral
      { begin: "\\b(0|[1-9](_?[0-9])*)n\\b" },
      // NonDecimalIntegerLiteral
      { begin: "\\b0[xX][0-9a-fA-F](_?[0-9a-fA-F])*n?\\b" },
      { begin: "\\b0[bB][0-1](_?[0-1])*n?\\b" },
      { begin: "\\b0[oO][0-7](_?[0-7])*n?\\b" },
      // LegacyOctalIntegerLiteral (does not include underscore separators)
      // https://tc39.es/ecma262/#sec-additional-syntax-numeric-literals
      { begin: "\\b0[0-7]+n?\\b" }
    ],
    relevance: 0
  }, O = {
    className: "subst",
    begin: "\\$\\{",
    end: "\\}",
    keywords: g,
    contains: []
    // defined later
  }, M = {
    begin: ".?html`",
    end: "",
    starts: {
      end: "`",
      returnEnd: !1,
      contains: [
        n.BACKSLASH_ESCAPE,
        O
      ],
      subLanguage: "xml"
    }
  }, P = {
    begin: ".?css`",
    end: "",
    starts: {
      end: "`",
      returnEnd: !1,
      contains: [
        n.BACKSLASH_ESCAPE,
        O
      ],
      subLanguage: "css"
    }
  }, v = {
    begin: ".?gql`",
    end: "",
    starts: {
      end: "`",
      returnEnd: !1,
      contains: [
        n.BACKSLASH_ESCAPE,
        O
      ],
      subLanguage: "graphql"
    }
  }, B = {
    className: "string",
    begin: "`",
    end: "`",
    contains: [
      n.BACKSLASH_ESCAPE,
      O
    ]
  }, L = {
    className: "comment",
    variants: [
      n.COMMENT(
        /\/\*\*(?!\/)/,
        "\\*/",
        {
          relevance: 0,
          contains: [
            {
              begin: "(?=@[A-Za-z]+)",
              relevance: 0,
              contains: [
                {
                  className: "doctag",
                  begin: "@[A-Za-z]+"
                },
                {
                  className: "type",
                  begin: "\\{",
                  end: "\\}",
                  excludeEnd: !0,
                  excludeBegin: !0,
                  relevance: 0
                },
                {
                  className: "variable",
                  begin: i + "(?=\\s*(-)|$)",
                  endsParent: !0,
                  relevance: 0
                },
                // eat spaces (not newlines) so we can find
                // types or variables
                {
                  begin: /(?=[^\n])\s/,
                  relevance: 0
                }
              ]
            }
          ]
        }
      ),
      n.C_BLOCK_COMMENT_MODE,
      n.C_LINE_COMMENT_MODE
    ]
  }, X = [
    n.APOS_STRING_MODE,
    n.QUOTE_STRING_MODE,
    M,
    P,
    v,
    B,
    // Skip numbers when they are part of a variable name
    { match: /\$\d+/ },
    y
    // This is intentional:
    // See https://github.com/highlightjs/highlight.js/issues/3288
    // hljs.REGEXP_MODE
  ];
  O.contains = X.concat({
    // we need to pair up {} inside our subst to prevent
    // it from ending too early by matching another }
    begin: /\{/,
    end: /\}/,
    keywords: g,
    contains: [
      "self"
    ].concat(X)
  });
  const J = [].concat(L, O.contains), G = J.concat([
    // eat recursive parens in sub expressions
    {
      begin: /(\s*)\(/,
      end: /\)/,
      keywords: g,
      contains: ["self"].concat(J)
    }
  ]), Z = {
    className: "params",
    // convert this to negative lookbehind in v12
    begin: /(\s*)\(/,
    // to match the parms with
    end: /\)/,
    excludeBegin: !0,
    excludeEnd: !0,
    keywords: g,
    contains: G
  }, se = {
    variants: [
      // class Car extends vehicle
      {
        match: [
          /class/,
          /\s+/,
          i,
          /\s+/,
          /extends/,
          /\s+/,
          t.concat(i, "(", t.concat(/\./, i), ")*")
        ],
        scope: {
          1: "keyword",
          3: "title.class",
          5: "keyword",
          7: "title.class.inherited"
        }
      },
      // class Car
      {
        match: [
          /class/,
          /\s+/,
          i
        ],
        scope: {
          1: "keyword",
          3: "title.class"
        }
      }
    ]
  }, te = {
    relevance: 0,
    match: t.either(
      // Hard coded exceptions
      /\bJSON/,
      // Float32Array, OutT
      /\b[A-Z][a-z]+([A-Z][a-z]*|\d)*/,
      // CSSFactory, CSSFactoryT
      /\b[A-Z]{2,}([A-Z][a-z]+|\d)+([A-Z][a-z]*)*/,
      // FPs, FPsT
      /\b[A-Z]{2,}[a-z]+([A-Z][a-z]+|\d)*([A-Z][a-z]*)*/
      // P
      // single letters are not highlighted
      // BLAH
      // this will be flagged as a UPPER_CASE_CONSTANT instead
    ),
    className: "title.class",
    keywords: {
      _: [
        // se we still get relevance credit for JS library classes
        ...An,
        ...On
      ]
    }
  }, fe = {
    label: "use_strict",
    className: "meta",
    relevance: 10,
    begin: /^\s*['"]use (strict|asm)['"]/
  }, pe = {
    variants: [
      {
        match: [
          /function/,
          /\s+/,
          i,
          /(?=\s*\()/
        ]
      },
      // anonymous function
      {
        match: [
          /function/,
          /\s*(?=\()/
        ]
      }
    ],
    className: {
      1: "keyword",
      3: "title.function"
    },
    label: "func.def",
    contains: [Z],
    illegal: /%/
  }, be = {
    relevance: 0,
    match: /\b[A-Z][A-Z_0-9]+\b/,
    className: "variable.constant"
  };
  function _e(R) {
    return t.concat("(?!", R.join("|"), ")");
  }
  const Ne = {
    match: t.concat(
      /\b/,
      _e([
        ...Mn,
        "super",
        "import",
        "await"
      ].map((R) => `${R}\\s*\\(`)),
      i,
      t.lookahead(/\s*\(/)
    ),
    className: "title.function",
    relevance: 0
  }, re = {
    begin: t.concat(/\./, t.lookahead(
      t.concat(i, /(?![0-9A-Za-z$_(])/)
    )),
    end: i,
    excludeBegin: !0,
    keywords: "prototype",
    className: "property",
    relevance: 0
  }, Te = {
    match: [
      /get|set/,
      /\s+/,
      i,
      /(?=\()/
    ],
    className: {
      1: "keyword",
      3: "title.function"
    },
    contains: [
      {
        // eat to avoid empty params
        begin: /\(\)/
      },
      Z
    ]
  }, le = "(\\([^()]*(\\([^()]*(\\([^()]*\\)[^()]*)*\\)[^()]*)*\\)|" + n.UNDERSCORE_IDENT_RE + ")\\s*=>", ye = {
    match: [
      /const|var|let/,
      /\s+/,
      i,
      /\s*/,
      /=\s*/,
      /(async\s*)?/,
      // async is optional
      t.lookahead(le)
    ],
    keywords: "async",
    className: {
      1: "keyword",
      3: "title.function"
    },
    contains: [
      Z
    ]
  };
  return {
    name: "JavaScript",
    aliases: ["js", "jsx", "mjs", "cjs"],
    keywords: g,
    // this will be extended by TypeScript
    exports: { PARAMS_CONTAINS: G, CLASS_REFERENCE: te },
    illegal: /#(?![$_A-Za-z])/,
    contains: [
      n.SHEBANG({
        label: "shebang",
        binary: "node",
        relevance: 5
      }),
      fe,
      n.APOS_STRING_MODE,
      n.QUOTE_STRING_MODE,
      M,
      P,
      v,
      B,
      L,
      // Skip numbers when they are part of a variable name
      { match: /\$\d+/ },
      y,
      te,
      {
        scope: "attr",
        match: i + t.lookahead(":"),
        relevance: 0
      },
      ye,
      {
        // "value" container
        begin: "(" + n.RE_STARTERS_RE + "|\\b(case|return|throw)\\b)\\s*",
        keywords: "return throw case",
        relevance: 0,
        contains: [
          L,
          n.REGEXP_MODE,
          {
            className: "function",
            // we have to count the parens to make sure we actually have the
            // correct bounding ( ) before the =>.  There could be any number of
            // sub-expressions inside also surrounded by parens.
            begin: le,
            returnBegin: !0,
            end: "\\s*=>",
            contains: [
              {
                className: "params",
                variants: [
                  {
                    begin: n.UNDERSCORE_IDENT_RE,
                    relevance: 0
                  },
                  {
                    className: null,
                    begin: /\(\s*\)/,
                    skip: !0
                  },
                  {
                    begin: /(\s*)\(/,
                    end: /\)/,
                    excludeBegin: !0,
                    excludeEnd: !0,
                    keywords: g,
                    contains: G
                  }
                ]
              }
            ]
          },
          {
            // could be a comma delimited list of params to a function call
            begin: /,/,
            relevance: 0
          },
          {
            match: /\s+/,
            relevance: 0
          },
          {
            // JSX
            variants: [
              { begin: o.begin, end: o.end },
              { match: c },
              {
                begin: u.begin,
                // we carefully check the opening tag to see if it truly
                // is a tag and not a false positive
                "on:begin": u.isTrulyOpeningTag,
                end: u.end
              }
            ],
            subLanguage: "xml",
            contains: [
              {
                begin: u.begin,
                end: u.end,
                skip: !0,
                contains: ["self"]
              }
            ]
          }
        ]
      },
      pe,
      {
        // prevent this from getting swallowed up by function
        // since they appear "function like"
        beginKeywords: "while if switch catch for"
      },
      {
        // we have to count the parens to make sure we actually have the correct
        // bounding ( ).  There could be any number of sub-expressions inside
        // also surrounded by parens.
        begin: "\\b(?!function)" + n.UNDERSCORE_IDENT_RE + "\\([^()]*(\\([^()]*(\\([^()]*\\)[^()]*)*\\)[^()]*)*\\)\\s*\\{",
        // end parens
        returnBegin: !0,
        label: "func.def",
        contains: [
          Z,
          n.inherit(n.TITLE_MODE, { begin: i, className: "title.function" })
        ]
      },
      // catch ... so it won't trigger the property rule below
      {
        match: /\.\.\./,
        relevance: 0
      },
      re,
      // hack: prevents detection of keywords in some circumstances
      // .keyword()
      // $keyword = x
      {
        match: "\\$" + i,
        relevance: 0
      },
      {
        match: [/\bconstructor(?=\s*\()/],
        className: { 1: "title.function" },
        contains: [Z]
      },
      Ne,
      be,
      se,
      Te,
      {
        match: /\$[(.]/
        // relevance booster for a pattern common to JS libs: `$(something)` and `$.something`
      }
    ]
  };
}
function er(n) {
  const t = n.regex, a = jt(n), i = Ce, o = [
    "any",
    "void",
    "number",
    "boolean",
    "string",
    "object",
    "never",
    "symbol",
    "bigint",
    "unknown"
  ], c = {
    begin: [
      /namespace/,
      /\s+/,
      n.IDENT_RE
    ],
    beginScope: {
      1: "keyword",
      3: "title.class"
    }
  }, u = {
    beginKeywords: "interface",
    end: /\{/,
    excludeEnd: !0,
    keywords: {
      keyword: "interface extends",
      built_in: o
    },
    contains: [a.exports.CLASS_REFERENCE]
  }, g = {
    className: "meta",
    relevance: 10,
    begin: /^\s*['"]use strict['"]/
  }, _ = [
    "type",
    // "namespace",
    "interface",
    "public",
    "private",
    "protected",
    "implements",
    "declare",
    "abstract",
    "readonly",
    "enum",
    "override",
    "satisfies"
  ], h = {
    $pattern: Ce,
    keyword: yn.concat(_),
    literal: Sn,
    built_in: vn.concat(o),
    "variable.language": wn
  }, A = {
    className: "meta",
    begin: "@" + i
  }, y = (v, B, $) => {
    const L = v.contains.findIndex((X) => X.label === B);
    if (L === -1)
      throw new Error("can not find mode to replace");
    v.contains.splice(L, 1, $);
  };
  Object.assign(a.keywords, h), a.exports.PARAMS_CONTAINS.push(A);
  const O = a.contains.find((v) => v.scope === "attr"), M = Object.assign(
    {},
    O,
    { match: t.concat(i, t.lookahead(/\s*\?:/)) }
  );
  a.exports.PARAMS_CONTAINS.push([
    a.exports.CLASS_REFERENCE,
    // class reference for highlighting the params types
    O,
    // highlight the params key
    M
    // Added for optional property assignment highlighting
  ]), a.contains = a.contains.concat([
    A,
    c,
    u,
    M
    // Added for optional property assignment highlighting
  ]), y(a, "shebang", n.SHEBANG()), y(a, "use_strict", g);
  const P = a.contains.find((v) => v.label === "func.def");
  return P.relevance = 0, Object.assign(a, {
    name: "TypeScript",
    aliases: [
      "ts",
      "tsx",
      "mts",
      "cts"
    ]
  }), a;
}
function Rn(n) {
  const t = n.regex, a = t.concat(/[\p{L}_]/u, t.optional(/[\p{L}0-9_.-]*:/u), /[\p{L}0-9_.-]*/u), i = /[\p{L}0-9._:-]+/u, o = {
    className: "symbol",
    begin: /&[a-z]+;|&#[0-9]+;|&#x[a-f0-9]+;/
  }, c = {
    begin: /\s/,
    contains: [
      {
        className: "keyword",
        begin: /#?[a-z_][a-z1-9_-]+/,
        illegal: /\n/
      }
    ]
  }, u = n.inherit(c, {
    begin: /\(/,
    end: /\)/
  }), g = n.inherit(n.APOS_STRING_MODE, { className: "string" }), _ = n.inherit(n.QUOTE_STRING_MODE, { className: "string" }), h = {
    endsWithParent: !0,
    illegal: /</,
    relevance: 0,
    contains: [
      {
        className: "attr",
        begin: i,
        relevance: 0
      },
      {
        begin: /=\s*/,
        relevance: 0,
        contains: [
          {
            className: "string",
            endsParent: !0,
            variants: [
              {
                begin: /"/,
                end: /"/,
                contains: [o]
              },
              {
                begin: /'/,
                end: /'/,
                contains: [o]
              },
              { begin: /[^\s"'=<>`]+/ }
            ]
          }
        ]
      }
    ]
  };
  return {
    name: "HTML, XML",
    aliases: [
      "html",
      "xhtml",
      "rss",
      "atom",
      "xjb",
      "xsd",
      "xsl",
      "plist",
      "wsf",
      "svg"
    ],
    case_insensitive: !0,
    unicodeRegex: !0,
    contains: [
      {
        className: "meta",
        begin: /<![a-z]/,
        end: />/,
        relevance: 10,
        contains: [
          c,
          _,
          g,
          u,
          {
            begin: /\[/,
            end: /\]/,
            contains: [
              {
                className: "meta",
                begin: /<![a-z]/,
                end: />/,
                contains: [
                  c,
                  u,
                  _,
                  g
                ]
              }
            ]
          }
        ]
      },
      n.COMMENT(
        /<!--/,
        /-->/,
        { relevance: 10 }
      ),
      {
        begin: /<!\[CDATA\[/,
        end: /\]\]>/,
        relevance: 10
      },
      o,
      // xml processing instructions
      {
        className: "meta",
        end: /\?>/,
        variants: [
          {
            begin: /<\?xml/,
            relevance: 10,
            contains: [
              _
            ]
          },
          {
            begin: /<\?[a-z][a-z0-9]+/
          }
        ]
      },
      {
        className: "tag",
        /*
        The lookahead pattern (?=...) ensures that 'begin' only matches
        '<style' as a single word, followed by a whitespace or an
        ending bracket.
        */
        begin: /<style(?=\s|>)/,
        end: />/,
        keywords: { name: "style" },
        contains: [h],
        starts: {
          end: /<\/style>/,
          returnEnd: !0,
          subLanguage: "css"
        }
      },
      {
        className: "tag",
        // See the comment in the <style tag about the lookahead pattern
        begin: /<script(?=\s|>)/,
        end: />/,
        keywords: { name: "script" },
        contains: [h],
        starts: {
          end: /<\/script>/,
          returnEnd: !0,
          subLanguage: "javascript"
        }
      },
      // we need this for now for jSX
      {
        className: "tag",
        begin: /<>|<\/>/
      },
      // open tag
      {
        className: "tag",
        begin: t.concat(
          /</,
          t.lookahead(t.concat(
            a,
            // <tag/>
            // <tag>
            // <tag ...
            t.either(/\/>/, />/, /\s/)
          ))
        ),
        end: /\/?>/,
        contains: [
          {
            className: "name",
            begin: a,
            relevance: 0,
            starts: h
          }
        ]
      },
      // close tag
      {
        className: "tag",
        begin: t.concat(
          /<\//,
          t.lookahead(t.concat(
            a,
            />/
          ))
        ),
        contains: [
          {
            className: "name",
            begin: a,
            relevance: 0
          },
          {
            begin: />/,
            relevance: 0,
            endsParent: !0
          }
        ]
      }
    ]
  };
}
const nr = "([-+]?)(\\b0[xX][a-fA-F0-9]+|(\\b\\d+(\\.\\d*)?|\\.\\d+)([eE][-+]?\\d+)?)|NaN|[-+]?Infinity", tr = {
  scope: "number",
  match: nr,
  relevance: 0
};
function rr(n) {
  const t = {
    className: "attr",
    begin: /(("(\\.|[^\\"\r\n])*")|('(\\.|[^\\'\r\n])*'))(?=\s*:)/,
    relevance: 1.01
  }, a = {
    match: /[{}[\],:]/,
    className: "punctuation",
    relevance: 0
  }, i = [
    "true",
    "false",
    "null"
  ], o = {
    scope: "literal",
    beginKeywords: i.join(" ")
  };
  return {
    name: "JSON",
    aliases: ["jsonc", "json5"],
    keywords: {
      literal: i
    },
    contains: [
      t,
      a,
      n.APOS_STRING_MODE,
      n.QUOTE_STRING_MODE,
      o,
      tr,
      n.C_LINE_COMMENT_MODE,
      n.C_BLOCK_COMMENT_MODE
    ],
    illegal: "\\S"
  };
}
const ir = (n) => ({
  IMPORTANT: {
    scope: "meta",
    begin: "!important"
  },
  BLOCK_COMMENT: n.C_BLOCK_COMMENT_MODE,
  HEXCOLOR: {
    scope: "number",
    begin: /#(([0-9a-fA-F]{3,4})|(([0-9a-fA-F]{2}){3,4}))\b/
  },
  UNICODE_RANGE: {
    scope: "number",
    begin: /\b[Uu]\+[0-9A-Fa-f][0-9A-Fa-f?]{0,5}(-[0-9A-Fa-f][0-9A-Fa-f]{0,5})?/
  },
  FUNCTION_DISPATCH: {
    className: "built_in",
    begin: /[\w-]+(?=\()/
  },
  ATTRIBUTE_SELECTOR_MODE: {
    scope: "selector-attr",
    begin: /\[/,
    end: /\]/,
    illegal: "$",
    contains: [
      n.APOS_STRING_MODE,
      n.QUOTE_STRING_MODE
    ]
  },
  CSS_NUMBER_MODE: {
    scope: "number",
    begin: n.NUMBER_RE + "(%|em|ex|ch|rem|vw|vh|vmin|vmax|cm|mm|in|pt|pc|px|deg|grad|rad|turn|s|ms|Hz|kHz|dpi|dpcm|dppx)?",
    relevance: 0
  },
  CSS_VARIABLE: {
    className: "attr",
    begin: /--[A-Za-z_][A-Za-z0-9_-]*/
  }
}), ar = [
  "a",
  "abbr",
  "address",
  "article",
  "aside",
  "audio",
  "b",
  "blockquote",
  "body",
  "button",
  "canvas",
  "caption",
  "cite",
  "code",
  "dd",
  "del",
  "details",
  "dfn",
  "div",
  "dl",
  "dt",
  "em",
  "fieldset",
  "figcaption",
  "figure",
  "footer",
  "form",
  "h1",
  "h2",
  "h3",
  "h4",
  "h5",
  "h6",
  "header",
  "hgroup",
  "html",
  "i",
  "iframe",
  "img",
  "input",
  "ins",
  "kbd",
  "label",
  "legend",
  "li",
  "main",
  "mark",
  "menu",
  "nav",
  "object",
  "ol",
  "optgroup",
  "option",
  "p",
  "picture",
  "q",
  "quote",
  "samp",
  "section",
  "select",
  "source",
  "span",
  "strong",
  "summary",
  "sup",
  "table",
  "tbody",
  "td",
  "textarea",
  "tfoot",
  "th",
  "thead",
  "time",
  "tr",
  "ul",
  "var",
  "video"
], sr = [
  "defs",
  "g",
  "marker",
  "mask",
  "pattern",
  "svg",
  "switch",
  "symbol",
  "feBlend",
  "feColorMatrix",
  "feComponentTransfer",
  "feComposite",
  "feConvolveMatrix",
  "feDiffuseLighting",
  "feDisplacementMap",
  "feFlood",
  "feGaussianBlur",
  "feImage",
  "feMerge",
  "feMorphology",
  "feOffset",
  "feSpecularLighting",
  "feTile",
  "feTurbulence",
  "linearGradient",
  "radialGradient",
  "stop",
  "circle",
  "ellipse",
  "image",
  "line",
  "path",
  "polygon",
  "polyline",
  "rect",
  "text",
  "use",
  "textPath",
  "tspan",
  "foreignObject",
  "clipPath"
], or = [
  ...ar,
  ...sr
], cr = [
  "any-hover",
  "any-pointer",
  "aspect-ratio",
  "color",
  "color-gamut",
  "color-index",
  "device-aspect-ratio",
  "device-height",
  "device-width",
  "display-mode",
  "forced-colors",
  "grid",
  "height",
  "hover",
  "inverted-colors",
  "monochrome",
  "orientation",
  "overflow-block",
  "overflow-inline",
  "pointer",
  "prefers-color-scheme",
  "prefers-contrast",
  "prefers-reduced-motion",
  "prefers-reduced-transparency",
  "resolution",
  "scan",
  "scripting",
  "update",
  "width",
  // TODO: find a better solution?
  "min-width",
  "max-width",
  "min-height",
  "max-height"
].sort().reverse(), lr = [
  "active",
  "any-link",
  "blank",
  "checked",
  "current",
  "default",
  "defined",
  "dir",
  // dir()
  "disabled",
  "drop",
  "empty",
  "enabled",
  "first",
  "first-child",
  "first-of-type",
  "fullscreen",
  "future",
  "focus",
  "focus-visible",
  "focus-within",
  "has",
  // has()
  "host",
  // host or host()
  "host-context",
  // host-context()
  "hover",
  "indeterminate",
  "in-range",
  "invalid",
  "is",
  // is()
  "lang",
  // lang()
  "last-child",
  "last-of-type",
  "left",
  "link",
  "local-link",
  "not",
  // not()
  "nth-child",
  // nth-child()
  "nth-col",
  // nth-col()
  "nth-last-child",
  // nth-last-child()
  "nth-last-col",
  // nth-last-col()
  "nth-last-of-type",
  //nth-last-of-type()
  "nth-of-type",
  //nth-of-type()
  "only-child",
  "only-of-type",
  "optional",
  "out-of-range",
  "past",
  "placeholder-shown",
  "read-only",
  "read-write",
  "required",
  "right",
  "root",
  "scope",
  "target",
  "target-within",
  "user-invalid",
  "valid",
  "visited",
  "where"
  // where()
].sort().reverse(), dr = [
  "after",
  "backdrop",
  "before",
  "cue",
  "cue-region",
  "first-letter",
  "first-line",
  "grammar-error",
  "marker",
  "part",
  "placeholder",
  "selection",
  "slotted",
  "spelling-error"
].sort().reverse(), ur = [
  "accent-color",
  "align-content",
  "align-items",
  "align-self",
  "alignment-baseline",
  "all",
  "anchor-name",
  "animation",
  "animation-composition",
  "animation-delay",
  "animation-direction",
  "animation-duration",
  "animation-fill-mode",
  "animation-iteration-count",
  "animation-name",
  "animation-play-state",
  "animation-range",
  "animation-range-end",
  "animation-range-start",
  "animation-timeline",
  "animation-timing-function",
  "appearance",
  "aspect-ratio",
  "backdrop-filter",
  "backface-visibility",
  "background",
  "background-attachment",
  "background-blend-mode",
  "background-clip",
  "background-color",
  "background-image",
  "background-origin",
  "background-position",
  "background-position-x",
  "background-position-y",
  "background-repeat",
  "background-size",
  "baseline-shift",
  "block-size",
  "border",
  "border-block",
  "border-block-color",
  "border-block-end",
  "border-block-end-color",
  "border-block-end-style",
  "border-block-end-width",
  "border-block-start",
  "border-block-start-color",
  "border-block-start-style",
  "border-block-start-width",
  "border-block-style",
  "border-block-width",
  "border-bottom",
  "border-bottom-color",
  "border-bottom-left-radius",
  "border-bottom-right-radius",
  "border-bottom-style",
  "border-bottom-width",
  "border-collapse",
  "border-color",
  "border-end-end-radius",
  "border-end-start-radius",
  "border-image",
  "border-image-outset",
  "border-image-repeat",
  "border-image-slice",
  "border-image-source",
  "border-image-width",
  "border-inline",
  "border-inline-color",
  "border-inline-end",
  "border-inline-end-color",
  "border-inline-end-style",
  "border-inline-end-width",
  "border-inline-start",
  "border-inline-start-color",
  "border-inline-start-style",
  "border-inline-start-width",
  "border-inline-style",
  "border-inline-width",
  "border-left",
  "border-left-color",
  "border-left-style",
  "border-left-width",
  "border-radius",
  "border-right",
  "border-right-color",
  "border-right-style",
  "border-right-width",
  "border-spacing",
  "border-start-end-radius",
  "border-start-start-radius",
  "border-style",
  "border-top",
  "border-top-color",
  "border-top-left-radius",
  "border-top-right-radius",
  "border-top-style",
  "border-top-width",
  "border-width",
  "bottom",
  "box-align",
  "box-decoration-break",
  "box-direction",
  "box-flex",
  "box-flex-group",
  "box-lines",
  "box-ordinal-group",
  "box-orient",
  "box-pack",
  "box-shadow",
  "box-sizing",
  "break-after",
  "break-before",
  "break-inside",
  "caption-side",
  "caret-color",
  "clear",
  "clip",
  "clip-path",
  "clip-rule",
  "color",
  "color-interpolation",
  "color-interpolation-filters",
  "color-profile",
  "color-rendering",
  "color-scheme",
  "column-count",
  "column-fill",
  "column-gap",
  "column-rule",
  "column-rule-color",
  "column-rule-style",
  "column-rule-width",
  "column-span",
  "column-width",
  "columns",
  "contain",
  "contain-intrinsic-block-size",
  "contain-intrinsic-height",
  "contain-intrinsic-inline-size",
  "contain-intrinsic-size",
  "contain-intrinsic-width",
  "container",
  "container-name",
  "container-type",
  "content",
  "content-visibility",
  "corner-bottom-left-shape",
  "corner-bottom-right-shape",
  "corner-shape",
  "corner-top-left-shape",
  "corner-top-right-shape",
  "counter-increment",
  "counter-reset",
  "counter-set",
  "cue",
  "cue-after",
  "cue-before",
  "cursor",
  "cx",
  "cy",
  "direction",
  "display",
  "dominant-baseline",
  "empty-cells",
  "enable-background",
  "field-sizing",
  "fill",
  "fill-opacity",
  "fill-rule",
  "filter",
  "flex",
  "flex-basis",
  "flex-direction",
  "flex-flow",
  "flex-grow",
  "flex-shrink",
  "flex-wrap",
  "float",
  "flood-color",
  "flood-opacity",
  "flow",
  "font",
  "font-display",
  "font-family",
  "font-feature-settings",
  "font-kerning",
  "font-language-override",
  "font-optical-sizing",
  "font-palette",
  "font-size",
  "font-size-adjust",
  "font-smooth",
  "font-smoothing",
  "font-stretch",
  "font-style",
  "font-synthesis",
  "font-synthesis-position",
  "font-synthesis-small-caps",
  "font-synthesis-style",
  "font-synthesis-weight",
  "font-variant",
  "font-variant-alternates",
  "font-variant-caps",
  "font-variant-east-asian",
  "font-variant-emoji",
  "font-variant-ligatures",
  "font-variant-numeric",
  "font-variant-position",
  "font-variation-settings",
  "font-weight",
  "forced-color-adjust",
  "gap",
  "glyph-orientation-horizontal",
  "glyph-orientation-vertical",
  "grid",
  "grid-area",
  "grid-auto-columns",
  "grid-auto-flow",
  "grid-auto-rows",
  "grid-column",
  "grid-column-end",
  "grid-column-start",
  "grid-gap",
  "grid-row",
  "grid-row-end",
  "grid-row-start",
  "grid-template",
  "grid-template-areas",
  "grid-template-columns",
  "grid-template-rows",
  "hanging-punctuation",
  "height",
  "hyphenate-character",
  "hyphenate-limit-chars",
  "hyphens",
  "icon",
  "image-orientation",
  "image-rendering",
  "image-resolution",
  "ime-mode",
  "initial-letter",
  "initial-letter-align",
  "inline-size",
  "inset",
  "inset-area",
  "inset-block",
  "inset-block-end",
  "inset-block-start",
  "inset-inline",
  "inset-inline-end",
  "inset-inline-start",
  "isolation",
  "justify-content",
  "justify-items",
  "justify-self",
  "kerning",
  "left",
  "letter-spacing",
  "lighting-color",
  "line-break",
  "line-height",
  "line-height-step",
  "list-style",
  "list-style-image",
  "list-style-position",
  "list-style-type",
  "margin",
  "margin-block",
  "margin-block-end",
  "margin-block-start",
  "margin-bottom",
  "margin-inline",
  "margin-inline-end",
  "margin-inline-start",
  "margin-left",
  "margin-right",
  "margin-top",
  "margin-trim",
  "marker",
  "marker-end",
  "marker-mid",
  "marker-start",
  "marks",
  "mask",
  "mask-border",
  "mask-border-mode",
  "mask-border-outset",
  "mask-border-repeat",
  "mask-border-slice",
  "mask-border-source",
  "mask-border-width",
  "mask-clip",
  "mask-composite",
  "mask-image",
  "mask-mode",
  "mask-origin",
  "mask-position",
  "mask-repeat",
  "mask-size",
  "mask-type",
  "masonry-auto-flow",
  "math-depth",
  "math-shift",
  "math-style",
  "max-block-size",
  "max-height",
  "max-inline-size",
  "max-width",
  "min-block-size",
  "min-height",
  "min-inline-size",
  "min-width",
  "mix-blend-mode",
  "nav-down",
  "nav-index",
  "nav-left",
  "nav-right",
  "nav-up",
  "none",
  "normal",
  "object-fit",
  "object-position",
  "offset",
  "offset-anchor",
  "offset-distance",
  "offset-path",
  "offset-position",
  "offset-rotate",
  "opacity",
  "order",
  "orphans",
  "outline",
  "outline-color",
  "outline-offset",
  "outline-style",
  "outline-width",
  "overflow",
  "overflow-anchor",
  "overflow-block",
  "overflow-clip-margin",
  "overflow-inline",
  "overflow-wrap",
  "overflow-x",
  "overflow-y",
  "overlay",
  "overscroll-behavior",
  "overscroll-behavior-block",
  "overscroll-behavior-inline",
  "overscroll-behavior-x",
  "overscroll-behavior-y",
  "padding",
  "padding-block",
  "padding-block-end",
  "padding-block-start",
  "padding-bottom",
  "padding-inline",
  "padding-inline-end",
  "padding-inline-start",
  "padding-left",
  "padding-right",
  "padding-top",
  "page",
  "page-break-after",
  "page-break-before",
  "page-break-inside",
  "paint-order",
  "pause",
  "pause-after",
  "pause-before",
  "perspective",
  "perspective-origin",
  "place-content",
  "place-items",
  "place-self",
  "pointer-events",
  "position",
  "position-anchor",
  "position-visibility",
  "print-color-adjust",
  "quotes",
  "r",
  "resize",
  "rest",
  "rest-after",
  "rest-before",
  "right",
  "rotate",
  "row-gap",
  "ruby-align",
  "ruby-position",
  "scale",
  "scroll-behavior",
  "scroll-margin",
  "scroll-margin-block",
  "scroll-margin-block-end",
  "scroll-margin-block-start",
  "scroll-margin-bottom",
  "scroll-margin-inline",
  "scroll-margin-inline-end",
  "scroll-margin-inline-start",
  "scroll-margin-left",
  "scroll-margin-right",
  "scroll-margin-top",
  "scroll-padding",
  "scroll-padding-block",
  "scroll-padding-block-end",
  "scroll-padding-block-start",
  "scroll-padding-bottom",
  "scroll-padding-inline",
  "scroll-padding-inline-end",
  "scroll-padding-inline-start",
  "scroll-padding-left",
  "scroll-padding-right",
  "scroll-padding-top",
  "scroll-snap-align",
  "scroll-snap-stop",
  "scroll-snap-type",
  "scroll-timeline",
  "scroll-timeline-axis",
  "scroll-timeline-name",
  "scrollbar-color",
  "scrollbar-gutter",
  "scrollbar-width",
  "shape-image-threshold",
  "shape-margin",
  "shape-outside",
  "shape-rendering",
  "speak",
  "speak-as",
  "src",
  // @font-face
  "stop-color",
  "stop-opacity",
  "stroke",
  "stroke-dasharray",
  "stroke-dashoffset",
  "stroke-linecap",
  "stroke-linejoin",
  "stroke-miterlimit",
  "stroke-opacity",
  "stroke-width",
  "tab-size",
  "table-layout",
  "text-align",
  "text-align-all",
  "text-align-last",
  "text-anchor",
  "text-combine-upright",
  "text-decoration",
  "text-decoration-color",
  "text-decoration-line",
  "text-decoration-skip",
  "text-decoration-skip-ink",
  "text-decoration-style",
  "text-decoration-thickness",
  "text-emphasis",
  "text-emphasis-color",
  "text-emphasis-position",
  "text-emphasis-style",
  "text-indent",
  "text-justify",
  "text-orientation",
  "text-overflow",
  "text-rendering",
  "text-shadow",
  "text-size-adjust",
  "text-transform",
  "text-underline-offset",
  "text-underline-position",
  "text-wrap",
  "text-wrap-mode",
  "text-wrap-style",
  "timeline-scope",
  "top",
  "touch-action",
  "transform",
  "transform-box",
  "transform-origin",
  "transform-style",
  "transition",
  "transition-behavior",
  "transition-delay",
  "transition-duration",
  "transition-property",
  "transition-timing-function",
  "translate",
  "unicode-bidi",
  "unicode-range",
  "user-modify",
  "user-select",
  "vector-effect",
  "vertical-align",
  "view-timeline",
  "view-timeline-axis",
  "view-timeline-inset",
  "view-timeline-name",
  "view-transition-name",
  "visibility",
  "voice-balance",
  "voice-duration",
  "voice-family",
  "voice-pitch",
  "voice-range",
  "voice-rate",
  "voice-stress",
  "voice-volume",
  "white-space",
  "white-space-collapse",
  "widows",
  "width",
  "will-change",
  "word-break",
  "word-spacing",
  "word-wrap",
  "writing-mode",
  "x",
  "y",
  "z-index",
  "zoom"
].sort().reverse();
function gr(n) {
  const t = n.regex, a = ir(n), i = { begin: /-(webkit|moz|ms|o)-(?=[a-z])/ }, o = "and or not only", c = /@-?\w[\w]*(-\w+)*/, u = "[a-zA-Z-][a-zA-Z0-9_-]*", g = [
    n.APOS_STRING_MODE,
    n.QUOTE_STRING_MODE
  ];
  return {
    name: "CSS",
    case_insensitive: !0,
    illegal: /[=|'\$]/,
    keywords: { keyframePosition: "from to" },
    classNameAliases: {
      // for visual continuity with `tag {}` and because we
      // don't have a great class for this?
      keyframePosition: "selector-tag"
    },
    contains: [
      a.BLOCK_COMMENT,
      i,
      // to recognize keyframe 40% etc which are outside the scope of our
      // attribute value mode
      a.CSS_NUMBER_MODE,
      {
        className: "selector-id",
        begin: /#[A-Za-z0-9_-]+/,
        relevance: 0
      },
      {
        className: "selector-class",
        begin: "\\." + u,
        relevance: 0
      },
      a.ATTRIBUTE_SELECTOR_MODE,
      {
        className: "selector-pseudo",
        variants: [
          { begin: ":(" + lr.join("|") + ")" },
          { begin: ":(:)?(" + dr.join("|") + ")" }
        ]
      },
      // we may actually need this (12/2020)
      // { // pseudo-selector params
      //   begin: /\(/,
      //   end: /\)/,
      //   contains: [ hljs.CSS_NUMBER_MODE ]
      // },
      a.CSS_VARIABLE,
      {
        className: "attribute",
        begin: "\\b(" + ur.join("|") + ")\\b"
      },
      // attribute values
      {
        begin: /:/,
        end: /[;}{]/,
        contains: [
          a.BLOCK_COMMENT,
          a.HEXCOLOR,
          a.IMPORTANT,
          a.CSS_NUMBER_MODE,
          a.UNICODE_RANGE,
          ...g,
          // needed to highlight these as strings and to avoid issues with
          // illegal characters that might be inside urls that would trigger the
          // languages illegal stack
          {
            begin: /(url|data-uri)\(/,
            end: /\)/,
            relevance: 0,
            // from keywords
            keywords: { built_in: "url data-uri" },
            contains: [
              ...g,
              {
                className: "string",
                // any character other than `)` as in `url()` will be the start
                // of a string, which ends with `)` (from the parent mode)
                begin: /[^)]/,
                endsWithParent: !0,
                excludeEnd: !0
              }
            ]
          },
          a.FUNCTION_DISPATCH
        ]
      },
      {
        begin: t.lookahead(/@/),
        end: "[{;]",
        relevance: 0,
        illegal: /:/,
        // break on Less variables @var: ...
        contains: [
          {
            className: "keyword",
            begin: c
          },
          {
            begin: /\s/,
            endsWithParent: !0,
            excludeEnd: !0,
            relevance: 0,
            keywords: {
              $pattern: /[a-z-]+/,
              keyword: o,
              attribute: cr.join(" ")
            },
            contains: [
              {
                begin: /[a-z-]+(?=:)/,
                className: "attribute"
              },
              ...g,
              a.CSS_NUMBER_MODE
            ]
          }
        ]
      },
      {
        className: "selector-tag",
        begin: "\\b(" + or.join("|") + ")\\b"
      }
    ]
  };
}
function fr(n) {
  const t = n.regex, a = {
    begin: /<\/?[A-Za-z_]/,
    end: ">",
    subLanguage: "xml",
    relevance: 0
  }, i = { match: /^ {0,3}([-*_])[ \t]*(?:\1[ \t]*){2,}$/ }, o = {
    className: "code",
    variants: [
      // TODO: fix to allow these to work with sublanguage also
      { begin: "(`{3,})[^`](.|\\n)*?\\1`*[ ]*" },
      { begin: "(~{3,})[^~](.|\\n)*?\\1~*[ ]*" },
      // needed to allow markdown as a sublanguage to work
      {
        begin: "```",
        end: "```+[ ]*$"
      },
      {
        begin: "~~~",
        end: "~~~+[ ]*$"
      },
      { begin: "`.+?`" },
      {
        begin: "(?=^( {4}|\\t))",
        // use contains to gobble up multiple lines to allow the block to be whatever size
        // but only have a single open/close tag vs one per line
        contains: [
          {
            begin: "^( {4}|\\t)",
            end: "(\\n)$"
          }
        ],
        relevance: 0
      }
    ]
  }, c = {
    className: "bullet",
    begin: "^[ 	]*([*+-]|(\\d+\\.))(?=\\s+)",
    end: "\\s+",
    excludeEnd: !0
  }, u = {
    begin: /^\[[^\n]+\]:/,
    returnBegin: !0,
    contains: [
      {
        className: "symbol",
        begin: /\[/,
        end: /\]/,
        excludeBegin: !0,
        excludeEnd: !0
      },
      {
        className: "link",
        begin: /:\s*/,
        end: /$/,
        excludeBegin: !0
      }
    ]
  }, g = /[A-Za-z][A-Za-z0-9+.-]*/, _ = {
    variants: [
      // too much like nested array access in so many languages
      // to have any real relevance
      {
        begin: /\[.+?\]\[.*?\]/,
        relevance: 0
      },
      // popular internet URLs
      {
        begin: /\[.+?\]\(((data|javascript|mailto):|(?:http|ftp)s?:\/\/).*?\)/,
        relevance: 2
      },
      {
        begin: t.concat(/\[.+?\]\(/, g, /:\/\/.*?\)/),
        relevance: 2
      },
      // relative urls
      {
        begin: /\[.+?\]\([./?&#].*?\)/,
        relevance: 1
      },
      // whatever else, lower relevance (might not be a link at all)
      {
        begin: /\[.*?\]\(.*?\)/,
        relevance: 0
      }
    ],
    returnBegin: !0,
    contains: [
      {
        // empty strings for alt or link text
        match: /\[(?=\])/
      },
      {
        className: "string",
        relevance: 0,
        begin: "\\[",
        end: "\\]",
        excludeBegin: !0,
        returnEnd: !0
      },
      {
        className: "link",
        relevance: 0,
        begin: "\\]\\(",
        end: "\\)",
        excludeBegin: !0,
        excludeEnd: !0
      },
      {
        className: "symbol",
        relevance: 0,
        begin: "\\]\\[",
        end: "\\]",
        excludeBegin: !0,
        excludeEnd: !0
      }
    ]
  }, h = {
    className: "strong",
    contains: [],
    // defined later
    variants: [
      {
        begin: /_{2}(?!\s)/,
        end: /_{2}/
      },
      {
        begin: /\*{2}(?!\s)/,
        end: /\*{2}/
      }
    ]
  }, A = {
    className: "emphasis",
    contains: [],
    // defined later
    variants: [
      {
        begin: /\*(?![*\s])/,
        end: /\*/
      },
      {
        begin: /_(?![_\s])/,
        end: /_/,
        relevance: 0
      }
    ]
  }, y = n.inherit(h, { contains: [] }), O = n.inherit(A, { contains: [] });
  h.contains.push(O), A.contains.push(y);
  let M = [
    a,
    _
  ];
  return [
    h,
    A,
    y,
    O
  ].forEach(($) => {
    $.contains = $.contains.concat(M);
  }), M = M.concat(h, A), {
    name: "Markdown",
    aliases: [
      "md",
      "mkdown",
      "mkd"
    ],
    contains: [
      {
        className: "section",
        variants: [
          {
            begin: "^#{1,6}",
            end: "$",
            contains: M
          },
          {
            begin: "(?=^.+?\\n[=-]{2,}$)",
            contains: [
              { begin: "^[=-]*$" },
              {
                begin: "^",
                end: "\\n",
                contains: M
              }
            ]
          }
        ]
      },
      a,
      c,
      // must come before BOLD/ITALIC so that a `***` or `___` thematic break
      // isn't mistaken for the start of bold text
      i,
      h,
      A,
      {
        className: "quote",
        begin: "^>\\s+",
        contains: M,
        end: "$"
      },
      o,
      _,
      u,
      {
        //https://spec.commonmark.org/0.31.2/#entity-references
        scope: "literal",
        match: /&([a-zA-Z0-9]+|#[0-9]{1,7}|#[Xx][0-9a-fA-F]{1,6});/
      }
    ]
  };
}
function pr(n) {
  const t = [
    "bool",
    "byte",
    "char",
    "decimal",
    "delegate",
    "double",
    "dynamic",
    "enum",
    "float",
    "int",
    "long",
    "nint",
    "nuint",
    "object",
    "sbyte",
    "short",
    "string",
    "ulong",
    "uint",
    "ushort"
  ], a = [
    "public",
    "private",
    "protected",
    "static",
    "internal",
    "protected",
    "abstract",
    "async",
    "extern",
    "override",
    "unsafe",
    "virtual",
    "new",
    "sealed",
    "partial"
  ], i = [
    "default",
    "false",
    "null",
    "true"
  ], o = [
    "abstract",
    "as",
    "base",
    "break",
    "case",
    "catch",
    "class",
    "const",
    "continue",
    "do",
    "else",
    "event",
    "explicit",
    "extern",
    "finally",
    "fixed",
    "for",
    "foreach",
    "goto",
    "if",
    "implicit",
    "in",
    "interface",
    "internal",
    "is",
    "lock",
    "namespace",
    "new",
    "operator",
    "out",
    "override",
    "params",
    "private",
    "protected",
    "public",
    "readonly",
    "record",
    "ref",
    "return",
    "scoped",
    "sealed",
    "sizeof",
    "stackalloc",
    "static",
    "struct",
    "switch",
    "this",
    "throw",
    "try",
    "typeof",
    "unchecked",
    "unsafe",
    "using",
    "virtual",
    "void",
    "volatile",
    "while"
  ], c = [
    "add",
    "alias",
    "and",
    "ascending",
    "args",
    "async",
    "await",
    "by",
    "descending",
    "dynamic",
    "equals",
    "file",
    "from",
    "get",
    "global",
    "group",
    "init",
    "into",
    "join",
    "let",
    "nameof",
    "not",
    "notnull",
    "on",
    "or",
    "orderby",
    "partial",
    "record",
    "remove",
    "required",
    "scoped",
    "select",
    "set",
    "unmanaged",
    "value|0",
    "var",
    "when",
    "where",
    "with",
    "yield"
  ], u = {
    keyword: o.concat(c),
    built_in: t,
    literal: i
  }, g = n.inherit(n.TITLE_MODE, { begin: "[a-zA-Z](\\.?\\w)*" }), _ = "\\d(_*\\d)*", h = "([uU][lL]?|[lL][uU]?)?", y = {
    className: "number",
    variants: [
      { begin: "\\b0[bB]_*[01](_*[01])*" + h },
      { begin: "(-?)\\b0[xX]_*[a-fA-F0-9](_*[a-fA-F0-9])*" + h },
      { begin: "(-?)(\\b" + _ + "(\\.(" + _ + ")?)?|\\." + _ + ")([eE][-+]?" + _ + ")?" + "([fFdDmM]|[uU][lL]?|[lL][uU]?)?" }
    ],
    relevance: 0
  }, O = {
    className: "string",
    begin: /"""("*)(?!")(.|\n)*?"""\1/,
    relevance: 1
  }, M = {
    className: "string",
    begin: '@"',
    end: '"',
    contains: [{ begin: '""' }]
  }, P = n.inherit(M, { illegal: /\n/ }), v = {
    className: "subst",
    begin: /\{/,
    end: /\}/,
    keywords: u
  }, B = n.inherit(v, { illegal: /\n/ }), $ = {
    className: "string",
    begin: /\$"/,
    end: '"',
    illegal: /\n/,
    contains: [
      { begin: /\{\{/ },
      { begin: /\}\}/ },
      n.BACKSLASH_ESCAPE,
      B
    ]
  }, L = {
    className: "string",
    begin: /\$@"/,
    end: '"',
    contains: [
      { begin: /\{\{/ },
      { begin: /\}\}/ },
      { begin: '""' },
      v
    ]
  }, X = n.inherit(L, {
    illegal: /\n/,
    contains: [
      { begin: /\{\{/ },
      { begin: /\}\}/ },
      { begin: '""' },
      B
    ]
  });
  v.contains = [
    L,
    $,
    M,
    n.APOS_STRING_MODE,
    n.QUOTE_STRING_MODE,
    y,
    n.C_BLOCK_COMMENT_MODE
  ], B.contains = [
    X,
    $,
    P,
    n.APOS_STRING_MODE,
    n.QUOTE_STRING_MODE,
    y,
    n.inherit(n.C_BLOCK_COMMENT_MODE, { illegal: /\n/ })
  ];
  const J = { variants: [
    O,
    L,
    $,
    M,
    n.APOS_STRING_MODE,
    n.QUOTE_STRING_MODE
  ] }, G = {
    begin: "<",
    end: ">",
    contains: [
      { beginKeywords: "in out" },
      g
    ]
  }, Z = n.IDENT_RE + "(<" + n.IDENT_RE + "(\\s*,\\s*" + n.IDENT_RE + ")*>)?(\\[\\])?", se = {
    // prevents expressions like `@class` from incorrect flagging
    // `class` as a keyword
    begin: "@" + n.IDENT_RE,
    relevance: 0
  };
  return {
    name: "C#",
    aliases: [
      "cs",
      "c#"
    ],
    keywords: u,
    illegal: /::/,
    contains: [
      n.COMMENT(
        "///",
        "$",
        {
          returnBegin: !0,
          contains: [
            {
              className: "doctag",
              variants: [
                {
                  begin: "///",
                  relevance: 0
                },
                { begin: "<!--|-->" },
                {
                  begin: "</?",
                  end: ">"
                }
              ]
            }
          ]
        }
      ),
      n.C_LINE_COMMENT_MODE,
      n.C_BLOCK_COMMENT_MODE,
      {
        className: "meta",
        begin: "#",
        end: "$",
        keywords: { keyword: "if else elif endif define undef warning error line region endregion pragma checksum" }
      },
      J,
      y,
      {
        beginKeywords: "class interface",
        relevance: 0,
        end: /[{;=]/,
        illegal: /[^\s:,]/,
        contains: [
          { beginKeywords: "where class" },
          g,
          G,
          n.C_LINE_COMMENT_MODE,
          n.C_BLOCK_COMMENT_MODE
        ]
      },
      {
        beginKeywords: "namespace",
        relevance: 0,
        end: /[{;=]/,
        illegal: /[^\s:]/,
        contains: [
          g,
          n.C_LINE_COMMENT_MODE,
          n.C_BLOCK_COMMENT_MODE
        ]
      },
      {
        beginKeywords: "record",
        relevance: 0,
        end: /[{;=]/,
        illegal: /[^\s:]/,
        contains: [
          g,
          G,
          n.C_LINE_COMMENT_MODE,
          n.C_BLOCK_COMMENT_MODE
        ]
      },
      {
        // [Attributes("")]
        className: "meta",
        begin: "^\\s*\\[(?=[\\w])",
        excludeBegin: !0,
        end: "\\]",
        excludeEnd: !0,
        contains: [
          {
            className: "string",
            begin: /"/,
            end: /"/
          }
        ]
      },
      {
        // Expression keywords prevent 'keyword Name(...)' from being
        // recognized as a function definition
        beginKeywords: "new return throw await else",
        relevance: 0
      },
      {
        className: "function",
        begin: "(" + Z + "\\s+)+" + n.IDENT_RE + "\\s*(<[^=]+>\\s*)?\\(",
        returnBegin: !0,
        end: /\s*[{;=]/,
        excludeEnd: !0,
        keywords: u,
        contains: [
          // prevents these from being highlighted `title`
          {
            beginKeywords: a.join(" "),
            relevance: 0
          },
          {
            begin: n.IDENT_RE + "\\s*(<[^=]+>\\s*)?\\(",
            returnBegin: !0,
            contains: [
              n.TITLE_MODE,
              G
            ],
            relevance: 0
          },
          { match: /\(\)/ },
          {
            className: "params",
            begin: /\(/,
            end: /\)/,
            excludeBegin: !0,
            excludeEnd: !0,
            keywords: u,
            relevance: 0,
            contains: [
              J,
              y,
              n.C_BLOCK_COMMENT_MODE
            ]
          },
          n.C_LINE_COMMENT_MODE,
          n.C_BLOCK_COMMENT_MODE
        ]
      },
      se
    ]
  };
}
function br(n) {
  const t = n.regex, a = {}, i = {
    begin: /\$\{/,
    end: /\}/,
    contains: [
      "self",
      {
        begin: /:-/,
        contains: [a]
      }
      // default values
    ]
  };
  Object.assign(a, {
    className: "variable",
    variants: [
      { begin: t.concat(
        /\$[\w\d#@][\w\d_]*/,
        // negative look-ahead tries to avoid matching patterns that are not
        // Perl at all like $ident$, @ident@, etc.
        "(?![\\w\\d])(?![$])"
      ) },
      i
    ]
  });
  const o = {
    className: "subst",
    begin: /\$\(/,
    end: /\)/,
    contains: [n.BACKSLASH_ESCAPE]
  }, c = n.inherit(
    n.COMMENT(),
    {
      match: [
        /(^|\s)/,
        /#.*$/
      ],
      scope: {
        2: "comment"
      }
    }
  ), u = {
    begin: /<<-?\s*(?=\w+)/,
    starts: { contains: [
      n.END_SAME_AS_BEGIN({
        begin: /(\w+)/,
        end: /(\w+)/,
        className: "string"
      })
    ] }
  }, g = {
    className: "string",
    begin: /"/,
    end: /"/,
    contains: [
      n.BACKSLASH_ESCAPE,
      a,
      o
    ]
  };
  o.contains.push(g);
  const _ = {
    match: /\\"/
  }, h = {
    className: "string",
    begin: /'/,
    end: /'/
  }, A = {
    match: /\\'/
  }, y = {
    begin: /\$?\(\(/,
    end: /\)\)/,
    contains: [
      {
        begin: /\d+#[0-9a-f]+/,
        className: "number"
      },
      n.NUMBER_MODE,
      a
    ]
  }, O = [
    "fish",
    "bash",
    "zsh",
    "sh",
    "csh",
    "ksh",
    "tcsh",
    "dash",
    "scsh"
  ], M = n.SHEBANG({
    binary: `(${O.join("|")})`,
    relevance: 10
  }), P = {
    className: "function",
    begin: /\w[\w\d_]*\s*\(\s*\)\s*\{/,
    returnBegin: !0,
    contains: [n.inherit(n.TITLE_MODE, { begin: /\w[\w\d_]*/ })],
    relevance: 0
  }, v = [
    "if",
    "then",
    "else",
    "elif",
    "fi",
    "time",
    "for",
    "while",
    "until",
    "in",
    "do",
    "done",
    "case",
    "esac",
    "coproc",
    "function",
    "select"
  ], B = [
    "true",
    "false"
  ], $ = { match: /(\/[a-z._-]+)+/ }, L = [
    "break",
    "cd",
    "continue",
    "eval",
    "exec",
    "exit",
    "export",
    "getopts",
    "hash",
    "pwd",
    "readonly",
    "return",
    "shift",
    "test",
    "times",
    "trap",
    "umask",
    "unset"
  ], X = [
    "alias",
    "bind",
    "builtin",
    "caller",
    "command",
    "declare",
    "echo",
    "enable",
    "help",
    "let",
    "local",
    "logout",
    "mapfile",
    "printf",
    "read",
    "readarray",
    "source",
    "sudo",
    "type",
    "typeset",
    "ulimit",
    "unalias"
  ], J = [
    "autoload",
    "bg",
    "bindkey",
    "bye",
    "cap",
    "chdir",
    "clone",
    "comparguments",
    "compcall",
    "compctl",
    "compdescribe",
    "compfiles",
    "compgroups",
    "compquote",
    "comptags",
    "comptry",
    "compvalues",
    "dirs",
    "disable",
    "disown",
    "echotc",
    "echoti",
    "emulate",
    "fc",
    "fg",
    "float",
    "functions",
    "getcap",
    "getln",
    "history",
    "integer",
    "jobs",
    "kill",
    "limit",
    "log",
    "noglob",
    "popd",
    "print",
    "pushd",
    "pushln",
    "rehash",
    "sched",
    "setcap",
    "setopt",
    "stat",
    "suspend",
    "ttyctl",
    "unfunction",
    "unhash",
    "unlimit",
    "unsetopt",
    "vared",
    "wait",
    "whence",
    "where",
    "which",
    "zcompile",
    "zformat",
    "zftp",
    "zle",
    "zmodload",
    "zparseopts",
    "zprof",
    "zpty",
    "zregexparse",
    "zsocket",
    "zstyle",
    "ztcp"
  ], G = [
    "chcon",
    "chgrp",
    "chown",
    "chmod",
    "cp",
    "dd",
    "df",
    "dir",
    "dircolors",
    "ln",
    "ls",
    "mkdir",
    "mkfifo",
    "mknod",
    "mktemp",
    "mv",
    "realpath",
    "rm",
    "rmdir",
    "shred",
    "sync",
    "touch",
    "truncate",
    "vdir",
    "b2sum",
    "base32",
    "base64",
    "cat",
    "cksum",
    "comm",
    "csplit",
    "cut",
    "expand",
    "fmt",
    "fold",
    "head",
    "join",
    "md5sum",
    "nl",
    "numfmt",
    "od",
    "paste",
    "ptx",
    "pr",
    "sha1sum",
    "sha224sum",
    "sha256sum",
    "sha384sum",
    "sha512sum",
    "shuf",
    "sort",
    "split",
    "sum",
    "tac",
    "tail",
    "tr",
    "tsort",
    "unexpand",
    "uniq",
    "wc",
    "arch",
    "basename",
    "chroot",
    "date",
    "dirname",
    "du",
    "echo",
    "env",
    "expr",
    "factor",
    // "false", // keyword literal already
    "groups",
    "hostid",
    "id",
    "link",
    "logname",
    "nice",
    "nohup",
    "nproc",
    "pathchk",
    "pinky",
    "printenv",
    "printf",
    "pwd",
    "readlink",
    "runcon",
    "seq",
    "sleep",
    "stat",
    "stdbuf",
    "stty",
    "tee",
    "test",
    "timeout",
    // "true", // keyword literal already
    "tty",
    "uname",
    "unlink",
    "uptime",
    "users",
    "who",
    "whoami",
    "yes"
  ];
  return {
    name: "Bash",
    aliases: [
      "sh",
      "zsh"
    ],
    keywords: {
      $pattern: /\b[a-z][a-z0-9._-]+\b/,
      keyword: v,
      literal: B,
      built_in: [
        ...L,
        ...X,
        // Shell modifiers
        "set",
        "shopt",
        ...J,
        ...G
      ]
    },
    contains: [
      M,
      // to catch known shells and boost relevancy
      n.SHEBANG(),
      // to catch unknown shells but still highlight the shebang
      P,
      y,
      c,
      u,
      $,
      g,
      _,
      h,
      A,
      a
    ]
  };
}
ne.registerLanguage("javascript", Vt);
ne.registerLanguage("typescript", er);
ne.registerLanguage("xml", Rn);
ne.registerLanguage("html", Rn);
ne.registerLanguage("json", rr);
ne.registerLanguage("css", gr);
ne.registerLanguage("markdown", fr);
ne.registerLanguage("csharp", pr);
ne.registerLanguage("bash", br);
const kn = wt(), Er = new vt({
  extensions: kn,
  markedOptions: {
    gfm: !0,
    breaks: !1
  }
});
async function Tr(n, t, a, i) {
  const o = (a ?? "markdown").toLowerCase(), c = i?.direction === "rtl" ? "rtl" : "ltr";
  let u;
  if (o === "html")
    u = t ?? "";
  else {
    const g = o === "json" ? JSON.parse(t || '{"type":"doc","content":[]}') : Er.parse(t ?? "");
    u = Kt({
      extensions: kn,
      content: g,
      staticEditorOptions: { textDirection: c }
    });
  }
  n.innerHTML = u, i?.enableHighlight !== !1 && n.querySelectorAll("pre code").forEach((g) => {
    ne.highlightElement(g);
  }), i?.enableMermaid !== !1 && n.querySelector("code.language-mermaid, .language-mermaid") && await hr(n);
}
async function hr(n) {
  const t = Array.from(n.querySelectorAll("code.language-mermaid, .language-mermaid"));
  if (t.length === 0)
    return;
  const a = await import("./mermaid.min-B-MeBWhb.js").then((o) => o.m);
  a.default.initialize({ startOnLoad: !1, securityLevel: "strict" });
  let i = 0;
  for (const o of t) {
    const c = o.textContent ?? "", u = document.createElement("div");
    u.className = "sb-mermaid";
    const g = `sb-mermaid-${i++}`;
    try {
      const { svg: _ } = await a.default.render(g, c);
      u.innerHTML = _, (o.closest("pre") ?? o).replaceWith(u);
    } catch {
    }
  }
}
export {
  Nr as c,
  Zt as g,
  Tr as r
};
