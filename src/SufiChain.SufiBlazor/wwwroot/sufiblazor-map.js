/**
 * SufiBlazor Map JavaScript Module
 * Built on Leaflet + OpenStreetMap tiles.
 *
 * Loaded on-demand when SbMap / map helpers are used.
 */

const maps = new Map();
let mapIdCounter = 0;
let leafletLoaded = false;
let leafletLoadPromise = null;

const LEAFLET_CDN_JS = 'https://unpkg.com/leaflet@1.9.4/dist/leaflet.js';
const LEAFLET_CDN_CSS = 'https://unpkg.com/leaflet@1.9.4/dist/leaflet.css';

function getContentBasePath() {
    const scripts = document.querySelectorAll('script[src*="sufiblazor-map.js"]');
    if (scripts.length > 0) {
        const src = scripts[0].src;
        return src.substring(0, src.lastIndexOf('/'));
    }
    return '_content/SufiChain.SufiBlazor';
}

function loadScript(src) {
    return new Promise((resolve) => {
        if (document.querySelector(`script[src="${src}"]`)) {
            resolve(true);
            return;
        }
        const script = document.createElement('script');
        script.src = src;
        script.onload = () => resolve(true);
        script.onerror = () => resolve(false);
        document.head.appendChild(script);
    });
}

function loadCss(href) {
    if (!document.querySelector(`link[href="${href}"]`)) {
        const link = document.createElement('link');
        link.rel = 'stylesheet';
        link.href = href;
        document.head.appendChild(link);
    }
}

async function ensureLeafletLoaded() {
    if (typeof L !== 'undefined') {
        leafletLoaded = true;
        return true;
    }

    if (leafletLoadPromise) {
        return leafletLoadPromise;
    }

    leafletLoadPromise = (async () => {
        const basePath = getContentBasePath();
        const bundledJs = `${basePath}/vendor/leaflet/leaflet.js`;
        const bundledCss = `${basePath}/vendor/leaflet/leaflet.css`;

        loadCss(bundledCss);
        let loaded = await loadScript(bundledJs);

        if (!loaded || typeof L === 'undefined') {
            console.log('SufiBlazor: Loading Leaflet from CDN...');
            loadCss(LEAFLET_CDN_CSS);
            loaded = await loadScript(LEAFLET_CDN_JS);
        }

        leafletLoaded = loaded && typeof L !== 'undefined';
        if (leafletLoaded) {
            // Fix default icon paths when CSS is served from static content
            const iconBase = `${basePath}/vendor/leaflet/images/`;
            try {
                delete L.Icon.Default.prototype._getIconUrl;
                L.Icon.Default.mergeOptions({
                    iconRetinaUrl: `${iconBase}marker-icon-2x.png`,
                    iconUrl: `${iconBase}marker-icon.png`,
                    shadowUrl: `${iconBase}marker-shadow.png`
                });
            } catch {
                // ignore icon path setup failures; markers still work with CDN defaults
            }
        }

        return leafletLoaded;
    })();

    return leafletLoadPromise;
}

function getEntry(mapId) {
    return maps.get(mapId);
}

/**
 * Initialize a Leaflet map on the given element.
 * @returns {Promise<string>} mapId
 */
