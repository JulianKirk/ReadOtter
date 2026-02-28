window.InputHandler = {
    _dotNetRef: null,
    _listener: null,

    register: function (dotNetRef, shiftRepeatKeys) {
        this.unregister();
        this._dotNetRef = dotNetRef;
        var allowedRepeat = new Set(shiftRepeatKeys || []);
        this._listener = (event) => {
            if (event.repeat) {
                if (!event.shiftKey || !allowedRepeat.has(event.key)) return;
            }
            dotNetRef.invokeMethodAsync('HandleKeyDown', event.key);
        };
        window.addEventListener('keydown', this._listener);
    },

    unregister: function () {
        if (this._listener) {
            window.removeEventListener('keydown', this._listener);
            this._listener = null;
        }
        this._dotNetRef = null;
    }
};

window.clickElement = (element) => {
    element.click();
};

window.LinkHandler = {
    _dotNetRef: null,
    _listener: null,

    register: function (dotNetRef, containerSelector) {
        this.unregister(containerSelector);
        this._dotNetRef = dotNetRef;
        this._listener = (event) => {
            var anchor = event.target.closest('a[href]');
            if (!anchor) return;
            var href = anchor.getAttribute('href');
            if (href && !href.startsWith('http://') && !href.startsWith('https://')) {
                event.preventDefault();
                dotNetRef.invokeMethodAsync('HandleLinkClick', href);
            }
        };
        var container = document.querySelector(containerSelector);
        if (container) container.addEventListener('click', this._listener);
    },

    unregister: function (containerSelector) {
        var container = document.querySelector(containerSelector);
        if (container && this._listener) {
            container.removeEventListener('click', this._listener);
        }
        this._listener = null;
        this._dotNetRef = null;
    }
};