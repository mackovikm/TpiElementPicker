// TPI Element Picker - skript vkladany do stranky pres
// CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync.
// Zajistuje: zvyrazneni prvku pod mysi, zachyceni kliknuti (bez akce stranky),
// serializaci DOM do stromu a odeslani do hostitelske aplikace.
(function () {
    if (window.__tpi) { return; }

    var SKIP = { SCRIPT: 1, STYLE: 1, NOSCRIPT: 1, LINK: 1, META: 1, TEMPLATE: 1 };
    var nodes = [];
    var pickMode = false;
    var overlay = null;
    var lastHover = null;

    function post(msg) {
        try {
            if (window.chrome && window.chrome.webview) {
                window.chrome.webview.postMessage(JSON.stringify(msg));
            }
        } catch (e) { /* ignorovat */ }
    }

    function ensureOverlay() {
        if (overlay && overlay.parentNode) { return overlay; }
        overlay = document.createElement('div');
        overlay.setAttribute('data-tpi-overlay', '1');
        overlay.style.cssText = [
            'position:fixed', 'pointer-events:none', 'z-index:2147483647',
            'border:2px solid #0062cc', 'background:rgba(0,98,204,0.12)',
            'border-radius:2px', 'display:none', 'box-sizing:border-box'
        ].join(';');
        (document.body || document.documentElement).appendChild(overlay);
        return overlay;
    }

    function showOverlay(el) {
        if (!el || !el.getBoundingClientRect) { return; }
        var r = el.getBoundingClientRect();
        var o = ensureOverlay();
        o.style.left = r.left + 'px';
        o.style.top = r.top + 'px';
        o.style.width = r.width + 'px';
        o.style.height = r.height + 'px';
        o.style.display = 'block';
    }

    function hideOverlay() {
        if (overlay) { overlay.style.display = 'none'; }
    }

    function attrs(el) {
        var out = {};
        if (!el.attributes) { return out; }
        for (var i = 0; i < el.attributes.length; i++) {
            var a = el.attributes[i];
            if (a.name === 'data-tpi-overlay') { continue; }
            out[a.name] = a.value;
        }
        return out;
    }

    function ownText(el) {
        var text = '';
        for (var i = 0; i < el.childNodes.length; i++) {
            var n = el.childNodes[i];
            if (n.nodeType === 3) { text += n.nodeValue; }
        }
        text = (text || '').replace(/\s+/g, ' ').trim();
        if (!text && el.tagName === 'INPUT') { text = el.value || ''; }
        return text.length > 120 ? text.substring(0, 120) + '…' : text;
    }

    function cssPath(el) {
        if (!el || el.nodeType !== 1) { return ''; }
        if (el.id) { return '#' + CSS.escape(el.id); }
        var parts = [];
        var cur = el;
        while (cur && cur.nodeType === 1 && parts.length < 12) {
            var sel = cur.tagName.toLowerCase();
            if (cur.id) { parts.unshift('#' + CSS.escape(cur.id)); break; }
            var parent = cur.parentNode;
            if (parent && parent.children) {
                var same = [];
                for (var i = 0; i < parent.children.length; i++) {
                    if (parent.children[i].tagName === cur.tagName) { same.push(parent.children[i]); }
                }
                if (same.length > 1) { sel += ':nth-of-type(' + (same.indexOf(cur) + 1) + ')'; }
            }
            parts.unshift(sel);
            cur = cur.parentElement;
        }
        return parts.join(' > ');
    }

    function xpath(el) {
        if (!el || el.nodeType !== 1) { return ''; }
        var parts = [];
        var cur = el;
        while (cur && cur.nodeType === 1) {
            var index = 1;
            var sib = cur.previousElementSibling;
            while (sib) {
                if (sib.tagName === cur.tagName) { index++; }
                sib = sib.previousElementSibling;
            }
            parts.unshift(cur.tagName.toLowerCase() + '[' + index + ']');
            cur = cur.parentElement;
        }
        return '/' + parts.join('/');
    }

    function describe(el, withPaths) {
        var d = {
            idx: (typeof el.__tpiIdx === 'number') ? el.__tpiIdx : -1,
            tag: el.tagName ? el.tagName.toLowerCase() : '',
            id: el.id || '',
            name: el.getAttribute ? (el.getAttribute('name') || '') : '',
            inputType: (el.tagName === 'INPUT' && el.getAttribute) ? (el.getAttribute('type') || 'text') : '',
            role: el.getAttribute ? (el.getAttribute('role') || '') : '',
            cls: el.className && typeof el.className === 'string'
                ? el.className.split(/\s+/).filter(function (c) { return !!c; }) : [],
            text: ownText(el),
            attrs: attrs(el)
        };
        if (withPaths) {
            d.css = cssPath(el);
            d.xpath = xpath(el);
        }
        return d;
    }

    function buildNode(el) {
        el.__tpiIdx = nodes.length;
        nodes.push(el);

        var node = describe(el, false);
        node.children = [];

        for (var i = 0; i < el.children.length; i++) {
            var child = el.children[i];
            if (SKIP[child.tagName]) { continue; }
            if (child.getAttribute && child.getAttribute('data-tpi-overlay')) { continue; }
            node.children.push(buildNode(child));
        }
        return node;
    }

    function buildTree() {
        nodes = [];
        var root = document.documentElement;
        return buildNode(root);
    }

    function ancestorsOf(el) {
        var list = [];
        var cur = el.parentElement;
        var depth = 0;
        while (cur && depth < 12) {
            list.push(describe(cur, false));
            cur = cur.parentElement;
            depth++;
        }
        return list;
    }

    function onMouseOver(e) {
        if (!pickMode) { return; }
        lastHover = e.target;
        showOverlay(e.target);
    }

    function onClick(e) {
        if (!pickMode) { return; }
        e.preventDefault();
        e.stopPropagation();

        var el = e.target;
        if (typeof el.__tpiIdx !== 'number') { buildTree(); }

        post({
            type: 'pick',
            url: location.href,
            title: document.title,
            node: describe(el, true),
            ancestors: ancestorsOf(el)
        });
        return false;
    }

    function onKeyDown(e) {
        if (pickMode && e.key === 'Escape') {
            setPick(false);
            post({ type: 'pickmode', on: false });
        }
    }

    function setPick(on) {
        pickMode = !!on;
        if (!pickMode) { hideOverlay(); }
        document.documentElement.style.cursor = pickMode ? 'crosshair' : '';
    }

    function sendTree() {
        var root = buildTree();
        post({ type: 'tree', url: location.href, title: document.title, root: root, count: nodes.length });
    }

    function highlight(idx) {
        var el = nodes[idx];
        if (!el) { return; }
        showOverlay(el);
        try { el.scrollIntoView({ block: 'center', inline: 'center' }); } catch (e) { }
        showOverlay(el);
        setTimeout(function () { if (!pickMode) { hideOverlay(); } }, 2500);
    }

    function describeIndex(idx) {
        var el = nodes[idx];
        if (!el) { return; }
        post({
            type: 'pick',
            url: location.href,
            title: document.title,
            node: describe(el, true),
            ancestors: ancestorsOf(el)
        });
    }

    function fillLogin(userSelector, user, passSelector, pass, submitSelector) {
        try {
            var u = document.querySelector(userSelector);
            if (u) { u.value = user; u.dispatchEvent(new Event('input', { bubbles: true })); }
            var p = document.querySelector(passSelector);
            if (p) { p.value = pass; p.dispatchEvent(new Event('input', { bubbles: true })); }
            if (submitSelector) {
                var s = document.querySelector(submitSelector);
                if (s) { s.click(); }
            }
            return true;
        } catch (e) { return false; }
    }

    document.addEventListener('mouseover', onMouseOver, true);
    document.addEventListener('click', onClick, true);
    document.addEventListener('mousedown', function (e) { if (pickMode) { e.preventDefault(); e.stopPropagation(); } }, true);
    document.addEventListener('keydown', onKeyDown, true);
    window.addEventListener('scroll', function () { if (pickMode && lastHover) { showOverlay(lastHover); } }, true);

    window.__tpi = {
        setPick: setPick,
        sendTree: sendTree,
        highlight: highlight,
        describeIndex: describeIndex,
        fillLogin: fillLogin,
        version: 1
    };

    if (document.readyState === 'complete' || document.readyState === 'interactive') {
        post({ type: 'ready', url: location.href, title: document.title });
    } else {
        document.addEventListener('DOMContentLoaded', function () {
            post({ type: 'ready', url: location.href, title: document.title });
        });
    }
})();