export async function initMap(element, dotNetRef, options) {
    const ok = await ensureLeafletLoaded();
    if (!ok) {
        throw new Error('Failed to load Leaflet');
    }

    const mapId = `sb-map-${++mapIdCounter}`;
    const opts = options || {};

    const map = L.map(element, {
        zoomControl: opts.interactive !== false,
        dragging: opts.interactive !== false,
        scrollWheelZoom: opts.interactive !== false,
        doubleClickZoom: opts.interactive !== false,
        boxZoom: opts.interactive !== false,
        keyboard: opts.interactive !== false,
        attributionControl: opts.showAttribution !== false
    }).setView([opts.latitude ?? 0, opts.longitude ?? 0], opts.zoom ?? 2);

    if (opts.minZoom != null) {
        map.setMinZoom(opts.minZoom);
    }
    if (opts.maxZoom != null) {
        map.setMaxZoom(opts.maxZoom);
    }

    const tileUrl = opts.tileUrl || 'https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png';
    const attribution = opts.tileAttribution ||
        '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>';

    L.tileLayer(tileUrl, {
        attribution,
        maxZoom: opts.maxZoom ?? 19
    }).addTo(map);

    map.on('click', (e) => {
        if (dotNetRef) {
            dotNetRef.invokeMethodAsync('OnMapClick', e.latlng.lat, e.latlng.lng);
        }
    });

    map.on('moveend', () => {
        if (dotNetRef) {
            const c = map.getCenter();
            dotNetRef.invokeMethodAsync('OnMapMoveEnd', c.lat, c.lng, map.getZoom());
        }
    });

    maps.set(mapId, {
        map,
        markers: new Map(),
        circles: new Map(),
        interactive: opts.interactive !== false
    });

    // Leaflet needs a size invalidate after dialog/layout transitions
    setTimeout(() => {
        try {
            map.invalidateSize();
        } catch {
            // ignore
        }
    }, 50);

    return mapId;
}

export async function destroyMap(mapId) {
    const entry = getEntry(mapId);
    if (!entry) {
        return;
    }

    entry.markers.forEach((m) => {
        try {
            entry.map.removeLayer(m);
        } catch {
            // ignore
        }
    });
    entry.circles.forEach((c) => {
        try {
            entry.map.removeLayer(c);
        } catch {
            // ignore
        }
    });
    entry.map.remove();
    maps.delete(mapId);
}

export async function setView(mapId, latitude, longitude, zoom) {
    const entry = getEntry(mapId);
    if (!entry) {
        return;
    }
    if (zoom != null) {
        entry.map.setView([latitude, longitude], zoom);
    } else {
        entry.map.setView([latitude, longitude]);
    }
}

export async function setInteractive(mapId, interactive) {
    const entry = getEntry(mapId);
    if (!entry) {
        return;
    }

    entry.interactive = !!interactive;
    const map = entry.map;
    if (interactive) {
        map.dragging.enable();
        map.scrollWheelZoom.enable();
        map.doubleClickZoom.enable();
        map.boxZoom.enable();
        map.keyboard.enable();
        if (map.zoomControl) {
            map.zoomControl.addTo(map);
        }
    } else {
        map.dragging.disable();
        map.scrollWheelZoom.disable();
        map.doubleClickZoom.disable();
        map.boxZoom.disable();
        map.keyboard.disable();
        if (map.zoomControl) {
            map.removeControl(map.zoomControl);
        }
    }
}

export async function invalidateSize(mapId) {
    const entry = getEntry(mapId);
    if (!entry) {
        return;
    }
    entry.map.invalidateSize();
}

function syncAccuracyCircle(entry, markerId, options) {
    const existing = entry.circles.get(markerId);
    if (existing) {
        entry.map.removeLayer(existing);
        entry.circles.delete(markerId);
    }

    if (options.accuracyMeters != null && options.accuracyMeters > 0) {
        const circle = L.circle([options.latitude, options.longitude], {
            radius: options.accuracyMeters,
            color: options.color || '#3388ff',
            fillColor: options.color || '#3388ff',
            fillOpacity: 0.15,
            weight: 1
        }).addTo(entry.map);
        entry.circles.set(markerId, circle);
    }
}

