window.InputHandler = {
    _dotNetRef: null,
    _listener: null,

    register: function (dotNetRef) {
        this._dotNetRef = dotNetRef;
        this._listener = (event) => {
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