export async function addMarker(mapId, markerId, options, dotNetRef) {
    const entry = getEntry(mapId);
    if (!entry) {
        return;
    }

    const opts = options || {};
    const marker = L.marker([opts.latitude, opts.longitude], {
        draggable: !!opts.draggable,
        title: opts.title || ''
    }).addTo(entry.map);

    if (opts.title) {
        marker.bindTooltip(opts.title);
    }

    marker.on('click', () => {
        if (dotNetRef) {
            dotNetRef.invokeMethodAsync('OnMarkerClick', markerId);
        }
    });

    marker.on('dragend', () => {
        const pos = marker.getLatLng();
        syncAccuracyCircle(entry, markerId, {
            latitude: pos.lat,
            longitude: pos.lng,
            accuracyMeters: opts.accuracyMeters,
            color: opts.color
        });
        if (dotNetRef) {
            dotNetRef.invokeMethodAsync('OnMarkerDragEnd', markerId, pos.lat, pos.lng);
        }
    });

    entry.markers.set(markerId, marker);
    syncAccuracyCircle(entry, markerId, opts);
}

export async function updateMarker(mapId, markerId, options) {
    const entry = getEntry(mapId);
    if (!entry) {
        return;
    }

    const marker = entry.markers.get(markerId);
    if (!marker) {
        return;
    }

    const opts = options || {};
    marker.setLatLng([opts.latitude, opts.longitude]);
    marker.options.draggable = !!opts.draggable;
    if (opts.draggable) {
        marker.dragging?.enable();
    } else {
        marker.dragging?.disable();
    }

    if (opts.title) {
        marker.bindTooltip(opts.title);
        marker.options.title = opts.title;
    }

    syncAccuracyCircle(entry, markerId, opts);
}

export async function removeMarker(mapId, markerId) {
    const entry = getEntry(mapId);
    if (!entry) {
        return;
    }

    const marker = entry.markers.get(markerId);
    if (marker) {
        entry.map.removeLayer(marker);
        entry.markers.delete(markerId);
    }

    const circle = entry.circles.get(markerId);
    if (circle) {
        entry.map.removeLayer(circle);
        entry.circles.delete(markerId);
    }
}

/**
 * Browser geolocation helper.
 * @returns {Promise<{latitude:number, longitude:number, accuracyMeters?:number}|{error:true, code:number, message:string}>}
 */
export async function getCurrentPosition(options) {
    const opts = options || {};
    if (!navigator.geolocation) {
        return { error: true, code: 2, message: 'Geolocation is not supported by this browser.' };
    }

    return new Promise((resolve) => {
        navigator.geolocation.getCurrentPosition(
            (pos) => {
                resolve({
                    latitude: pos.coords.latitude,
                    longitude: pos.coords.longitude,
                    accuracyMeters: pos.coords.accuracy
                });
            },
            (err) => {
                resolve({
                    error: true,
                    code: err.code,
                    message: err.message || 'Geolocation failed.'
                });
            },
            {
                enableHighAccuracy: opts.enableHighAccuracy !== false,
                timeout: opts.timeoutMs ?? 10000,
                maximumAge: opts.maximumAgeMs ?? 0
            }
        );
    });
}

/**
 * Nominatim search. Prefer routing through a host proxy in production.
 * @returns {Promise<Array>}
 */
export async function searchNominatim(query, options) {
    const opts = options || {};
    const base = (opts.baseUrl || 'https://nominatim.openstreetmap.org').replace(/\/$/, '');
    const params = new URLSearchParams({
        q: query,
        format: 'json',
        addressdetails: '0',
        limit: String(opts.limit ?? 8)
    });

    if (opts.language) {
        params.set('accept-language', opts.language);
    }
    if (opts.countryCodes) {
        params.set('countrycodes', opts.countryCodes);
    }

    const response = await fetch(`${base}/search?${params.toString()}`, {
        headers: {
            Accept: 'application/json'
        }
    });

    if (!response.ok) {
        throw new Error(`Nominatim search failed: ${response.status}`);
    }

    const data = await response.json();
    return (data || []).map((item) => ({
        displayName: item.display_name,
        latitude: parseFloat(item.lat),
        longitude: parseFloat(item.lon),
        boundingBox: item.boundingbox
            ? item.boundingbox.map((v) => parseFloat(v))
            : null,
        osmType: item.osm_type,
        osmId: item.osm_id != null ? Number(item.osm_id) : null
    }));
}
